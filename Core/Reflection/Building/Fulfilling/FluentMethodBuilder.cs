using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Jay.Annotations;

namespace Jay.Reflection.Building.Fulfilling
{
    public class Test
    {
        public Test()
        {
            CustomAttributeBuilder bldr = default!;
            
        }
    }

    internal class BLDR : IMethodBuilder
    {
        private readonly MethodBuilder _methodBuilder;

        public string Name => _methodBuilder.Name;

        public MethodAttributes MethodAttributes => _methodBuilder.Attributes;

        public CallingConventions CallingConventions => _methodBuilder.CallingConvention;

        public BLDR(MethodBuilder methodBuilder)
        {
            ArgumentNullException.ThrowIfNull(methodBuilder);
            _methodBuilder = methodBuilder;
        }

        public void AddAttribute<TAttribute>() where TAttribute : Attribute, new()
        {
            var customAttributeBuilder = RuntimeBuilder.GetCustomAttributeBuilder<TAttribute>();
            _methodBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        public void AddAttribute<TAttribute>(params object?[] args) where TAttribute : Attribute
        {
            var customAttributeBuilder = RuntimeBuilder.GetCustomAttributeBuilder<TAttribute>(args);
            _methodBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        public void AddAttribute(Type attributeType, params object?[] args)
        {
            var customAttributeBuilder = RuntimeBuilder.GetCustomAttributeBuilder(attributeType, args);
            _methodBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        public IGenericMethodBuilder MakeGeneric(params string[] genericTypeNames)
        {
            return new G_BLDR(_methodBuilder, _methodBuilder.DefineGenericParameters(genericTypeNames));
        }

        public void SetParameters(params Action<IParameterBuilder>[] buildParameters)
        {
            var parameterCount = buildParameters.Length;
            _methodBuilder.SetParameters();

            for (var p = 0; p < buildParameters.Length; p++)
            {
                var build = buildParameters[p];
            }
        }
    }

    internal class G_BLDR : BLDR, IGenericMethodBuilder
    {
        private readonly GenericTypeParameterBuilder[] _genericTypeParameterBuilders;

        public Type[] GenericTypes { get; }

        internal G_BLDR(MethodBuilder methodBuilder, GenericTypeParameterBuilder[] genericTypeParameterBuilders) 
            : base(methodBuilder)
        {
            _genericTypeParameterBuilders = genericTypeParameterBuilders;
            GenericTypes = new Type[_genericTypeParameterBuilders.Length];
            for (int i = 0; i < _genericTypeParameterBuilders.Length; i++)
                GenericTypes[i] = _genericTypeParameterBuilders[i];
        }
    }

    public interface IFluent<T>
        where T : IFluent<T>
    {

    }

    public interface IConfigAttributes<T> : IFluent<T>
        where T : IConfigAttributes<T>
    {
        T AddAttribute(Type attributeType, params object?[] attributeCtorArgs);
        T AddAttribute<TAttribute>(params object?[] attributeCtorArgs)
            where TAttribute : Attribute;
        T AddAttribute<TAttribute>()
            where TAttribute : Attribute, new();
    }

    public interface IConfigMethod<T> : IConfigAttributes<T>, IFluent<T>
        where T : IConfigMethod<T>
    {
        T Name(string name);
        T MethodAttributes(MethodAttributes attributes);
        T CallingConventions(CallingConventions conventions);

        IConfigGenericMethod AsGenericMethod(params string[] genericTypeNames);

        T AddParameter(Action<IConfigParameter> configureParameter);
    }

    public interface IConfigGenericMethod<T> : IConfigMethod<T>, IConfigAttributes<T>, IFluent<T>
        where T : IConfigGenericMethod<T>
    {
        TypeInfo[] GenericTypes { get; }
    }

    public interface IConfigGenericMethod : IConfigGenericMethod<IConfigGenericMethod>
    {

    }



    public interface IConfigParameter<T> : IConfigAttributes<T>, IFluent<T>
        where T : IConfigParameter<T>
    {
        int Index { get; }

        T Name(string? name);
        T ParameterAttributes(ParameterAttributes attributes);
        T ParameterType(Type type);
        T AddDefault(object? defaultValue);
    }

    public interface IConfigParameter : IConfigParameter<IConfigParameter>
    {

    }
    

  
}
