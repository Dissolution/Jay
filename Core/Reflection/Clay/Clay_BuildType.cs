using System;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Debugging;

namespace Jay.Reflection
{
    public static partial class Clay
    {
        public static partial class Build
        {
            private const string AssemblyName = "Jay.Reflection.Clay";
            private const string ModuleName = "Build";

            private static readonly AssemblyBuilder _assemblyBuilder;
            private static readonly ModuleBuilder _moduleBuilder;

            static Build()
            {
                _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(AssemblyName),
                                                                         AssemblyBuilderAccess.Run);
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule(ModuleName);
            }

            public static TypeBuilder GetTypeBuilder(string typeName,
                                                     TypeAttributes typeAttributes = TypeAttributes.Public)
                => _moduleBuilder.DefineType(typeName, typeAttributes, typeof(Build));
            
            public static Type Type(string typeName,
                                    Action<TypeBuilder> buildType)
            {
                var typeBuilder = _moduleBuilder.DefineType(typeName, TypeAttributes.Public, typeof(Build));
                buildType(typeBuilder);
                return typeBuilder.CreateType() ??
                       throw new InvalidOperationException("Could not build type");
            }
        }
    }
}