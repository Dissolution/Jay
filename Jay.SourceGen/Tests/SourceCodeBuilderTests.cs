﻿using System.Diagnostics;
using Jay.Reflection.CodeBuilding;
using Xunit;

namespace Jay.SourceGen.Tests;

public class SourceCodeBuilderTests
{
    [Fact]
    public void AnythingWorks()
    {
        using var code = new CodeBuilder();
        code.AutoGeneratedHeader()
            .Using("System.Text")
            .Namespace("Jay.Testing")
            .WriteLine("public static class SuperThing")
            .BracketBlock(
                classBlock =>
                {
                    classBlock.Write("public SuperThing(")
                        .Code(typeof(int))
                        .Write(" id = ")
                        .Code(147)
                        .Write(", ")
                        .Code(typeof(string))
                        .Code(" name = ")
                        .Code("Joe")
                        .WriteLine(')')
                        .BracketBlock(
                            ctorBlock =>
                            {
                                ctorBlock.WriteLine("throw new NotImplementedException();");
                            })
                        .NewLine();
                })
            .NewLine();

        string str = code.ToString();
        Debugger.Break();
        Assert.True(true);
    }
}