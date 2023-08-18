namespace Jay.Debugging;

public static class Swallow
{
    /// <summary>
    /// A deep wrapper for <see cref="IEnumerable{T}" /> that ignores all thrown exceptions
    /// at every level of enumeration, only returning values that could be acquired without error
    /// </summary>
    /// <param name="enumerable"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> SwallowEnumerate<T>(this IEnumerable<T>? enumerable)
    {
        if (enumerable is null) yield break;
        IEnumerator<T> enumerator;
        try
        {
            enumerator = enumerable.GetEnumerator();
        }
        catch (Exception)
        {
            yield break;
        }

        while (true)
        {
            // Move next
            try
            {
                if (!enumerator.MoveNext())
                {
                    // stop enumerating
                    enumerator.Dispose();
                    yield break;
                }
            }
            catch (Exception)
            {
                // ignore this, stop enumerating
                enumerator.Dispose();
                yield break;
            }

            // Yield current
            T current;
            try
            {
                current = enumerator.Current;
            }
            catch (Exception)
            {
                // ignore this, but continue enumerating
                continue;
            }

            yield return current;
        }
    }
}