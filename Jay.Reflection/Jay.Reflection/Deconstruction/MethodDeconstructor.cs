using System.Diagnostics;
using Jay.Reflection.Emitting;
using Jay.Reflection.Exceptions;
using Jay.Validation;

namespace Jay.Reflection.Deconstruction;

internal sealed class ThisParameter : ParameterInfo
{
    public ThisParameter(MemberInfo member)
    {
        MemberImpl = member;
        ClassImpl = member.DeclaringType;
        NameImpl = "this";
        PositionImpl = 0;
    }
}

public sealed class MethodDeconstructor
{
    private static readonly OpCode[] _oneByteOpCodes;
    private static readonly OpCode[] _twoByteOpCodes;
    
    static MethodDeconstructor()
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

    public static EmissionStream ReadEmissions(MethodBase method)
    {
        return new MethodDeconstructor(method).GetEmissions();
    }
    
    
    private readonly Type[] _methodGenericArguments;
    private readonly Type[] _declaringTypeGenericArguments;

    private readonly byte[] _ilBytes;
    
    public Module Module => Method.Module;
    public MethodBase Method { get; }
    public ParameterInfo[] Parameters { get; }
    public IList<LocalVariableInfo> Locals { get; }
    
    private MethodDeconstructor(MethodBase methodBase)
    {
        Validate.IsNotNull(methodBase);
        this.Method = methodBase;
        MethodBody body = methodBase.GetMethodBody() ?? throw new ArgumentException("Method has no Body", nameof(methodBase));
        this.Locals = body.LocalVariables;
        _ilBytes = body.GetILAsByteArray() ?? throw new ArgumentException("Method Body has no IL Bytes", nameof(methodBase));
        
        // We need to get all parameters, including the implicit This for instance methods
        if (methodBase.IsStatic)
        {
            Parameters = methodBase.GetParameters();
        }
        else
        {
            var methodParams = methodBase.GetParameters();
            Parameters = new ParameterInfo[methodParams.Length + 1];
            Parameters[0] = new ThisParameter(methodBase);
            Easy.CopyTo(methodParams, Parameters.AsSpan(1));
        }
        
        _methodGenericArguments = methodBase.IsGenericMethod ? methodBase.GetGenericArguments() : Type.EmptyTypes;
        
        var ownerType = methodBase.OwnerType();
        _declaringTypeGenericArguments = ownerType.IsGenericType ? ownerType.GetGenericArguments() : Type.EmptyTypes;
    }
    
    private OpCode ReadOpCode(ByteArrayReader ilBytes)
    {
        if (!ilBytes.TryRead(out byte op))
        {
            throw new ReflectedException("Unable to read the first byte of an OpCode");
        }
        // Is not two-byte opcode signifier
        if (op != 0XFE)
        {
            return _oneByteOpCodes[op];
        }
        if (!ilBytes.TryRead(out op))
        {
            throw new ReflectedException("Unable to read the second byte of an OpCode");
        }
        return _twoByteOpCodes[op];
    }
    
    private object GetVariable(OpCode opCode, int index)
    {
        if (opCode.Name!.Contains("loc"))
        {
            return Locals[index];
        }
        else
        {
            return Parameters[index];
        }
    }
    
    private object? ReadOperand(OpCode opcode, ByteArrayReader ilBytes)
    {
        switch (opcode.OperandType)
        {
            case OperandType.InlineSwitch:
                int length = ilBytes.Read<int>();
                int baseOffset = ilBytes.Position + (4 * length);
                int[] branches = new int[length];
                for (int i = 0; i < length; i++)
                {
                    branches[i] = ilBytes.Read<int>() + baseOffset;
                }
                return branches;
            case OperandType.ShortInlineBrTarget:
                return (ilBytes.Read<sbyte>() + ilBytes.Position);
            case OperandType.InlineBrTarget:
                return ilBytes.Read<int>() + ilBytes.Position;
            case OperandType.ShortInlineI:
                if (opcode == OpCodes.Ldc_I4_S)
                    return ilBytes.Read<sbyte>();
                else
                    return ilBytes.Read();
            case OperandType.InlineI:
                return ilBytes.Read<int>();
            case OperandType.ShortInlineR:
                return ilBytes.Read<float>();
            case OperandType.InlineR:
                return ilBytes.Read<double>();
            case OperandType.InlineI8:
                return ilBytes.Read<long>();
            case OperandType.InlineSig:
                return Module.ResolveSignature(ilBytes.Read<int>());
            case OperandType.InlineString:
                return Module.ResolveString(ilBytes.Read<int>());
            case OperandType.InlineField:
            {
                int metadataToken = ilBytes.Read<int>();
                FieldInfo? field = null;
                try
                {
                    field = Module.ResolveField(metadataToken);
                }
                catch (Exception ex)
                {
                    var info = CodePart.ToCode(ex);
                    Debugger.Break();
                }
                
                return field;
            }
            case OperandType.InlineTok:
            case OperandType.InlineType:
            case OperandType.InlineMethod:
            {
                int metadataToken = ilBytes.Read<int>();
                MemberInfo? member = null;
                try
                {
                    member = Module.ResolveMember(metadataToken, _declaringTypeGenericArguments, _methodGenericArguments);
                }
                catch (Exception ex)
                {
                    var info = CodePart.ToCode(ex);
                    Debugger.Break();
                }
                try
                {
                    member = Module.ResolveMember(metadataToken);
                }
                catch (Exception ex)
                {
                    var info = CodePart.ToCode(ex);
                    Debugger.Break();
                }
                return member;
            }
            case OperandType.ShortInlineVar:
                return GetVariable(opcode, ilBytes.Read());
            case OperandType.InlineVar:
                return GetVariable(opcode, ilBytes.Read<short>());
            case OperandType.InlineNone:
            default:
                return null;
        }
    }
    
    public EmissionStream GetEmissions()
    {
        var emissionStream = new EmissionStream();
        ByteArrayReader ilBytes = new ByteArrayReader(_ilBytes);
        while (ilBytes.AvailableByteCount > 0)
        {
            int pos = ilBytes.Position;
            var opCode = ReadOpCode(ilBytes);
            object? operand = ReadOperand(opCode, ilBytes);
            var emission = new OpCodeEmission(opCode, operand);
            var line = new EmissionLine(pos, emission);
            emissionStream.AddLast(line);
        }
        
        // Resolve branches
        foreach (var opInstruction in emissionStream
            .SelectWhere((EmissionLine line, out OpCodeEmission opCodeEmission) => line.Emission.Is(out opCodeEmission!)))
        {
            switch (opInstruction.OpCode.OperandType)
            {
                case OperandType.ShortInlineBrTarget:
                case OperandType.InlineBrTarget:
                {
                    if (!opInstruction.Arg.Is<int>(out var offset))
                        throw new InvalidOperationException();
                    if (!emissionStream.TryFindByOffset(offset, out var line))
                        throw new InvalidOperationException();
                    opInstruction.Arg = line.Emission;
                    break;
                }
                case OperandType.InlineSwitch:
                {
                    if (!opInstruction.Arg.Is<int[]>(out var offsets))
                        throw new InvalidOperationException();
                    var branches = new Emission[offsets.Length];
                    for (int i = 0; i < offsets.Length; i++)
                    {
                        if (!emissionStream.TryFindByOffset(offsets[i], out var line))
                            throw new InvalidOperationException();
                        branches[i] = line.Emission;
                    }
                    opInstruction.Arg = branches;
                    break;
                }
            }
        }
        
        // fin
        return emissionStream;
    }
}