// namespace Jay.SourceGen.Reflection;
//
// public sealed class EventSig : MemberSig,
//     IEquatable<EventSig>, IEquatable<IEventSymbol>, IEquatable<EventInfo>
// {
//     [return: NotNullIfNotNull(nameof(eventInfo))]
//     public static implicit operator EventSig?(EventInfo? eventInfo) => Create(eventInfo);
//
//     public static bool operator ==(EventSig? left, EventSig? right) => FastEqual(left, right);
//     public static bool operator !=(EventSig? left, EventSig? right) => !FastEqual(left, right);
//     public static bool operator ==(EventSig? left, IEventSymbol? right) => FastEquality(left, right);
//     public static bool operator !=(EventSig? left, IEventSymbol? right) => !FastEquality(left, right);
//     public static bool operator ==(EventSig? left, EventInfo? right) => FastEquality(left, right);
//     public static bool operator !=(EventSig? left, EventInfo? right) => !FastEquality(left, right);
//
//     [return: NotNullIfNotNull(nameof(eventSymbol))]
//     public static EventSig? Create(IEventSymbol? eventSymbol)
//     {
//         if (eventSymbol is null)
//             return null;
//
//         return new EventSig(eventSymbol);
//     }
//
//     [return: NotNullIfNotNull(nameof(eventInfo))]
//     public static EventSig? Create(EventInfo? eventInfo)
//     {
//         if (eventInfo is null)
//             return null;
//
//         return new EventSig(eventInfo);
//     }
//
//     public TypeSig? EventType { get; }
//     public MethodSig? HandlerSig { get; }
//
//     public MethodSig? AddMethod { get; }
//     public MethodSig? RemoveMethod { get; }
//     public MethodSig? RaiseMethod { get; }
//
//     public EventSig(IEventSymbol eventSymbol)
//         : base(eventSymbol)
//     {
//         this.EventType = TypeSig.Create(eventSymbol.Type);
//
//         var invokeSymbol = eventSymbol
//             .Type
//             .GetMembers("Invoke")
//             .OfType<IMethodSymbol>()
//             .OneOrDefault();
//         this.HandlerSig = MethodSig.Create(invokeSymbol);
//         this.AddMethod = MethodSig.Create(eventSymbol.AddMethod);
//         this.RemoveMethod = MethodSig.Create(eventSymbol.RemoveMethod);
//         this.RaiseMethod = MethodSig.Create(eventSymbol.RaiseMethod);
//     }
//
//     public EventSig(EventInfo eventInfo)
//         : base(eventInfo)
//     {
//         this.EventType = TypeSig.Create(eventInfo.EventHandlerType);
//
//         var invokeMethod = eventInfo
//             .EventHandlerType
//             .GetInvokeMethod();
//         this.HandlerSig = MethodSig.Create(invokeMethod);
//         this.AddMethod = MethodSig.Create(eventInfo.AddMethod);
//         this.RemoveMethod = MethodSig.Create(eventInfo.RemoveMethod);
//         this.RaiseMethod = MethodSig.Create(eventInfo.RaiseMethod);
//     }
//
//     public bool Equals(EventSig? eventSig)
//     {
//         return base.Equals(eventSig)
//             && FastEqual(EventType, eventSig.EventType)
//             && FastEqual(HandlerSig, eventSig.HandlerSig)
//             && FastEqual(AddMethod, eventSig.AddMethod)
//             && FastEqual(RemoveMethod, eventSig.RemoveMethod)
//             && FastEqual(RaiseMethod, eventSig.RaiseMethod);
//     }
//
//     public override bool Equals(MemberSig? memberSig)
//     {
//         return memberSig is EventSig eventSig && Equals(eventSig);
//     }
//
//     public override bool Equals(Sig? signature)
//     {
//         return signature is EventSig eventSig && Equals(eventSig);
//     }
//
//     public bool Equals(IEventSymbol? eventSymbol) => Equals(Create(eventSymbol));
//
//     public bool Equals(EventInfo? eventInfo) => Equals(Create(eventInfo));
//
//     public override bool Equals(ISymbol? symbol) => Equals(Create(symbol));
//
//     public override bool Equals(MemberInfo? memberInfo) => Equals(Create(memberInfo));
//
//     public override bool Equals(object? obj)
//     {
//         return obj switch
//         {
//             EventSig eventSig => Equals(eventSig),
//             IEventSymbol eventSymbol => Equals(eventSymbol),
//             EventInfo eventInfo => Equals(eventInfo),
//             _ => false
//         };
//     }
//
//     public override int GetHashCode()
//     {
//         return Hasher.Combine(base.GetHashCode(), EventType, HandlerSig, AddMethod, RemoveMethod, RaiseMethod);
//     }
//
//     public override string ToString()
//     {
//         return TextBuilder.New
//             .Append(Attributes)
//             .Append(Visibility)
//             .Append(' ')
//             .Append(Keywords)
//             .Append(' ')
//             .Append(EventType)
//             .Append(' ')
//             .Append(Name)
//             .Append(';')
//             .ToStringAndDispose();
//     }
// }