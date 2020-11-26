using UnityEngine;

namespace DesignPlatform.Core {

    /// <summary>
    /// The underlying building grid on which all building elements are placed.
    /// </summary>
    public static class Grid {

        //[SerializeField]

        public static float size = 1f;
        public static float subSize = size / 10f;

        /// <summary>
        /// Finds the nearest location on the grid.
        /// </summary>
        /// <param name="position">Point to find grid location for.</param>
        /// <returns>Closest grid location.</returns>
        public static Vector3 GetNearestGridpoint(Vector3 position) {
            int xCount = Mathf.RoundToInt(position.x / size);
            int yCount = Mathf.RoundToInt(position.y / size);
            int zCount = Mathf.RoundToInt(position.z / size);

            Vector3 result = new Vector3(
                xCount * size,
                yCount * size,
                zCount * size);

            return result;
        }

        /// <summary>
        /// Finds the nearest location on the subgrid which is 1/10 the size of the main grid.
        /// </summary>
        /// <param name="position">Point to find subgrid location for.</param>
        /// <returns>Closest subgrid location.</returns>
        public static Vector3 GetNearestSubGridpoint(Vector3 position) {
            float xCount = Mathf.RoundToInt(position.x / subSize);
            float yCount = Mathf.RoundToInt(position.y / subSize);
            float zCount = Mathf.RoundToInt(position.z / subSize);

            Vector3 result = new Vector3(
                xCount * subSize,
                yCount * subSize,
                zCount * subSize);

            return result;
        }
    }
}