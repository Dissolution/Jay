using System.Reflection;
using System.Reflection.Emit;
using Jay.Reflection.Extensions;

namespace Jay.Reflection.Building.Backing;

public interface IFieldImplementer
{
    FieldBuilder ImplementField(FieldInfo field);
}

public interface IBackingFieldImplementer
{
    FieldBuilder ImplementBackingField(PropertyInfo property);
}

internal class BackingFieldImplementer : Implementer, IBackingFieldImplementer
{
    public BackingFieldImplementer(TypeBuilder typeBuilder, 
        IAttributeImplementer attributeImplementer) 
        : base(typeBuilder, attributeImplementer)
    {
    }

    public FieldBuilder ImplementBackingField(PropertyInfo property)
    {
        string fieldName = MemberNaming.CreateBackingFieldName(property);
        FieldAttributes fieldAttributes = FieldAttributes.Private;
        
        if (!property.CanWrite && property.GetSetter() is null)
        {
            fieldAttributes |= FieldAttributes.InitOnly;
        }

        if (property.IsStatic())
        {
            fieldAttributes |= FieldAttributes.Static;
        }

        return _typeBuilder.DefineField(
            fieldName,
            property.PropertyType,
            fieldAttributes);
    }
}

public interface IPropertyImplementer
{
    PropertyImpl ImplementProperty(PropertyInfo property);
}

public interface IMethodImplementer
{
    MethodBuilder ImplementMethod(MethodInfo method);
}

public interface ISetMethodImplementer
{
    MethodBuilder? ImplementSetMethod(FieldBuilder backingField, PropertyBuilder property);
}

public interface IConstructorImplementer
{
    ConstructorBuilder ImplementConstructor(ConstructorInfo ctor);
}

public interface IInstanceRefCtorImplementer
{
    ConstructorImpl ImplementInstanceReferenceConstructor(ConstructorInfo ctor);
}

public sealed record class ConstructorImpl(FieldBuilder InstanceField, ConstructorBuilder Constructor);

public abstract class Implementer
{
    protected static CallingConventions GetCallingConventions(MemberInfo member)
    {
        if (member.IsStatic()) return CallingConventions.Standard;
        return CallingConventions.HasThis;
    }

    protected readonly TypeBuilder _typeBuilder;
    protected readonly IAttributeImplementer _attributeImplementer;

    protected Implementer(
        TypeBuilder typeBuilder,
        IAttributeImplementer attributeImplementer)
    {
        _typeBuilder = typeBuilder;
        _attributeImplementer = attributeImplementer;
    }
}

public class PropertyImplementer : Implementer, IPropertyImplementer
{
    protected readonly IBackingFieldImplementer _backingFieldImplementer;
    protected readonly IPropertyGetMethodImplementer _getMethodImplementer;
    protected readonly ISetMethodImplementer _setMethodImplementer;

    protected PropertyImplementer(
        TypeBuilder typeBuilder,
        IAttributeImplementer attributeImplementer,
        IBackingFieldImplementer backingFieldImplementer, 
        IPropertyGetMethodImplementer getMethodImplementer, 
        ISetMethodImplementer setMethodImplementer)
        : base(typeBuilder, attributeImplementer)
    {
        _backingFieldImplementer = backingFieldImplementer;
        _getMethodImplementer = getMethodImplementer;
        _setMethodImplementer = setMethodImplementer;
    }

    public virtual PropertyImpl ImplementProperty(PropertyInfo property)
    {
        var fieldBuilder = _backingFieldImplementer.ImplementBackingField(property);
        var parameterTypes = property.GetIndexParameterTypes();
        var propertyBuilder = _typeBuilder.DefineProperty(
            property.Name,
            PropertyAttributes.None,
            GetCallingConventions(property),
            property.PropertyType,
            parameterTypes);
        _attributeImplementer.ImplementAttributes(property, propertyBuilder.SetCustomAttribute);
        var getMethodBuilder = _getMethodImplementer.ImplementGetMethod(fieldBuilder, propertyBuilder);
        var setMethodBuilder = _setMethodImplementer.ImplementSetMethod(fieldBuilder, propertyBuilder);
        return new PropertyImpl
        (
            fieldBuilder,
            getMethodBuilder,
            setMethodBuilder,
            propertyBuilder
        );
    }
}