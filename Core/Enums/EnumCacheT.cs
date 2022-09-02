using Jay.Reflection;
using System.Diagnostics;
using System.Reflection;
using Jay.Text;
using static InlineIL.IL;
using System;

namespace Jay.Enums
{
    public abstract class EnumTypeCache
    {
        protected internal readonly Attribute[] _attributes;
        protected internal readonly string[] _names;
        protected internal readonly ulong[] _values;

        public Type EnumType { get; }
        public string Name { get; }
        public IReadOnlyList<Attribute> Attributes => _attributes;
        public bool HasFlags { get; }
        public int MemberCount => _values.Length;
        public IReadOnlyList<string> Names => _names;
        public abstract IReadOnlyList<object> MemberObjects { get; }

        protected EnumTypeCache(Type enumType, FieldInfo[] fields)
        {
            Debug.Assert(enumType.IsEnum);
            this.EnumType = enumType;
            _attributes = Attribute.GetCustomAttributes(enumType);
            HasFlags = _attributes.OfType<FlagsAttribute>().Any();
            int len = fields.Length;
            var names = new string[len];
            var values = new ulong[len];
            for (var i = 0; i < len; i++)
            {
                var field = fields[i];
                names[i] = field.Name;
                object? cv = field.GetRawConstantValue();
                Debugger.Break();
                values[i] = (ulong)cv;
            }
            _names = names;
            _values = values;
        }
    }

    public static class EnumCache<TEnum>
        where TEnum : struct, Enum
    {
        private static readonly Type _enumType;
        private static readonly bool _hasFlagsAttribute;
        private static readonly int _memberCount;
        private static readonly string[] _names;
        private static readonly TEnum[] _members;
        private static readonly ulong[] _values;

        public static bool HasFlags => _hasFlagsAttribute;
        public static int MemberCount => _memberCount;
        public static IReadOnlyList<string> Names => _names;
        public static IReadOnlyList<TEnum> Members => _members;

        static EnumCache()
        {
            _enumType = typeof(TEnum);
            _hasFlagsAttribute = typeof(TEnum).HasAttribute<FlagsAttribute>();
            var fields = typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static);
            int len = fields.Length;
            _memberCount = len;
            var names = new string[len];
            var values = new ulong[len];
            var members = new TEnum[len];
            TEnum member;
            for (var i = 0; i < len; i++)
            {
                var field = fields[i];
                names[i] = field.Name;
                member = (TEnum)field.GetValue(null)!;
                members[i] = member;
                values[i] = AsUInt64(member);
            }
            _names = names;
            _members = members;
            _values = values;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong AsUInt64(TEnum @enum)
        {
            Emit.Ldarg(nameof(@enum));
            Emit.Conv_U8();
            return Return<ulong>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefault(TEnum @enum)
        {
            Emit.Ldarg(nameof(@enum));
            Emit.Ldc_I4_0();
            Emit.Ceq();
            return Return<bool>();
        }

        private static int[] GetFlagIndices(ulong enumValue)
        {
            if (enumValue == 0UL)
            {
                if (_memberCount > 0)
                    return new int[1] { 0 };
                return Array.Empty<int>();
            }

            Span<int> flagBits = stackalloc int[64];
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
                    flagBits[flagCount++] = index;
                }

                index--;
            }

            if (enumValue != 0)
            {
                Debugger.Break();
                return Array.Empty<int>();
            }

            Debug.Assert(flagCount > 0);
            var indices = new int[flagCount];
            var i = 0;
            indices[i++] = flagBits[--flagCount];
            while (--flagCount >= 0)
            {
                indices[i++] = flagBits[flagCount];
            }

            Debug.Assert(i == indices.Length);

            return indices;
        }

        public static TEnum[] GetFlags(TEnum @enum)
        {
            var value = AsUInt64(@enum);
            var flagIndices = GetFlagIndices(value);
            var members = new TEnum[flagIndices.Length];
            for (var i = 0; i < flagIndices.Length; i++)
            {
                members[i] = _members[flagIndices[i]];
            }
            return members;
        }

        private static int FindIndex(TEnum @enum)
        {
            // Search by value
            ulong value = AsUInt64(@enum);
            // Values are sorted ascending
            int index = Array.BinarySearch<ulong>(_values, value);
            // Normalize to -1 as a failed match
            if ((uint)index > _memberCount)
                return -1;
            return index;
        }

        public static string GetString(TEnum @enum)
        {
            // Search by value
            ulong value = AsUInt64(@enum);
            // Values are sorted ascending
            int index = Array.BinarySearch<ulong>(_values, value);
            // Exact match?
            if ((uint)index <= _memberCount)
            {
                return _names[index];
            }

            return TextBuilder.Build(value, 
                (text, enumValue) 
                    => text.AppendDelimit(" | ", 
                        GetFlagIndices(enumValue), 
                        (tb, flagIndex) => tb.Write(_names[flagIndex])));
        }
    }
}
