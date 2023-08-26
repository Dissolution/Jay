namespace Jay.Reflection.Tests.Adapters;

public class TestConstructors
{
    [Fact]
    public void TestConstruct()
    {
        foreach (var type in TestGenerator.TestTypes)
        {
            var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var ctor in ctors)
            {
                var ctorParameters = ctor.GetParameters();
                int count = ctorParameters.Length;
                var ctorArgs = new object?[count];
                for (var i = 0; i < count; i++)
                {
                    ctorArgs[i] = TestGenerator.Object(ctorParameters[i].ParameterType);
                }
                object? instance = ctor.Construct<object?>(ctorArgs);
                Assert.True(instance is not null);
                Assert.Equal(instance.GetType(), type);
            }
        }
    }
}