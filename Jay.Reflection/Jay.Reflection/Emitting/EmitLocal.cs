using Jay.Reflection.Validation;
using Validate = Jay.Validation.Validate;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay.Reflection.Emitting;

public readonly struct EmitLocal : 
    #if NET7_0_OR_GREATER
    IEqualityOperators<EmitLocal, EmitLocal, bool>, 
    IEqualityOperators<EmitLocal, LocalBuilder, bool>, 
    IEqualityOperators<EmitLocal, LocalVariableInfo, bool>,
    #endif
    IEquatable<EmitLocal>, 
    IEquatable<LocalBuilder>,
    IEquatable<LocalVariableInfo>,
    IToCode
{
    public static bool operator ==(EmitLocal left, EmitLocal right) => left.Equals(right);
    public static bool operator !=(EmitLocal left, EmitLocal right) => !left.Equals(right);
    public static bool operator ==(EmitLocal left, LocalBuilder? right) => left.Equals(right);
    public static bool operator !=(EmitLocal left, LocalBuilder? right) => !left.Equals(right);
    public static bool operator ==(EmitLocal left, LocalVariableInfo? right) => left.Equals(right);
    public static bool operator !=(EmitLocal left, LocalVariableInfo? right) => !left.Equals(right);

    public string Name { get; }
    public ushort Index { get; }
    public Type Type { get; }
    public bool IsPinned { get; }
    public bool IsShortForm => Index <= byte.MaxValue;

    public EmitLocal(string name, ushort index, Type type, bool isPinned = false)
    {
        
        Validate.IsNotNullOrEmpty(name);
        this.Name = name;
        if (index == ushort.MaxValue)
            throw new ArgumentException("65535 maximum indexes supported", nameof(index));
        this.Index = index;
        Validate.IsNotNull(type);
        this.Type = type;
        this.IsPinned = isPinned;
    }
    public EmitLocal(string name, LocalBuilder localBuilder)
    {
        Validate.IsNotNullOrEmpty(name);
        this.Name = name;
        Validate.IsNotNull(localBuilder);
        this.Index = (ushort)localBuilder.LocalIndex;
        this.Type = localBuilder.LocalType;
        this.IsPinned = localBuilder.IsPinned;
    }
    public EmitLocal(string name, LocalVariableInfo localVariableInfo)
    {
        Validate.IsNotNullOrEmpty(name);
        this.Name = name;
        Validate.IsNotNull(localVariableInfo);
        this.Index = (ushort)localVariableInfo.LocalIndex;
        this.Type = localVariableInfo.LocalType;
        this.IsPinned = localVariableInfo.IsPinned;
    }

    public bool Equals(EmitLocal emitLocal)
    {
        return Easy.Equal(this.Name, emitLocal.Name) &&
            Easy.Equal(this.Index, emitLocal.Index) &&
            Easy.Equal(this.Type, emitLocal.Type) &&
            Easy.Equal(this.IsPinned, emitLocal.IsPinned);
    }

    public bool Equals(LocalBuilder? localBuilder)
    {
        return localBuilder is not null && 
            Easy.Equal(this.Index, localBuilder.LocalIndex) &&
            Easy.Equal(this.Type, localBuilder.LocalType) &&
            Easy.Equal(this.IsPinned, localBuilder.IsPinned);
    }

    public bool Equals(LocalVariableInfo? localVariableInfo)
    {
        return localVariableInfo is not null && 
            Easy.Equal(this.Index, localVariableInfo.LocalIndex) &&
            Easy.Equal(this.Type, localVariableInfo.LocalType) &&
            Easy.Equal(this.IsPinned, localVariableInfo.IsPinned);
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            EmitLocal emitterLocal => Equals(emitterLocal),
            LocalBuilder localBuilder => Equals(localBuilder),
            LocalVariableInfo localVariableInfo => Equals(localVariableInfo),
            _ => false,
        };
    }
    public override int GetHashCode()
    {
        return Index;
    }

    public void WriteCodeTo(CodeBuilder codeBuilder)
    {
        codeBuilder.Append('[');
        codeBuilder.Append(this.Index);
        codeBuilder.Append("] ");
        codeBuilder.Append(this.Type);
        codeBuilder.Append(' ');
        codeBuilder.Append(this.Name);
        if (this.IsPinned)
        {
            codeBuilder.Append(" 📌");
        }
    }

    public override string ToString() => CodeBuilder.Render(this);
}