﻿using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Jay.Text;

namespace Jay.Reflection.Building.Emission;

public class Instruction : IEquatable<Instruction>
{
    private static TextBuilder AppendOffset(TextBuilder builder, Instruction instruction)
    {
        return builder.Append("IL_")
                      .AppendFormat(instruction.Offset, "x4");
    }

    protected static bool OperandEquals(LocalBuilder x, LocalBuilder y)
    {
        return x.IsPinned == y.IsPinned &&
               x.LocalIndex == y.LocalIndex &&
               x.LocalType == y.LocalType;
    }

    protected static bool Equals(Label[] x, Label[] y)
    {
        int len = x.Length;
        if (y.Length != len) return false;
        for (var i = 0; i < len; i++)
        {
            if (x[i] != y[i]) return false;
        }
        return true;
    }

    protected static bool OperandEquals(Type[] x, Type[] y)
    {
        var len = x.Length;
        if (y.Length != len) return false;
        for (int i = 0; i < len; i++)
        {
            if (x[i] != y[i]) return false;
        }
        return true;
    }

    protected static bool OperandEquals(object?[] x, object?[] y)
    {
        var len = x.Length;
        if (y.Length != len) return false;
        for (int i = 0; i < len; i++)
        {
            if (!OperandEquals(x[i], y[i])) return false;
        }
        return true;
    }

    protected static bool OperandEquals(object? x, object? y)
    {
        if (x is null) return y is null;
        if (y is null) return false;
        if (x is byte xByte)
            return y is byte yByte && xByte == yByte;
        if (x is sbyte xSByte)
            return y is sbyte ySByte && xSByte == ySByte;
        if (x is short xShort)
            return y is short yShort && xShort == yShort;
        if (x is int xInt)
            return y is int yInt && xInt == yInt;
        if (x is long xLong)
            return y is long yLong && xLong == yLong;
        if (x is float xFloat)
            return y is float yFloat && Math.Abs(xFloat - yFloat) < float.Epsilon;
        if (x is double xDouble)
            return y is double yDouble && Math.Abs(xDouble - yDouble) < double.Epsilon;
        if (x is string xString)
            return y is string yString && xString == yString;
        if (x is FieldInfo xFieldInfo)
            return y is FieldInfo yFieldInfo && xFieldInfo == yFieldInfo;
        if (x is MethodInfo xMethodInfo)
            return y is MethodInfo yMethodInfo && xMethodInfo == yMethodInfo;
        if (x is ConstructorInfo xConstructorInfo)
            return y is ConstructorInfo yConstructorInfo && xConstructorInfo == yConstructorInfo;
        if (x is Type xType)
            return y is Type yType && xType == yType;
        if (x is LocalBuilder xLocalBuilder)
            return y is LocalBuilder yLocalBuilder && Equals(xLocalBuilder, yLocalBuilder);
        if (x is Label xLabel)
            return y is Label yLabel && xLabel == yLabel;
        if (x is Label[] xLabels)
            return y is Label[] yLabels && Equals(xLabels, yLabels);
        if (x is bool xBool)
            return y is bool yBool && xBool == yBool;
        if (x is Type[] xTypes)
            return y is Type[] yTypes && Equals(xTypes, yTypes);
        if (x is CallingConvention xCallingConvention)
            return y is CallingConvention yCallingConvention && xCallingConvention == yCallingConvention;
        if (x is CallingConventions xCallingConventions)
            return y is CallingConventions yCallingConventions && xCallingConventions == yCallingConventions;
        if (x is object?[] xArray)
            return y is object?[] yArray && Equals(xArray, yArray);

        throw new NotImplementedException();
    }
    
    public int Offset { get; }
    public OpCode OpCode { get; }
    public ILGeneratorMethod GenMethod { get; }
    public object? Arg { get; internal set; }

    public int Size
    {
        get
        {
            int size = OpCode.Size;

            switch (OpCode.OperandType)
            {
                case OperandType.InlineSwitch:
                {
                    if (!(Arg is Instruction[] instructions))
                        throw new InvalidOperationException();
                    size += (1 + instructions.Length) * 4;
                    break;
                }
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    size += 8;
                    break;
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineMethod:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    size += 4;
                    break;
                case OperandType.InlineVar:
                    size += 2;
                    break;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    size += 1;
                    break;
            }

            return size;
        }
    }

    public Instruction(int offset, OpCode opCode, object? arg = null)
    {
        this.Offset = offset;
        this.OpCode = opCode;
        this.GenMethod = ILGeneratorMethod.None;
        this.Arg = arg;
    }
    public Instruction(int offset, ILGeneratorMethod genMethod)
    {
        this.Offset = offset;
        this.OpCode = default;
        this.GenMethod = genMethod;
        this.Arg = null;
    }
    public Instruction(int offset, ILGeneratorMethod genMethod, object? arg)
    {
        this.Offset = offset;
        this.OpCode = default;
        this.GenMethod = genMethod;
        this.Arg = arg;
    }
    public Instruction(int offset, ILGeneratorMethod genMethod, params object?[] args)
    {
        this.Offset = offset;
        this.OpCode = default;
        this.GenMethod = genMethod;
        this.Arg = args;
    }

    public bool Equals(Instruction? instruction)
    {
        return instruction is not null &&
               instruction.Offset == this.Offset &&
               instruction.OpCode == this.OpCode &&
               instruction.GenMethod == this.GenMethod &&
               OperandEquals(instruction.Arg, this.Arg);
    }

    public override bool Equals(object? obj)
    {
        return obj is Instruction instruction && Equals(instruction);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Offset, OpCode, GenMethod, Arg);
    }

    public override string ToString()
    {
        using var text = new TextBuilder();
        AppendOffset(text, this);
        text.Append(": ");
        if (GenMethod == ILGeneratorMethod.None)
        {
            text.Append(OpCode.Name);
            if (Arg is not null)
            {
                text.Append(' ');
                switch (OpCode.OperandType)
                {
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        text.Append((Instruction)Arg);
                        break;
                    case OperandType.InlineSwitch:
                        var labels = (Instruction[])Arg;
                        for (int i = 0; i < labels.Length; i++)
                        {
                            if (i > 0)
                            {
                                text.Append(',');
                            }
                            AppendOffset(text, labels[i]);
                        }

                        break;
                    case OperandType.InlineString:
                        text.Append('\"').Append(Arg).Append('\"');
                        break;
                    default:
                        text.Append(Arg);
                        break;
                }
            }
        }
        else
        {
            text.Append(GenMethod)
                .Append('(')
                // TODO: Break this out
                .Append(Arg)
                .Append(')');
        }

        return text.ToString();
    }
}