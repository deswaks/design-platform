using UnityEngine;

namespace DesignPlatform.Core {
    public static class Grid {
        [SerializeField]
        public static float size = 1f;
        public static float subSize = size / 10f;

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