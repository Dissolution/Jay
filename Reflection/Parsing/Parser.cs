/*using System.Diagnostics;
using System.Globalization;
using Jay;
using Jay.Collections;
using Jay.Comparison;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Emission;
using Jay.Reflection.Parsing;

namespace Jay.Reflection.Parsing;

public delegate bool TryParseDelegate<T>(ReadOnlySpan<char> text, ParseOptions? options, [MaybeNullWhen(false)] out T value);

public static class Parser
{
    private static readonly ConcurrentTypeDictionary<Delegate> _tryParseCache = new();
    private static readonly IReadOnlyDictionary<Type, FieldInfo> _parseOptionsFields;

    static Parser()
    {
        var parseOptionFields = new Dictionary<Type, FieldInfo>();
        foreach (var field in typeof(ParseOptions).GetFields(Reflect.Flags.Instance))
        {
            Debug.Assert(!parseOptionFields.ContainsKey(field.FieldType));
            parseOptionFields[field.FieldType] = field;
        }

        _parseOptionsFields = parseOptionFields;
    }

    private static bool TryLoadParameter(IFluentILEmitter? emitter, ParameterInfo parameter)
    {
        // `ReadOnlySpan<char> text`
        if (parameter.ParameterType == typeof(ReadOnlySpan<char>))
        {
            emitter?.Ldarg_0();
            return true;
        }

        // `bool ignoreCase` and `bool throwOnFailure`   (from Enum)
        if (parameter.ParameterType == typeof(bool))
        {
            if (string.Equals(parameter.Name, "ignoreCase", StringComparison.OrdinalIgnoreCase))
            {
                emitter?.Ldc_I4_1(); // true
                return true;
            }

            if (string.Equals(parameter.Name, "throwOnFailure"))
            {
                emitter?.Ldc_I4_0(); // false
                return true;
            }

            return false;
        }

        // Check if we can fulfill with a Property on ParseOptions
        if (_parseOptionsFields.TryGetValue(parameter.ParameterType, out var parseOptionsField))
        {
            emitter?.Ldarg_1().Ldfld(parseOptionsField);
            return true;
        }

        Debugger.Break();
        return false;
    }


    private static IComparer<MethodInfo> TryParseMethodSorter { get; } = new FuncComparer<MethodInfo>(
        (firstMethod, secondMethod) =>
        {
            var firstMethodParams = firstMethod!.GetParameters();
            var secondMethodParams = secondMethod!.GetParameters();
            // We want to fill the most parameters
            int c = secondMethodParams.Length.CompareTo(firstMethodParams.Length);
            if (c != 0) return c;
            // Prefer ReadOnlySpan<char> to string?
            var f = firstMethodParams[0];
            var s = secondMethodParams[0];
            if (f.ParameterType == typeof(ReadOnlySpan<char>))
            {
                if (s.ParameterType == typeof(ReadOnlySpan<char>))
                {
                    return 0;
                }

                return -1;
            }

            if (s.ParameterType == typeof(ReadOnlySpan<char>))
            {
                return 1;
            }

            return 0;
        });


    private static bool IsValidTryParseMethod(MethodInfo method)
    {
        if (method.Name != "TryParse") return false;
        if (method.ReturnType != typeof(bool)) return false;
        var methodParams = method.GetParameters();
        if (methodParams.Length < 2) return false;
        var firstParam = methodParams[0];
        if (firstParam.ParameterType != typeof(string) &&
            firstParam.ParameterType != typeof(ReadOnlySpan<char>)) return false;
        var lastParam = methodParams[^1];
        if (!lastParam.IsOut) return false;
        var lastParamType = lastParam.ParameterType.GetElementType()!;
        var type = method.ReflectedType;
        if (lastParamType != method.ReflectedType)
        {
            if (lastParamType.IsGenericParameter)
            {
                var constraints = lastParamType.GetGenericParameterConstraints();
                var len = constraints.Length;
                if (len > 0)
                {
                    for (var i = 0; i < len; i++)
                    {
                        var constraint = constraints[i];
                        if (!method.ReflectedType.Implements(constraint))
                            return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private static RuntimeDelegateBuilder<TryParseDelegate<T>> CreateTryParseDelegate<T>(Type type)
    {
        Debug.Assert(type == typeof(T));

        // TryParse methods to examine
        List<MethodInfo> tryParseMethods;

        if (type.IsEnum)
        {
            tryParseMethods = typeof(Enum).GetMethods(Reflect.Flags.Static).ToList();
        }
        else
        {
            tryParseMethods = type.GetMethods(Reflect.Flags.Static).ToList();
        }

        // Filter
        tryParseMethods = tryParseMethods.Where(method =>
            {
                // Has to be named 'TryParse'
                if (method.Name != "TryParse") return false;
                // Has to return 'bool'
                if (method.ReturnType != typeof(bool)) return false;
                // Validate params
                var parameters = method.GetParameters();
                var len = parameters.Length;
                if (len < 2) return false;
                // First has to be string or ReadOnlySpan<char>
                var first = parameters[0];
                if (first.ParameterType != typeof(string) &&
                    first.ParameterType != typeof(ReadOnlySpan<char>))
                {
                    return false;
                }

                // Last one has to be 'out T'
                var last = parameters[^1];
                if (!last.IsOut) return false;
                var lastParameterType = last.ParameterType.GetElementType()!;
                if (lastParameterType != type)
                {
                    if (type.IsEnum && lastParameterType.IsGenericParameter)
                    {
                        var constraints = lastParameterType.GetGenericParameterConstraints();
                        if (!constraints.Contains(typeof(ValueType)))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                // All the rest have to be ones we can fill
                for (var i = len - 2; i >= 1; i--)
                {
                    var param = parameters[i];
                    if (!TryLoadParameter(null, param)) return false;
                }

                return true;
            })
            .Order(TryParseMethodSorter)
            // consume
            .ToList();

        // None?
        if (tryParseMethods.Count == 0)
        {
            var parseMethods = typeof(Enum).GetMethods(Reflect.Flags.Static)
                .Where(method => method.Name.Contains("Parse", StringComparison.OrdinalIgnoreCase))
                .ToList();

            throw new InvalidOperationException();
        }

        // Fill the best
        var bestMethod = tryParseMethods[0];
        if (bestMethod.IsGenericMethod)
        {
            bestMethod = bestMethod.MakeGenericMethod(type);
        }

        DelegateInfo tryParseDelegateInfo = DelegateInfo.For(bestMethod);

        var builder = RuntimeBuilder.CreateRuntimeDelegateBuilder<TryParseDelegate<T>>($"TryParse_{type}");
        var emitter = builder.Emitter;

        // Load each method parameter except the last (which will be 'out T')
        int count = tryParseDelegateInfo.ParameterCount - 1;
        for (var i = 0; i < count; i++)
        {
            var param = tryParseDelegateInfo.Parameters[i];
            var loaded = TryLoadParameter(emitter, param);
            Debug.Assert(loaded);
        }

        // Arg 2 is the out param
        emitter.Ldarg_2()
            // Call the method
            .Call(tryParseDelegateInfo.Method)
            // the bool is on the stack, so we're done
            .Ret();

        var il = emitter.ToString();

        return builder;
    }

    private static bool TryParseObject(ReadOnlySpan<char> text,
        ParseOptions? options,
        [MaybeNullWhen(false)] out object? obj)
    {

    }

    private static ParseDelegates<T> CreateDelegates<T>(Type type)
    {
        var tryParseBuilder = CreateTryParseDelegate<T>(type);

        var objectParseDelegate = RuntimeBuilder.CreateDelegate<TryParseDelegate<object>>(
            $"TryParse_object_{type.Name}",
            emitter =>
            {
                // Load text
                emitter.Ldarg(0)
                    // Load options
                    .Ldarg(1)
                    // We need a spot for the 'out T'
                    .DeclareLocal(type, out var value)
                    // Get the address
                    .Ldloca(value)
                    // Call the generic method
                    .Call(tryParseBuilder.DynamicMethod)
                    // store the return
                    .DeclareLocal<bool>(out var ret)
                    .Stloc(ret)
                    // pull the value out, box it, store in out object
                    .Ldloc(value)
                    .Box(type)
                    .Starg(2)
                    // reload the return
                    .Ldloc(ret)
                    // done
                    .Ret();

                var il = emitter.ToString();
            });

        return new ParseDelegates<T>(tryParseBuilder.CreateDelegate(), objectParseDelegate);
    }

    private static TryParseDelegate<T> GetTryParseDelegate<T>()
    {
        var parseDelegates =
            _tryParseCache.GetOrAdd<T>(static type => CreateDelegates<T>(type)) as ParseDelegates<T>;
        return parseDelegates!.TryParse;
    }


    public static bool TryParse<T>(this ReadOnlySpan<char> text, ParseOptions? options,
        [MaybeNullWhen(false)] out T value)
    {
        var tpDel = GetTryParseDelegate<T>();
        return tpDel(text, options ?? ParseOptions.Default, out value);
    }

    public static bool TryParse<T>([NotNullWhen(true)] this string? text, ParseOptions? options,
        [MaybeNullWhen(false)] out T value)
    {
        var tpDel = GetTryParseDelegate<T>();
        return tpDel(text.AsSpan(), options ?? ParseOptions.Default, out value);
    }
}

/*

public partial class Parser
{
    private static Result TryConvert(ReadOnlySpan<char> text, out bool boolean, ParseOptions options = default)
    {
        if (text.Length == 0)
        {
            boolean = default;
            return ParseException.Create<bool>(text, options);
        }

        if (text.Length == 1 || options.UseFirstChar)
        {
            char c = text[0];
            if (c is '1' or 'T' or 't' or 'Y' or 'y')
            {
                boolean = true;
                return true;
            }
            if (c is '0' or 'F' or 'f' or 'N' or 'n')
            {
                boolean = false;
                return true;
            }

            boolean = default;
            return ParseException.Create<bool>(text, options);
        }

        if (text.Equals(bool.TrueString, options.Comparison))
        {
            boolean = true;
            return true;
        }

        if (text.Equals(bool.FalseString, options.Comparison))
        {
            boolean = false;
            return true;
        }

        boolean = default;
        return ParseException.Create<bool>(text, options);
    }

    private static Result TryConvert(ReadOnlySpan<char> text, out char character, ParseOptions options = default)
    {
        if (text.Length == 0)
        {
            character = default;
            return ParseException.Create<char>(text, options);
        }

        if (text.Length == 1 || options.UseFirstChar)
        {
            character = text[0];
            return true;
        }

        character = default;
        return ParseException.Create<char>(text, options);
    }

    private static Result TryConvert(ReadOnlySpan<char> text, out Guid guid, ParseOptions options = default)
    {
        if (options.HasExactFormat(out var exactFormat) &&
            Guid.TryParseExact(text, exactFormat, out guid))
        {
            return true;
        }

        if (Guid.TryParse(text, out guid))
        {
            return true;
        }

        guid = Guid.Empty;
        return ParseException.Create<char>(text, options);
    }

    private static Result TryConvert(ReadOnlySpan<char> text, out TimeSpan timeSpan, ParseOptions options = default)
    {
        if (options.HasExactFormat(out var exactFormat) &&
            TimeSpan.TryParseExact(text, exactFormat, options.FormatProvider, options.TimeSpanStyles, out timeSpan))
        {
            return true;
        }

        if (options.ExactFormats != null &&
            TimeSpan.TryParseExact(text, options.ExactFormats, options.FormatProvider, options.TimeSpanStyles, out timeSpan))
        {
            return true;
        }

        if (TimeSpan.TryParse(text, options.FormatProvider, out timeSpan))
        {
            return true;
        }

        timeSpan = TimeSpan.Zero;
        return ParseException.Create<TimeSpan>(text, options);
    }

    private static Result TryConvert(ReadOnlySpan<char> text, out DateTime dateTime, ParseOptions options = default)
    {
        if (options.HasExactFormat(out var exactFormat) &&
            DateTime.TryParseExact(text, exactFormat, options.FormatProvider, options.DateTimeStyles, out dateTime))
        {
            return true;
        }

        if (options.ExactFormats != null &&
            DateTime.TryParseExact(text, options.ExactFormats, options.FormatProvider, options.DateTimeStyles, out dateTime))
        {
            return true;
        }

        if (DateTime.TryParse(text, options.FormatProvider, options.DateTimeStyles, out dateTime))
        {
            return true;
        }

        dateTime = DateTime.Now;
        return ParseException.Create<DateTime>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out DateTimeOffset dateTimeOffset, ParseOptions options = default)
    {
        if (options.HasExactFormat(out var exactFormat) &&
            DateTimeOffset.TryParseExact(text, exactFormat, options.FormatProvider, options.DateTimeStyles, out dateTimeOffset))
        {
            return true;
        }

        if (options.ExactFormats != null &&
            DateTimeOffset.TryParseExact(text, options.ExactFormats, options.FormatProvider, options.DateTimeStyles, out dateTimeOffset))
        {
            return true;
        }

        if (DateTimeOffset.TryParse(text, options.FormatProvider, options.DateTimeStyles, out dateTimeOffset))
        {
            return true;
        }

        dateTimeOffset = DateTimeOffset.Now;
        return ParseException.Create<DateTimeOffset>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out byte value, ParseOptions options = default)
    {
        if (byte.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<byte>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out sbyte value, ParseOptions options = default)
    {
        if (sbyte.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<sbyte>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out short value, ParseOptions options = default)
    {
        if (short.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<short>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out ushort value, ParseOptions options = default)
    {
        if (ushort.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<ushort>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out int value, ParseOptions options = default)
    {
        if (int.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<int>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out uint value, ParseOptions options = default)
    {
        if (uint.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<uint>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out long value, ParseOptions options = default)
    {
        if (long.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<long>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out ulong value, ParseOptions options = default)
    {
        if (ulong.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<ulong>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out float value, ParseOptions options = default)
    {
        if (float.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<float>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out double value, ParseOptions options = default)
    {
        if (double.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<double>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out decimal value, ParseOptions options = default)
    {
        if (decimal.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<decimal>(text, options);
    }

    public static Result TryConvert(ReadOnlySpan<char> text, out string str, ParseOptions options = default)
    {
        str = new string(text);
        return true;
    }
}
#1#*/