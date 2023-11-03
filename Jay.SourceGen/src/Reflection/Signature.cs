// namespace Jay.SourceGen.Reflection;
//
// public abstract class Sig : IEquatable<Sig>
// {
//     public static implicit operator Sig(FieldInfo fieldInfo) => new FieldSig(fieldInfo);
//     public static implicit operator Sig(PropertyInfo propertyInfo) => new PropertySig(propertyInfo);
//     public static implicit operator Sig(EventInfo eventInfo) => new EventSig(eventInfo);
//     public static implicit operator Sig(ConstructorInfo ctorInfo) => new MethodSig(ctorInfo);
//     public static implicit operator Sig(MethodInfo methodInfo) => new MethodSig(methodInfo);
//     public static implicit operator Sig(ParameterInfo parameterInfo) => new ParameterSig(parameterInfo);
//     public static implicit operator Sig(AttributeData attributeData) => new AttributeSig(attributeData);
//     public static implicit operator Sig(CustomAttributeData customAttrData) => new AttributeSig(customAttrData);
//
//     public static bool operator ==(Sig? left, Sig? right) => FastEqual(left, right);
//     public static bool operator !=(Sig? left, Sig? right) => !FastEqual(left, right);
//     
//     public static bool TryCreate([AllowNull, NotNullWhen(true)] object? obj, [NotNullWhen(true)] out Sig? signature)
//     {
//         switch (obj)
//         {
//             case IFieldSymbol fieldSymbol:
//                 signature = new FieldSig(fieldSymbol);
//                 return true;
//             case FieldInfo fieldInfo:
//                 signature = new FieldSig(fieldInfo);
//                 return true;
//             case IPropertySymbol propertySymbol:
//                 signature = new PropertySig(propertySymbol);
//                 return true;
//             case PropertyInfo propertyInfo:
//                 signature = new PropertySig(propertyInfo);
//                 return true;
//             case IEventSymbol eventSymbol:
//                 signature = new EventSig(eventSymbol);
//                 return true;
//             case EventInfo eventInfo:
//                 signature = new EventSig(eventInfo);
//                 return true;
//             case IMethodSymbol methodSymbol:
//                 signature = new MethodSig(methodSymbol);
//                 return true;
//             case MethodBase methodBase:
//                 signature = new MethodSig(methodBase);
//                 return true;
//             case Type type:
//                 signature = new TypeSig(type);
//                 return true;
//             case IParameterSymbol parameterSymbol:
//                 signature = new ParameterSig(parameterSymbol);
//                 return true;
//             case ParameterInfo parameterInfo:
//                 signature = new ParameterSig(parameterInfo);
//                 return true;
//             case AttributeData attributeData:
//                 signature = new AttributeSig(attributeData);
//                 return true;
//             case CustomAttributeData customAttrData:
//                 signature = new AttributeSig(customAttrData);
//                 return true;
//             default:
//                 signature = default;
//                 return false;
//         }
//     }
//     
//     public string Name { get; }
//     public Visibility Visibility { get; }
//     public Keywords Keywords { get; }
//
//     protected Sig(string name, Visibility visibility, Keywords keywords)
//     {
//         this.Name = name;
//         this.Visibility = visibility;
//         this.Keywords = keywords;
//     }
//
//     public virtual bool Equals(Sig? signature)
//     {
//         return signature is not null 
//             && TypeEqual(this, signature) 
//             && TextEqual(Name, signature.Name) 
//             && FastEqual(Visibility, signature.Visibility) 
//             && FastEqual(Keywords, signature.Keywords);
//     }
//     
//     public override bool Equals(object? obj)
//     {
//         return TryCreate(obj, out var signature) && Equals(signature);
//     }
//
//     /// <summary>
//     /// <b>WARNING</b>: <see cref="Sig"/> and derivatives are mutable!
//     /// </summary>
//     public override int GetHashCode()
//     {
//         return Hasher.Combine(GetType(), Name, Visibility, Keywords);
//     }
// }