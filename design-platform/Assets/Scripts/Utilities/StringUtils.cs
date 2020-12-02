using System.Globalization;

namespace DesignPlatform.Utils {

    /// <summary>
    /// Helper functions for strings of text.
    /// </summary>
    public static class StringUtils {

        /// <summary>
        /// Convert a string to title case (This Is An Example Of Title Case)
        /// </summary>
        /// <param name="str">string to convert.</param>
        /// <returns>The string converted to title case.</returns>
        public static string ToTitleCase(string str) {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            return textInfo.ToTitleCase(str.ToLower().Replace("_", " "));
        }

    }
}

