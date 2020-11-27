using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace DesignPlatform.Core {
    public partial class Building {

        /// <summary>
        /// All spaces of this building.
        /// </summary>
        public List<Space> Spaces { get; private set; } = new List<Space>();


        /// <summary>
        /// Builds a new space and adds it to the managed building list.
        /// </summary>
        /// <param name="buildShape">Defines the shape of the new space.</param>
        /// <param name="preview">Decides whether the new space should be for preview or placed in model.</param>
        /// <param name="templateSpace">Optionally profile a template object from which location and rotation will be copied to the new space.</param>
        /// <returns></returns>
        public Space BuildSpace(SpaceShape buildShape = SpaceShape.RECTANGLE, bool preview = false, Space templateSpace = null) {
            GameObject newSpaceGameObject = new GameObject("Space");
            Space newSpace = (Space)newSpaceGameObject.AddComponent(typeof(Space));

            if (preview) {
                newSpace.InitSpace(buildShape: buildShape, building: this, type: SpaceFunction.PREVIEW);
            }
            else {
                newSpace.InitSpace(buildShape: buildShape, building: this, type: SpaceFunction.DEFAULT);
                if (templateSpace != null) {
                    newSpaceGameObject.transform.position = templateSpace.transform.position;
                    newSpaceGameObject.transform.rotation = templateSpace.transform.rotation;
                }
            }
            if (preview == false) { Spaces.Add(newSpace); }
            return newSpace;
        }

        /// <summary>
        /// Removes a space from the managed building list.
        /// </summary>
        /// <param name="space">Space object to remove.</param>
        public void RemoveSpace(Space space) {
            if (Spaces.Contains(space)) { Spaces.Remove(space); }
        }


    }
}
