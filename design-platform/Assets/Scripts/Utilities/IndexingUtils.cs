namespace DesignPlatform.Utils {

    /// <summary>
    /// Helper functions for indexing problems
    /// </summary>
    public static class IndexingUtils {

        /// <summary>
        /// Wraps an index to a given length, which means that indices
        /// above the length will be mapped to the length in a circular
        /// fashion and indices below 0 will be mapped starting from the end.
        /// </summary>
        /// <param name="i">index to wrap.</param>
        /// <param name="n">length of list to wrap the index to.</param>
        /// <returns></returns>
        public static int WrapIndex(int i, int n) {
            return (i % n + n) % n;
        }
    }
}