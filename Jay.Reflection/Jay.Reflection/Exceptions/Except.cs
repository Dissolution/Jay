using Jay.Reflection.Adapters;
using Jay.Reflection.Caching;
using Jay.Reflection.Collections;

namespace Jay.Reflection.Exceptions;

public static class Except
{
    private static readonly ConcurrentMemberDelegateMap _exceptionCtorCache = new();

    private static TDelegate GetCtor<TException, TDelegate>()
        where TException : Exception
        where TDelegate : Delegate
    {
        var key = MemberDelegate.Create<Type, TDelegate>(typeof(TException));
        var del = _exceptionCtorCache.GetOrAdd(key,
            (md) =>
            {
                var argTypes = md.DelegateInfo.ParameterTypes;
                var ctor = md.Member
                    .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(ctor => Easy.SeqEqual<Type>(ctor.GetParameterTypes(), argTypes))
                    .OneOrDefault();
                if (ctor is not null)
                {
                    var result = RuntimeMethodAdapter.TryAdapt<TDelegate>(ctor, out var del);
                    if (result)
                    {
                        return del!;
                    }
                }
                throw new InvalidOperationException("Cannot");
            });
        return del;
    }
    
    public static TException New<TException>(InterpolatedCodeBuilder message, Exception? innerException = null)
        where TException : Exception
    {
        var ctor = GetCtor<TException, Func<string?, Exception?, TException>>();
        return ctor(message.ToStringAndDispose(), innerException);
    }

    // public static TException New<TException>(ref InterpolatedRenderer message, params object?[]? additionalCtorArgs)
    //     where TException : Exception
    // {
    //     var ctor = GetCtor<TException, Func<string?, object?[]?, TException>>();
    //     return ctor(message.ToStringAndDispose(), additionalCtorArgs);
    // }
    //
    // public static TException New<TException>(params object?[]? ctorArgs)
    //     where TException : Exception
    // {
    //     
    // }
}