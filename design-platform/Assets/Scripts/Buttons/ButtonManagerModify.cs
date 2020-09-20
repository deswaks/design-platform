using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class ButtonManagerModify : MonoBehaviour
{
    public Main main;
    // Tilføjet ABC
    private List <int> verticalFace;
    private List<int> baseFace;
    private List<Vector3> baseFaceCorners;
    private Vector3 facePos;
    //public GameObject moveHandlePrefab;
    //private GameObject moveHandle;
    //private Room prefabRoom;



    public void Move()
    {
        main.modifyMode.selectedRoom.SetIsInMoveMode(isInMoveMode: true);
    }
    public void Rotate()
    {
        if (main.modifyMode.selectedRoom != null)
        {
            main.modifyMode.selectedRoom.Rotate();
        }
    }
    public void Modify()
    {
        //-----------------------------------------------------------------------------------------------------------------
        // Get the GameObject of the selected room
        ProBuilderMesh pb = (ProBuilderMesh)main.modifyMode.selectedRoom.gameObject.GetComponent(typeof(ProBuilderMesh));
        //ProBuilderMesh pb = main.modifyMode.selectedRoom.mesh3D;

        //-----------------------------------------------------------------------------------------------------------------
        // Two lists of idecies for:  1) Vertical faces 2) The base face

        verticalFace = new List<int>();
        baseFace = new List<int>();

        for (int i = 0; i < pb.faces.Count; i++)
        {
            if (Math.Normal(pb, pb.faces[i]).y == 0)
            {
                verticalFace.Add(i);
            }
            else if (Math.Normal(pb, pb.faces[i]).y == -1)
            {
                baseFace.Add(i);
            }
        }

        // List of vertical faces
        verticalFace.Select(i => pb.faces[i]);
        // List with base face
        baseFace.Select(i => pb.faces[i]);

        //-----------------------------------------------------------------------------------------------------------------
        // Get base face corners
        baseFaceCorners = new List<Vector3>();
        foreach (int index in pb.faces[baseFace[0]].distinctIndexes)
        {
            baseFaceCorners.Add(pb.positions[index]);
        }

        //Debug.Log(baseFaceCorners.Count);

        //-----------------------------------------------------------------------------------------------------------------
        // set handels on extrudeable faces

        //for(int i = 0; i < verticalFace.Count; i ++)
        //{
        //    moveHandle = Instantiate(prefabRoom.moveHandlePrefab);
        //    Vector3 handlePosition = FaceCenter(pb, verticalFace[i]);
        //    //handlePosition.y = height + 0.01f;
        //    moveHandle.transform.position = handlePosition;

        //    moveHandle.transform.SetParent(gameObject.transform, true);
        //}


        // Method for the average point of a face

        /**
     *	Returns the average of each vertex position in a face.
     *	In local space.
     */
        Vector3 FaceCenter(ProBuilderMesh proB, Face face)
        {
            var vertices = proB.positions;

            Vector3 average = Vector3.zero;

            // face holds triangle data.  distinctIndices is a
            // cached collection of the distinct indices that
            // make up the triangles. Ex:
            // tris = {0, 1, 2, 2, 3, 0}
            // distinct indices = {0, 1, 2, 3}
            foreach (int index in face.distinctIndexes)
            {
                average.x += vertices[index].x;
                average.y += vertices[index].y;
                average.z += vertices[index].z;
            }

            float len = (float)face.distinctIndexes.Count;

            average.x /= len;
            average.y /= len;
            average.z /= len;

            return average;
        }



        Vector3 faceCenter = FaceCenter(pb, pb.faces[2]);





        //-----------------------------------------------------------------------------------------------------------------
        // Face to extrude


        //-----------------------------------------------------------------------------------------------------------------
        // Distance from original face to mouse pointer projected onto the selected faces normal vector


        //-----------------------------------------------------------------------------------------------------------------
        // Extrude

        //Vælger de faces i pb.faces der er på indices i listen verticalFaceArry
        //verticalFaceArray.Select(i => pb.faces[i]);

        pb.Extrude( verticalFace.Select(i => pb.faces[i]) , ExtrudeMethod.IndividualFaces, 1);
        pb.ToMesh();
        pb.Refresh();

    }
    public void Properties()
    {
        Debug.Log("Properties function is not implemented");
    }
    public void Delete()
    {
        if (main.modifyMode.selectedRoom != null)
        {
            main.modifyMode.selectedRoom.Delete();
        }
    }
}
