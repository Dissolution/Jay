using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Dumping;

namespace Jay.Reflection.Building.Fulfilling;

public abstract class PropertyFulfiller
{
    protected readonly TypeBuilder _typeBuilder;
    protected readonly List<PropertyFulfiller> _fulfillers;

    protected PropertyFulfiller(TypeBuilder typeBuilder)
    {
        _typeBuilder = typeBuilder;
        _fulfillers = new List<PropertyFulfiller>(0);
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
        
        var field = _typeBuilder.DefineField()
        
        var prop = _typeBuilder.DefineProperty(name: property.Name,
                                               attributes: property.Attributes,
                                               callingConvention: CallingConventions.HasThis,
                                               returnType: property.PropertyType,
                                               returnTypeRequiredCustomModifiers: null,
                                               returnTypeOptionalCustomModifiers: null,
                                               parameterTypes: parameterTypes,
                                               parameterTypeRequiredCustomModifiers: null,
                                               parameterTypeOptionalCustomModifiers: null);
    }

}

public class Fulfiller
{
    protected readonly HashSet<Type> _interfaceTypes;
    protected readonly IPropertyFulfiller _propertyFulfiller;

    public Fulfiller(params Type[] interfaceTypes)
    {
        _interfaceTypes = new HashSet<Type>(interfaceTypes.Length);
        foreach (var interfaceType in interfaceTypes)
        {
            if (interfaceType == typeof(INotifyPropertyChanged))
            {
                _propertyFulfiller
            }
        }
    }

    public Type GenerateImplementationType()
    {
        throw new NotImplementedException();
    }

    public static TInterface CreateImplementation<TInterface>()
        where TInterface : class  // closest constraint to : interface
    {
        var interfaceType = typeof(TInterface);
        if (!interfaceType.IsInterface)
            throw Dump.GetException<ArgumentException>($"{typeof(TInterface)} is not an interface", nameof(TInterface));
        var interfaces = typeof(TInterface).GetInterfaces();
        var interfaceTypes = new Type[interfaces.Length + 1];
        interfaceTypes[0] = interfaceType;
        interfaces.CopyTo(interfaceTypes, 1);
        var implementationType = GenerateImplementationType(interfaceTypes);
        var instance =  Activator.CreateInstance(implementationType) as TInterface;
        if (instance is null)
            throw new InvalidOperationException("Unable to create an implementation");
        return instance;
    }

    public object CreateImplementation<T1, T2>() => throw new NotImplementedException();
    public object CreateImplementation<T1, T2, T3>() => throw new NotImplementedException();
    public object CreateImplementation<T1, T2, T3, T4>() => throw new NotImplementedException();
}