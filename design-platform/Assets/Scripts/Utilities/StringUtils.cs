using System.Globalization;

namespace StringUtils {
    public static class StringUtils {
        public static string ToTitleCase(string str) {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            return textInfo.ToTitleCase(str.ToLower().Replace("_", " "));
        }

    }
}

