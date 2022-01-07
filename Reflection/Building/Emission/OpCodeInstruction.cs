using System.Reflection.Emit;
using Jay.Text;

namespace Jay.Reflection.Emission;

public sealed class OpCodeInstruction : Instruction, IEquatable<OpCodeInstruction>
{
    private static TextBuilder AppendLabel(TextBuilder builder, OpCodeInstruction instruction)
    {
        return builder.Append("IL_")
            .Append(instruction.Offset.ToString("x4"));
    }

    private static bool Equals(Label[] x, Label[] y)
    {
        int len = x.Length;
        if (y.Length != len) return false;
        for (var i = 0; i < len; i++)
        {
            if (x[i] != y[i]) return false;
        }
        return true;
    }

    private static bool OperandEquals(object? x, object? y)
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
        // We cannot deal with this type of value
        throw new NotImplementedException();
        return false;
    }

    public int Offset { get; }
    public OpCode OpCode { get; }
    public object? Operand { get; set; }

    public int Size
    {
        get
        {
            int size = OpCode.Size;

            switch (OpCode.OperandType)
            {
                case OperandType.InlineSwitch:
                {
                    if (!(Operand is OpCodeInstruction[] instructions))
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

    public OpCodeInstruction(int offset, OpCode opCode, object? operand = null)
    {
        this.Offset = offset;
        this.OpCode = opCode;
        this.Operand = operand;
    }

    public bool Equals(OpCodeInstruction? opCodeInstruction)
    {
        return opCodeInstruction is not null &&
               opCodeInstruction.Offset == this.Offset &&
               opCodeInstruction.OpCode == this.OpCode &&
               OperandEquals(opCodeInstruction.Operand, this.Operand);
    }

    public override bool Equals(Instruction? instruction)
    {
        return instruction is OpCodeInstruction oci && Equals(oci);
    }

    public override bool Equals(object? obj)
    {
        return obj is OpCodeInstruction oci && Equals(oci);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Offset, OpCode, Operand);
    }

    public override string ToString()
    {
        using var text = new TextBuilder();
        AppendLabel(text, this).Append(": ").Append(OpCode.Name);
        if (Operand is not null)
        {
            text.Append(' ');

            switch (OpCode.OperandType)
            {
                case OperandType.ShortInlineBrTarget:
                case OperandType.InlineBrTarget:
                    AppendLabel(text, (OpCodeInstruction)Operand);
                    break;
                case OperandType.InlineSwitch:
                    var labels = (OpCodeInstruction[])Operand;
                    for (int i = 0; i < labels.Length; i++)
                    {
                        if (i > 0)
                        {
                            text.Append(',');
                        }
                        AppendLabel(text, labels[i]);
                    }

                    break;
                case OperandType.InlineString:
                    text.Append('\"').Append(Operand).Append('\"');
                    break;
                default:
                    text.Append(Operand);
                    break;
            }
        }
        return text.ToString();
    }
}