﻿using System;

namespace AlgoLib.Extensions
{
    public static class ArrayExtensions
    {
        public static void Swap<T>(this T[] array, int x, int y)
        {
            T temp = array[x];
            array[x] = array[y];
            array[y] = temp;
        }
    }
}