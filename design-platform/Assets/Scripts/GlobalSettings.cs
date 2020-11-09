using System;
using System.IO;

namespace DesignPlatform.Core {
    public static class GlobalSettings {
        //public static string savepath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\DesignPlatform\RoomNodes.json";
        private static string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\DesignPlatform";
        private static string saveFileName = "RoomNodes.json";
        public static bool ShowWallLines = true;
        public static bool ShowRoomTags = true;
        public static bool ShowOpeningLines = false;

        public static string GetSavePath() {

            if (!Directory.Exists(saveFolder)) {
                Directory.CreateDirectory(saveFolder);
            }
            return saveFolder + @"\" + saveFileName;
        }



    }
}