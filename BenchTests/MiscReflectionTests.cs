using Jay.Reflection;
using Jay.Reflection.Internal;
using Jay.Reflection.Search;

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
            var isUnmanaged = type.IsUnmanaged();
            var isReference = type.IsReferenceOrContainsReferences();
            Assert.Equal(isReference, !isUnmanaged);
        }
    }
}