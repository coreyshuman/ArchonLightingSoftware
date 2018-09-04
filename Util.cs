using System.Collections.Generic;
using System.Linq;

namespace ArchonLightingSystem
{
    public static class Util
    {
        public static IEnumerable<T> SliceRow<T>(this T[,] array, int row)
        {
            for (var i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
            {
                yield return array[row, i];
            }
        }

        public static IEnumerable<T> SliceColumn<T>(this T[,] array, int column)
        {
            for (var i = array.GetLowerBound(0); i <= array.GetUpperBound(0); i++)
            {
                yield return array[i, column];
            }
        }

        public static string[] ToStringArray(this byte[] array)
        {
            return array.Select(d => ((int)d).ToString()).ToArray();
        }
    }
}
