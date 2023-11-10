namespace Jay.SourceGen.Reflection;

public record class EventSignature : MemberSignature
{
    public static EventSignature? Create(IEventSymbol? eventSymbol)
    {
        if (eventSymbol is null) return null;
        var sig = new EventSignature()
        {
            Name = eventSymbol.Name,
            Visibility = eventSymbol.GetVisibility(),
            Keywords = eventSymbol.GetKeywords(),
            Attributes = Attributes.From(eventSymbol),
            BaseType = TypeSignature.Create(eventSymbol.ContainingType),
            HandlerType = TypeSignature.Create(eventSymbol.Type),
            Adder = MethodSignature.Create(eventSymbol.AddMethod),
            Remover = MethodSignature.Create(eventSymbol.RemoveMethod),
            Raiser = MethodSignature.Create(eventSymbol.RaiseMethod),
        };
        return sig;
    }
    
    public static EventSignature? Create(EventInfo? eventInfo)
    {
        if (eventInfo is null) return null;
        var sig = new EventSignature()
        {
            Name = eventInfo.Name,
            Visibility = eventInfo.GetVisibility(),
            Keywords = eventInfo.GetKeywords(),
            Attributes = Attributes.From(eventInfo),
            BaseType = TypeSignature.Create(eventInfo.DeclaringType),
            HandlerType = TypeSignature.Create(eventInfo.EventHandlerType),
            Adder = MethodSignature.Create(eventInfo.AddMethod),
            Remover = MethodSignature.Create(eventInfo.RemoveMethod),
            Raiser = MethodSignature.Create(eventInfo.RaiseMethod),
        };
        return sig;
    }

    
    
    public TypeSignature? HandlerType { get; set; } = null;
    public MethodSignature? Adder { get; set; } = null;
    public MethodSignature? Remover { get; set; } = null;
    public MethodSignature? Raiser { get; set; } = null;
}