using Jay.Reflection.Caching;
using Jay.Reflection.Emitting.Args;
using Argument = Jay.Reflection.Emitting.Args.Argument;

namespace Jay.Reflection.Emitting;

public sealed class SmartEmitter
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

    public SmartEmitter Load(Argument argument)
    {
        argument.EmitLoad(_genEmitter);
        return this;
    }

    public SmartEmitter LoadAddress(Argument argument)
    {
        argument.EmitLoadAddress(_genEmitter);
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
            case EmitterLocal local:
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

    public SmartEmitter Store(EmitterLocal local)
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
        ArgumentExtensions.EmitCast(
            _genEmitter,
            sourceType,
            destType);
        return this;
    }

    public SmartEmitter Cast(Argument source, Type destType)
    {
        ArgumentExtensions.EmitCast(
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
        out EmitterLocal emitterLocal,
        [CallerArgumentExpression(nameof(emitterLocal))]
        string localName = "") => DeclareLocal(
        typeof(T),
        false,
        out emitterLocal,
        localName);

    public SmartEmitter DeclareLocal<T>(
        bool isPinned,
        out EmitterLocal emitterLocal,
        [CallerArgumentExpression(nameof(emitterLocal))]
        string localName = "") => DeclareLocal(
        typeof(T),
        isPinned,
        out emitterLocal,
        localName);

    public SmartEmitter DeclareLocal(
        Type type,
        out EmitterLocal emitterLocal,
        [CallerArgumentExpression(nameof(emitterLocal))]
        string localName = "") => DeclareLocal(
        type,
        false,
        out emitterLocal,
        localName);

    public SmartEmitter DeclareLocal(
        Type type,
        bool isPinned,
        out EmitterLocal emitterLocal,
        [CallerArgumentExpression(nameof(emitterLocal))]
        string localName = "")
    {
        _genEmitter.DeclareLocal(
            type,
            isPinned,
            out emitterLocal,
            localName);
        return this;
    }

    public SmartEmitter Define(
        out EmitterLabel emitterLabel,
        [CallerArgumentExpression(nameof(emitterLabel))]
        string lblName = "")
    {
        _genEmitter.DefineLabel(out emitterLabel, lblName);
        return this;
    }

    public SmartEmitter Mark(EmitterLabel emitterLabel)
    {
        _genEmitter.MarkLabel(emitterLabel);
        return this;
    }


    public override string ToString()
    {
        return this.Emissions.ToString();
    }
}