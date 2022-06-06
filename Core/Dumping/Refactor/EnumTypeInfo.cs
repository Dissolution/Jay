using System.Diagnostics;
using System.Reflection;

namespace Jay.Dumping.Refactor;

public static class EnumTypeInfo<TEnum> where TEnum : struct, Enum
{
    static EnumTypeInfo()
    {
        Type = typeof(TEnum);
        Debug.Assert(Type.IsEnum);
        Attributes = Attribute.GetCustomAttributes(Type, true);
        Name = Type.Dump();

        var fields = Type.GetFields(BindingFlags.Public | BindingFlags.Static);
        var infos = new Dictionary<TEnum, EnumMemberInfo<TEnum>>(fields.Length, EnumComparer<TEnum>.Default);
        foreach (var field in fields)
        {
            var info = new EnumMemberInfo<TEnum>(field);
            infos.Add(info.Enum, info);
        }
        _enumInfos = infos;
    }

    public static Type Type { get; }
    public static Attribute[] Attributes { get; }
    public static string Name { get; }

    public static IReadOnlyCollection<TEnum> Members => _enumInfos.Keys;

    public static EnumMemberInfo<TEnum> GetInfo(TEnum e)
    {
        if (_enumInfos.TryGetValue(e, out var info))
            return info;
        return new EnumMemberInfo<TEnum>(e);
    }

    private static readonly Dictionary<TEnum, EnumMemberInfo<TEnum>> _enumInfos;

}