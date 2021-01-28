using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using DesignPlatform.Core;
using DesignPlatform.UI;
using DesignPlatform.Geometry;

namespace DesignPlatform.Database {
    /// <summary>
    /// Contains functions for reading/writing from/to local database (JSON).
    /// </summary>
    public class LocalDatabase {

        /// <summary>
        /// Creates a single Space in Unity corresponding to the given SpaceNode
        /// </summary>
        /// <param name="spaceNode">Node to create as a space in Unity</param>
        public static void CreateUnitySpaceFromSpaceNode(SpaceNode spaceNode) {
            // Builds space
            Core.Space newSpace = Building.Instance.BuildSpace(spaceNode.shape);
            //Gets control points from graph data
            
            List<Vector3> controlPoints = GraphUtils.StringListToVector3List(spaceNode.vertices);
            newSpace.SetControlPoints(controlPoints);
            newSpace.Function = spaceNode.type;
            newSpace.UpdateRender2D();
        }

        /// <summary>
        /// Creates a single Space in Unity corresponding to the given SpaceNode
        /// </summary>
        /// <param name="openingNode">Node to create as an opening in Unity</param>
        public static void CreateUnityOpeningFromOpeningNode(OpeningNode openingNode) {
            // Builds opening
            Opening newOpening = Building.Instance.BuildOpening(
                openingNode.openingShape, 
                GraphUtils.StringToVector3(openingNode.position), 
                Quaternion.Euler(GraphUtils.StringToVector3(openingNode.rotation))
                );
        }

        /// <summary>
        /// Reads a .json file with space node definitions and creates correponding spaces in Unity
        /// </summary>
        /// <param name="jsonPath">Full path of json file, with backslashes.</param>
        public static void CreateAllUnitySpacesFromJson(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : Settings.SaveFolderPath + "SpaceNodes.json";

            IEnumerable<SpaceNode> spaceNodes = LoadSpaceNodesFromJson(jsonPath);
            // Loops through spaces nodes and creates correponding Unity spaces
            foreach (SpaceNode spaceNode in spaceNodes) {
                CreateUnitySpaceFromSpaceNode(spaceNode);
            }
        }

        /// <summary>
        /// Reads a .json file with spaces node definitions and creates correponding Spaces in Unity
        /// </summary>
        /// <param name="jsonPath">Full path of json file, with backslashes.</param>
        public static void CreateAllUnityOpeningsFromJson(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : Settings.SaveFolderPath;

            IEnumerable<OpeningNode> openingNodes = LoadOpeningNodesFromJson(jsonPath);
            // Loops through space nodes and creates correponding Unity spaces
            foreach (OpeningNode openingNode in openingNodes) {
                CreateUnityOpeningFromOpeningNode(openingNode);
            }
        }

        /// <summary>
        /// Converts all Unity spaces to spaceNodes and then saves them to a json file at the specified path.
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        public static void SaveAllUnitySpacesAndOpeningsToJson(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : Settings.SaveFolderPath;

            // Collects Unity space as spaceNodes
            List<SpaceNode> spaceNodes = UnitySpacesToSpaceNodes(Building.Instance.Spaces);
            List<OpeningNode> openingNodes = UnityOpeningsToOpeningNodes(Building.Instance.Openings);


            // Serializes spaceNodes to json format
            string spacesJsonString = JsonConvert.SerializeObject(spaceNodes);
            string openingsJsonString = JsonConvert.SerializeObject(openingNodes);

            // Saves file
            File.WriteAllText(jsonPath + "SpaceNodes.json", spacesJsonString);
            File.WriteAllText(jsonPath + "OpeningNodes.json", openingsJsonString);


            // Generates notification in corner of screen
            string notificationTitle = "File saved";
            string notificationText = "The design has been saved in " + Settings.SaveFolderPath;

            GameObject notificationParent = Object.FindObjectsOfType<Canvas>().Where(c => c.gameObject.name == "UI").First().gameObject;
            Rect parentRect = notificationParent.GetComponent<RectTransform>().rect;
            Vector3 newLocation = new Vector3(parentRect.width / 2 - 410, -parentRect.height / 2 + 150, 0);

            GameObject notificationObject = NotificationHandler.GenerateNotification(notificationText, notificationTitle, newLocation, notificationParent, 5);

        }
        /// <summary>
        /// Converts all Unity spaces to SpaceNodes and then saves them to a json file at the specified path.
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        public static void SaveAllUnitySpacesToJson(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : Settings.SaveFolderPath + "SpaceNodes.json";

            // Collects Unity space as SpaceNodes
            List<SpaceNode> spaceNodes = UnitySpacesToSpaceNodes(Building.Instance.Spaces);

            // Serializes SpaceNodes to json format
            string jsonString = JsonConvert.SerializeObject(spaceNodes);

            // Saves file
            File.WriteAllText(jsonPath, jsonString);


            // Generates notification in corner of screen
            string notificationTitle = "File saved";
            string notificationText = "The file has been saved at " + Settings.SaveFolderPath + "SpaceNodes.json";

            GameObject notificationParent = Object.FindObjectsOfType<Canvas>().Where(c => c.gameObject.name == "UI").First().gameObject;
            Rect parentRect = notificationParent.GetComponent<RectTransform>().rect;
            Vector3 newLocation = new Vector3(parentRect.width / 2 - 410, -parentRect.height / 2 + 150, 0);

            GameObject notificationObject = NotificationHandler.GenerateNotification(notificationText, notificationTitle, newLocation, notificationParent, 5);


        }


        /// <summary>
        /// Saves all wall elements as JSON file.
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        public static void SaveAllWallElementsToJson(string jsonPath = null)
        {
            jsonPath = jsonPath != null ? jsonPath : Settings.SaveFolderPath + "WallElements.json";

            List<CLTElement> wallElements = Building.Instance.CLTElements;

            // Collects Unity space as SpaceNodes
            List<WallElementNode> wallElementNodes = new List<WallElementNode>();

            foreach (CLTElement element in wallElements) {
                wallElementNodes.Add(new WallElementNode {
                    vertices = GraphUtils.Vector3ListToStringList(new List<Vector3> { element.StartJoint.point, element.EndJoint.point}),
                    startJointType = element.StartJoint.jointType.ToString(),
                    endJointType = element.EndJoint.jointType.ToString(),
                    midPointJointTypes = element.MidJoints.Select(p => p.jointType.ToString()).ToArray()
                });
            }

            // Serializes SpaceNodes to json format
            string jsonString = JsonConvert.SerializeObject(wallElementNodes);

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
        /// Loads SpaceNodes specified in the given json file.
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        /// <returns>List of SpaceNodes in json file.</returns>
        public static IEnumerable<SpaceNode> LoadSpaceNodesFromJson(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : Settings.SaveFolderPath + "SpaceNodes.json";

            // Reads json file
            string jsonString = File.ReadAllText(jsonPath);

            // Deserializes the json string into SpaceNode objects
            return JsonConvert.DeserializeObject<IEnumerable<SpaceNode>>(jsonString);
        }

        /// <summary>
        /// Loads SpaceNodes specified in the given json file.
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        /// <returns>SpaceNodes specified in the given json file.</returns>
        public static IEnumerable<OpeningNode> LoadOpeningNodesFromJson(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : Settings.SaveFolderPath;

            // Reads json file
            string jsonString = File.ReadAllText(jsonPath + "OpeningNodes.json");

            // Deserializes the json string into SpaceNode objects
            return JsonConvert.DeserializeObject<IEnumerable<OpeningNode>>(jsonString);
        }


        /// <summary>
        /// Creates a list of SpaceNodes corresponding to the list of Unity Spaces
        /// </summary>
        /// <param name="spaces">Unity Spaces to convert.</param>
        /// <returns>List of SpaceNodes created</returns>
        public static List<SpaceNode> UnitySpacesToSpaceNodes(List<Core.Space> spaces) {

            List<SpaceNode> spaceNodes = new List<SpaceNode>();

            foreach (Core.Space space in spaces) {

                System.Random rd = new System.Random();

                SpaceNode spaceNode = new SpaceNode {
                    id = rd.Next(0, 5000),                              /////////////////////////// SKAL OPDATERES
                    name = space.Shape.ToString().ToLower(),    /////////////////////////// SKAL OPDATERES
                    area = 17.5f,                                       /////////////////////////// SKAL OPDATERES
                    type = space.Function,
                    shape = space.Shape,
                    vertices = GraphUtils.Vector3ListToStringList(space.GetControlPoints())
                };
                spaceNodes.Add(spaceNode);
            }
            return spaceNodes;
        }

        /// <summary>
        /// Creates a list of OpeningNodes corresponding to the list of Unity Openings
        /// </summary>
        /// <param name="openings">Unity Spaces to convert.</param>
        /// <returns>List of SpaceNodes created</returns>
        public static List<OpeningNode> UnityOpeningsToOpeningNodes(List<Opening> openings) {

            List<OpeningNode> openingNodes = new List<OpeningNode>();

            foreach (Opening opening in openings) {
                //Debug.Log("Loc:" + opening.LocationPoint);
                OpeningNode openingNode = new OpeningNode {
                    openingShape = opening.Function,
                    height = opening.Height,
                    width = opening.Width,
                    position = GraphUtils.Vector3ToString(opening.LocationPoint,2),
                    rotation = GraphUtils.Vector3ToString(opening.transform.rotation.eulerAngles),
                };
                //Debug.Log("saved Loc:" + openingNode.position);

                openingNodes.Add(openingNode);
            }
            return openingNodes;
        }


        /// <summary>
        /// Finds all interfaces belonging to all spaces and returns them as InterfaceNodes.
        /// </summary>
        /// <returns>List of InterfaceNodes created.</returns>
        public static List<WallElementNode> AllSpaceInterfacesToInterfaceNodes() {
            //List<Interface> allInterfaces = Building.Instance.Walls.Select(w => w.Interface).ToList();
            List<Interface> allInterfaces = Building.Instance.Interfaces.Where(i => i.Orientation== Orientation.VERTICAL).ToList();

            //allInterfaces.ForEach(interFace => Debug.Log(interFace.GetStartPoint() + ", " + interFace.GetEndPoint()));

            List < WallElementNode> interfaceNodes = new List<WallElementNode>();

            foreach (Interface iface in allInterfaces) {

                WallElementNode node = new WallElementNode {
                    vertices = GraphUtils.Vector3ListToStringList(new List<Vector3> { iface.StartPoint, iface.EndPoint })
                };
                interfaceNodes.Add(node);
            }
            return interfaceNodes;
        }
    }
}