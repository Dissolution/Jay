using Dunet;
using Jay.Reflection.Adapters.Args;
using Jay.Reflection.Caching;

namespace Jay.Reflection.Emitting;

public static class SmartEmitterExtensions
{
    public static TEmitter LoadInstanceFor<TEmitter>(
        this TEmitter emitter,
        Arg arg, MemberInfo member)
        where TEmitter : FluentEmitter<TEmitter>
    {
        if (member.TryGetInstanceType(out var instanceType))
        {
            // Do we need a value type's instance?
            if (instanceType.IsValueType)
            {
                // We need a reference to the value

                // We have a T?
                if (arg.RootType == instanceType)
                {
                    if (arg.IsByRef)
                    {
                        arg.EmitLoad(emitter);
                    }
                    else
                    {
                        arg.EmitLoadAddress(emitter);
                    }
                    return emitter;
                }

                // We have an object?
                if (arg.RootType == typeof(object))
                {
                    arg.EmitLoad(emitter);
                    // ref object?
                    if (arg.IsByRef)
                    {
                        emitter.Ldind_Ref();
                    }
                    return emitter.Unbox(instanceType);
                }

                // cannot
                throw new InvalidOperationException();
            }
            // we just need a non-value
            else
            {
                // We have a T?
                if (arg.RootType == instanceType)
                {
                    arg.EmitLoad(emitter);
                    if (arg.IsByRef)
                    {
                        emitter.Ldind(arg.RootType);
                    }
                    return emitter;
                }

                // source is object or implements instance type
                if (arg.RootType == typeof(object) ||
                    arg.RootType.Implements(instanceType))
                {
                    arg.EmitLoad(emitter);
                    // ref object?
                    if (arg.IsByRef)
                    {
                        emitter.Ldobj(arg.RootType);
                    }
                    return emitter.Castclass(instanceType);
                }

                // cannot
                throw new InvalidOperationException();
            }
        }
        // no instance to load, ok!
        return emitter;
    }
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

    public SmartEmitter Load(Arg arg)
    {
        arg.EmitLoad(_genEmitter);
        return this;
    }

    public SmartEmitter LoadAddress(Arg arg)
    {
        arg.EmitLoadAddress(_genEmitter);
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
            // case FieldInfo field:
            //     return Load(field);
            case ParameterInfo parameter:
                return Load(parameter);
            case EmitLocal local:
                return Load(local);
            default:
                throw new NotImplementedException();
        }
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
        ArgExtensions.EmitCast(
            _genEmitter,
            sourceType,
            destType);
        return this;
    }

    public SmartEmitter Cast(Arg source, Type destType)
    {
        ArgExtensions.EmitCast(
            _genEmitter,
            source.Type,
            destType);
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
        string localName = "") => DeclareLocal(
        typeof(T),
        false,
        out emitLocal,
        localName);

    public SmartEmitter DeclareLocal<T>(
        bool isPinned,
        out EmitLocal emitLocal,
        [CallerArgumentExpression(nameof(emitLocal))]
        string localName = "") => DeclareLocal(
        typeof(T),
        isPinned,
        out emitLocal,
        localName);

    public SmartEmitter DeclareLocal(
        Type type,
        out EmitLocal emitLocal,
        [CallerArgumentExpression(nameof(emitLocal))]
        string localName = "") => DeclareLocal(
        type,
        false,
        out emitLocal,
        localName);

    public SmartEmitter DeclareLocal(
        Type type,
        bool isPinned,
        out EmitLocal emitLocal,
        [CallerArgumentExpression(nameof(emitLocal))]
        string localName = "")
    {
        _genEmitter.DeclareLocal(
            type,
            isPinned,
            out emitLocal,
            localName);
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