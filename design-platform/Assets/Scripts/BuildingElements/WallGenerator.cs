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
                if (walls == null || walls.Count == 0) BuildAllWallsAsCLTElements();
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
        /// Builds walls for all vertical interfaces of the whole building.
        /// </summary>
        /// <returns>All walls of the building.</returns>
        public List<Wall> BuildAllWalls() {
            foreach (Opening opening in Openings) {
                opening.AttachClosestFaces();
            }
            foreach (Interface interFace in InterfacesVertical) {
                BuildWall(interFace);
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
