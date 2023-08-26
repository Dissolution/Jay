namespace Jay.Reflection.Builders;

public static class Delegates
{
    public static Type GetActionType(params Type[] argumentTypes)
    {
        var type = argumentTypes.Length switch
        {
            00 => typeof(Action),
            01 => typeof(Action<>),
            02 => typeof(Action<,>),
            03 => typeof(Action<,,>),
            04 => typeof(Action<,,,>),
            05 => typeof(Action<,,,,>),
            06 => typeof(Action<,,,,,>),
            07 => typeof(Action<,,,,,,>),
            08 => typeof(Action<,,,,,,,>),
            09 => typeof(Action<,,,,,,,,>),
            10 => typeof(Action<,,,,,,,,,>),
            11 => typeof(Action<,,,,,,,,,,>),
            12 => typeof(Action<,,,,,,,,,,,>),
            13 => typeof(Action<,,,,,,,,,,,,>),
            14 => typeof(Action<,,,,,,,,,,,,,>),
            15 => typeof(Action<,,,,,,,,,,,,,,>),
            16 => typeof(Action<,,,,,,,,,,,,,,,>),
            _ => throw new ArgumentException("Only supports 0-16 generic types", nameof(argumentTypes)),
        };
        return type.MakeGenericType(argumentTypes);
    }
    
    public static Type GetFuncType(Type returnType, params Type[] argumentTypes)
    {
        int argCount = argumentTypes.Length;
        var type = argCount switch
        {
            00 => typeof(Func<>),
            01 => typeof(Func<,>),
            02 => typeof(Func<,,>),
            03 => typeof(Func<,,,>),
            04 => typeof(Func<,,,,>),
            05 => typeof(Func<,,,,,>),
            06 => typeof(Func<,,,,,,>),
            07 => typeof(Func<,,,,,,,>),
            08 => typeof(Func<,,,,,,,,>),
            09 => typeof(Func<,,,,,,,,,>),
            10 => typeof(Func<,,,,,,,,,,>),
            11 => typeof(Func<,,,,,,,,,,,>),
            12 => typeof(Func<,,,,,,,,,,,,>),
            13 => typeof(Func<,,,,,,,,,,,,,>),
            14 => typeof(Func<,,,,,,,,,,,,,,>),
            15 => typeof(Func<,,,,,,,,,,,,,,,>),
            16 => typeof(Func<,,,,,,,,,,,,,,,,>),
            _ => throw new ArgumentException("Only supports 0-16 generic types", nameof(argumentTypes)),
        };
        // have to account for last type being return type
        var funcTypes = new Type[argCount + 1];
        Easy.CopyTo<Type>(argumentTypes, funcTypes);
        funcTypes[^1] = returnType;
        return type.MakeGenericType(funcTypes);
    }
}