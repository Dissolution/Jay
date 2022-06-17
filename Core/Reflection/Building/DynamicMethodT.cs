using System.Reflection;
using System.Reflection.Emit;
using Jay.Reflection.Building.Emission;
using Jay.Reflection.Caching;

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
    public MethodSig MethodSig { get; }
    public IReadOnlyList<ParameterInfo> Parameters { get; }
    public Type ReturnType { get; }
    
    
    public DynamicMethod(DynamicMethod dynamicMethod)
    {
        _dynamicMethod = dynamicMethod;
        var invokeMethod = typeof(TDelegate).GetMethod("Invoke", Reflect.PublicFlags)!;
        this.MethodSig = MethodSig.Of(invokeMethod);
        this.Parameters = invokeMethod.GetParameters();
        this.ReturnType = invokeMethod.ReturnType;
    }

    public TDelegate CreateDelegate() => _dynamicMethod.CreateDelegate<TDelegate>();

    public Result.Result TryCreateDelegate([NotNullWhen(true)] out TDelegate? @delegate)
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