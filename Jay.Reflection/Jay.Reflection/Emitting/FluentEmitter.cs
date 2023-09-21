using System.Diagnostics;
using System.Runtime.InteropServices;
using Jay.Reflection.Caching;
using Jay.Reflection.Searching;
using Jay.Reflection.Validation;


// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable IdentifierTypo

namespace Jay.Reflection.Emitting;

public class FluentEmitter<TSelf> : ICodePart
    where TSelf : FluentEmitter<TSelf>
{
    [return: NotNullIfNotNull(nameof(name))]
    protected static string GetVariableName(string? name, int count)
    {
        if (string.IsNullOrWhiteSpace(name))
            return count.ToString();

        // Fix 'out var XYZ' having 'var XYZ' as a name
        var i = name!.LastIndexOf(' ');
        if (i >= 0)
        {
            return name[(i + 1)..];
        }
        return name;
    }


    protected readonly TSelf _self;
    protected readonly List<EmitterLabel> _emitLabels;
    protected readonly List<EmitterLocal> _emitLocals;

    public EmissionStream Emissions { get; }

    public virtual int Offset => Emissions.Count;

    protected FluentEmitter()
    {
        _self = (TSelf)this;
        _emitLabels = new(0);
        _emitLocals = new(0);
        this.Emissions = new();
    }

    protected EmitterLabel CreateEmitLabel(string? lblName)
    {
        int lblCount = _emitLabels.Count;
        var emitLabel = new EmitterLabel(
            name: GetVariableName(lblName, lblCount),
            position: lblCount);
        _emitLabels.Add(emitLabel);
        return emitLabel;
    }

    protected EmitterLocal CreateEmitLocal(string? localName, Type type, bool isPinned = false)
    {
        int localCount = _emitLocals.Count;
        var emitLocal = new EmitterLocal(
            name: GetVariableName(localName, localCount),
            index: (ushort)localCount,
            type: type,
            isPinned: isPinned);
        _emitLocals.Add(emitLocal);
        return emitLocal;
    }

    public TSelf Emit(Emission emission)
    {
        this.Emissions.AddLast(new EmissionLine(this.Offset, emission));
        return _self;
    }

#region Emit OpCode
    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_"/>
    public virtual TSelf Emit(OpCode opCode) => Emit(new OpCodeEmission(opCode));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="byte"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Byte_"/>
    public virtual TSelf Emit(OpCode opCode, byte arg) => Emit(new OpCodeEmission(opCode, arg));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="sbyte"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_SByte_"/>
    public virtual TSelf Emit(OpCode opCode, sbyte arg) => Emit(new OpCodeEmission(opCode, arg));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="short"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int16_"/>
    public virtual TSelf Emit(OpCode opCode, short arg) => Emit(new OpCodeEmission(opCode, arg));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="int"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int32_"/>
    public virtual TSelf Emit(OpCode opCode, int arg) => Emit(new OpCodeEmission(opCode, arg));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="long"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int64_"/>
    public virtual TSelf Emit(OpCode opCode, long arg) => Emit(new OpCodeEmission(opCode, arg));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="float"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Single_"/>
    public virtual TSelf Emit(OpCode opCode, float arg) => Emit(new OpCodeEmission(opCode, arg));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="double"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Double_"/>
    public virtual TSelf Emit(OpCode opCode, double arg) => Emit(new OpCodeEmission(opCode, arg));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="string"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="str">The <see cref="string"/>to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_String_"/>
    public virtual TSelf Emit(OpCode opCode, string? str) => Emit(new OpCodeEmission(opCode, str));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream and leaves space to include a <see cref="Label"/> when fixes are done.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="emitterLabel">The <see cref="Label"/> to branch from this location.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_Label_"/>
    public virtual TSelf Emit(OpCode opCode, EmitterLabel emitterLabel) => Emit(new OpCodeEmission(opCode, emitterLabel));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream and leaves space to include a <see cref="Label"/> when fixes are done.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="emitLabels">The <see cref="Label"/>s of which to branch to from this locations.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="emitLabels"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException">If <paramref name="emitLabels"/> is empty.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_Label___"/>
    public virtual TSelf Emit(OpCode opCode, params EmitterLabel[] emitLabels) => Emit(new OpCodeEmission(opCode, emitLabels));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the index of the given <see cref="LocalBuilder"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="emitterLocal">The <see cref="LocalBuilder"/> to emit the index of.</param>
    /// <exception cref="InvalidOperationException">If <paramref name="opCode"/> is a single-byte instruction and <paramref name="emitterLocal"/> has an index greater than <see cref="byte.MaxValue"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_LocalBuilder_"/>
    public virtual TSelf Emit(OpCode opCode, EmitterLocal emitterLocal) => Emit(new OpCodeEmission(opCode, emitterLocal));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="FieldInfo"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="field">The <see cref="ArgumentNullException"/> to emit.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_FieldInfo_"/>
    public virtual TSelf Emit(OpCode opCode, FieldInfo field) => Emit(new OpCodeEmission(opCode, field));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="ConstructorInfo"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="ctor">The <see cref="ConstructorInfo"/> to emit.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="ctor"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_ConstructorInfo_"/>
    public virtual TSelf Emit(OpCode opCode, ConstructorInfo ctor) => Emit(new OpCodeEmission(opCode, ctor));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="method">The <see cref="MethodInfo"/> to emit.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException">If <paramref name="method"/> is a generic method for which <see cref="MethodBase.IsGenericMethodDefinition"/> is <see langword="false"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_MethodInfo_"/>
    public virtual TSelf Emit(OpCode opCode, MethodInfo method) => Emit(new OpCodeEmission(opCode, method));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="Type"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="type">The <see cref="Type"/> to emit.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Type_"/>
    public virtual TSelf Emit(OpCode opCode, Type type) => Emit(new OpCodeEmission(opCode, type));

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="SignatureHelper"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="signature">A helper for constructing a signature token.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="signature"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_SignatureHelper_"/>
    public virtual TSelf Emit(OpCode opCode, SignatureHelper signature) => Emit(new OpCodeEmission(opCode, signature));
#endregion

#region Try/Catch/Finally
    /// <summary>
    /// Begins an exception block for a non-filtered exception.
    /// </summary>
    /// <param name="emitterLabel">
    /// The <see cref="Label"/> for the end of the block.
    /// This will leave you in the correct place to execute <see langword="finally"/> blocks or to finish the <see langword="try"/>.
    /// </param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptionblock"/>
    public virtual TSelf BeginExceptionBlock(
        out EmitterLabel emitterLabel,
        [CallerArgumentExpression(nameof(emitterLabel))]
        string lblName = "")
    {
        emitterLabel = CreateEmitLabel(lblName);
        return Emit(GeneratorEmission.BeginExceptionBlock(emitterLabel));
    }

    /// <summary>
    /// Begins a <see langword="catch"/> block.
    /// </summary>
    /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="exceptionType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> is not an <see cref="Exception"/> type.</exception>
    /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock"/>
    public virtual TSelf BeginCatchBlock(Type exceptionType) => Emit(GeneratorEmission.BeginCatchBlock(exceptionType));

    /// <summary>
    /// Begins a <see langword="catch"/> block.
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</typeparam>
    /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock"/>
    public TSelf BeginCatchBlock<TException>() where TException : Exception => BeginCatchBlock(typeof(TException));

    /// <summary>
    /// Begins a <see langword="finally"/> block in the stream.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfinallyblock"/>
    public virtual TSelf BeginFinallyBlock() => Emit(GeneratorEmission.BeginFinallyBlock());

    /// <summary>
    /// Begins an exception block for a filtered exception.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <exception cref="NotSupportedException">This <see cref="Emission.IILGenerator{TGenerator}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptfilterblock"/>
    public virtual TSelf BeginExceptFilterBlock() => Emit(GeneratorEmission.BeginExceptFilterBlock());

    /// <summary>
    /// Begins an exception fault block in the stream.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <exception cref="NotSupportedException">This <see cref="Emission.IILGenerator{TGenerator}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfaultblock"/>
    public virtual TSelf BeginFaultBlock() => Emit(GeneratorEmission.BeginFaultBlock());

    /// <summary>
    /// Ends an exception block.
    /// </summary>
    /// <exception cref="InvalidOperationException">If this operation occurs in an unexpected place in the stream.</exception>
    /// <exception cref="NotSupportedException">If the stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endexceptionblock"/>
    public virtual TSelf EndExceptionBlock() => Emit(GeneratorEmission.EndExceptionBlock());

    /// <summary>
    /// Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfilter?view=netcore-3.0"/>
    public TSelf Endfilter() => Emit(OpCodes.Endfilter);

    /// <summary>
    /// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
    /// </summary>
    /// <remarks>Note that the Endfault and Endfinally instructions are aliases - they correspond to the same opcode.</remarks>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfinally"/>
    public TSelf Endfault() => Emit(OpCodes.Endfinally);

    /// <summary>
    /// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfinally"/>
    public TSelf Endfinally() => Emit(OpCodes.Endfinally);

    /// <summary>
    /// Starts a <c>try<c> block containing <paramref name="tryBlock"/>
    /// </summary>
    public TryCatchFinallyEmitter<TSelf> Try(Action<TSelf> tryBlock)
    {
        return new TryCatchFinallyEmitter<TSelf>(_self)
            .Try(tryBlock);
    }
#endregion

#region Scope
    /// <summary>
    /// Begins a lexical scope.
    /// </summary>
    /// <exception cref="NotSupportedException">This <see cref="IILGenerator{T}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginscope?view=netcore-3.0"/>
    public virtual TSelf BeginScope() => Emit(GeneratorEmission.BeginScope());

    /// <summary>
    /// Ends a lexical scope.
    /// </summary>
    /// <exception cref="NotSupportedException">If this <see cref="IILGenerator{T}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endscope?view=netcore-3.0"/>
    public virtual TSelf EndScope() => Emit(GeneratorEmission.EndScope());

    public TSelf Scoped(Action<TSelf> scopedBlock)
    {
        scopedBlock(this.BeginScope());
        return this.EndScope();
    }

    /// <summary>
    /// Specifies the <see langword="namespace"/> to be used in evaluating locals and watches for the current active lexical scope.
    /// </summary>
    /// <param name="nameSpace">The namespace to be used in evaluating locals and watches for the current active lexical scope.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="nameSpace"/> is <see langword="null"/> or has a Length of 0.</exception>
    /// <exception cref="NotSupportedException">If this <see cref="IILGenerator{T}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.usingnamespace?view=netcore-3.0"/>
    public virtual TSelf UsingNamespace(string nameSpace) => Emit(GeneratorEmission.UsingNamespace(nameSpace));
#endregion

#region Locals
#region Declare
    /// <summary>
    /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The type of the <see cref="LocalBuilder"/>.</param>
    /// <param name="emitterLocalns the declared <see cref="LocalBuilder"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="type"/> was created with <see cref="TypeBuilder.CreateType"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
    public TSelf DeclareLocal(
        Type type,
        out EmitterLocal emitterLocal,
        [CallerArgumentExpression(nameof(emitterLocal))]
        string localName = "")
        => DeclareLocal(type, false, out emitterLocal, localName);

    /// <summary>
    /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="LocalBuilder"/>.</typeparam>
    /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
    public TSelf DeclareLocal<T>(out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string localName = "") => DeclareLocal(typeof(T), false, out local, localName);

    /// <summary>
    /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The type of the <see cref="LocalBuilder"/>.</param>
    /// <param name="isPinned">Whether or not the <see cref="LocalBuilder"/> should be pinned in memory.</param>
    /// <param name="emitterLocalns the declared <see cref="LocalBuilder"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="type"/> was created with <see cref="TypeBuilder.CreateType"/>.</exception>
    /// <exception cref="InvalidOperationException">If the method body of the enclosing method was created with <see cref="M:MethodBuilder.CreateMethodBody"/>.</exception>
    /// <exception cref="NotSupportedException">If the method this <see cref="Emission.IILGenerator{TGenerator}"/> is associated with is not wrapping a <see cref="MethodBuilder"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_System_Boolean_"/>
    public virtual TSelf DeclareLocal(
        Type type,
        bool isPinned,
        out EmitterLocal emitterLocal,
        [CallerArgumentExpression(nameof(emitterLocal))]
        string localName = "")
    {
        emitterLocal = CreateEmitLocal(localName, type, isPinned);
        return Emit(GeneratorEmission.DeclareLocal(emitterLocal));
    }

    /// <summary>
    /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="LocalBuilder"/>.</typeparam>
    /// <param name="pinned">Whether or not the <see cref="LocalBuilder"/> should be pinned in memory.</param>
    /// <param name="emitterLocalns the declared <see cref="LocalBuilder"/>.</param>
    /// <exception cref="InvalidOperationException">If the method body of the enclosing method was created with <see cref="M:MethodBuilder.CreateMethodBody"/>.</exception>
    /// <exception cref="NotSupportedException">If the method this <see cref="Emission.IILGenerator{TGenerator}"/> is associated with is not wrapping a <see cref="MethodBuilder"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_System_Boolean_"/>
    public TSelf DeclareLocal<T>(
        bool pinned, out EmitterLocal emitterLocal,
        [CallerArgumentExpression(nameof(emitterLocal))]
        string localName = "")
        => DeclareLocal(typeof(T), pinned, out emitterLocal, localName);
#endregion

#region Load
    /// <summary>
    /// Loads the given <see cref="EmitterLocal"/>'s value onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
    public TSelf Ldloc(EmitterLocal emitterLocal)
    {
        switch (emitterLocal.Index)
        {
            case 0:
                return Emit(OpCodes.Ldloc_0);
            case 1:
                return Emit(OpCodes.Ldloc_1);
            case 2:
                return Emit(OpCodes.Ldloc_2);
            case 3:
                return Emit(OpCodes.Ldloc_3);
            default:
            {
                if (emitterLocal.IsShortForm)
                    return Emit(OpCodes.Ldloc_S, emitterLocal);

                return Emit(OpCodes.Ldloc, emitterLocal);
            }
        }
    }

    /// <summary>
    /// Loads the given short-form <see cref="EmitterLocal"/>'s value onto the stack.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="local"/> is not short-form.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
    public TSelf Ldloc_S(EmitterLocal local) => Ldloc(local);

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 0 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_0"/>
    public TSelf Ldloc_0() => Emit(OpCodes.Ldloc_0);

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 1 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_1"/>
    public TSelf Ldloc_1() => Emit(OpCodes.Ldloc_1);

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 2 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_2"/>
    public TSelf Ldloc_2() => Emit(OpCodes.Ldloc_2);

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 3 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_3"/>
    public TSelf Ldloc_3() => Emit(OpCodes.Ldloc_3);

    /// <summary>
    /// Loads the address of the given <see cref="EmitterLocal"/> variable.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca_s"/>
    public TSelf Ldloca(EmitterLocal local)
    {
        if (local.IsShortForm)
            return Emit(OpCodes.Ldloca_S, local);

        return Emit(OpCodes.Ldloca, local);
    }

    /// <summary>
    /// Loads the address of the given short-form <see cref="EmitterLocal"/> variable.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="local"/> is not short-form.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca_s"/>
    public TSelf Ldloca_S(EmitterLocal local)
    {
        if (local.IsShortForm)
            return Emit(OpCodes.Ldloca_S, local);

        return Emit(OpCodes.Ldloca, local);
    }
#endregion

#region Store
    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the given <see cref="EmitterLocal"/>.
    /// </summary>
    /// <param name="local">The <see cref="EmitterLocal"/> to store the value in.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
    public TSelf Stloc(EmitterLocal local)
    {
        switch (local.Index)
        {
            case 0:
                return Emit(OpCodes.Stloc_0);
            case 1:
                return Emit(OpCodes.Stloc_1);
            case 2:
                return Emit(OpCodes.Stloc_2);
            case 3:
                return Emit(OpCodes.Stloc_3);
            default:
            {
                if (local.IsShortForm)
                    return Emit(OpCodes.Stloc_S, local);

                return Emit(OpCodes.Stloc, local);
            }
        }
    }

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the given short-form <see cref="EmitterLocal"/>.
    /// </summary>
    /// <param name="local">The short-form <see cref="EmitterLocal"/> to store the value in.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
    public TSelf Stloc_S(EmitterLocal local) => Stloc(local);

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 0.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_0"/>
    public TSelf Stloc_0() => Emit(OpCodes.Stloc_0);

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 1.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_1"/>
    public TSelf Stloc_1() => Emit(OpCodes.Stloc_1);

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 2.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_2"/>
    public TSelf Stloc_2() => Emit(OpCodes.Stloc_2);

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 3.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_3"/>
    public TSelf Stloc_3() => Emit(OpCodes.Stloc_3);
#endregion
#endregion

#region Labels
    /// <summary>
    /// Declares a new <see cref="Label"/>.
    /// </summary>
    /// <param name="emitterLabel">Returns the new <see cref="Label"/> that can be used for branching.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.definelabel"/>
    public virtual TSelf DefineLabel(
        out EmitterLabel emitterLabel,
        [CallerArgumentExpression(nameof(emitterLabel))]
        string lblName = "")
    {
        emitterLabel = CreateEmitLabel(lblName);
        return Emit(GeneratorEmission.DefineLabel(emitterLabel));
    }

    /// <summary>
    /// Marks the stream's current position with the given <see cref="Label"/>.
    /// </summary>
    /// <param name="emitterLabelsee cref="Label"/> for which to set an index.</param>
    /// <exception cref="ArgumentException">If the <paramref name="emitterLabel an invalid index.</exception>
    /// <exception cref="ArgumentException">If the <paramref name="emitterLabel already been marked.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.marklabel"/>
    public virtual TSelf MarkLabel(EmitterLabel emitterLabel) => Emit(GeneratorEmission.MarkLabel(emitterLabel));

    /// <summary>
    /// Implements a jump table.
    /// </summary>
    /// <param name="labels">The labels for the jumptable.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="labels"/> is <see langword="null"/> or empty.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.switch"/>
    public TSelf Switch(params EmitterLabel[] labels)
    {
        if (labels is null || labels.Length == 0)
            throw new ArgumentNullException(nameof(labels));

        return Emit(OpCodes.Switch, labels);
    }

    public TSelf DefineAndMarkLabel(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .MarkLabel(label);
#endregion

#region Method Calling
    /// <summary>
    /// Puts a <see cref="OpCodes.Call"/>, <see cref="OpCodes.Callvirt"/>, or <see cref="OpCodes.Newobj"/> instruction onto the stream to call a <see langword="varargs"/> <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="methodInfo">The <see langword="varargs"/> <see cref="MethodInfo"/> to be called.</param>
    /// <param name="optionalParameterTypes">The types of the Option arguments if the method is a <see langword="varargs"/> method; otherwise, <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="methodInfo"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcall"/>
    public virtual TSelf EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes) => Emit(GeneratorEmission.EmitCall(methodInfo, optionalParameterTypes));

    /// <summary>
    /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call.
    /// </summary>
    /// <param name="callingConventions">The managed calling conventions to be used.</param>
    /// <param name="returnType">The <see cref="Type"/> of the result.</param>
    /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
    /// <param name="optionalParameterTypes">The types of the Option arguments for <see langword="varargs"/> calls.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="returnType"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="optionalParameterTypes"/> is not <see langword="null"/> or empty but <paramref name="callingConventions"/> does not include the <see cref="CallingConventions.VarArgs"/> flag.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli#System_Reflection_Emit_ILGenerator_EmitCalli_System_Reflection_Emit_OpCode_System_Reflection_CallingConventions_System_Type_System_Type___System_Type___"/>
    public virtual TSelf EmitCalli(CallingConventions callingConventions, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes)
        => Emit(GeneratorEmission.EmitCalli(callingConventions, returnType, parameterTypes, optionalParameterTypes));

    /// <summary>
    /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call.
    /// </summary>
    /// <param name="callingConvention">The unmanaged calling convention to be used.</param>
    /// <param name="returnType">The <see cref="Type"/> of the result.</param>
    /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="returnType"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli#System_Reflection_Emit_ILGenerator_EmitCalli_System_Reflection_Emit_OpCode_System_Runtime_InteropServices_CallingConvention_System_Type_System_Type___"/>
    public virtual TSelf EmitCalli(CallingConvention callingConvention, Type? returnType, Type[]? parameterTypes) => Emit(GeneratorEmission.EmitCalli(callingConvention, returnType, parameterTypes));

    /// <summary>
    /// Calls the given <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> that will be called.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.call"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.callvirt"/>
    public TSelf Call(MethodBase method)
    {
        if (method is ConstructorInfo ctor)
        {
            return Emit(OpCodes.Newobj, ctor);
        }
        else if (method is MethodInfo methodInfo)
        {
            return Emit(methodInfo.GetCallOpCode(), methodInfo);
        }
        else
        {
            throw new NotImplementedException();
        }


        /* TODO:
         * Note :
         * When calling methods of System.Object on value types, consider using the constrained prefix with the callvirt instruction instead of emitting a call instruction.
         * This removes the need to emit different IL depending on whether or not the value type overrides the method, avoiding a potential versioning problem.
         * Consider using the constrained prefix when invoking interface methods on value types, since the value type method implementing the interface method can be changed using a MethodImpl.
         * These issues are described in more detail in the Constrained opcode.
         */
    }

    /// <summary>
    /// Calls the given late-bound <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> that will be called.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.callvirt"/>
    public TSelf Callvirt(MethodInfo method) => Call(method);

    /// <summary>
    /// Constrains the <see cref="Type"/> on which a virtual method call (<see cref="OpCodes.Callvirt"/>) is made.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to constrain the <see cref="OpCodes.Callvirt"/> upon.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained?view=netcore-3.0"/>
    public TSelf Constrained(Type type) => Emit(OpCodes.Constrained, type);

    /// <summary>
    /// Constrains the <see cref="Type"/> on which a virtual method call (<see cref="OpCodes.Callvirt"/>) is made.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> to constrain the <see cref="OpCodes.Callvirt"/> upon.</typeparam>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained?view=netcore-3.0"/>
    public TSelf Constrained<T>() => Emit(OpCodes.Constrained, typeof(T));

    /// <summary>
    /// Pushes an unmanaged pointer (<see cref="IntPtr"/>) to the native code implementing the given <see cref="MethodInfo"/> onto the stack.
    /// </summary>
    /// <param name="method">The method to get pointer to.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldftn"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldvirtftn"/>
    public TSelf Ldftn(MethodInfo method) => Emit(OpCodes.Ldftn, method);

    /// <summary>
    /// Pushes an unmanaged pointer (<see cref="IntPtr"/>) to the native code implementing the given virtual <see cref="MethodInfo"/> onto the stack.
    /// </summary>
    /// <param name="method">The method to get pointer to.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldvirtftn"/>
    public TSelf Ldvirtftn(MethodInfo method) => Emit(OpCodes.Ldvirtftn, method);

    /// <summary>
    /// Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual call instruction is executed.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.tailcall"/>
    public TSelf Tailcall() => Emit(OpCodes.Tailcall);
#endregion

#region Arguments
    /// <summary>
    /// Returns an unmanaged pointer to the argument list of the current method.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.arglist?view=netcore-3.0"/>
    public TSelf Arglist() => Emit(OpCodes.Arglist);

#region Ldarg
    /// <summary>
    /// Loads the argument with the specified <paramref name="index"/> onto the stack.
    /// </summary>
    /// <param name="index">The index of the argument to load.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg"/>
    public TSelf Ldarg(int index)
    {
        if (index < 0 || index > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Argument index must be between 0 and {short.MaxValue}");

        if (index == 0)
            return Emit(OpCodes.Ldarg_0);
        if (index == 1)
            return Emit(OpCodes.Ldarg_1);
        if (index == 2)
            return Emit(OpCodes.Ldarg_2);
        if (index == 3)
            return Emit(OpCodes.Ldarg_3);
        if (index <= byte.MaxValue)
            return Emit(OpCodes.Ldarg_S, (byte)index);

        return Emit(OpCodes.Ldarg, (short)index);
    }

    public TSelf Ldarg(ParameterInfo parameter) => Ldarg(parameter.Position);


    /// <summary>
    /// Loads the argument with the specified short-form <paramref name="index"/> onto the stack.
    /// </summary>
    /// <param name="index">The short-form index of the argument to load.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_s"/>
    public TSelf Ldarg_S(int index) => Ldarg(index);

    /// <summary>
    /// Loads the argument at index 0 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_0"/>
    public TSelf Ldarg_0() => Emit(OpCodes.Ldarg_0);

    /// <summary>
    /// Loads the argument at index 1 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_1"/>
    public TSelf Ldarg_1() => Emit(OpCodes.Ldarg_1);

    /// <summary>
    /// Loads the argument at index 2 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_2"/>
    public TSelf Ldarg_2() => Emit(OpCodes.Ldarg_2);

    /// <summary>
    /// Loads the argument at index 3 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_3"/>
    public TSelf Ldarg_3() => Emit(OpCodes.Ldarg_3);

    /// <summary>
    /// Loads the address of the argument with the specified <paramref name="index"/> onto the stack.
    /// </summary>
    /// <param name="index">The index of the argument address to load.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarga"/>
    public TSelf Ldarga(int index)
    {
        if (index < 0 || index > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Argument index must be between 0 and {short.MaxValue}");

        if (index <= byte.MaxValue)
            return Emit(OpCodes.Ldarga_S, (byte)index);

        return Emit(OpCodes.Ldarga, (short)index);
    }

    /// <summary>
    /// Loads the address of the argument with the specified short-form <paramref name="index"/> onto the stack.
    /// </summary>
    /// <param name="index">The short-form index of the argument address to load.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarga_s"/>
    public TSelf Ldarga_S(int index) => Ldarga(index);

    public TSelf Ldarga(ParameterInfo parameter) => Ldarga(parameter.Position);
#endregion

#region Starg
    /// <summary>
    /// Stores the value on top of the stack in the argument at the given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index of the argument.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.starg"/>
    public TSelf Starg(int index)
    {
        if (index < 0 || index > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Argument index must be between 0 and {short.MaxValue}");

        if (index <= byte.MaxValue)
            return Emit(OpCodes.Starg_S, (byte)index);

        return Emit(OpCodes.Starg, (short)index);
    }

    /// <summary>
    /// Stores the value on top of the stack in the argument at the given short-form <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The short-form index of the argument.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.starg_s"/>
    public TSelf Starg_S(int index) => Starg(index);
#endregion
#endregion

#region Debugging
    /// <summary>
    /// Signals the Common Language Infrastructure (CLI) to inform the debugger that a breakpoint has been tripped.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.break"/>
    public TSelf Break() => Emit(OpCodes.Break);

    /// <summary>
    /// Fills space if opcodes are patched. No meaningful operation is performed, although a processing cycle can be consumed.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.nop"/>
    public TSelf Nop() => Emit(OpCodes.Nop);

#region Prefix
    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix1"/>
    [Obsolete("This is a reserved instruction.", true)]
    public TSelf Prefix1() => Emit(OpCodes.Prefix1);

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix2"/>
    [Obsolete("This is a reserved instruction.", true)]
    public TSelf Prefix2() => Emit(OpCodes.Prefix2);

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix3"/>
    [Obsolete("This is a reserved instruction.", true)]
    public TSelf Prefix3() => Emit(OpCodes.Prefix1);

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix4"/>
    [Obsolete("This is a reserved instruction.", true)]
    public TSelf Prefix4() => Emit(OpCodes.Prefix4);

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix5"/>
    [Obsolete("This is a reserved instruction.", true)]
    public TSelf Prefix5() => Emit(OpCodes.Prefix5);

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix6"/>
    [Obsolete("This is a reserved instruction.", true)]
    public TSelf Prefix6() => Emit(OpCodes.Prefix6);

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix7"/>
    [Obsolete("This is a reserved instruction.", true)]
    public TSelf Prefix7() => Emit(OpCodes.Prefix7);

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefixref"/>
    [Obsolete("This is a reserved instruction.", true)]
    public TSelf Prefixref() => Emit(OpCodes.Prefixref);
#endregion
#endregion

#region Exceptions
    /// <summary>
    /// Emits the instructions to throw an <see cref="ArithmeticException"/> if the value on the stack is not a finite number.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ckfinite"/>
    public TSelf Ckfinite() => Emit(OpCodes.Ckfinite);

    /// <summary>
    /// Rethrows the current exception.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an <see langword="catch"/> block.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rethrow"/>
    public TSelf Rethrow() => Emit(OpCodes.Rethrow);

    /// <summary>
    /// Throws the <see cref="Exception"/> currently on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Exception"/> <see cref="object"/> on the stack is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.throw"/>
    public TSelf Throw() => Emit(OpCodes.Throw);

    public virtual TSelf ThrowException(Type exceptionType)
    {
        ValidateType.IsExceptionType(exceptionType);

        var ctor = MemberSearch.OneOrDefault<ConstructorInfo>(
            exceptionType,
            new() { Visibility = Visibility.Instance, ParameterTypes = Type.EmptyTypes, });
        if (ctor is not null)
        {
            return Newobj(ctor)
                .Throw();
        }
        else
        {
            //return LoadUninitialized(exceptionType).Throw();
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Emits the instructions to throw an <see cref="Exception"/>.
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/> to throw.</typeparam>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.throwexception?view=netcore-3.0"/>
    public TSelf ThrowException<TException>()
        where TException : Exception, new()
        => ThrowException(typeof(TException));

    public TSelf ThrowException<TException>(params object?[] exceptionArgs)
        where TException : Exception
    {
        var exceptionArgTypes = exceptionArgs.GetElementTypes();

        // Find the ctor we can call with these args
        var validCtors = typeof(TException)
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(
                ctor =>
                {
                    var ctorParams = ctor.GetParameters();
                    int ctorParamsCount = ctorParams.Length;
                    if (ctorParamsCount != exceptionArgTypes.Length)
                        return false;

                    for (var i = 0; i < ctorParamsCount; i++)
                    {
                        var argType = exceptionArgTypes[i];
                        var paramType = ctorParams[i].ParameterType;
                        if (!argType.Implements(paramType))
                            return false;
                    }
                    return true;
                })
            .ToList();
        Debugger.Break();
        var ctor = validCtors[0];
        foreach (var arg in exceptionArgs)
        {
            this.LoadValue(arg);
        }
        this.Newobj(ctor)
            .Throw();

        var il = this.ToString();
        Debugger.Break();

        return this.Nop();
    }
#endregion

#region Math
    /// <summary>
    /// Adds two values and pushes the result onto the stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add?view=netcore-3.0"/>
    public TSelf Add() => Emit(OpCodes.Add);

    /// <summary>
    /// Adds two <see cref="int"/>s, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add_ovf?view=netcore-3.0"/>
    public TSelf Add_Ovf() => Emit(OpCodes.Add_Ovf);

    /// <summary>
    /// Adds two <see cref="uint"/>s, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add_ovf_un?view=netcore-3.0"/>
    public TSelf Add_Ovf_Un() => Emit(OpCodes.Add_Ovf_Un);

    /// <summary>
    /// Divides two values and pushes the result as a <see cref="float"/> or <see cref="int"/> quotient onto the evaluation stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.div"/>
    public TSelf Div() => Emit(OpCodes.Div);

    /// <summary>
    /// Divides two unsigned values and pushes the result as a <see cref="int"/> quotient onto the evaluation stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.div_un"/>
    public TSelf Div_Un() => Emit(OpCodes.Div_Un);

    /// <summary>
    /// Multiplies two values and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul"/>
    public TSelf Mul() => Emit(OpCodes.Mul);

    /// <summary>
    /// Multiplies two integer values, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul_ovf"/>
    public TSelf Mul_Ovf() => Emit(OpCodes.Mul_Ovf);

    /// <summary>
    /// Multiplies two unsigned integer values, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul_ovf_un"/>
    public TSelf Mul_Ovf_Un() => Emit(OpCodes.Mul_Ovf_Un);

    /// <summary>
    /// Divides two values and pushes the remainder onto the evaluation stack.
    /// </summary>
    /// <exception cref="DivideByZeroException">If the second value is zero.</exception>
    /// <exception cref="OverflowException">If computing the remainder between <see cref="int.MinValue"/> and <see langword="-1"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rem"/>
    public TSelf Rem() => Emit(OpCodes.Rem);

    /// <summary>
    /// Divides two unsigned values and pushes the remainder onto the evaluation stack.
    /// </summary>
    /// <exception cref="DivideByZeroException">If the second value is zero.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rem_un"/>
    public TSelf Rem_Un() => Emit(OpCodes.Rem_Un);

    /// <summary>
    /// Subtracts one value from another and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub"/>
    public TSelf Sub() => Emit(OpCodes.Sub);

    /// <summary>
    /// Subtracts one integer value from another, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub_ovf"/>
    public TSelf Sub_Ovf() => Emit(OpCodes.Sub_Ovf);

    /// <summary>
    /// Subtracts one unsigned integer value from another, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub_ovf_un"/>
    public TSelf Sub_Ovf_Un() => Emit(OpCodes.Sub_Ovf_Un);
#endregion

#region Bitwise
    /// <summary>
    /// Computes the bitwise AND (<see langword="&amp;"/>) of two values and pushes the result onto the stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.and?view=netcore-3.0"/>
    public TSelf And() => Emit(OpCodes.And);

    /// <summary>
    /// Negates a value (<see langword="-"/>) and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.neg"/>
    public TSelf Neg() => Emit(OpCodes.Neg);

    /// <summary>
    /// Computes the one's complement (<see langword="~"/>) of a value and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.not"/>
    public TSelf Not() => Emit(OpCodes.Not);

    /// <summary>
    /// Computes the bitwise OR (<see langword="|"/>) of two values and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.or"/>
    public TSelf Or() => Emit(OpCodes.Or);

    /// <summary>
    /// Shifts an integer value to the left (<see langword="&lt;&lt;"/>) by a specified number of bits, pushing the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shl"/>
    public TSelf Shl() => Emit(OpCodes.Shl);

    /// <summary>
    /// Shifts an integer value to the right (<see langword="&gt;&gt;"/>) by a specified number of bits, pushing the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shr"/>
    public TSelf Shr() => Emit(OpCodes.Shr);

    /// <summary>
    /// Shifts an unsigned integer value to the right (<see langword="&gt;&gt;"/>) by a specified number of bits, pushing the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shr_un"/>
    public TSelf Shr_Un() => Emit(OpCodes.Shr_Un);

    /// <summary>
    /// Computes the bitwise XOR (<see langword="^"/>) of a value and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.xor"/>
    public TSelf Xor() => Emit(OpCodes.Xor);
#endregion

#region Branching
#region Unconditional
    /// <summary>
    /// Unconditionally transfers control to the given <see cref="Label"/>.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br_s?view=netcore-3.0"/>
    public TSelf Br(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Br_S, label);

        return Emit(OpCodes.Br, label);
    }

    /// <summary>
    /// Unconditionally transfers control to the given short-form <see cref="Label"/>.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br_s?view=netcore-3.0"/>
    public TSelf Br_S(EmitterLabel label) => Br(label);

    public TSelf Br(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Br(label);

    /// <summary>
    /// Exits the current method and jumps to the given <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="method">The metadata token for a <see cref="MethodInfo"/> to jump to.</param>
    /// <exception cref="ArgumentNullException">If the <paramref name="method"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.jmp"/>
    public TSelf Jmp(MethodInfo method) => Emit(OpCodes.Jmp, method);

    /// <summary>
    /// Exits a internal region of code, unconditionally transferring control to the given <see cref="Label"/>.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave_s"/>
    public TSelf Leave(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Leave_S, label);

        return Emit(OpCodes.Leave, label);
    }

    /// <summary>
    /// Exits a internal region of code, unconditionally transferring control to the given short-form <see cref="Label"/>.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> is not short-form.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave_s"/>
    public TSelf Leave_S(EmitterLabel label) => Leave(label);

    public TSelf Leave(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Leave(label);

    /// <summary>
    /// Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.
    /// </summary>
    public TSelf Ret() => Emit(OpCodes.Ret);
#endregion

#region True
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if value is <see langword="true"/>, not-<see langword="null"/>, or non-zero.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue_s?view=netcore-3.0"/>
    public TSelf Brtrue(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Brtrue_S, label);

        return Emit(OpCodes.Brtrue, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if value is <see langword="true"/>, not-<see langword="null"/>, or non-zero.
    /// </summary>
    /// <param name="label">The short-form<see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue_s?view=netcore-3.0"/>
    public TSelf Brtrue_S(EmitterLabel label) => Brtrue(label);

    public TSelf Brtrue(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Brtrue(label);
#endregion

#region False
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if value is <see langword="false"/>, <see langword="null"/>, or zero.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse_s?view=netcore-3.0"/>
    public TSelf Brfalse(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Brfalse_S, label);

        return Emit(OpCodes.Brfalse, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if value is <see langword="false"/>, <see langword="null"/>, or zero.
    /// </summary>
    /// <param name="label">The short-form<see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse_s?view=netcore-3.0"/>
    public TSelf Brfalse_S(EmitterLabel label) => Brfalse(label);

    public TSelf Brfalse(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Brfalse(label);
#endregion

#region ==
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if two values are equal.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq_s?view=netcore-3.0"/>
    public TSelf Beq(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Beq_S, label);

        return Emit(OpCodes.Beq, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if two values are equal.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq_s?view=netcore-3.0"/>
    public TSelf Beq_S(EmitterLabel label) => Beq(label);

    public TSelf Beq(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Beq(label);
#endregion

#region !=
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if two unsigned or unordered values are not equal (<see langword="!="/>).
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un_s?view=netcore-3.0"/>
    public TSelf Bne_Un(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Bne_Un_S, label);

        return Emit(OpCodes.Bne_Un, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if two unsigned or unordered values are not equal (<see langword="!="/>).
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un_s?view=netcore-3.0"/>
    public TSelf Bne_Un_S(EmitterLabel label) => Bne_Un(label);

    public TSelf Bne_Un(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Bne_Un(label);
#endregion

#region >=
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_s?view=netcore-3.0"/>
    public TSelf Bge(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Bge_S, label);

        return Emit(OpCodes.Bge, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_s?view=netcore-3.0"/>
    public TSelf Bge_S(EmitterLabel label) => Bge(label);

    public TSelf Bge(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Bge(label);


    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un_s?view=netcore-3.0"/>
    public TSelf Bge_Un(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Bge_Un_S, label);

        return Emit(OpCodes.Bge_Un, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un_s?view=netcore-3.0"/>
    public TSelf Bge_Un_S(EmitterLabel label) => Bge_Un(label);

    public TSelf Bge_Un(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Bge_Un(label);
#endregion

#region >
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_s?view=netcore-3.0"/>
    public TSelf Bgt(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Bgt_S, label);

        return Emit(OpCodes.Bgt, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_s?view=netcore-3.0"/>
    public TSelf Bgt_S(EmitterLabel label) => Bgt(label);

    public TSelf Bgt(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Bgt(label);


    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un_s?view=netcore-3.0"/>
    public TSelf Bgt_Un(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Bgt_Un_S, label);

        return Emit(OpCodes.Bgt_Un, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un_s?view=netcore-3.0"/>
    public TSelf Bgt_Un_S(EmitterLabel label) => Bgt_Un(label);

    public TSelf Bgt_Un(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Bgt_Un(label);
#endregion

#region <=
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_s?view=netcore-3.0"/>
    public TSelf Ble(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Ble_S, label);

        return Emit(OpCodes.Ble, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_s?view=netcore-3.0"/>
    public TSelf Ble_S(EmitterLabel label) => Ble(label);

    public TSelf Ble(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Ble(label);


    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un_s?view=netcore-3.0"/>
    public TSelf Ble_Un(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Ble_Un_S, label);

        return Emit(OpCodes.Ble_Un, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un_s?view=netcore-3.0"/>
    public TSelf Ble_Un_S(EmitterLabel label) => Ble_Un(label);

    public TSelf Ble_Un(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Ble_Un(label);
#endregion

#region <
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_s?view=netcore-3.0"/>
    public TSelf Blt(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Blt_S, label);

        return Emit(OpCodes.Blt, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_s?view=netcore-3.0"/>
    public TSelf Blt_S(EmitterLabel label) => Blt(label);

    public TSelf Blt(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Blt(label);


    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un_s?view=netcore-3.0"/>
    public TSelf Blt_Un(EmitterLabel label)
    {
        if (label.IsShortForm)
            return Emit(OpCodes.Blt_Un_S, label);

        return Emit(OpCodes.Blt_Un, label);
    }

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un_s?view=netcore-3.0"/>
    public TSelf Blt_Un_S(EmitterLabel label) => Blt_Un(label);

    public TSelf Blt_Un(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string lblName = "")
        => DefineLabel(out label, lblName)
            .Blt_Un(label);
#endregion
#endregion

#region Boxing / Unboxing / Casting
    /// <summary>
    /// Converts a value into an <see cref="object"/> reference.
    /// </summary>
    /// <param name="valueType">The <see cref="Type"/> of value that is to be boxed.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.box"/>
    public TSelf Box(Type valueType) => Emit(OpCodes.Box, valueType);

    /// <summary>
    /// Converts a value into an <see cref="object"/> reference.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of value that is to be boxed.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.box"/>
    public TSelf Box<T>() => Emit(OpCodes.Box, typeof(T));

    /// <summary>
    /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to a value-type pointer.
    /// </summary>
    /// <param name="valueType">The value type that is to be unboxed.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="valueType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="valueType"/> is not a value type.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox"/>
    public TSelf Unbox(Type valueType)
    {
        ValidateType.IsValueType(valueType);
        return Emit(OpCodes.Unbox, valueType);
    }

    /// <summary>
    /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to a value-type pointer.
    /// </summary>
    /// <typeparam name="T">The value type that is to be unboxed.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox"/>
    public TSelf Unbox<T>()
        where T : struct
        => Unbox(typeof(T));

    /// <summary>
    /// Converts the boxed representation (<see cref="object"/>) value to its unboxed value.
    /// </summary>
    /// <param name="type">The Type of value to unbox/castclass the value to</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox_any"/>
    public TSelf Unbox_Any(Type type) => Emit(OpCodes.Unbox_Any, type);

    /// <summary>
    /// Converts the boxed representation (<see cref="object"/>) value to its unboxed value.
    /// </summary>
    /// <typeparam name="T">The type that is to be unboxed.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox_any"/>
    public TSelf Unbox_Any<T>() => Unbox_Any(typeof(T));

    /// <summary>
    /// Casts an <see cref="object"/> into the given <see langword="class"/>.
    /// </summary>
    /// <param name="classType">The <see cref="Type"/> of <see langword="class"/> to cast to.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="classType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="classType"/> is not a <see langword="class"/> type.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.castclass"/>
    public TSelf Castclass(Type classType)
    {
        ValidateType.IsClassOrInterfaceType(classType);
        return Emit(OpCodes.Castclass, classType);
    }

    /// <summary>
    /// Casts an <see cref="object"/> into the given <see langword="class"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <see langword="class"/> to cast to.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.castclass"/>
    public TSelf Castclass<T>()
        where T : class
        => Emit(OpCodes.Castclass, typeof(T));

    /// <summary>
    /// Tests whether an <see cref="object"/> is an instance of a given <see langword="class"/> <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of <see langword="class"/> to cast to.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="type"/> is not a <see langword="class"/> type.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.isinst"/>
    public TSelf Isinst(Type type) => Emit(OpCodes.Isinst, type);

    /// <summary>
    /// Tests whether an <see cref="object"/> is an instance of a given <see langword="class"/> <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <see langword="class"/> to cast to.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.isinst"/>
    public TSelf Isinst<T>() => Emit(OpCodes.Isinst, typeof(T));
#endregion

#region Conv
#region nativeint
    /// <summary>
    /// Converts the value on the stack to a <see cref="IntPtr"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i?view=netcore-3.0"/>
    public TSelf Conv_I() => Emit(OpCodes.Conv_I);

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i?view=netcore-3.0"/>
    public TSelf Conv_Ovf_I() => Emit(OpCodes.Conv_Ovf_I);

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i_un?view=netcore-3.0"/>
    public TSelf Conv_Ovf_I_Un() => Emit(OpCodes.Conv_Ovf_I_Un);
#endregion

#region sbyte
    /// <summary>
    /// Converts the value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i1?view=netcore-3.0"/>
    public TSelf Conv_I1() => Emit(OpCodes.Conv_I1);

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i1?view=netcore-3.0"/>
    public TSelf Conv_Ovf_I1() => Emit(OpCodes.Conv_Ovf_I1);

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i1_un?view=netcore-3.0"/>
    public TSelf Conv_Ovf_I1_Un() => Emit(OpCodes.Conv_Ovf_I1_Un);
#endregion

#region short
    /// <summary>
    /// Converts the value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i2?view=netcore-3.0"/>
    public TSelf Conv_I2() => Emit(OpCodes.Conv_I2);

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i2?view=netcore-3.0"/>
    public TSelf Conv_Ovf_I2() => Emit(OpCodes.Conv_Ovf_I2);

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i2_un?view=netcore-3.0"/>
    public TSelf Conv_Ovf_I2_Un() => Emit(OpCodes.Conv_Ovf_I2_Un);
#endregion

#region int
    /// <summary>
    /// Converts the value on the stack to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i4?view=netcore-3.0"/>
    public TSelf Conv_I4() => Emit(OpCodes.Conv_I4);

    /// <summary>
    /// Converts the signed value on the stack to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i4?view=netcore-3.0"/>
    public TSelf Conv_Ovf_I4() => Emit(OpCodes.Conv_Ovf_I4);

    /// <summary>
    /// Converts the unsigned value on the stack to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i4_un?view=netcore-3.0"/>
    public TSelf Conv_Ovf_I4_Un() => Emit(OpCodes.Conv_Ovf_I4_Un);
#endregion

#region long
    /// <summary>
    /// Converts the value on the stack to a <see cref="long"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i8?view=netcore-3.0"/>
    public TSelf Conv_I8() => Emit(OpCodes.Conv_I8);

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i8?view=netcore-3.0"/>
    public TSelf Conv_Ovf_I8() => Emit(OpCodes.Conv_Ovf_I8);

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i8_un?view=netcore-3.0"/>
    public TSelf Conv_Ovf_I8_Un() => Emit(OpCodes.Conv_Ovf_I8_Un);
#endregion

#region nativeuuint
    /// <summary>
    /// Converts the value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u?view=netcore-3.0"/>
    public TSelf Conv_U() => Emit(OpCodes.Conv_U);

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u?view=netcore-3.0"/>
    public TSelf Conv_Ovf_U() => Emit(OpCodes.Conv_Ovf_U);

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u_un?view=netcore-3.0"/>
    public TSelf Conv_Ovf_U_Un() => Emit(OpCodes.Conv_Ovf_U_Un);
#endregion

#region byte
    /// <summary>
    /// Converts the value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u1?view=netcore-3.0"/>
    public TSelf Conv_U1() => Emit(OpCodes.Conv_U1);

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u1?view=netcore-3.0"/>
    public TSelf Conv_Ovf_U1() => Emit(OpCodes.Conv_Ovf_U1);

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u1_un?view=netcore-3.0"/>
    public TSelf Conv_Ovf_U1_Un() => Emit(OpCodes.Conv_Ovf_U1_Un);
#endregion

#region uushort
    /// <summary>
    /// Converts the value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u2?view=netcore-3.0"/>
    public TSelf Conv_U2() => Emit(OpCodes.Conv_U2);

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u2?view=netcore-3.0"/>
    public TSelf Conv_Ovf_U2() => Emit(OpCodes.Conv_Ovf_U2);

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u2_un?view=netcore-3.0"/>
    public TSelf Conv_Ovf_U2_Un() => Emit(OpCodes.Conv_Ovf_U2_Un);
#endregion

#region uuint
    /// <summary>
    /// Converts the value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u4?view=netcore-3.0"/>
    public TSelf Conv_U4() => Emit(OpCodes.Conv_U4);

    /// <summary>
    /// Converts the signed value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u4?view=netcore-3.0"/>
    public TSelf Conv_Ovf_U4() => Emit(OpCodes.Conv_Ovf_U4);

    /// <summary>
    /// Converts the unsigned value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u4_un?view=netcore-3.0"/>
    public TSelf Conv_Ovf_U4_Un() => Emit(OpCodes.Conv_Ovf_U4_Un);
#endregion

#region uulong
    /// <summary>
    /// Converts the value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u8?view=netcore-3.0"/>
    public TSelf Conv_U8() => Emit(OpCodes.Conv_U8);

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u8?view=netcore-3.0"/>
    public TSelf Conv_Ovf_U8() => Emit(OpCodes.Conv_Ovf_U8);

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u8_un?view=netcore-3.0"/>
    public TSelf Conv_Ovf_U8_Un() => Emit(OpCodes.Conv_Ovf_U8_Un);
#endregion

#region float / double
    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="float"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r_un?view=netcore-3.0"/>
    public TSelf Conv_R_Un() => Emit(OpCodes.Conv_R_Un);

    /// <summary>
    /// Converts the value on the stack to a <see cref="float"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r4?view=netcore-3.0"/>
    public TSelf Conv_R4() => Emit(OpCodes.Conv_R4);

    /// <summary>
    /// Converts the value on the stack to a <see cref="double"/>.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r8"/>
    public TSelf Conv_R8() => Emit(OpCodes.Conv_R8);
#endregion
#endregion

#region Comparison
    /// <summary>
    /// Compares two values. If they are equal (<see langword="=="/>), (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ceq?view=netcore-3.0"/>
    public TSelf Ceq() => Emit(OpCodes.Ceq);

    /// <summary>
    /// Compares two values. If the first value is greater than (<see langword="&gt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cgt?view=netcore-3.0"/>
    public TSelf Cgt() => Emit(OpCodes.Cgt);

    /// <summary>
    /// Compares two unsigned or unordered values. If the first value is greater than (<see langword="&gt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cgt_un?view=netcore-3.0"/>
    public TSelf Cgt_Un() => Emit(OpCodes.Cgt_Un);

    public TSelf Cge()
        => Clt()
            .Not();

    public TSelf Cge_Un()
        => Clt_Un()
            .Not();


    /// <summary>
    /// Compares two values. If the first value is less than (<see langword="&lt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.clt?view=netcore-3.0"/>
    public TSelf Clt() => Emit(OpCodes.Clt);

    /// <summary>
    /// Compares two unsigned or unordered values. If the first value is less than (<see langword="&lt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.clt_un?view=netcore-3.0"/>
    public TSelf Clt_Un() => Emit(OpCodes.Clt_Un);

    public TSelf Cle()
        => Cgt()
            .Not();

    public TSelf Cle_Un()
        => Cgt_Un()
            .Not();
#endregion

#region byte*  /  byte[]  /  ref byte
    /// <summary>
    /// Copies a number of bytes from a source address to a destination address.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpblk"/>
    public TSelf Cpblk() => Emit(OpCodes.Cpblk);

    /// <summary>
    /// Initializes a specified block of memory at a specific address to a given size and initial value.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initblk"/>
    public TSelf Initblk() => Emit(OpCodes.Initblk);

    /// <summary>
    /// Allocates a certain number of bytes from the local dynamic memory pool and pushes the address (<see langword="byte*"/>) of the first allocated byte onto the stack.
    /// </summary>
    /// <exception cref="StackOverflowException">If there is insufficient memory to service this request.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.localloc"/>
    public TSelf Localloc() => Emit(OpCodes.Localloc);
#endregion

#region Copy / Duplicate
    /// <summary>
    /// Copies the <see langword="struct"/> located at the <see cref="IntPtr"/> source address to the <see cref="IntPtr"/> destination address.
    /// </summary>
    /// <param name="valueType">The <see cref="Type"/> of <see langword="struct"/> that is to be copied.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="valueType"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpobj"/>
    public TSelf Cpobj(Type valueType)
    {
        ValidateType.IsValueType(valueType);
        return Emit(OpCodes.Cpobj, valueType);
    }

    /// <summary>
    /// Copies the <see langword="struct"/> located at the <see cref="IntPtr"/> source address to the <see cref="IntPtr"/> destination address.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <see langword="struct"/> that is to be copied.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpobj"/>
    public TSelf Cpobj<T>()
        where T : struct
        => Emit(OpCodes.Cpobj, typeof(T));

    /// <summary>
    /// Copies a value, and then pushes the copy onto the evaluation stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.dup"/>
    public TSelf Dup() => Emit(OpCodes.Dup);
#endregion

#region Value Transformation / Creation
    /// <summary>
    /// Initializes each field of the <see langword="struct"/> at a specified address to a <see langword="null"/> reference or 0 primitive.
    /// </summary>
    /// <param name="valueType">The <see langword="struct"/> to be initialized.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="valueType"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="valueType"/> is not a struct.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initobj"/>
    public TSelf Initobj(Type valueType)
    {
        ValidateType.IsValueType(valueType);
        return Emit(OpCodes.Initobj, valueType);
    }

    /// <summary>
    /// Initializes each field of the <see langword="struct"/> at a specified address to a <see langword="null"/> reference or 0 primitive.
    /// </summary>
    /// <typeparam name="T">The <see langword="struct"/> to be initialized.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initobj"/>
    public TSelf Initobj<T>()
        where T : struct
        => Emit(OpCodes.Initobj, typeof(T));

    /// <summary>
    /// Creates a new <see cref="object"/> or <see langword="struct"/> and pushes it onto the stack.
    /// </summary>
    /// <param name="ctor">The <see cref="ConstructorInfo"/> to use to create the object.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="ctor"/> is null.</exception>
    /// <exception cref="OutOfMemoryException">If there is insufficient memory to satisfy the request.</exception>
    /// <exception cref="MissingMethodException">If the <paramref name="ctor"/> could not be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newobj"/>
    public TSelf Newobj(ConstructorInfo ctor) => Emit(OpCodes.Newobj, ctor);

    /// <summary>
    /// Removes the value currently on top of the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.pop"/>
    public TSelf Pop() => Emit(OpCodes.Pop);

    public TSelf PopIf(Type? stackType)
    {
        if (stackType == null || stackType == typeof(void))
            return _self;

        return _self.Pop();
    }
#endregion

#region Load Value
#region LoaD Constant (LDC)
    /// <summary>
    /// Pushes the given <see cref="int"/> onto the stack.
    /// </summary>
    /// <param name="value">The value to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4"/>
    public TSelf Ldc_I4(int value)
    {
        if (value == -1)
            return Emit(OpCodes.Ldc_I4_M1);
        if (value == 0)
            return Emit(OpCodes.Ldc_I4_0);
        if (value == 1)
            return Emit(OpCodes.Ldc_I4_1);
        if (value == 2)
            return Emit(OpCodes.Ldc_I4_2);
        if (value == 3)
            return Emit(OpCodes.Ldc_I4_3);
        if (value == 4)
            return Emit(OpCodes.Ldc_I4_4);
        if (value == 5)
            return Emit(OpCodes.Ldc_I4_5);
        if (value == 6)
            return Emit(OpCodes.Ldc_I4_6);
        if (value == 7)
            return Emit(OpCodes.Ldc_I4_7);
        if (value == 8)
            return Emit(OpCodes.Ldc_I4_8);
        if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
            return Emit(OpCodes.Ldc_I4_S, (sbyte)value);

        return Emit(OpCodes.Ldc_I4, value);
    }

    /// <summary>
    /// Pushes the given <see cref="sbyte"/> onto the stack.
    /// </summary>
    /// <param name="value">The short-form value to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_s"/>
    public TSelf Ldc_I4_S(sbyte value) => Ldc_I4(value);

    /// <summary>
    /// Pushes the given <see cref="int"/> value of -1 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_m1"/>
    public TSelf Ldc_I4_M1() => Emit(OpCodes.Ldc_I4_M1);

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 0 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_0"/>
    public TSelf Ldc_I4_0() => Emit(OpCodes.Ldc_I4_0);

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 1 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_1"/>
    public TSelf Ldc_I4_1() => Emit(OpCodes.Ldc_I4_1);

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 2 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_2"/>
    public TSelf Ldc_I4_2() => Emit(OpCodes.Ldc_I4_2);

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 3 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_3"/>
    public TSelf Ldc_I4_3() => Emit(OpCodes.Ldc_I4_3);

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 4 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_4"/>
    public TSelf Ldc_I4_4() => Emit(OpCodes.Ldc_I4_4);

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 5 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_5"/>
    public TSelf Ldc_I4_5() => Emit(OpCodes.Ldc_I4_5);

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 6 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_6"/>
    public TSelf Ldc_I4_6() => Emit(OpCodes.Ldc_I4_6);

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 7 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_7"/>
    public TSelf Ldc_I4_7() => Emit(OpCodes.Ldc_I4_7);

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 8 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_8"/>
    public TSelf Ldc_I4_8() => Emit(OpCodes.Ldc_I4_8);

    /// <summary>
    /// Pushes the given <see cref="long"/> onto the stack.
    /// </summary>
    /// <param name="value">The value to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i8"/>
    public TSelf Ldc_I8(long value) => Emit(OpCodes.Ldc_I8, value);

    /// <summary>
    /// Pushes the given <see cref="float"/> onto the stack.
    /// </summary>
    /// <param name="value">The value to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_r4"/>
    public TSelf Ldc_R4(float value) => Emit(OpCodes.Ldc_R4, value);

    /// <summary>
    /// Pushes the given <see cref="double"/> onto the stack.
    /// </summary>
    /// <param name="value">The value to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_r8"/>
    public TSelf Ldc_R8(double value) => Emit(OpCodes.Ldc_R8, value);
#endregion

    /// <summary>
    /// Pushes a <see langword="null"/> <see cref="object"/> onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldnull"/>
    public TSelf Ldnull() => Emit(OpCodes.Ldnull);

    /// <summary>
    /// Pushes a <see cref="string"/> onto the stack.
    /// </summary>
    /// <param name="text">The <see cref="string"/> to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldstr"/>
    public TSelf Ldstr(string? text) => Emit(OpCodes.Ldstr, text ?? string.Empty);

#region Ldtoken
    /// <summary>
    /// Converts a metadata token to its runtime representation and pushes it onto the stack.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to convert to a <see cref="RuntimeTypeHandle"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
    public TSelf Ldtoken(Type type) => Emit(OpCodes.Ldtoken, type);

    /// <summary>
    /// Converts a metadata token to its runtime representation and pushes it onto the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> to convert to a <see cref="RuntimeFieldHandle"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
    public TSelf Ldtoken(FieldInfo field) => Emit(OpCodes.Ldtoken, field);

    /// <summary>
    /// Converts a metadata token to its runtime representation and pushes it onto the stack.
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> to convert to a <see cref="RuntimeMethodHandle"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
    public TSelf Ldtoken(MethodInfo method) => Emit(OpCodes.Ldtoken, method);
#endregion

    public bool TryLoadValue<T>(T? value)
    {
        if (value is null)
        {
            Ldnull();
            return true;
        }
        if (value is bool boolean)
        {
            if (boolean)
            {
                Ldc_I4_1();
            }
            else
            {
                Ldc_I4_0();
            }
            return true;
        }
        if (value is byte b)
        {
            Ldc_I4(b);
            return true;
        }
        if (value is sbyte sb)
        {
            Ldc_I4(sb);
            return true;
        }
        if (value is short s)
        {
            Ldc_I4(s);
            return true;
        }
        if (value is ushort us)
        {
            Ldc_I4(us);
            return true;
        }
        if (value is int i)
        {
            Ldc_I4(i);
            return true;
        }
        if (value is uint ui)
        {
            Ldc_I8(ui);
            return true;
        }
        if (value is long l)
        {
            Ldc_I8(l);
            return true;
        }
        if (value is ulong ul)
        {
            Ldc_I8((long)ul)
                .Conv_U();
            return true;
        }
        if (value is float f)
        {
            Ldc_R4(f);
            return true;
        }
        if (value is double d)
        {
            Ldc_R8(d);
            return true;
        }
        if (value is string str)
        {
            Ldstr(str);
            return true;
        }
        if (value is Type type)
        {
            LoadType(type);
            return true;
        }
        if (value is EmitterLocal local)
        {
            Ldloc(local);
            return true;
        }

        return false;

        throw new NotImplementedException();
    }

    /// <summary>
    /// Loads the given <paramref name="value"/> onto the stack
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public TSelf LoadValue<T>(T? value)
    {
        if (!TryLoadValue<T>(value))
            throw new InvalidOperationException();

        return _self;
    }

    public TSelf LoadType(Type type)
    {
        return Ldtoken(type)
            .Call(MemberCache.Methods.Type_GetTypeFromHandle);
    }

    public TSelf LoadType<T>() => LoadType(typeof(T));


    /// <summary>
    /// Loads the default value of a <paramref name="type"/> onto the stack, exactly like default(Type)
    /// </summary>
    public TSelf LoadDefault(Type type)
    {
        // Value types require more code
        if (type.IsValueType)
        {
            return DeclareLocal(type, out var defaultValue)
                .Ldloca(defaultValue)
                .Initobj(type)
                .Ldloc(defaultValue);
        }
        // Anything else defaults to null
        return Ldnull();
    }

    public TSelf LoadDefault<T>() => LoadDefault(typeof(T));

    public TSelf LoadDefaultAddress(Type type)
    {
        // Value types require more code
        if (type.IsValueType)
        {
            return DeclareLocal(type, out var defaultValue)
                .Ldloca(defaultValue)
                .Initobj(type)
                .Ldloca(defaultValue);
        }
        // Anything else defaults to null
        return Ldnulla();
    }

    public TSelf LoadDefaultAddress<T>() => LoadDefaultAddress(typeof(T));


    /// <summary>
    /// Loads a <c>null</c> reference onto the stack
    /// </summary>
    public TSelf Ldnulla()
        => Ldc_I4_0()
            .Conv_U();
#endregion

#region Arrays
    /// <summary>
    /// Pushes the number of elements of a zero-based, one-dimensional <see cref="Array"/> onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldlen"/>
    public TSelf Ldlen() => Emit(OpCodes.Ldlen);

#region Load Element
    /// <summary>
    /// Loads the element from an array index onto the stack as the given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the element to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem"/>
    public TSelf Ldelem(Type type)
    {
        if (type == typeof(IntPtr))
            return Emit(OpCodes.Ldelem_I);
        if (type == typeof(sbyte))
            return Emit(OpCodes.Ldelem_I1);
        if (type == typeof(short))
            return Emit(OpCodes.Ldelem_I2);
        if (type == typeof(int))
            return Emit(OpCodes.Ldelem_I4);
        if (type == typeof(long))
            return Emit(OpCodes.Ldelem_I8);
        if (type == typeof(byte))
            return Emit(OpCodes.Ldelem_U1);
        if (type == typeof(ushort))
            return Emit(OpCodes.Ldelem_U2);
        if (type == typeof(uint))
            return Emit(OpCodes.Ldelem_U4);
        if (type == typeof(float))
            return Emit(OpCodes.Ldelem_R4);
        if (type == typeof(double))
            return Emit(OpCodes.Ldelem_R8);
        if (type == typeof(object))
            return Emit(OpCodes.Ldelem_Ref);

        return Emit(OpCodes.Ldelem, type);
    }

    /// <summary>
    /// Loads the element from an array index onto the stack as the given <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the element to load.</typeparam>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem"/>
    public TSelf Ldelem<T>() => Ldelem(typeof(T));

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="IntPtr"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="IntPtr"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i"/>
    public TSelf Ldelem_I() => Emit(OpCodes.Ldelem_I);

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="sbyte"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="sbyte"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i1"/>
    public TSelf Ldelem_I1() => Emit(OpCodes.Ldelem_I1);

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="short"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="short"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i2"/>
    public TSelf Ldelem_I2() => Emit(OpCodes.Ldelem_I2);

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="int"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i4"/>
    public TSelf Ldelem_I4() => Emit(OpCodes.Ldelem_I4);

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="long"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="long"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i8"/>
    public TSelf Ldelem_I8() => Emit(OpCodes.Ldelem_I8);

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="byte"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="byte"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u1"/>
    public TSelf Ldelem_U1() => Emit(OpCodes.Ldelem_U1);

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="ushort"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="ushort"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u2"/>
    public TSelf Ldelem_U2() => Emit(OpCodes.Ldelem_U2);

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="uint"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="uint"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u4"/>
    public TSelf Ldelem_U4() => Emit(OpCodes.Ldelem_U4);

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="float"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="float"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_r4"/>
    public TSelf Ldelem_R4() => Emit(OpCodes.Ldelem_R4);

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="double"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="double"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_r8"/>
    public TSelf Ldelem_R8() => Emit(OpCodes.Ldelem_R8);

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="object"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="object"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_ref"/>
    public TSelf Ldelem_Ref() => Emit(OpCodes.Ldelem_Ref);

    /// <summary>
    /// Loads the element from an array index onto the stack as an address to a value of the given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the element to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelema"/>
    public TSelf Ldelema(Type type) => Emit(OpCodes.Ldelema, type);

    /// <summary>
    /// Loads the element from an array index onto the stack as an address to a value of the given <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the element to load.</typeparam>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelema"/>
    public TSelf Ldelema<T>() => Emit(OpCodes.Ldelema, typeof(T));
#endregion

#region Store Element
    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the value on the stack with the given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the element to store.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem"/>
    public TSelf Stelem(Type type)
    {
        if (type == typeof(IntPtr))
            return Emit(OpCodes.Stelem_I);
        if (type == typeof(sbyte))
            return Emit(OpCodes.Stelem_I1);
        if (type == typeof(short))
            return Emit(OpCodes.Stelem_I2);
        if (type == typeof(int))
            return Emit(OpCodes.Stelem_I4);
        if (type == typeof(long))
            return Emit(OpCodes.Stelem_I8);
        if (type == typeof(float))
            return Emit(OpCodes.Stelem_R4);
        if (type == typeof(double))
            return Emit(OpCodes.Stelem_R8);
        if (type == typeof(object))
            return Emit(OpCodes.Stelem_Ref);

        return Emit(OpCodes.Stelem, type);
    }

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the value on the stack with the given <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the element to store.</typeparam>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem"/>
    public TSelf Stelem<T>() => Stelem(typeof(T));

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="IntPtr"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="IntPtr"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i"/>
    public TSelf Stelem_I() => Emit(OpCodes.Stelem_I);

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="sbyte"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="sbyte"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i1"/>
    public TSelf Stelem_I1() => Emit(OpCodes.Stelem_I1);

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="short"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="short"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i2"/>
    public TSelf Stelem_I2() => Emit(OpCodes.Stelem_I2);

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="int"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="int"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i4"/>
    public TSelf Stelem_I4() => Emit(OpCodes.Stelem_I4);

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="long"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="long"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i8"/>
    public TSelf Stelem_I8() => Emit(OpCodes.Stelem_I8);

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="float"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="float"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_r4"/>
    public TSelf Stelem_R4() => Emit(OpCodes.Stelem_R4);

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="double"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="double"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_r8"/>
    public TSelf Stelem_R8() => Emit(OpCodes.Stelem_R8);

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="object"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="object"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_ref"/>
    public TSelf Stelem_Ref() => Emit(OpCodes.Stelem_Ref);
#endregion

    /// <summary>
    /// Pushes an <see cref="object"/> reference to a new zero-based, one-dimensional <see cref="Array"/> whose elements are the given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <param name="type">The type of values that can be stored in the array.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newarr"/>
    public TSelf Newarr(Type type) => Emit(OpCodes.Newarr, type);

    /// <summary>
    /// Pushes an <see cref="object"/> reference to a new zero-based, one-dimensional <see cref="Array"/> whose elements are the given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of values that can be stored in the array.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newarr"/>
    public TSelf Newarr<T>() => Emit(OpCodes.Newarr, typeof(T));

    /// <summary>
    /// Specifies that the subsequent array address operation performs no type check at run time, and that it returns a managed pointer whose mutability is restricted.
    /// </summary>
    /// <remarks>This instruction can only appear before a <see cref="Ldelema"/> instruction.</remarks>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.readonly"/>
    public TSelf Readonly() => Emit(OpCodes.Readonly);
#endregion

#region Fields
    /// <summary>
    /// Loads the value of the given <see cref="FieldInfo"/> onto the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> whose value to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldfld"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsfld"/>
    public TSelf Ldfld(FieldInfo field)
    {
        if (field.IsStatic)
            return Emit(OpCodes.Ldsfld, field);

        return Emit(OpCodes.Ldfld, field);
    }

    /// <summary>
    /// Loads the value of the given static <see cref="FieldInfo"/> onto the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> whose value to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsfld"/>
    public TSelf Ldsfld(FieldInfo field) => Ldfld(field);

    /// <summary>
    /// Loads the address of the given <see cref="FieldInfo"/> onto the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> whose address to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldflda"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsflda"/>
    public TSelf Ldflda(FieldInfo field)
    {
        if (field.IsStatic)
            return Emit(OpCodes.Ldsflda, field);

        return Emit(OpCodes.Ldflda, field);
    }

    /// <summary>
    /// Loads the address of the given <see cref="FieldInfo"/> onto the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> whose address to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsflda"/>
    public TSelf Ldsflda(FieldInfo field) => Ldflda(field);

    /// <summary>
    /// Replaces the value stored in the given <see cref="FieldInfo"/> with the value on the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> whose value to replace.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="NullReferenceException">If the instance value/pointer on the stack is <see langword="null"/> and the <paramref name="field"/> is not <see langword="static"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stfld"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stsfld"/>
    public TSelf Stfld(FieldInfo field)
    {
        if (field.IsStatic)
            return Emit(OpCodes.Stsfld, field);

        return Emit(OpCodes.Stfld, field);
    }

    /// <summary>
    /// Replaces the value stored in the given static <see cref="FieldInfo"/> with the value on the stack.
    /// </summary>
    /// <param name="field">The static <see cref="FieldInfo"/> whose value to replace.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
    /// <exception cref="NullReferenceException">If the instance value/pointer on the stack is <see langword="null"/> and the <paramref name="field"/> is not <see langword="static"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stsfld"/>
    public TSelf Stsfld(FieldInfo field) => Stfld(field);
#endregion

#region Load / Store via Address
    /// <summary>
    /// Loads a value from an address onto the stack.
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
    public TSelf Ldobj(Type type)
    {
        if (type == typeof(IntPtr))
            return Ldind_I();
        if (type == typeof(byte) || type == typeof(bool))
            return Ldind_I1();
        if (type == typeof(short))
            return Ldind_I2();
        if (type == typeof(int))
            return Ldind_I4();
        if (type == typeof(long) || type == typeof(ulong))
            return Ldind_I8();
        if (type == typeof(byte))
            return Ldind_U1();
        if (type == typeof(ushort))
            return Ldind_U2();
        if (type == typeof(uint))
            return Ldind_U4();
        if (type == typeof(float))
            return Ldind_R4();
        if (type == typeof(double))
            return Ldind_R8();
        if (type == typeof(object) || type.IsClass)
            return Ldind_Ref();

        return Emit(OpCodes.Ldobj, type);
    }

    /// <summary>
    /// Loads a value from an address onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
    public TSelf Ldobj<T>() => Ldobj(typeof(T));

#region Ldind
    /// <summary>
    /// Loads a value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
    /// <remarks>This is just an alias for Ldobj</remarks>
    public TSelf Ldind(Type type) => Ldobj(type);

    /// <summary>
    /// Loads a value from an address onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
    /// <remarks>This is just an alias for Ldobj</remarks>
    public TSelf Ldind<T>() => Ldobj<T>();

    /// <summary>
    /// Loads a <see cref="IntPtr"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i"/>
    public TSelf Ldind_I() => Emit(OpCodes.Ldind_I);

    /// <summary>
    /// Loads a <see cref="sbyte"/> value from an address onto the stack as an <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i1"/>
    public TSelf Ldind_I1() => Emit(OpCodes.Ldind_I1);

    /// <summary>
    /// Loads a <see cref="short"/> value from an address onto the stack as an <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i2"/>
    public TSelf Ldind_I2() => Emit(OpCodes.Ldind_I2);

    /// <summary>
    /// Loads a <see cref="int"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i4"/>
    public TSelf Ldind_I4() => Emit(OpCodes.Ldind_I4);

    /// <summary>
    /// Loads a <see cref="long"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i8"/>
    public TSelf Ldind_I8() => Emit(OpCodes.Ldind_I8);

    /// <summary>
    /// Loads a <see cref="byte"/> value from an address onto the stack as an <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u1"/>
    public TSelf Ldind_U1() => Emit(OpCodes.Ldind_U1);

    /// <summary>
    /// Loads a <see cref="ushort"/> value from an address onto the stack as an <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u2"/>
    public TSelf Ldind_U2() => Emit(OpCodes.Ldind_U2);

    /// <summary>
    /// Loads a <see cref="uint"/> value from an address onto the stack onto the stack as an <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u4"/>
    public TSelf Ldind_U4() => Emit(OpCodes.Ldind_U4);

    /// <summary>
    /// Loads a <see cref="float"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_r4"/>
    public TSelf Ldind_R4() => Emit(OpCodes.Ldind_R4);

    /// <summary>
    /// Loads a <see cref="double"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_r8"/>
    public TSelf Ldind_R8() => Emit(OpCodes.Ldind_R8);

    /// <summary>
    /// Loads a <see cref="object"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_ref"/>
    public TSelf Ldind_Ref() => Emit(OpCodes.Ldind_Ref);
#endregion

    /// <summary>
    /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="TypeLoadException">If <paramref name="type"/> cannot be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
    public TSelf Stobj(Type type)
    {
        if (type == typeof(IntPtr))
            return Stind_I();
        if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(bool))
            return Stind_I1();
        if (type == typeof(short) || type == typeof(ushort))
            return Stind_I2();
        if (type == typeof(int) || type == typeof(uint))
            return Stind_I4();
        if (type == typeof(long) || type == typeof(ulong))
            return Stind_I8();
        if (type == typeof(float))
            return Stind_R4();
        if (type == typeof(double))
            return Stind_R8();
        if (type == typeof(object) || type.IsClass)
            return Stind_Ref();

        return Emit(OpCodes.Stobj, type);
    }

    /// <summary>
    /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
    /// </summary>
    /// <exception cref="TypeLoadException">If the given <see cref="Type"/> cannot be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
    public TSelf Stobj<T>() => Stobj(typeof(T));

#region Stind
    /// <summary>
    /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="TypeLoadException">If <paramref name="type"/> cannot be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
    /// <remarks>This is just an alias for Stobj</remarks>
    public TSelf Stind(Type type) => Stobj(type);

    /// <summary>
    /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
    /// </summary>
    /// <exception cref="TypeLoadException">If the given <see cref="Type"/> cannot be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
    /// <remarks>This is just an alias for Stobj</remarks>
    public TSelf Stind<T>() => Stobj(typeof(T));

    /// <summary>
    /// Stores a <see cref="IntPtr"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i"/>
    public TSelf Stind_I() => Emit(OpCodes.Stind_I);

    /// <summary>
    /// Stores a <see cref="sbyte"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i1"/>
    public TSelf Stind_I1() => Emit(OpCodes.Stind_I1);

    /// <summary>
    /// Stores a <see cref="short"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i2"/>
    public TSelf Stind_I2() => Emit(OpCodes.Stind_I2);

    /// <summary>
    /// Stores a <see cref="int"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i4"/>
    public TSelf Stind_I4() => Emit(OpCodes.Stind_I4);

    /// <summary>
    /// Stores a <see cref="long"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i8"/>
    public TSelf Stind_I8() => Emit(OpCodes.Stind_I8);

    /// <summary>
    /// Stores a <see cref="float"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_r4"/>
    public TSelf Stind_R4() => Emit(OpCodes.Stind_R4);

    /// <summary>
    /// Stores a <see cref="double"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_r8"/>
    public TSelf Stind_R8() => Emit(OpCodes.Stind_R8);

    /// <summary>
    /// Stores a <see cref="object"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_ref"/>
    public TSelf Stind_Ref() => Emit(OpCodes.Stind_Ref);
#endregion


    /// <summary>
    /// Indicates that an address on the stack might not be aligned to the natural size of the immediately following
    /// <see cref="Ldind"/>, <see cref="Stind"/>, <see cref="Ldfld"/>, <see cref="Stfld"/>, <see cref="Ldobj"/>, <see cref="Stobj"/>, <see cref="Initblk"/>, or <see cref="Cpblk"/> instruction.
    /// </summary>
    /// <param name="alignment">Specifies the generated code should assume the address is <see cref="byte"/>, double-<see cref="byte"/>, or quad-<see cref="byte"/> aligned.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="alignment"/> is not 1, 2, or 4.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unaligned"/>
    public TSelf Unaligned(int alignment)
    {
        if (alignment != 1 && alignment != 2 && alignment != 4)
            throw new ArgumentOutOfRangeException(nameof(alignment), alignment, "Alignment can only be 1, 2, or 4");

        return Emit(OpCodes.Unaligned, (byte)alignment);
    }

    /// <summary>
    /// Indicates that an address currently on the stack might be volatile, and the results of reading that location cannot be cached or that multiple stores to that location cannot be suppressed.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.volatile"/>
    public TSelf Volatile() => Emit(OpCodes.Volatile);
#endregion

#region Upon Type
    /// <summary>
    /// Pushes a typed reference to an instance of a given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of reference to push.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mkrefany"/>
    public TSelf Mkrefany(Type type) => Emit(OpCodes.Mkrefany, type);

    /// <summary>
    /// Pushes a typed reference to an instance of a given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of reference to push.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mkrefany"/>
    public TSelf Mkrefany<T>() => Emit(OpCodes.Mkrefany, typeof(T));

    /// <summary>
    /// Retrieves the type token embedded in a typed reference.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanytype"/>
    public TSelf Refanytype() => Emit(OpCodes.Refanytype);

    /// <summary>
    /// Retrieves the address (<see langword="&amp;"/>) embedded in a typed reference.
    /// </summary>
    /// <param name="type">The type of reference to retrieve the address.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
    /// <exception cref="InvalidCastException">If <paramref name="type"/> is not the same as the <see cref="Type"/> of the reference.</exception>
    /// <exception cref="TypeLoadException">If <paramref name="type"/> cannot be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanyval"/>
    public TSelf Refanyval(Type type) => Emit(OpCodes.Refanyval, type);

    /// <summary>
    /// Retrieves the address (<see langword="&amp;"/>) embedded in a typed reference.
    /// </summary>
    /// <typeparam name="T">The type of reference to retrieve the address.</typeparam>
    /// <exception cref="InvalidCastException">If <typeparamref name="T"/> is not the same as the <see cref="Type"/> of the reference.</exception>
    /// <exception cref="TypeLoadException">If <typeparamref name="T"/> cannot be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanyval"/>
    public TSelf Refanyval<T>() => Emit(OpCodes.Refanyval, typeof(T));

    /// <summary>
    /// Pushes the size, in <see cref="byte"/>s, of a given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get the size of.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sizeof"/>
    public TSelf Sizeof(Type type)
    {
        ValidateType.IsValueType(type);
        return Emit(OpCodes.Sizeof, type);
    }

    /// <summary>
    /// Pushes the size, in <see cref="byte"/>s, of a given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> to get the size of.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if the given <see cref="Type"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sizeof"/>
    public TSelf Sizeof<T>()
        where T : unmanaged
        => Emit(OpCodes.Sizeof, typeof(T));
#endregion

#region Advanced
    public TSelf EmitParamsCountCheck(ParameterInfo paramsParameter, int requiredCount)
    {
        return _self
            .Ldarg(paramsParameter)
            .Ldlen()
            .Ldc_I4(requiredCount)
            .Beq(out var lblLengthEqual)
            .Ldstr($"{requiredCount} parameters are required in the params array")
            .Ldstr(paramsParameter.Name)
            .Newobj(MemberSearch.One<ArgumentException, ConstructorInfo>(new() { ParameterTypes = new[] { typeof(string), typeof(string) }, }))
            .Throw()
            .MarkLabel(lblLengthEqual);
    }

    public TSelf EmitLoadParams(ParameterInfo paramsParameter, ReadOnlySpan<ParameterInfo> destParameters)
    {
        int len = destParameters.Length;
        // None to load?
        if (len == 0)
            return _self;

        // Params -> Params?
        if (len == 1
            && destParameters[0]
                .IsParams())
        {
            _self.Ldarg(paramsParameter);
        }
        else
        {
            // extract each parameter in turn
            for (var i = 0; i < len; i++)
            {
                _self.Ldarg(paramsParameter)
                    .Ldc_I4(i)
                    .Ldelem(destParameters[i].ParameterType);
            }
        }

        // Everything will be loaded!
        return _self;
    }
#endregion

    public void DeclareTo(CodeBuilder codeBuilder)
    {
        this.Emissions.DeclareTo(codeBuilder);
    }

    public override string ToString()
    {
        return CodePart.ToDeclaration(this);
    }
}