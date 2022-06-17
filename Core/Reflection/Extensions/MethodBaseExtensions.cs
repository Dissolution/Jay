using System.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Adapting;
using Jay.Reflection.Building.Deconstruction;
using Jay.Reflection.Building.Emission;
using Jay.Reflection.Caching;

namespace Jay.Reflection.Extensions;

public static class MethodBaseExtensions
{
    public static Visibility Visibility(this MethodBase? method)
    {
        Visibility visibility = Reflection.Visibility.None;
        if (method is null)
            return visibility;
        if (method.IsPrivate)
            visibility |= Reflection.Visibility.Private;
        if (method.IsFamily)
            visibility |= Reflection.Visibility.Protected;
        if (method.IsAssembly)
            visibility |= Reflection.Visibility.Internal;
        if (method.IsPublic)
            visibility |= Reflection.Visibility.Public;
        return visibility;
    }

    public static bool IsStatic(this MethodBase? method)
    {
        return method is not null && method.IsStatic;
    }

    public static Type ReturnType(this MethodBase? method)
    {
        if (method is null)
            return typeof(void);
        if (method is MethodInfo methodInfo)
            return methodInfo.ReturnType;
        if (method is ConstructorInfo constructorInfo)
            return constructorInfo.DeclaringType!;
        return typeof(void);
    }

    public static InstructionStream GetInstructions(this MethodBase method)
    {
        return new RuntimeDeconstructor(method)
            .GetInstructions();
    }
    
    public static Result.Result TryAdapt<TDelegate>(this MethodBase method, [NotNullWhen(true)] out TDelegate? @delegate)
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

    public static TDelegate Adapt<TDelegate>(this MethodBase method)
        where TDelegate : Delegate
    {
        return DelegateMemberCache.Instance
                                  .GetOrAdd(method, dm =>
                                  {
                                      TryAdapt<TDelegate>((dm.Member as MethodInfo)!, out var del).ThrowIfFailed();
                                      return del!;
                                  });
    }
}