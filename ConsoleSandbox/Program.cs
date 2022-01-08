using System.Diagnostics;
using Jay.Reflection;


var thing = new Thing();
thing.Happened += (sender, eventArgs) => Console.WriteLine($"#1--{DateTime.Now:O}\tSender: {sender}\tArgs: {eventArgs}");
thing.Happened += (sender, eventArgs) => Console.WriteLine($"#2--{DateTime.Now:O}\tSender: {sender}\tArgs: {eventArgs}");
thing.Happened += (sender, eventArgs) => Console.WriteLine($"#3--{DateTime.Now:O}\tSender: {sender}\tArgs: {eventArgs}");
thing.Happened += (sender, eventArgs) => Console.WriteLine($"#4--{DateTime.Now:O}\tSender: {sender}\tArgs: {eventArgs}");
var happenedEvent = typeof(Thing).GetEvent(nameof(Thing.Happened), Reflect.AllFlags)!;
var raiser = happenedEvent.CreateRaiser<Thing, EventArgs>();
raiser(ref thing, EventArgs.Empty);
var disposer = happenedEvent.CreateDisposer<Thing>();
disposer(ref thing);
raiser(ref thing, EventArgs.Empty);


Debugger.Break();

return 0;

public class Thing
{
    public event EventHandler<EventArgs>? Happened;

    public Thing()
    {

    }
}