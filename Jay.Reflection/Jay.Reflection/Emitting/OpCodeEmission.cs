namespace Jay.Reflection.Emitting;

public sealed class OpCodeEmission : Emission
{
    public OpCode OpCode { get; }

    public override int Size
    {
        get
        {
            int size = OpCode.Size;
            switch (OpCode.OperandType)
            {
                case OperandType.InlineSwitch:
                {
                    if (!(Arg is Emission[] instructions))
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
                default:
                    break;
            }
            return size;
        }
    }

    public OpCodeEmission(OpCode opCode)
        : base(opCode.Name!)
    {
        this.OpCode = opCode;
    }
    
    public OpCodeEmission(OpCode opCode, object? arg)
        : base(opCode.Name!, arg)
    {
        this.OpCode = opCode;
    }

    public override void WriteCodeTo(CodeBuilder codeBuilder)
    {
        codeBuilder.Append(Name);

        if (HasArgs)
        {
            codeBuilder.Append("    ");
            codeBuilder.Append(this.Arg);
        }
    }
}