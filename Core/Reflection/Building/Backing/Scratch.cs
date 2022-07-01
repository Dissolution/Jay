using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Building.Backing;



public interface IEqualsBacker
{
    (MethodInfo EqualMethod, MethodInfo GetHashCodeMethod) CreateEqualityMethods(
        IReadOnlyDictionary<PropertyInfo, (FieldInfo, PropertyInfo)> implementedProperties);
}

public interface IToStringBacker
{
    MethodInfo CreateToStringMethod(
        IReadOnlyDictionary<PropertyInfo, (FieldInfo, PropertyInfo)> implementedProperties);
}

public class InterfaceImplementer
{
    public static Type CreateImplementationType<TInterface>() where TInterface : class
    {
        return new InterfaceImplementer(typeof(TInterface)).CreateImplementingType();
    }

    protected HashSet<Type>? _interfaces;
    protected TypeBuilder? _typeBuilder;
    protected IAttributeImplementer? _attributeImplementer;

    protected readonly Dictionary<string, FieldBuilder> _builtFields = new(StringComparer.OrdinalIgnoreCase);
    protected readonly Dictionary<string, PropertyBuilder> _builtProperties = new(StringComparer.OrdinalIgnoreCase);
    protected readonly Dictionary<string, EventBuilder> _builtEvents = new(StringComparer.OrdinalIgnoreCase);
    protected readonly Dictionary<string, ConstructorBuilder> _builtConstructors = new(StringComparer.OrdinalIgnoreCase);
    protected readonly Dictionary<string, MethodBuilder> _builtMethods = new(StringComparer.OrdinalIgnoreCase);

    public Type InterfaceType { get; }

    public InterfaceImplementer(Type interfaceType)
    {
        ArgumentNullException.ThrowIfNull(interfaceType);
        if (!interfaceType.IsInterface)
            throw new ArgumentException("InterfaceBacker must be passed an interface type", nameof(interfaceType));
        this.InterfaceType = interfaceType;
    }

    private void ImplementProperties()
    {
        IBackingFieldImplementer backingFieldImplementer = new BackingFieldImplementer(_typeBuilder!, _attributeImplementer!);
        IPropertyGetMethodImplementer getMethodImplementer = new DefaultInstancePropertyGetMethodImplementer(_typeBuilder!, _attributeImplementer!);
        IPropertySetMethodImplementer setMethodImplementer;
        var notifyPropertyChanging = _interfaces!.Contains(typeof(INotifyPropertyChanging));
        var notifyPropertyChanged = _interfaces!.Contains(typeof(INotifyPropertyChanged));
        if (notifyPropertyChanging || notifyPropertyChanged)
        {
            setMethodImplementer = new NotifyPropertySetMethodImplementer(_typeBuilder!,
                _attributeImplementer!,
                null,
                null);
            throw new NotImplementedException();
        }
        else
        {
            setMethodImplementer = new DefaultInstancePropertySetMethodImplementer(_typeBuilder!, _attributeImplementer!);
        }

        IPropertyImplementer propertyImplementer = new PropertyImplementer(_typeBuilder!,
            _attributeImplementer!,
            backingFieldImplementer,
            getMethodImplementer,
            setMethodImplementer);
        
        var properties = _interfaces.SelectMany(i => i.GetProperties(BindingFlags.Public | BindingFlags.Instance));
        foreach (var property in properties)
        {
            if (_builtProperties.ContainsKey(property.Name)) continue;
            var pack = propertyImplementer.ImplementProperty(property);
            _builtFields.Add(pack.BackingField.Name, pack.BackingField);
            if (pack.GetMethod is not null)
            {
                _builtMethods.Add(pack.GetMethod.Name, pack.GetMethod);
            }
            if (pack.SetMethod is not null)
            {
                _builtMethods.Add(pack.SetMethod.Name, pack.SetMethod);
            }
            _builtProperties.Add(pack.Property.Name, pack.Property);
        }
    }
    
    public Type CreateImplementingType()
    {
        // What all do we need to implement?
        _interfaces = new HashSet<Type>(InterfaceType.GetInterfaces())
        {
            InterfaceType
        };
        
        Debugger.Break();

        _typeBuilder = RuntimeBuilder.DefineType(
            TypeAttributes.Public | TypeAttributes.Class,
            $"{InterfaceType.Name}_Impl");
        _attributeImplementer = 
        
        
        // Properties
        ImplementProperties();
        

        IPropertyBacker propertyBacker;

        if (interfaces.Contains(typeof(INotifyCollectionChanged)) ||
            interfaces.Contains(typeof(INotifyPropertyChanging)))
        {
            propertyBacker = new NotifyPropertyBacker();
        }
        else
        {
            propertyBacker = new DefaultInstancePropertyBacker();
        }

        IEqualsBacker equalsBacker;
        IToStringBacker toStringBacker;
        
        if (interfaces.Contains(typeof(IEquatable<>)))
        {
            equalsBacker = new PropertyEqualsBacker();
            toStringBacker = new PropertyToStringBacker();
        }
        else
        {
            equalsBacker = new ReferenceEqualsBacker();
            toStringBacker = new DefaultToStringBacker();
        }

        var properties = InterfaceType.GetProperties(
            BindingFlags.Public | BindingFlags.Instance);
        Debugger.Break();



        throw new NotImplementedException();
    }
}