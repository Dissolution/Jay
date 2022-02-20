using System.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Adapting;

namespace Jay.Reflection;

public static class MethodInfoExtensions
{
    public static Result TryAdapt<TDelegate>(this MethodInfo method, [NotNullWhen(true)] out TDelegate? @delegate)
    where TDelegate : Delegate
    {
        var dynamicMethod = RuntimeBuilder.CreateDynamicMethod<TDelegate>($"{typeof(TDelegate)}_{method.GetType()}_adapter");
        var adapter = new DelegateMethodAdapter<TDelegate>(method);
        var result = adapter.TryAdapt(dynamicMethod.Emitter);
        if (!result)
        {
            @delegate = null;
            return result;
        }
        result = dynamicMethod.TryCreateDelegate(out @delegate);
        return result;
    }

    public static TDelegate Adapt<TDelegate>(this MethodInfo method)
        where TDelegate : Delegate
    {
        return DelegateMemberCache.Instance
                                  .GetOrAdd(method, dm =>
                                  {
                                      TryAdapt<TDelegate>((dm.Member as MethodInfo)!, out var del).ThrowIfFailed();
                                      return del!;
                                  });
    }

    public static TReturn Invoke<TInstance, TReturn>(this MethodInfo method,
                                                     ref TInstance instance,
                                                     params object?[] args)
    {
        var del = DelegateMemberCache.Instance
                                     .GetOrAdd<Invoker<TInstance, TReturn>>(method, _ =>
                                     {
                                         TryAdapt<Invoker<TInstance, TReturn>>(method, out var del).ThrowIfFailed();
                                         return del!;
                                     });
        return del(ref instance, args);
    }
}