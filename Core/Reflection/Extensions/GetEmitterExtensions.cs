using System.Reflection.Emit;
using Jay.Reflection.Building.Emission;

namespace Jay.Reflection.Extensions;

public static  class GetEmitterExtensions
{
    public static IILGeneratorEmitter GetEmitter(this ILGenerator ilGenerator)
    {
        return new ILGeneratorEmitter(ilGenerator);
    }

    public static IILGeneratorEmitter GetEmitter(this DynamicMethod dynamicMethod)
    {
        return new ILGeneratorEmitter(dynamicMethod.GetILGenerator());
    }

    public static IILGeneratorEmitter GetEmitter(this MethodBuilder methodBuilder)
    {
        return new ILGeneratorEmitter(methodBuilder.GetILGenerator());
    }

    public static IILGeneratorEmitter GetEmitter(this ConstructorBuilder constructorBuilder)
    {
        return new ILGeneratorEmitter(constructorBuilder.GetILGenerator());
    }
}