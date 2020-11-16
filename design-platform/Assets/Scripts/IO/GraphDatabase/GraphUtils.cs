using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using System.Numerics;


namespace DesignPlatform.Database {
    public static class GraphUtils {

        /// <summary>
        /// Converts a single string signifying a point (e.g. "(1;2;3)") to Vector3 format
        /// </summary>
        /// <param name="pointString">Point string to vectorize, e.g. "(1;2;3)"</param>
        /// <param name="seperator">Seperator character, e.g. ";"</param>
        /// <returns></returns>
        public static Vector3 StringToVector3(string pointString = "(0;0;0)", string seperator = ";") {
            int start = pointString.IndexOf("(");
            int firstSeperator = pointString.IndexOf(seperator);
            int secondSeperator = pointString.LastIndexOf(seperator);
            int end = pointString.IndexOf(")");

            float xVal = float.Parse(pointString.Substring(start + 1, firstSeperator - start - 1), System.Globalization.CultureInfo.InvariantCulture);
            float yVal = float.Parse(pointString.Substring(firstSeperator + 1, secondSeperator - firstSeperator - 1), System.Globalization.CultureInfo.InvariantCulture);
            float zVal = float.Parse(pointString.Substring(secondSeperator + 1, end - secondSeperator - 1), System.Globalization.CultureInfo.InvariantCulture);

            return new Vector3(xVal, yVal, zVal);
        }

        /// <summary>
        /// Converts a list of Vector3 to a list of strings signifying the points
        /// </summary>
        /// <param name="points"></param>
        /// <returns>List of strings signifying the points</returns>
        public static string[] Vector3ListToStringList(IEnumerable<Vector3> points) {
            List<string> stringPoints = new List<string>();
            points.ToList().ForEach(v => stringPoints.Add(
               "(" +
               ((int)Math.Round(v.x)).ToString().Replace(",", ".") +
               ";" +
               ((int)Math.Round(v.y)).ToString().Replace(",", ".") +
               ";" +
               ((int)Math.Round(v.z)).ToString().Replace(",", ".") +
               ")"
               )
            );
            return stringPoints.ToArray();
        }

        /// <summary>
        /// Converts a list of strings signifying points to a list of Vector3 points
        /// </summary>
        /// <param name="pointStrings"></param>
        /// <returns>List of Vector3 points</returns>
        public static List<Vector3> StringListToVector3List(IEnumerable<string> pointStrings) {
            List<Vector3> points = new List<Vector3>();
            if (pointStrings != null) {
                points = pointStrings.ToList().Select(p => StringToVector3(p)).ToList();
            }
            return points;
        }
    }
}
