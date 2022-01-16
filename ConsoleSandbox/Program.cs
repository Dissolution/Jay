using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Reflection.Dumping;
using Jay.Reflection.Search;
using Jay.Text.Dumping;


//var result = MemberSearch.TryFind<MethodInfo>(() => RuntimeHelpers.GetUninitializedObject(default), out var method);

var type = typeof(List<int>);

string rep = new TypeDumper().GetRep(type);

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