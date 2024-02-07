// namespace Jay.Text.Tests.TextBuilderTests;
//
// public class ReplaceTests
// {
//     [Fact]
//     public void ReplaceChar()
//     {
//         using var text = new TextBuilder();
//         text.Append(TestData.LOREM_IPSUM);
//         Assert.Equal(TestData.LOREM_IPSUM.Length, text.Length);
//         Assert.Equal(4, text.AsSpan().CountInstances('o'));
//         Assert.Equal(1, text.AsSpan().CountInstances('.'));
//         text.Replace('o', '.');
//         Assert.Equal(TestData.LOREM_IPSUM.Length, text.Length);
//         Assert.Equal(0, text.AsSpan().CountInstances('o'));
//         Assert.Equal(5, text.AsSpan().CountInstances('.'));
//     }
//
//     
//
//     [Fact]
//     public void ReplaceStringExact()
//     {
//         using var text = new TextBuilder();
//         text.Append(TestData.LOREM_IPSUM);
//         Assert.Equal(TestData.LOREM_IPSUM.Length, text.Length);
//         Assert.Equal(4, text.Written.CountInstances('o'));
//         Assert.Equal(1, text.Written.CountInstances('.'));
//         text.Replace("o", ".");
//         Assert.Equal(TestData.LOREM_IPSUM.Length, text.Length);
//         Assert.Equal(0, text.Written.CountInstances('o'));
//         Assert.Equal(5, text.Written.CountInstances('.'));
//
//         // ip / it
//         int ipCount = TestExtensions.CountInstances(text.Written, "ip");
//         Assert.Equal(2, ipCount);
//         int itCount = TestExtensions.CountInstances(text.Written, "it");
//         Assert.Equal(2, itCount);
//
//         text.Replace("ip", "it");
//         ipCount = TestExtensions.CountInstances(text.Written, "ip");
//         Assert.Equal(0, ipCount);
//         itCount = TestExtensions.CountInstances(text.Written, "it");
//         Assert.Equal(4, itCount);
//     }
//
//     [Fact]
//     public void ReplaceStringShrink()
//     {
//         using var text = new TextBuilder();
//         text.Append(TestData.STUTTER);
//         Assert.Equal(TestData.STUTTER.Length, text.Length);
//         int thCount = TestExtensions.CountInstances(text.Written, "th");
//         Assert.Equal(3, thCount);
//
//         text.Replace("th", "d");
//         Assert.Equal(TestData.STUTTER.Length - 3, text.Length);
//         thCount = TestExtensions.CountInstances(text.Written, "th");
//         Assert.Equal(0, thCount);
//     }
//     
//     [Fact]
//     public void ReplaceStringShrinkIgnoreCase()
//     {
//         using var text = new TextBuilder();
//         text.Append(TestData.STUTTER);
//         Assert.Equal(TestData.STUTTER.Length, text.Length);
//         int thCount = TestExtensions.CountInstances(text.Written, "TH", StringComparison.OrdinalIgnoreCase);
//         Assert.Equal(4, thCount);
//
//         text.Replace("th", "d", StringComparison.OrdinalIgnoreCase);
//         Assert.Equal(TestData.STUTTER.Length - 4, text.Length);
//         thCount = TestExtensions.CountInstances(text.Written, "TH", StringComparison.OrdinalIgnoreCase);
//         Assert.Equal(0, thCount);
//     }
//     
//     [Fact]
//     public void ReplaceStringGrow()
//     {
//         using var text = new TextBuilder();
//         text.Append(TestData.STUTTER);
//         Assert.Equal(TestData.STUTTER.Length, text.Length);
//         int thCount = TestExtensions.CountInstances(text.Written, "th");
//         Assert.Equal(3, thCount);
//
//         text.Replace("th", "bgi");
//         Assert.Equal(TestData.STUTTER.Length + 3, text.Length);
//         thCount = TestExtensions.CountInstances(text.Written, "th");
//         Assert.Equal(0, thCount);
//     }
//     
//     [Fact]
//     public void ReplaceStringGrowIgnoreCase()
//     {
//         using var text = new TextBuilder();
//         text.Append(TestData.STUTTER);
//         Assert.Equal(TestData.STUTTER.Length, text.Length);
//         int thCount = TestExtensions.CountInstances(text.Written, "th", StringComparison.OrdinalIgnoreCase);
//         Assert.Equal(4, thCount);
//
//         text.Replace("th", "bgi", StringComparison.OrdinalIgnoreCase);
//         Assert.Equal(TestData.STUTTER.Length + 4, text.Length);
//         thCount = TestExtensions.CountInstances(text.Written, "th");
//         Assert.Equal(0, thCount);
//     }
// }