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
using DesignPlatform.Geometry;

namespace DesignPlatform.Export {

    /// <summary>
    /// Contains functions to export drawings of the building to PDF format.
    /// </summary>
    public static class PdfExporter {

        /// <summary>Contains the decimal scale from model to paper space for different drawings scales.</summary>
        public static Dictionary<string, float> Scales = new Dictionary<string, float>() {
            {"1:50", 56.692f},
            {"1:100", 28.346f}
        };

        /// <summary>Describes the chosen scale.</summary>
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
        /// Draws each space of the building.
        /// </summary>
        /// <param name="gfx">Drawing to draw spaces on.</param>
        private static void DrawSpaces(XGraphics gfx) {
            // Definition of Pens and brushes
            XSolidBrush spaceBrush = new XSolidBrush(XColors.BlanchedAlmond);
            XPen wallPen1 = new XPen(XColors.Black, 2);
            XPen wallPen2 = new XPen(XColors.Black, 6);
            wallPen2.LineCap = XLineCap.Square;

            foreach (Core.Space space in Building.Instance.Spaces) {
                DrawSpacePolygon(gfx, spaceBrush, space);
            }
            foreach (Core.Space space in Building.Instance.Spaces) {
                DrawWallLines(gfx, wallPen1, wallPen2, space);
            }
        }

        /// <summary>
        /// Draw a room tag for each space of the building
        /// </summary>
        /// <param name="gfx">Drawing to draw spaces on.</param>
        private static void TagSpaces(XGraphics gfx) {
            // Font definitions
            const string facename = "Times New Roman";
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
            XFont nameFont = new XFont(facename, 12, XFontStyle.Bold, options);
            XFont areaFont = new XFont(facename, 10, XFontStyle.Regular, options);

            foreach (Core.Space space in Building.Instance.Spaces) {
                WriteSpaceName(gfx, nameFont, space);
                WriteSpaceArea(gfx, areaFont, space);
            }
        }
        
        /// <summary>
        /// Write the room name of a space.
        /// </summary>
        /// <param name="gfx">Drawing to write space names on.</param>
        /// <param name="tagFont">Font to write room name in.</param>
        /// <param name="space">Space to write room name for.</param>
        private static void WriteSpaceName(XGraphics gfx, XFont tagFont, Core.Space space) {
            XStringFormat tagFormat = new XStringFormat();
            tagFormat.Alignment = XStringAlignment.Center;
            XPoint tagLocation = ConvertUnitsModelToPaper(gfx, space.GetTagLocation());
            List<string> tagLines = new List<string>();
            using (System.IO.StringReader reader = new System.IO.StringReader(Settings.SpaceTypeNames[space.Function])) {
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

        /// <summary>
        /// Write the area of a space.
        /// </summary>
        /// <param name="gfx">Drawing to write space area on.</param>
        /// <param name="tagFont">Font to write area in.</param>
        /// <param name="space">Space to write area for.</param>
        private static void WriteSpaceArea(XGraphics gfx, XFont tagFont, Core.Space space) {
            XStringFormat tagFormat = new XStringFormat();
            tagFormat.Alignment = XStringAlignment.Center;
            XPoint tagLocation = ConvertUnitsModelToPaper(gfx, space.GetTagLocation());
            string tagText = space.Area.ToString() + "m²";
            gfx.DrawString(tagText, tagFont, XBrushes.Black, tagLocation.X, tagLocation.Y + tagFont.Size / 2 + 2, tagFormat);
        }

        /// <summary>
        /// Draw a space as a polygon.
        /// </summary>
        /// <param name="gfx">Drawing to draw the polygon on.</param>
        /// <param name="spaceBrush">Style to draw the polygon in.</param>
        /// <param name="space">Space to draw polygon shape for.</param>
        private static void DrawSpacePolygon(XGraphics gfx, XSolidBrush spaceBrush, Core.Space space) {
            double centerX = gfx.PageSize.Width / 2;
            double centerY = gfx.PageSize.Height / 2;
            List<XPoint> polylinePoints = space.GetControlPoints(closed: true).Select(p => new XPoint(p.x * scale + centerX, -p.z * scale + centerY)).ToList();
            gfx.DrawPolygon(spaceBrush, polylinePoints.ToArray(), XFillMode.Alternate);
        }

        /// <summary>
        /// Draws the lines of the walls of a space object.
        /// </summary>
        /// <param name="gfx">Drawing to draw the lines on.</param>
        /// <param name="penArchitectural">Style to use for non-structural walls when drawing.</param>
        /// <param name="penStructural">Style to use for structural walls when drawing.</param>
        /// <param name="space">Space to draw walls for.</param>
        private static void DrawWallLines(XGraphics gfx, XPen penArchitectural, XPen penStructural, Core.Space space) {
            Dictionary<int, List<DistributedLoad>> loadTable = LoadDistributor.DistributeAreaLoads(space);
            List<int> structuralWalls = loadTable.Keys.ToList();
            List<XPoint> drawPoints = space.GetControlPoints(closed: true).Select(p => ConvertUnitsModelToPaper(gfx, p)).ToList();

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

        /// <summary>
        /// Converts point coordinates from model units to paper units.
        /// </summary>
        /// <param name="gfx">Drawing to convert units for.</param>
        /// <param name="point">Point coordinates to convert into paper units.</param>
        /// <returns>The point with coordinates in paper units.</returns>
        private static XPoint ConvertUnitsModelToPaper(XGraphics gfx, Vector3 point) {
            double centerX = gfx.PageSize.Width / 2;
            double centerY = gfx.PageSize.Height / 2;
            return new XPoint(point.x * scale + centerX, -point.z * scale + centerY);
        }
    }
}