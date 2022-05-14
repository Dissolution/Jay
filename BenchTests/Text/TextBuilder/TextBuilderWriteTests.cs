using System.Diagnostics;
using System.Reflection;
using Jay.Text.Scratch;

namespace Jay.BenchTests.Text;

public class TextBuilderWriteTests
{
    public delegate void WriteSpanAction(ref TextBuilder builder, ReadOnlySpan<char> text);
    public delegate void WriteCharAction(ref TextBuilder builder, char ch);

    private readonly List<WriteSpanAction> _writeSpanMethods;
    private readonly List<WriteCharAction> _writeCharMethods;
    
    public TextBuilderWriteTests()
    {
        var writeMethods = typeof(TextBuilder)
                           .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                           .Where(method => method.Name.StartsWith("Write"))
                           .ToList();

        _writeSpanMethods = writeMethods.Where(method => method.Name.StartsWith("WriteSpan"))
                                        .Select(method => method.CreateDelegate<WriteSpanAction>())
                                        .ToList();
        _writeCharMethods = writeMethods.Where(method => method.Name.StartsWith("WriteChar"))
                                        .Select(method => method.CreateDelegate<WriteCharAction>())
                                        .ToList();
    }

    [Fact]
    public void TestWriteSpanMethods()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        try
        {
            foreach (var method in _writeSpanMethods)
            {
                method(ref text, "abc 123");
                Assert.Equal(7, text.Length);
                Assert.Equal('a', text[0]);
                Assert.Equal('b', text[1]);
                Assert.Equal('c', text[2]);
                Assert.Equal(' ', text[3]);
                Assert.Equal('1', text[4]);
                Assert.Equal('2', text[5]);
                Assert.Equal('3', text[6]);
                text.Clear();
                Assert.Equal(0, text.Length);
            }
        }
        finally
        {
            text.Dispose();
        }
    }

    [Fact]
    public void TestWriteCharMethods()
    {
        foreach (var writeChar in _writeCharMethods)
        {
            TextBuilder text = new();
            try
            {
                for (int i = char.MinValue; i <= char.MaxValue; i++)
                {
                    char ch = (char)i;
                    writeChar(ref text, ch);
                    Assert.Equal(i + 1, text.Length);
                    Assert.Equal(ch, text[i]);
                }
            }
            finally
            {
                text.Dispose();
            }
        }
    }

    [Fact]
    public void TestAppendChaining()
    {
        using var text = new TextBuilder();
        Assert.Equal(0, text.Length);
        text.AppendChar('a');
        Assert.Equal(1, text.Length);
        Assert.Equal('a', text[0]);
        text.AppendChar('b');
        Assert.Equal(2, text.Length);
        Assert.Equal('b', text[1]);
        text.AppendChar('c').AppendChar('d').AppendChar('e');
        Assert.Equal(5, text.Length);
        Assert.Equal('c', text[2]);
        Assert.Equal('d', text[3]);
        Assert.Equal('e', text[4]);

        text.AppendDelimit(",", Enumerable.Range(1, 4), (ref TextBuilder builder, int value) => builder.Write<int>(value));
        Assert.Equal(12, text.Length);
        //Assert.Equal(3, text.Where());
        Debugger.Break();
    }
}