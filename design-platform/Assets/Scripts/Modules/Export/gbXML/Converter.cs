using gbXMLSerializer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gbx = gbXMLSerializer;

namespace gbXML {
    public static class Converter {
        public static gbx.gbXML gb;

        public static gbx.Space XmlSpaceFromRoom(Room room) {
            
            // Create space
            gbx.Space space = new gbx.Space();
            //space.id = "Space_" + room.name;
            //space.Name = "Space_" + room.name;

            // Space area
            gbx.Area area = new gbx.Area();
            area.val = string.Format("{0:N2}", room.GetFloorArea());
            space.spacearea = area;

            // Space volume
            gbx.Volume vol = new gbx.Volume();
            vol.val = string.Format("{0:N2}", room.GetVolume());
            space.spacevol = vol;

            // Shell geometry
            gbx.ShellGeometry shellGeometry = new gbx.ShellGeometry();
            shellGeometry.unit = gbx.lengthUnitEnum.Meters;
            shellGeometry.id = "sg" + space.Name;

            // Closed shell
            gbx.ClosedShell closedShell = new gbx.ClosedShell();
            shellGeometry.ClosedShell = closedShell;

            Vector3[,] surfacesVertices = room.GetWallVertices();

            // Make polyloop arrays
            closedShell.PolyLoops = gbx.prod.makePolyLoopArray(surfacesVertices.Length);
            for (int i = 0; i < closedShell.PolyLoops.Length; i++) {
                
                // Make polyloop points
                closedShell.PolyLoops[i].Points = gbx.BasicSerialization.makeCartesianPtArray(surfacesVertices.GetLength(1));
                for (int j = 0; j < closedShell.PolyLoops[i].Points.Length; j++) {
                    CartesianPoint cartesianPoint = new CartesianPoint();
                    cartesianPoint.Coordinate = new string[3];
                    cartesianPoint.Coordinate[0] = string.Format("{0:N3}", surfacesVertices[i, j].x);
                    cartesianPoint.Coordinate[1] = string.Format("{0:N3}", surfacesVertices[i, j].y);
                    cartesianPoint.Coordinate[2] = string.Format("{0:N3}", surfacesVertices[i, j].z);
                    closedShell.PolyLoops[i].Points[j] = cartesianPoint;
                }
            }

            return space;
        }
    }
}
