using Jay.Collections;
using Jay.Text;
using System.Linq;

namespace Jay.Sandbox.T4
{
    public abstract class CodeTextGenerator
    {
        
    }

    public sealed class HoldGenerator : CodeTextGenerator
    {
        public string GenerateValueHolders(int count)
        {
            // [Conditional("DEBUG")]
            // public static void Debug<T1>(T1? value1) { }
            return TextBuilder.Build(sb =>
            {
                for (var i = 1; i <= count; i++)
                {
                    var range = Enumerable.Range(1, i).ToArray();
                    
                    sb.Append("[Conditional(\"DEBUG\")]").AppendNewLine()
                      .Append("public static void Debug<")
                      .AppendDelimit(", ", range.Select(x => $"T{x}"))
                      .Append(">(")
                      .AppendDelimit(", ", range.Select(x => $"T{x}? value{x}"))
                      .Append(") { }").AppendNewLine();
                }
            });
        }
    }
}