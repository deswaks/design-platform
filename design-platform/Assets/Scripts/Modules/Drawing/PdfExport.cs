﻿using System.Diagnostics;
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

    public static Dictionary<string, float> Scales = new Dictionary<string, float>() {
        {"1:50", 56.692f},
        {"1:100", 28.346f}
    };
    public static float scale = Scales["1:100"];

    /// <summary>
    /// Prints all the rooms of the building to a pdf file
    /// </summary>
    public static void ExportPlan() {
        // Create a new PDF document with an empty page
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        page.Size = PageSize.A3;
        page.Orientation = PageOrientation.Landscape;

        // Get an XGraphics and xFont objects for drawing
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

        // Draw
        DrawRooms(gfx);

        // Save the document
        document.Save("Plan.pdf");
        Process.Start("Plan.pdf"); //Start viewer
    }

    /// <summary>
    /// Draws each room of the building using the given XGraphics object
    /// </summary>
    private static void DrawRooms(XGraphics gfx) {
        // Definition of Pens and brushes
        XSolidBrush roomBrush = new XSolidBrush(XColors.BlanchedAlmond);
        XPen wallPen1 = new XPen(XColors.Black, 2);
        XPen wallPen2 = new XPen(XColors.Black, 6);
        wallPen2.LineCap = XLineCap.Square;

        foreach (Room room in Building.Instance.GetRooms()) {
            drawRoomPolygon(gfx, roomBrush, room);
            drawWallLines(gfx, wallPen1, wallPen2, room);
        }
    }

    private static void drawRoomPolygon(XGraphics gfx, XSolidBrush roomBrush, Room room) {
        double centerX = gfx.PageSize.Width / 2;
        double centerY = gfx.PageSize.Height / 2;
        List<XPoint> polylinePoints = room.GetControlPoints(closed: true).Select(p => new XPoint(p.x * scale + centerX, -p.z * scale + centerY)).ToList();
        gfx.DrawPolygon(roomBrush, polylinePoints.ToArray(), XFillMode.Alternate);
    }

    private static void drawWallLines(XGraphics gfx, XPen penArchitectural, XPen penStructural, Room room) {
        double centerX = gfx.PageSize.Width / 2;
        double centerY = gfx.PageSize.Height / 2;
        
        Dictionary<int, List<Structural.Load>> loadTable = Structural.LoadDistribution.AreaLoad(room);
        List<int> structuralWalls = loadTable.Keys.ToList();
        List<XPoint> drawPoints = room.GetControlPoints(closed: true).Select(p => new XPoint(p.x * scale + centerX, -p.z * scale + centerY)).ToList();

        // Draw each wall using architectural or structural brush according to whether the wall is in load list
        for (int iWall = 0; iWall < drawPoints.Count-1; iWall++) {
            if (structuralWalls.Contains(iWall)) {
                gfx.DrawLine(penStructural, drawPoints[iWall], drawPoints[iWall + 1]);
            }
            else {
                gfx.DrawLine(penArchitectural, drawPoints[iWall], drawPoints[iWall + 1]);
            }
        }
    }
}
