using System.ComponentModel;

namespace Jay.Utilities;

public partial struct Hasher
{
    private uint _v1, _v2, _v3, _v4;
    private uint _queue1, _queue2, _queue3;
    private uint _length;

    private void AddHashCode(int value)
    {
        // The original xxHash works as follows:
        // 0. Initialize immediately. We can't do this in a struct (no
        //    default ctor).
        // 1. Accumulate blocks of length 16 (4 uints) into 4 accumulators.
        // 2. Accumulate remaining blocks of length 4 (1 uint) into the
        //    hash.
        // 3. Accumulate remaining blocks of length 1 into the hash.

        // There is no need for #3 as this type only accepts ints. _queue1,
        // _queue2 and _queue3 are basically a buffer so that when
        // ToHashCode is called we can execute #2 correctly.

        // We need to initialize the xxHash32 state (_v1 to _v4) lazily (see
        // #0) nd the last place that can be done if you look at the
        // original code is just before the first block of 16 bytes is mixed
        // in. The xxHash32 state is never used for streams containing fewer
        // than 16 bytes.

        // To see what's really going on here, have a look at the Combine
        // methods.

        uint val = (uint)value;

        // Storing the value of _length locally shaves of quite a few bytes
        // in the resulting machine code.
        uint previousLength = _length++;
        uint position = previousLength % 4;

        // Switch can't be inlined.
        if (position == 0)
            _queue1 = val;
        else if (position == 1)
            _queue2 = val;
        else if (position == 2)
            _queue3 = val;
        else // position == 3
        {
            if (previousLength == 3)
                Initialize(out _v1, out _v2, out _v3, out _v4);

            _v1 = Round(_v1, _queue1);
            _v2 = Round(_v2, _queue2);
            _v3 = Round(_v3, _queue3);
            _v4 = Round(_v4, val);
        }
    }

    public void Add<T>(T? value)
    {
        this.AddHashCode(value?.GetHashCode() ?? 0);
    }

    public void Add<T>(T? value, IEqualityComparer<T>? comparer)
    {
        this.AddHashCode(value is null ? 0 : (comparer?.GetHashCode(value) ?? value.GetHashCode()));
    }

    public int ToHashCode()
    {
        // Storing the value of _length locally shaves of quite a few bytes
        // in the resulting machine code.
        uint length = _length;

        // position refers to the *next* queue position in this method, so
        // position == 1 means that _queue1 is populated; _queue2 would have
        // been populated on the next call to Add.
        uint position = length % 4;

        // If the length is less than 4, _v1 to _v4 don't contain anything
        // yet. xxHash32 treats this differently.

        uint hash = length < 4 ? MixEmptyState() : MixState(_v1, _v2, _v3, _v4);

        // _length is incremented once per Add(Int32) and is therefore 4
        // times too small (xxHash length is in bytes, not ints).

        hash += length * 4;

        // Mix what remains in the queue

        // Switch can't be inlined right now, so use as few branches as
        // possible by manually excluding impossible scenarios (position > 1
        // is always false if position is not > 0).
        if (position > 0)
        {
            hash = QueueRound(hash, _queue1);
            if (position > 1)
            {
                hash = QueueRound(hash, _queue2);
                if (position > 2)
                    hash = QueueRound(hash, _queue3);
            }
        }

        hash = MixFinal(hash);
        return (int)hash;
    }

    [Obsolete("Hasher is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.",
        error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException();

    [Obsolete("Hasher is a mutable struct and should not be compared with other HashCodes.", error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new NotSupportedException();
}