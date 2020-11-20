using DesignPlatform.Core;
using gbXMLSerializer;
using System.Collections.Generic;
using UnityEngine;
using gbs = gbXMLSerializer;

namespace DesignPlatform.Export {
    public static class GbxmlConverter {

        public static gbs.Space XmlSpaceFromRoom(Room room) {

            // Create space
            gbs.Space space = new gbs.Space();
            space.id = "Space_";// + room.name;
            space.Name = "Space_";// + room.name;

            // Space area
            Area area = new Area();
            area.val = string.Format("{0:N2}", room.GetFloorArea());
            space.spacearea = area;

            // Space volume
            Volume vol = new Volume();
            vol.val = string.Format("{0:N2}", room.GetVolume());
            space.spacevol = vol;

            // Shell geometry
            ShellGeometry shellGeometry = new ShellGeometry();
            shellGeometry.unit = lengthUnitEnum.Meters;
            shellGeometry.id = "sg" + space.Name;
            space.ShellGeo = shellGeometry;

            // Closed shell
            ClosedShell closedShell = new ClosedShell();
            shellGeometry.ClosedShell = closedShell;

            List<List<Vector3>> surfacesVertices = room.GetSurfaceVertices();

            // Make polyloop arrays
            closedShell.PolyLoops = prod.makePolyLoopArray(surfacesVertices.Count);
            for (int i = 0; i < closedShell.PolyLoops.GetLength(0); i++) {

                // Make polyloop points
                closedShell.PolyLoops[i].Points = BasicSerialization.makeCartesianPtArray(surfacesVertices[i].Count);

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
