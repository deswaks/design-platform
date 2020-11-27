using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace DesignPlatform.Core {
    public partial class Building {

        /// <summary>
        /// All the openings of this building
        /// </summary>
        public List<Opening> Openings {
            get { return Spaces.SelectMany(r => r.Openings).Distinct().ToList(); }
        }

        /// <summary>
        /// Build an opening.
        /// </summary>
        /// <param name="function">Function of the new opening.</param>
        /// <param name="preview">Set whether the new opening should be used for preview or be added to the model.</param>
        /// <returns></returns>
        public Opening BuildOpening(OpeningFunction function,
                                    bool preview = false) {
            return BuildOpening(function: function, position: Vector3.zero,
                                rotation: Quaternion.identity, preview: preview);
        }
        /// <summary>
        /// Build an opening with the given transform.
        /// </summary>
        /// <param name="function">Function of the new opening.</param>
        /// <param name="position">Position of the new opening</param>
        /// <param name="rotation">Rotation of the new opening</param>
        /// <param name="preview">Set whether the new opening should be used for preview or be added to the model.</param>
        /// <returns></returns>
        public Opening BuildOpening(OpeningFunction function,
                                    Vector3 position,
                                    Quaternion rotation,
                                    bool preview = false) {
            // Create game object
            GameObject newOpeningGameObject = new GameObject("Opening");
            newOpeningGameObject.transform.position = position;
            newOpeningGameObject.transform.rotation = rotation;

            // Create opening component
            Opening newOpening = (Opening)newOpeningGameObject.AddComponent(typeof(Opening));
            if (preview) newOpening.InitializeOpening(function: function, state: OpeningState.PREVIEW);
            else newOpening.InitializeOpening(function: function, state: OpeningState.PLACED);

            return newOpening;
        }
        /// <summary>
        /// Build an opening with the transform of the given template.
        /// </summary>
        /// <param name="function">Function of the new opening.</param>
        /// <param name="templateOpening">Template opening to use the transform from</param>
        /// <param name="preview">Set whether the new opening should be used for preview or be added to the model.</param>
        /// <returns></returns>
        public Opening BuildOpening(OpeningFunction function,
                                    Opening templateOpening = null,
                                    bool preview = false) {
            return BuildOpening(function: function, position: templateOpening.transform.position,
                                rotation: templateOpening.transform.rotation, preview: preview);
        }
    }
}
