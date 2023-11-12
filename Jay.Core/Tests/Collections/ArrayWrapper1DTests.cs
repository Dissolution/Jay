using Jay.Collections;

namespace Jay.Tests.Collections;

public sealed class ArrayWrapper1DTests
{
    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[1] { new char[0] };
        yield return new object[1] { new char[3] { 'A', 'B', 'C' } };
        yield return new object[1] { Guid.NewGuid().ToString("N").ToCharArray() };
    }
    
    
    [Theory]
    [MemberData(nameof(GetTestData))]
    public void TestEnumeration(char[] typedArray)
    {
        ArrayWrapper<char> wrapper = new((Array)typedArray);
        var e = typedArray.GetEnumerator();
        using var w = wrapper.GetEnumerator();
        
        while(true)
        {
            var eMoved = e.MoveNext();
            var wMoved = w.MoveNext();
            Assert.Equal(eMoved, wMoved);
            if (!eMoved) break;
            Assert.Equal(e.Current, w.Current);
        }
    }
    
    [Theory]
    [MemberData(nameof(GetTestData))]
    public void TestIndexer(char[] typedArray)
    {
        ArrayWrapper<char> wrapper = new((Array)typedArray);
        Assert.Equal(typedArray.Length, wrapper.Count);
        for (var i = 0; i < wrapper.Count; i++)
        {
            Assert.Equal(typedArray[i], wrapper[i]);
        }
        Assert.Throws<IndexOutOfRangeException>(() => wrapper[wrapper.Count]);
    }
}