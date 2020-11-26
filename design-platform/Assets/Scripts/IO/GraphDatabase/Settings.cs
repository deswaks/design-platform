using System;
using System.IO;

namespace DesignPlatform.Database {

    /// <summary>
    /// Contains settings for the database.
    /// </summary>
    public static class Settings {

        private static string saveFolderPath = "Exports/Saved building/";

        /// <summary>
        /// The path to the general save folder for the program.
        /// </summary>
        public static string SaveFolderPath {
            get {
                if (Directory.Exists(saveFolderPath)) return saveFolderPath;
                else {
                    Directory.CreateDirectory(saveFolderPath);
                    return saveFolderPath;
                }
            }
        }
    }
}