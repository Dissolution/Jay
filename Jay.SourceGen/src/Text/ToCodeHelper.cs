using Jay.Enums;
using Jay.SourceGen.Reflection;
using Jay.SourceGen.Utilities;

namespace Jay.SourceGen.Text;

public static class ToCodeHelper
{
    private static readonly Dictionary<Type, Delegate?> _cache = new();

    static ToCodeHelper()
    {
        // // Load ToCodeProviders
        // var toCodeProviderType = typeof(ToCodeProvider<>);
        // var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        // int count = assemblies.Length;
        // for (var i = 0; i < count; i++)
        // {
        //     try
        //     {
        //         var types = assemblies[i].ExportedTypes;
        //         foreach (var type in types)
        //         {
        //             if (type.IsConstructedGenericType)
        //             {
        //                 if (type.GetGenericTypeDefinition() == toCodeProviderType)
        //                 {
        //                     // Construct an instance
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         DebugLog.Log(ex);
        //         // Ignore this assembly
        //     }
        // }
        
        AddToCode<Keywords>(static (cb, keywords) =>
        {
            var flags = keywords.GetFlags();
            if (flags.Length == 0) return false;
            cb.Delimit(" ", keywords.GetFlags(), static (cb, k) => cb.Append(k, Casing.Lower));
            return true;
        });
        
        AddToCode<Visibility>(static (cb, vis) =>
        {
            bool wrote = false;
            if (vis.HasFlag(Visibility.Static))
            {
                cb.Append("static ");
                wrote = true;
            }
            if (vis.HasFlag(Visibility.Private))
            {
                cb.Append("private ");
                wrote = true;
            }
            if (vis.HasFlag(Visibility.Protected))
            {
                cb.Append("protected ");
                wrote = true;
            }
            if (vis.HasFlag(Visibility.Internal))
            {
                cb.Append("internal ");
                wrote = true;
            }
            if (vis.HasFlag(Visibility.Public))
            {
                cb.Append("public ");
                wrote = true;
            }

            if (wrote)
            {
                cb.TryRemove(^1..);
            }
            return wrote;
        });
    }

    public static void AddToCode<T>(CBVP<T> writeValueTo)
    {
        _cache[typeof(T)] = writeValueTo;
    }
    
    public static bool WriteValueTo<T>([AllowNull, NotNullWhen(true)] T value, CodeBuilder codeBuilder)
    {
        if (value is null) 
            return false;
        if (value is IToCode)
            return ((IToCode)value).WriteTo(codeBuilder);
        if (_cache.TryGetValue(typeof(T), out var @delegate) &&
            @delegate is CBVP<T> build)
        {
            return build(codeBuilder, value);
        }

        // if (value is Enum e)
        // {
        //     //Console.WriteLine(e);
        //     throw new NotImplementedException();
        // }

        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(null, null);
        }
        else
        {
            str = value.ToString();
        }

        if (string.IsNullOrEmpty(str))
            return false;

        codeBuilder.Append(str);
        return true;
    }
}