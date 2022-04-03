using System.Buffers.Text;
using System.Diagnostics;
using System.Reflection;
using Jay.Text;
using Lyph.Scratch;


using var text = TextBuilder.Borrow();


while (!Console.KeyAvailable)
{
    



}




string consoleOutput = text.ToString();
Debugger.Break();
Console.WriteLine(consoleOutput);
Console.WriteLine("Press Enter to close this window.");
Console.ReadLine();
return 0;


namespace ConsoleSandbox
{
  
}