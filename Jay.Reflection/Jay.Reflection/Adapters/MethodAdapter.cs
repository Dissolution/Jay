using Jay.Reflection.Caching;

namespace Jay.Reflection.Adapters;

public static class MethodAdapter
{
    internal static Invoke<TInstance, TReturn> CreateInvokeDelegate<TInstance, TReturn>(MethodBase method)
    {
        return RuntimeMethodAdapter.Adapt<Invoke<TInstance, TReturn>>(method);
    }

    public static Invoke<TInstance, TReturn> GetInvokeDelegate<TInstance, TReturn>(MethodBase method)
    {
        return MemberDelegateCache.GetOrAdd(method, CreateInvokeDelegate<TInstance, TReturn>);
    }
    
    public static TReturn Invoke<TInstance, TReturn>(this MethodBase method,
        ref TInstance instance,
        params object?[] args)
    {
        return GetInvokeDelegate<TInstance, TReturn>(method)(ref instance, args);
    }
  
}