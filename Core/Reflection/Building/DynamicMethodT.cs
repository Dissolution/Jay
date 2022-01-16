using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using Jay.Reflection.Building.Emission;
using Jay.Utilities;

namespace Jay.Reflection.Building;

public class DynamicMethod<TDelegate>
    where TDelegate : Delegate
{
    public static implicit operator DynamicMethod(DynamicMethod<TDelegate> dynamicMethod) =>
        dynamicMethod._dynamicMethod;

    protected readonly DynamicMethod _dynamicMethod;
    protected ILGenerator? _ilGenerator;

    public ILGenerator ILGenerator => _ilGenerator ??= _dynamicMethod.GetILGenerator();
    public IILGeneratorEmitter Emitter => new ILGeneratorEmitter(this.ILGenerator);
    //public IILGeneratorFluentEmitter FluentEmitter => new ILGeneratorEmitter(this.ILGenerator);
    public DelegateSig DelegateSignature { get; }
    public ParameterInfo[] Parameters => DelegateSignature.Parameters;
    public Type ReturnType => DelegateSignature.ReturnType;

    public DynamicMethod(DynamicMethod dynamicMethod)
    {
        _dynamicMethod = dynamicMethod;
        this.DelegateSignature = DelegateSig.Of<TDelegate>();
    }

    public TDelegate CreateDelegate() => _dynamicMethod.CreateDelegate<TDelegate>();

    public Result TryCreateDelegate([NotNullWhen(true)] out TDelegate? @delegate)
    {
        try
        {
            @delegate = _dynamicMethod.CreateDelegate<TDelegate>();
            return true;
        }
        catch (Exception ex)
        {
            @delegate = null;
            return ex;
        }
    }
}