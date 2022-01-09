using System.Diagnostics;
using System.Reflection;
using Jay.Reflection;


var thing = new Thing();
var idProperty = typeof(Thing).GetProperty(nameof(Thing.Id), BindingFlags.Public | BindingFlags.Instance)!;
var backingField = idProperty.GetBackingField();

Debugger.Break();

return 0;

public class Thing
{
    private int _id;

    public int Id
    {
        get => _id;
        set => _id = value;
    }
}