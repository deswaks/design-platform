using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace DesignPlatform.Core {
    public partial class Building {

        /// <summary> All slabs of this building </summary>
        private List<Slab> slabs = new List<Slab>();

        /// <summary>
        /// All slab objects of this building
        /// </summary>
        public List<Slab> Slabs {
            get {
                if (slabs.Count == 0) BuildAllSlabs();
                return slabs;
            }
        }


        /// <summary>
        /// Builds slabs for all horizontal interfaces of the whole building.
        /// </summary>
        /// <returns>All walls of the building.</returns>
        public List<Slab> BuildAllSlabs() {
            foreach (Interface interFace in InterfacesHorizontal) BuildSlab(interFace);
            return Slabs;
        }

        /// <summary>
        /// Build a 3D slab representation.
        /// </summary>
        /// <param name="interFace">Interface to build slab on.</param>
        /// <returns>The newly built slab.</returns>
        public Slab BuildSlab(Interface interFace) {
            GameObject newSlabGameObject = new GameObject("slab");
            Slab newSlab = (Slab)newSlabGameObject.AddComponent(typeof(Slab));
            newSlab.InitializeSlab(interFace);
            slabs.Add(newSlab);
            return newSlab;
        }

        /// <summary>
        /// Removes all slabs of the whole building.
        /// </summary>
        public void DeleteAllSlabs() {
            int amount = slabs.Count;
            if (amount > 0) {
                for (int i = 0; i < amount; i++) {
                    slabs[0].DeleteSlab();
                }
            }
        }

        /// <summary>
        /// Removes slab from the managed building list.
        /// </summary>
        /// <param name="slab">Slab object to remove.</param>
        public void RemoveSlab(Slab slab) {
            if (slabs.Contains(slab)) slabs.Remove(slab);
        }
    }
}
