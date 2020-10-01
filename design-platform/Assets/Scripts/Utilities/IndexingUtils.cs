using System;

public static class IndexingUtils
{
    public static int WrapIndex(int i, int listLength) {
        i = i % listLength;
        if (i < 0) {
            return -i;
        }
        return i;
    }


}
