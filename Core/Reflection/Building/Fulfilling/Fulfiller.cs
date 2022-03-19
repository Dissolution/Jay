using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Dumping;
using Jay.Text;

namespace Jay.Reflection.Building.Fulfilling;

public abstract class PropertyFulfiller
{
    private static string GetFieldName(string propertyName)
    {
        return string.Create(propertyName.Length + 1, propertyName, (span, name) =>
        {
            span[0] = '_';
            span[1] = char.ToLower(name[0]);
            TextHelper.CopyTo(name.AsSpan(1), span[2..]);
        });
    }

    private static string GetPropertyGetterName(PropertyInfo property)
    {
        return $"get_{property.Name}";
    }
    
    private static string GetPropertySetterName(PropertyInfo property)
    {
        return $"set_{property.Name}";
    }
    
    protected readonly TypeBuilder _typeBuilder;
    protected readonly List<PropertyFulfiller> _fulfillers;

    protected PropertyFulfiller(TypeBuilder typeBuilder)
    {
        _typeBuilder = typeBuilder;
        _fulfillers = new List<PropertyFulfiller>(0);
    }

    protected void EmitPropertySetter(MethodBuilder setter)
    {
        var emitter = setter.GetEmitter();
        
    }
    
    public void Add(PropertyFulfiller propertyFulfiller)
    {
        _fulfillers.Add(propertyFulfiller);
    }

    public (FieldBuilder BackingField, PropertyBuilder Property) Fulfill(PropertyInfo property)
    {
        Type[]? parameterTypes;
        var indexParameters = property.GetIndexParameters();
        if (indexParameters.Length > 0)
        {
            Debugger.Break();
            parameterTypes = new Type[indexParameters.Length];
            for (var i = 0; i < indexParameters.Length; i++)
            {
                parameterTypes[i] = indexParameters[i].ParameterType;
            }
        }
        else
        {
            parameterTypes = null;
        }

        var field = _typeBuilder.DefineField(fieldName: GetFieldName(property.Name),
                                             type: property.PropertyType,
                                             FieldAttributes.Private);
        
        var prop = _typeBuilder.DefineProperty(name: property.Name,
                                               attributes: property.Attributes,
                                               callingConvention: CallingConventions.HasThis,
                                               returnType: property.PropertyType,
                                               returnTypeRequiredCustomModifiers: null,
                                               returnTypeOptionalCustomModifiers: null,
                                               parameterTypes: parameterTypes,
                                               parameterTypeRequiredCustomModifiers: null,
                                               parameterTypeOptionalCustomModifiers: null);
        var propSetter = _typeBuilder.DefineMethod(name: GetPropertySetterName(property),
                                                   attributes: MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.SpecialName,
                                                   callingConvention: CallingConventions.HasThis,
                                                   returnType: property.PropertyType,
                                                   parameterTypes: Array.Empty<Type>());
        EmitPropertySetter(propSetter);
    }
    
    

}

