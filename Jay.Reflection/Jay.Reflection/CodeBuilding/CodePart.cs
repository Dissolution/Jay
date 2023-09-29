using Jay.Collections;

namespace Jay.Reflection.CodeBuilding;

public abstract class CodePart
{
    private static readonly ConcurrentTypeMap<Delegate> _cache;

    static CodePart()
    {
        _cache = new();
        Add(WriteString<DBNull>(nameof(DBNull)));
        Add(WriteString<bool>("bool"));
        Add(WriteString<char>("char"));
        Add(WriteString<sbyte>("sbyte"));
        Add(WriteString<byte>("byte"));
        Add(WriteString<short>("short"));
        Add(WriteString<ushort>("ushort"));
        Add(WriteString<int>("int"));
        Add(WriteString<uint>("uint"));
        Add(WriteString<long>("long"));
        Add(WriteString<ulong>("ulong"));
        Add(WriteString<float>("float"));
        Add(WriteString<double>("double"));
        Add(WriteString<decimal>("decimal"));
        Add(WriteString<string>("string"));
        Add(WriteString<object>("object"));
        _cache.AddOrUpdate(typeof(void), WriteString<object>("void"));
        //Add(WriteString<void>("void"));
        Add(WriteString<nint>("nint"));
        Add(WriteString<nuint>("nuint"));
        Add(WriteString<DateTime>(nameof(DateTime)));
        Add(WriteString<DateTimeOffset>(nameof(DateTimeOffset)));
        Add(WriteString<TimeSpan>(nameof(TimeSpan)));
        Add(WriteString<Guid>(nameof(Guid)));
        // Add(WriteString<TimeOnly>(nameof(TimeOnly)));
        // Add(WriteString<DateOnly>(nameof(DateOnly)));
    }

    internal static void SetString<T>(string str)
    {
        _cache.AddOrUpdate<T>((WriteCode<T>)((_, code) => code.Write(str)));
    }

    private static WriteCode<T> WriteString<T>(string str) => (_, code) => code.Write(str);

    internal static void SetString(Type type, string str)
    {
        // _cache.Set(type, Delegate.CreateDelegate(
        //         typeof(WriteCode<>).MakeGenericType(type),
        //         MemberSearch.One<MemberInfo>(typeof(CodePart), new MemberSearchOptions()
        //         {
        //             Name= nameof(WriteString),
        //             GenericTypeCount = 1,
        //         })))
        //         typeof(CodePart).GetMethod(nameof(WriteString))
        throw new NotImplementedException();
    }
    //internal static void SetToString<T>()

    private static void Add<T>(WriteCode<T> writeCode) => _cache.AddOrUpdate<T>(writeCode);
    //private static void Add(Type type, WriteCode<object?> writeCode) => _cache.Set<T>(writeCode);

    private static void WriteToString<T>(T? value, CodeBuilder code)
    {
        code.Write(value?.ToString());
    }

    private static WriteCode<T> CreateWriteCode<T>(Type type)
    {
        if (type == typeof(int))
            return WriteToString;

        // if (type == typeof(Type))
        //     return WriteType;
        throw new NotImplementedException();
        // MemberInfo
        // ParameterInfo
        // Exception
        // 
    }

    private static WriteCode<T> GetWriteCode<T>()
    {
        return _cache.GetOrAdd<T>(static t => CreateWriteCode<T>(t))
            .AsValid<WriteCode<T>>();
    }

    public static string ToDeclaration(ICodePart codePart)
    {
        return CodeBuilder.New
            .Invoke(codePart.DeclareTo)
            .ToStringAndDispose();
    }
   

    public static void DeclareTo<T>(T? value, CodeBuilder code)
    {
        throw new NotImplementedException();
    }

    public static string ToCode(ref InterpolatedCode code)
    {
        return code.ToStringAndDispose();
    }

    public static string ToCode<T>(T? value)
    {
        throw new NotImplementedException();
    }
}