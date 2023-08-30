namespace IMPL.SourceGen.Writers;

public sealed class SimpleEventSigWriter : IEventSigWriter
{
    public static IEventSigWriter Instance { get; } = new SimpleEventSigWriter();

    public void Write(EventSig eventSig, CodeBuilder codeBuilder)
    {
        codeBuilder
            .Append(eventSig.Visibility, "lc")
            .AppendIf(eventSig.Instic == Instic.Instance, " ", " static ")
            .AppendKeywords(eventSig.Keywords)
            .Append("event ")
            .Append(eventSig.EventType)
            .Append("? ")
            .Append(eventSig.Name)
            .AppendLine(';');
    }
}
