namespace Jay.Reflection.Emitting;

public static class GetEmitterExtensions
{
    public static FluentGeneratorEmitter GetEmitter(this ILGenerator ilGenerator) => new(ilGenerator);
    public static FluentGeneratorEmitter GetEmitter(this DynamicMethod dynamicMethod) => new(dynamicMethod.GetILGenerator());
    public static FluentGeneratorEmitter GetEmitter(this MethodBuilder methodBuilder) => new(methodBuilder.GetILGenerator());
    public static FluentGeneratorEmitter GetEmitter(this ConstructorBuilder constructorBuilder) => new(constructorBuilder.GetILGenerator());
}