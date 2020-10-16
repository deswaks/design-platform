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
//using System.Numerics;


namespace GraphDatabase {
    public class GraphProgram {

        public static void WorkIt() {
            var db = new DatabaseConnection();

            // Sets up the data model, defining how the C# classes should be handled as Nodes in the graph
            NeoConfig.ConfigureDataModel();


            //Debug.Log(Utils.StringListToVector3List(room1.vertices)[0]);
            //db.DeleteAllNodesWithLabel("Room");

            db.PushAllUnityRoomsToGraph();


            // LOAD AND BUILD ALL ROOMS FROM GRAPH
            //foreach(int id in db.GetAllRoomIdsInGraph()) {
            //    RoomNode RoomNode = db.GetRoomById(id);
            //    Utils.CreateRoomFromGraph(RoomNode);
            //}


            Debug.Log("Graph stuff done!");

        }
    }
   
    public class DatabaseConnection {

        private readonly BoltGraphClient _graphClient;

        public DatabaseConnection() {
            //First create a 'Driver' instance.
            var driver = Neo4j.Driver.V1.GraphDatabase.Driver("bolt://localhost:7687", Config.Builder.WithEncryptionLevel(EncryptionLevel.None).ToConfig());

            // Attach it to a bolt graph client
            _graphClient = new BoltGraphClient(driver);

            // Connect to the client
            _graphClient.Connect();
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
            List<Room> buildingRooms = Building.Instance.GetRooms();
            Debug.Log(buildingRooms.Count);

            foreach(Room room in buildingRooms) {
                System.Random rd = new System.Random();
                RoomNode RoomNode = new RoomNode {
                    id = rd.Next(0, 5000),                              /////////////////////////// SKAL OPDATERES
                    name = room.GetRoomShape().ToString().ToLower(),    /////////////////////////// SKAL OPDATERES
                    area = 17.5f,                                       /////////////////////////// SKAL OPDATERES
                    type = room.RoomType,
                    shape = room.GetRoomShape(),
                    vertices = Utils.Vector3ListToStringList(room.GetControlPoints())
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
        public RoomNode GetRoomById(int Id) {
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
        /// Searches for and returns a list of all room nodes in graph
        /// </summary>
        /// <returns>List of RoomNodes</returns>
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
        /// Deletes all nodes in the graph with the given label
        /// </summary>
        /// <param name="label">Label of nodes to delete</param>
        public void DeleteAllNodesWithLabel(string label = "") {
            _graphClient.Cypher
                .Match(String.Format("(r:{0})", label))
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
