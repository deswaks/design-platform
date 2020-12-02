using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace DesignPlatform.Core {
    public partial class Building {

        /// <summary> All walls of this building </summary>
        private List<Wall> walls = new List<Wall>();

        /// <summary>
        /// All wall objects of this building.
        /// </summary>
        public List<Wall> Walls {
            get {
                if (walls == null || walls.Count == 0) BuildAllWalls();
                return walls;
            }
        }

        /// <summary>
        /// Build a 3d wall representation
        /// </summary>
        /// <param name="interFace">Interface to build wall on.</param>
        /// <returns>The newly built wall.</returns>
        public Wall BuildWall(Interface interFace) {
            GameObject newWallGameObject = new GameObject("Wall");
            Wall newWall = (Wall)newWallGameObject.AddComponent(typeof(Wall));
            newWall.InitializeWall(interFace);
            walls.Add(newWall);
            return newWall;
        }

        /// <summary>
        /// Build a 3D wall representation.
        /// </summary>
        /// <param name="cltElement">CLT element to base the wall upon</param>
        /// <returns>The newly built wall.</returns>
        public Wall BuildWall(CLTElement cltElement) {

            // Create and initialize the new wall
            GameObject newWallGameObject = new GameObject("Wall");
            Wall newWall = (Wall)newWallGameObject.AddComponent(typeof(Wall));
            newWall.InitializeWall(cltElement);

            // Add to managed list
            walls.Add(newWall);

            return newWall;
        }

        /// <summary>
        /// Builds walls for all CLT elements / vertical interfaces of the whole building as specified in the program settings.
        /// </summary>
        /// <returns>All walls of the building.</returns>
        public List<Wall> BuildAllWalls() {
            // Make sure that openings are connected correctly
            Openings.ForEach(o => o.AttachClosestFaces());

            // Build the walls
            if (Settings.WallSource == WallSource.CLT_ELEMENT) {
                CLTElements.ForEach(cltElement => BuildWall(cltElement));
            }
            if (Settings.WallSource == WallSource.INTERFACE) {
                InterfacesVertical.ForEach(interFace => BuildWall(interFace));
            }
            
            return Walls;
        }

        /// <summary>
        /// Removes all walls of the whole building.
        /// </summary>
        public void DeleteAllWalls() {
            int amount = walls.Count;
            if (amount > 0) {
                for (int i = 0; i < amount; i++) {
                    walls[0].Delete();
                }
            }
        }

        /// <summary>
        /// Removes a wall from the managed building list.
        /// </summary>
        /// <param name="wall">Wall object to remove.</param>
        public void RemoveWall(Wall wall) {
            if (walls.Contains(wall)) walls.Remove(wall);
        }


    }
}
