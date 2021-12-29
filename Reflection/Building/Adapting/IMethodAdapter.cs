using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Jay.Reflection.Building.Emission;

namespace Jay.Reflection;

public enum MethodAdapterSafety
{
    // Safe
    Safe = 0,

    // Weird, but OK
    AllowRefAsIn,
    AllowRefAsOut,

    // Side Effects
    AllowNonRefStructInstance,

    // Dangerous
    AllowInAsRef,
    AllowInAsOut,
    AllowOutAsRef,

    // WTF?
    AllowOutAsIn,
    
}

public interface IMethodAdapter
{
    MethodAdapterSafety Safety { get; }

    Result TryAdapt(Type delegateType, MethodBase method, [NotNullWhen(true)] out Delegate? @delegate);

    Result TryAdapt<TDelegate>(MethodBase method, [NotNullWhen(true)] out TDelegate? @delegate)
        where TDelegate : Delegate;
}

internal class MethodAdapter : IMethodAdapter
{
    public MethodAdapterSafety Safety { get; }

    public MethodAdapter(MethodAdapterSafety safety = MethodAdapterSafety.Safe)
    {
        this.Safety = safety;
    }

    protected Result TryLoadInstance(InstructionStream ilStream, MethodBase method, ref int pOffset)
    {

    }

    public Result TryAdapt(Type delegateType, MethodBase method, [NotNullWhen(true)] out Delegate? @delegate)
    {
        throw new NotImplementedException();
    }

    public Result TryAdapt<TDelegate>(MethodBase method, [NotNullWhen(true)] out TDelegate? @delegate) 
        where TDelegate : Delegate
    {
        var result = TryAdapt(typeof(TDelegate), method, out var del);
        if (!result)
        {
            @delegate = default;
            return result;
        }

        if (!del.Is<TDelegate>(out @delegate))
        {
            return new InvalidOperationException($"Could not cast {del?.GetType()} to {typeof(TDelegate)}");
        }

        return result;
    }
}