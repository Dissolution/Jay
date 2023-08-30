
namespace Jay.Reflection.Tests.Adapters;


public class TestFields
{
    [Fact]
    public void TestGet()
    {
        foreach (var type in EntityGenerator.EntityTypes)
        {
            var instance = EntityGenerator.New(type);
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var srFieldValue = field.GetValue(instance);
                var jrFieldValue = field.GetValue<object, object>(ref instance);
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
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var originalValue = field.GetValue<object, object>(ref instance);
                var value = EntityGenerator.New(field.FieldType);
                field.SetValue<object, object>(ref instance, value);
                var newValue = field.GetValue<object, object>(ref instance);
                Assert.False(Easy.Equal(originalValue, newValue));
                Assert.True(Easy.Equal(value, newValue));
            }
        }
    }
}