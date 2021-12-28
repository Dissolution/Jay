using System.Reflection.Emit;
using System.Text;

using Jay.Extensions;
using Jay.Text;

namespace Jay.Reflection.Building.Emission;

public class InstructionStream : LinkedList<IInstruction>
{
    public IOpCodeInstruction? FindOpCodeInstructionWithOffset(int offset)
    {
        if (offset < 0 || this.Count == 0)
            return null;
        foreach (var node in this.OfType<IOpCodeInstruction>())
        {
            if (node.Offset == offset)
                return node;
            if (node.Offset > offset)
                return null;
        }
        return null;
    }
}

public enum ILGeneratorMethod
{
    None = 0,

    MarkLabel, // arg = Label
}

public interface IInstructionFactory
{

}

public interface IOpCodeInstructionFactory : IInstructionFactory
{
    IOpCodeInstruction Create(OpCode opCode, object? operand = null);
}

public interface IGeneratorInstructionFactory : IInstructionFactory
{
    IGeneratorInstruction Create(ILGeneratorMethod method, object? arg = null);
}

public interface IInstruction : IEquatable<IInstruction>, IRenderable
{

}

public interface IOpCodeInstruction : IInstruction, IEquatable<IOpCodeInstruction>
{
    int Offset { get; }
    OpCode OpCode { get; }
    object? Operand { get; }
}

public interface IGeneratorInstruction : IInstruction
{
    ILGeneratorMethod Method { get; }
    object? Arg { get; }
}

internal class GeneratorInstructionFactory : IGeneratorInstructionFactory
{
    protected readonly ILGenerator _ilGenerator;

    public GeneratorInstructionFactory(ILGenerator ilGenerator)
    {
        _ilGenerator = ilGenerator;
    }

    public IGeneratorInstruction Create(ILGeneratorMethod method, object? arg = null)
    {
        switch (method)
        {
            case ILGeneratorMethod.None:
                throw new NotImplementedException();
            case ILGeneratorMethod.MarkLabel:
            {
                if (arg is not Label lbl)
                    throw new ArgumentException("MarkLabel must be accompanied by a Label", nameof(arg));
                return new MarkLabelInstruction(lbl);
            }
            default:
                throw new NotImplementedException();
        }
    }
}

internal sealed class OpCodeInstruction : IOpCodeInstruction
{
    private static TextBuilder AppendLabel(TextBuilder builder, IOpCodeInstruction instruction)
    {
        return builder.Append("IL_")
               .Append(instruction.Offset.ToString("x4"));
    }

    public int Offset { get; }
    public OpCode OpCode { get; }
    public object? Operand { get; internal set; }

    public int Size
    {
        get
        {
            int size = OpCode.Size;

            switch (OpCode.OperandType)
            {
                case OperandType.InlineSwitch:
                    {
                        if (!(Operand is IInstruction[] instructions))
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

    public bool Equals(IInstruction? instruction)
    {
        return instruction is IOpCodeInstruction opCodeInstruction && Equals(opCodeInstruction);
    }

    public bool Equals(IOpCodeInstruction? instruction)
    {
        return instruction is not null &&
               instruction.Offset == this.Offset &&
               instruction.OpCode == this.OpCode &&
               Equals(instruction.Operand, this.Operand);
    }

    public void Render(TextBuilder text)
    {
        AppendLabel(text, this).Append(": ").Append(OpCode.Name);
        if (Operand is not null)
        {
            text.Append(' ');

            switch (OpCode.OperandType)
            {
                case OperandType.ShortInlineBrTarget:
                case OperandType.InlineBrTarget:
                    AppendLabel(text, (IOpCodeInstruction)Operand);
                    break;
                case OperandType.InlineSwitch:
                    var labels = (IOpCodeInstruction[])Operand;
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
    }

    public override bool Equals(object? obj)
    {
        if (obj is IOpCodeInstruction instruction)
            return Equals(instruction);
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Offset, OpCode, Operand);
    }
}

public abstract class GeneratorInstruction : IGeneratorInstruction
{
    public ILGeneratorMethod Method { get; }
    public object? Arg { get; }

    protected GeneratorInstruction(ILGeneratorMethod method, object? arg)
    {
        Method = method;
        Arg = arg;
    }

    public bool Equals(IInstruction? instruction)
    {
        return instruction is GeneratorInstruction generatorInstruction && 
               generatorInstruction.Method == this.Method &&
               Equals(generatorInstruction.Arg, this.Arg);
    }

    public virtual void Render(TextBuilder builder)
    {
        builder.Append(Method).Append('(').Append(Arg).Append(')');
    }
}

public sealed class MarkLabelInstruction : GeneratorInstruction
{
    public Label Label { get; }

    public MarkLabelInstruction(Label label)
        : base(ILGeneratorMethod.MarkLabel, label)
    {
        this.Label = label;
    }

    public override void Render(TextBuilder builder)
    {
        builder.Append("MarkLabel IL_").AppendFormat(Label.GetHashCode(), "X4");
    }
}