using System;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Text;

namespace Jay.Reflection.Runtime
{
    public static class RuntimeTypeBuilder
    {
        internal static readonly AssemblyBuilder _assemblyBuilder;
        internal static readonly ModuleBuilder _moduleBuilder;

        static RuntimeTypeBuilder()
        {
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(nameof(RuntimeBuilder)),
                                                                     AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(RuntimeBuilder.Module.Name);
        }
        
        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public sealed class SignificantAttribute : Attribute
        {
            public bool Significant { get; }
            
            public SignificantAttribute()
            {
                
            }
        }

        public enum Participates
        {
            All = 0,
            Specific = 1,
        }
        
        [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
        public sealed class ParticipatesAttribute : Attribute
        {
            public Participates Participates { get; }

            public ParticipatesAttribute(Participates participates)
            {
                this.Participates = participates;
            }
        }

        public static TypeBuilder CreateTypeBuilder(string name,
                                                    TypeAttributes typeAttributes,
                                                    Type? parentType = null)

        {
            return _moduleBuilder.DefineType(name, typeAttributes, parentType);
        }
        
        public static TInterface BuildRecordType<TInterface>()
            where TInterface : class
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
                throw new ArgumentException("The generic type must be an interface type", nameof(TInterface));
            
            Participates participates;
            var attr = interfaceType.GetCustomAttribute<ParticipatesAttribute>();
            if (attr != null)
            {
                participates = attr.Participates;
            }
            else
            {
                participates = Participates.All;
            }

            var interfaceName = interfaceType.Name;
            string typeName = TextBuilder.Build(text =>
            {
                char c = interfaceName[0];
                int i = c == 'I' ? 1 : 0;
                c = interfaceName[i];
                if (RuntimeBuilder.IsValidIdentifierChar(c, true))
                    text.Write(c);
                for (i++; i < interfaceName.Length; i++)
                {
                    c = interfaceName[i];
                    if (RuntimeBuilder.IsValidIdentifierChar(c, false))
                        text.Write(c);
                }
            });
            var typeBuilder = CreateTypeBuilder(typeName,
                                                TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Class,
                                                typeof(RuntimeTypeBuilder));
          
            throw new NotImplementedException();


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