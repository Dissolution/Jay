using System.Reflection.Emit;
using System.Text;

using Jay.Extensions;

namespace Jay.Reflection.Building.Emission;

public class InstructionStream : LinkedList<Instruction>
{
    public Instruction? FindInstruction(int offset)
    {
        if (offset < 0 || this.Count == 0)
            return null;
        var last = this.Last!;
        if (offset > last.Value.Offset)
            return null;
        var first = this.First!;
        if (offset - first.Value.Offset <= last.Value.Offset - offset)
        {
            var node = first;
            while (node is not null)
            {
                if (node.Value.Offset == offset)
                    return node.Value;
                node = node.Next;
            }
        }
        else
        {
            var node = last;
            while (node is not null)
            {
                if (node.Value.Offset == offset)
                    return node.Value;
                node = node.Previous;
            }
        }
        return null;
    }
}

public enum ILGeneratorMethod
{
    None = 0,
}

public interface IInstructionFactory
{
    public Instruction Create(OpCode opCode, object? operand = null);
    public Instruction Create(ILGeneratorMethod method, object? arg = null);
}

public sealed class Instruction
{
    private static void AppendLabel(StringBuilder builder, Instruction instruction)
    {
        if (instruction.Offset.TryGetValue(out var offset))
        {
            builder.Append("IL_")
                .Append(offset.ToString("x4"));
        }
        else
        {
            builder.Append("       ");   // 7 spaces
        }
    }

    public int? Offset { get; }
    public OpCode OpCode { get; }
    public ILGeneratorMethod ILGeneratorMethod { get; }
    public object? Operand { get; internal set; }

    public int Size
    {
        get
        {
            if (ILGeneratorMethod != ILGeneratorMethod.None)
                return 0;
            int size = OpCode.Size;

            switch (OpCode.OperandType)
            {
                case OperandType.InlineSwitch:
                    {
                        if (!(Operand is Instruction[] instructions))
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

    internal Instruction(int offset, OpCode opCode, object? operand = null)
    {
        this.Offset = offset;
        this.OpCode = opCode;
        this.ILGeneratorMethod = ILGeneratorMethod.None;
        this.Operand = operand;
    }
    internal Instruction(ILGeneratorMethod ilGeneratorMethod, object? operand = null)
    {
        this.Offset = null;
        this.OpCode = default;
        this.ILGeneratorMethod = ilGeneratorMethod;
        this.Operand = operand;
    }

    public override string ToString()
    {
        var instruction = new StringBuilder();

        AppendLabel(instruction, this);
        instruction.Append(':');
        instruction.Append(' ');
        if (ILGeneratorMethod != ILGeneratorMethod.None)
        {
            instruction.Append(ILGeneratorMethod)
                .Append('(');
            if (Operand is Array array)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    if (i > 0)
                    {
                        instruction.Append(',');
                    }
                    instruction.Append(array.GetValue(i));
                }
            }
            else
            {
                instruction.Append(Operand);
            }
        }
        else
        {
            instruction.Append(OpCode.Name);

            if (Operand is not null)
            {
                instruction.Append(' ');

                switch (OpCode.OperandType)
                {
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        AppendLabel(instruction, (Instruction)Operand);
                        break;
                    case OperandType.InlineSwitch:
                        var labels = (Instruction[])Operand;
                        for (int i = 0; i < labels.Length; i++)
                        {
                            if (i > 0)
                                instruction.Append(',');

                            AppendLabel(instruction, labels[i]);
                        }

                        break;
                    case OperandType.InlineString:
                        instruction.Append('\"');
                        instruction.Append(Operand);
                        instruction.Append('\"');
                        break;
                    default:
                        instruction.Append(Operand);
                        break;
                }
            }
        }

        return instruction.ToString();
    }
}