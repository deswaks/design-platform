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
using DesignPlatform.Core;
using StructuralAnalysis;

namespace DesignPlatform.PdfExport {
    public static class PdfExporter {

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
            TagRooms(gfx);

            // Save the document
            document.Save("Exports/Plan.pdf");
            Process.Start("Exports\\Plan.pdf"); //Start viewer
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
            }
            foreach (Room room in Building.Instance.GetRooms()) {
                drawWallLines(gfx, wallPen1, wallPen2, room);
            }
        }

        private static void TagRooms(XGraphics gfx) {
            // Font definitions
            const string facename = "Times New Roman";
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
            XFont nameFont = new XFont(facename, 12, XFontStyle.Bold, options);
            XFont areaFont = new XFont(facename, 10, XFontStyle.Regular, options);

            XStringFormat tagFormat = new XStringFormat();
            tagFormat.Alignment = XStringAlignment.Center;

            foreach (Room room in Building.Instance.GetRooms()) {
                WriteRoomName(gfx, nameFont, room, tagFormat);
                WriteRoomArea(gfx, areaFont, room, tagFormat);
            }
        }

        private static void WriteRoomName(XGraphics gfx, XFont tagFont, Room room, XStringFormat tagFormat) {
            XPoint tagLocation = WorldToPaper(gfx, room.GetTagLocation());
            string roomText = room.gameObject.GetComponent<MeshRenderer>().material.ToString().Split(' ')[0].Split('_')[1];
            string tagText = roomText.Substring(4, roomText.Length - 4);
            gfx.DrawString(tagText, tagFont, XBrushes.Black, tagLocation.X, tagLocation.Y - tagFont.Size / 2 - 2, tagFormat);
        }

        private static void WriteRoomArea(XGraphics gfx, XFont tagFont, Room room, XStringFormat tagFormat) {
            XPoint tagLocation = WorldToPaper(gfx, room.GetTagLocation());
            string tagText = room.GetFloorArea().ToString() + "m²";
            gfx.DrawString(tagText, tagFont, XBrushes.Black, tagLocation.X, tagLocation.Y + tagFont.Size / 2 + 2, tagFormat);
        }

        private static void drawRoomPolygon(XGraphics gfx, XSolidBrush roomBrush, Room room) {
            double centerX = gfx.PageSize.Width / 2;
            double centerY = gfx.PageSize.Height / 2;
            List<XPoint> polylinePoints = room.GetControlPoints(closed: true).Select(p => new XPoint(p.x * scale + centerX, -p.z * scale + centerY)).ToList();
            gfx.DrawPolygon(roomBrush, polylinePoints.ToArray(), XFillMode.Alternate);
        }

        private static void drawWallLines(XGraphics gfx, XPen penArchitectural, XPen penStructural, Room room) {


            Dictionary<int, List<Load>> loadTable = LoadDistribution.AreaLoad(room);
            List<int> structuralWalls = loadTable.Keys.ToList();
            List<XPoint> drawPoints = room.GetControlPoints(closed: true).Select(p => WorldToPaper(gfx, p)).ToList();

            // Draw each wall using architectural or structural brush according to whether the wall is in load list
            for (int iWall = 0; iWall < drawPoints.Count - 1; iWall++) {
                if (structuralWalls.Contains(iWall)) {
                    gfx.DrawLine(penStructural, drawPoints[iWall], drawPoints[iWall + 1]);
                }
                else {
                    gfx.DrawLine(penArchitectural, drawPoints[iWall], drawPoints[iWall + 1]);
                }
            }
        }

        private static XPoint WorldToPaper(XGraphics gfx, Vector3 point) {
            double centerX = gfx.PageSize.Width / 2;
            double centerY = gfx.PageSize.Height / 2;
            return new XPoint(point.x * scale + centerX, -point.z * scale + centerY);
        }
    }
}