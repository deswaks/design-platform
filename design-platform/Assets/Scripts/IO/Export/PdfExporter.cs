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

namespace DesignPlatform.Export {
    public static class PdfExporter {

        public static Dictionary<string, float> Scales = new Dictionary<string, float>() {
        {"1:50", 56.692f},
        {"1:100", 28.346f}
    };
        public static float scale = Scales["1:100"];

        /// <summary>
        /// Prints all the spaces of the building to a pdf file
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
            DrawSpaces(gfx);
            TagSpaces(gfx);

            // Save the document
            document.Save("Exports/Plan.pdf");
            UnityEngine.Debug.Log("Successfully exported gbxml to: ~Exports/Plan.pdf");
            Process.Start("Exports\\Plan.pdf"); //Start viewer
        }

        /// <summary>
        /// Draws each space of the building using the given XGraphics object
        /// </summary>
        private static void DrawSpaces(XGraphics gfx) {
            // Definition of Pens and brushes
            XSolidBrush spaceBrush = new XSolidBrush(XColors.BlanchedAlmond);
            XPen wallPen1 = new XPen(XColors.Black, 2);
            XPen wallPen2 = new XPen(XColors.Black, 6);
            wallPen2.LineCap = XLineCap.Square;

            foreach (Core.Space space in Building.Instance.Spaces) {
                drawSpacePolygon(gfx, spaceBrush, space);
            }
            foreach (Core.Space space in Building.Instance.Spaces) {
                drawWallLines(gfx, wallPen1, wallPen2, space);
            }
        }

        private static void TagSpaces(XGraphics gfx) {
            // Font definitions
            const string facename = "Times New Roman";
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
            XFont nameFont = new XFont(facename, 12, XFontStyle.Bold, options);
            XFont areaFont = new XFont(facename, 10, XFontStyle.Regular, options);

            XStringFormat tagFormat = new XStringFormat();
            tagFormat.Alignment = XStringAlignment.Center;

            foreach (Core.Space space in Building.Instance.Spaces) {
                WriteSpaceName(gfx, nameFont, space, tagFormat);
                WriteSpaceArea(gfx, areaFont, space, tagFormat);
            }
        }

        private static void WriteSpaceName(XGraphics gfx, XFont tagFont, Core.Space space, XStringFormat tagFormat) {
            XPoint tagLocation = WorldToPaper(gfx, space.GetTagLocation());
            List<string> tagLines = new List<string>();
            using (System.IO.StringReader reader = new System.IO.StringReader(space.TypeName)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    tagLines.Add(line);
                }
            }
            for (int i = 0; i < tagLines.Count; i++) {
                double offset = (tagFont.Size / 2 - 2) + (tagFont.Size - 2) * (tagLines.Count - i - 1);
                gfx.DrawString(tagLines[i], tagFont, XBrushes.Black, tagLocation.X, tagLocation.Y - offset, tagFormat);
            }
        }

        private static void WriteSpaceArea(XGraphics gfx, XFont tagFont, Core.Space space, XStringFormat tagFormat) {
            XPoint tagLocation = WorldToPaper(gfx, space.GetTagLocation());
            string tagText = space.GetFloorArea().ToString() + "m²";
            gfx.DrawString(tagText, tagFont, XBrushes.Black, tagLocation.X, tagLocation.Y + tagFont.Size / 2 + 2, tagFormat);
        }

        private static void drawSpacePolygon(XGraphics gfx, XSolidBrush spaceBrush, Core.Space space) {
            double centerX = gfx.PageSize.Width / 2;
            double centerY = gfx.PageSize.Height / 2;
            List<XPoint> polylinePoints = space.GetControlPoints(closed: true).Select(p => new XPoint(p.x * scale + centerX, -p.z * scale + centerY)).ToList();
            gfx.DrawPolygon(spaceBrush, polylinePoints.ToArray(), XFillMode.Alternate);
        }

        private static void drawWallLines(XGraphics gfx, XPen penArchitectural, XPen penStructural, Core.Space space) {


            Dictionary<int, List<Load>> loadTable = LoadDistribution.AreaLoad(space);
            List<int> structuralWalls = loadTable.Keys.ToList();
            List<XPoint> drawPoints = space.GetControlPoints(closed: true).Select(p => WorldToPaper(gfx, p)).ToList();

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