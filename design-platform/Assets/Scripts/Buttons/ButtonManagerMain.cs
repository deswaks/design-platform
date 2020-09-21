using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagerMain : MonoBehaviour
{
    public PdfExport pdfExport;

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void Save()
    {

        pdfExport.PrintRooms();
        //drawingExporter.popUpDrawing();
        //Debug.Log("Save function is not implemented");
    }

    public void Load()
    {
        Debug.Log("Load function is not implemented");
    }

}
