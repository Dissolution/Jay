using System.Reflection;
using System.Reflection.Emit;
using Jay.Reflection.Building.Emission;
using Jay.Reflection.Caching;

namespace Jay.Reflection.Building;

public class RuntimeMethod
{
    public static implicit operator DynamicMethod(RuntimeMethod runtimeMethod) => runtimeMethod.DynamicMethod;

    protected readonly DynamicMethod _dynamicMethod;
    protected readonly MethodSig _methodSig;
    private ILGenerator? _ilGenerator = null;
    private ILGeneratorEmitter? _emitter = null;

    public DynamicMethod DynamicMethod => _dynamicMethod;
    public MethodSig MethodSig => _methodSig;

    public ILGenerator ILGenerator => _ilGenerator ??= DynamicMethod.GetILGenerator();
    public IILGeneratorEmitter Emitter => _emitter ??= new ILGeneratorEmitter(this.ILGenerator);
    public IReadOnlyList<ParameterInfo> Parameters => _methodSig.Parameters;
    public Type[] ParameterTypes => _methodSig.ParameterTypes;
    public int ParameterCount => _methodSig.ParameterCount;
    public Type ReturnType => _methodSig.ReturnType;

    public RuntimeMethod(DynamicMethod dynamicMethod, MethodSig methodSig)
    {
        _dynamicMethod = dynamicMethod;
        _methodSig = methodSig;
    }

    public Delegate CreateDelegate()
    {
        return _dynamicMethod.CreateDelegate(_methodSig.DelegateType);
    }
    
    public Delegate CreateDelegate(object? target)
    {
        return _dynamicMethod.CreateDelegate(_methodSig.DelegateType, target);
    }
    
    public Result TryCreateDelegate([NotNullWhen(true)] out Delegate? @delegate)
    {
        try
        {
            @delegate = _dynamicMethod.CreateDelegate(_methodSig.DelegateType);
            return true;
        }
        catch (Exception ex)
        {
            @delegate = null;
            return ex;
        }
    }
    
    public Result TryCreateDelegate(object? target, [NotNullWhen(true)] out Delegate? @delegate)
    {
        try
        {
            @delegate = _dynamicMethod.CreateDelegate(_methodSig.DelegateType, target);
            return true;
        }
        catch (Exception ex)
        {
            @delegate = null;
            return ex;
        }
    }
}

