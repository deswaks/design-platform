using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using DesignPlatform.Core;

namespace DesignPlatform.Database {
    public class LocalDatabase {

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
        public static void CreateAllUnityRoomsFromJson(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : GlobalSettings.GetSavePath();

            IEnumerable<RoomNode> roomNodes = LoadRoomNodesFromJson(jsonPath);
            // Loops through room nodes and creates correponding Unity rooms
            foreach (RoomNode roomNode in roomNodes) {
                CreateUnityRoomFromRoomNode(roomNode);
            }

        }

        /// <summary>
        /// Converts all Unity rooms to RoomNodes and then saves them to a json file at the specified path.
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        public static void SaveAllUnityRoomsToJson(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : GlobalSettings.GetSavePath();

            // Collects Unity room as RoomNodes
            List<RoomNode> roomNodes = UnityRoomsToRoomNodes(Building.Instance.GetRooms());

            // Serializes RoomNodes to json format
            string jsonString = JsonConvert.SerializeObject(roomNodes);

            // Saves file
            File.WriteAllText(jsonPath, jsonString);


            // Generates notification in corner of screen
            string notificationTitle = "File saved";
            string notificationText = "The file has been saved at " + GlobalSettings.GetSavePath();


            GameObject notificationParent = Object.FindObjectsOfType<Canvas>().Where(c => c.gameObject.name == "UI").First().gameObject;
            Rect parentRect = notificationParent.GetComponent<RectTransform>().rect;
            Vector3 newLocation = new Vector3(parentRect.width / 2 - 410, -parentRect.height / 2 + 150, 0);

            GameObject notificationObject = NotificationHandler.GenerateNotification(notificationText, notificationTitle, newLocation, notificationParent, 5);


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
        /// Converts all Unity rooms to RoomNodes and then saves them to a json file at the specified path.
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        public static void SaveAllUnityInterfacesToJson(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : GlobalSettings.GetSaveFolder() + @"\interfaces.json";

            // Collects Unity room as RoomNodes
            List<InterfaceNode> interfaceNodes = AllRoomInterfacesToInterfaceNodes(Building.Instance.rooms);

            // Serializes RoomNodes to json format
            string jsonString = JsonConvert.SerializeObject(interfaceNodes);


            // Saves file
            File.WriteAllText(jsonPath, jsonString);

            //// Generates notification in corner of screen
            //string notificationTitle = "File saved";
            //string notificationText = "The file has been saved at " + GlobalSettings.GetSavePath();

            //GameObject notificationParent = Object.FindObjectsOfType<Canvas>().Where(c => c.gameObject.name == "UI").First().gameObject;
            //Rect parentRect = notificationParent.GetComponent<RectTransform>().rect;
            //Vector3 newLocation = new Vector3(parentRect.width / 2 - 410, -parentRect.height / 2 + 150, 0);

            //GameObject notificationObject = NotificationHandler.GenerateNotification(notificationText, notificationTitle, newLocation, notificationParent, 5);

        }
        /// <summary>
        /// Loads RoomNodes specified in the given json file.
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        /// <returns></returns>
        public static IEnumerable<RoomNode> LoadRoomNodesFromJson(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : GlobalSettings.GetSavePath();

            // Reads json file
            string jsonString = File.ReadAllText(jsonPath);

            // Deserializes the json string into RoomNode objects
            return JsonConvert.DeserializeObject<IEnumerable<RoomNode>>(jsonString);
        }


        /// <summary>
        /// Creates a list of RoomNodes corresponding to the list of Unity Rooms
        /// </summary>
        /// <param name="rooms">Unity Rooms to convert.</param>
        /// <returns>List of RoomNodes created</returns>
        public static List<RoomNode> UnityRoomsToRoomNodes(List<Room> rooms) {

            List<RoomNode> roomNodes = new List<RoomNode>();

            foreach (Room room in rooms) {

                System.Random rd = new System.Random();

                RoomNode roomNode = new RoomNode {
                    id = rd.Next(0, 5000),                              /////////////////////////// SKAL OPDATERES
                    name = room.GetRoomShape().ToString().ToLower(),    /////////////////////////// SKAL OPDATERES
                    area = 17.5f,                                       /////////////////////////// SKAL OPDATERES
                    type = room.roomType,
                    shape = room.GetRoomShape(),
                    vertices = GraphUtils.Vector3ListToStringList(room.GetControlPoints())
                };
                roomNodes.Add(roomNode);

            }
            return roomNodes;
        }

        /// <summary>
        /// Using input list of rooms, finds all interfaces belonging to those rooms and returns them as InterfaceNodes.
        /// </summary>
        /// <param name="rooms">List of rooms</param>
        /// <returns>List of InterfaceNodes created.</returns>
        public static List<InterfaceNode> AllRoomInterfacesToInterfaceNodes(List<Room> rooms) {
            List<Interface> allInterfaces = Building.Instance.interfaces; //rooms.SelectMany(r => r.faces.SelectMany(f => f.interfaces)).ToList();

            List < InterfaceNode> interfaceNodes = new List<InterfaceNode>();


            foreach (Interface iface in allInterfaces) {

                InterfaceNode node = new InterfaceNode {
                    vertices = GraphUtils.Vector3ListToStringList(new List<Vector3> { iface.GetStartPoint(), iface.GetEndPoint() })
                };

                Debug.Log(node.vertices);

                interfaceNodes.Add(node);

            }

            return interfaceNodes;
        }

    }
}