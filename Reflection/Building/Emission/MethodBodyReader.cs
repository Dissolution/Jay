using System.Reflection.Emit;

namespace Jay.Reflection.Emission;

public class MethodBodyReader
{
    private static readonly OpCode[] _oneByteOpCodes;
    private static readonly OpCode[] _twoByteOpCodes;

    static MethodBodyReader()
    {
        _oneByteOpCodes = new OpCode[0xE1];
        _twoByteOpCodes = new OpCode[0x1F];

        var fields = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            var opCode = (OpCode)field.GetValue(null)!;
            if (opCode.OpCodeType == OpCodeType.Nternal)
                continue;

            if (opCode.Size == 1)
                _oneByteOpCodes[opCode.Value] = opCode;
            else
                _twoByteOpCodes[opCode.Value & 0xFF] = opCode;
        }
    }

    private readonly MethodBase _method;
    private readonly MethodBody _body;
    private readonly Module _module;
    private readonly Type[] _typeArguments;
    private readonly Type[] _methodArguments;
    private readonly ByteBuffer _il;
    private readonly ParameterInfo _thisParameter;
    private readonly ParameterInfo[] _parameters;
    private readonly IList<LocalVariableInfo> _locals;
    private readonly InstructionStream<OpCodeInstruction> _instructions;

    public MethodBodyReader(MethodBase method)
    {
        this._method = method;

        this._body = method.GetMethodBody() ?? throw new ArgumentException("Method has no body");
        
        byte[] bytes = _body.GetILAsByteArray() ?? throw new ArgumentException("Can not get the body of the method");
        
        if (!(method is ConstructorInfo))
            _methodArguments = method.GetGenericArguments();

        if (method.DeclaringType != null)
            _typeArguments = method.DeclaringType.GetGenericArguments();

        if (!method.IsStatic)
            _thisParameter = new ThisParameter(method);
        _parameters = method.GetParameters();
        _locals = _body.LocalVariables;
        _module = method.Module;
        _il = new ByteBuffer(bytes);
        _instructions = new(); //(bytes.Length + 1) / 2);
    }

    private void ReadInstructions()
    {
        while (_il.Position < _il.Length)
        {
            var instruction = new OpCodeInstruction(_il.Position, ReadOpCode());

            ReadOperand(instruction);

            _instructions.AddLast(instruction);
        }

        ResolveBranches();
    }

    private void ReadOperand(OpCodeInstruction instruction)
    {
        switch (instruction.OpCode.OperandType)
        {
            case OperandType.InlineNone:
                break;
            case OperandType.InlineSwitch:
                int length = _il.Read<int>();
                int baseOffset = _il.Position + (4 * length);
                int[] branches = new int[length];
                for (int i = 0; i < length; i++)
                    branches[i] = _il.Read<int>() + baseOffset;

                instruction.Operand = branches;
                break;
            case OperandType.ShortInlineBrTarget:
                instruction.Operand = (((sbyte)_il.ReadByte()) + _il.Position);
                break;
            case OperandType.InlineBrTarget:
                instruction.Operand = _il.Read<int>() + _il.Position;
                break;
            case OperandType.ShortInlineI:
                if (instruction.OpCode == OpCodes.Ldc_I4_S)
                    instruction.Operand = (sbyte)_il.ReadByte();
                else
                    instruction.Operand = _il.ReadByte();
                break;
            case OperandType.InlineI:
                instruction.Operand = _il.Read<int>();
                break;
            case OperandType.ShortInlineR:
                instruction.Operand = _il.Read<float>();
                break;
            case OperandType.InlineR:
                instruction.Operand = _il.Read<double>();
                break;
            case OperandType.InlineI8:
                instruction.Operand = _il.Read<long>();
                break;
            case OperandType.InlineSig:
                instruction.Operand = _module.ResolveSignature(_il.Read<int>());
                break;
            case OperandType.InlineString:
                instruction.Operand = _module.ResolveString(_il.Read<int>());
                break;
            case OperandType.InlineTok:
            case OperandType.InlineType:
            case OperandType.InlineMethod:
            case OperandType.InlineField:
                instruction.Operand = _module.ResolveMember(_il.Read<int>(), _typeArguments, _methodArguments);
                break;
            case OperandType.ShortInlineVar:
                instruction.Operand = GetVariable(instruction, _il.ReadByte());
                break;
            case OperandType.InlineVar:
                instruction.Operand = GetVariable(instruction, _il.Read<short>());
                break;
            default:
                throw new NotSupportedException();
        }
    }

    private void ResolveBranches()
    {
        foreach (var instruction in _instructions)
        {
            switch (instruction.OpCode.OperandType)
            {
                case OperandType.ShortInlineBrTarget:
                case OperandType.InlineBrTarget:
                    instruction.Operand = _instructions.FindWithOffset((int)instruction.Operand!);
                    break;
                case OperandType.InlineSwitch:
                    var offsets = (int[])instruction.Operand;
                    var branches = new OpCodeInstruction[offsets.Length];
                    for (int j = 0; j < offsets.Length; j++)
                        {
                        branches[j] = _instructions.FindWithOffset(offsets[j]);
                    }

                    instruction.Operand = branches;
                    break;
            }
        }
    }


    private object GetVariable(OpCodeInstruction instruction, int index)
    {
        return TargetsLocalVariable(instruction.OpCode)
            ? (object)GetLocalVariable(index)
            : (object)GetParameter(index);
    }

    private static bool TargetsLocalVariable(OpCode opCode)
    {
        return opCode.Name!.Contains("loc");
    }

    private LocalVariableInfo GetLocalVariable(int index)
    {
        return _locals[index];
    }

    private ParameterInfo GetParameter(int index)
    {
        if (_method.IsStatic)
            return _parameters[index];

        if (index == 0)
            return _thisParameter;

        return _parameters[index - 1];
    }

    private OpCode ReadOpCode()
    {
        byte op = _il.ReadByte();
        return op != 0xfe
            ? _oneByteOpCodes[op]
            : _twoByteOpCodes[_il.ReadByte()];
    }

    public static InstructionStream<OpCodeInstruction> GetInstructions(MethodBase method)
    {
        var reader = new MethodBodyReader(method);
        reader.ReadInstructions();
        return reader._instructions;
    }

    public sealed class ThisParameter : ParameterInfo
    {
        public ThisParameter(MethodBase method)
        {
            this.MemberImpl = method;
            this.ClassImpl = method.DeclaringType;
            this.NameImpl = "this";
            this.PositionImpl = -1;
        }
    }
}