using Jay.Concurrency;
using Jay.Utilities;
using Jay.Text.Building;
using var _ = OnlyApplication.Acquire();

doThing($"blah");
doThing($"fblah");


Console.WriteLine("Press Enter to exit the application");
Console.ReadLine();
return;


void doThing(ref InterpolatedTextWriter text)
{
    string str = text.ToStringAndDispose();
    Console.WriteLine(str);
}