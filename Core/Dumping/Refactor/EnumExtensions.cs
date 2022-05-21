using System.Diagnostics;
using System.Reflection;
using InlineIL;

namespace Jay.Dumping.Refactor;

public static class EnumExtensions
{
    public sealed class EnumInfo<TEnum> : IEquatable<EnumInfo<TEnum>>, IEquatable<TEnum>
        where TEnum : struct, Enum
    {
        private readonly TEnum _enum;
        private readonly string _name;
        private readonly Attribute[] _attributes;

        public TEnum Enum => _enum;
        public string Name => _name;
        public IReadOnlyList<Attribute> Attributes => _attributes;

        public EnumInfo(FieldInfo enumMemberField)
        {
            _name = enumMemberField.Name;
            _attributes = Attribute.GetCustomAttributes(enumMemberField, true);
            _enum = (TEnum)enumMemberField.GetValue(null)!;
        }

        public EnumInfo(TEnum flag, string? name = null)
        {
            _enum = flag;
            _name = name ?? flag.ToString();
            _attributes = Array.Empty<Attribute>();
        }

        public bool Equals(EnumInfo<TEnum>? enumInfo)
        {
            return enumInfo is not null && EnumTypeInfo<TEnum>.Equals(Enum, enumInfo.Enum);
        }

        public bool Equals(TEnum @enum)
        {
            return EnumTypeInfo<TEnum>.Equals(Enum, @enum);
        }

        public override bool Equals(object? obj)
        {
            if (obj is EnumInfo<TEnum> enumInfo) return Equals(enumInfo);
            if (obj is TEnum @enum) return Equals(@enum);
            if (obj is Enum) Debugger.Break();
            return false;
        }

        public override int GetHashCode()
        {
            IL.Emit.Ldarg_0();
            IL.Emit.Ldfld(FieldRef.Field(typeof(EnumInfo<TEnum>), nameof(_enum)));
            IL.Emit.Conv_I4();
            return IL.Return<int>();
        }

        public override string ToString()
        {
            return $"{EnumTypeInfo<TEnum>.Name}.{Name}";
        }
    }
    
    public static class EnumTypeInfo<TEnum> where TEnum : struct, Enum
    {
        static EnumTypeInfo()
        {
            Type = typeof(TEnum);
            Debug.Assert(Type.IsEnum);
            Attributes = Attribute.GetCustomAttributes(Type, true);
            Name = Type.Dump();

            var fields = Type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var infos = new Dictionary<TEnum, EnumInfo<TEnum>>(fields.Length, EnumComparer<TEnum>.Default);
            foreach (var field in fields)
            {
                var info = new EnumInfo<TEnum>(field);
                infos.Add(info.Enum, info);
            }
            _enumInfos = infos;
        }

        public static Type Type { get; }
        public static Attribute[] Attributes { get; }
        public static string Name { get; }

        public static IReadOnlyCollection<TEnum> Members => _enumInfos.Keys;

        public static EnumInfo<TEnum> GetInfo(TEnum e)
        {
            if (_enumInfos.TryGetValue(e, out var info))
                return info;
            return new EnumInfo<TEnum>(e);
        }

        private static readonly Dictionary<TEnum, EnumInfo<TEnum>> _enumInfos;

    }

    public static EnumInfo<TEnum> GetInfo<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        return EnumTypeInfo<TEnum>.GetInfo(@enum);
    }
}