using System.Globalization;

namespace DesignPlatform.Utils {

    /// <summary>
    /// Helper functions for strings of text.
    /// </summary>
    public static class StringUtils {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(string str) {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            return textInfo.ToTitleCase(str.ToLower().Replace("_", " "));
        }

    }
}

