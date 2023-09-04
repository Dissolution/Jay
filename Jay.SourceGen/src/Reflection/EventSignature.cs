namespace Jay.SourceGen.Reflection;

public sealed class EventSignature : MemberSignature,
    IEquatable<EventSignature>, IEquatable<IEventSymbol>, IEquatable<EventInfo>
{
    [return: NotNullIfNotNull(nameof(eventInfo))]
    public static implicit operator EventSignature?(EventInfo? eventInfo) => Create(eventInfo);

    public static bool operator ==(EventSignature? left, EventSignature? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(EventSignature? left, EventSignature? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(EventSignature? left, IEventSymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(EventSignature? left, IEventSymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(EventSignature? left, EventInfo? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(EventSignature? left, EventInfo? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    [return: NotNullIfNotNull(nameof(eventSymbol))]
    public static EventSignature? Create(IEventSymbol? eventSymbol)
    {
        if (eventSymbol is null) return null;
        return new EventSignature(eventSymbol);
    }
    [return: NotNullIfNotNull(nameof(eventInfo))]
    public static EventSignature? Create(EventInfo? eventInfo)
    {
        if (eventInfo is null) return null;
        return new EventSignature(eventInfo);
    }

    public TypeSignature? EventType { get; set; } = null;
    public MethodSignature? HandlerSig {get;set;} = null;

    public MethodSignature? AddMethod { get; set; } = null;
    public MethodSignature? RemoveMethod { get; set; } = null;
    public MethodSignature? RaiseMethod { get; set; } = null;

    public EventSignature()
        : base(SigType.Event)
    {

    }

    public EventSignature(IEventSymbol eventSymbol)
        : base(SigType.Event, eventSymbol)
    {
        EventType = TypeSignature.Create(eventSymbol.Type);
        
        var invokeSymbol = eventSymbol
            .Type
            .GetMembers("Invoke")
            .OfType<IMethodSymbol>()
            .OneOrDefault();
        HandlerSig = MethodSignature.Create(invokeSymbol);


        AddMethod = MethodSignature.Create(eventSymbol.AddMethod);
        RemoveMethod = MethodSignature.Create(eventSymbol.RemoveMethod);
        RaiseMethod = MethodSignature.Create(eventSymbol.RaiseMethod);
    }

    public EventSignature(EventInfo eventInfo)
        : base(SigType.Event, eventInfo)
    {
        EventType = TypeSignature.Create(eventInfo.EventHandlerType);

        var invokeMethod = eventInfo
            .EventHandlerType
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
            .Where(method => method.Name == "Invoke")
            .OneOrDefault();

        AddMethod = MethodSignature.Create(eventInfo.AddMethod);
        RemoveMethod = MethodSignature.Create(eventInfo.RemoveMethod);
        RaiseMethod = MethodSignature.Create(eventInfo.RaiseMethod);
    }

    public bool Equals(EventSignature? eventSig)
    {
        return eventSig is not null &&
            Name == eventSig.Name &&
            EventType == eventSig.EventType;
    }

    public bool Equals(IEventSymbol? eventSymbol)
    {
        return eventSymbol is not null &&
            Name == eventSymbol.Name &&
            EventType == eventSymbol.Type;
    }

    public bool Equals(EventInfo? eventInfo)
    {
        return eventInfo is not null &&
            Name == eventInfo.Name &&
            EventType == eventInfo.EventHandlerType;
    }

    public override bool Equals(Signature? signature)
    {
        return signature is EventSignature eventSig && Equals(eventSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IEventSymbol eventSymbol && Equals(eventSymbol);
    }

    public override bool Equals(MemberInfo? memberInfo)
    {
        return memberInfo is EventInfo eventInfo && Equals(eventInfo);
    }

    public override bool Equals(MemberSignature? memberSig)
    {
        return memberSig is EventSignature eventSig && Equals(eventSig);
    }

    public override bool Equals(object? obj)
    {
        if (obj is EventSignature eventSig) return Equals(eventSig);
        if (obj is IEventSymbol eventSymbol) return Equals(eventSymbol);
        if (obj is EventInfo eventInfo) return Equals(eventInfo);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(SigType.Event, Name, EventType);
    }


    public override string ToString()
    {
        return CodeBuilder.New
            .Append(Visibility, "lc")
            .AppendIf(this.Instic == Instic.Static, " static ", " ")
            .AppendKeywords(Keywords)
            .Append("event ")
            .Append(EventType).Append(' ').Append(Name)
            .ToStringAndDispose();
    }
}
