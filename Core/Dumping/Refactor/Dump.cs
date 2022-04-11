using Jay.Collections;

namespace Jay.Dumping.Refactor;

public static class DumpExtensions
{
    public static string Dump<T>(this T? value, DumpOptions options = default) 
        => Refactor.Dump.Value<T>(value, options);
}

public readonly struct DumpOptions
{
    public readonly bool TypeHints = false;

    public DumpOptions() { }

    public DumpOptions(bool typeHints)
    {
        this.TypeHints = typeHints;
    }
}

    


public static partial class Dump
{
    private static readonly ConcurrentTypeDictionary<object?> _cache = new();



    public static string Value<T>(T? value, DumpOptions options = default)
    {
        throw new NotImplementedException();
    }
}