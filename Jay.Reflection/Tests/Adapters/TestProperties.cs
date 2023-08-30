
namespace Jay.Reflection.Tests.Adapters;


public class TestProperties
{
    [Fact]
    public void TestGet()
    {
        foreach (var type in EntityGenerator.EntityTypes)
        {
            var instance = EntityGenerator.New(type);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var propertyInfo in properties)
            {
                var srFieldValue = propertyInfo.GetValue(instance);
                var jrFieldValue = propertyInfo.GetValue<object, object>(ref instance);
                Assert.True(Easy.Equal(srFieldValue, jrFieldValue));
            }
        }
    }
    
    [Fact]
    public void TestSet()
    {
        foreach (var type in EntityGenerator.EntityTypes)
        {
            var instance = EntityGenerator.New(type);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var property in properties)
            {
                var originalValue = property.GetValue<object, object>(ref instance);
                var value = EntityGenerator.New(property.PropertyType);
                property.SetValue<object, object>(ref instance, value);
                var newValue = property.GetValue<object, object>(ref instance);
                Assert.False(Easy.Equal(originalValue, newValue));
                Assert.True(Easy.Equal(value, newValue));
            }
        }
    }
}