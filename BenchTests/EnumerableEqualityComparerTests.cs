using Jay.BenchTests.Entities;
using Jay.Comparision;

// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable StackAllocInsideLoop

namespace Jay.BenchTests;

public class EnumerableEqualityComparerTests
{
    [Fact]
    public void ValueEqualsValue()
    {
        var eq = EnumerableEqualityComparer<int>.Default;
        Assert.True(eq.Equals(5, 5));
        Assert.True(eq.Equals(int.MinValue, int.MinValue));
        Assert.True(eq.Equals(0, default(int)));
        Assert.False(eq.Equals(1, 56));
        Assert.Equal(eq.GetHashCode(147), eq.GetHashCode(147));
        Assert.NotEqual(eq.GetHashCode(33), eq.GetHashCode(78));
    }
    
    [Fact]
    public void ValueEqualsValues()
    {
        var comparer = EnumerableEqualityComparer<int>.Default;

        var array = new int[1] { 11 };
        Assert.True(comparer.Equals(11, array));
        Assert.Equal(comparer.GetHashCode(11), comparer.GetHashCode(array));
        
        Span<int> span = stackalloc int[1] { 22 };
        Assert.True(comparer.Equals(22, span));
        Assert.Equal(comparer.GetHashCode(22), comparer.GetHashCode(span));
        
        IEnumerable<int> enumerable = Enumerable.Range(33, 1);
        Assert.True(comparer.Equals(33, enumerable));
        Assert.Equal(comparer.GetHashCode(33), comparer.GetHashCode(enumerable));
    }

    [Fact]
    public void ValuesEqualsValues()
    {
        var comparer = EnumerableEqualityComparer<int>.Default;
        
        foreach (int length in new[] { 0, 1, 128 })
        {
            int[] array = new int[length];
#pragma warning disable CA2014
            Span<int> span = stackalloc int[length];
#pragma warning restore CA2014
            List<int> list = new List<int>(length);
            IEnumerable<int> enumerable = Enumerable.Range(0, length);
            for (var i = 0; i < length; i++)
            {
                array[i] = i;
                span[i] = i;
                list.Add(i);
            }
            
            Assert.True(comparer.Equals(array, span));
            Assert.True(comparer.Equals(array, list));
            Assert.True(comparer.Equals(array, enumerable));
            Assert.True(comparer.Equals(span, list));
            Assert.True(comparer.Equals(span, enumerable));
            Assert.True(comparer.Equals(list, enumerable));
        }
    }

    private static IEnumerable<TestClass?> YieldNull()
    {
        yield return null;
    }

    [Fact]
    public void NullEquals()
    {
        var intComparer = EnumerableEqualityComparer<int>.Default;
        Assert.True(intComparer.Equals(null, null));

        var testComparer = EnumerableEqualityComparer<TestClass?>.Default;
        TestClass? nullTest = null;
        TestClass?[] array = new TestClass?[1] { null };
        Span<TestClass?> span = array;
        List<TestClass?> list = new List<TestClass?>(1) { null };
        IEnumerable<TestClass?> enumerable = YieldNull();

        Assert.True(testComparer.Equals(nullTest, nullTest));
        Assert.True(testComparer.Equals(nullTest, array));
        Assert.True(testComparer.Equals(nullTest, (TestClass?[]?)null));
        Assert.True(testComparer.Equals(nullTest, span));
        Assert.True(testComparer.Equals(nullTest, list));
        Assert.True(testComparer.Equals(nullTest, enumerable));
        Assert.True(testComparer.Equals(nullTest, (IEnumerable<TestClass?>?)null));
    }
}