using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Collections;
using Jay.Validation;

namespace Jay.Reflection.Building;

public static class ExceptionBuilder
{
    internal delegate TException ExceptionCtor<out TException>(string message, Exception? innerException)
        where TException : Exception;

    private static readonly ConcurrentTypeDictionary<Delegate> _ctorCache;

    static ExceptionBuilder()
    {
        _ctorCache = new ConcurrentTypeDictionary<Delegate>();
    }

    private static ExceptionCtor<TException> CreateCtor<TException>(Type exceptionType)
        where TException : Exception
    {
        var dm = RuntimeBuilder.CreateDynamicMethod<ExceptionCtor<TException>>($"ctor_{typeof(TException).Name}");
        var emitter = dm.Emitter;
        ConstructorInfo? ctor;
        ctor = exceptionType.GetConstructor(Reflect.InstanceFlags, new Type[2] { typeof(string), typeof(Exception) });
        if (ctor is not null)
        {
            emitter.Ldarg(0)
                   .Ldarg(1)
                   .Newobj(ctor)
                   .Ret();
        }
        else
        {
            ctor = exceptionType.GetConstructor(Reflect.InstanceFlags, new Type[1] { typeof(string) });
            if (ctor is not null)
            {
                emitter.Ldarg(0)
                       .Newobj(ctor)
                       .Ret();
            }
            else
            {
                ctor = exceptionType.GetConstructor(Reflect.InstanceFlags, new Type[1] { typeof(Exception) });
                if (ctor is not null)
                {
                    emitter.Ldarg(1)
                           .Newobj(ctor)
                           .Ret();
                }
                else
                {
                    ctor = exceptionType.GetConstructor(Reflect.InstanceFlags, Type.EmptyTypes);
                    if (ctor is not null)
                    {
                        emitter.Newobj(ctor)
                               .Ret();
                    }
                    else
                    {
                        emitter.LoadUninitialized(exceptionType)
                               .Ret();
                    }
                }
            }
        }

        return dm.CreateDelegate();
    }


    internal static ExceptionCtor<TException> GetCtor<TException>()
        where TException : Exception
    {
        return (_ctorCache.GetOrAdd(typeof(TException), CreateCtor<TException>) as
            ExceptionCtor<TException>).ThrowIfNull();
    }

    public static TException CreateException<TException>(ref DefaultInterpolatedStringHandler message,
                                                         Exception? innerException = null)
        where TException : Exception
    {
        return GetCtor<TException>()(message.ToStringAndClear(), innerException);
    }
}