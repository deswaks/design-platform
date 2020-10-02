using System;

public static class IndexingUtils
{
    /// <summary>
    /// Wrap an index such that it will always be within the length of the list.
    /// Negative values will be from the end of the list.
    /// Values above the list length will wrap circularly.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="listLength">Length of the list that the index should be wrapped to</param>
    /// <returns>wrapped index</returns>
    public static int WrapIndex(int index, int listLength) {
        return ((index % listLength) + listLength) % listLength;
    }

    public static void Main() {
        Console.WriteLine(WrapIndex(-1, 5));
        Console.WriteLine(WrapIndex(-2, 5));
        Console.WriteLine(WrapIndex(0, 5));
        Console.WriteLine(WrapIndex(2, 5));
        Console.WriteLine(WrapIndex(5, 5));
    }
}
