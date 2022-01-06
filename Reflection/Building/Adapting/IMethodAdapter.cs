using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Reflection.Emission;

namespace Jay.Reflection.Adapting;

public enum Safety
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

public class AdapterException : ReflectionException
{
    public AdapterException(string? message = null, Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}

public class DelegateMethodAdapter
{
    public DelegateSig DelegateSignature { get; }
    public MethodBase Method { get; }
    public Safety Safety { get; }

    public DelegateMethodAdapter(DelegateSig delegateSig,
                                 MethodBase method,
                                 Safety safety = Safety.Safe)
    {
        this.DelegateSignature = delegateSig;
        this.Method = method ?? throw new ArgumentNullException(nameof(method));
        this.Safety = safety;
    }

    protected Result TryLoadArgs<TEmitter>(TEmitter emitter)
        where TEmitter : IOpCodeEmitter<TEmitter>, IGenEmitter<TEmitter>
    {
        var methodSig = DelegateSig.Of(Method);

        // Static method
        if (Method.IsStatic)
        {
            // Does not need an instance passed in
            if (methodSig.ParameterCount > DelegateSignature.ParameterCount)
            {
                // More args needed than provided
                return new AdapterException($"{methodSig.ParameterCount} parameters were needed but only {DelegateSignature.ParameterCount} were provided");
            }

            if (methodSig.ParameterCount == 0)
            {
                Debug.Assert(DelegateSignature.ParameterCount == 0);
                // Nothing to load
                return true;
            }


        }
    }

    public Result TryAdapt<TEmitter>(TEmitter emitter)
        where TEmitter : IOpCodeEmitter<TEmitter>, IGenEmitter<TEmitter>
    {
        var methodSig = DelegateSig.Of(Method);

        // Static method
        if (Method.IsStatic)
        {
            // Does not need an instance passed in
            if (DelegateSignature.ParameterCount == 0)
            {
                if (methodSig.ParameterCount == 0)
                {
                    // Nothing to load
                }
            }
        }
    }
}