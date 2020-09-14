using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateWall : MonoBehaviour
{
    bool creating;
    ShowMousePosistion pointer;
    public GameObject cornerPrefab;
    public GameObject wallPrefab;
    GameObject lastCorner;

    // Start is called before the first frame update
    void Start()
    {
        pointer = GetComponent<ShowMousePosistion>();
    }

    // Update is called once per frame
    void Update()
    {
        getInput();
    }

    void getInput()
    {
        if (Input.GetMouseButtonDown(0)) {
            startWall();
        } else if (Input.GetMouseButtonUp(0)) {
            setWall();
        } else {
            if (creating) {
                updateWall();
            }   
        }
    }
    void startWall() {
        creating = true;
        Vector3 startPos = pointer.getWorldPoint();
        startPos = pointer.snapPosition(startPos);
        GameObject startCorner = Instantiate(cornerPrefab, startPos, Quaternion.identity);
        startCorner.transform.position = new Vector3(startPos.x, startPos.y + 0.3f, startPos.z);
        lastCorner = startCorner;
    }
    void setWall() {
        creating = false;
    }
    void updateWall() {
        Vector3 current = pointer.getWorldPoint();
        current = pointer.snapPosition(current);
        current = new Vector3(current.x, current.y + 0.3f, current.z);
        if (!current.Equals(lastCorner.transform.position))
        {
            createWallSegment(current);
        }
    }

    void createWallSegment(Vector3 current)
    {
        GameObject newCorner = Instantiate(cornerPrefab, current, Quaternion.identity);
        Vector3 middle = Vector3.Lerp(newCorner.transform.position, lastCorner.transform.position, 0.5f);
        GameObject newWall = Instantiate(wallPrefab, middle, Quaternion.identity);
        newWall.transform.LookAt(lastCorner.transform);
        lastCorner = newCorner;
    }
}
