using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Building.Backing;

internal class ConstructorImplementer : Implementer, IConstructorImplementer
{
    /// <inheritdoc />
    public ConstructorImplementer(TypeBuilder typeBuilder, IAttributeImplementer attributeImplementer) : base(typeBuilder, attributeImplementer)
    {
    }

    /// <inheritdoc />
    public ConstructorBuilder ImplementConstructor(ConstructorInfo ctor)
    {
        var constructorBuilder = _typeBuilder.DefineConstructor(ctor.Attributes, GetCallingConventions(ctor), ctor.GetParameterTypes());
        _attributeImplementer.ImplementAttributes(ctor, constructorBuilder.SetCustomAttribute);
        return constructorBuilder;
    }

    /// <inheritdoc />
    public ConstructorBuilder ImplementDefaultConstructor(MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.SpecialName)
    {
        var constructorBuilder = _typeBuilder.DefineDefaultConstructor(attributes);
        return constructorBuilder;
    }
}