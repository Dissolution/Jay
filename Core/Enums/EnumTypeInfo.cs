﻿using System.Reflection;
using Jay.Collections;

namespace Jay.Enums;

public static class EnumInfo
{
    private static readonly ConcurrentTypeDictionary<EnumTypeInfo> _cache;

    static EnumInfo()
    {
        _cache = new ConcurrentTypeDictionary<EnumTypeInfo>();
    }

    public static EnumTypeInfo<TEnum> For<TEnum>()
        where TEnum : struct, Enum
    {
        return (_cache.GetOrAdd<TEnum>(_ => new EnumTypeInfo<TEnum>()) as EnumTypeInfo<TEnum>)!;
    }

    public static EnumMemberInfo<TEnum> For<TEnum>(TEnum @enum)
        where TEnum : struct, Enum
    {
        return For<TEnum>().GetMemberInfo(@enum);
    }
}


public abstract class EnumTypeInfo
{
    private readonly Attribute[] _attributes;

    public Type EnumType { get; }
    public IReadOnlyList<Attribute> Attributes => _attributes;
    public bool HasFlags { get; }
    public string Name { get; }

    protected EnumTypeInfo(Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        if (!enumType.IsEnum || !enumType.IsValueType)
            throw new ArgumentException("You must specify a valid enum type", nameof(enumType));
        this.EnumType = enumType;
        _attributes = Attribute.GetCustomAttributes(EnumType, true);
        this.HasFlags = _attributes.OfType<FlagsAttribute>().Any();
        this.Name = EnumType.Name;
    }

    public override string ToString()
    {
        return $"typeof({EnumType.Name}";
    }
}

public class EnumTypeInfo<TEnum> : EnumTypeInfo
    where TEnum : struct, Enum
{
    private readonly Dictionary<TEnum, EnumMemberInfo<TEnum>> _enumMemberInfos;
    
    public IReadOnlyCollection<TEnum> Members => _enumMemberInfos.Keys;
    
    protected internal EnumTypeInfo()
        : base(typeof(TEnum))
    {
        var fields = EnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        var infos = new Dictionary<TEnum, EnumMemberInfo<TEnum>>(fields.Length, EnumComparer<TEnum>.Default);
        foreach (var field in fields)
        {
            var info = new EnumMemberInfo<TEnum>(field);
            infos.Add(info.Enum, info);
        }
        _enumMemberInfos = infos;
    }

    public EnumMemberInfo<TEnum> GetMemberInfo(TEnum @enum)
    {
        return _enumMemberInfos.GetOrAdd(@enum, e => new EnumMemberInfo<TEnum>(e));
    }
    
    public static bool TryParse(ReadOnlySpan<char> value, out TEnum @enum)
    {
        if (value.Length == 0)
        {
            @enum = default;
            return false;
        }

        // Try to use the built-in parser
        if (Enum.TryParse(value, true, out @enum))
        {
            return true;
        }
        
        // TODO: Check int value and string values against name/dump

        @enum = default;
        return false;
    }
}