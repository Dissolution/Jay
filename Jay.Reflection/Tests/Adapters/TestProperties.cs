using Jay.Reflection.Comparison;

namespace Jay.Reflection.Tests.Adapters;


public class TestProperties
{
    [Fact]
    public void TestGet()
    {
        foreach (var type in TestGenerator.TestTypes)
        {
            var instance = TestGenerator.New(type);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var propertyInfo in properties)
            {
                var srFieldValue = propertyInfo.GetValue(instance);
                var jrFieldValue = propertyInfo.GetValue<object, object>(ref instance);
                Assert.True(EasyComparer.Equals(srFieldValue, jrFieldValue));
            }
        }
    }
    
    [Fact]
    public void TestSet()
    {
        foreach (var type in TestGenerator.TestTypes)
        {
            var instance = TestGenerator.New(type);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var property in properties)
            {
                var originalValue = property.GetValue<object, object>(ref instance);
                var value = TestGenerator.Object(property.PropertyType);
                property.SetValue<object, object>(ref instance, value);
                var newValue = property.GetValue<object, object>(ref instance);
                Assert.False(EasyComparer.Equals(originalValue, newValue));
                Assert.True(EasyComparer.Equals(value, newValue));
            }
        }
    }
}