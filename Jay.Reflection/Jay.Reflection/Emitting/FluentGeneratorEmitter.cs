#if !NETSTANDARD2_0
using System.Runtime.InteropServices;
#endif
using Jay.Reflection.Validation;

namespace Jay.Reflection.Emitting;

public sealed class FluentGeneratorEmitter : FluentEmitter<FluentGeneratorEmitter>
{
    private readonly ILGenerator _ilGenerator;
    private readonly List<Label> _labels;
    private readonly List<LocalBuilder> _locals;

    public override int Offset => _ilGenerator.ILOffset;

    public SmartEmitter Smart => new SmartEmitter(this);

    public FluentGeneratorEmitter(ILGenerator ilGenerator)
    {
        _ilGenerator = ilGenerator;
        _labels = new(0);
        _locals = new(0);
    }

    private EmitterLabel CreateEmitLabel(string? lblName, Label label)
    {
        var emitLabel = new EmitterLabel(
            name: GetVariableName(lblName, _labels.Count),
            label: label);
        _labels.Add(label);
        _emitLabels.Add(emitLabel);
        return emitLabel;
    }

    private Label GetLabel(EmitterLabel emitterLabel)
    {
        return _labels
            .Where(emitterLabel.Equals)
            .One();
    }

    private EmitterLocal CreateEmitLocal(string? localName, LocalBuilder localBuilder)
    {
        var emitLocal = new EmitterLocal(
            name: GetVariableName(localName, _locals.Count),
            localBuilder: localBuilder);
        _locals.Add(localBuilder);
        _emitLocals.Add(emitLocal);
        return emitLocal;
    }

    private LocalBuilder GetLocalBuilder(EmitterLocal emitterLocal)
    {
        int index = emitterLocal.Index;
        if ((uint)index < _locals.Count)
            return _locals[index];
        throw new ArgumentOutOfRangeException(
            nameof(emitterLocal),
            emitterLocal,
            CodePart.ToDeclaration(emitterLocal));
    }

#region Emit OpCode

    public override FluentGeneratorEmitter Emit(OpCode opCode)
    {
        _ilGenerator.Emit(opCode);
        return base.Emit(opCode);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, byte arg)
    {
        _ilGenerator.Emit(opCode, arg);
        return base.Emit(opCode, arg);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, sbyte arg)
    {
        _ilGenerator.Emit(opCode, arg);
        return base.Emit(opCode, arg);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, short arg)
    {
        _ilGenerator.Emit(opCode, arg);
        return base.Emit(opCode, arg);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, int arg)
    {
        _ilGenerator.Emit(opCode, arg);
        return base.Emit(opCode, arg);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, long arg)
    {
        _ilGenerator.Emit(opCode, arg);
        return base.Emit(opCode, arg);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, float arg)
    {
        _ilGenerator.Emit(opCode, arg);
        return base.Emit(opCode, arg);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, double arg)
    {
        _ilGenerator.Emit(opCode, arg);
        return base.Emit(opCode, arg);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, string? str)
    {
        _ilGenerator.Emit(opCode, str ?? "");
        return base.Emit(opCode, str);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, EmitterLabel emitterLabel)
    {
        var label = GetLabel(emitterLabel);
        _ilGenerator.Emit(opCode, label);
        return base.Emit(opCode, emitterLabel);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, params EmitterLabel[] emitLabels)
    {
        var labels = Array.ConvertAll(emitLabels, el => GetLabel(el));
        _ilGenerator.Emit(opCode, labels);
        return base.Emit(opCode, emitLabels);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, EmitterLocal emitterLocal)
    {
        var local = GetLocalBuilder(emitterLocal);
        _ilGenerator.Emit(opCode, local);
        return base.Emit(opCode, emitterLocal);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, FieldInfo field)
    {
        _ilGenerator.Emit(opCode, field);
        return base.Emit(opCode, field);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, ConstructorInfo ctor)
    {
        _ilGenerator.Emit(opCode, ctor);
        return base.Emit(opCode, ctor);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, MethodInfo method)
    {
        _ilGenerator.Emit(opCode, method);
        return base.Emit(opCode, method);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, Type type)
    {
        _ilGenerator.Emit(opCode, type);
        return base.Emit(opCode, type);
    }

    public override FluentGeneratorEmitter Emit(OpCode opCode, SignatureHelper signature)
    {
        _ilGenerator.Emit(opCode, signature);
        return base.Emit(opCode, signature);
    }

#endregion

#region Try/Catch/Finally

    public override FluentGeneratorEmitter BeginExceptionBlock(
        out EmitterLabel emitterLabel,
        [CallerArgumentExpression(nameof(emitterLabel))]
        string lblName = "")
    {
        var label = _ilGenerator.BeginExceptionBlock();
        emitterLabel = CreateEmitLabel(lblName, label);
        return Emit(GeneratorEmission.BeginExceptionBlock(emitterLabel));
    }

    public override FluentGeneratorEmitter BeginCatchBlock(Type exceptionType)
    {
        _ilGenerator.BeginCatchBlock(exceptionType);
        return base.BeginCatchBlock(exceptionType);
    }

    public override FluentGeneratorEmitter BeginFinallyBlock()
    {
        _ilGenerator.BeginFinallyBlock();
        return base.BeginFinallyBlock();
    }

    public override FluentGeneratorEmitter BeginExceptFilterBlock()
    {
        _ilGenerator.BeginExceptFilterBlock();
        return base.BeginExceptFilterBlock();
    }

    public override FluentGeneratorEmitter BeginFaultBlock()
    {
        _ilGenerator.BeginFaultBlock();
        return base.BeginFaultBlock();
    }

    public override FluentGeneratorEmitter EndExceptionBlock()
    {
        _ilGenerator.EndExceptionBlock();
        return base.EndExceptionBlock();
    }

#endregion

#region Scope

    public override FluentGeneratorEmitter BeginScope()
    {
        _ilGenerator.BeginScope();
        return base.BeginScope();
    }

    public override FluentGeneratorEmitter EndScope()
    {
        _ilGenerator.EndScope();
        return base.EndScope();
    }

    public override FluentGeneratorEmitter UsingNamespace(string nameSpace)
    {
        _ilGenerator.UsingNamespace(nameSpace);
        return base.UsingNamespace(nameSpace);
    }

#endregion

#region Locals

    public override FluentGeneratorEmitter DeclareLocal(
        Type type,
        bool isPinned,
        out EmitterLocal emitterLocal,
        [CallerArgumentExpression(nameof(emitterLocal))]
        string localName = "")
    {
        var localBuilder = _ilGenerator.DeclareLocal(type, isPinned);
        emitterLocal = CreateEmitLocal(localName, localBuilder);
        return Emit(GeneratorEmission.DeclareLocal(emitterLocal));
    }

#endregion

#region Labels

    public override FluentGeneratorEmitter DefineLabel(
        out EmitterLabel emitterLabel,
        [CallerArgumentExpression(nameof(emitterLabel))]
        string lblName = "")
    {
        var label = _ilGenerator.DefineLabel();
        emitterLabel = CreateEmitLabel(lblName, label);
        return Emit(GeneratorEmission.DefineLabel(emitterLabel));
    }

    public override FluentGeneratorEmitter MarkLabel(EmitterLabel emitterLabel)
    {
        var label = GetLabel(emitterLabel);
        _ilGenerator.MarkLabel(label);
        return Emit(GeneratorEmission.MarkLabel(emitterLabel));
    }

#endregion

#region Method Calling

    public override FluentGeneratorEmitter EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes)
    {
        _ilGenerator.EmitCall(
            methodInfo.GetCallOpCode(),
            methodInfo,
            optionalParameterTypes);
        return base.EmitCall(methodInfo, optionalParameterTypes);
    }

    public override FluentGeneratorEmitter EmitCalli(
        CallingConventions callingConventions, Type? returnType,
        Type[]? parameterTypes, Type[]? optionalParameterTypes)
    {
        _ilGenerator.EmitCalli(
            OpCodes.Calli,
            callingConventions,
            returnType,
            parameterTypes,
            optionalParameterTypes);
        return base.EmitCalli(
            callingConventions,
            returnType,
            parameterTypes,
            optionalParameterTypes);
    }
#if !NETSTANDARD2_0
    public override FluentGeneratorEmitter EmitCalli(CallingConvention callingConvention, Type? returnType, Type[]? parameterTypes)
    {
        _ilGenerator.EmitCalli(OpCodes.Calli, callingConvention, returnType, parameterTypes);
        return base.EmitCalli(callingConvention, returnType, parameterTypes);
    }
#endif

#endregion

#region ILGenerator

    public FluentGeneratorEmitter WriteLine(string? text)
    {
        _ilGenerator.EmitWriteLine(text ?? "");
        return Emit(GeneratorEmission.WriteLine(text ?? ""));
    }

    public FluentGeneratorEmitter WriteLine(FieldInfo fieldInfo)
    {
        _ilGenerator.EmitWriteLine(fieldInfo);
        return Emit(GeneratorEmission.WriteLine(fieldInfo));
    }

    public FluentGeneratorEmitter WriteLine(EmitterLocal emitterLocal)
    {
        var local = GetLocalBuilder(emitterLocal);
        _ilGenerator.EmitWriteLine(local);
        return Emit(GeneratorEmission.WriteLine(emitterLocal));
    }

    public override FluentGeneratorEmitter ThrowException(Type exceptionType)
    {
        ValidateType.IsExceptionType(exceptionType);
        _ilGenerator.ThrowException(exceptionType);
        return Emit(GeneratorEmission.ThrowException(exceptionType));
    }

#endregion
}