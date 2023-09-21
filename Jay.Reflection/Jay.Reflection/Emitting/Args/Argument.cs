using Dunet;

namespace Jay.Reflection.Emitting.Args;

[Union]
public partial record Argument : ICodePart
{
    public static implicit operator Argument(Type? type) => new Argument.Stack { Type = type ?? typeof(void) };
    public static implicit operator Argument(EmitterLocal local) => new Argument.Local(local) { Type = local.Type };
    public static implicit operator Argument(ParameterInfo parameter) => new Argument.Parameter(parameter) { Type = parameter.ParameterType };
    
    public partial record Field(Argument? Instance, FieldInfo FieldInfo);
    public partial record Local(EmitterLocal EmitterLocal);
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