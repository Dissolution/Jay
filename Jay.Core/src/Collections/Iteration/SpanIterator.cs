using System.ComponentModel;
using Jay.Text;
using static InlineIL.IL;

namespace Jay.Collections.Iteration;

/// <summary>
/// An iterator over a <c>ReadOnlySpan&lt;</c><typeparamref name="T"/><c>&gt;</c>
/// </summary>
/// <typeparam name="T"></typeparam>
public ref struct SpanIterator<T>
{
    private readonly ReadOnlySpan<T> _span;
    private int _position;

    internal int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }
    internal int RemainingLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length - _position;
    }

    internal ReadOnlySpan<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span;
    }

    public int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _position = value;
    }

    public ReadOnlySpan<T> Remaining
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[_position..];
    }

    public SpanIterator(ReadOnlySpan<T> span)
    {
        _span = span;
        _position = 0;
    }

#region Peek
    public bool TryPeek([MaybeNullWhen(false)] out T item)
    {
        if (_position < Length)
        {
            item = _span[_position];
            return true;
        }
        item = default;
        return false;
    }

    public bool TryPeek(int count, out ReadOnlySpan<T> items)
    {
        if ((_position + (uint)count) <= Length)
        {
            items = _span.Slice(_position, count);
            return true;
        }
        items = default;
        return false;
    }

    public T Peek()
    {
        if (_position >= Length)
        {
            throw new InvalidOperationException();
        }
        return _span[_position];
    }

    public ReadOnlySpan<T> Peek(int count)
    {
        if ((_position + (uint)count) > Length)
        {
            throw new InvalidOperationException();
        }
        return _span.Slice(_position, count);
    }

    public ref SpanIterator<T> Peek(out T peeked)
    {
        peeked = this.Peek();
        Emit.Ldarg_0();
        Emit.Ret();
        throw Unreachable();
    }

    public ref SpanIterator<T> Peek(int count, out ReadOnlySpan<T> peeked)
    {
        peeked = this.Peek(count);
        Emit.Ldarg_0();
        Emit.Ret();
        throw Unreachable();
    }
#endregion

#region Skip
    public bool TrySkip()
    {
        int index = _position;
        if (index < Length)
        {
            _position = index + 1;
            return true;
        }
        return false;
    }

    public bool TrySkip(int count)
    {
        if (count <= 0)
            return true;

        int index = _position;
        int newIndex = index + count;
        if (newIndex <= Length)
        {
            _position = newIndex;
            return true;
        }

        return false;
    }

    public ref SpanIterator<T> Skip()
    {
        if (!TrySkip())
            throw new InvalidOperationException();
        Emit.Ldarg_0();
        Emit.Ret();
        throw Unreachable();
    }

    public ref SpanIterator<T> Skip(int count)
    {
        if (!TrySkip(count))
            throw new InvalidOperationException();
        Emit.Ldarg_0();
        Emit.Ret();
        throw Unreachable();
    }
#endregion

#region Skip XYZ
    public ref SpanIterator<T> SkipWhile(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = Length;
        while (index < len && itemPredicate(span[index]))
        {
            index += 1;
        }
        _position = index;
        Emit.Ldarg_0();
        Emit.Ret();
        throw Unreachable();
    }

    public ref SpanIterator<T> SkipWhile(T match)
        => ref SkipWhile(item => EqualityComparer<T>.Default.Equals(item, match));




    public ref SpanIterator<T> SkipUntil(Func<T, bool> itemPredicate)
        => ref SkipWhile(item => !itemPredicate(item));

    public ref SpanIterator<T> SkipUntil(T match)
        => ref SkipWhile(item => !EqualityComparer<T>.Default.Equals(item, match));

    public ref SpanIterator<T> SkipAny(params T[] matches)
        => ref SkipWhile(item => matches.Contains(item));
    
    public ref SpanIterator<T> SkipAny(IReadOnlyCollection<T> matches)
        => ref SkipWhile(item => matches.Contains(item));
    
    public ref SpanIterator<T> SkipAll()
        => ref SkipWhile(static _ => true);
#endregion

#region Take
    public bool TryTake([MaybeNullWhen(false)] out T taken)
    {
        int index = _position;
        if (index < Length)
        {
            _position = index + 1;
            taken = _span[index];
            return true;
        }

        taken = default;
        return false;
    }

    public bool TryTake(int count, out ReadOnlySpan<T> taken)
    {
        if (count <= 0)
        {
            taken = default;
            return true;
        }

        int index = _position;
        int newIndex = index + count;
        if (newIndex <= Length)
        {
            _position = newIndex;
            taken = _span.Slice(index, count);
            return true;
        }

        taken = default;
        return false;
    }

    public T Take()
    {
        if (TryTake(out var taken)) return taken;
        throw new InvalidOperationException();
    }

    public ReadOnlySpan<T> Take(int count)
    {
        if (TryTake(count, out var taken)) return taken;
        throw new InvalidOperationException();
    }

    public ref SpanIterator<T> Take(out T taken)
    {
        taken = Take();
        Emit.Ldarg_0();
        Emit.Ret();
        throw Unreachable();
    }

    public ref SpanIterator<T> Take(int count, out ReadOnlySpan<T> taken)
    {
        taken = Take(count);
        Emit.Ldarg_0();
        Emit.Ret();
        throw Unreachable();
    }
#endregion

#region Take XYZ
    public ref SpanIterator<T> TakeWhile(Func<T, bool> itemPredicate, out ReadOnlySpan<T> taken)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = Length;
        while (index < len && itemPredicate(span[index]))
        {
            index += 1;
        }
        _position = index;
        taken = span.Slice(start, index - start);
        Emit.Ldarg_0();
        Emit.Ret();
        throw Unreachable();
    }

    public ref SpanIterator<T> TakeWhile(T match, out ReadOnlySpan<T> taken)
        => ref TakeWhile(item => EqualityComparer<T>.Default.Equals(item, match), out taken);

    public ref SpanIterator<T> TakeUntil(Func<T, bool> itemPredicate, out ReadOnlySpan<T> taken)
        => ref TakeWhile(item => !itemPredicate(item), out taken);

    public ref SpanIterator<T> TakeUntil(T match, out ReadOnlySpan<T> taken)
        => ref TakeWhile(item => !EqualityComparer<T>.Default.Equals(item, match), out taken);

    public ref SpanIterator<T> TakeAny(T[] matches, out ReadOnlySpan<T> taken)
        => ref TakeWhile(item => matches.Contains(item), out taken);
    
    public ref SpanIterator<T> TakeAny(IReadOnlyCollection<T> matches, out ReadOnlySpan<T> taken)
        => ref TakeWhile(item => matches.Contains(item), out taken);
    
    public ref SpanIterator<T> TakeAll(out ReadOnlySpan<T> taken)
        => ref TakeWhile(static _ => true, out taken);
#endregion
    
    #region IEnumerator Support
    [EditorBrowsable(EditorBrowsableState.Never)]
    public T Current => _span[_position];

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool MoveNext() => TrySkip();
#endregion

    public override string ToString()
    {
        /* We want to show our position in the source span like this:
         * ...abcdef|ghijkl...
         */

        var builder = StringBuilderPool.Shared.Rent();
        int index = _position;
        var span = _span;
        
        // hack for Span<char> ToString looking good
        var delimiter = typeof(T) == typeof(char) ? "" : ",";
        
        // previous characters
        int prev = index - 16;
        if (prev > 0)
        {
            builder.Append('…');
        }
        else
        {
            prev = 0;
        }
        for (var i = prev; i < index; i++)
        {
            if (i > prev)
            {
                builder.Append(delimiter);
            }
            builder.Append<T>(span[i]);
        }
        
        // position indicator
        builder.Append('|');
        
        // next characters
        var end = span.Length;
        int next = index + 16;
        bool postpend;
        if (next < end)
        {
            postpend = true;
        }
        else
        {
            postpend = false;
            next = end;
        }
       
        for (var i = index; i < next; i++)
        {
            if (i > index)
            {
                builder.Append(delimiter);
            }
            builder.Append<T>(span[i]);
        }
        if (postpend)
        {
            builder.Append('…');
        }

        return builder.ReturnToString();
    }
}