﻿using System;
using Jay.Reflection;

namespace Jay.Tests;

public class ImplementsTests
{
    [Fact]
    public void ImplementsNullableSyntax()
    {
        Assert.True(typeof(int?).Implements(typeof(Nullable<int>)));
        Assert.True(typeof(int?).Implements<Nullable<int>>());
        Assert.True(typeof(Nullable<int>).Implements(typeof(int?)));
        Assert.True(typeof(Nullable<int>).Implements<int?>());
    }

    [Fact]
    public void ImplementsNullableOpen()
    {
        var nullableIntType = typeof(int?);
        Assert.True(nullableIntType.Implements(typeof(Nullable<>)));
    }
}