#region SETUP
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Jay;
using Jay.Comparison;
using Jay.Dumping.Extensions;
using Jay.Extensions;
using Jay.Reflection;
using Jay.Text.TextBuilder;
using Jay.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, _) => { })
    .ConfigureLogging(logging => logging.AddConsole())
    .Build();


await host.StartAsync();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var token = lifetime.ApplicationStopping;
var logger = host.Services.GetRequiredService<ILogger<Program>>();
#endregion

// WORK

var exprType = typeof(Expression);
var exprPropNames = exprType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
    .Select(prop => prop.Name)
    .ToHashSet();

var expressionTypes = Reflect.AllExportedTypes
    .Where(type => type != exprType)
    .Where(type => type.Implements(exprType))
    .Where(type => type.BaseType == exprType)
    .OrderBy(type => type.Name)
    .ToList();

using var text = new TextBuilder();
foreach (var expressionType in expressionTypes)
{
    var properties = expressionType
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(prop => prop.PropertyType != typeof(bool))
        .ToList();
    var excessProperties = properties.ExceptBy(exprPropNames, prop => prop.Name);
    //Debugger.Break();
    var varName = expressionType.Name
        .Select<char, char>((c, i) => i == 0 ? c.ToLower() : c)
        .ToArray()
        .AsSpan()
        .ToString();
    text.Append($"case {expressionType.Name} {varName}:").NewLine()
        .Append("{").NewLine();
    foreach (var excessProperty in excessProperties)
    {
        text.Append("    ");
        var epType = excessProperty.PropertyType;
        var epName = $"{varName}.{excessProperty.Name}";
        if (epType.Implements<Expression>())
        {
            text.Append($"ParseExpression(data, {epName});").NewLine();
        }
        else if (epType.Implements<IEnumerable<Expression>>())
        {
            text.Append($"ParseExpressions(data, {epName});").NewLine();
        }
        else if (epType.Implements(typeof(IEnumerable)) && epType != typeof(string))
        {
            var valueType = epType.GetGenericArguments()[0];
            if (valueType.IsClass)
            {
                text.Append($"// {valueType}").NewLine()
                    .Append("    ");
            }

            text.Append($"ParseEnumerable(data, {epName});").NewLine();
        }
        else
        {
            text.Append($"ParseValue(data, {epName});").NewLine();
        }
    }
    text.Append("    return;").NewLine();
    text.Append("}").NewLine();
}
   
string code = text.ToString();

Debugger.Break();


/*


var parseTypes = Reflect.AllExportedTypes
    .Where(type => Enumerable.Any(type.GetMethods(Reflect.Flags.Static), method => method.Name == "TryParse"))
    .Where(type => !type.Name.Contains("Extension", StringComparison.OrdinalIgnoreCase ))
    .ToList();

var parameterComparer = new FuncEqualityComparer<ParameterInfo>(
    (a, b) => a?.ParameterType == b?.ParameterType,
    p => Hasher.Generate<Type>(p?.ParameterType)
);

var uniqueFirsts = new HashSet<ParameterInfo>(parameterComparer);
var uniqueParameters = new HashSet<ParameterInfo[]>(new EnumerableEqualityComparer<ParameterInfo>(parameterComparer));

/* Notes:
 * If we want to support TryParse(Type,..., out object obj), we can look for TryParse methods that have a Type as the first param
 *  
 

foreach (var type in parseTypes)
{
    var tryParseMethods = type
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Where(method =>
        {
            if (method.Name != "TryParse") return false;
            if (method.ReturnType != typeof(bool)) return false;
            var methodParams = method.GetParameters();
            if (methodParams.Length < 2) return false;
            /*var firstParam = methodParams[0];
            if (firstParam.ParameterType != typeof(string) &&
                firstParam.ParameterType != typeof(ReadOnlySpan<char>)) return false;* /
            var lastParam = methodParams[^1];
            if (!lastParam.IsOut) return false;
            var lastParamType = lastParam.ParameterType.GetElementType()!;
            if (lastParamType != type)
            {
                if (lastParamType.IsGenericParameter)
                {
                    var constraints = lastParamType.GetGenericParameterConstraints();
                    if (!constraints.All(type.Implements))
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        })
        .ToList();

    foreach (var tryParseMethod in tryParseMethods)
    {
        var parameters = tryParseMethod.GetParameters();
        uniqueFirsts.Add(parameters[0]);
        // Only care about the middle params, as the last one is out T
        var slice = parameters[1..^1];
        uniqueParameters.Add(slice);
        if (parameters.Any(p => p.ParameterType == typeof(NumberFormatInfo)))
            Debugger.Break();
    }

    //Debugger.Break();
}

var output = TextBuilder.Borrow();
output.AppendDelimit<ParameterInfo>(Environment.NewLine, uniqueFirsts, (t, p) => t.Append(p.Dump()))
    .AppendNewLines(2)
    .AppendDelimit<ParameterInfo[]>(Environment.NewLine, uniqueParameters,
        (tb, ps) => tb.AppendDelimit<ParameterInfo>(", ", ps, (t, p) => t.Append(p.Dump())));

var str = output.ToString();

Debugger.Break();

string text = BindingFlags.Static.ToString();

Debugger.Break();

*/
#region TEARDOWN
lifetime.StopApplication();
await host.WaitForShutdownAsync();
return 0; // OK
#endregion