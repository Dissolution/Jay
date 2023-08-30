namespace Jay.SourceGen.Reflection;

public sealed class EventSig : MemberSig,
    IEquatable<EventSig>, IEquatable<IEventSymbol>, IEquatable<EventInfo>
{
    [return: NotNullIfNotNull(nameof(eventInfo))]
    public static implicit operator EventSig?(EventInfo? eventInfo) => EventSig.Create(eventInfo);

    public static bool operator ==(EventSig? left, EventSig? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(EventSig? left, EventSig? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(EventSig? left, IEventSymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(EventSig? left, IEventSymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(EventSig? left, EventInfo? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(EventSig? left, EventInfo? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    [return: NotNullIfNotNull(nameof(eventSymbol))]
    public static EventSig? Create(IEventSymbol? eventSymbol)
    {
        if (eventSymbol is null) return null;
        return new EventSig(eventSymbol);
    }
    [return: NotNullIfNotNull(nameof(eventInfo))]
    public static EventSig? Create(EventInfo? eventInfo)
    {
        if (eventInfo is null) return null;
        return new EventSig(eventInfo);
    }

    public TypeSig? EventType { get; set; } = null;
    public MethodSig? HandlerSig {get;set;} = null;

    public MethodSig? AddMethod { get; set; } = null;
    public MethodSig? RemoveMethod { get; set; } = null;
    public MethodSig? RaiseMethod { get; set; } = null;

    public EventSig()
        : base(SigType.Event)
    {

    }

    public EventSig(IEventSymbol eventSymbol)
        : base(SigType.Event, eventSymbol)
    {
        this.EventType = TypeSig.Create(eventSymbol.Type);
        
        var invokeSymbol = eventSymbol
            .Type
            .GetMembers("Invoke")
            .OfType<IMethodSymbol>()
            .OneOrDefault();
        this.HandlerSig = MethodSig.Create(invokeSymbol);


        this.AddMethod = MethodSig.Create(eventSymbol.AddMethod);
        this.RemoveMethod = MethodSig.Create(eventSymbol.RemoveMethod);
        this.RaiseMethod = MethodSig.Create(eventSymbol.RaiseMethod);
    }

    public EventSig(EventInfo eventInfo)
        : base(SigType.Event, eventInfo)
    {
        this.EventType = TypeSig.Create(eventInfo.EventHandlerType);

        var invokeMethod = eventInfo
            .EventHandlerType
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
            .Where(method => method.Name == "Invoke")
            .OneOrDefault();

        this.AddMethod = MethodSig.Create(eventInfo.AddMethod);
        this.RemoveMethod = MethodSig.Create(eventInfo.RemoveMethod);
        this.RaiseMethod = MethodSig.Create(eventInfo.RaiseMethod);
    }

    public bool Equals(EventSig? eventSig)
    {
        return eventSig is not null &&
            this.Name == eventSig.Name &&
            this.EventType == eventSig.EventType;
    }

    public bool Equals(IEventSymbol? eventSymbol)
    {
        return eventSymbol is not null &&
            this.Name == eventSymbol.Name &&
            this.EventType == eventSymbol.Type;
    }

    public bool Equals(EventInfo? eventInfo)
    {
        return eventInfo is not null &&
            this.Name == eventInfo.Name &&
            this.EventType == eventInfo.EventHandlerType;
    }

    public override bool Equals(Sig? signature)
    {
        return signature is EventSig eventSig && Equals(eventSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IEventSymbol eventSymbol && Equals(eventSymbol);
    }

    public override bool Equals(MemberInfo? memberInfo)
    {
        return memberInfo is EventInfo eventInfo && Equals(eventInfo);
    }

    public override bool Equals(MemberSig? memberSig)
    {
        return memberSig is EventSig eventSig && Equals(eventSig);
    }

    public override bool Equals(object? obj)
    {
        if (obj is EventSig eventSig) return Equals(eventSig);
        if (obj is IEventSymbol eventSymbol) return Equals(eventSymbol);
        if (obj is EventInfo eventInfo) return Equals(eventInfo);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(SigType.Event, Name, EventType);
    }


    public override string ToString()
    {
        return CodeBuilder.New
            .Append(this.Visibility, "lc")
            .AppendIf(this.Instic == Instic.Static, " static ", " ")
            .AppendKeywords(this.Keywords)
            .Append("event ")
            .Append(this.EventType).Append(' ').Append(this.Name)
            .ToStringAndDispose();
    }
}
