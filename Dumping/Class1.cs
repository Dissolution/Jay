global using Jay.Text;
using System.Diagnostics;
using Jay;
using Jay.Collections;
using Jay.Reflection;

namespace Dumping;

[Flags]
public enum DumpOptions
{
    Simple = 0,
    Details = 1 << 0,
}

public static class Dumpers
{
    private static readonly List<IDumper> _dumpers;
    private static readonly ConcurrentTypeDictionary<IDumper> _dumperMap;

    static Dumpers()
    {
        var dumperTypes = AppDomain.CurrentDomain
                                   .GetAssemblies()
                                   .SelectMany(assembly => assembly.ExportedTypes)
                                   .Where(type => type.Implements<IDumper>())
                                   .Where(type => type.IsClass && !type.IsAbstract && !type.IsInterface &&
                                                  !type.IsNested)
                                   .SelectWhere((Type type, out IDumper dumper) =>
                                   {
                                       try
                                       {
                                           if (Activator.CreateInstance(type).Is(out dumper))
                                           {
                                               return true;
                                           }
                                           return false;
                                       }
                                       catch (Exception ex)
                                       {
                                           Debugger.Break();
                                           dumper = null!;
                                           return false;
                                       }
                                   });
        _dumpers = new List<IDumper>(dumperTypes);
        _dumperMap = new ConcurrentTypeDictionary<IDumper>();
    }

    internal static IDumper GetDumper(Type type)
    {
        IDumper? dumper;
        if (_dumperMap.TryGetValue(type, out dumper))
        {
            return dumper!;
        }

        for (var i = 0; i < _dumpers.Count; i++)
        {
            dumper = _dumpers[i];
            if (dumper.CanDump(type))
            {
                return (_dumperMap[type] = dumper);
            }
        }


    }

    public static bool DumpNull<T>(TextBuilder text, T? value, DumpOptions options = default)
    {
        if (value is null)
        {
            if (options.HasFlag(DumpOptions.Details))
            {
                text.Append('(')
                    .AppendDump(typeof(T))
                    .Append(')');
            }
            text.Write("null");
            return true;
        }
        return false;
    }

    public static TextBuilder AppendDump<T>(this TextBuilder builder, T? value, DumpOptions options = default)
    {
        
    }
}

public interface IDumper
{
    bool CanDump(Type type);

    void Dump(TextBuilder builder, object? value, DumpOptions options = default);
}

public interface IDumper<T> : IDumper
{
    bool IDumper.CanDump(Type type)
    {
        return type.Implements<T>();
    }

    void IDumper.Dump(TextBuilder text, object? value, DumpOptions options)
    {
        if (
    }

    void Dump(TextBuilder builder, T? value, DumpOptions options = default);
}