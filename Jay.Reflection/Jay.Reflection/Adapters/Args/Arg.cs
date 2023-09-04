using Dunet;
using Jay.Reflection.Emitting;

namespace Jay.Reflection.Adapters.Args;

[Union]
public partial record Arg : ICodePart
{
    public static implicit operator Arg(Type? type) => new Arg.Stack { Type = type ?? typeof(void) };
    public static implicit operator Arg(EmitLocal local) => new Arg.Local(local) { Type = local.Type };
    public static implicit operator Arg(ParameterInfo parameter) => new Arg.Parameter(parameter) { Type = parameter.ParameterType };
    
    public partial record Field(Arg? Instance, FieldInfo FieldInfo);
    public partial record Local(EmitLocal EmitLocal);
    public partial record Parameter(ParameterInfo ParameterInfo);
    public partial record Stack();
    
    public required Type Type { get; init; }

    public bool IsByRef => Type.IsByRef;
    public Type RootType
    {
        get
        {
            if (Type.IsByRef)
                return Type.GetElementType().ThrowIfNull();
            return Type;
        }
    }

    public abstract void EmitLoad<TEmitter>(TEmitter emitter)
        where TEmitter : FluentEmitter<TEmitter>;

    public abstract void EmitLoadAddress<TEmitter>(TEmitter emitter)
        where TEmitter : FluentEmitter<TEmitter>;

    public abstract void EmitStore<TEmitter>(TEmitter emitter)
        where TEmitter : FluentEmitter<TEmitter>;

    public abstract void DeclareTo(CodeBuilder codeBuilder);
}