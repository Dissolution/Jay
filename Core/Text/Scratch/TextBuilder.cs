using System.Buffers;
using Jay.Exceptions;
using static InlineIL.IL;

namespace Jay.Text.Scratch;

public delegate void TextBuilderWriteValue<in T>(ref TextBuilder textBuilder, T? value);

public ref partial struct TextBuilder
{
    internal const int MinCapacity = 1024;
    internal const int MaxCapacity = 0x7FFFFFC7 / sizeof(char); // Array.MaxLength

    public static implicit operator TextBuilder(Span<char> buffer) => new TextBuilder(buffer);

    static TextBuilder()
    {
        //DefaultInterpolatedStringHandler
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void* RefToVoidPointer(ref TextBuilder textBuilder)
    {
        Emit.Ldarg(nameof(textBuilder));
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ref TextBuilder VoidPointerToRef(void* pointer)
    {
        Emit.Ldarg(nameof(pointer));
        Emit.Ret();
        throw Unreachable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TextBuilder Ref(in TextBuilder textBuilder)
    {
        Emit.Ldarg(nameof(textBuilder));
        Emit.Ret();
        throw Unreachable();
    }
}

public ref partial struct TextBuilder
{
    private char[]? _charArray;
    private Span<char> _charSpan;
    private int _length;

    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((uint)index < (uint)_length)
            {
                return ref _charSpan[index];
            }

            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be between 0 and {_length - 1}");
        }
    }

    internal int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charSpan.Length;
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _length = value;
    }

    public Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charSpan[.._length];
    }

    internal Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charSpan[_length..];
    }

    public TextBuilder()
    {
        _charSpan = _charArray = ArrayPool<char>.Shared.Rent(MinCapacity);
        _length = 0;
    }

    public TextBuilder(int minCapacity)
    {
        _charSpan = _charArray = ArrayPool<char>.Shared.Rent(Math.Clamp(minCapacity, MinCapacity, MaxCapacity));
        _length = 0;
    }

    public TextBuilder(Span<char> buffer)
    {
        _charArray = null;
        _charSpan = buffer;
        _length = 0;
    }

#region Benchmark Methods

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowByAtLeast(int amount)
    {
        var newCapacity = Math.Clamp((Capacity + amount) * 2, MinCapacity, MaxCapacity);
        char[] newArray = ArrayPool<char>.Shared.Rent(newCapacity);
        TextHelper.CopyTo(Written, newArray);
        char[]? toReturn = _charArray;
        _charSpan = _charArray = newArray;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowByB(int amount)
    {
        var span = _charSpan;
        var newCapacity = Math.Clamp((span.Length + amount) * 2, MinCapacity, MaxCapacity);
        char[] newArray = ArrayPool<char>.Shared.Rent(newCapacity);
        TextHelper.Copy(in span.GetPinnableReference(),
            ref newArray[0],
            span.Length);
        char[]? toReturn = _charArray;
        _charSpan = _charArray = newArray;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowByInl(int amount)
    {
        var newCapacity = Math.Clamp((Capacity + amount) * 2, MinCapacity, MaxCapacity);
        char[] newArray = ArrayPool<char>.Shared.Rent(newCapacity);
        TextHelper.CopyTo(Written, newArray);
        char[]? toReturn = _charArray;
        _charSpan = _charArray = newArray;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private Span<char> GrowSpan(int amount)
    {
        var newCapacity = Math.Clamp((Capacity + amount) * 2, MinCapacity, MaxCapacity);
        char[] newArray = ArrayPool<char>.Shared.Rent(newCapacity);
        TextHelper.CopyTo(Written, newArray);
        char[]? toReturn = _charArray;
        _charSpan = _charArray = newArray;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }

        return Available;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowCopy(char ch)
    {
        GrowByAtLeast(1);
        _charSpan[_length] = ch;
        _length++;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowCopy(ReadOnlySpan<char> text)
    {
        GrowByInl(text.Length);
        TextHelper.CopyTo(text, Available);
        _length += text.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TextBuilder AppendChar(char ch)
    {
        WriteCharC(ch);
        // unsafe
        // {
        //     return ref VoidPointerToRef(RefToVoidPointer(ref this));
        // }
        Emit.Ldarg(0); // should be ref this
        Emit.Ret();
        throw Unreachable();
    }

#region WriteChar

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharA(char ch)
    {
        if (_length == _charSpan.Length)
        {
            GrowByAtLeast(1);
        }

        _charSpan[_length++] = ch;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharB(char ch)
    {
        if (_charSpan.Length - _length == 0)
        {
            GrowByAtLeast(1);
        }

        _charSpan[_length++] = ch;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteChar_BB(char ch)
    {
        if (_charSpan.Length - _length == 0)
        {
            GrowByB(1);
        }

        _charSpan[_length++] = ch;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharC(char ch)
    {
        if (_charSpan.Length - _length == 0)
        {
            GrowByAtLeast(1);
        }

        _charSpan[_length] = ch;
        _length++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteChar_CB(char ch)
    {
        if (_charSpan.Length - _length == 0)
        {
            GrowByB(1);
        }

        _charSpan[_length] = ch;
        _length++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharC1(char ch)
    {
        int len = _length;
        if (_charSpan.Length - len == 0)
        {
            GrowByAtLeast(1);
        }

        _charSpan[len] = ch;
        _length = len + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteChar_C1B(char ch)
    {
        int len = _length;
        if (_charSpan.Length - len == 0)
        {
            GrowByB(1);
        }

        _charSpan[len] = ch;
        _length = len + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharC2(char ch)
    {
        Span<char> avail = Available;
        if (Available.Length == 0)
        {
            avail = GrowSpan(1);
        }

        avail[0] = ch;
        _length++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharC3(char ch)
    {
        int newLen = _length + 1;
        if (newLen >= _charSpan.Length)
        {
            GrowByAtLeast(1);
        }

        _charSpan[_length] = ch;
        _length = newLen;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharD(char ch)
    {
        Span<char> chars = _charSpan;
        int pos = _length;
        if (pos < chars.Length)
        {
            chars[pos] = ch;
            _length = pos + 1;
        }
        else
        {
            GrowCopy(ch);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharE(char ch)
    {
        int pos = _length;
        if (pos >= _charSpan.Length)
        {
            GrowByAtLeast(1);
        }

        _charSpan[pos] = ch;
        _length = pos + 1;
    }

#endregion

#region WriteSpan

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpan_3Locals(ReadOnlySpan<char> span)
    {
        int spanLen = span.Length;
        int len = _length;
        int newLen = spanLen + len;
        if (newLen >= _charSpan.Length)
        {
            GrowByAtLeast(spanLen);
        }

        TextHelper.Copy(in span.GetPinnableReference(),
            ref _charSpan[len],
            spanLen);
        _length = newLen;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpan_SpanLen_Len(ReadOnlySpan<char> span)
    {
        int spanLen = span.Length;
        int len = _length;
        if (spanLen + len >= _charSpan.Length)
        {
            GrowByAtLeast(spanLen);
        }

        TextHelper.Copy(in span.GetPinnableReference(),
            ref _charSpan[len],
            spanLen);
        _length = spanLen + len;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpan_Len_NewLen(ReadOnlySpan<char> span)
    {
        int len = _length;
        int newLen = span.Length + len;
        if (newLen >= _charSpan.Length)
        {
            GrowByAtLeast(span.Length);
        }

        TextHelper.Copy(in span.GetPinnableReference(),
            ref _charSpan[len],
            span.Length);
        _length = newLen;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpan_SpanLen_NewLen(ReadOnlySpan<char> span)
    {
        int spanLen = span.Length;
        int newLen = spanLen + _length;
        if (newLen >= _charSpan.Length)
        {
            GrowByAtLeast(spanLen);
        }

        TextHelper.Copy(in span.GetPinnableReference(),
            ref _charSpan[_length],
            spanLen);
        _length = newLen;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpan_SpanLen(ReadOnlySpan<char> span)
    {
        int spanLen = span.Length;
        if (spanLen + _length >= _charSpan.Length)
        {
            GrowByAtLeast(spanLen);
        }

        TextHelper.Copy(in span.GetPinnableReference(),
            ref _charSpan[_length],
            spanLen);
        _length += spanLen;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpan_NewLen(ReadOnlySpan<char> span)
    {
        int newLen = span.Length + _length;
        if (newLen >= _charSpan.Length)
        {
            GrowByAtLeast(span.Length);
        }

        TextHelper.Copy(in span.GetPinnableReference(),
            ref _charSpan[_length],
            span.Length);
        _length = newLen;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpan_Len(ReadOnlySpan<char> span)
    {
        int len = _length;
        if (len + span.Length >= _charSpan.Length)
        {
            GrowByAtLeast(span.Length);
        }

        TextHelper.Copy(in span.GetPinnableReference(),
            ref _charSpan[len],
            span.Length);
        _length = len + span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSpan(ReadOnlySpan<char> span)
    {
        if (_length + span.Length >= _charSpan.Length)
        {
            GrowByAtLeast(span.Length);
        }

        TextHelper.Copy(in span.GetPinnableReference(),
            ref _charSpan[_length],
            span.Length);
        _length += span.Length;
    }

#endregion

#endregion


    private ref TextBuilder This
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Emit.Ldarga(0);
            Emit.Ret();
            throw Unreachable();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TextBuilder Append(char ch)
    {
        WriteCharC(ch);
        Emit.Ldarga(0);
        Emit.Ret();
        throw Unreachable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TextBuilder Append(ReadOnlySpan<char> text)
    {
        WriteSpan_3Locals(text);
        Emit.Ldarga(0);
        Emit.Ret();
        throw Unreachable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TextBuilder Append(string? text)
    {
        if (text is not null)
        {
            WriteStringDirect(text);
        }
        return ref This;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal void WriteStringDirect(string text)
    {
        int textLength = text.Length;
        int len = _length;
        int newLen = textLength + len;
        if (newLen >= _charSpan.Length)
        {
            GrowByAtLeast(textLength);
        }

        TextHelper.Copy(in text.GetPinnableReference(),
            ref _charSpan[len],
            textLength);
        _length = newLen;
    }

    public void Write<T>(T? value)
    {
        string? s;
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, default)) // constrained call avoiding boxing for value types
                {
                    GrowByAtLeast(charsWritten);
                }

                _length += charsWritten;
                return;
            }

            s = ((IFormattable)value).ToString(format: null, formatProvider: null); // constrained call avoiding boxing for value types
        }
        else
        {
            s = value?.ToString();
        }

        if (s is not null)
        {
            WriteStringDirect(s);
        }
    }

    public ref TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter,
        IEnumerable<T> values,
        TextBuilderWriteValue<T> writeValue)
    {
        using var e = values.GetEnumerator();
        if (e.MoveNext())
        {
            writeValue(ref this, e.Current);
            while (e.MoveNext())
            {
                WriteSpan(delimiter);
                writeValue(ref this, e.Current);
            }
        }
        return ref This;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        _length = 0;
    }

    public ref struct Enumerator
    {
        private TextBuilder _textBuilder;

        public int Index { get; private set; }
        public char Current { get; private set; }
        
        public Enumerator(ref TextBuilder textBuilder)
        {
            _textBuilder = textBuilder;
            this.Index = -1;
            this.Current = default;
        }
        
        public bool MoveNext()
        {
            int nextIndex = Index + 1;
            if (nextIndex >= _textBuilder._length)
            {
                return false;
            }
        }
    }

    public Enumerator GetEnumerator() => new Enumerator(ref this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        char[]? toReturn = _charArray;
        this = default; // defensive clear
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(string? text) => TextHelper.Equals(Written, text);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(string? text, StringComparison comparison) => TextHelper.Equals(Written, text, comparison);

    public override bool Equals(object? obj)
    {
        if (obj is string str)
            return TextHelper.Equals(Written, str);
        if (obj is char[] chars)
            return TextHelper.Equals(Written, chars);
        return false;
    }

    public override int GetHashCode() => UnsuitableException.ThrowGetHashCode(typeof(TextBuilder));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        return new string(_charArray, 0, _length);
    }
}