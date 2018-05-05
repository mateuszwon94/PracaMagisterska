/*
 * THIS FILE IS NOT BEING COMPILED AND BUILT WITH A PROJECT
 * This is only helper file for testing application. 
 */

using System;

public class Program {
    public static void TestMethod() {
        int i = 0,
            j = 10;

        i += j - 30;
        j /= (4 * 19) % 4;

        bool v1 = true,
             v2 = false;

        bool r1 = v1 & v2,
             r2 = v1 | v2,
             r3 = v1 ^ v2;

        i >>= 1;
        j <<= 1;
    }
}