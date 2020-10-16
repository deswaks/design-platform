using gbXMLSerializer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gbs = gbXMLSerializer;

namespace gbXML {
    public static class Converter {

        public static gbs.Space XmlSpaceFromRoom(Room room) {

            // Create space
            gbs.Space space = new gbs.Space();
            space.id = "Space_";// + room.name;
            space.Name = "Space_";// + room.name;

            // Space area
            gbs.Area area = new gbs.Area();
            area.val = string.Format("{0:N2}", room.GetFloorArea());
            space.spacearea = area;

            // Space volume
            gbs.Volume vol = new gbs.Volume();
            vol.val = string.Format("{0:N2}", room.GetVolume());
            space.spacevol = vol;

            // Shell geometry
            gbs.ShellGeometry shellGeometry = new gbs.ShellGeometry();
            shellGeometry.unit = gbs.lengthUnitEnum.Meters;
            shellGeometry.id = "sg" + space.Name;
            space.ShellGeo = shellGeometry;

            // Closed shell
            gbs.ClosedShell closedShell = new gbs.ClosedShell();
            shellGeometry.ClosedShell = closedShell;

            List<List<Vector3>> surfacesVertices = room.GetSurfaceVertices();

            // Make polyloop arrays
            closedShell.PolyLoops = gbs.prod.makePolyLoopArray(surfacesVertices.Count);
            for (int i = 0; i < closedShell.PolyLoops.GetLength(0); i++) {

                // Make polyloop points
                closedShell.PolyLoops[i].Points = gbs.BasicSerialization.makeCartesianPtArray(surfacesVertices[i].Count);

                for (int j = 0; j < closedShell.PolyLoops[i].Points.GetLength(0); j++) {
                    List<string> coordinates = new List<string>();
                    coordinates.Add(string.Format("{0:N3}", surfacesVertices[i][j].x));
                    coordinates.Add(string.Format("{0:N3}", surfacesVertices[i][j].y));
                    coordinates.Add(string.Format("{0:N3}", surfacesVertices[i][j].z));

                    CartesianPoint cartesianPoint = new CartesianPoint();
                    cartesianPoint.Coordinate = coordinates.ToArray();
                    closedShell.PolyLoops[i].Points[j] = cartesianPoint;
                }
            }

            return space;
        }
    }
}
