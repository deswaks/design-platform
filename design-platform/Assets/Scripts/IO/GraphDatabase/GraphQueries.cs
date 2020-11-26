using System;
using System.Linq;
using System.Collections.Generic;
using Neo4jClient;
using Neo4jClient.Extension.Cypher;
using Neo4jClient.Extensions;
using Neo4jClient.Extension.Cypher.Attributes;

using UnitsNet;
using Neo4j.Driver.V1;
using Neo4j.Driver;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using DesignPlatform.Core;


//using System.Numerics;


namespace DesignPlatform.Database {
    public class GraphProgram {

        public static void WorkIt() {
            var db = new GraphDatabase();
            // Sets up the data model, defining how the C# classes should be handled as Nodes in the graph
            NeoConfig.ConfigureDataModel();

            //db.DeleteAllNodesWithLabel("Space");

            //db.PushAllUnitySpacesToGraph();


            //db.GetAllGraphEntities();

            LocalDatabase.CreateAllUnitySpacesFromJson(@"C:\Users\Administrator\Desktop\SpaceNodes.json");
            // LOAD AND BUILD ALL ROOMS FROM GRAPH
            //foreach(int id in db.GetAllSpaceIdsInGraph()) {
            //    SpaceNode SpaceNode = db.GetSpaceById(id);
            //GraphUtils.CreateSpaceFromGraph(SpaceNode);
            //}


            Debug.Log("Graph stuff done!");

        }
    }

    public class GraphDatabase {

        private static GraphDatabase instance;
        private readonly BoltGraphClient _graphClient;

        public static GraphDatabase Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new GraphDatabase()); }
        }

        /// <summary>
        /// Establishes connection to Neo4j graph database at "bolt://localhost:7687" using no credentials.
        /// </summary>
        public GraphDatabase() {
            //First create a 'Driver' instance.
            var driver = Neo4j.Driver.V1.GraphDatabase.Driver("bolt://localhost:7687", Config.Builder.WithEncryptionLevel(EncryptionLevel.None).ToConfig());

            // Attach it to a bolt graph client
            _graphClient = new BoltGraphClient(driver);

            // Connect to the client
            _graphClient.Connect();

        }


        public void LoadAndBuildUnitySpacesFromGraph() {
            // LOAD AND BUILD ALL ROOMS FROM GRAPH
            foreach (int id in GetAllSpaceIdsInGraph()) {
                SpaceNode SpaceNode = GetSpaceNodeById(id);
                LocalDatabase.CreateUnitySpaceFromSpaceNode(SpaceNode);
            }
        }

        /// <summary>
        /// Reads SpaceNodes specified in the json file and merges them into graph
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        public void ImportJsonSpaceNodesToGraph(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : Settings.SaveFolderPath+"SpaceNodes.json";

            IEnumerable<SpaceNode> spaceNodes = LocalDatabase.LoadSpaceNodesFromJson(jsonPath);

            foreach (SpaceNode spaceNode in spaceNodes) {
                CreateOrMergeSpaceNode(spaceNode);
            }

            #region oldCode
            // TANKEN MÅ VÆRE FØRST AT LOADE FRA JSON TIL ROOMNODES OG SÅ DEREFTER SKRIVE ROOMNODES TIL GRAF

            //CALL apoc.load.json('complete-db.json') YIELD value
            //UNWIND value.items AS item
            //CREATE(i: Item(name: item.name, id: item.id)
            //_graphClient.Cypher
            //    .Call("apoc.load.json(\"file:///C:/Users/Administrator/Desktop/FromUnity.json \")")
            //    .Yield("value AS space")
            //    .Merge("(p:Space {name: value.name, id: value.id, vertices: value.vertices, shape: value.shape, type: value.type})")
            //    .ExecuteWithoutResults();

            ////CALL apoc.load.json("file:///C:/Users/Administrator/Desktop/FromUnity.json")
            ////YIELD value
            ////MERGE (p:Space {name: value.name, id: value.id, vertices: value.vertices, shape: value.shape, type: value.type})

            //_graphClient.Cypher
            //    .MergeEntity(space, "space")
            //    .ExecuteWithoutResults();
            #endregion
        }
        public List<SpaceNode> GetAllSpaceNodes() {

            List<SpaceNode> SpaceNodes = _graphClient.Cypher
                .Match("(space:Space)")
                .Return(space => space.As<SpaceNode>())
                .Results
                .ToList();

            return SpaceNodes;

            #region old_code
            ////////////////// VIRKER SEMI //////////////////////////////////////

            // DENNE METODE VIRKER
            //var query = _graphClient.Cypher
            //        .Match("(p:Space)")
            //        .Return(p => new { 
            //            ID = p.As<Space>().id,
            //            V = p.As<Space>().vertices 
            //        });

            //var results = query.Results.ToList();

            // DENNE METODE VIRKER OG RETURNERER NYE RUM MED EGENSKABER FRA GRAF
            //var query2 = _graphClient.Cypher
            //    .Match("(p:Space)")
            //    .Return(p => 
            //    new Space { 
            //        id = p.As<Space>().id,
            //        name = p.As<Space>().name,
            //        type = p.As<Space>().type,
            //        area = p.As<Space>().area,
            //        vertices = p.As<Space>().vertices,

            //});
            //var results2 = query2.Results.ToList();

            //foreach (var result in results)
            //{
            //    result.N.Properties.Keys.ToList().ForEach(k => Console.WriteLine(k));
            //    //nNodes.Add(JsonConvert.DeserializeObject(result.N.Properties) );
            //}

            //.Match("( space:Space)")
            //.Return(space => space.As<Space>())
            //.Results
            //.ToList();
            #endregion
        }

        /// <summary>
        /// Finds and returns list of IDs for all spaces in graph
        /// </summary>
        /// <returns>List of IDs of all spaces in graph</returns>
        public List<int> GetAllSpaceIdsInGraph() {

            var query = _graphClient.Cypher
                        .Match("(p:Space)")
                        .Return(p =>
                            p.As<SpaceNode>().id
            );
            var results = query.Results.ToList();

            return results.Select(r => Convert.ToInt32(r)).ToList();
        }

        /// <summary>
        /// Saves/inserts all spaces from Unity into graph database
        /// </summary>
        public void PushAllUnitySpacesToGraph() {
            // Finds all spaces currently in Unity project
            Debug.Log(Building.Instance.Spaces.Count);

            foreach (Core.Space space in Building.Instance.Spaces) {
                System.Random rd = new System.Random();
                SpaceNode SpaceNode = new SpaceNode {
                    id = rd.Next(0, 5000),                              /////////////////////////// SKAL OPDATERES
                    name = space.Shape.ToString().ToLower(),    /////////////////////////// SKAL OPDATERES
                    area = 17.5f,                                       /////////////////////////// SKAL OPDATERES
                    type = space.Type,
                    shape = space.Shape,
                    vertices = GraphUtils.Vector3ListToStringList(space.GetControlPoints())
                };

                CreateOrMergeSpaceNode(SpaceNode);
            }
        }


        /// <summary>
        /// Create space in graph (or merge with existing: overwriting its parameters)  
        /// </summary>
        public void CreateOrMergeSpaceNode(SpaceNode space) {

            _graphClient.Cypher
                .MergeEntity(space, "space")
                .ExecuteWithoutResults();
        }


        /// <summary>
        /// Create adjacency relationship between two spaces
        /// </summary>
        public void CreateOrMergeAdjacentRelationship(SpaceNode space, SpaceNode secondSpace) {
            _graphClient.Cypher
                .MergeEntity(space, "space")
                .MergeEntity(secondSpace, "secondSpace")
                .MergeRelationship(new AdjacentSpaceRelationship("space", "secondSpace"))
                .ExecuteWithoutResults();
        }


        /// <summary>
        /// Finds a space in the graph based on its Id
        /// </summary>
        /// <param name="Id">Id to search for</param>
        /// <returns>SpaceNode with given id</returns>
        public SpaceNode GetSpaceNodeById(int Id) {
            //string queryId = Id.ToString();

            SpaceNode foundSpace = _graphClient.Cypher
                .Match("( space:Space)")
                .Where<SpaceNode>(space => space.id == Id)
                .Return(space => space.As<SpaceNode>())
                .Results
                .First();
            return foundSpace;
        }

        /// <summary>
        /// Deletes all nodes in the graph with the given label
        /// </summary>
        /// <param name="label">Label of nodes to delete</param>
        public void DeleteAllNodesWithLabel(string label = "") {
            _graphClient.Cypher
                .Match(string.Format("(r:{0})", label))
                .DetachDelete("r")
                .ExecuteWithoutResultsAsync();
        }

        //public void QueryPersonStuff() {
        //    //var agent = SampleDataFactory.GetWellKnownPerson(7);

        //    var niklas = new Person {
        //        Id = 25,
        //        Name = "Anders Bomannowich",
        //        Sex = Gender.Male,
        //        HomeAddress = new Address { Street = "Rådsmandsgade 15", Suburb = "Nørrebro" },
        //        WorkAddress = new Address { Street = "Bredgade 25X", Suburb = "Lyngby" },
        //        IsOperative = true,
        //        SerialNumber = 123456,
        //        SpendingAuthorisation = 100.23m,
        //        DateCreated = DateTimeOffset.Parse("2015-07-11T08:00:00+10:00")
        //    };

        //    _graphClient.Cypher
        //        .CreateEntity(niklas, "niggus")
        //        .CreateEntity(niklas.HomeAddress, "homeAddress")
        //        .CreateEntity(niklas.WorkAddress, "workAddress")
        //        .CreateRelationship(new HomeAddressRelationship("niggus", "homeAddress"))
        //        .CreateRelationship(new WorkAddressRelationship("niggus", "workAddress"))
        //        .ExecuteWithoutResults();

        //    Console.WriteLine("Done query stuff!");
        //}

    }
}
