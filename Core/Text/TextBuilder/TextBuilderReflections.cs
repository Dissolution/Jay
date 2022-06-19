using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Jay.Validation;

namespace Jay.Text;

internal static class TextBuilderReflections
{
    public static MethodInfo WriteString { get; }

    static TextBuilderReflections()
    {
        WriteString = typeof(TextBuilder).GetMethod(
                nameof(TextBuilder.Write),
                BindingFlags.Public | BindingFlags.Instance,
                new Type[] { typeof(string) })
            .ThrowIfNull("Could not find TextBuilder.Write(string)");
    }
    
    public static MethodInfo GetWriteValue(Type valueType)
    {
        // Examine all Write(?) methods
        var writeMethods = typeof(TextBuilder)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.Name == nameof(TextBuilder.Write))
            .Where(method => method.GetParameters().Length == 1)
            .ToList();
        var writeMethod = writeMethods.FirstOrDefault(method =>
        {
            var arg = method.GetParameters()[0];
            return arg.ParameterType == valueType;
        });
        if (writeMethod is not null)
            return writeMethod;
        
        // Use Write<T>
        writeMethod = writeMethods.Where(method => method.ContainsGenericParameters)
            .OneOrDefault();
        if (writeMethod is not null)
            return writeMethod.MakeGenericMethod(valueType);

        throw new InvalidOperationException();
    }
}