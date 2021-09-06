using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Jay.Debugging;


namespace Jay.Reflection.Runtime
{
    internal class NotifyTest : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;
        
        /// <inheritdoc />
        public event PropertyChangingEventHandler? PropertyChanging;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
    }


    public sealed class RuntimeTypeBuilder
    {
        internal readonly AssemblyBuilder _assemblyBuilder;
        internal readonly ModuleBuilder _moduleBuilder;

        internal RuntimeTypeBuilder()
        {
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(nameof(RuntimeBuilder)),
                                                                     AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(RuntimeBuilder.Module.Name);
        }

        public TypeBuilder CreateTypeBuilder(string name,
                                             TypeAttributes typeAttributes,
                                             Type? parentType = null)
        {
            return _moduleBuilder.DefineType(name, 
                                             typeAttributes, 
                                             parentType ?? typeof(RuntimeTypeBuilder));
        }

        // private MethodBuilder CreateOnNotifyChangingMethod(TypeBuilder typeBuilder)
        // {
        //     var eventBuilder = typeBuilder.DefineField()
        //     
        //     var methodBuilder = typeBuilder.DefineMethod("OnPropertyChanging",
        //                                     MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot,
        //                                     CallingConventions.HasThis,
        //                                     typeof(void),
        //                                     new Type[] {typeof(string)});
        //     var propertyNameParameter = methodBuilder.DefineParameter(0, ParameterAttributes.Optional, "propertyName");
        //     propertyNameParameter.SetConstant((string?) null);
        //     propertyNameParameter.SetCustomAttribute(new CustomAttributeBuilder(typeof(CallerMemberNameAttribute).GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes),
        //                                                                         Array.Empty<object?>()));
        //     methodBuilder.Generate(gen =>
        //     {
        //         gen.Ldarg(0)
        //            .Ldfld()
        //     });
        // }
        /* 

    // [26 13 - 26 94]
    IL_0001: ldarg.0      // this
    IL_0002: ldfld        class [System.ObjectModel]System.ComponentModel.PropertyChangingEventHandler Jay.Reflection.Runtime.NotifyTest::PropertyChanging
    IL_0007: dup
    IL_0008: brtrue.s     IL_000d
    IL_000a: pop
    IL_000b: br.s         IL_001a
    IL_000d: ldarg.0      // this
    IL_000e: ldarg.1      // propertyName
    IL_000f: newobj       instance void [System.ObjectModel]System.ComponentModel.PropertyChangingEventArgs::.ctor(string)
    IL_0014: callvirt     instance void [System.ObjectModel]System.ComponentModel.PropertyChangingEventHandler::Invoke(object, class [System.ObjectModel]System.ComponentModel.PropertyChangingEventArgs)
    IL_0019: nop

    // [27 9 - 27 10]
    IL_001a: ret

  } // end of method NotifyTest::OnPropertyChanging
         *
         *
         *
         * 
         */
        
        public Type BuildImplementation<TInterface>()
            where TInterface : class
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
                throw new ArgumentException("The generic type must be an interface type", nameof(TInterface));
         
            var interfaceName = interfaceType.Name;
            string typeName = interfaceName.TrimStart('I');
            var typeBuilder = CreateTypeBuilder(typeName,
                                                TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Class);
            
            // What other interfaces are we implementing?
            bool implNotifyChanged = false;
            bool implNotifyChanging = false;
            bool implDisposable = false;
            var interfaces = interfaceType.GetInterfaces();
            foreach (var face in interfaces)
            {
                if (face == typeof(INotifyPropertyChanged))
                    implNotifyChanged = true;
                else if (face == typeof(INotifyPropertyChanging))
                    implNotifyChanging = true;
                else if (face == typeof(IDisposable))
                    implDisposable = true;
                else
                {
                    Hold.Debug(face);
                    throw new ArgumentException("The generic type has an interface that cannot be implemented");
                }
            }
            
            MethodBuilder? onNotifyMethod;
            if (implNotifyChanged || implNotifyChanging)
            {
                //onNotifyMethod = typeBuilder.DefineMethod();
            }
            
            // We always have an empty constructor
            var ctorBuilder = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            
            // Properties
            var properties = interfaceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                string name = property.Name;
                // We always have a base field
                var fieldBuilder = typeBuilder.DefineField(RuntimeBuilder.ToFieldName(name),
                                                           property.PropertyType,
                                                           FieldAttributes.Private);
                // The property
                if (property.GetIndexParameters().Length > 0)
                    throw new NotImplementedException();
                var propertyBuilder = typeBuilder.DefineProperty(name,
                                                                 PropertyAttributes.None,
                                                                 property.PropertyType,
                                                                 null);
                // Setter?
                if (property.SetMethod != null)
                {
                    var propertySetMethod = typeBuilder.DefineMethod($"set_{name}",
                                                                     MethodAttributes.Private,
                                                                     CallingConventions.HasThis,
                                                                     typeof(void),
                                                                     new Type[] {property.PropertyType});
                    
                }
            }
            
            
            
            
            
            typeBuilder.AddInterfaceImplementation(interfaceType);
            Type? implType = typeBuilder.CreateType();
            if (implType is null)
                throw new InvalidOperationException($"Cannot build a implementation of {interfaceName}");
            return implType;
        }
    }

    internal sealed class RecordTypeBuilder
    {
        private readonly TypeBuilder _typeBuilder;
        private readonly Type _interfaceType;

        public RecordTypeBuilder(TypeBuilder typeBuilder, Type interfaceType)
        {
            _typeBuilder = typeBuilder;
            _interfaceType = interfaceType;
            
            var interfaces = interfaceType.GetInterfaces();
            foreach (var interfac in interfaces)
            {
                throw new NotImplementedException();
            }
        }

        public Type BuildType()
        {
            throw new NotImplementedException();
        }
    }
}