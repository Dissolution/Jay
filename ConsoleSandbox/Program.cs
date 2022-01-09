using System.Diagnostics;
using System.Reflection;
using Jay.Reflection;
//
//
// var thing = new Thing();
// var idProperty = typeof(Thing).GetProperty(nameof(Thing.Id), BindingFlags.Public | BindingFlags.Instance)!;
// var backingField = idProperty.GetBackingField();


object? a = 147;

string tOne = Scope.GetType(a);
string tTwo = Scope.GetGenType(a);
string tThree = Scope.GetGenValueType(a);

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




internal class Scope
{
    public static string GetType(object? obj)
    {
        if (obj is null)
            return "null";
        if (obj is int i)
            return "int";

        return obj.GetType().Name;
    }
    public static string GetGenType<T>(T value) => typeof(T).Name;

    public static string GetGenValueType<T>(T value)
    {
        if (value is null) return "null";
        if (value is int i) return "int";
        return value.GetType().Name;
    }

}