using Jay.Collections;

namespace Jay.Tests.Collections;

public sealed class ArrayWrapper2DTests
{
    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[1] { new char[0,0] };
        yield return new object[1] { new char[2, 3] { { 'A', 'B', 'C' }, { '1', '2', '3' }, } };
    }
    
    
    [Theory]
    [MemberData(nameof(GetTestData))]
    public void TestEnumeration(char[,] typedArray)
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
    public void TestIndexer(char[,] typedArray)
    {
        ArrayWrapper<char> wrapper = new((Array)typedArray);
        Assert.Equal(typedArray.Length, wrapper.Count);

        for (var x = 0; x < typedArray.GetLength(0); x++)
        for (var y = 0; y < typedArray.GetLength(1); y++)
        {
            int[] indices = new int[2] { x, y };
            Assert.Equal(typedArray[x,y], wrapper[indices]);
        }
    }
}