using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Jay.Collections;
using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Search;
using Jay.Text;
using Jay.Validation;

namespace Jay.Dumping;

public static class Dump
{
    public sealed class DefaultDumper : IDumper
    {
        public bool CanDump(Type type) => true;

        void IDumper.DumpObject(TextBuilder text, object? value, DumpLevel level) => Dump<object>(text, value, level);

        private static string? GetFormat(Type type, DumpLevel level)
        {
            if (type == typeof(TimeSpan))
            {
                return level.HasFlag(DumpLevel.Detailed) ? "G" : "g";
            }
            else if (type == typeof(DateTime))
            {
                return level.HasFlag(DumpLevel.Detailed) ? "O" : "yyyy-MM-dd HH:mm:ss.f";
            }
            else if (type == typeof(DateTimeOffset))
            {
                return level.HasFlag(DumpLevel.Detailed) ? "O" : "yyyy-MM-dd HH:mm:ss.f zzz";
            }
            else if (type == typeof(Guid))
            {
                return "D";
            }
            else if (type == typeof(decimal))
            {
                return "N";
            }
            else
            {
                return null;
            }
        }

        public void Dump<T>(TextBuilder text, T? value, DumpLevel level = DumpLevel.Default)
        {
            if (Dumping.Dump.DumpNull(text, value, level)) return;
            text.AppendFormat(value, GetFormat(value.GetType(), level), null);
        }
    }
    
    private static readonly List<IDumper> _dumpers;
    private static readonly ConcurrentTypeDictionary<IDumper> _dumperMap;

    public static DefaultDumper Default { get; } = new DefaultDumper();

    static Dump()
    {
        _dumperMap = new ConcurrentTypeDictionary<IDumper>();
        var dumperTypes = AppDomain.CurrentDomain
                                   .GetAssemblies()
                                   .SelectMany(assembly => assembly.ExportedTypes)
                                   .Where(type => type.Implements<IDumper>())
                                   .Where(type => type.IsClass && !type.IsAbstract && !type.IsInterface && !type.IsNested)
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
    }

    internal static bool DumpNull<T>(TextBuilder text, [NotNullWhen(false)] T? value, DumpLevel level)
    {
        if (value is null)
        {
            if (level.HasFlag<DumpLevel>(DumpLevel.Detailed))
            {
                text.Append('(')
                    .AppendDump(typeof(T))
                    .Write(')');
            }
            text.Write("null");
            return true;
        }
        return false;
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

        return Default;
    }

    internal static IDumper<T> GetDumper<T>() => (GetDumper(typeof(T)) as IDumper<T>).ThrowIfNull();

    public static TextBuilder AppendDump<T>(this TextBuilder text, T? value, DumpLevel level = DumpLevel.Default)
    {
        GetDumper<T>().DumpValue(text, value, level);
        return text;
    }

    public static string Value<T>(T? value, DumpLevel level = DumpLevel.Default)
    {
        using var text = new TextBuilder();
        GetDumper<T>().DumpValue(text, value, level);
        return text.ToString();
    }

    public static string Text(ref DumpStringHandler dumpFormattedString)
    {
        return dumpFormattedString.ToStringAndClear();
    }

    internal static TException GetException<TException>(ref DumpStringHandler message, Exception? innerException = null)
        where TException : Exception
    {
        var ctor = ExceptionBuilder.GetCtor<TException>();
        var ex = ctor(message.ToStringAndClear(), innerException);
        return ex;
    }

    [DoesNotReturn]
    public static void ThrowException<TException>(ref DumpStringHandler message, Exception? innerException = null)
        where TException : Exception
    {
        throw GetException<TException>(ref message, innerException);
    }

    [DoesNotReturn]
    public static void ThrowException<TException>(ref DumpStringHandler message, params object?[] args)
        where TException : Exception
    {
        var argTypes = new Type?[args.Length + 1];
        argTypes[0] = typeof(string);
        for (var i = 0; i < args.Length; i++)
        {
            argTypes[i + 1] = args[0]?.GetType();
        }

        var ctor = MemberSearch.FindBestConstructor(typeof(TException),
                                   Reflect.InstanceFlags,
                                   MemberSearch.MemberExactness.CanIgnoreInputArgs,
                                   argTypes)
                               .ThrowIfNull();
        var ex = ctor.Construct<TException>(args);
        throw ex;
    }
}