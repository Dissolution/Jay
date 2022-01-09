using System.Reflection.Emit;
using Jay.Reflection.Emission;

namespace Jay.Reflection;

public static class DynamicMethodExtensions
{
    public static ILGenerator ILGenerator(this DynamicMethod dynamicMethod)
    {
        return dynamicMethod.GetILGenerator();
    }

    public static IILGeneratorEmitter Emitter(this DynamicMethod dynamicMethod)
    {
        return new ILGeneratorEmitter(dynamicMethod.GetILGenerator());
    }
    // public static IILGeneratorFluentEmitter FluentEmitter(this DynamicMethod dynamicMethod)
    // {
    //     return new ILGeneratorEmitter(dynamicMethod.GetILGenerator());
    // }
}