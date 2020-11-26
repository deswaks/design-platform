﻿using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPlatform.Core;

namespace DesignPlatform.Export {
    public static class GbxmlExporter {
        public static gbXMLSerializer.gbXML gbx = new gbXMLSerializer.gbXML();

        public static void ClearXML() {
            gbx = new gbXMLSerializer.gbXML();
        }

        public static void Export() {

            // Create campus
            gbx.Campus = new gbXMLSerializer.Campus();
            gbx.Campus.Buildings = new gbXMLSerializer.Building[1];

            // Create Building
            gbXMLSerializer.Building building = new gbXMLSerializer.Building();
            building.id = "CLT House";
            building.buildingType = gbXMLSerializer.buildingTypeEnum.SingleFamily;
            building.Area = 0.0f;
            //building.bldgStories = ;
            gbx.Campus.Buildings[0] = building;

            // Create spaces
            List<gbXMLSerializer.Space> spaces = new List<gbXMLSerializer.Space>();
            foreach (Core.Space room in Building.Instance.Spaces) {
                gbXMLSerializer.Space space = GbxmlConverter.XmlSpaceFromSpace(room);
                building.Area += (float)space.Area;
                spaces.Add(space);
            }
            building.Spaces = spaces.ToArray();

            // Save and open file
            string result = gbXMLSerializer.BasicSerialization.CreateXML("Exports/building.gbxml", gbx);
            //Process.Start("Exports\\building.gbxml"); //Start viewer
            UnityEngine.Debug.Log("Successfully exported gbxml to: ~Exports/building.gbxml");
        }


    }
}
