// namespace Jay.SourceGen.Reflection;
//
// public sealed class PropertySig : MemberSig,
//     IEquatable<PropertySig>, IEquatable<IPropertySymbol>, IEquatable<PropertyInfo>
// {
//     [return: NotNullIfNotNull(nameof(propertyInfo))]
//     public static implicit operator PropertySig?(PropertyInfo? propertyInfo) => Create(propertyInfo);
//
//     public static bool operator ==(PropertySig? left, PropertySig? right) => FastEqual(left, right);
//     public static bool operator !=(PropertySig? left, PropertySig? right) => !FastEqual(left, right);
//     public static bool operator ==(PropertySig? left, IPropertySymbol? right) => FastEquality(left, right);
//     public static bool operator !=(PropertySig? left, IPropertySymbol? right) => !FastEquality(left, right);
//     public static bool operator ==(PropertySig? left, PropertyInfo? right) => FastEquality(left, right);
//     public static bool operator !=(PropertySig? left, PropertyInfo? right) => !FastEquality(left, right);
//
//     [return: NotNullIfNotNull(nameof(propertySymbol))]
//     public static PropertySig? Create(IPropertySymbol? propertySymbol)
//     {
//         if (propertySymbol is null)
//             return null;
//
//         return new PropertySig(propertySymbol);
//     }
//
//     [return: NotNullIfNotNull(nameof(propertyInfo))]
//     public static PropertySig? Create(PropertyInfo? propertyInfo)
//     {
//         if (propertyInfo is null)
//             return null;
//
//         return new PropertySig(propertyInfo);
//     }
//
//     public TypeSig? PropertyType { get; init; }
//     public MethodSig? GetMethod { get; init; }
//     public MethodSig? SetMethod { get; init; }
//     public bool IsIndexer { get; init; }
//     public IReadOnlyList<ParameterSig> Parameters { get; init; }
//
//     public bool HasGetter => GetMethod is not null;
//     public bool HasSetter => SetMethod is not null;
//
//     public PropertySig(IPropertySymbol propertySymbol)
//         : base(propertySymbol)
//     {
//         this.PropertyType = new(propertySymbol.Type);
//         this.GetMethod = MethodSig.Create(propertySymbol.GetMethod);
//         this.SetMethod = MethodSig.Create(propertySymbol.SetMethod);
//         this.IsIndexer = propertySymbol.IsIndexer;
//         this.Parameters = propertySymbol.Parameters.Select(static p => new ParameterSig(p))
//             .ToList();
//     }
//
//     public PropertySig(PropertyInfo propertyInfo)
//         : base(propertyInfo)
//     {
//         this.PropertyType = new(propertyInfo.PropertyType);
//         this.GetMethod = MethodSig.Create(propertyInfo.GetMethod);
//         this.SetMethod = MethodSig.Create(propertyInfo.SetMethod);
//         this.Parameters = propertyInfo.GetIndexParameters()
//             .Select(static p => new ParameterSig(p))
//             .ToList();
//         this.IsIndexer = Parameters.Count > 0;
//     }
//
//     public override bool Equals(Sig? signature)
//     {
//         return signature is PropertySig propertySig && Equals(propertySig);
//     }
//
//     public override bool Equals(MemberSig? memberSig)
//     {
//         return memberSig is PropertySig propertySig && Equals(propertySig);
//     }
//
//     public bool Equals(PropertySig? propertySig)
//     {
//         return base.Equals(propertySig) 
//             && FastEqual(PropertyType, propertySig.PropertyType) 
//             && FastEqual(GetMethod, propertySig.GetMethod) 
//             && FastEqual(SetMethod, propertySig.SetMethod);
//     }
//
//     public bool Equals(IPropertySymbol? propertySymbol) => Equals(Create(propertySymbol));
//
//     public bool Equals(PropertyInfo? propertyInfo) => Equals(Create(propertyInfo));
//     
//     public override bool Equals(ISymbol? symbol) => Equals(Create(symbol));
//
//     public override bool Equals(MemberInfo? memberInfo) => Equals(Create(memberInfo));
//
//     public override bool Equals(object? obj)
//     {
//         return obj switch
//         {
//             PropertySig propertySig => Equals(propertySig),
//             IPropertySymbol propertySymbol => Equals(propertySymbol),
//             PropertyInfo propertyInfo => Equals(propertyInfo),
//             _ => false,
//         };
//     }
//
//     public override int GetHashCode()
//     {
//         return Hasher.Combine(base.GetHashCode(), PropertyType, GetMethod, SetMethod);
//     }
//
//     public override string ToString()
//     {
//         using var code = new TextBuilder();
//         code.Append(Attributes)
//             .Append(Visibility)
//             .Append(' ')
//             .Append(Keywords)
//             .Append(' ')
//             .Append(PropertyType)
//             .Append(' ')
//             .Append(Name);
//         if (!HasGetter && !HasSetter)
//         {
//             code.Write(';');
//         }
//         else
//         {
//             code.Write("{ ");
//             var getter = GetMethod;
//             if (getter is not null)
//             {
//                 if (getter.Visibility != Visibility)
//                 {
//                     code.Append(getter.Visibility)
//                         .Append(' ');
//                 }
//                 code.Write("get; ");
//             }
//             var setter = SetMethod;
//             if (setter is not null)
//             {
//                 if (setter.Visibility != Visibility)
//                 {
//                     code.Append(setter.Visibility)
//                         .Append(' ');
//                 }
//                 if (setter.Keywords.HasFlags(Keywords.Init))
//                 {
//                     code.Write("init; ");
//                 }
//                 else
//                 {
//                     code.Write("set; ");
//                 }
//             }
//             code.Write('}');
//         }
//         return code.ToString();
//     }
// }