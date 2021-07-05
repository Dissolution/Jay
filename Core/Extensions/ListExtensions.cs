using System.Collections.Generic;

namespace Jay
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
        }
    }
}