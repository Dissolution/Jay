using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Xunit.Sdk;

namespace Jay.Reflection.Tests.Entities;

public static class TestGenerator
{
    private static int _sharedInt = 0;
    private static long _sharedLong = 0L;

    public static Type[] TestTypes { get; } = new Type[] { typeof(TestStruct), typeof(TestReadonlyStruct), typeof(TestClass) };
    
    public static int Int()
    {
        return Interlocked.Increment(ref _sharedInt);
    }

    public static string String()
    {
        long value = Interlocked.Increment(ref _sharedLong);
        var span = Scary.UnmanagedToByteSpan<long>(ref value);
        return Convert.ToHexString(span);
    }

    public static object? Object()
    {
        Span<byte> buffer = stackalloc byte[16];
        RandomNumberGenerator.Fill(buffer);
        var str = String();
        var guid = new Guid(buffer);
        return Tuple.Create(str, guid);
    }

    public static object? Object(Type type)
    {
        if (type == typeof(int))
        {
            return Int();
        }
        else if (type == typeof(string))
        {
            return String();
        }
        else if (type == typeof(object))
        {
            return Object();
        }
        else
        {
            throw new NotImplementedException();
        }
    }
    
    [return: NotNull]
    public static T New<T>() => (T)New(typeof(T));
    
    [return: NotNull]
    public static object New(Type type)
    {
        object obj = RuntimeHelpers.GetUninitializedObject(type);
        // set every field
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            object? fieldValue = Object(field.FieldType);
            field.SetValue(obj, fieldValue);
        }
        return obj;
    }
}