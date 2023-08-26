using Dunet;
using Jay.Reflection.Adapters.Args;
using Jay.Reflection.Caching;

namespace Jay.Reflection.Emitting;

[Union]
public partial record ValueRef
{
    public static implicit operator Type(ValueRef valueRef) => valueRef.Type;
    
    public partial record Field(FieldInfo FieldInfo)
    {
        public override Type Type => this.FieldInfo.FieldType;
    }

    public partial record Local(EmitLocal EmitLocal)
    {
        public override Type Type => this.EmitLocal.Type;
    }

    public partial record Parameter(ParameterInfo ParameterInfo)
    {
        public override Type Type => this.ParameterInfo.ParameterType;
    }
    
    public abstract Type Type { get; }

    public bool IsByRef => Type.IsByRef;

    public Type RootType => IsByRef ? Type.GetElementType().ThrowIfNull() : Type;
}

public sealed class SmartEmitter : IToCode
{
    private readonly FluentGeneratorEmitter _genEmitter;

    public EmissionStream Emissions => _genEmitter.Emissions;

    public SmartEmitter(FluentGeneratorEmitter genEmitter)
    {
        _genEmitter = genEmitter;
    }

    public SmartEmitter Emit(Emission emission)
    {
        throw new NotImplementedException();
    }

    public SmartEmitter Load(ValueRef valueRef)
    {
        valueRef.Match(
            field => _genEmitter.Ldfld(field.FieldInfo),
            local => _genEmitter.Ldloc(local.EmitLocal),
            parameter => _genEmitter.Ldarg((ParameterInfo)parameter.ParameterInfo));
        return this;
    }
    
    public SmartEmitter LoadAddress(ValueRef valueRef)
    {
        valueRef.Match(
            field => _genEmitter.Ldflda(field.FieldInfo),
            local => _genEmitter.Ldloca(local.EmitLocal),
            parameter => _genEmitter.Ldarga((ParameterInfo)parameter.ParameterInfo));
        return this;
    }

    public SmartEmitter LoadConst<T>(T? constant)
    {
        switch (constant)
        {
            case null:
                _genEmitter.Ldnull();
                return this;
            case bool boolean:
                _genEmitter.Ldc_I4(boolean ? 1 : 0);
                return this;
            case byte b:
                _genEmitter.Ldc_I4(b);
                return this;
            case sbyte sb:
                _genEmitter.Ldc_I4(sb);
                return this;
            case short s:
                _genEmitter.Ldc_I4(s);
                return this;
            case ushort us:
                _genEmitter.Ldc_I4(us);
                return this;
            case int i:
                _genEmitter.Ldc_I4(i);
                return this;
            case uint ui:
                _genEmitter.Ldc_I8(ui);
                return this;
            case long l:
                _genEmitter.Ldc_I8(l);
                return this;
            case ulong ul:
                _genEmitter.Ldc_I8((long)ul).Conv_U();
                return this;
            case float f:
                _genEmitter.Ldc_R4(f);
                return this;
            case double d:
                _genEmitter.Ldc_R8(d);
                return this;
            case string str:
                _genEmitter.Ldstr(str);
                return this;
            case Type type:
                _genEmitter
                    .Ldtoken(type)
                    .Call(MemberCache.Methods.Type_GetTypeFromHandle);
                return this;
            case FieldInfo field:
                return Load(field);
            case ParameterInfo parameter:
                return Load(parameter);
            case EmitLocal local:
                return Load(local);
            default:
                throw new NotImplementedException();
        }
    }

    public SmartEmitter LoadInstanceFor(ValueRef valueRef, MemberInfo member)
    {
        if (member.TryGetInstanceType(out var instanceType))
        {
            // Do we need a value type's instance?
            if (instanceType.IsValueType)
            {
                // We need a reference to the value
                
                // We have a T?
                if (valueRef.RootType == instanceType)
                {
                    if (valueRef.IsByRef)
                        return Load(valueRef);
                    return LoadAddress(valueRef);
                }
                
                // We have an object?
                if (valueRef.RootType == typeof(object))
                {
                    Load(valueRef);
                    // ref object?
                    if (valueRef.IsByRef)
                    {
                        _genEmitter.Ldind_Ref();
                    }
                    _genEmitter.Unbox(instanceType);
                    return this;
                }

                // cannot
                throw new InvalidOperationException();
            }
            // we just need a non-value
            else
            {
                // We have a T?
                if (valueRef.RootType == instanceType)
                {
                    Load(valueRef);
                    if (valueRef.IsByRef)
                    {
                        _genEmitter.Ldind(valueRef.RootType);
                    }
                    return this;
                }
                
                // source is object or implements instance type
                if (valueRef.RootType == typeof(object) ||
                    valueRef.RootType.Implements(instanceType))
                {
                    Load(valueRef);
                    // ref object?
                    if (valueRef.IsByRef)
                    {
                        _genEmitter.Ldobj(valueRef.RootType);
                    }
                    _genEmitter.Castclass(instanceType);
                    return this;
                }
                
                // cannot
                throw new InvalidOperationException();
            }
        }
        // no instance to load, ok!
        return this;
    }
    
    
    public SmartEmitter Store(FieldInfo field)
    {
        _genEmitter.Stfld(field);
        return this;
    }
    public SmartEmitter Store(EmitLocal local)
    {
        _genEmitter.Stloc(local);
        return this;
    }
    public SmartEmitter Store(ParameterInfo parameter)
    {
        _genEmitter.Starg(parameter.Position);
        return this;
    }

    public SmartEmitter Cast(Type sourceType, Type destType)
    {
        ArgExtensions.EmitCast(_genEmitter, sourceType, destType);
        return this;
    }
    public SmartEmitter Cast(ValueRef source, Type destType)
    {
        ArgExtensions.EmitCast(_genEmitter, source.Type, destType);
        return this;
    }

    public SmartEmitter Call(MethodBase method)
    {
        _genEmitter.Call(method);
        return this;
    }

    public SmartEmitter Return()
    {
        _genEmitter.Ret();
        return this;
    }

    public SmartEmitter DeclareLocal<T>(
        out EmitLocal emitLocal,
        [CallerArgumentExpression(nameof(emitLocal))]
        string localName = "") => DeclareLocal(typeof(T), false, out emitLocal, localName);
    public SmartEmitter DeclareLocal<T>(
        bool isPinned,
        out EmitLocal emitLocal,
        [CallerArgumentExpression(nameof(emitLocal))]
        string localName = "") => DeclareLocal(typeof(T), isPinned, out emitLocal, localName);
    public SmartEmitter DeclareLocal(
        Type type,
        out EmitLocal emitLocal,
        [CallerArgumentExpression(nameof(emitLocal))]
        string localName = "") => DeclareLocal(type, false, out emitLocal, localName);
    
    public SmartEmitter DeclareLocal(
        Type type, 
        bool isPinned, 
        out EmitLocal emitLocal, 
        [CallerArgumentExpression(nameof(emitLocal))]
        string localName = "")
    {
        _genEmitter.DeclareLocal(type, isPinned, out emitLocal, localName);
        return this;
    }
    public SmartEmitter Define(
        out EmitLabel emitLabel, 
        [CallerArgumentExpression(nameof(emitLabel))]
        string lblName = "")
    {
        _genEmitter.DefineLabel(out emitLabel, lblName);
        return this;
    }
    public SmartEmitter Mark(EmitLabel emitLabel)
    {
        _genEmitter.MarkLabel(emitLabel);
        return this;
    }

    
    
    public void WriteCodeTo(CodeBuilder codeBuilder)
    {
        this.Emissions.WriteCodeTo(codeBuilder);
    }
}