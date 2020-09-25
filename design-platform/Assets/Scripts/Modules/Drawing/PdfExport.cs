using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using UnityEngine;
using System;
using System.Linq;

public static class PdfExport {

    public static Dictionary<string, float> scale = new Dictionary<string, float>() {
        {"1:50", 56.692f},
        {"1:100", 28.346f}
    };

    /// <summary>
    /// Prints all the rooms of the building to a pdf file
    /// </summary>
    public static void PrintRooms() {
        // Create a new PDF document with an empty page
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        page.Size = PageSize.A3;
        page.Orientation = PageOrientation.Landscape;

        // Get an XGraphics and xFont objects for drawing
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

        // Draw
        DrawRooms(gfx, scale["1:100"]);

        // Save the document
        document.Save("Plan.pdf");
        Process.Start("Plan.pdf"); //Start viewer
    }

    /// <summary>
    /// Draws each room of the building using the given XGraphics object
    /// </summary>
    private static void DrawRooms(XGraphics gfx, float scale) {
        double pageWidth = gfx.PageSize.Width;
        double pageHeight = gfx.PageSize.Height;

        // Room pen
        XPen pen = new XPen(XColors.Black, 2);
        pen.LineCap = XLineCap.Round;
        pen.LineJoin = XLineJoin.Bevel;

        foreach (Room room in Building.Instance.GetRooms()) {
            List<XPoint> polylinePoints = new List<XPoint>();

            // Add all control points
            foreach (Vector3 controlPoint in room.GetControlPoints()) {
                XPoint newPoint = new XPoint(
                    controlPoint[0] * scale + gfx.PageSize.Width/2,
                    -controlPoint[2] * scale + gfx.PageSize.Height/2);
                polylinePoints.Add(newPoint);
            }
            polylinePoints.Add(polylinePoints[0]); // Add first controlpoint again            
            
            gfx.DrawLines(pen, polylinePoints.ToArray());
        }
    }
}
