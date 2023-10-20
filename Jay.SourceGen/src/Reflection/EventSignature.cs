namespace Jay.SourceGen.Reflection;

public sealed class EventSignature : MemberSignature,
    IEquatable<EventSignature>, IEquatable<IEventSymbol>, IEquatable<EventInfo>
{
    [return: NotNullIfNotNull(nameof(eventInfo))]
    public static implicit operator EventSignature?(EventInfo? eventInfo) => Create(eventInfo);

    public static bool operator ==(EventSignature? left, EventSignature? right) => FastEqual(left, right);
    public static bool operator !=(EventSignature? left, EventSignature? right) => !FastEqual(left, right);
    public static bool operator ==(EventSignature? left, IEventSymbol? right) => FastEquality(left, right);
    public static bool operator !=(EventSignature? left, IEventSymbol? right) => !FastEquality(left, right);
    public static bool operator ==(EventSignature? left, EventInfo? right) => FastEquality(left, right);
    public static bool operator !=(EventSignature? left, EventInfo? right) => !FastEquality(left, right);

    [return: NotNullIfNotNull(nameof(eventSymbol))]
    public static EventSignature? Create(IEventSymbol? eventSymbol)
    {
        if (eventSymbol is null)
            return null;

        return new EventSignature(eventSymbol);
    }

    [return: NotNullIfNotNull(nameof(eventInfo))]
    public static EventSignature? Create(EventInfo? eventInfo)
    {
        if (eventInfo is null)
            return null;

        return new EventSignature(eventInfo);
    }

    public TypeSignature? EventType { get; }
    public MethodSignature? HandlerSig { get; }

    public MethodSignature? AddMethod { get; }
    public MethodSignature? RemoveMethod { get; }
    public MethodSignature? RaiseMethod { get; }

    public EventSignature(IEventSymbol eventSymbol)
        : base(eventSymbol)
    {
        this.EventType = TypeSignature.Create(eventSymbol.Type);

        var invokeSymbol = eventSymbol
            .Type
            .GetMembers("Invoke")
            .OfType<IMethodSymbol>()
            .OneOrDefault();
        this.HandlerSig = MethodSignature.Create(invokeSymbol);
        this.AddMethod = MethodSignature.Create(eventSymbol.AddMethod);
        this.RemoveMethod = MethodSignature.Create(eventSymbol.RemoveMethod);
        this.RaiseMethod = MethodSignature.Create(eventSymbol.RaiseMethod);
    }

    public EventSignature(EventInfo eventInfo)
        : base(eventInfo)
    {
        this.EventType = TypeSignature.Create(eventInfo.EventHandlerType);

        var invokeMethod = eventInfo
            .EventHandlerType
            .GetInvokeMethod();
        this.HandlerSig = MethodSignature.Create(invokeMethod);
        this.AddMethod = MethodSignature.Create(eventInfo.AddMethod);
        this.RemoveMethod = MethodSignature.Create(eventInfo.RemoveMethod);
        this.RaiseMethod = MethodSignature.Create(eventInfo.RaiseMethod);
    }

    public bool Equals(EventSignature? eventSig)
    {
        return base.Equals(eventSig)
            && FastEqual(EventType, eventSig.EventType)
            && FastEqual(HandlerSig, eventSig.HandlerSig)
            && FastEqual(AddMethod, eventSig.AddMethod)
            && FastEqual(RemoveMethod, eventSig.RemoveMethod)
            && FastEqual(RaiseMethod, eventSig.RaiseMethod);
    }

    public override bool Equals(MemberSignature? memberSig)
    {
        return memberSig is EventSignature eventSig && Equals(eventSig);
    }

    public override bool Equals(Signature? signature)
    {
        return signature is EventSignature eventSig && Equals(eventSig);
    }

    public bool Equals(IEventSymbol? eventSymbol) => Equals(Create(eventSymbol));

    public bool Equals(EventInfo? eventInfo) => Equals(Create(eventInfo));

    public override bool Equals(ISymbol? symbol) => Equals(Create(symbol));

    public override bool Equals(MemberInfo? memberInfo) => Equals(Create(memberInfo));

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            EventSignature eventSig => Equals(eventSig),
            IEventSymbol eventSymbol => Equals(eventSymbol),
            EventInfo eventInfo => Equals(eventInfo),
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(base.GetHashCode(), EventType, HandlerSig, AddMethod, RemoveMethod, RaiseMethod);
    }

    public override void DeclareTo(CodeBuilder code)
    {
        code.Append(Attributes)
            .Append(Visibility)
            .Append(' ')
            .Append(Keywords)
            .Append(' ')
            .Append(EventType)
            .Append(' ')
            .Append(Name)
            .Append(';');
    }
}