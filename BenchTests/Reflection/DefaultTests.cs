using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using InlineIL;
using Jay.Randomization;
using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Internal;
using Jay.Validation;

using static InlineIL.IL;

namespace Jay.BenchTests;

public class DefaultTests
{
    [Fact]
    public void ClassDefaultIsNull()
    {
        var types = Reflect.AllExportedTypes()
                           .Where(type => type.IsClass)
                           .Shuffled()
                           .Take(100);
        foreach (var type in types)
        {
            var def = TypeCache.Default(type);
            Assert.True(def is null);
        }
    }
        
    [Fact]
    public void StructDefaultActivatorCreateInstance()
    {
        var types = Reflect.AllExportedTypes()
                           .Where(type => type.IsValueType)
                           .Where(type => !type.IsByRef && !type.IsByRefLike)
                           .Where(type => !type.IsGenericType)
                           .Shuffled()
                           .Take(100);
        foreach (var type in types)
        {
            if (!Result.TryInvoke(() => Activator.CreateInstance(type), out var instance))
                continue;
            if (instance is null)
                continue;
            var def = typeof(DefaultTests)
                      .GetMethod(nameof(GetDefault), Reflect.StaticFlags)
                      .ThrowIfNull()
                      .MakeGenericMethod(type)
                      .Invoke(null, null);
            var def2 = GetDefaultOfType(type);
            var defOrig = typeof(DefaultTests)
                          .GetMethod(nameof(GetDefaultOrig), Reflect.StaticFlags)
                          .ThrowIfNull()
                          .MakeGenericMethod(type)
                          .Invoke(null, null);

            Assert.True(instance.Equals(defOrig));
            Assert.True(instance.Equals(def));
            Assert.True(instance.Equals(def2));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T GetDefault<T>()
    {
        DeclareLocals(new LocalVar("value", typeof(T)));
        Emit.Ldloca("value");
        Emit.Initobj<T>();
        Emit.Ldloc("value");
        return Return<T>();
    }
        
    [return: MaybeNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T GetDefaultOrig<T>()
    {
        return default(T);
    }

    private static object GetDefaultOfType(Type type)
    {
        var del = RuntimeBuilder.CreateDelegate<Func<Type, object>>(emitter =>
        {
            emitter.DeclareLocal(type, out var local)
                   .Ldloca(local)
                   .Initobj(type)
                   .Ldloc(local)
                   .Box(type)
                   .Ret();
        });
        return del(type);
    }
}