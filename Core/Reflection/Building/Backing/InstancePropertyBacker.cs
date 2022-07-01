using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Reflection.Extensions;

namespace Jay.Reflection.Building.Backing;

public abstract class InstancePropertyBacker : PropertyBacker
{
    protected InstancePropertyBacker(TypeBuilder typeBuilder) 
        : base(typeBuilder)
    {
        if (typeBuilder.IsStatic())
            throw new ArgumentException("Must pass an instance TypeBuilder", nameof(typeBuilder));
    }

    protected override FieldBuilder DefineField(PropertyInfo property)
    {
        return _typeBuilder.DefineField(
            MemberNaming.CreateBackingFieldName(property.Name),
            property.PropertyType,
            FieldAttributes.Private);
    }

    protected override PropertyImpl DefineProperty(PropertyInfo property, FieldBuilder backingField)
    {
        MethodBuilder? getMethod = null;
        MethodBuilder? setMethod = null;
        var parameterTypes = property.GetIndexParameterTypes();
        var propertyBuilder = _typeBuilder.DefineProperty(
            property.Name,
            PropertyAttributes.None,
            CallingConventions.HasThis,
            property.PropertyType,
            parameterTypes);
        var attrs = Attribute.GetCustomAttributes(property);
        if (attrs.Length > 0)
        {
            Debugger.Break();
            // foreach (var attr in attrs)
            // {
            //     var cab = new CustomAttributeBuilder();
            // }
        }
        // Getter?
        if (property.CanRead || property.GetGetter() is not null)
        {
            getMethod = _typeBuilder.DefineMethod(
                $"get_{property.Name}",
                MethodAttributes.Private | MethodAttributes.Final,
                CallingConventions.HasThis,
                property.PropertyType,
                parameterTypes);
            getMethod.Emit(emitter =>
            {
                emitter
            });
            propertyBuilder.SetGetMethod(getMethod);
        }

        if (property.CanWrite || property.GetSetter() is not null)
        {
            Type[] setParams = new Type[parameterTypes.Length + 1];
            setParams[0] = property.PropertyType;
            parameterTypes.CopyTo(setParams.AsSpan(1));
            setMethod = _typeBuilder.DefineMethod(
                $"set_{property.Name}",
                MethodAttributes.Private | MethodAttributes.Final,
                CallingConventions.HasThis,
                typeof(void),
                setParams);
            propertyBuilder.SetSetMethod(setMethod);
        }

        return new PropertyImpl(backingField, getMethod, setMethod, propertyBuilder);
    }
}

public abstract class IndexerPropertyBacker : InstancePropertyBacker
{
    
}