using UnityEngine;

namespace DesignPlatform.Core {
    public static class Grid {
        [SerializeField]
        public static float size = 1f;

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
            float xCount = Mathf.RoundToInt(position.x / size)/10f;
            float yCount = Mathf.RoundToInt(position.y / size)/10f;
            float zCount = Mathf.RoundToInt(position.z / size)/10f;

            Vector3 result = new Vector3(
                (xCount * size)*10f,
                (yCount * size)*10f,
                (zCount * size)*10f);

            return result;
        }
    }
}