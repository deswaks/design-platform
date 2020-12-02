using DesignPlatform.Core;
using gbXMLSerializer;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DesignPlatform.Export {
    public static class GbxmlConverter {

        public static gbXMLSerializer.Space XmlSpaceFromSpace(Core.Space space) {

            // Create space
            gbXMLSerializer.Space xmlSpace = new gbXMLSerializer.Space();
            xmlSpace.id = "Space_";// + room.name;
            xmlSpace.Name = "Space_";// + room.name;

            // Space area
            Area area = new Area();
            area.val = string.Format("{0:N2}", space.Area);
            xmlSpace.spacearea = area;

            // Space volume
            Volume vol = new Volume();
            vol.val = string.Format("{0:N2}", space.Volume);
            xmlSpace.spacevol = vol;

            // Shell geometry
            ShellGeometry shellGeometry = new ShellGeometry();
            shellGeometry.unit = lengthUnitEnum.Meters;
            shellGeometry.id = "sg" + xmlSpace.Name;
            xmlSpace.ShellGeo = shellGeometry;

            // Closed shell
            ClosedShell closedShell = new ClosedShell();
            shellGeometry.ClosedShell = closedShell;

            List<List<Vector3>> surfacesVertices = GetSpaceSurfaceVertices(space);

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<List<Vector3>> GetSpaceSurfaceVertices(Core.Space space) {
            List<List<Vector3>> surfacesVertices = new List<List<Vector3>>();
            List<Vector3> vertices;

            // Add Floor surface
            vertices = space.GetControlPoints().Select(p => new Vector3(p.x, p.y, p.z)).ToList();
            surfacesVertices.Add(vertices);

            // Add wall surfaces
            int j = space.ControlPoints.Count - 1;
            for (int i = 0; i < space.ControlPoints.Count; i++) {
                vertices = new List<Vector3> {
                    space.ControlPoints[i],
                    space.ControlPoints[i] + new Vector3(0, space.Height, 0),
                    space.ControlPoints[j] + new Vector3(0, space.Height, 0),
                    space.ControlPoints[j]
                };
                surfacesVertices.Add(vertices);
                j = i;
            }

            // Add Ceiling vertices
            vertices = space.GetControlPoints().Select(p => new Vector3(p.x, p.y + space.Height, p.z)).ToList();
            surfacesVertices.Add(vertices);

            return surfacesVertices;
        }
    }
}
