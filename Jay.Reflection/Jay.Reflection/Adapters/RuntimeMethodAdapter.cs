using Jay.Reflection.Adapters.Args;
using Jay.Reflection.Builders;
using Jay.Reflection.Emitting;
using Jay.Reflection.Exceptions;
using Jay.Reflection.Info;

namespace Jay.Reflection.Adapters;



/// <summary>
/// Creates a <see cref="Delegate"/> to call a <see cref="MethodBase"/>
/// </summary>
public class RuntimeMethodAdapter
{
    public MethodBase Method { get; }

    public DelegateInfo MethodSig { get; }

    public DelegateInfo DelegateSig { get; }

    public RuntimeDelegateBuilder RuntimeDelegateBuilder { get; }

    public FluentGeneratorEmitter Emitter => RuntimeDelegateBuilder.Emitter;

    public RuntimeMethodAdapter(MethodBase method, DelegateInfo delegateSig)
    {
        Method = method;
        MethodSig = DelegateInfo.For(method);
        DelegateSig = delegateSig;
        RuntimeDelegateBuilder = RuntimeBuilder.CreateRuntimeDelegateBuilder(delegateSig);
    }

    private ReflectedException GetAdaptEx(InterpolatedCodeBuilder message,
        Exception? innerException = null)
    {
        var ex = new ReflectedException(ref message, innerException)
        {
            Data =
            {
                { nameof(Method), Method },
                { nameof(DelegateSig), DelegateSig },
            },
        };
        return ex;
    }
    
    
    private bool HasInstanceParam(out int offset)
    {
        // The instance will be the first parameter
        var instanceParameter = DelegateSig.Parameters.FirstOrDefault();

            // We do not have to have an instance
            if (instanceParameter is null)
            {
                offset = 0;
                return true;
            }
            
            // If they use ref NoInstance as a helper, we can ignore
            if (instanceParameter.GetNonRefType() == typeof(NoInstance))
            {
                offset = 1;
                return true;
            }
            
            // Did they mark an instance attribute?
            if (instanceParameter.GetCustomAttribute<InstanceAttribute>() is not null)
            {
                offset = 1;
                return true;
            }

            // We're unsure
            offset = 0;
            return false;
    }

    private Result TryLoadParams(int delegateParamOffset, int methodParamOffset = 0)
    {
        if (DelegateSig.ParameterCount - delegateParamOffset != 1)
            return false;// GetAdaptEx($"No params available as the only/last parameter");
        var paramsParameter = DelegateSig.Parameters.Last();
        if (!paramsParameter.IsParams())
            return false;//GetAdaptEx($"Not params");
        Emitter.EmitLoadParams(paramsParameter, MethodSig.Parameters.AsSpan(methodParamOffset));
        return true;
    }

    private Result TryLoadArgs(int delegateParamOffset, int methodParamOffset = 0)
    {
        var lastInstructionNode = Emitter.Emissions.Last;
        var delParams = DelegateSig.Parameters;
        var methParams = MethodSig.Parameters;
        if (delParams.Length - delegateParamOffset != methParams.Length - methodParamOffset)
            return GetAdaptEx($"Incorrect number of parameters available");
        Result result;
        int m = methodParamOffset;
        int d = delegateParamOffset;
        while (m < methParams.Length && d < delParams.Length)
        {
            Arg delArg = delParams[d];
            Arg methArg = methParams[m];
            result = Emitter.TryEmitCast(delArg, methArg);
            if (!result)
            {
                Emitter.Emissions.RemoveAfter(lastInstructionNode);
                return result;
            }
            m++;
            d++;
        }
        return true;
    }

    private Result TryLoadInstanceArgs()
    {
        Result result;
        
        // Static Method
        if (Method.IsStatic)
        {
            int mOffset = 0;
            // Check for throwaway
            result = HasInstanceParam(out int dOffset);
            if (result)
            {
                // If the first required arg is the instance, maybe we should load it
                if (MethodSig.ParameterCount >= 1 && 
                    CanAdaptType(MethodSig.Parameters[0], Method.OwnerType()) &&
                    DelegateSig.ParameterCount > 0 &&
                    CanAdaptType(DelegateSig.Parameters[0], MethodSig.Parameters[0]))
                {
                    // Check if we use this that we have 1:1 leftover or params
                    if (MethodSig.ParameterCount == DelegateSig.ParameterCount ||
                        (DelegateSig.ParameterCount > 1 && DelegateSig.Parameters[1].IsParams()))
                    {
                        // Okay!
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    // We know we had an instance param
                    Arg instance = DelegateSig.Parameters[0];
                    result = Emitter.TryEmitCast(instance, MethodSig.Parameters[0]);
                    if (result)
                    {
                        mOffset = 1;
                        dOffset = 1;
                    }
                }
                
                // Check for params
                result = TryLoadParams(dOffset, mOffset);
                if (result) return true;
                // Check for Args
                result = TryLoadArgs(dOffset, mOffset);
                if (result) return true;
            }
            else
            {
                // First, assume we have an instance, then assume we don't if that doesn't work
                dOffset = 1;
                while (dOffset >= 0)
                {
                    // Check for params
                    result = TryLoadParams(dOffset);
                    if (result) return true;
                    
                    // Check for Args
                    result = TryLoadArgs(dOffset);
                    if (result) return true;
                    
                    dOffset--;
                }
            }
           
            // Nothing worked
            return GetAdaptEx($"Could not understand static method adapt");
        }

        // Instance Method
        if (DelegateSig.ParameterCount == 0)
            return GetAdaptEx($"There must be an instance argument for instance Methods");
        if (!Method.TryGetInstanceType(out var instanceType))
            return GetAdaptEx($"Could not find Instance Type for Method");
        Arg instanceArg = DelegateSig.Parameters[0];
        result = Emitter.TryEmitCast(instanceArg, instanceType);
        if (!result) return result;
        
        // Check for Params
        result = TryLoadParams(1);
        if (result) return true;
        // Otherwise, load args 1 by 1
        result = TryLoadArgs(1);
        
        // Cleanup result
        if (result) return true;
        return result;
    }

    public static Result TryAdapt(MethodBase method, DelegateInfo delegateSig, [NotNullWhen(true)] out Delegate? adapterDelegate)
    {
        // faster return
        adapterDelegate = default;

        var adapter = new RuntimeMethodAdapter(method, delegateSig);
        // Load (possible) instance + all args
        var result = adapter.TryLoadInstanceArgs();
        if (!result) 
            return result;
        // Call the original method
        adapter.Emitter.Call(method);
        Arg methodReturn = method.ReturnType();
        Arg delReturn = delegateSig.ReturnType;
        result = adapter.Emitter.TryEmitCast(methodReturn, delReturn);
        if (!result) return result;
        adapter.Emitter.Ret();
        adapterDelegate = adapter.RuntimeDelegateBuilder.CreateDelegate();
        return true;
    }

    public static Result TryAdapt<TDelegate>(MethodBase method, [NotNullWhen(true)] out TDelegate? @delegate)
        where TDelegate : Delegate
    {
        var result = TryAdapt(method, DelegateInfo.For<TDelegate>(), out var del);
        if (result)
        {
            @delegate = del as TDelegate;
            if (@delegate is not null) return true;
            return new InvalidOperationException("Del is not del");
        }
        @delegate = default;
        return result;
    }

    public static TDelegate Adapt<TDelegate>(MethodBase method)
        where TDelegate : Delegate
    {
        var result = TryAdapt<TDelegate>(method, out var del);
        if (!result)
            result.ThrowIfError();
        return del!;
    }

    public static bool CanAdaptType(Arg input, Arg output)
    {
        return ArgExtensions.CanCast(input, output);
    }
    
    public static bool CanAdaptTypes(Type[] inputTypes, Type[] outputTypes)
    {
        int len = inputTypes.Length;
        // Mismatched count?
        if (outputTypes.Length != len)
        {
            // Only works if inputParameters is just a params
            //return len == 1 && inputTypes[0] == typeof(object[]);
            return false;
        }
        
        for (var i = 0; i < len; i++)
        {
            Arg input = inputTypes[i];
            Arg output = outputTypes[i];
            if (!ArgExtensions.CanCast(input, output))
                return false;
        }
        
        return true;
    }
}