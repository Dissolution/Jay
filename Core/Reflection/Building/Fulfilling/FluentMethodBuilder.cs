using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Jay.Annotations;
using Jay.Collections;
using Jay.Dumping;
using Jay.Reflection.Building.Emission;
using Jay.Reflection.Search;
using Jay.Validation;

namespace Jay.Reflection.Building.Fulfilling
{
    public sealed record EqualityMethods(MethodInfo GetDefaultMethod, MethodInfo EqualsMethod);

    public static class OperatorCache
    {
        private static readonly ConcurrentTypeDictionary<EqualityMethods> _equalsMethods;

        static OperatorCache()
        {
            _equalsMethods = new();
        }

        internal static EqualityMethods GetEqualityMethods(Type type)
        {
            return _equalsMethods.GetOrAdd(type, FindEqualityMethods);
        }

        private static EqualityMethods FindEqualityMethods(Type type)
        {
            var ecType = typeof(EqualityComparer<>).MakeGenericType(type);
            var getDefaultMethod = ecType.GetMethod("get_Default", Reflect.StaticFlags)
                                         .ThrowIfNull();
            var equalsMethod = ecType.GetMethod("Equals", Reflect.InstanceFlags, new Type[2] { type, type })
                                     .ThrowIfNull();
            return new(getDefaultMethod, equalsMethod);
        }
    }

    public class Test
    {
        public Test()
        {
            dynamic builder = default!;
            builder.DefineMethod("SetValue<T>", MethodAttributes.Public)
                   .Return(typeof(void));
                   //.Parameters(p => p.Method.GenericTypes[0]);

            ITypeBuilder bldr = default!;
            bldr.DefineMethod("SetValue<T>", MethodAttributes.Public)
                .AddAttribute<MethodImplAttribute>(MethodImplOptions.AggressiveInlining)
                .Return(typeof(void))
                .Parameters(p => ("field", p.Method.GenericTypes[0].MakeByRefType()),
                    p => ("value", p.Method.GenericTypes[0]),
                    p => p.Name("propertyName").Type<string>().AddAttribute<CallerMemberNameAttribute>())
                .Emit((meth, emitter) =>
                {
                    var equalityMethods = OperatorCache.GetEqualityMethods(meth.GenericTypes[0]);
                    emitter.Call(equalityMethods.GetDefaultMethod)
                           .Ldarg(1)
                           .Ldobj(meth.GenericTypes[0])
                           .Ldarg(2)
                           .Call(equalityMethods.EqualsMethod)
                           .Brtrue(out var lblEnd);

                });

            TypeBuilder b = default!;
            var m = b.DefineMethod("SetValue", MethodAttributes.Public);
            m.GetEmitter();

            //
            //
            // IConfigMethod cm = default!;
            // var config = cm.Name("SetValue")
            //       .AsGenericMethod("T");
            // config.MethodAttributes(MethodAttributes.Public)
            //       .Parameters(p => p.Type(config.GenericTypes[0].MakeByRefType()).Name("field"),
            //           p => p.Type(config.GenericTypes[0]).Name("value"),
            //           p => p.Type<string>().AddDefault<string?>(null).AddAttribute<CallerMemberNameAttribute>())
            //       .AddAttribute<MethodImplAttribute>(MethodImplOptions.AggressiveInlining);

        }
    }


    public interface ITypeBuilder
    {
        IMethodBuilder DefineMethod(string? name,
                                    MethodAttributes methodAttributes,
                                    CallingConventions conventions = default);
    }

    public interface IMethodBuilder
    {
        Type[] GenericTypes { get; }

        IMethodBuilder Return(Type? returnType);
        IMethodBuilder Return(Func<ConfigureParameterBuilder, ConfigureParameterBuilder> configParameter);

        IMethodBuilder Parameters(params Func<ConfigureParameterBuilder, ConfigureParameterBuilder>[] configParameters);

        IMethodBuilder AddAttribute<TAttribute>()
            where TAttribute : Attribute, new();

        IMethodBuilder AddAttribute<TAttribute>(params object?[] args)
            where TAttribute : Attribute;

        IMethodBuilder Emit(Action<IILGeneratorEmitter> emit);
        IMethodBuilder Emit(Action<IMethodBuilder, IILGeneratorEmitter> emit);

    }

    public class ConfigureMethodBuilder : IMethodBuilder
    {
        private readonly MethodBuilder _methodBuilder;

        public Type[] GenericTypes { get; private set; }

        public ConfigureMethodBuilder(MethodBuilder methodBuilder)
        {
            _methodBuilder = methodBuilder;
        }

        
        public IMethodBuilder Return(Type? returnType)
        {
            _methodBuilder.SetReturnType(returnType);
            return this;
        }

        public IMethodBuilder Return(Func<ConfigureParameterBuilder, ConfigureParameterBuilder> configParameter)
        {
            var cpb = new ConfigureParameterBuilder(this, 0);
            configParameter(cpb);
            _methodBuilder.SetReturnType(cpb._type);
            var pb = _methodBuilder.DefineParameter(0, cpb._parameterAttributes, cpb._name);
            foreach (var ab in cpb._attributeBuilders)
            {
                pb.SetCustomAttribute(ab);
            }

            return this;
        }

        public IMethodBuilder Parameters(params Func<ConfigureParameterBuilder, ConfigureParameterBuilder>[] configParameters)
        {
            throw new NotImplementedException();
        }

        public IMethodBuilder AddAttribute<TAttribute>() where TAttribute : Attribute, new()
        {
            throw new NotImplementedException();
        }

        public IMethodBuilder AddAttribute<TAttribute>(params object?[] args) where TAttribute : Attribute
        {
            throw new NotImplementedException();
        }

        public IMethodBuilder Emit(Action<IILGeneratorEmitter> emit)
        {
            throw new NotImplementedException();
        }

        public IMethodBuilder Emit(Action<IMethodBuilder, IILGeneratorEmitter> emit)
        {
            throw new NotImplementedException();
        }
    }

    public class ConfigureParameterBuilder
    {
        public static implicit operator ConfigureParameterBuilder(Type type)
        {
            return new ConfigureParameterBuilder().Type(type);
        }

        public static implicit operator ConfigureParameterBuilder((string?, Type) tuple)
        {
            return new ConfigureParameterBuilder().Name(tuple.Item1).Type(tuple.Item2);
        }

        internal string? _name;
        internal Type? _type;
        internal ParameterAttributes _parameterAttributes;
        internal bool _hasDefault;
        internal object? _default;

        internal readonly List<CustomAttributeBuilder> _attributeBuilders;

        public int Position { get; }
        public IMethodBuilder Method { get; }

        private ConfigureParameterBuilder() : this(null, -1) { }

        public ConfigureParameterBuilder(ConfigureMethodBuilder methodBuilder, int position)
        {
            this.Position = position;
            this.Method = methodBuilder;
            _name = null;
            _type = null;
            _parameterAttributes = System.Reflection.ParameterAttributes.None;
            _hasDefault = false;
            _default = default;
            _attributeBuilders = new List<CustomAttributeBuilder>(0);
        }

        public ConfigureParameterBuilder Name(string? name)
        {
            _name = name;
            return this;
        }

        public ConfigureParameterBuilder Type(Type? type)
        {
            _type = type;
            return this;
        }
        public ConfigureParameterBuilder Type<T>() => Type(typeof(T));

        public ConfigureParameterBuilder ParameterAttributes(ParameterAttributes attributes)
        {
            _parameterAttributes = attributes;
            return this;
        }

        public ConfigureParameterBuilder AddAttribute<TAttribute>()
            where TAttribute : Attribute, new()
        {
            _attributeBuilders.Add(RuntimeBuilder.GetCustomAttributeBuilder<TAttribute>());
            return this;
        }
        public ConfigureParameterBuilder AddAttribute<TAttribute>(params object?[] args)
            where TAttribute : Attribute
        {
            _attributeBuilders.Add(RuntimeBuilder.GetCustomAttributeBuilder<TAttribute>(args));
            return this;
        }

        public ConfigureParameterBuilder AddDefault(object? defaultValue)
        {
            _default = defaultValue;
            _hasDefault = true;
            _parameterAttributes |= System.Reflection.ParameterAttributes.HasDefault;
            return this;
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

        T Parameters(params Action<IConfigParameter>[] configureParameters);
    }

    public interface IConfigMethod : IConfigMethod<IConfigMethod>
    {

    }

    // public class ConfigMethod : IConfigMethod
    // {
    //     private readonly MethodBuilder _methodBuilder;
    //
    //     public ConfigMethod(MethodBuilder methodBuilder)
    //     {
    //         _methodBuilder = methodBuilder;
    //     }
    //
    //     public IConfigMethod AddAttribute(Type attributeType, params object?[] attributeCtorArgs)
    //     {
    //         _methodBuilder.SetCustomAttribute(RuntimeBuilder.GetCustomAttributeBuilder(attributeType, attributeCtorArgs));
    //         return this;
    //     }
    //
    //     public IConfigMethod AddAttribute<TAttribute>(params object?[] attributeCtorArgs) where TAttribute : Attribute
    //     {
    //         _methodBuilder.SetCustomAttribute(RuntimeBuilder.GetCustomAttributeBuilder<TAttribute>(attributeCtorArgs));
    //         return this;
    //     }
    //
    //     public IConfigMethod AddAttribute<TAttribute>() where TAttribute : Attribute, new()
    //     {
    //         _methodBuilder.SetCustomAttribute(RuntimeBuilder.GetCustomAttributeBuilder<TAttribute>());
    //         return this;
    //     }
    //
    //     private static readonly FieldInfo _methodBuilderNameField = typeof(MethodBuilder)
    //                                                                 .GetField("m_strName", Reflect.InstanceFlags)
    //                                                                 .ThrowIfNull("Cannot find MethodBuilder.Name backing field");
    //     public IConfigMethod Name(string name)
    //     {
    //         if (!RuntimeBuilder.IsValidMemberName(name))
    //             Dump.ThrowException<ArgumentException>($"{name} is not a valid method name", nameof(name));
    //         _methodBuilderNameField.SetValue<MethodBuilder, string>(_methodBuilder, name);
    //         return this;
    //     }
    //
    //     private static readonly FieldInfo _methodBuilderAttributesField = typeof(MethodBuilder)
    //                                                                 .GetField("m_iAttributes", Reflect.InstanceFlags)
    //                                                                 .ThrowIfNull("Cannot find MethodBuilder.Attributes backing field");
    //
    //     public IConfigMethod MethodAttributes(MethodAttributes attributes)
    //     {
    //         _methodBuilderAttributesField.SetValue<MethodBuilder, MethodAttributes>(_methodBuilder, attributes);
    //         return this;
    //     }
    //
    //     private static readonly FieldInfo _methodBuilderCallingConventionsField = typeof(MethodBuilder)
    //                                                                       .GetField("m_callingConvention", Reflect.InstanceFlags)
    //                                                                       .ThrowIfNull("Cannot find MethodBuilder.CallingConventions backing field");
    //
    //     public IConfigMethod CallingConventions(CallingConventions conventions)
    //     {
    //         _methodBuilderCallingConventionsField.SetValue<MethodBuilder, CallingConventions>(_methodBuilder, conventions);
    //         return this;
    //     }
    //
    //     public IConfigGenericMethod AsGenericMethod(params string[] genericTypeNames)
    //     {
    //         return new ConfigGenericMethod(_methodBuilder, _methodBuilder.DefineGenericParameters(genericTypeNames));
    //     }
    //
    //     public IConfigMethod Parameters(Action<IConfigParameter> configureParameter)
    //     {
    //         throw new NotImplementedException();
    //     }
    // }

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
        T Type(Type type);
        T Type<TParameter>() => Type(typeof(TParameter));
        T AddDefault(object? defaultValue);
        T AddDefault<TParameter>(TParameter? defaultValue) => AddDefault((object?)defaultValue);
    }

    public interface IConfigParameter : IConfigParameter<IConfigParameter>
    {

    }
    

  
}
