/*
 * THIS FILE IS NOT BEING COMPILED AND BUILT WITH A PROJECT
 * This is only helper file for testing application. 
 */

using System;

public class Program {
    public static int TestMethod(int x, int y) {
        if ( x < 0 || y < 0 )
            return x % y;
        else if ( x % 2 == 0 && y % 2 == 0 )
            return x + y;
        else if ( x % 2 == 1 && y % 2 == 0 )
            return x - y;
        else if ( x % 2 == 0 && y % 2 == 1 )
            return x / y;
        else // if ( x % 2 == 1 && y % 2 == 1 )
            return x * y;
    }
}