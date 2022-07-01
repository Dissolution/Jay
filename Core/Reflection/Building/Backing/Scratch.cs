using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Building.Backing;
// public interface IPropertyBacker
// {
//     (FieldBuilder Field, PropertyBuilder Property) CreateProperty(TypeBuilder typeBuilder,
//         PropertyAttributes attributes,
//         Type propertyType,
//         string name);
// }
//
//
// //STATIC PROPERY BUILDERT?
//
// public class PropertyBacker : IPropertyBacker
// {
//     /// <inheritdoc />
//     public (FieldBuilder Field, PropertyBuilder Property) CreateProperty(TypeBuilder typeBuilder, 
//         PropertyAttributes attributes,
//         Type propertyType, 
//         string name)
//     {
//         string propertyName = MemberNaming.CreateMemberName(name);
//         FieldAttributes fieldAttributes;
//         CallingConventions callingConventions;
//
//         if (typeBuilder.IsStatic())
//         {
//             fieldAttributes = FieldAttributes.Private | FieldAttributes.Static;
//             callingConventions = CallingConventions.Standard;
//         }
//         else
//         {
//             fieldAttributes = FieldAttributes.Private;
//             callingConventions = CallingConventions.HasThis;
//         }
//         
//         var fieldBuilder = typeBuilder.DefineField(
//             MemberNaming.FieldName(propertyName),
//             propertyType,
//             fieldAttributes);
//         var propertyBuilder = typeBuilder.DefineProperty(
//             propertyName,
//             attributes,
//             callingConventions,
//             propertyType,
//             null);
//
//         var getMethod = typeBuilder.DefineMethod($"get_{propertyName}",
//             MethodAttributes.Private,
//             callingConventions,
//             propertyType,
//             Type.EmptyTypes);
//         var getEmitter = getMethod.GetEmitter()
//             
//
//         return (fieldBuilder, propertyBuilder);
//     }
// }
//
// public class NotifyPropertyBacker : IPropertyBacker
// {
//     private readonly bool _changed;
//     private readonly bool _changing;
//
//     public NotifyPropertyBacker(bool changed = true, bool changing = false)
//     {
//         _changed = changed;
//         _changing = changing;
//     }
// }

// StaticPropertyBacker



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
        ISetMethodImplementer setMethodImplementer;
        
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