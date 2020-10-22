using UnityEngine;

public static class Grid {
    [SerializeField]
    public static float size = 2f;

    public static Vector3 GetNearestGridpoint(Vector3 position) {
        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = Mathf.RoundToInt(position.z / size);

        Vector3 result = new Vector3(
            (float)xCount * size,
            (float)yCount * size,
            (float)zCount * size);

        return result;
    }
}