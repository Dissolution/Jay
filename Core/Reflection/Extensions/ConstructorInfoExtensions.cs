using System.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Caching;

namespace Jay.Reflection;

public static class ConstructorInfoExtensions
{
    public static Constructor<TInstance> CreateConstructor<TInstance>(this ConstructorInfo ctor)
    {
        ArgumentNullException.ThrowIfNull(ctor);
        return RuntimeBuilder.CreateDelegate<Constructor<TInstance>>($"{typeof(TInstance)}.ctor(params)",
                                                                     method =>
                                                                     {
                                                                         method.Emitter
                                                                               .LoadParams(method.Parameters[0],
                                                                                   ctor.GetParameters())
                                                                               .Newobj(ctor)
                                                                               .Cast(ctor.DeclaringType!,
                                                                                   method.ReturnType)
                                                                               .Ret();
                                                                     });
    }

    public static TInstance Construct<TInstance>(this ConstructorInfo ctor, params object?[] args)
    {
        var del = DelegateMemberCache.Instance
                                     .GetOrAdd(ctor, CreateConstructor<TInstance>);
        return del(args);
    }
}