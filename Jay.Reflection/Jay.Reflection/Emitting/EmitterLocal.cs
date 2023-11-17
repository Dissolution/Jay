#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay.Reflection.Emitting;

public readonly struct EmitterLocal :
#if NET7_0_OR_GREATER
    IEqualityOperators<EmitterLocal, EmitterLocal, bool>, 
    IEqualityOperators<EmitterLocal, LocalBuilder, bool>, 
    IEqualityOperators<EmitterLocal, LocalVariableInfo, bool>,
#endif
    IEquatable<EmitterLocal>,
    IEquatable<LocalBuilder>,
    IEquatable<LocalVariableInfo>
{
    public static bool operator ==(EmitterLocal left, EmitterLocal right) => left.Equals(right);
    public static bool operator !=(EmitterLocal left, EmitterLocal right) => !left.Equals(right);
    public static bool operator ==(EmitterLocal left, LocalBuilder? right) => left.Equals(right);
    public static bool operator !=(EmitterLocal left, LocalBuilder? right) => !left.Equals(right);
    public static bool operator ==(EmitterLocal left, LocalVariableInfo? right) => left.Equals(right);
    public static bool operator !=(EmitterLocal left, LocalVariableInfo? right) => !left.Equals(right);

    public string Name { get; }
    public ushort Index { get; }
    public Type Type { get; }
    public bool IsPinned { get; }
    public bool IsShortForm => Index <= byte.MaxValue;

    public EmitterLocal(string name, ushort index, Type type, bool isPinned = false)
    {
        Throw.IfNullOrEmpty(name);
        this.Name = name;
        if (index == ushort.MaxValue)
            throw new ArgumentException("65535 maximum indexes supported", nameof(index));

        this.Index = index;
        Throw.IfNull(type);
        this.Type = type;
        this.IsPinned = isPinned;
    }

    public EmitterLocal(string name, LocalBuilder localBuilder)
    {
        Throw.IfNullOrEmpty(name);
        this.Name = name;
        Throw.IfNull(localBuilder);
        this.Index = (ushort)localBuilder.LocalIndex;
        this.Type = localBuilder.LocalType.ThrowIfNull();
        this.IsPinned = localBuilder.IsPinned;
    }

    public EmitterLocal(string name, LocalVariableInfo localVariableInfo)
    {
        Throw.IfNullOrEmpty(name);
        this.Name = name;
        Throw.IfNull(localVariableInfo);
        this.Index = (ushort)localVariableInfo.LocalIndex;
        this.Type = localVariableInfo.LocalType.ThrowIfNull();
        this.IsPinned = localVariableInfo.IsPinned;
    }

    public bool Equals(EmitterLocal emitterLocal)
    {
        return Easy.FastEqual(this.Name, emitterLocal.Name)
            && Easy.FastEqual(this.Index, emitterLocal.Index)
            && Easy.FastEqual(this.Type, emitterLocal.Type)
            && Easy.FastEqual(this.IsPinned, emitterLocal.IsPinned);
    }

    public bool Equals(LocalBuilder? localBuilder)
    {
        return localBuilder is not null
            && Easy.FastEqual(this.Index, localBuilder.LocalIndex)
            && Easy.FastEqual(this.Type, localBuilder.LocalType)
            && Easy.FastEqual(this.IsPinned, localBuilder.IsPinned);
    }

    public bool Equals(LocalVariableInfo? localVariableInfo)
    {
        return localVariableInfo is not null
            && Easy.FastEqual(this.Index, localVariableInfo.LocalIndex)
            && Easy.FastEqual(this.Type, localVariableInfo.LocalType)
            && Easy.FastEqual(this.IsPinned, localVariableInfo.IsPinned);
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            EmitterLocal emitterLocal => Equals(emitterLocal),
            LocalBuilder localBuilder => Equals(localBuilder),
            LocalVariableInfo localVariableInfo => Equals(localVariableInfo),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        return Index;
    }
    
    public override string ToString() => TextBuilder.New
        .Append($"[{Index}] {Type} {Name}")
        .If(IsPinned, cb => cb.Write(" 📌"))
        .ToStringAndDispose();
}