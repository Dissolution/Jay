using Jay.Utilities;
using Jay.Text.Building;
using var _ = OnlyApplication.Acquire();

DoThing("blah");
DoThing($"fblah");


Console.WriteLine("Press Enter to exit the application");
Console.ReadLine();
return;


void DoThing(InterpolatedTextBuilder text)
{
    string str = text.ToStringAndDispose();
    Console.WriteLine(str);
}