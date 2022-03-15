namespace Jay.Reflection.Search;

public static class Mirror
{
    public static IEnumerable<Type> AllAssembliesExportedTypes()
    {
        return AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(assembly =>
                        {
                            try
                            {
                                return assembly.ExportedTypes;
                            }
                            catch //(Exception ex)
                            {
                                return Array.Empty<Type>();
                            }
                        });
    }
}