using Bogus;

namespace Jay.Reflection.Tests.Entities;

public static class EntityGenerator
{
    public static IReadOnlyCollection<Type> EntityTypes { get; } = new HashSet<Type>()
    {
        typeof(TestClass),
        typeof(TestReadonlyStruct),
        typeof(TestStruct),
    };

    private static readonly Faker _faker = new();
    private static readonly HashSet<Type> _generatableTypes = new()
    {
        typeof(int),
        typeof(string),
        typeof(Guid),
    };


    private static Result<object> GetRandomValue(Type type)
    {
        if (type == typeof(int))
            return _faker.Random.Number(int.MinValue, int.MaxValue);
        if (type == typeof(string))
            return _faker.Random.Utf16String(
                4,
                24,
                true);
        if (type == typeof(Guid))
            return Guid.NewGuid();
        if (type == typeof(object))
        {
            type = _faker.PickRandom<Type>(_generatableTypes);
            return GetRandomValue(type);
        }
        return new InvalidOperationException();
    }

    [return: NotNull]
    public static T New<T>() => (T)New(typeof(T));

    [return: NotNull]
    public static object New(Type type)
    {
        if (GetRandomValue(type).IsOk(out var obj))
            return obj;
        
        obj = Scary.GetUninitializedObject(type);
        // set every field
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            object fieldValue = GetRandomValue(field.FieldType).OkValueOrThrowError();
            field.SetValue(obj, fieldValue);
        }
        return obj;
    }
}