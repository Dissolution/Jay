using System.Reflection;
using Jay.Reflection.Building.Emission;
using Jay.Reflection.Exceptions;

namespace Jay.Reflection.Building.Adapting;

public class DelegateMethodAdapter
{
    public DelegateSig DelegateSignature { get; }
    public MethodBase Method { get; }
    public DelegateSig MethodSignature { get; }
    public Safety Safety { get; }

    public DelegateMethodAdapter(DelegateSig delegateSig,
                                 MethodBase method,
                                 Safety safety = Safety.Safe)
    {
        this.DelegateSignature = delegateSig;
        this.Method = method ?? throw new ArgumentNullException(nameof(method));
        this.MethodSignature = DelegateSig.Of(Method);
        this.Safety = safety;
    }

    protected Result TryLoadArgs<TEmitter>(TEmitter emitter, int offset)
        where TEmitter : class, IFluentEmitter<TEmitter>
    {
        // None?
        if (MethodSignature.ParameterCount == 0)
        {
            return true;
        }

        // Check 1:1
        if (MethodSignature.ParameterCount == (DelegateSignature.ParameterCount - offset))
        {
            return new NotImplementedException();
        }
        // Check for only params
        else if (DelegateSignature.ParameterCount == offset + 1 &&
                 DelegateSignature.Parameters[offset].IsObjectArray() &&
                 !MethodSignature.Parameters[0].IsObjectArray())
        {
            return Result.Try(() => emitter.LoadParams(DelegateSignature.Parameters[offset], MethodSignature.Parameters));
        }
        // Check for optional method params
        else if (MethodSignature.Parameters.Reverse().Any(p => p.HasDefaultValue))
        {
            return new NotImplementedException();
        }
        // TODO: Other checks?
        else
        {
            return new NotImplementedException();
        }
    }

    protected Result TryCastReturn<TEmitter>(TEmitter emitter)
        where TEmitter : class, IFluentEmitter<TEmitter>
    {
        // Does delegate have one?
        if (DelegateSignature.ReturnType != typeof(void))
        {
            // Does method?
            if (MethodSignature.ReturnType != typeof(void))
            {
                return Result.Try(() => emitter.Cast(MethodSignature.ReturnType, DelegateSignature.ReturnType));
            }
            else
            {
                if (Safety.HasFlag<Safety>(Safety.AllowReturnDefault))
                {
                    emitter.LoadDefault(DelegateSignature.ReturnType);
                    return true;
                }
                return new AdapterException($"Delegate requires a returned {DelegateSignature.ReturnType} value, Method does not return one");
            }
        }
        else
        {
            // Does method?
            if (MethodSignature.ReturnType != typeof(void))
            {
                if (Safety.HasFlag<Safety>(Safety.AllowReturnDiscard))
                {
                    emitter.Pop();
                    return true;
                }
                return new AdapterException($"Delegate is an action, Method returns a {MethodSignature.ReturnType}");
            }
            else
            {
                // void -> void
                return true;
            }
        }
    }

    public Result TryAdapt<TEmitter>(TEmitter emitter)
        where TEmitter : class, IFluentEmitter<TEmitter>
    {
        Result result;
        ParameterInfo? possibleInstanceParam;
        if (DelegateSignature.ParameterCount > 0)
        {
            possibleInstanceParam = DelegateSignature.Parameters[0];
        }
        else
        {
            possibleInstanceParam = null;
        }
        int offset = default;
        result = Result.Try(() => emitter.LoadInstanceFor(Method, possibleInstanceParam, out offset));
        if (!result) return result;
        result = TryLoadArgs(emitter, offset);
        if (!result) return result;
        result = TryCastReturn(emitter);
        if (!result) return result; 
        emitter.Ret();
        return true;
    }
}

public class DelegateMethodAdapter<TDelegate> : DelegateMethodAdapter
    where TDelegate : Delegate
{
    public DelegateMethodAdapter(MethodBase method,
                                 Safety safety = Safety.Safe)
        : base(DelegateSig.Of<TDelegate>(), method, safety)
    {

    }
}