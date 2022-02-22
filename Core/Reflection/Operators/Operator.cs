using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Comparision;
using Jay.Reflection.Building.Emission;
using Jay.Reflection.Building.Fulfilling;

namespace Jay.Reflection.Operators;

public enum Operator
{
    None = 0,
    Equals,
}

public class OpCache
{
    public Action<IILGeneratorEmitter> EmitOp { get; }
}

public static class Operators
{
    private readonly record struct OpKey(Operator Operator, Type[] Types);


    private static readonly ConcurrentDictionary<OpKey, OpCache> _cache;

    static Operators()
    {
        _cache = new();
    }

    public static IILGeneratorEmitter EmitEquals(this IILGeneratorEmitter emitter, params Type[] types)
    {
        Type firstArgType;
        Type secondArgType;
        Type returnType = typeof(bool);
        if (types.Length == 1)
        {
            firstArgType = secondArgType = types[0];
        }
        else if (types.Length == 2)
        {
            firstArgType = types[0];
            secondArgType = types[1];
        }
        else
        {
            throw new ArgumentException("You must pass 1 or 2 two Type arguments", nameof(types));
        }

        var equalsMethod = firstArgType.GetMethods(Reflect.InstanceFlags)
                                       .Where(method => string.Equals(method.Name, "Equals"))
                                       .Where(method => method.ReturnType == typeof(bool))
                                       .Where(method =>
                                       {
                                           var methodParameters = method.GetParameters();
                                           return methodParameters.Length == 1 &&
                                                  methodParameters[0].ParameterType == secondArgType;
                                       })
                                       .OrderBy(method => method.Name, "Equals", "op_Equality")
                                       .FirstOrDefault();
        if (equalsMethod is not null)
        {
            emitter.Call(equalsMethod);
        }

        throw new NotImplementedException();
    }
    
    public static IILGeneratorEmitter EmitOp(this IILGeneratorEmitter emitter,
                                             Operator op,
                                             params Type[] types)
    {
        throw new NotImplementedException();
    }
}



