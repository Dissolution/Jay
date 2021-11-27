using System.Runtime.CompilerServices;
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
}