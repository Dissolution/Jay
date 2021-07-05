using System;
using System.Reflection;
using Jay.Reflection;
using Jay.Reflection.Emission;

namespace Jay.Debugging.Dumping
{
    public static partial class Dumper
    {
        public static TException NewEx<TException>(FormattableString message,
                                                   Exception? innerException = null)
            where TException : Exception
        {
            var ctor = typeof(TException).GetConstructor(Reflect.InstanceFlags,
                                                         typeof(string), typeof(Exception));
            if (ctor != null)
            {
                var adapter = ctor.Adapt<Func<string, Exception?, TException>>();
                return adapter(Format(message), innerException);
            }

            ctor = typeof(TException).GetConstructor(Reflect.InstanceFlags,
                                                     typeof(string));
            if (ctor != null)
            {
                var adapter = ctor.Adapt<Func<string, TException>>();
                return adapter(Format(message));
            }

            if (innerException != null)
            {
                ctor = typeof(TException).GetConstructor(Reflect.InstanceFlags,
                                                         typeof(Exception));
                if (ctor != null)
                {
                    var adapter = ctor.Adapt<Func<Exception, TException>>();
                    return adapter(innerException);
                }
            }

            ctor = typeof(TException).GetConstructor(Reflect.InstanceFlags);
            if (ctor != null)
            {
                var adapter = ctor.Adapt<Func<TException>>();
                return adapter();
            }

            throw new InvalidOperationException(Format($"Cannot construct a {typeof(TException)}"));
        }

        // public static TException NewEx<TException>(params object?[] args)
        //     where TException : Exception
        // {
        //     
        // }
    }
}