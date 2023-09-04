// using System.Collections;
//
// namespace Jay.Reflection.CodeBuilding;
//
// public static class ExceptionToCode
// {
//     public static void WriteCodeTo(this Exception exception, CodeBuilder code)
//     {
//         code.Write("Type: ").AppendLine(exception.GetType())
//             .Write("Message: ").AppendLine(exception.Message)
//             .If(
//                 exception.HResult != 0,
//                 cb => cb.AppendLine($"HResult: 0x{exception.HResult:X8} ({exception.HResult})"))
//             .If(
//                 exception.HelpLink.IsNonWhiteSpace,
//                 cb =>
//                 {
//                     cb.Write("HelpLink: ").AppendLine(exception.HelpLink);
//                     //todo add help based on errorcode
//                 })
//             .If(
//                 exception.Source.IsNonWhiteSpace,
//                 cb => cb.Write("Source: ").AppendLine(exception.Source))
//             .Write("TargetSite: ").AppendLine(exception.TargetSite)
//             .AppendLine("StackTrace:")
//             .AppendLine("-----------")
//             .AppendLine(exception.StackTrace)
//             .AppendLine("-----------")
//             .If(
//                 exception.Data.Count > 0,
//                 cb => cb.Write("Data:")
//                     .Delimit(
//                         d => d.NewLine().Write("  "),
//                         exception.Data.Cast<DictionaryEntry>(),
//                         (b, entry) => b.Write(entry.Key).Write(": ").Write(entry.Value)))
//             .If(
//                 exception.InnerException is not null,
//                 cb => cb.Write("InnerException:")
//                     .Indented("    ", i => i.NewLine().Write(exception.InnerException))
//                     .NewLine())
//             .If(
//                 exception is AggregateException,
//                 cb => cb.AppendLine("InnerExceptions:")
//                     .Indented(
//                         "    ",
//                         i => i
//                             .NewLine()
//                             .EnumerateAppendLines((exception as AggregateException)!.InnerExceptions)))
//             ;
//     }
// }