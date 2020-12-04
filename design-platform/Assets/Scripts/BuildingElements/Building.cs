using DesignPlatform.Geometry;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace DesignPlatform.Core {

    /// <summary>
    /// The building class is responsible for creation of new building elements
    /// and functions as a general access point for all created elements.
    /// </summary>
    public partial class Building {

        /// <summary> Main building instance </summary>
        private static Building instance;

        /// <summary>
        /// Main building instance. All access to the building class should use this single instance.
        /// </summary>
        public static Building Instance {
            get { return instance ?? (instance = new Building()); }
        }
        
        /// <summary>
        /// All the faces of this building
        /// </summary>
        public List<Face> Faces {
            get { return Spaces.SelectMany(r => r.Faces).Distinct().ToList(); }
        }
        
        /// <summary>
        /// The boundaries of the building in the X and Y axis on the form { minX, maxX, minY, maxY }
        /// </summary>
        public List<float> Bounds {
            get {
                float minX = 0; float maxX = 0;
                float minY = 0; float maxY = 0;
                foreach (Space space in Spaces) {
                    foreach (Vector3 controlPoint in space.GetControlPoints()) {
                        if (controlPoint[0] < minX) { minX = controlPoint[0]; }
                        if (controlPoint[0] > maxX) { minX = controlPoint[0]; }
                        if (controlPoint[2] < minY) { minX = controlPoint[2]; }
                        if (controlPoint[2] > maxY) { minX = controlPoint[2]; }
                    }
                }
                return new List<float> { minX, maxX, minY, maxY };
            }
        }

        /// <summary>
        /// Destroys and all elements with a 3D representation (visible in POV mode).
        /// </summary>
        public void RebuildPOVElements() {
            if (Spaces == null || Spaces.Count == 0) return;

            // Delete preexisting	
            DeleteAllWalls();
            DeleteAllSlabs();
            DeleteAllInterfaces();
            DeleteAllCLTElements();
            DeleteAllRoofElements();

            // Build new
            BuildAllInterfaces();
            BuildAllCLTElements();
            BuildAllWalls();
            BuildAllRoofElements();
            BuildAllSlabs();
        }

    }
}