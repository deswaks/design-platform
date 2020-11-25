namespace DesignPlatform.Utils {
    public static class IndexingUtils {
        public static int WrapIndex(int i, int n) {
            return (i % n + n) % n;
        }
    }
}