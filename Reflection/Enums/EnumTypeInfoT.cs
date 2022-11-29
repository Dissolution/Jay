using System.Diagnostics;
using System.Numerics;
using Jay.Text;
using Jay.Comparision;
using Jay.Enums;

namespace Jay.Reflection.Enums;

public sealed class EnumTypeInfo<TEnum> : EnumTypeInfo
    where TEnum : struct, Enum
{
    private readonly TEnum[] _members;

    public IReadOnlyList<TEnum> Members => _members;

    public EnumComparer<TEnum> Comparer { get; } = new EnumComparer<TEnum>();

    public EnumTypeInfo() : base(typeof(TEnum))
    {
        _enumFields = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        _memberCount = _enumFields.Length;
        _members = new TEnum[_memberCount];
        _names = new string[_memberCount];
        _values = new ulong[_memberCount];
        for (var i = 0; i < _memberCount; i++)
        {
            var field = _enumFields[i];
            var member = (TEnum)field.GetValue(null)!;
            _members[i] = member;
            _names[i] = field.Name;
            _values[i] = member.ToUInt64();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int FindIndex(ulong value)
    {
        // Values are sorted ascending
        int index = Array.BinarySearch(_values, value);
        // Normalize to -1 as a failed match
        if ((uint)index > _memberCount)
            return -1;
        return index;
    }

    private int FindIndex(TEnum @enum)
    {
        // Search by value
        ulong value = @enum.ToUInt64();
        // Values are sorted ascending
        int index = Array.BinarySearch(_values, value);
        // Normalize to -1 as a failed match
        if ((uint)index > _memberCount)
            return -1;
        return index;
    }


    private int[] GetFlagIndices(ulong enumValue)
    {
        if (enumValue == 0UL)
        {
            if (_memberCount > 0)
                return new int[1] { 0 };
            return Array.Empty<int>();
        }

        Span<int> flagIndices = stackalloc int[64];
        var values = _values;
        ulong value;
        int index = _values.Length - 1;
        while (index >= 0)
        {
            value = values[index];
            if (value == enumValue)
            {
                return new int[1] { index };
            }
            // Cannot be matched
            if (value < enumValue)
            {
                break;
            }

            index--;
        }

        // Now look for multiple matches, storing the indices of the values into our span.
        int flagCount = 0;
        while (index >= 0)
        {
            ulong currentValue = values[index];
            if (index == 0 && currentValue == 0)
            {
                break;
            }

            if ((enumValue & currentValue) == currentValue)
            {
                enumValue -= currentValue;
                flagIndices[flagCount++] = index;
            }

            index--;
        }

        if (enumValue != 0)
        {
            Debugger.Break();
            return Array.Empty<int>();
        }

        Debug.Assert(flagCount > 0);
        // Reverse flag indexes?
        var indices = new int[flagCount];
        var i = 0;
        indices[i++] = flagIndices[--flagCount];
        while (--flagCount >= 0)
        {
            indices[i++] = flagIndices[flagCount];
        }

        Debug.Assert(i == indices.Length);

        return indices;
    }

    public TEnum[] GetFlags(TEnum @enum)
    {
        var value = @enum.ToUInt64();
        var count = BitOperations.PopCount(value);
        var members = new TEnum[count];
        int m = 0;
        var mask = 1UL;
        for (var i = 0; i < 64; i++)
        {
            if ((value & mask) == mask)
            {
                members[m++] = EnumInfo.FromUInt64<TEnum>(mask);
            }
            mask <<= 1;
        }

        var flagIndices = GetFlagIndices(value);
        var membersB = new TEnum[flagIndices.Length];
        for (var i = 0; i < flagIndices.Length; i++)
        {
            membersB[i] = _members[flagIndices[i]];
        }

        var eq = EnumerableEqualityComparer<TEnum>.Default.Equals(members, membersB);
        Debug.Assert(eq);

        return members;
    }

    public string GetString(TEnum @enum)
    {
        // Search by value
        ulong value = @enum.ToUInt64();
        // Values are sorted ascending
        int index = Array.BinarySearch(_values, value);
        // Exact match?
        if ((uint)index <= _memberCount)
        {
            return _names[index];
        }

        var writer = new CharSpanBuilder();
        var indices = GetFlagIndices(value);
        for (var i = 0; i < indices.Length; i++)
        {
            if (i > 0) writer.Write(" | ");
            writer.Write(_names[indices[i]]);
        }
        return writer.ToStringAndDispose();
    }

    public bool TryParse(string? text, out TEnum @enum)
    {
        if (string.IsNullOrEmpty(text))
        {
            @enum = default;
            return false;
        }

        // Does it contain flag separators?
        if (text.Contains('|') || text.Contains(','))
        {
            var flagTexts = text.Split(new char[2] { '|', ',' },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            @enum = default;
            foreach (var flagText in flagTexts)
            {
                if (TryParse(flagText, out var flag))
                {
                    @enum.AddFlag(flag);
                }
                else
                {
                    @enum = default;
                    return false;
                }
            }
            return true;
        }

        for (var i = 0; i < _memberCount; i++)
        {
            if (string.Equals(_names[i], text, StringComparison.OrdinalIgnoreCase))
            {
                @enum = _members[i];
                return true;
            }
        }

        if (ulong.TryParse(text, out ulong value))
        {
            var index = FindIndex(value);
            if (index >= 0)
            {
                @enum = _members[index];
                return true;
            }
        }

        @enum = default;
        return false;
    }
}