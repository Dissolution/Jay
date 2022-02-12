using System.Diagnostics;
using System.Reflection;
using Jay.Collections;
using Jay.Reflection;
using Jay.Reflection.Building.Fulfilling;
using Jay.Validation;



public static class MethodCache
{
    private static readonly ConcurrentTypeDictionary<EqualityMethods> _equalsMethods;

    static MethodCache()
    {
        _equalsMethods = new();
       
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