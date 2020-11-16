using UnityEngine;

public class customGrid : MonoBehaviour {
    public GameObject placedObject;
    public float gridSize;
    Plane basePlane = new Plane(Vector3.up, Vector3.zero);
    Vector3 gridPosition;
    Vector3 hitPoint;

    void Start() {
    }

    void Update() {
        //create new ray from camera mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Check ray plane collision and save distance
        float distance;
        //if (Physics.Raycast(ray, out hit))
        if (basePlane.Raycast(ray, out distance)) {
            // Find world coordinates of ray plane collision
            hitPoint = ray.GetPoint(distance);
        }

        gridPosition.x = Mathf.Round(hitPoint[0] / gridSize) * gridSize;
        gridPosition.y = Mathf.Round(hitPoint[1] / gridSize) * gridSize;
        gridPosition.z = Mathf.Round(hitPoint[2] / gridSize) * gridSize;

        placedObject.transform.position = gridPosition;
    }
}
