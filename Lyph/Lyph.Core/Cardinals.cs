using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lyph;

public sealed class Cardinals<T> : IEquatable<Cardinals<T>>,
                                   IEnumerable<T>
{
    public static bool operator ==(Cardinals<T> left, Cardinals<T> right) => left.Equals(right);
    public static bool operator !=(Cardinals<T> left, Cardinals<T> right) => !left.Equals(right);

    private readonly T[] _directions;

    public T Left => _directions[(int)CardinalDirection.Left];
    public T Top => _directions[(int)CardinalDirection.Top];
    public T Right => _directions[(int)CardinalDirection.Right];
    public T Bottom => _directions[(int)CardinalDirection.Bottom];

    public Cardinals(T left, T top, T right, T bottom)
    {
        _directions = new T[4] { left, top, right, bottom };
    }

    IEnumerator IEnumerable.GetEnumerator() => _directions.GetEnumerator();
    public IEnumerator<T> GetEnumerator()
    {
        yield return _directions[0];
        yield return _directions[1];
        yield return _directions[2];
        yield return _directions[3];
    }

    public bool Equals(Cardinals<T>? cardinals)
    {
        if (cardinals is not null)
        {
            var otherDirections = cardinals._directions;
            for (var i = 0; i < 4; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(otherDirections[i], _directions[i]))
                    return false;
            }
            return true;
        }
        return false;
    }

    public override bool Equals(object? obj)
    {
        return obj is Cardinals<T> cardinals && Equals(cardinals);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Top, Right, Bottom);
    }

    public override string ToString()
    {
        return $"[{Left},{Top},{Right},{Bottom}]";
    }
}