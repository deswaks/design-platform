﻿using System;
using System.IO;

public static class GlobalSettings {
    //public static string savepath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\DesignPlatform\RoomNodes.json";
    private static string saveFolder    = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\DesignPlatform";
    private static string saveFileName  = "RoomNodes.json";

    public static string GetSavePath() {

        if (!Directory.Exists(saveFolder)) {
            Directory.CreateDirectory(saveFolder);
        }
        return saveFolder + @"\" + saveFileName;
    }

}