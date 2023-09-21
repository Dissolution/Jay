using Jay.Reflection.Builders;
using Jay.Reflection.Caching;
using Jay.Reflection.Emitting.Args;

namespace Jay.Reflection.Adapters;

public static class ConstructorAdapter
{
    private static Construct<TInstance> CreateConstructDelegate<TInstance>(ConstructorInfo ctor)
    {
        return RuntimeBuilder.BuildDelegate<Construct<TInstance>>(
            $"construct_{ctor.OwnerType().Name}",
            builder => builder
                .Emitter
                .EmitLoadParams(builder.FirstParameter!, ctor.GetParameters())
                .Newobj(ctor)
                .EmitCast(ctor.DeclaringType!, typeof(TInstance))
                .Ret());
    }

    public static Construct<TInstance> GetConstructDelegate<TInstance>(ConstructorInfo ctor)
    {
        return MemberDelegateCache.GetOrAdd(ctor, CreateConstructDelegate<TInstance>);
    }

    public static TInstance Construct<TInstance>(this ConstructorInfo constructorInfo, params object?[] args)
    {
        return GetConstructDelegate<TInstance>(constructorInfo)(args);
    }
}