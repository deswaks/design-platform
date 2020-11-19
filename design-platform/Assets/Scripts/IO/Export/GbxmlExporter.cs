using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gs = gbXMLSerializer;
using DesignPlatform.Core;

namespace DesignPlatform.Export {
    public static class GbxmlExporter {
        public static gs.gbXML gbx = new gs.gbXML();

        public static void ClearXML() {
            gbx = new gs.gbXML();
        }

        public static void Export() {

            // Create campus
            gbx.Campus = new gs.Campus();
            gbx.Campus.Buildings = new gs.Building[1];

            // Create Building
            gs.Building building = new gs.Building();
            building.id = "CLT House";
            building.buildingType = gs.buildingTypeEnum.SingleFamily;
            building.Area = 0.0f;
            //building.bldgStories = ;
            gbx.Campus.Buildings[0] = building;

            // Create spaces
            List<gs.Space> spaces = new List<gs.Space>();
            foreach (Room room in Building.Instance.Rooms) {
                gs.Space space = GbxmlConverter.XmlSpaceFromRoom(room);
                building.Area += (float)space.Area;
                spaces.Add(space);
            }
            building.Spaces = spaces.ToArray();

            // Save and open file
            string result = gs.BasicSerialization.CreateXML("Exports/building.gbxml", gbx);
            //Process.Start("Exports\\building.gbxml"); //Start viewer
            UnityEngine.Debug.Log("Successfully exported gbxml to: ~Exports/building.gbxml");
        }


    }
}
