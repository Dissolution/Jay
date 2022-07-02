using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Debugging;
using Jay.Dumping;

namespace Jay.Reflection.Building.Backing;

public class PropertyImplementer : Implementer, IPropertyImplementer
{
    protected readonly IBackingFieldImplementer _backingFieldImplementer;
    protected readonly IPropertyGetMethodImplementer _getMethodImplementer;
    protected readonly IPropertySetMethodImplementer _propertySetMethodImplementer;

    public PropertyImplementer(TypeBuilder typeBuilder,
        IBackingFieldImplementer backingFieldImplementer, 
        IPropertyGetMethodImplementer getMethodImplementer, 
        IPropertySetMethodImplementer propertySetMethodImplementer)
        : base(typeBuilder)
    {
        _backingFieldImplementer = backingFieldImplementer;
        _getMethodImplementer = getMethodImplementer;
        _propertySetMethodImplementer = propertySetMethodImplementer;
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
        AttributeImplementer.ImplementAttributes(property, propertyBuilder.SetCustomAttribute);
        var tuple = (property.CanRead, property.GetGetter(), property.CanWrite, property.GetSetter());
        var dump = Dumper.Dump(tuple);
        Hold.Debug(dump);
        Debugger.Break();
        
        
        // TODO: Do not implement if property isn't getter/setter?
        var getMethodBuilder = _getMethodImplementer.ImplementGetMethod(fieldBuilder, propertyBuilder);
        var setMethodBuilder = _propertySetMethodImplementer.ImplementSetMethod(fieldBuilder, propertyBuilder);
        return new PropertyImpl
        (
            fieldBuilder,
            getMethodBuilder,
            setMethodBuilder,
            propertyBuilder
        );
    }
}