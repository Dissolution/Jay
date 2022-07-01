using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Building.Backing;

public interface IAttributeImplementer
{
    void ImplementAttributes(MemberInfo copyAttributesFrom,
        Action<CustomAttributeBuilder> addAttribute);
}

internal class AttributeImplementer : Implementer, IAttributeImplementer
{
    public AttributeImplementer(TypeBuilder typeBuilder, IAttributeImplementer attributeImplementer) 
        : base(typeBuilder, attributeImplementer)
    {
    }

    public void ImplementAttributes(MemberInfo copyAttributesFrom, 
        Action<CustomAttributeBuilder> addAttribute)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(copyAttributesFrom, true);
        if (attributes.Length == 0) return;
        foreach (var attribute in attributes)
        {
            Debugger.Break();
            var cab = new CustomAttributeBuilder(
                con: (ConstructorInfo)null!,
                constructorArgs: (object?[])null!,
                namedProperties: null!,
                propertyValues: null!,
                namedFields: null!,
                fieldValues: null!);
            addAttribute(cab);
        }
    }
}