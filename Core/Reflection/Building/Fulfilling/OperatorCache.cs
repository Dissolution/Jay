using System.Diagnostics;
using System.Reflection;
using Jay.Collections;
using Jay.Validation;

namespace Jay.Reflection.Building.Fulfilling;

public static class OperatorCache
{
    private static readonly ConcurrentTypeDictionary<EqualityMethods> _equalsMethods;

    static OperatorCache()
    {
        _equalsMethods = new();
        _delegateCombineMethod = new Lazy<MethodInfo>(() =>
        {
            return typeof(Delegate).GetMethod(nameof(Delegate.Combine),
                                       BindingFlags.Public | BindingFlags.Static,
                                       new Type[2] { typeof(Delegate), typeof(Delegate) })
                                   .ThrowIfNull();
        });
        _delegateRemoveMethod = new Lazy<MethodInfo>(() =>
        {
            return typeof(Delegate).GetMethod(nameof(Delegate.Remove),
                                       BindingFlags.Public | BindingFlags.Static,
                                       new Type[2] { typeof(Delegate), typeof(Delegate) })
                                   .ThrowIfNull();
        });
    }

    internal static EqualityMethods GetEqualityMethods(Type type)
    {
        return _equalsMethods.GetOrAdd(type, FindEqualityMethods);
    }

    private static EqualityMethods FindEqualityMethods(Type type)
    {
        var ecType = typeof(EqualityComparer<>).MakeGenericType(type);
        var getDefaultMethod = ecType.GetMethod("get_Default", Reflect.StaticFlags)
                                     .ThrowIfNull();
        var equalsMethod = ecType.GetMethod("Equals", Reflect.InstanceFlags, new Type[2] { type, type })
                                 .ThrowIfNull();
        return new(getDefaultMethod, equalsMethod);
    }

    private static readonly Lazy<MethodInfo> _delegateCombineMethod;

    public static MethodInfo DelegateCombineMethod => _delegateCombineMethod.Value;

    private static readonly Lazy<MethodInfo> _delegateRemoveMethod;

    public static MethodInfo DelegateRemoveMethod => _delegateRemoveMethod.Value;




    public static MethodInfo InterlockedCompareExchange(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (type.IsClass || type.IsInterface)
        {
            return typeof(Interlocked)
                   .GetMethods(BindingFlags.Public | BindingFlags.Static)
                   .Where(method => method.Name == nameof(Interlocked.CompareExchange))
                   .FirstOrDefault(method =>
                   {
                       if (method.IsGenericMethod)
                       {
                           var methodGenericTypes = method.GetGenericArguments();
                           Debugger.Break();
                           // Have to find T : class method
                       }

                       throw new NotImplementedException();
                   })
                   .ThrowIfNull();
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}