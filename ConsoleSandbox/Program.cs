<<<<<<< HEAD
﻿using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Reflection.Search;


//var result = MemberSearch.TryFind<MethodInfo>(() => RuntimeHelpers.GetUninitializedObject(default), out var method);

var type = typeof(List<int>);


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

=======
﻿using System.Runtime.CompilerServices;
using Jay.Text;

Test.Thing("Id = {0}", 3);
Test.Thing($"id = {3}");



Console.WriteLine("Press Enter to close");
Console.ReadLine();

static class Test
{
    static Test()
    {

    }


    public static void Thing(RawString format, params object?[] args)
    {

    }

    // public static void Thing(string? format)
    // {
    //
    // }
    //
    // public static void Thing(string? format, params object?[] args)
    // {
    //
    // }

    public static void Thing(FormattableString format)
    {

    }
>>>>>>> Text
}