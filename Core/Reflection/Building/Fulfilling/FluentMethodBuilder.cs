using System.Reflection;
using System.Reflection.Emit;
using Jay.Reflection.Building.Emission;

namespace Jay.Reflection.Building.Fulfilling;

public class Test
{
    public Test()
    {
    }
}


public interface ITypeBuilder
{
    MethodBuilder DefineMethod(string? name,
                               MethodAttributes methodAttributes,
                               CallingConventions conventions = default,
                               Action<IMethodBuilder>? configureMethod = null);
}

public interface IMethod
{
    string Name { get;}
    MethodAttributes MethodAttributes { get;}
    CallingConventions CallingConventions { get;}

    Type[] GenericTypes { get;}
}

public interface IMethodBuilder : IMethod
{
    IMethodBuilder Return(Type? returnType, Action<IParameterBuilder>? configureParameter = null);
    IMethodBuilder Return<T>(Action<IParameterBuilder>? configureParameter = null);

    IMethodBuilder AddParameter(Type type,
                                string name,
                                ParameterAttributes attributes,
                                Action<IParameterBuilder>? configureParameter = null);

    IMethodBuilder AddParameter(Type type,
                                string name,
                                Action<IParameterBuilder>? configureParameter = null)
        => AddParameter(type, name, ParameterAttributes.None, configureParameter);
      

    IMethodBuilder AddParameter<T>(string name,
                                   ParameterAttributes attributes,
                                   Action<IParameterBuilder<T>>? configureParameter = null);

    IMethodBuilder AddParameter<T>(string name,
                                   Action<IParameterBuilder<T>>? configureParameter = null) 
        => AddParameter<T>(name, ParameterAttributes.None, configureParameter);

    IMethodBuilder AddAttribute<TAttribute>()
        where TAttribute : Attribute, new();

    IMethodBuilder AddAttribute<TAttribute>(params object?[] args)
        where TAttribute : Attribute;

    IMethodBuilder Emit(Action<IILGeneratorEmitter> emit);

}

public interface IParameter
{
    string? Name { get; }
    int Position { get; }
    ParameterAttributes ParameterAttributes { get; }
    Type Type { get; }
}

public interface IParameterBuilder : IParameter
{
    IParameterBuilder SetDefault(object? defaultValue);

    IParameterBuilder AddAttribute<TAttribute>(params object?[] args)
        where TAttribute : Attribute;
}

public interface IParameterBuilder<in T> : IParameter
{
    IParameterBuilder<T> SetDefault(T? defaultValue);

    IParameterBuilder<T> AddAttribute<TAttribute>(params object?[] args)
        where TAttribute : Attribute;
}