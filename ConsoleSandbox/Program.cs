using System.Diagnostics;
using Jay.Text;


using var text = TextBuilder.Borrow();





string consoleOutput = text.ToString();
Debugger.Break();
Console.WriteLine(consoleOutput);
Console.WriteLine("Press Enter to close this window.");
Console.ReadLine();
return 0;


namespace ConsoleSandbox
{
  
}