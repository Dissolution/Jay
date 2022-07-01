using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Building.Backing;

public interface IPropertyGetMethodImplementer
{
    MethodBuilder ImplementGetMethod(FieldBuilder backingField, PropertyBuilder property);
}

internal class DefaultInstancePropertyGetMethodImplementer : Implementer, IPropertyGetMethodImplementer
{
    public DefaultInstancePropertyGetMethodImplementer(TypeBuilder typeBuilder, IAttributeImplementer attributeImplementer) : base(typeBuilder, attributeImplementer)
    {
    }

    public MethodBuilder ImplementGetMethod(FieldBuilder backingField, PropertyBuilder property)
    {
        if (property.GetIndexParameters().Length > 0)
            throw new NotImplementedException();
        if (property.IsStatic())
            throw new NotImplementedException();

        var getMethod = _typeBuilder.DefineMethod(
                $"get_{property.Name}",
                MethodAttributes.Private,
                GetCallingConventions(property),
                property.PropertyType,
                Type.EmptyTypes)
            .Emit(emitter => emitter.Ldarg_0() // this
                .Ldfld(backingField)
                .Ret());

        property.SetGetMethod(getMethod);
        return getMethod;
    }
}

internal class DefaultStaticPropertyGetMethodImplementer : Implementer, IPropertyGetMethodImplementer
{
    public DefaultStaticPropertyGetMethodImplementer(TypeBuilder typeBuilder, IAttributeImplementer attributeImplementer) : base(typeBuilder, attributeImplementer)
    {
    }

    public MethodBuilder ImplementGetMethod(FieldBuilder backingField, PropertyBuilder property)
    {
        if (property.GetIndexParameters().Length > 0)
            throw new NotImplementedException();
        if (!property.IsStatic())
            throw new NotImplementedException();

        var getMethod = _typeBuilder.DefineMethod(
                $"get_{property.Name}",
                MethodAttributes.Private | MethodAttributes.Static,
                GetCallingConventions(property),
                property.PropertyType,
                Type.EmptyTypes)
            .Emit(emitter => emitter.Ldsfld(backingField).Ret());

        property.SetGetMethod(getMethod);
        return getMethod;
    }
}
