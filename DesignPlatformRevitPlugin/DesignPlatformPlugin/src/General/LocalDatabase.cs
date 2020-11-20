using DesignPlatform.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DesignPlatform.Database {
    public class LocalDatabase {

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
        

        public static IEnumerable<WallElementNode> LoadInterfaceNodesFromJson(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : GlobalSettings.GetSavePath();

            // Reads json file
            string jsonString = File.ReadAllText(jsonPath);

            // Deserializes the json string into RoomNode objects
            return JsonConvert.DeserializeObject<IEnumerable<WallElementNode>>(jsonString);
        }
    }
}