using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Reflection.Building;
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

public interface IDelegateMemberBuilder
{
    DelegateSig DelegateSig { get; }
    MemberInfo Member { get; }
    MethodAdapterSafety Safety { get; }

    Result TryBuild([NotNullWhen(true)] out Delegate? @delegate);
}

internal abstract class DelegateMemberBuilder : IDelegateMemberBuilder
{
    public DelegateSig DelegateSig { get; }
    public MemberInfo Member { get; }
    public MethodAdapterSafety Safety { get; }

    protected DelegateMemberBuilder(DelegateSig sig, MemberInfo member, MethodAdapterSafety safety)
    {
        this.DelegateSig = sig;
        this.Member = member;
        this.Safety = safety;
    }

    protected DynamicMethod CreateDynamicMethod()
    {
        return RuntimeBuilder.CreateDynamicMethod(null, this.DelegateSig);
    }
}

public interface IDelegateMemberBuilder<TDelegate> : IDelegateMemberBuilder
    where TDelegate : Delegate
{
    Result TryAdapt([NotNullWhen(true)] out TDelegate? @delegate);
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

    protected Result ThinkWeCanLoadArgs(MethodBase method, DelegateSig sig, int pOffset)
    {

    }

    protected Result TryLoadInstance(InstructionStream ilStream, Type instanceType, DelegateSig sig, out int pOffset)
    {
        Debug.Assert(instanceType != null);
        Debug.Assert(instanceType != typeof(void));
        Debug.Assert(!instanceType.IsStatic());
        if (sig.ParameterCount == 0)
        {
            pOffset = default;
            return new ArgumentException("No instance parameter provided");
        }
        Debug.Assert(sig.ParameterCount >= 1);
        var sigInstance = sig.Parameters[0];
        var sigInstAccess = sigInstance.GetAccess(out var sigInstType);
        // If we have exactly what we need
        if (instanceType.IsValueType)
        {
            // We really want to get a ref instance for a value type, it produces no side effects
            if (sigInstAccess == ParameterInfoExtensions.Access.In ||
                sigInstAccess == ParameterInfoExtensions.Access.Ref)
            {

            }
        }
    }

    protected Result TryLoadInstance(InstructionStream ilStream, MethodBase method, DelegateSig sig, out int pOffset)
    {
        // Static method?
        if (method.IsStatic)
        {
            // We do not have to load an instance
            if (sig.ParameterCount == 0)
            {
                // All good
                pOffset = 0;
                return true;
            }

            Debug.Assert(sig.ParameterCount >= 1);

            // We want to check if a throwaway one was provided
            var possibleInstanceType = sig.Parameters[0].ParameterType;
            if (possibleInstanceType == typeof(void) || possibleInstanceType == typeof(Static))
            {
                // Use this throwaway
                pOffset = 1;
                return true;
            }
            // We can accept Type in only specific circumstances
            if (possibleInstanceType == typeof(Type))
            {
                var result = ThinkWeCanLoadArgs(method, sig, 1);
                if (result)
                {
                    pOffset = 1;
                    return result;
                }
            }
            
            // Assume none was provided
            pOffset = 0;
            return true;
        }



        var owner = method.ReflectedType;
        if (owner is not null)
        {

        }
    }

    public Result TryAdapt(MethodBase method, Type delegateType, [NotNullWhen(true)] out Delegate? @delegate)
    {
        throw new NotImplementedException();
    }

    public Result TryAdapt<TDelegate>(MethodBase method, [NotNullWhen(true)] out TDelegate? @delegate) 
        where TDelegate : Delegate
    {
        var result = TryAdapt(method, typeof(TDelegate), out var del);
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