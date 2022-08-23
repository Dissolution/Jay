namespace Jay.Randomization;

public static class Extensions
{
    private static IReadOnlyList<T> ShuffledList<T>(IList<T> list)
    {
        /* To initialize an array a of n elements to a randomly shuffled copy of source, both 0-based:
         *  for i from 0 to n − 1 do
         *      j ← random integer such that 0 ≤ j ≤ i
         *      if j ≠ i
         *          a[i] ← a[j]
         *      a[j] ← source[i]
         */
        var n = list.Count;
        var dest = new T[n];
        for (var i = 0; i < n; i++)
        {
            var j = Randomizer.Instance.ZeroTo(i);
            if (j != i)
            {
                dest[i] = dest[j];
            }

            dest[j] = list[i];
        }

        return dest;
    }

    private static IReadOnlyList<T> ShuffledList<T>(IReadOnlyList<T> list)
    {
        var n = list.Count;
        var dest = new T[n];
        for (var i = 0; i < n; i++)
        {
            var j = Randomizer.Instance.ZeroTo(i);
            if (j != i)
            {
                dest[i] = dest[j];
            }

            dest[j] = list[i];
        }

        return dest;
    }


    private static IReadOnlyList<T> ShuffledEnumerable<T>(IEnumerable<T> enumerable)
    {
        /* To initialize an empty array a to a randomly shuffled copy of source whose length is not known:
         *   while source.moreDataAvailable
         *       j ← random integer such that 0 ≤ j ≤ a.length
         *       if j = a.length
         *           a.append(source.next)
         *       else
         *           a.append(a[j])
         *           a[j] ← source.next                         
         */
        var dest = new List<T>(0);
        using (var e = enumerable.GetEnumerator())
        {
            while (e.MoveNext())
            {
                var j = Randomizer.Instance.ZeroTo(dest.Count);
                if (j == dest.Count)
                {
                    dest.Add(e.Current);
                }
                else
                {
                    dest.Add(dest[j]);
                    dest[j] = e.Current;
                }
            }
        }

        return dest;
    }


    public static void Shuffle<T>(this Span<T> span)
    {
        /* -- To shuffle an array a of n elements (indices 0..n-1):
         * for i from n−1 downto 1 do
         *  j ← random integer such that 0 ≤ j ≤ i
         *  exchange a[j] and a[i]
         */
        int r;
        T temp;
        for (var i = span.Length - 1; i > 0; i--)
        {
            r = Randomizer.Instance.ZeroTo(i);
            temp = span[i];
            span[i] = span[r];
            span[r] = temp;
        }
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        /* -- To shuffle an array a of n elements (indices 0..n-1):
         * for i from n−1 downto 1 do
         *  j ← random integer such that 0 ≤ j ≤ i
         *  exchange a[j] and a[i]
         */
        int r;
        T temp;
        for (var i = list.Count - 1; i > 0; i--)
        {
            r = Randomizer.Instance.ZeroTo(i);
            temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }

    public static IReadOnlyList<T> Shuffled<T>(this IEnumerable<T> enumerable)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        return enumerable switch
               {
                   null => throw new ArgumentNullException(nameof(enumerable)),
                   IList<T> list => ShuffledList(list),
                   IReadOnlyList<T> readOnlyList => ShuffledList(readOnlyList),
                   // ReSharper disable once PossibleMultipleEnumeration
                   _ => ShuffledEnumerable(enumerable)
               };
    }
}