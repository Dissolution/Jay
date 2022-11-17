/*using System.Reflection;
using Jay.Extensions;
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
        // See if we can write the valueType directly (not through T)
        var writeMethod = writeMethods.FirstOrDefault(method =>
        {
            var arg = method.GetParameters()[0];
            return arg.ParameterType == valueType;
        });
        if (writeMethod is not null)
            return writeMethod;
        
        // Use Write<T>
        writeMethod = writeMethods
            .Where(method => method.ContainsGenericParameters)
            .OneOrDefault();
        if (writeMethod is not null)
        {
            // Make generic + cast
            return writeMethod.MakeGenericMethod(valueType);
        }
            

        throw new InvalidOperationException();
    }
}*/