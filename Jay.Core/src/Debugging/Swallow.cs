namespace Jay.Debugging;

/// <summary>
/// Swallows <see cref="Exception"/>s
/// </summary>
public static class Swallow
{
    /// <summary>
    /// A deep wrapper for <see cref="IEnumerable{T}"/> that ignores all thrown exceptions
    /// at every level of enumeration, only returning values that could be acquired without error
    /// </summary>
    public static IEnumerable<T> Swallowed<T>(this IEnumerable<T>? enumerable)
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