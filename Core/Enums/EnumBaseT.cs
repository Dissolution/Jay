using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay
{
    [StructLayout(LayoutKind.Sequential)]
    public abstract class EnumBase<TEnum> : IEquatable<TEnum>, IComparable<TEnum>
        where TEnum : EnumBase<TEnum>
    {
        public static bool operator ==(EnumBase<TEnum>? x, TEnum? y) => Equals(x, y);
        public static bool operator !=(EnumBase<TEnum>? x, TEnum? y) => !Equals(x, y);
        public static bool operator >(EnumBase<TEnum>? x, TEnum? y) => Compare(x, y) > 0;
        public static bool operator >=(EnumBase<TEnum>? x, TEnum? y) => Compare(x, y) >= 0;
        public static bool operator <(EnumBase<TEnum>? x, TEnum? y) => Compare(x, y) < 0;
        public static bool operator <=(EnumBase<TEnum>? x, TEnum? y) => Compare(x, y) <= 0;
        
        protected static readonly Type _enumType = typeof(TEnum);
        protected static readonly TEnum[] _members;

        public static IReadOnlyList<TEnum> Members => _members;
        
        static EnumBase()
        {
            // Find all of our member fields / properties
            var enums = new List<TEnum>();
            foreach (var memberInfo in typeof(TEnum).GetMembers(BindingFlags.Public | BindingFlags.Static))
            {
                if (memberInfo is FieldInfo fieldInfo &&
                    fieldInfo.FieldType == _enumType)
                {
                    var e = (fieldInfo.GetValue(null) as TEnum)!;
                    if (e._memberValue is null)
                        e._memberValue = enums.Count;
                    if (e._memberName is null)
                        e._memberName = fieldInfo.Name;
                    enums.Add(e);
                }
                else if (memberInfo is PropertyInfo propertyInfo &&
                         propertyInfo.PropertyType == _enumType)
                {
                    var e = (propertyInfo.GetValue(null) as TEnum)!;
                    if (e._memberValue is null)
                        e._memberValue = enums.Count;
                    if (e._memberName is null)
                        e._memberName = propertyInfo.Name;
                    enums.Add(e);
                }
            }
            _members = enums.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Equals(EnumBase<TEnum>? x, EnumBase<TEnum>? y) => x?._memberValue == y?._memberValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Compare(EnumBase<TEnum>? x, EnumBase<TEnum>? y)
        {
            int? xValue = x?._memberValue;
            int? yValue = y?._memberValue;
            if (xValue == yValue) return 0;
            if (xValue is null)
                return -1;
            if (yValue is null)
                return 1;
            return xValue.Value.CompareTo(yValue.Value);
        }

        public static bool TryParse(int value, [MaybeNullWhen(false)] out TEnum? @enum)
        {
            foreach (var member in _members)
            {
                if (member._memberValue == value)
                {
                    @enum = member;
                    return true;
                }
            }
            @enum = null;
            return false;
        }
        
        public static bool TryParse(string? name, [MaybeNullWhen(false)] out TEnum? @enum)
        {
            foreach (var member in _members)
            {
                if (string.Equals(member._memberName, name, StringComparison.OrdinalIgnoreCase))
                {
                    @enum = member;
                    return true;
                }
            }
            @enum = null;
            return false;
        }


        private int? _memberValue;
        private string? _memberName;

        public bool IsDefault => _memberValue.GetValueOrDefault(0) == 0;
        
        protected EnumBase()
        {
            _memberValue = null;
            _memberName = null;
        }

        protected EnumBase([CallerMemberName] string? name = null)
        {
            _memberValue = null;
            _memberName = name;
        }
        
        protected EnumBase(int value, [CallerMemberName] string? name = null)
        {
            _memberValue = value;
            _memberName = name;
        }

        public int CompareTo(TEnum? @enum) => Compare(this, @enum);

        public bool Equals(TEnum? @enum) => Equals(this, @enum);

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return _memberValue.GetValueOrDefault(0) == 0;
            if (obj is TEnum @enum)
                return Equals(@enum);
            return false;
        }

        public override int GetHashCode()
        {
            return _memberValue!.GetValueOrDefault(0);
        }

        public override string ToString()
        {
            return _memberName!;
        }
    }
}