using System;

public static class IndexingUtils
{
    public static int WrapIndex(int i, int n) {
        return ((i % n) + n) % n;
    }

    public static void Main() {
        Console.WriteLine(WrapIndex(-1, 5));
        Console.WriteLine(WrapIndex(-2, 5));
        Console.WriteLine(WrapIndex(0, 5));
        Console.WriteLine(WrapIndex(2, 5));
        Console.WriteLine(WrapIndex(5, 5));
    }
}
