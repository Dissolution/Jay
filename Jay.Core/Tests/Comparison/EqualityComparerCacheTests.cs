using Jay.Comparison;

namespace Jay.Tests.Comparison;

public class EqualityComparerCacheTests
{
    [Fact]
    public void CheckTEquals()
    {
        bool boolean = true;
        Assert.True(EqualityComparerCache.Equals<bool>(boolean, true));
        Assert.False(EqualityComparerCache.Equals<bool>(boolean, false));
        int int32 = 147;
        Assert.True(EqualityComparerCache.Equals<int>(int32, 147));
        Assert.False(EqualityComparerCache.Equals<int>(int32, 13));
        Guid guid = Guid.NewGuid();
        Assert.True(EqualityComparerCache.Equals<Guid>(guid, guid));
        Assert.False(EqualityComparerCache.Equals<Guid>(guid, Guid.NewGuid()));
        string nonNullString = "abc";
        Assert.True(EqualityComparerCache.Equals<string>(nonNullString, "abc"));
        Assert.False(EqualityComparerCache.Equals<string>(nonNullString, "ABC"));
        Assert.False(EqualityComparerCache.Equals<string>(nonNullString, null));
        string? nullString = null;
        Assert.True(EqualityComparerCache.Equals<string>(nullString, null));
        Assert.False(EqualityComparerCache.Equals<string>(nullString, string.Empty));
        Assert.True(EqualityComparerCache.Equals<string?>(nullString, null));
        Assert.False(EqualityComparerCache.Equals<string?>(nullString, ""));
        DateTime dateTime = new DateTime(1955, 11, 25, 8, 0, 0);
        Assert.True(EqualityComparerCache.Equals<DateTime>(dateTime, dateTime));
        Assert.False(EqualityComparerCache.Equals<DateTime>(dateTime, DateTime.Now));
        TestingClass testClass = new(guid, nonNullString);
        Assert.True(EqualityComparerCache.Equals<TestingClass>(testClass, testClass));
        Assert.True(EqualityComparerCache.Equals<TestingClass>(testClass, new(guid, "joe")));
        Assert.False(EqualityComparerCache.Equals<TestingClass>(testClass, new()));
        Assert.False(EqualityComparerCache.Equals<TestingClass>(testClass, null));
    }
    
    [Fact]
    public void CheckObjectEquals()
    {
        bool boolean = true;
        Assert.True(EqualityComparerCache.Equals((object?)boolean, (object?)true));
        Assert.False(EqualityComparerCache.Equals((object?)boolean, (object?)false));
        int int32 = 147;
        Assert.True(EqualityComparerCache.Equals((object?)int32, (object?)147));
        Assert.False(EqualityComparerCache.Equals((object?)int32, (object?)13));
        Guid guid = Guid.NewGuid();
        Assert.True(EqualityComparerCache.Equals((object?)guid, (object?)guid));
        Assert.False(EqualityComparerCache.Equals((object?)guid, (object?)Guid.NewGuid()));
        string nonNullString = "abc";
        Assert.True(EqualityComparerCache.Equals((object?)nonNullString, (object?)"abc"));
        Assert.False(EqualityComparerCache.Equals((object?)nonNullString, (object?)"ABC"));
        Assert.False(EqualityComparerCache.Equals((object?)nonNullString, (object?)null));
        string? nullString = null;
        Assert.True(EqualityComparerCache.Equals((object?)nullString, (object?)null));
        Assert.False(EqualityComparerCache.Equals((object?)nullString, (object?)string.Empty));
        Assert.True(EqualityComparerCache.Equals((object?)nullString, (object?)null));
        Assert.False(EqualityComparerCache.Equals((object?)nullString, (object?)""));
        DateTime dateTime = new DateTime(1955, 11, 25, 8, 0, 0);
        Assert.True(EqualityComparerCache.Equals((object?)dateTime, (object?)dateTime));
        Assert.False(EqualityComparerCache.Equals((object?)dateTime, (object?)DateTime.Now));
        TestingClass testClass = new(guid, nonNullString);
        Assert.True(EqualityComparerCache.Equals((object?)testClass, (object?)testClass));
        Assert.True(EqualityComparerCache.Equals((object?)testClass, (object?)new TestingClass(guid, "joe")));
        Assert.False(EqualityComparerCache.Equals((object?)testClass, (object?)new TestingClass()));
        Assert.False(EqualityComparerCache.Equals((object?)testClass, (object?)null));
    }
}