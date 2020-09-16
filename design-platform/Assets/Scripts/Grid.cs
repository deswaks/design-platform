using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private float size = 2f;

    public Vector3 GetNearestGridpoint(Vector3 position)
    {
        position -= transform.position;

        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = Mathf.RoundToInt(position.z / size);

        Vector3 result = new Vector3(
            (float)xCount * size,
            (float)yCount * size,
            (float)zCount * size);

        result += transform.position;

        return result;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f,0.0f,0.0f,0.5f);
        for (float x = 0; x < 40; x += size)
        {
            for (float z = 0; z < 40; z += size)
            {
                var point = GetNearestGridpoint(new Vector3(x, 0f, z));
                Gizmos.DrawSphere(point, 0.07f);
            }
        }
    }
}