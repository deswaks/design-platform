using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gbx = gbXMLSerializer;

namespace gbXML {
    public static class gbXMLExporter {

        public static void Export() {
            // Create gbXML
            gbx.gbXML gb = new gbx.gbXML();
            Converter.gb = gb;

            // Create campus
            gb.Campus = new gbx.Campus();
            gb.Campus.Buildings = new gbx.Building[1];

            // Create Building
            gbx.Building building = new gbx.Building();
            gb.Campus.Buildings[0] = building;

            // Create spaces
            List<gbx.Space> spaces = new List<gbx.Space>();
            foreach (Room room in Building.Instance.GetRooms()) {
                gbx.Space space = Converter.XmlSpaceFromRoom(room);
                building.Area += (float)space.Area;
                spaces.Add(space);
            }
            gb.Campus.Buildings[0].Spaces = spaces.ToArray();

            string filename = "test.xml";
            string result = gbx.BasicSerialization.CreateXML(filename, gb);
        }

        
    }
}
