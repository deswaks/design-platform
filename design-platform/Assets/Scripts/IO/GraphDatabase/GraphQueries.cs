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

            //db.DeleteAllNodesWithLabel("Room");

            //db.PushAllUnityRoomsToGraph();


            //db.GetAllGraphEntities();

            LocalDatabase.CreateAllUnityRoomsFromJson(@"C:\Users\Administrator\Desktop\RoomNodes.json");
            // LOAD AND BUILD ALL ROOMS FROM GRAPH
            //foreach(int id in db.GetAllRoomIdsInGraph()) {
            //    RoomNode RoomNode = db.GetRoomById(id);
            //GraphUtils.CreateRoomFromGraph(RoomNode);
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


        public void LoadAndBuildUnityRoomsFromGraph() {
            // LOAD AND BUILD ALL ROOMS FROM GRAPH
            foreach (int id in GetAllRoomIdsInGraph()) {
                RoomNode RoomNode = GetRoomNodeById(id);
                LocalDatabase.CreateUnityRoomFromRoomNode(RoomNode);
            }
        }

        /// <summary>
        /// Reads RoomNodes specified in the json file and merges them into graph
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        public void ImportJsonRoomNodesToGraph(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : Settings.SaveFolderPath+"RoomNodes.json";

            IEnumerable<RoomNode> roomNodes = LocalDatabase.LoadRoomNodesFromJson(jsonPath);

            foreach (RoomNode roomNode in roomNodes) {
                CreateOrMergeRoomNode(roomNode);
            }

            #region oldCode
            // TANKEN MÅ VÆRE FØRST AT LOADE FRA JSON TIL ROOMNODES OG SÅ DEREFTER SKRIVE ROOMNODES TIL GRAF

            //CALL apoc.load.json('complete-db.json') YIELD value
            //UNWIND value.items AS item
            //CREATE(i: Item(name: item.name, id: item.id)
            //_graphClient.Cypher
            //    .Call("apoc.load.json(\"file:///C:/Users/Administrator/Desktop/FromUnity.json \")")
            //    .Yield("value AS room")
            //    .Merge("(p:Room {name: value.name, id: value.id, vertices: value.vertices, shape: value.shape, type: value.type})")
            //    .ExecuteWithoutResults();

            ////CALL apoc.load.json("file:///C:/Users/Administrator/Desktop/FromUnity.json")
            ////YIELD value
            ////MERGE (p:Room {name: value.name, id: value.id, vertices: value.vertices, shape: value.shape, type: value.type})

            //_graphClient.Cypher
            //    .MergeEntity(room, "room")
            //    .ExecuteWithoutResults();
            #endregion
        }
        public List<RoomNode> GetAllRoomNodes() {

            List<RoomNode> RoomNodes = _graphClient.Cypher
                .Match("(room:Room)")
                .Return(room => room.As<RoomNode>())
                .Results
                .ToList();

            return RoomNodes;

            #region old_code
            ////////////////// VIRKER SEMI //////////////////////////////////////

            // DENNE METODE VIRKER
            //var query = _graphClient.Cypher
            //        .Match("(p:Room)")
            //        .Return(p => new { 
            //            ID = p.As<Room>().id,
            //            V = p.As<Room>().vertices 
            //        });

            //var results = query.Results.ToList();

            // DENNE METODE VIRKER OG RETURNERER NYE RUM MED EGENSKABER FRA GRAF
            //var query2 = _graphClient.Cypher
            //    .Match("(p:Room)")
            //    .Return(p => 
            //    new Room { 
            //        id = p.As<Room>().id,
            //        name = p.As<Room>().name,
            //        type = p.As<Room>().type,
            //        area = p.As<Room>().area,
            //        vertices = p.As<Room>().vertices,

            //});
            //var results2 = query2.Results.ToList();

            //foreach (var result in results)
            //{
            //    result.N.Properties.Keys.ToList().ForEach(k => Console.WriteLine(k));
            //    //nNodes.Add(JsonConvert.DeserializeObject(result.N.Properties) );
            //}

            //.Match("( room:Room)")
            //.Return(room => room.As<Room>())
            //.Results
            //.ToList();
            #endregion
        }

        /// <summary>
        /// Finds and returns list of IDs for all rooms in graph
        /// </summary>
        /// <returns>List of IDs of all rooms in graph</returns>
        public List<int> GetAllRoomIdsInGraph() {

            var query = _graphClient.Cypher
                        .Match("(p:Room)")
                        .Return(p =>
                            p.As<RoomNode>().id
            );
            var results = query.Results.ToList();

            return results.Select(r => Convert.ToInt32(r)).ToList();
        }

        /// <summary>
        /// Saves/inserts all rooms from Unity into graph database
        /// </summary>
        public void PushAllUnityRoomsToGraph() {
            // Finds all rooms currently in Unity project
            Debug.Log(Building.Instance.Rooms.Count);

            foreach (Room room in Building.Instance.Rooms) {
                System.Random rd = new System.Random();
                RoomNode RoomNode = new RoomNode {
                    id = rd.Next(0, 5000),                              /////////////////////////// SKAL OPDATERES
                    name = room.Shape.ToString().ToLower(),    /////////////////////////// SKAL OPDATERES
                    area = 17.5f,                                       /////////////////////////// SKAL OPDATERES
                    type = room.Type,
                    shape = room.Shape,
                    vertices = GraphUtils.Vector3ListToStringList(room.GetControlPoints())
                };

                CreateOrMergeRoomNode(RoomNode);
            }
        }


        /// <summary>
        /// Create room in graph (or merge with existing: overwriting its parameters)  
        /// </summary>
        public void CreateOrMergeRoomNode(RoomNode room) {

            _graphClient.Cypher
                .MergeEntity(room, "room")
                .ExecuteWithoutResults();
        }


        /// <summary>
        /// Create adjacency relationship between two rooms
        /// </summary>
        public void CreateOrMergeAdjacentRelationship(RoomNode room, RoomNode secondRoom) {
            _graphClient.Cypher
                .MergeEntity(room, "room")
                .MergeEntity(secondRoom, "secondRoom")
                .MergeRelationship(new AdjacentRoomRelationship("room", "secondRoom"))
                .ExecuteWithoutResults();
        }


        /// <summary>
        /// Finds a room in the graph based on its Id
        /// </summary>
        /// <param name="Id">Id to search for</param>
        /// <returns>RoomNode with given id</returns>
        public RoomNode GetRoomNodeById(int Id) {
            //string queryId = Id.ToString();

            RoomNode foundRoom = _graphClient.Cypher
                .Match("( room:Room)")
                .Where<RoomNode>(room => room.id == Id)
                .Return(room => room.As<RoomNode>())
                .Results
                .First();
            return foundRoom;
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
