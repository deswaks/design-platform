﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagerMain : MonoBehaviour
{
    public void QuitApplication()
    {
        Application.Quit();
    }

    public void ExportPDF() {
        PdfExport.ExportPlan();
        //drawingExporter.popUpDrawing();
        //Debug.Log("Save function is not implemented");
    }

    public void ExportGbXML() {
        gbXML.Exporter.Export();
    }

    public void Save()
    {
        Debug.Log("Save function is not implemented");
    }

    public void Load()
    {
        Debug.Log("Load function is not implemented");
    }

}
