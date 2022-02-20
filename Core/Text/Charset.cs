using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Jay.Text;

public ref struct Charset
{
    private ushort _bits;

    public bool this[int bit]
    {
        get
        {
            return (_bits & bit) == unchecked((uint)bit);
        }
        set
        {
            unchecked
            {
                if (value)
                {
                    _data |= (uint)bit;
                }
                else
                {
                    _data &= ~(uint)bit;
                }
            }
        }
    }
    
    public Charset(ushort data)
    {
        _bits = data;
    }

    public Charset(Charset chars)
    {
        _bits = chars._bits;
    }
    
}


/*


        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether all the specified bits are set.</para>
        /// </devdoc>
        public bool this[int bit]
        {
            get
            {
                return (_data & bit) == unchecked((uint)bit);
            }
            set
            {
                unchecked
                {
                    if (value)
                    {
                        _data |= (uint)bit;
                    }
                    else
                    {
                        _data &= ~(uint)bit;
                    }
                }
            }
        }

        /// <devdoc>
        ///    <para>Gets or sets the value for the specified section.</para>
        /// </devdoc>
        public int this[Section section]
        {
            get
            {
                unchecked
                {
                    return (int)((_data & (uint)(section.Mask << section.Offset)) >> section.Offset);
                }
            }
            set
            {
                // The code should really have originally validated "(value & section.Mask) == value" with
                // an exception (it instead validated it with a Debug.Assert, which does little good in a
                // public method when in a Release build).  We don't include such a check now as it would
                // likely break things and for little benefit.

                value <<= section.Offset;
                int offsetMask = (0xFFFF & (int)section.Mask) << section.Offset;
                _data = unchecked((_data & ~(uint)offsetMask) | ((uint)value & (uint)offsetMask));
            }
        }

        /// <devdoc>
        ///    returns the raw data stored in this bit vector...
        /// </devdoc>
        public int Data
        {
            get
            {
                return unchecked((int)_data);
            }
        }

        /// <devdoc>
        ///    <para> Creates the first mask in a series.</para>
        /// </devdoc>
        public static int CreateMask()
        {
            return CreateMask(0);
        }

        /// <devdoc>
        ///     Creates the next mask in a series.
        /// </devdoc>
        public static int CreateMask(int previous)
        {
            if (previous == 0)
            {
                return 1;
            }

            if (previous == unchecked((int)0x80000000))
            {
                throw new InvalidOperationException(SR.BitVectorFull);
            }

            return previous << 1;
        }

        /// <devdoc>
        ///    <para>Creates the first section in a series, with the specified maximum value.</para>
        /// </devdoc>
        public static Section CreateSection(short maxValue)
        {
            return CreateSectionHelper(maxValue, 0, 0);
        }

        /// <devdoc>
        ///    <para>Creates the next section in a series, with the specified maximum value.</para>
        /// </devdoc>
        public static Section CreateSection(short maxValue, Section previous)
        {
            return CreateSectionHelper(maxValue, previous.Mask, previous.Offset);
        }

        private static Section CreateSectionHelper(short maxValue, short priorMask, short priorOffset)
        {
            if (maxValue < 1)
            {
                throw new ArgumentException(SR.Format(SR.Argument_InvalidValue_TooSmall, nameof(maxValue), 1), nameof(maxValue));
            }

            short offset = (short)(priorOffset + BitOperations.PopCount((uint)(ushort)priorMask));
            if (offset >= 32)
            {
                throw new InvalidOperationException(SR.BitVectorFull);
            }

            short mask = (short)(BitOperations.RoundUpToPowerOf2((uint)(ushort)maxValue + 1) - 1);
            return new Section(mask, offset);
        }

        public override bool Equals([NotNullWhen(true)] object? o) => o is BitVector32 other && _data == other._data;

        public override int GetHashCode() => _data.GetHashCode();

        public static string ToString(BitVector32 value)
        {
            return string.Create(/*"BitVector32{".Length* /12 + /*32 bits* /32 + /*"}".Length"* /1, value, (dst, v) =>
            {
                ReadOnlySpan<char> prefix = "BitVector32{";
                prefix.CopyTo(dst);
                dst[dst.Length - 1] = '}';

                int locdata = unchecked((int)v._data);
                dst = dst.Slice(prefix.Length, 32);
                for (int i = 0; i < dst.Length; i++)
                {
                    dst[i] = (locdata & 0x80000000) != 0 ? '1' : '0';
                    locdata <<= 1;
                }
            });
        }

        public override string ToString()
        {
            return ToString(this);
        }

        /// <devdoc>
        ///    <para>
        ///       Represents an section of the vector that can contain a integer number.</para>
        /// </devdoc>
        public readonly struct Section
        {
            private readonly short _mask;
            private readonly short _offset;

            internal Section(short mask, short offset)
            {
                _mask = mask;
                _offset = offset;
            }

            public short Mask => _mask;

            public short Offset => _offset;

            public override bool Equals([NotNullWhen(true)] object? o) => o is Section other && Equals(other);

            public bool Equals(Section obj)
            {
                return obj._mask == _mask && obj._offset == _offset;
            }

            public static bool operator ==(Section a, Section b)
            {
                return a.Equals(b);
            }

            public static bool operator !=(Section a, Section b)
            {
                return !(a == b);
            }

            public override int GetHashCode() => HashCode.Combine(_mask, _offset);

            public static string ToString(Section value)
            {
                return $"Section{{0x{value.Mask:x}, 0x{value.Offset:x}}}";
            }

            public override string ToString()
            {
                return ToString(this);
            }
        }
    }
}

 
 */


/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Jay.Collections;

// ReSharper disable TooWideLocalVariableScope

namespace Jay.Text
{
    /// <summary>
    /// Represents an efficiently stored set of characters.
    /// </summary>
    public struct CharSet : ISet<char>, IReadOnlySlice<char>
    {
        /* This was calculated as follows:
         * char.MaxValue == ushort.MaxValue == 65,535 == 256 * 256
         * ASCII range: 0-127
         *
         * So, 256 seems to be a pretty good block size.
         #1#
        private const int BLOCK_SIZE = 256;

        /// <summary>
        /// Upper-bound for any <see langword="for"/> loop iterating over all valid chars
        /// </summary>
        private const int UPPER_BOUND = (int)char.MaxValue + 1; //i < UPPER_BOUND

        private BoolArray?[]? _table;       // Storage
        private int _count;                 //The actual number of chars we contain
        private int _maxBlock;              //The largest block we've put a value into


        #region Properties

        /// <summary>
        /// Gets or sets whether this <see cref="CharSet"/> contains the specified <see cref="char"/>.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool this[char c]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                _table ??= new BoolArray[BLOCK_SIZE];
                var block = c / BLOCK_SIZE;
                if (block > _maxBlock) return false;
                var section = _table[block];
                if (section is null) return false;
                return section[c % BLOCK_SIZE];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _table ??= new bool[BLOCK_SIZE][];
                var block = c / BLOCK_SIZE;
                if (!value && block > _maxBlock) return;
                ref BoolArray? section = ref _table[block];
                if (section is null)
                {
                    if (!value) return; //Already false
                    section = new BoolArray(BLOCK_SIZE);
                }

                var existing = section.GetRef(c % BLOCK_SIZE);
                if (value)
                {
                    if (!existing)
                    {
                        section[c % BLOCK_SIZE] = true;
                        if (block > _maxBlock)
                            _maxBlock = block;
                        _count++;
                    }
                }
                else
                {
                    if (existing)
                    {
                        section[c % BLOCK_SIZE] = false;
                        RecalculateMaxBlock();
                        _count--;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this <see cref="CharSet"/> contains a <see cref="char"/> as specified with a block and offset.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal bool this[int block, int offset]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                _table ??= new bool[BLOCK_SIZE][];
                if (block > _maxBlock) return false;
                var section = _table[block];
                if (section is null) return false;
                return section[offset];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _table ??= new bool[BLOCK_SIZE][];
                if (!value && block > _maxBlock) return;
                ref BoolArray? section = ref _table[block];
                if (section is null)
                {
                    if (!value) return; //Already false
                    section = new BoolArray(BLOCK_SIZE);
                }

                bool existing = section[offset];
                if (value)
                {
                    if (!existing)
                    {
                        section[offset] = true;
                        if (block > _maxBlock)
                            _maxBlock = block;
                        _count++;
                    }
                }
                else
                {
                    if (existing)
                    {
                        section[offset] = false;
                        RecalculateMaxBlock();
                        _count--;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the total number of <see cref="char"/>s stored in this <see cref="CharSet"/>.
        /// </summary>
        public int Count => _count;

        /// <inheritdoc />
        bool ICollection<char>.IsReadOnly => false;

        #endregion /Properties

        #region Constructor
        /// <summary>
        /// Create a new <see cref="CharSet"/> containing the specified characters.
        /// </summary>
        /// <param name="characters"></param>
        public CharSet(params char[] characters)
        {
            _table = new BoolArray[BLOCK_SIZE][];
            _count = _maxBlock = 0;
            AddRange(characters);
        }

        /// <summary>
        /// Create a new <see cref="CharSet"/> containing the specified characters.
        /// </summary>
        /// <param name="characters"></param>
        public CharSet(ReadOnlySpan<char> characters)
        {
            _table = new bool[BLOCK_SIZE][];
            _count = _maxBlock = 0;
            _hashCode = new Lazy<int>(Random.Int);
            AddRange(characters);
        }

        /// <summary>
        /// Create a new <see cref="CharSet"/> containing the specified characters.
        /// </summary>
        /// <param name="characters"></param>
        public CharSet(IEnumerable<char> characters)
        {
            _table = new bool[BLOCK_SIZE][];
            _count = _maxBlock = 0;
            _hashCode = new Lazy<int>(Random.Int);
            AddRange(characters);
        }

        /// <summary>
        /// Create a new <see cref="CharSet"/> containing all the characters in the specified <see cref="string"/>.
        /// </summary>
        /// <param name="text"></param>
        public CharSet(string text)
        {
            _table = new bool[BLOCK_SIZE][];
            _count = _maxBlock = 0;
            _hashCode = new Lazy<int>(Random.Int);
            AddRange((ReadOnlySpan<char>) text);
        }

        /// <summary>
        /// Create a new <see cref="CharSet"/> containing all the characters in the specified <see cref="string"/>s.
        /// </summary>
        /// <param name="strings"></param>
        public CharSet(params string[] strings)
        {
            _table = new bool[BLOCK_SIZE][];
            _count = _maxBlock = 0;
            _hashCode = new Lazy<int>(Random.Int);
            if (strings != null && strings.Length > 0)
            {
                for (var i = 0; i < strings.Length; i++)
                {
                    AddRange((ReadOnlySpan<char>) strings[i]);
                }
            }
        }

        /// <summary>
        /// Create a new <see cref="CharSet"/> containing all the characters in the specified <see cref="string"/>s.
        /// </summary>
        /// <param name="strings"></param>
        public CharSet(IEnumerable<string> strings)
        {
            _table = new bool[BLOCK_SIZE][];
            _count = _maxBlock = 0;
            _hashCode = new Lazy<int>(Random.Int);
            if (strings != null)
            {
                foreach (var text in strings)
                {
                    AddRange((ReadOnlySpan<char>) text);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a <see cref="char"/> from a block and offset.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char GetChar(int block, int offset) => (char) ((block * BLOCK_SIZE) + offset);

        /// <summary>
        /// Gets a block and an offset from a <see cref="char"/>.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (int block, int offset) GetBlockOffset(char c) => (c / BLOCK_SIZE, c % BLOCK_SIZE);

        /// <summary>
        /// Attempts to get whether or not the specified <see cref="char"/> has a value.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetValue(char c, out bool value)
        {
            var block = c / BLOCK_SIZE;
            if (block > _maxBlock)
                return (value = false);
            _table ??= new bool[BLOCK_SIZE][];
            var section = _table[block];
            if (section is null)
                return (value = false); //Could not get because I did not want to create.
            var offset = c % BLOCK_SIZE;
            value = section[offset];
            return true;
        }

        /// <summary>
        /// Gets or creates a ref to the bool that determines if a specific <see cref="char"/> is in this <see cref="CharSet"/>.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref bool GetOrCreateRef(char c)
        {
            var block = c / BLOCK_SIZE;
            _maxBlock = block;
            _table ??= new bool[BLOCK_SIZE][];
            //bool[]? section = _table[block];
            //if (section == null)
            //{
            //    section = (_table[block] = new bool[UPPER_BOUND]);
            //}
            ref bool[]? section = ref _table[block];
            if (section is null)
                section = new bool[UPPER_BOUND];

            var offset = c % BLOCK_SIZE;
            return ref section[offset];
        }

        /// <summary>
        /// Recalculates _maxBlock when we remove a character / characters from this <see cref="CharSet"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RecalculateMaxBlock()
        {
            _table ??= new bool[BLOCK_SIZE][];
            for (var b = BLOCK_SIZE - 1; b >= 0; b--)
            {
                if (_table[b] != null)
                {
                    _maxBlock = b;
                    return;
                }
            }
        }

        #endregion

        #region Public Methods
        /// <inheritdoc />
        public void Add(char c)
        {
            ref bool value = ref GetOrCreateRef(c);
            if (value) return;
            _count++;
            value = true;
        }
        /// <inheritdoc />
        bool ISet<char>.Add(char c)
        {
            ref bool value = ref GetOrCreateRef(c);
            if (value) return false;
            _count++;
            return (value = true);
        }

        /// <summary>
        /// Try to add the specified <see cref="char"/> to this <see cref="CharSet"/>.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool TryAdd(char c)
        {
            ref bool value = ref GetOrCreateRef(c);
            //If the value existed, we did not add
            if (value)
                return false;
            //Adding
            _count++;
            return value = true;
        }

        /// <inheritdoc />
        public void AddRange(params char[] characters)
        {
            if (characters is null) return;
            for (var i = 0; i < characters.Length; i++)
            {
                this[characters[i]] = true;
            }
        }

        /// <inheritdoc />
        public void AddRange(ReadOnlySpan<char> characters)
        {
            for (var i = 0; i < characters.Length; i++)
            {
                this[characters[i]] = true;
            }
        }

        /// <inheritdoc />
        public void AddRange(IEnumerable<char> characters)
        {
            if (characters is null) return;
            foreach (var c in characters)
            {
                this[c] = true;
            }
        }

        /// <inheritdoc />
        public bool Remove(char c)
        {
            int block = c / BLOCK_SIZE;
            if (block > _maxBlock) return false;
            _table ??= new bool[BLOCK_SIZE][];
            var section = _table[block];
            if (section is null)
            {
                //Cannot be true, so cannot be removed.
                return false;
            }

            var offset = c % BLOCK_SIZE;
            //Ref
            ref bool value = ref section[offset];
            //Does it exist?
            if (value)
            {
                //We're removing it
                _count--;
                //Set to false
                value = false;
                //Recalc
                RecalculateMaxBlock();
                //Removed
                return true;
            }

            //Not to remove
            return false;
        }

        /// <summary>
        /// Does this character set contain the specified <see cref="char"/>?
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool Contains(char c) => this[c];

        /// <summary>
        /// Does this <see cref="CharSet"/> contain the specified <see cref="char"/> as determined by the specified <see cref="TextComparer"/>.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(char c, TextComparer comparer)
        {
            if (comparer is null) return this[c];
            _table ??= new bool[BLOCK_SIZE][];
            for (var b = 0; b <= _maxBlock; b++)
            {
                var section = _table[b];
                if (section is null) continue;
                for (var o = 0; o < BLOCK_SIZE; o++)
                {
                    if (section[o] && comparer.Equals(GetChar(b, o), c))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copies the contents of this character set to the specified array.
        /// </summary>
        /// <param name="array"></param>
        public void CopyTo(char[] array) => CopyTo(array, 0);

        /// <summary>
        /// Copies the contents of this character set to the specified array, starting at the specified index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(char[] array, int arrayIndex)
        {
            Validate.CopyTo(_count, array, arrayIndex).ThrowIfError();
            CopyTo(array.AsSpan(arrayIndex));
        }

        /// <summary>
        /// Copies the contents of this character set to the specified span.
        /// </summary>
        /// <param name="span"></param>
        public void CopyTo(Span<char> span)
        {
            Validate.CopyTo(_count, span).ThrowIfError();
            _table ??= new bool[BLOCK_SIZE][];
            var s = 0;
            for (var b = 0; b <= _maxBlock; b++)
            {
                var section = _table[b];
                if (section is null) continue;
                for (var o = 0; o < BLOCK_SIZE; o++)
                {
                    if (section[o])
                        span[s++] = GetChar(b, o);
                }
            }

            Debug.Assert(s == _count);
        }

        #region ISet<char>

        /// <summary>
        /// Is this <see cref="CharSet"/> a proper subset of the specified collection?
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public bool IsProperSubsetOf(IEnumerable<char> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            return IsProperSubsetOf(new CharSet(collection));
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <param name="charSet"></param>
        /// <returns></returns>
        public bool IsProperSubsetOf(CharSet charSet)
        {
            //If we are empty, return true for anything but an empty set.
            if (_count == 0)
                return charSet.Count > 0;
            //If we are the same size or bigger, we cannot be a PROPER subset
            if (_count >= charSet.Count)
                return false;
            _table ??= new bool[BLOCK_SIZE][];
            //Check that they have at least what we have
            for (var b = 0; b <= _maxBlock; b++)
            {
                var section = _table[b];
                if (section is null)
                    continue;
                for (var o = 0; o < BLOCK_SIZE; o++)
                {
                    if (section[o] && !charSet[b, o])
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Is this <see cref="CharSet"/> a subset of the specified collection?
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public bool IsSubsetOf(IEnumerable<char> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            return IsSubsetOf(new CharSet(collection));
        }

        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <param name="charSet"></param>
        /// <returns></returns>
        public bool IsSubsetOf(CharSet charSet)
        {
            //If we are empty, return true for anything.
            if (_count == 0)
                return true;
            //If we are bigger, we cannot be a PROPER subset
            if (_count > charSet._count)
                return false;
            _table ??= new bool[BLOCK_SIZE][];
            //Check that they have at least what we have
            for (var b = 0; b <= _maxBlock; b++)
            {
                var section = _table[b];
                if (section is null)
                    continue;
                for (var o = 0; o < BLOCK_SIZE; o++)
                {
                    if (section[o] && !charSet[b, o])
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Is this <see cref="CharSet"/> a proper superset of the specified collection?
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public bool IsProperSupersetOf(IEnumerable<char> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            return IsProperSupersetOf(new CharSet(collection));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="charSet"></param>
        /// <returns></returns>
        public bool IsProperSupersetOf(CharSet charSet)
        {
            //If we are empty, we cannot be
            if (_count == 0)
                return false;
            //If they are the same size or bigger, we cannot be a PROPER superset
            if (charSet._count >= _count)
                return false;
            //Check that we have at least what they have
            foreach (var c in charSet)
            {
                if (!this[c])
                    return false;
            }

            //We do!
            return true;
        }

        /// <summary>
        /// Is this <see cref="CharSet"/> a superset of the specified collection?
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public bool IsSupersetOf(IEnumerable<char> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            return IsSupersetOf(new CharSet(collection));
        }

        /// <summary>
        /// Is this <see cref="CharSet"/> a superset of the specified <see cref="CharSet"/>?
        /// </summary>
        /// <param name="charSet"></param>
        /// <returns></returns>
        public bool IsSupersetOf(CharSet charSet)
        {
            //If we are empty, they have to be
            if (_count == 0)
                return charSet._count == 0;
            //If they are bigger, we cannot be a PROPER superset
            if (charSet._count > _count)
                return false;
            //Check that we have at least what they have
            foreach (var c in charSet)
            {
                if (!this[c])
                    return false;
            }

            //We do!
            return true;
        }

        /// <summary>
        /// Does this <see cref="CharSet"/> overlap with the specified collection?
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public bool Overlaps(IEnumerable<char> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            return Overlaps(new CharSet(collection));
        }

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="charSet"></param>
        /// <returns></returns>
        public bool Overlaps(CharSet charSet)
        {
            foreach (var c in this)
            {
                if (charSet[c])
                    return true;
            }

            //They don't have anything we have
            return false;
        }

        /// <summary>
        /// Are the contents of this <see cref="CharSet"/> the same as the contents of the specified collection?
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public bool SetEquals(IEnumerable<char> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            return SetEquals(new CharSet(collection));
        }

        /// <summary>
        /// Are the contents of this <see cref="CharSet"/> the same as the contents of the specified <see cref="ReadOnlyCharacterSet"/>?
        /// </summary>
        /// <param name="charSet"></param>
        /// <returns></returns>
        public bool SetEquals(CharSet charSet)
        {
            if (_count != charSet._count)
                return false;
            _table ??= new bool[BLOCK_SIZE][];
            for (var b = 0; b <= _maxBlock; b++)
            {
                var section = _table[b];
                if (section is null)
                    continue;
                for (var o = 0; o < BLOCK_SIZE; o++)
                {
                    //If we have it, they have to have it.
                    if (section[o] && !charSet[b, o])
                        return false;
                }
            }

            //We know counts are the same
            return true;
        }

        /// <inheritdoc />
        public void ExceptWith(IEnumerable<char> other)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (other is null) return;
            //Removes all elements in the specified collection from the current set.
            foreach (var c in other)
            {
                Remove(c);
            }
        }

        /// <inheritdoc />
        public void IntersectWith(IEnumerable<char> other) => IntersectWith(new CharSet(other));

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="charSet"></param>
        public void IntersectWith(CharSet charSet)
        {
            if (charSet.Count == 0)
            {
                Clear();
                return;
            }
            _table ??= new bool[BLOCK_SIZE][];
            for (var b = 0; b <= _maxBlock; b++)
            {
                var section = _table[b];
                if (section is null)
                    continue;
                for (var o = 0; o < BLOCK_SIZE; o++)
                {
                    ref bool value = ref section[o];
                    if (value && !charSet[b, o])
                    {
                        //Remove
                        _count--;
                        value = false;
                    }
                }
            }

            RecalculateMaxBlock();
        }

        /// <inheritdoc />
        public void SymmetricExceptWith(IEnumerable<char> other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));
            SymmetricExceptWith(new CharSet(other));
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the current set or in the specified collection, but not both.
        /// </summary>
        /// <param name="charSet"></param>
        public void SymmetricExceptWith(CharSet charSet)
        {
            char c = char.MinValue;
            for (var i = 0; i < UPPER_BOUND; i++, c++)
            {
                if (this[c])
                {
                    if (charSet[c])
                        this[c] = false;
                }
                else
                {
                    if (charSet[c])
                        this[c] = true;
                }
            }
        }

        /// <inheritdoc />
        public void UnionWith(IEnumerable<char>? other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));
            UnionWith(new CharSet(other));
        }

        /// <summary>
        /// Modifies the current set so that it contains all elements that are present in the current set, in the specified collection, or in both.
        /// </summary>
        /// <param name="charSet"></param>
        public void UnionWith(CharSet charSet)
        {
            _table ??= new bool[BLOCK_SIZE][];
            //Iterate blocks
            for (var b = 0; b < BLOCK_SIZE; b++)
            {
                ref bool[]? section = ref _table[b];
                //Iterate offsets
                for (var o = 0; o < BLOCK_SIZE; o++)
                {
                    //If they have this character
                    if (charSet[b, o])
                    {
                        //See if we have to init the section
                        if (section is null)
                        {
                            section = new bool[BLOCK_SIZE];
                            //section.Initialize();
                        }

                        //Now we have this character
                        if (!section[o])
                        {
                            section[o] = true;
                            _count++;
                        }
                    }
                }
            }

            RecalculateMaxBlock();
        }

        #endregion

        /// <inheritdoc />
        public void Clear()
        {
            _table ??= new bool[BLOCK_SIZE][];
            for (var b = 0; b < BLOCK_SIZE; b++)
            {
                ref bool[]? section = ref _table[b];
                section?.Initialize();
            }
            _count = _maxBlock = 0;
        }


        public char[] ToArray()
        {
            var array = new char[_count];
            CopyTo(array);
            return array;
        }
        #endregion

        /// <inheritdoc />
        public IEnumerator<char> GetEnumerator()
        {
            _table ??= new bool[BLOCK_SIZE][];
            for (var b = 0; b <= _maxBlock; b++)
            {
                var section = _table[b];
                if (section is null)
                    continue;
                for (var o = 0; o < BLOCK_SIZE; o++)
                {
                    if (section[o])
                        yield return GetChar(b, o);
                }
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is CharSet set)
            {
                return SetEquals(set);
            }

            if (obj is char[] array)
            {
                return SetEquals(new CharSet(array));
            }

            if (obj is string text)
            {
                return SetEquals(new CharSet(text));
            }

            if (obj is IEnumerable<char> enumerable)
            {
                return SetEquals(enumerable);
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode() => _hashCode.Value;

        /// <inheritdoc />
        public override string ToString()
        {
            _table ??= new bool[BLOCK_SIZE][];
            return string.Create(_count, this, (span, me) =>
            {
                var i = 0;
                for (var b = 0; b < me._maxBlock; b++)
                {
                    var section = me._table![b];
                    if (section is null)
                        continue;
                    for (var o = 0; o < BLOCK_SIZE; o++)
                    {
                        if (section[o])
                            span[i++] = GetChar(b, o);
                    }
                }
            });
        }

        public static implicit operator CharSet(char[] array) => new CharSet(array);
        public static implicit operator CharSet(ReadOnlySpan<char> span) => new CharSet(span);
        public static implicit operator CharSet(string text) => new CharSet(text);

        public static CharSet operator |(CharSet x, CharSet y)
        {
            var set = new CharSet();
            for (var block = 0; block < BLOCK_SIZE; block++)
            {
                for (var offset = 0; offset < BLOCK_SIZE; offset++)
                {
                    if (x[block, offset] || y[block, offset])
                        set[block, offset] = true;
                }
            }

            return set;
        }

        public static CharSet operator &(CharSet x, CharSet y)
        {
            var set = new CharSet();
            for (var block = 0; block < BLOCK_SIZE; block++)
            {
                for (var offset = 0; offset < BLOCK_SIZE; offset++)
                {
                    if (x[block, offset] && y[block, offset])
                        set[block, offset] = true;
                }
            }

            return set;
        }

        public static CharSet operator ^(CharSet x, CharSet y)
        {
            var set = new CharSet();
            for (var block = 0; block < BLOCK_SIZE; block++)
            {
                for (var offset = 0; offset < BLOCK_SIZE; offset++)
                {
                    if (x[block, offset] ^ y[block, offset])
                        set[block, offset] = true;
                }
            }

            return set;
        }

        public static CharSet operator !(CharSet x)
        {
            var set = new CharSet();
            for (var block = 0; block < x._maxBlock; block++)
            {
                for (var offset = 0; offset < BLOCK_SIZE; offset++)
                {
                    if (!x[block, offset])
                        set[block, offset] = true;
                }
            }

            return set;
        }
    }
}
*/
