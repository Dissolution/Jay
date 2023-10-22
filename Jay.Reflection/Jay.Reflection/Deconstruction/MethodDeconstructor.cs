using System.Diagnostics;
using Jay.Memory;
using Jay.Reflection.Emitting;

namespace Jay.Reflection.Deconstruction;

public class MethodDeconstructor
{
    private static readonly OpCode[] _oneByteOpCodes;
    private static readonly OpCode[] _twoByteOpCodes;

    static MethodDeconstructor()
    {
        _oneByteOpCodes = new OpCode[225];
        _twoByteOpCodes = new OpCode[31];

        var opCodesFields = typeof(OpCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in opCodesFields)
        {
            var fieldValue = field.GetValue(null);
            if (fieldValue.Is<OpCode>(out var opCode))
            {
                if (opCode.OpCodeType == OpCodeType.Nternal)
                {
                    Debugger.Break();
                    continue;
                }

                if (opCode.Size == 1)
                {
                    _oneByteOpCodes[opCode.Value] = opCode;
                }
                else
                {
                    Debug.Assert(opCode.Size == 2);
                    _twoByteOpCodes[opCode.Value & 0xFF] = opCode;
                }
            }
        }
    }

    private static OpCode ReadOpCode(ref SpanReader<byte> ilBytes)
    {
        // Check the first byte
        if (!ilBytes.TryTake(out byte op))
            throw new InvalidOperationException("Could not read the first OpCode byte");
        
        // Check for two-byte signifier
        if (op != 0xFE)
        {
            return _oneByteOpCodes[op];
        }
        
        // Need the second byte
        if (!ilBytes.TryTake(out op))
            throw new InvalidOperationException("Could not read the second OpCode byte");

        return _twoByteOpCodes[op];
    }

    private static object? ReadOperand(MethodDeconstruction methodDeconstruction, OpCode opCode, ref SpanReader<byte> ilBytes)
    {
        var module = methodDeconstruction.Method.Module;
        switch (opCode.OperandType)
        {
            case OperandType.InlineField:
            {
                int metadataToken = ilBytes.Take<int>();
                FieldInfo? field = module.ResolveField(metadataToken);
                return field;
            }
            case OperandType.InlineMethod:
            {
                int metadataToken = ilBytes.Take<int>();
                MethodBase? method = module.ResolveMethod(metadataToken);
                return method;
            }
            case OperandType.InlineTok:
            {
                int metadataToken = ilBytes.Take<int>();
                MemberInfo? member = module.ResolveMember(metadataToken);
                return member;
            }
            case OperandType.InlineType:
            {
                int metadataToken = ilBytes.Take<int>();
                Type type = module.ResolveType(metadataToken);
                return type;
            }
            case OperandType.InlineSig:
            {
                int metadataToken = ilBytes.Take<int>();
                byte[] signature = module.ResolveSignature(metadataToken);
                return signature;
            }
            case OperandType.InlineString:
            {
                int metadataToken = ilBytes.Take<int>();
                string str = module.ResolveString(metadataToken);
                return str;
            }
            case OperandType.InlineVar:
            {
                short index = ilBytes.Take<short>();
                var variable = GetVariable(methodDeconstruction, opCode, index);
                return variable;
            }
            case OperandType.ShortInlineVar:
            {
                sbyte index = ilBytes.Take<sbyte>();
                var variable = GetVariable(methodDeconstruction, opCode, index);
                return variable;
            }
            case OperandType.InlineI:
                return ilBytes.Take<int>();
            case OperandType.ShortInlineI:
                return ilBytes.Take<sbyte>();
            case OperandType.InlineI8:
                return ilBytes.Take<long>();
            case OperandType.ShortInlineR:
                return ilBytes.Take<float>();
            case OperandType.InlineR:
                return ilBytes.Take<double>();
            case OperandType.InlineBrTarget:
                return ilBytes.Take<int>() + ilBytes.ReadCount;
            case OperandType.ShortInlineBrTarget:
                return ilBytes.Take<sbyte>() + ilBytes.ReadCount;
            case OperandType.InlineSwitch:
            {
                int count = ilBytes.Take<int>();
                int offset = ilBytes.ReadCount + (4 * count);
                int[] branches = new int[count];
                for (var i = 0; i < count; i++)
                {
                    branches[i] = ilBytes.Take<int>() + offset;
                }
                return branches;
            }
            case OperandType.InlineNone:
            default:
            {
                // No operand
                return null;
            }
        }
    }

    private static object GetVariable(MethodDeconstruction methodDeconstruction, OpCode opCode, int index)
    {
        if (opCode.Name!.Contains("loc"))
        {
            return methodDeconstruction.Locals[index];
        }
        else
        {
            return methodDeconstruction.Parameters[index];
        }
    }
    
    public static MethodDeconstruction Deconstruct(MethodBase method)
    {
        MethodBody? body = method.GetMethodBody();
        if (body is null)
            throw new ArgumentException("Cannot get MethodBody", nameof(method));
        byte[]? ilBytes = body.GetILAsByteArray();
        if (ilBytes is null)
            throw new ArgumentException("Cannot get IL Bytes", nameof(method));
        ParameterInfo[] parameters;
        if (method.IsStatic)
        {
            parameters = method.GetParameters();
        }
        else
        {
            var methodParameters = method.GetParameters();
            parameters = new ParameterInfo[methodParameters.Length + 1];
            parameters[0] = new ThisParameter(method);
            Easy.CopyTo(methodParameters, parameters.AsSpan(1));
        }
        var locals = body.LocalVariables;

        var methodDeconstruction = new MethodDeconstruction(method, body, ilBytes, parameters, locals.ToList());

        SpanReader<byte> ilReader = new(ilBytes);
        var emissions = new EmissionStream();
        
        // Read all of our emissions
        while (ilReader.UnreadCount > 0)
        {
            int pos = ilReader.ReadCount;
            var opCode = ReadOpCode(ref ilReader);
            object? operand = ReadOperand(methodDeconstruction, opCode, ref ilReader);
            var emission = new OpCodeEmission(opCode, operand);
            var line = new EmissionLine(pos, emission);
            emissions.AddLast(line);
        }
        
        // Now we resolve our branches
        var branchEmissions = emissions
            .SelectWhere((EmissionLine line, out OpCodeEmission opCodeEmission) => line.Emission.Is(out opCodeEmission!))
            .Where(emission => emission.OpCode.OperandType.HasAnyFlags(OperandType.ShortInlineBrTarget, OperandType.InlineBrTarget, OperandType.InlineSwitch))
            .ToList();
        foreach (var opEmission in branchEmissions)
        {
            if (opEmission.OpCode.OperandType == OperandType.ShortInlineBrTarget)
            {
                sbyte offset = opEmission.Arg.AsValid<sbyte>();
                if (!emissions.TryFindByOffset(offset, out var line))
                    throw new InvalidOperationException();
                opEmission.Arg = line.Emission;
            }
            else if (opEmission.OpCode.OperandType == OperandType.InlineBrTarget)
            {
                int offset = opEmission.Arg.AsValid<int>();
                if (!emissions.TryFindByOffset(offset, out var line))
                    throw new InvalidOperationException();
                opEmission.Arg = line.Emission;
            }
            else if (opEmission.OpCode.OperandType == OperandType.InlineSwitch)
            {
                int[] offsets = opEmission.Arg.AsValid<int[]>();
                var branches = new Emission[offsets.Length];
                for (int i = 0; i < offsets.Length; i++)
                {
                    if (!emissions.TryFindByOffset(offsets[i], out var line))
                        throw new InvalidOperationException();
                    branches[i] = line.Emission;
                }
                opEmission.Arg = branches;
                break;
            }
        }

        methodDeconstruction.Emissions = emissions;
        return methodDeconstruction;
    }
}