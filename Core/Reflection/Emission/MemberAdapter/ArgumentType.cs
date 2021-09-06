using System;
using System.Diagnostics;
using System.Reflection;
using Jay.Debugging.Dumping;
using Jay.Text;

namespace Jay.Reflection.Emission
{
    public class ArgumentType : IEquatable<ArgumentType>,
                                IEquatable<Type>
    {
        public static implicit operator Type(ArgumentType argumentType) => argumentType.Type;
        public static implicit operator ArgumentType(Type type) => new ArgumentType(type);
        public static implicit operator ArgumentType(ParameterInfo parameterInfo) => new ParameterType(parameterInfo);

        public static bool operator ==(ArgumentType? left, ArgumentType? right) => left?.Type == right?.Type;
        public static bool operator !=(ArgumentType? left, ArgumentType? right) => !(left == right);

        public static bool operator ==(ArgumentType? left, Type? right) => left?.Type == right;
        public static bool operator !=(ArgumentType? left, Type? right) => !(left == right);

        internal Type Type { get; }

        public ParameterModifier Modifier { get; set; }
        public Type RootType { get; protected set; }
        public string Name { get; protected set; }
        public bool IsValueType => RootType.IsValueType;

        public ArgumentType(Type type)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Name = Dumper.Format($"({Type})");
            if (type.IsByRef)
            {
                this.Modifier = ParameterModifier.Ref;
                this.RootType = type.GetElementType()!;
            }
            else if (type.IsByRefLike)
            {
                Debugger.Break();
                this.Modifier = ParameterModifier.Ref;
                this.RootType = type.GenericTypeArguments[0];
            }
            else if (type.IsPointer)
            {
                this.Modifier = ParameterModifier.Pointer;
                this.RootType = type.GetElementType()!;
            }
            else
            {
                this.Modifier = ParameterModifier.Default;
                this.RootType = type;
            }
        }

        /// <inheritdoc />
        public bool Equals(ArgumentType? argumentType)
        {
            if (argumentType is null) return false;
            return argumentType.Type == Type;
        }

        /// <inheritdoc />
        public bool Equals(Type? type)
        {
            if (type is null) return false;
            return type == Type;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            switch (obj)
            {
                case Type type:
                    return type == Type;
                case ArgumentType adapterType:
                    return adapterType.Type == Type;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public override int GetHashCode() => Type.GetHashCode();

        /// <inheritdoc />
        public override string ToString()
        {
            return TextBuilder.Build(this, (text, argumentType) =>
            {
                switch (argumentType.Modifier)
                {
                    case ParameterModifier.In:
                        text.Append("in ");
                        break;
                    case ParameterModifier.Ref:
                        text.Append("ref ");
                        break;
                    case ParameterModifier.Out:
                        text.Append("out ");
                        break;
                    case ParameterModifier.Pointer:
                    case ParameterModifier.Default:
                    default:
                        break;
                }

                text.AppendDump(argumentType.RootType);
                if (argumentType.Modifier == ParameterModifier.Pointer)
                    text.Append('*');
            });
        }
    }

    public class ParameterType : ArgumentType,
                                 IEquatable<ParameterInfo>
    {
        public static implicit operator ParameterType(ParameterInfo parameterInfo) => new ParameterType(parameterInfo);
        public static bool operator ==(ParameterType? left, ParameterType? right) => left?.Type == right?.Type;
        public static bool operator !=(ParameterType? left, ParameterType? right) => !(left == right);
    
        public static bool operator ==(ParameterType? left, ParameterInfo? right) => left?.Type == right?.ParameterType;
        public static bool operator !=(ParameterType? left, ParameterInfo? right) => !(left == right);

        public int Index { get; }
        public bool IsParams { get; }
        
        public ParameterType(ParameterInfo parameterInfo)
            : base(parameterInfo.ParameterType)
        {
            if (parameterInfo is null) 
                throw new ArgumentNullException(nameof(parameterInfo));
            this.Index = parameterInfo.Position;
            this.Name = parameterInfo.Name ?? $"p{Index}";
            if (Type.IsByRef || Type.IsByRefLike)
            {
                if (parameterInfo.IsIn)
                {
                    this.Modifier = ParameterModifier.In;
                }
                else if (parameterInfo.IsOut)
                {
                    this.Modifier = ParameterModifier.Out;
                }
                else
                {
                    this.Modifier = ParameterModifier.Ref;
                }

                if (Type.IsByRef)
                {
                    this.RootType = Type.GetElementType()!;
                }
                else
                {
                    Debug.Assert(Type.IsByRefLike);
                    Debugger.Break();
                }
            }
            else if (Type.IsPointer)
            {
                this.Modifier = ParameterModifier.Pointer;
                this.RootType = Type.GetElementType()!;
            }
            else
            {
                this.Modifier = ParameterModifier.Default;
                this.RootType = Type;
            }

            this.IsParams = Attribute.GetCustomAttribute(parameterInfo, typeof(ParamArrayAttribute), true) != null;
        }
        
        /// <inheritdoc />
        public bool Equals(ParameterInfo? parameterInfo)
        {
            if (parameterInfo is null) return false;
            return parameterInfo.ParameterType == Type;
        }
        
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj switch
                   {
                       ParameterInfo parameterInfo => parameterInfo.ParameterType == Type,
                       Type type => type == Type,
                       ArgumentType adapterType => adapterType.Type == Type,
                       _ => false
                   };
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return TextBuilder.Build(this, (tb, pType) =>
            {
                if (pType.Index >= 0)
                {
                    tb.Append(pType.Index).Append(": ");
                }

                switch (pType.Modifier)
                {
                    case ParameterModifier.In:
                        tb.Append("in ");
                        break;
                    case ParameterModifier.Ref:
                        tb.Append("ref ");
                        break;
                    case ParameterModifier.Out:
                        tb.Append("out ");
                        break;
                    case ParameterModifier.Pointer:
                    case ParameterModifier.Default:
                    default:
                        break;
                }

                tb.AppendDump(pType.RootType);
                if (pType.Modifier == ParameterModifier.Pointer)
                    tb.Append('*');
            });
        }
    }
}



/*
public sealed class ArgumentType : IEquatable<ArgumentType>,
                                    IEquatable<ParameterInfo>,
                                    IEquatable<Type>
{
    public static implicit operator Type(ArgumentType argumentType) => argumentType.Type;
    public static implicit operator ArgumentType(Type type) => new ArgumentType(type);
    public static implicit operator ArgumentType(ParameterInfo parameterInfo) => new ArgumentType(parameterInfo);
    
    public static bool operator ==(ArgumentType? left, ArgumentType? right) => left?.Type == right?.Type;
    public static bool operator !=(ArgumentType? left, ArgumentType? right) => !(left == right);
    
    public static bool operator ==(ArgumentType? left, Type? right) => left?.Type == right;
    public static bool operator !=(ArgumentType? left, Type? right) => !(left == right);

    public int Index { get; }
    public ParameterModifier Modifier { get; }
    internal Type Type { get; }
    public Type RootType { get; }
    public string Name { get; }
    public bool IsValueType => RootType.IsValueType;
    public bool IsParams { get; }

    public ArgumentType(ParameterInfo parameterInfo)
    {
        if (parameterInfo is null) 
            throw new ArgumentNullException(nameof(parameterInfo));
        this.Type = parameterInfo.ParameterType;
        this.Index = parameterInfo.Position;
        this.Name = parameterInfo.Name ?? $"p{Index}";
        if (Type.IsByRef || Type.IsByRefLike)
        {
            if (parameterInfo.IsIn)
            {
                this.Modifier = ParameterModifier.In;
            }
            else if (parameterInfo.IsOut)
            {
                this.Modifier = ParameterModifier.Out;
            }
            else
            {
                this.Modifier = ParameterModifier.Ref;
            }
            this.RootType = Type.GetElementType()!;
        }
        else if (Type.IsPointer)
        {
            this.Modifier = ParameterModifier.Pointer;
            this.RootType = Type.GetElementType()!;
        }
        else
        {
            this.Modifier = ParameterModifier.Default;
            this.RootType = Type;
        }

        this.IsParams = Attribute.GetCustomAttribute(parameterInfo, typeof(ParamArrayAttribute), true) != null;
    }
    
    public ArgumentType(Type type,
                         int index = -1,
                         string? name = null)
    {
        this.Type = type ?? throw new ArgumentNullException(nameof(type));
        this.Index = index;
        this.Name = name ?? "local";
        if (type.IsByRef || type.IsByRefLike)
        {
            this.Modifier = ParameterModifier.Ref;
            this.RootType = type.GetElementType()!;
        }
        else if (type.IsPointer)
        {
            this.Modifier = ParameterModifier.Pointer;
            this.RootType = type.GetElementType()!;
        }
        else
        {
            this.Modifier = ParameterModifier.Default;
            this.RootType = type;
        }
        this.IsParams = false;
    }

    /// <inheritdoc />
    public bool Equals(ArgumentType? argumentType)
    {
        if (argumentType is null) return false;
        return argumentType.Type == Type;
    }

    /// <inheritdoc />
    public bool Equals(ParameterInfo? parameterInfo)
    {
        if (parameterInfo is null) return false;
        return parameterInfo.ParameterType == Type;
    }

    /// <inheritdoc />
    public bool Equals(Type? type)
    {
        if (type is null) return false;
        return type == Type;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj switch
               {
                   ParameterInfo parameterInfo => parameterInfo.ParameterType == Type,
                   Type type => type == Type,
                   ArgumentType adapterType => adapterType.Type == Type,
                   _ => false
               };
    }

    /// <inheritdoc />
    public override int GetHashCode() => Type.GetHashCode();

    /// <inheritdoc />
    public override string ToString()
    {
        return TextBuilder.Build(this, (tb, pType) =>
        {
            if (pType.Index >= 0)
            {
                tb.Append(pType.Index).Append(": ");
            }

            switch (pType.Modifier)
            {
                case ParameterModifier.In:
                    tb.Append("in ");
                    break;
                case ParameterModifier.Ref:
                    tb.Append("ref ");
                    break;
                case ParameterModifier.Out:
                    tb.Append("out ");
                    break;
                case ParameterModifier.Pointer:
                case ParameterModifier.Default:
                default:
                    break;
            }

            tb.AppendDump(pType.RootType);
            if (pType.Modifier == ParameterModifier.Pointer)
                tb.Append('*');
        });
    }
}
}
*/