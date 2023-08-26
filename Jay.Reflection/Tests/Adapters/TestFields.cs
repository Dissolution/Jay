using Jay.Reflection.Comparison;

namespace Jay.Reflection.Tests.Adapters;


public class TestFields
{
    [Fact]
    public void TestGet()
    {
        foreach (var type in TestGenerator.TestTypes)
        {
            var instance = TestGenerator.New(type);
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var srFieldValue = field.GetValue(instance);
                var jrFieldValue = field.GetValue<object, object>(ref instance);
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
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var originalValue = field.GetValue<object, object>(ref instance);
                var value = TestGenerator.Object(field.FieldType);
                field.SetValue<object, object>(ref instance, value);
                var newValue = field.GetValue<object, object>(ref instance);
                Assert.False(EasyComparer.Equals(originalValue, newValue));
                Assert.True(EasyComparer.Equals(value, newValue));
            }
        }
    }
}