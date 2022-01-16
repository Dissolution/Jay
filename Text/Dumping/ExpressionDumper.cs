using System.Diagnostics;
using Jay.Collections;
using Jay.Validation;

namespace Jay.Text.Dumping;

public static class Dumpers
{
    private sealed class DefaultDumper : IDumper
    {
        public bool CanDump(Type type) => true;

        public void Dump(TextBuilder text, object? value, DumpLevel level = DumpLevel.Self)
        {
            text.Write(value);
        }
    }
    
    private static readonly IDumper _defaultDumper = new DefaultDumper();
    private static readonly List<IDumper> _dumpers;
    private static readonly ConcurrentTypeDictionary<IDumper> _dumperMap;

    static Dumpers()
    {
        _dumpers = new List<IDumper>();
        _dumperMap = new ConcurrentTypeDictionary<IDumper>();
        var dumperTypes = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .SelectMany(assembly => assembly.DefinedTypes)
                                .Where(type => type.IsAssignableTo(typeof(IDumper)))
                                .Where(type => type.IsClass && !type.IsAbstract && !type.IsInterface && !type.IsNested);
        foreach (var dumperType in dumperTypes)
        {
            try
            {
                if (Activator.CreateInstance(dumperType) is IDumper dumper)
                {
                    _dumpers.Add(dumper);
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }
    }

    internal static IDumper GetDumper(Type type)
    {
        if (_dumperMap.TryGetValue(type, out var dumper))
        {
            return dumper;
        }

        for (var i = 0; i < _dumpers.Count; i++)
        {
            dumper = _dumpers[i];
            if (dumper.CanDump(type))
            {
                return (_dumperMap[type] = dumper);
            }
        }

        return _defaultDumper;
    }

    internal static IDumper<T> GetDumper<T>() => (GetDumper(typeof(T)) as IDumper<T>).ThrowIfNull();

    public static void Dump<T>(TextBuilder text, T? value, DumpLevel level = DumpLevel.Self)
    {
        GetDumper<T>().Dump(text, value, level);
    }

    public static string Dump<T>(T? value, DumpLevel level = DumpLevel.Self)
    {
        using var text = new TextBuilder();
        GetDumper<T>().Dump(text, value, level);
        return text.ToString();
    }
}

[Flags]
public enum DumpLevel
{
    Self = 0,
    Details = 1 << 0,
    Surroundings = 1 << 1,
}

public interface IDumper
{
    bool CanDump(Type type);

    void Dump(TextBuilder text, object? value, DumpLevel level = DumpLevel.Self);

    string Dump(object? value, DumpLevel level = DumpLevel.Self)
    {
        using var text = new TextBuilder();
        Dump(text, value, level);
        return text.ToString();
    }
}

public interface IDumper<in T> : IDumper
{
    bool IDumper.CanDump(Type type)
    {
        return type.IsAssignableTo(typeof(T));
    }

    void IDumper.Dump(TextBuilder text, object? value, DumpLevel level)
    {
        if (value is T typed)
        {
            Dump(text, typed, level);
        }
        else
        {
            Dumpers.Dump(text, value, level);
        }
    }

    void Dump(TextBuilder text, T? value, DumpLevel level = DumpLevel.Self);

    string Dump(T? value, DumpLevel level = DumpLevel.Self)
    {
        using var text = new TextBuilder();
        Dump(text, value, level);
        return text.ToString();
    }
}


