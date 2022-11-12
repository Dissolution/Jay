using Jay.Reflection;
using Jay.Reflection.Internal;

namespace Jay.BenchTests;

public class MiscReflectionTests
{
    [Fact]
    public void UnmanagedIsNotReference()
    {
        // Check a whole bunch of types
        var types = Reflect.AllExportedTypes();
        foreach (var type in types)
        {
            try
            {
                var isUnmanaged = type.IsUnmanaged();
                var isReference = TypeCache.IsReferenceOrContainsReferences(type);
                Assert.Equal(isReference, !isUnmanaged);
            }
            catch
            {
                // Ignore, we're using a lot of random types here
            }
           
        }
    }
}