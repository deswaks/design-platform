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
using UnityEditor.Experimental.AssetImporters;


//using System.Numerics;


namespace GraphDatabase {
    public class GraphProgram {

        public static void WorkIt() {
            var db = new DatabaseConnection();
            // Sets up the data model, defining how the C# classes should be handled as Nodes in the graph
            NeoConfig.ConfigureDataModel();

            //db.DeleteAllNodesWithLabel("Room");

            //db.PushAllUnityRoomsToGraph();


            //db.GetAllGraphEntities();

            db.CreateAllUnityRoomsFromJson(@"C:\Users\Administrator\Desktop\RoomNodes.json");
            // LOAD AND BUILD ALL ROOMS FROM GRAPH
            //foreach(int id in db.GetAllRoomIdsInGraph()) {
            //    RoomNode RoomNode = db.GetRoomById(id);
            //GraphUtils.CreateRoomFromGraph(RoomNode);
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
        /// Creates a single Room in Unity corresponding to the given RoomNode
        /// </summary>
        /// <param name="RoomNode">RoomNode to create as a Room in Unity</param>
        public static void CreateUnityRoomFromRoomNode(RoomNode RoomNode) {
            // Builds room
            Room newRoom = Building.Instance.BuildRoom(RoomNode.shape, preview: false, templateRoom: null);
            //Gets control points from graph data
            List<Vector3> controlPoints = GraphUtils.StringListToVector3List(RoomNode.vertices);
            newRoom.SetControlPoints(controlPoints);
            newRoom.SetRoomType(RoomNode.type);
        }

        /// <summary>
        /// Reads a .json file with room node definitions and creates correponding Rooms in Unity
        /// </summary>
        /// <param name="jsonPath">Full path of json file, with backslashes.</param>
        public void CreateAllUnityRoomsFromJson(string jsonPath = @"C:\RoomNodes.json") {
            IEnumerable<RoomNode> roomNodes = LoadRoomNodesFromJson(jsonPath);
            // Loops through room nodes and creates correponding Unity rooms
            foreach(RoomNode roomNode in roomNodes) {
                CreateUnityRoomFromRoomNode(roomNode);
            }
        }

        /// <summary>
        /// Converts all Unity rooms to RoomNodes and then saves them to a json file at the specified path.
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        public void SaveAllUnityRoomsToJson(string savePath = @"C:\RoomNodes.json") {

            // Collects Unity room as RoomNodes
            List<RoomNode> roomNodes = UnityRoomsToRoomNodes(Building.Instance.GetRooms());

            // Serializes RoomNodes to json format
            string jsonString = JsonConvert.SerializeObject(roomNodes);

            // Saves file
            File.WriteAllText(savePath, jsonString);

            #region old_codie
            ////// Save the JSON to a file.
            //string path = @"C:\Users\Administrator\Desktop\Neo4JExport.json";
            //if (File.Exists(path)) File.Delete(path);
            ////File.WriteAllText(path, sJsonData);

            //string query = "CALL apoc.export.json.all(file::STRING ?, config = { } ::MAP ?) :: (file::STRING ?, " +
            //    "source::STRING ?, format::STRING ?, nodes::INTEGER ?, relationships::INTEGER ?, " +
            //    "properties::INTEGER ?, time::INTEGER ?, rows::INTEGER ?, batchSize::INTEGER ?, batches::INTEGER ?, " +
            //    "done::BOOLEAN ?, data::STRING ?)";

            //string query2 = "apoc.export.json.all(null,{stream:true,useTypes:true}) " +
            //"YIELD file, nodes, relationships, properties, data " +
            //"RETURN file, nodes, relationships, properties, data";

            //var query3 = _graphClient.Cypher
            //    .Call("apoc.export.json.all( \"Neo4JExport.json\" ,{ stream: true,useTypes: true})")
            //    .Yield("file, nodes, relationships, properties, data")
            //    //.Return( (file, nodes, relationships, properties, data) => new {F = file.As<string>(), N = nodes.As<string>(), R = relationships.As<string>(), P = properties.As<string>(), D = data.As<string>() })
            //    .Return(data => data.As<string>() )
            //    .Results;
            ////var RoomNodes = _graphClient.Cypher
            //// file = null      nodes = antal nodes som int     relationships = antal som int       properties = antal som int

            ////    .Match("(room:Room)")
            ////    .Return(room => room.As<RoomNode>())
            ////    .Results;
            //foreach(string s in query3) {
            //    Debug.Log(s);
            //}
            #endregion
        }

        /// <summary>
        /// Loads RoomNodes specified in the given json file.
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        /// <returns></returns>
        public IEnumerable<RoomNode> LoadRoomNodesFromJson(string savePath = @"C:\RoomNodes.json") {
            // Reads json file
            string jsonString = File.ReadAllText(savePath);

            // Deserializes the json string into RoomNode objects
            return JsonConvert.DeserializeObject<IEnumerable<RoomNode>>(jsonString);
        }


        /// <summary>
        /// Reads RoomNodes specified in the json file and merges them into graph
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        public void ImportJsonRoomNodesToGraph(string savePath = @"C:\RoomNodes.json") {

            IEnumerable<RoomNode> roomNodes = LoadRoomNodesFromJson(savePath);

            foreach(RoomNode roomNode in roomNodes) {
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

        /// <summary>
        /// Creates a list of RoomNodes corresponding to the list of Unity Rooms
        /// </summary>
        /// <param name="rooms">Unity Rooms to convert.</param>
        /// <returns>List of RoomNodes created</returns>
        public List<RoomNode> UnityRoomsToRoomNodes(List<Room> rooms) {
            
            List<RoomNode> roomNodes = new List<RoomNode>();

            foreach (Room room in rooms) {

                System.Random rd = new System.Random();

                RoomNode roomNode = new RoomNode {
                    id = rd.Next(0, 5000),                              /////////////////////////// SKAL OPDATERES
                    name = room.GetRoomShape().ToString().ToLower(),    /////////////////////////// SKAL OPDATERES
                    area = 17.5f,                                       /////////////////////////// SKAL OPDATERES
                    type = room.RoomType,
                    shape = room.GetRoomShape(),
                    vertices = GraphUtils.Vector3ListToStringList(room.GetControlPoints())
                };
                roomNodes.Add(roomNode);

            }
            return roomNodes;
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
