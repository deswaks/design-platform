using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace DesignPlatform.Core {
    public partial class Building {


        /// <summary>
        /// All the interfaces of this building
        /// </summary>
        public List<Interface> Interfaces {
            get { return Spaces.SelectMany(r => r.Interfaces).Distinct().ToList(); }
        }

        /// <summary>
        /// All the vertical interfaces of this building
        /// </summary>
        public List<Interface> InterfacesVertical {
            get { return Interfaces.Where(i => i.Orientation == Orientation.VERTICAL).ToList(); }
            private set {; }
        }

        /// <summary>
        /// All the horizontal interfaces of this building
        /// </summary>
        public List<Interface> InterfacesHorizontal {
            get { return Interfaces.Where(i => i.Orientation == Orientation.HORIZONTAL).ToList(); }
            private set {; }
        }

        /// <summary>
        /// Builds interfaces for all faces of the whole building.
        /// </summary>
        /// <returns>All interfaces of the building.</returns>
        public List<Interface> BuildAllInterfaces() {
            for (int r = 0; r < Spaces.Count; r++) {
                for (int f = 0; f < Spaces[r].Faces.Count; f++) {
                    Face face = Spaces[r].Faces[f];
                    BuildInterfaces(face);
                }
            }
            return Interfaces;
        }

        /// <summary>
        /// Build all the interfaces connected to the given face.
        /// </summary>
        /// <param name="face">Face to build interfaces for.</param>
        public void BuildInterfaces(Face face) {
            // Slab interface
            if (face.Orientation == Orientation.HORIZONTAL) {
                new Interface(face);
            }
            // Wall interface
            if (face.Orientation == Orientation.VERTICAL) {
                // Find points on face line from the controlpoints of all other spaces
                List<Vector3> splitPoints = new List<Vector3> { face.LocationLine.StartPoint, face.LocationLine.EndPoint };
                for (int r2 = 0; r2 < Spaces.Count; r2++) {
                    if (face.Space == Spaces[r2]) continue;
                    foreach (Vector3 point in Spaces[r2].GetControlPoints()) {
                        if (face.LocationLine.Intersects(point)) {
                            splitPoints.Add(point);
                        }
                    }
                }
                // Sort splitpoints between startpoint and endpoint
                splitPoints = splitPoints.OrderBy(p => face.LocationLine.ParameterAtPoint(p)).ToList();
                List<float> splitParameters = splitPoints.Select(p => face.LocationLine.ParameterAtPoint(p)).ToList();

                // Hvert interface-sted
                for (int i = 0; i < splitParameters.Count - 1; i++) {
                    Geometry.Line newInterfaceLine = new Geometry.Line(splitPoints[i], splitPoints[i + 1]);

                    // Check if an interface exists with the same points
                    Interface duplicateInterface = null;
                    foreach (Interface buildingInterface in Interfaces.Where(inte => inte.Orientation == Orientation.VERTICAL)) {
                        if (Vector3.Distance(buildingInterface.StartPoint, newInterfaceLine.StartPoint) < 0.001
                            && Vector3.Distance(buildingInterface.EndPoint, newInterfaceLine.EndPoint) < 0.001
                            || Vector3.Distance(buildingInterface.StartPoint, newInterfaceLine.EndPoint) < 0.001
                            && Vector3.Distance(buildingInterface.EndPoint, newInterfaceLine.StartPoint) < 0.001) {
                            duplicateInterface = buildingInterface;
                        }
                    }

                    // Attach to existing interface or create new
                    if (duplicateInterface == null) {
                        new Interface(face, splitParameters[i], splitParameters[i + 1]);
                    }
                    else {
                        duplicateInterface.AttachFace(face, splitParameters[i], splitParameters[i + 1]);
                    }
                }
            }
        }

        /// <summary>
        /// Removes all interface objects of the whole building.
        /// </summary>
        public void DeleteAllInterfaces() {
            foreach (Interface interFace in Interfaces) {
                interFace.Delete();
            }
        }


    }
}
