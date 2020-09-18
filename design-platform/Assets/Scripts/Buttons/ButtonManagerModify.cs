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
    private List <int> verticalFaceArray;

    public void Move()
    {
        Debug.Log("Move function is not implemented");
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

        // Get the GameObject of the selected room
        //ProBuilderMesh pb = (ProBuilderMesh)main.modifyMode.selectedRoom.gameObject.GetComponent(typeof(ProBuilderMesh));
        ProBuilderMesh pb = main.modifyMode.selectedRoom.mesh3D;

        verticalFaceArray = new List<int>();
        // Faces without horizontal faces
        //faceArray.
        for (int i = 0; i < pb.faces.Count; i++)
        {
            if (Math.Normal(pb, pb.faces[i]).y == 0)
            {
                verticalFaceArray.Add(i);
                //Debug.Log("weeed");
            }
        }

        // set handels on extrudeable faces




        //pb.SetFaceColor(pb.faces[2], Color.red);
        //pb.GetNormals()


        // Face to extrude


        // Distance from original face to mouse pointer projected onto the selected faces normal vector


        // Extrude

        //Vælger de faces i pb.faces der er på indices i listen verticalFaceArry
        //verticalFaceArray.Select(i => pb.faces[i]);

        pb.Extrude( verticalFaceArray.Select(i => pb.faces[i]) , ExtrudeMethod.IndividualFaces, 1);
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
