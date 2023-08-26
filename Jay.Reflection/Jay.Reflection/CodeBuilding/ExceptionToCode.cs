using System.Collections;

namespace Jay.Reflection.CodeBuilding;

public static class ExceptionToCode
{
    public static void WriteCodeTo(this Exception exception, CodeBuilder code)
    {
        code.Append("Type: ").AppendLine(exception.GetType())
            .Append("Message: ").AppendLine(exception.Message)
            .If(
                exception.HResult != 0,
                cb => cb.AppendLine($"HResult: 0x{exception.HResult:X8} ({exception.HResult})"))
            .If(
                exception.HelpLink.IsNonWhiteSpace,
                cb =>
                {
                    cb.Append("HelpLink: ").AppendLine(exception.HelpLink);
                    //todo add help based on errorcode
                })
            .If(
                exception.Source.IsNonWhiteSpace,
                cb => cb.Append("Source: ").AppendLine(exception.Source))
            .Append("TargetSite: ").AppendLine(exception.TargetSite)
            .AppendLine("StackTrace:")
            .AppendLine("-----------")
            .AppendLine(exception.StackTrace)
            .AppendLine("-----------")
            .If(
                exception.Data.Count > 0,
                cb => cb.Append("Data:")
                    .Delimit(
                        d => d.NewLine().Append("  "),
                        exception.Data.Cast<DictionaryEntry>(),
                        (b, entry) => b.Append(entry.Key).Append(": ").Append(entry.Value)))
            .If(
                exception.InnerException is not null,
                cb => cb.Append("InnerException:")
                    .Indented("    ", i => i.NewLine().Append(exception.InnerException))
                    .NewLine())
            .If(
                exception is AggregateException,
                cb => cb.AppendLine("InnerExceptions:")
                    .Indented(
                        "    ",
                        i => i
                            .NewLine()
                            .EnumerateAppendLines((exception as AggregateException)!.InnerExceptions)))
            ;
    }
}