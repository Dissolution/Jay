using InlineIL;

namespace Jay.Reflection;

/// <summary>
/// The main utility library for reflection
/// </summary>
public static class Reflect
{
    public static MethodInfo GetInvokeMethod<TDelegate>()
        where TDelegate : Delegate
    {
        return typeof(TDelegate)
            .GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance)
            .ThrowIfNull($"Could not find {typeof(TDelegate)}.Invoke(*)");
    }

    private static readonly Lazy<HashSet<Type>> _allKnownTypes = new(GetAllKnownTypes);
    private static readonly Lazy<HashSet<Type>> _allExportedTypes = new(GetAllExportedTypes);

    public static IReadOnlyCollection<Type> AllKnownTypes => _allKnownTypes.Value;

    public static IReadOnlyCollection<Type> AllExportedTypes => _allExportedTypes.Value;
    
    
    private static HashSet<Type> GetAllKnownTypes()
    {
        var allTypes = new HashSet<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var exportedTypes = assembly.GetTypes();
                foreach (var type in exportedTypes)
                {
                    allTypes.Add(type);
                }
            }
            catch
            {
                // Ignore this assembly
                IL.Emit.Nop();
            }
        }
        return allTypes;
    }
    
    private static HashSet<Type> GetAllExportedTypes()
    {
        var allTypes = new HashSet<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var exportedTypes = assembly.GetExportedTypes();
                foreach (var type in exportedTypes)
                {
                    allTypes.Add(type);
                }
            }
            catch
            {
                // Ignore this assembly
                IL.Emit.Nop();
            }
        }
        return allTypes;
    }


}