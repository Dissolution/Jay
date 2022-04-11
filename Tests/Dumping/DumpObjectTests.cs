using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jay.Dumping.Refactor;

namespace Jay.Tests.Dumping;

public class DumpObjectTests
{
    public static readonly DumpOptions DxOptions = new DumpOptions(true);

    [Fact]
    public void CanDumpNullObject()
    {
        object? obj = null;
        string d = Dump.Value<object?>(obj, default);
        Assert.True(d == string.Empty);
        string dx = Dump.Value<object?>(obj, DxOptions);
        Assert.True(dx == "(object)null");
    }

    [Fact]
    public void CanDumpNullString()
    {
        string? str = null;
        string d = Dump.Value<string?>(str, default);
        Assert.True(d == string.Empty);
        string dx = Dump.Value<string?>(str, DxOptions);
        Assert.True(d == "(string)null");
    }

    [Fact]
    public void CanDumpNullable()
    {
        Nullable<int> ni = null;
        string d = Dump.Value<int?>(ni, default);
        Assert.True(d == string.Empty);
        string dx = Dump.Value<int?>(ni, DxOptions);
        Assert.True(d == "(int?)null");

        object? obj = ni;
        d = Dump.Value<object?>(obj, default);
        Assert.True(d == string.Empty);
        dx = Dump.Value<object?>(obj, DxOptions);
        Assert.True(d == "(int?)null");
    }

    [Fact]
    public void CanDumpObject()
    {
        object? obj = new object();
        string d = Dump.Value<object?>(obj, default);
        Assert.True(d == "");
        string dx = Dump.Value<object?>(obj, DxOptions);
        Assert.True(dx == "(object)");
    }
}