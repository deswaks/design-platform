using DesignPlatform.Core;
using gbXMLSerializer;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Export {
    public static class GbxmlConverter {

        public static gbXMLSerializer.Space XmlSpaceFromSpace(Core.Space space) {

            // Create space
            gbXMLSerializer.Space xmlSpace = new gbXMLSerializer.Space();
            xmlSpace.id = "Space_";// + room.name;
            xmlSpace.Name = "Space_";// + room.name;

            // Space area
            Area area = new Area();
            area.val = string.Format("{0:N2}", space.GetFloorArea());
            xmlSpace.spacearea = area;

            // Space volume
            Volume vol = new Volume();
            vol.val = string.Format("{0:N2}", space.GetVolume());
            xmlSpace.spacevol = vol;

            // Shell geometry
            ShellGeometry shellGeometry = new ShellGeometry();
            shellGeometry.unit = lengthUnitEnum.Meters;
            shellGeometry.id = "sg" + xmlSpace.Name;
            xmlSpace.ShellGeo = shellGeometry;

            // Closed shell
            ClosedShell closedShell = new ClosedShell();
            shellGeometry.ClosedShell = closedShell;

            List<List<Vector3>> surfacesVertices = space.GetSurfaceVertices();

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

            return xmlSpace;
        }
    }
}
