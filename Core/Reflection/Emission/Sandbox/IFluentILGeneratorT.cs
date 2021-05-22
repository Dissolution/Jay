using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
// ReSharper disable IdentifierTypo

namespace Jay.Reflection.Emission.Sandbox
{
    public interface IFluentILGenerator<TEmitter> : IInstructions
        where TEmitter : IFluentILGenerator<TEmitter>
    {
        TEmitter Try(params (Type, Action<TEmitter, Type, Label>)[] catchBlocks);

        /// <summary>
        /// Begins a <see langword="catch"/> block.
        /// </summary>
        /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="exceptionType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> is not an <see cref="Exception"/> type.</exception>
        /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock"/>
        TEmitter BeginCatchBlock(Type exceptionType);

        /// <summary>
        /// Begins a <see langword="catch"/> block.
        /// </summary>
        /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</typeparam>
        /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock"/>
        TEmitter BeginCatchBlock<TException>()
            where TException : Exception;

        /// <summary>
        /// Begins an exception block for a filtered exception.
        /// </summary>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
        /// <exception cref="NotSupportedException">This <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptfilterblock"/>
        TEmitter BeginExceptFilterBlock();

        /// <summary>
        /// Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfilter"/>
        TEmitter Endfilter();

        /// <summary>
        /// Begins an exception block for a non-filtered exception.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> for the end of the block. This will leave you in the correct place to execute <see langword="finally"/> blocks or to finish the <see langword="try"/>.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptionblock"/>
        TEmitter BeginExceptionBlock(out Label label);

        /// <summary>
        /// Ends an exception block.
        /// </summary>
        /// <exception cref="InvalidInstructionException">If this Instruction occurs in an unexpected place in the stream.</exception>
        /// <exception cref="NotSupportedException">If the stream being emitted is not currently in an exception block.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endexceptionblock"/>
        TEmitter EndExceptionBlock();

        /// <summary>
        /// Begins an exception fault block in the stream.
        /// </summary>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
        /// <exception cref="NotSupportedException">This <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfaultblock"/>
        TEmitter BeginFaultBlock();

        /// <summary>
        /// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
        /// </summary>
        /// <remarks>Note that the Endfault and Endfinally instructions are aliases - they correspond to the same opcode.</remarks>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfinally"/>
        TEmitter Endfault();

        /// <summary>
        /// Begins a <see langword="finally"/> block in the stream.
        /// </summary>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfinallyblock"/>
        TEmitter BeginFinallyBlock();

        /// <summary>
        /// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfinally"/>
        TEmitter Endfinally();

        /// <summary>
        /// Begins a lexical scope.
        /// </summary>
        /// <exception cref="NotSupportedException">This <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginscope"/>
        TEmitter BeginScope();

        /// <summary>
        /// Ends a lexical scope.
        /// </summary>
        /// <exception cref="NotSupportedException">If this <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endscope"/>
        TEmitter EndScope();

        /// <summary>
        /// Specifies the <see langword="namespace"/> to be used in evaluating locals and watches for the current active lexical scope.
        /// </summary>
        /// <param name="namespace">The namespace to be used in evaluating locals and watches for the current active lexical scope.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="namespace"/> is <see langword="null"/> or has a Length of 0.</exception>
        /// <exception cref="NotSupportedException">If this <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.usingnamespace"/>
        TEmitter UsingNamespace(string @namespace);

        /// <summary>
        /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="localType">The type of the <see cref="LocalBuilder"/>.</param>
        /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="localType"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="localType"/> was created with <see cref="TypeBuilder.CreateType"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
        TEmitter DeclareLocal(Type localType, out LocalBuilder local);

        /// <summary>
        /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="LocalBuilder"/>.</typeparam>
        /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
        TEmitter DeclareLocal<T>(out LocalBuilder local);

        /// <summary>
        /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="localType">The type of the <see cref="LocalBuilder"/>.</param>
        /// <param name="pinned">Whether or not the <see cref="LocalBuilder"/> should be pinned in memory.</param>
        /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="localType"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="localType"/> was created with <see cref="TypeBuilder.CreateType"/>.</exception>
        /// <exception cref="InvalidOperationException">If the method body of the enclosing method was created with <see cref="M:MethodBuilder.CreateMethodBody"/>.</exception>
        /// <exception cref="NotSupportedException">If the method this <see cref="ILEmitter"/> is associated with is not wrapping a <see cref="MethodBuilder"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_System_Boolean_"/>
        TEmitter DeclareLocal(Type localType, bool pinned, out LocalBuilder local);

        /// <summary>
        /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="LocalBuilder"/>.</typeparam>
        /// <param name="pinned">Whether or not the <see cref="LocalBuilder"/> should be pinned in memory.</param>
        /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
        /// <exception cref="InvalidOperationException">If the method body of the enclosing method was created with <see cref="M:MethodBuilder.CreateMethodBody"/>.</exception>
        /// <exception cref="NotSupportedException">If the method this <see cref="ILEmitter"/> is associated with is not wrapping a <see cref="MethodBuilder"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_System_Boolean_"/>
        TEmitter DeclareLocal<T>(bool pinned, out LocalBuilder local);

        /// <summary>
        /// Loads the given <see cref="LocalBuilder"/>'s value onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
        TEmitter Ldloc(LocalBuilder local);

        /// <summary>
        /// Loads the given short-form <see cref="LocalBuilder"/>'s value onto the stack.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="local"/> is not short-form.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
        TEmitter Ldloc_S(LocalBuilder local);

        /// <summary>
        /// Loads the value of the <see cref="LocalBuilder"/> variable at index 0 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_0"/>
        TEmitter Ldloc_0();

        /// <summary>
        /// Loads the value of the <see cref="LocalBuilder"/> variable at index 1 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_1"/>
        TEmitter Ldloc_1();

        /// <summary>
        /// Loads the value of the <see cref="LocalBuilder"/> variable at index 2 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_2"/>
        TEmitter Ldloc_2();

        /// <summary>
        /// Loads the value of the <see cref="LocalBuilder"/> variable at index 3 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_3"/>
        TEmitter Ldloc_3();

        /// <summary>
        /// Loads the address of the given <see cref="LocalBuilder"/> variable.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca_s"/>
        TEmitter Ldloca(LocalBuilder local);

        /// <summary>
        /// Loads the address of the given short-form <see cref="LocalBuilder"/> variable.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="local"/> is not short-form.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca_s"/>
        TEmitter Ldloca_S(LocalBuilder local);

        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the given <see cref="LocalBuilder"/>.
        /// </summary>
        /// <param name="local">The <see cref="LocalBuilder"/> to store the value in.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
        TEmitter Stloc(LocalBuilder local);

        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the given short-form <see cref="LocalBuilder"/>.
        /// </summary>
        /// <param name="local">The short-form <see cref="LocalBuilder"/> to store the value in.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
        TEmitter Stloc_S(LocalBuilder local);

        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the <see cref="LocalBuilder"/> at index 0.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_0"/>
        TEmitter Stloc_0();

        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the <see cref="LocalBuilder"/> at index 1.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_1"/>
        TEmitter Stloc_1();

        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the <see cref="LocalBuilder"/> at index 2.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_2"/>
        TEmitter Stloc_2();

        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the <see cref="LocalBuilder"/> at index 3.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_3"/>
        TEmitter Stloc_3();

        /// <summary>
        /// Declares a new <see cref="Label"/>.
        /// </summary>
        /// <param name="label">Returns the new <see cref="Label"/> that can be used for branching.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.definelabel"/>
        TEmitter DefineLabel(out Label label);

        /// <summary>
        /// Marks the stream's current position with the given <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> for which to set an index.</param>
        /// <exception cref="ArgumentException">If the <paramref name="label"/> has an invalid index.</exception>
        /// <exception cref="ArgumentException">If the <paramref name="label"/> has already been marked.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.marklabel"/>
        TEmitter MarkLabel(Label label);

        /// <summary>
        /// Declares a new <see cref="Label"/> and marks the stream's current position with it.
        /// </summary>
        /// <param name="label">Returns the new <see cref="Label"/> marked with the stream's current position.</param>
        TEmitter DefineAndMarkLabel(out Label label);

        /// <summary>
        /// Implements a jump table.
        /// </summary>
        /// <param name="labels">The labels for the jumptable.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="labels"/> is <see langword="null"/> or empty.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.switch"/>
        TEmitter Switch(params Label[] labels);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_"/>
        TEmitter Emit(OpCode opCode);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="byte"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Byte_"/>
        TEmitter Emit(OpCode opCode, byte value);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="sbyte"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_SByte_"/>
        TEmitter Emit(OpCode opCode, sbyte value);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="short"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int16_"/>
        TEmitter Emit(OpCode opCode, short value);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="int"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int32_"/>
        TEmitter Emit(OpCode opCode, int value);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="long"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int64_"/>
        TEmitter Emit(OpCode opCode, long value);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="float"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Single_"/>
        TEmitter Emit(OpCode opCode, float value);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="double"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Double_"/>
        TEmitter Emit(OpCode opCode, double value);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="string"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="str">The <see cref="string"/>to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_String_"/>
        TEmitter Emit(OpCode opCode, string? str);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="FieldInfo"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="field">The <see cref="FieldInfo"/> to emit.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_FieldInfo_"/>
        TEmitter Emit(OpCode opCode, FieldInfo field);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="method">The <see cref="MethodInfo"/> to emit.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is a generic method for which <see cref="MethodBase.IsGenericMethodDefinition"/> is <see langword="false"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_MethodInfo_"/>
        TEmitter Emit(OpCode opCode, MethodInfo method);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="ConstructorInfo"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="ctor">The <see cref="ConstructorInfo"/> to emit.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="ctor"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_ConstructorInfo_"/>
        TEmitter Emit(OpCode opCode, ConstructorInfo ctor);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="SignatureHelper"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="signature">A helper for constructing a signature token.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="signature"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_SignatureHelper_"/>
        TEmitter Emit(OpCode opCode, SignatureHelper signature);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="Type"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="type">The <see cref="Type"/> to emit.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Type_"/>
        TEmitter Emit(OpCode opCode, Type type);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the index of the given <see cref="LocalBuilder"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="local">The <see cref="LocalBuilder"/> to emit the index of.</param>
        /// <exception cref="InvalidOperationException">If <paramref name="opCode"/> is a single-byte instruction and <paramref name="local"/> has an index greater than <see cref="byte.MaxValue"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_LocalBuilder_"/>
        TEmitter Emit(OpCode opCode, LocalBuilder local);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream and leaves space to include a <see cref="Label"/> when fixes are done.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="label">The <see cref="Label"/> to branch from this location.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_Label_"/>
        TEmitter Emit(OpCode opCode, Label label);

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream and leaves space to include a <see cref="Label"/> when fixes are done.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="labels">The <see cref="Label"/>s of which to branch to from this locations.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="labels"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="labels"/> is empty.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_Label___"/>
        TEmitter Emit(OpCode opCode, params Label[] labels);

        /// <summary>
        /// Puts a <see cref="OpCodes.Call"/>, <see cref="OpCodes.Callvirt"/>, or <see cref="OpCodes.Newobj"/> instruction onto the stream to call a <see langword="varargs"/> <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The <see langword="varargs"/> <see cref="MethodInfo"/> to be called.</param>
        /// <param name="optionParameterTypes">The types of the Option arguments if the method is a <see langword="varargs"/> method; otherwise, <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcall"/>
        TEmitter Call(MethodInfo method, params Type[]? optionParameterTypes);

        /// <summary>
        /// Calls the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> that will be called.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.call"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.callvirt"/>
        TEmitter Call(MethodInfo method);

        /// <summary>
        /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call.
        /// </summary>
        /// <param name="convention">The unmanaged calling convention to be used.</param>
        /// <param name="returnType">The <see cref="Type"/> of the result.</param>
        /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="returnType"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli#System_Reflection_Emit_ILGenerator_EmitCalli_System_Reflection_Emit_OpCode_System_Runtime_InteropServices_CallingConvention_System_Type_System_Type___"/>
        TEmitter Calli(CallingConvention convention, Type? returnType, params Type[]? parameterTypes);

        /// <summary>
        /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call.
        /// </summary>
        /// <param name="conventions">The managed calling conventions to be used.</param>
        /// <param name="returnType">The <see cref="Type"/> of the result.</param>
        /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
        /// <param name="optionParameterTypes">The types of the Option arguments for <see langword="varargs"/> calls.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="returnType"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="optionParameterTypes"/> is not <see langword="null"/> or empty but <paramref name="conventions"/> does not include the <see cref="CallingConventions.VarArgs"/> flag.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli#System_Reflection_Emit_ILGenerator_EmitCalli_System_Reflection_Emit_OpCode_System_Reflection_CallingConventions_System_Type_System_Type___System_Type___"/>
        TEmitter Calli(CallingConventions conventions, Type? returnType, Type[]? parameterTypes, params Type[]? optionParameterTypes);

        /// <summary>
        /// Calls the given late-bound <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> that will be called.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.callvirt"/>
        TEmitter Callvirt(MethodInfo method);

        /// <summary>
        /// Constrains the <see cref="Type"/> on which a virtual method call (<see cref="OpCodes.Callvirt"/>) is made.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to constrain the <see cref="OpCodes.Callvirt"/> upon.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained"/>
        TEmitter Constrained(Type type);

        /// <summary>
        /// Constrains the <see cref="Type"/> on which a virtual method call (<see cref="OpCodes.Callvirt"/>) is made.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to constrain the <see cref="OpCodes.Callvirt"/> upon.</typeparam>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained"/>
        TEmitter Constrained<T>();

        /// <summary>
        /// Pushes an unmanaged pointer (<see cref="IntPtr"/>) to the native code implementing the given <see cref="MethodInfo"/> onto the stack.
        /// </summary>
        /// <param name="method">The method to get pointer to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldftn"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldvirtftn"/>
        TEmitter Ldftn(MethodInfo method);

        /// <summary>
        /// Pushes an unmanaged pointer (<see cref="IntPtr"/>) to the native code implementing the given virtual <see cref="MethodInfo"/> onto the stack.
        /// </summary>
        /// <param name="method">The method to get pointer to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldvirtftn"/>
        TEmitter Ldvirtftn(MethodInfo method);

        /// <summary>
        /// Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual call instruction is executed.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.tailcall"/>
        TEmitter Tailcall();

        /// <summary>
        /// Signals the Common Language Infrastructure (CLI) to inform the debugger that a breakpoint has been tripped.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.break"/>
        TEmitter Break();

        /// <summary>
        /// Fills space if opcodes are patched. No meaningful operation is performed, although a processing cycle can be consumed.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.nop"/>
        TEmitter Nop();

        /// <summary>
        /// Emits the instructions to call <see cref="Console.WriteLine(string)"/>.
        /// </summary>
        /// <param name="text">The <see cref="string"/> to write to the console.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitwriteline#System_Reflection_Emit_ILGenerator_EmitWriteLine_System_String_"/>
        TEmitter WriteLine(string? text);

        /// <summary>
        /// Emits the instructions to call <see cref="Console.WriteLine(object)"/> with the value of the given <see cref="FieldInfo"/>.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose value is to be written to the console.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">If the <paramref name="field"/> contains a <see cref="TypeBuilder"/> or <see cref="EnumBuilder"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitwriteline#System_Reflection_Emit_ILGenerator_EmitWriteLine_System_Reflection_FieldInfo_"/>
        TEmitter WriteLine(FieldInfo field);

        /// <summary>
        /// Emits the instructions to call <see cref="Console.WriteLine(object)"/> with the value of the given <see cref="LocalBuilder"/>.
        /// </summary>
        /// <param name="local">The <see cref="LocalBuilder"/> whose value is to be written to the console.</param>
        /// <exception cref="ArgumentException">If the <paramref name="local"/> contains a <see cref="TypeBuilder"/> or <see cref="EnumBuilder"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitwriteline#System_Reflection_Emit_ILGenerator_EmitWriteLine_System_Reflection_Emit_LocalBuilder_"/>
        TEmitter WriteLine(LocalBuilder local);

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix1"/>
        TEmitter Prefix1();

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix2"/>
        TEmitter Prefix2();

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix3"/>
        TEmitter Prefix3();

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix4"/>
        TEmitter Prefix4();

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix5"/>
        TEmitter Prefix5();

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix6"/>
        TEmitter Prefix6();

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix7"/>
        TEmitter Prefix7();

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefixref"/>
        TEmitter Prefixref();

        /// <summary>
        /// Emits the instructions to throw an <see cref="Exception"/>.
        /// </summary>
        /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/> to throw.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="exceptionType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> is not an <see cref="Exception"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> does not have a default constructor.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.throwexception"/>
        TEmitter ThrowException(Type exceptionType);

        /// <summary>
        /// Emits the instructions to throw an <see cref="Exception"/>.
        /// </summary>
        /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/> to throw.</typeparam>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.throwexception"/>
        TEmitter ThrowException<TException>()
            where TException : Exception, new();

        /// <summary>
        /// Emits the instructions to throw an <see cref="ArithmeticException"/> if the value on the stack is not a finite number.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ckfinite"/>
        TEmitter Ckfinite();

        /// <summary>
        /// Rethrows the current exception.
        /// </summary>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an <see langword="catch"/> block.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rethrow"/>
        TEmitter Rethrow();

        /// <summary>
        /// Throws the <see cref="Exception"/> currently on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Exception"/> <see cref="object"/> on the stack is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.throw"/>
        TEmitter Throw();

        /// <summary>
        /// Adds two values and pushes the result onto the stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add"/>
        TEmitter Add();

        /// <summary>
        /// Adds two <see cref="int"/>s, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add_ovf"/>
        TEmitter Add_Ovf();

        /// <summary>
        /// Adds two <see cref="uint"/>s, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add_ovf_un"/>
        TEmitter Add_Ovf_Un();

        /// <summary>
        /// Divides two values and pushes the result as a <see cref="float"/> or <see cref="int"/> quotient onto the evaluation stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.div"/>
        TEmitter Div();

        /// <summary>
        /// Divides two unsigned values and pushes the result as a <see cref="int"/> quotient onto the evaluation stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.div_un"/>
        TEmitter Div_Un();

        /// <summary>
        /// Multiplies two values and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul"/>
        TEmitter Mul();

        /// <summary>
        /// Multiplies two integer values, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul_ovf"/>
        TEmitter Mul_Ovf();

        /// <summary>
        /// Multiplies two unsigned integer values, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul_ovf_un"/>
        TEmitter Mul_Ovf_Un();

        /// <summary>
        /// Divides two values and pushes the remainder onto the evaluation stack.
        /// </summary>
        /// <exception cref="DivideByZeroException">If the second value is zero.</exception>
        /// <exception cref="OverflowException">If computing the remainder between <see cref="int.MinValue"/> and <see langword="-1"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rem"/>
        TEmitter Rem();

        /// <summary>
        /// Divides two unsigned values and pushes the remainder onto the evaluation stack.
        /// </summary>
        /// <exception cref="DivideByZeroException">If the second value is zero.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rem_un"/>
        TEmitter Rem_Un();

        /// <summary>
        /// Subtracts one value from another and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub"/>
        TEmitter Sub();

        /// <summary>
        /// Subtracts one integer value from another, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub_ovf"/>
        TEmitter Sub_Ovf();

        /// <summary>
        /// Subtracts one unsigned integer value from another, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub_ovf_un"/>
        TEmitter Sub_Ovf_Un();

        /// <summary>
        /// Computes the bitwise AND (<see langword="&amp;"/>) of two values and pushes the result onto the stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.and"/>
        TEmitter And();

        /// <summary>
        /// Negates a value (<see langword="-"/>) and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.neg"/>
        TEmitter Neg();

        /// <summary>
        /// Computes the bitwise complement (<see langword="!"/>) of a value and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.not"/>
        TEmitter Not();

        /// <summary>
        /// Computes the bitwise OR (<see langword="|"/>) of two values and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.or"/>
        TEmitter Or();

        /// <summary>
        /// Shifts an integer value to the left (<see langword="&lt;&lt;"/>) by a specified number of bits, pushing the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shl"/>
        TEmitter Shl();

        /// <summary>
        /// Shifts an integer value to the right (<see langword="&gt;&gt;"/>) by a specified number of bits, pushing the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shr"/>
        TEmitter Shr();

        /// <summary>
        /// Shifts an unsigned integer value to the right (<see langword="&gt;&gt;"/>) by a specified number of bits, pushing the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shr_un"/>
        TEmitter Shr_Un();

        /// <summary>
        /// Computes the bitwise XOR (<see langword="^"/>) of a value and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.xor"/>
        TEmitter Xor();

        /// <summary>
        /// Returns an unmanaged pointer to the argument list of the current method.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.arglist"/>
        TEmitter Arglist();

        /// <summary>
        /// Loads the argument with the specified <paramref name="index"/> onto the stack.
        /// </summary>
        /// <param name="index">The index of the argument to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg"/>
        TEmitter Ldarg(int index);

        /// <summary>
        /// Loads the argument with the specified short-form <paramref name="index"/> onto the stack.
        /// </summary>
        /// <param name="index">The short-form index of the argument to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_s"/>
        TEmitter Ldarg_S(int index);

        /// <summary>
        /// Loads the argument at index 0 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_0"/>
        TEmitter Ldarg_0();

        /// <summary>
        /// Loads the argument at index 1 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_1"/>
        TEmitter Ldarg_1();

        /// <summary>
        /// Loads the argument at index 2 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_2"/>
        TEmitter Ldarg_2();

        /// <summary>
        /// Loads the argument at index 3 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_3"/>
        TEmitter Ldarg_3();

        /// <summary>
        /// Loads the address of the argument with the specified <paramref name="index"/> onto the stack.
        /// </summary>
        /// <param name="index">The index of the argument address to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarga"/>
        TEmitter Ldarga(int index);

        /// <summary>
        /// Loads the address of the argument with the specified short-form <paramref name="index"/> onto the stack.
        /// </summary>
        /// <param name="index">The short-form index of the argument address to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarga_s"/>
        TEmitter Ldarga_S(int index);

        /// <summary>
        /// Stores the value on top of the stack in the argument at the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the argument.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.starg"/>
        TEmitter Starg(int index);

        /// <summary>
        /// Stores the value on top of the stack in the argument at the given short-form <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The short-form index of the argument.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.starg_s"/>
        TEmitter Starg_S(int index);

        /// <summary>
        /// Unconditionally transfers control to the given <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br_s"/>
        TEmitter Br(Label label);

        /// <summary>
        /// Defines a <see cref="Label"/> and then unconditionally transfers control to it.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to define and then trasnfer to.</param>
        TEmitter Br(out Label label);

        /// <summary>
        /// Unconditionally transfers control to the given short-form <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br_s"/>
        TEmitter Br_S(Label label);

        /// <summary>
        /// Exits the current method and jumps to the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The metadata token for a <see cref="MethodInfo"/> to jump to.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="method"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.jmp"/>
        TEmitter Jmp(MethodInfo method);

        /// <summary>
        /// Exits a internal region of code, unconditionally transferring control to the given <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave_s"/>
        TEmitter Leave(Label label);

        /// <summary>
        /// Exits a internal region of code, unconditionally transferring control to the given short-form <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> is not short-form.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave_s"/>
        TEmitter Leave_S(Label label);

        /// <summary>
        /// Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.
        /// </summary>
        TEmitter Ret();

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if value is <see langword="true"/>, not-<see langword="null"/>, or non-zero.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue_s"/>
        TEmitter Brtrue(Label label);

        TEmitter Brtrue(out Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if value is <see langword="true"/>, not-<see langword="null"/>, or non-zero.
        /// </summary>
        /// <param name="label">The short-form<see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue_s"/>
        TEmitter Brtrue_S(Label label);

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if value is <see langword="false"/>, <see langword="null"/>, or zero.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse_s"/>
        TEmitter Brfalse(Label label);

        TEmitter Brfalse(out Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if value is <see langword="false"/>, <see langword="null"/>, or zero.
        /// </summary>
        /// <param name="label">The short-form<see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse_s"/>
        TEmitter Brfalse_S(Label label);

        TEmitter Beq(out Label label);

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if two values are equal.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq_s"/>
        TEmitter Beq(Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if two values are equal.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq_s"/>
        TEmitter Beq_S(Label label);

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if two unsigned or unordered values are not equal (<see langword="!="/>).
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un_s"/>
        TEmitter Bne_Un(Label label);

        TEmitter Bne_Un(out Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if two unsigned or unordered values are not equal (<see langword="!="/>).
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un_s"/>
        TEmitter Bne_Un_S(Label label);

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_s"/>
        TEmitter Bge(Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_s"/>
        TEmitter Bge_S(Label label);

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un_s"/>
        TEmitter Bge_Un(Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un_s"/>
        TEmitter Bge_Un_S(Label label);

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_s"/>
        TEmitter Bgt(Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_s"/>
        TEmitter Bgt_S(Label label);

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un_s"/>
        TEmitter Bgt_Un(Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un_s"/>
        TEmitter Bgt_Un_S(Label label);

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_s"/>
        TEmitter Ble(Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_s"/>
        TEmitter Ble_S(Label label);

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un_s"/>
        TEmitter Ble_Un(Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un_s"/>
        TEmitter Ble_Un_S(Label label);

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_s"/>
        TEmitter Blt(Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_s"/>
        TEmitter Blt_S(Label label);

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un_s"/>
        TEmitter Blt_Un(Label label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un_s"/>
        TEmitter Blt_Un_S(Label label);

        /// <summary>
        /// Converts a <see langword="struct"/> into an <see cref="object"/> reference.
        /// </summary>
        /// <param name="valueType">The <see cref="Type"/> of <see langword="struct"/> that is to be boxed.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="valueType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="valueType"/> is not a value type.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.box"/>
        TEmitter Box(Type valueType);

        /// <summary>
        /// Converts a <see langword="struct"/> into an <see cref="object"/> reference.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see langword="struct"/> that is to be boxed.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.box"/>
        TEmitter Box<T>()
            where T : struct;

        /// <summary>
        /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to a value-type pointer.
        /// </summary>
        /// <param name="valueType">The value type that is to be unboxed.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="valueType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="valueType"/> is not a value type.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox"/>
        TEmitter Unbox(Type valueType);

        /// <summary>
        /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to a value-type pointer.
        /// </summary>
        /// <typeparam name="T">The value type that is to be unboxed.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox"/>
        TEmitter Unbox<T>()
            where T : struct;

        /// <summary>
        /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to its unboxed value.
        /// </summary>
        /// <param name="valueType">The value type that is to be unboxed.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="valueType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="valueType"/> is not a value type.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox_any"/>
        TEmitter Unbox_Any(Type valueType);

        /// <summary>
        /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to its unboxed value.
        /// </summary>
        /// <typeparam name="T">The value type that is to be unboxed.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox_any"/>
        TEmitter Unbox_Any<T>()
            where T : struct;

        /// <summary>
        /// Casts an <see cref="object"/> into the given <see langword="class"/>.
        /// </summary>
        /// <param name="classType">The <see cref="Type"/> of <see langword="class"/> to cast to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="classType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="classType"/> is not a <see langword="class"/> type.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.castclass"/>
        TEmitter Castclass(Type classType);

        /// <summary>
        /// Casts an <see cref="object"/> into the given <see langword="class"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see langword="class"/> to cast to.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.castclass"/>
        TEmitter Castclass<T>()
            where T : class;

        /// <summary>
        /// Tests whether an <see cref="object"/> is an instance of a given <see langword="class"/> <see cref="Type"/>.
        /// </summary>
        /// <param name="classType">The <see cref="Type"/> of <see langword="class"/> to cast to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="classType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="classType"/> is not a <see langword="class"/> type.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.isinst"/>
        TEmitter Isinst(Type classType);

        /// <summary>
        /// Tests whether an <see cref="object"/> is an instance of a given <see langword="class"/> <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see langword="class"/> to cast to.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.isinst"/>
        TEmitter Isinst<T>()
            where T : class;

        /// <summary>
        /// Converts the value on the stack to a <see cref="IntPtr"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i"/>
        TEmitter Conv_I();

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i"/>
        TEmitter Conv_Ovf_I();

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i_un"/>
        TEmitter Conv_Ovf_I_Un();

        /// <summary>
        /// Converts the value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i1"/>
        TEmitter Conv_I1();

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i1"/>
        TEmitter Conv_Ovf_I1();

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i1_un"/>
        TEmitter Conv_Ovf_I1_Un();

        /// <summary>
        /// Converts the value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i2"/>
        TEmitter Conv_I2();

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i2"/>
        TEmitter Conv_Ovf_I2();

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i2_un"/>
        TEmitter Conv_Ovf_I2_Un();

        /// <summary>
        /// Converts the value on the stack to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i4"/>
        TEmitter Conv_I4();

        /// <summary>
        /// Converts the signed value on the stack to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i4"/>
        TEmitter Conv_Ovf_I4();

        /// <summary>
        /// Converts the unsigned value on the stack to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i4_un"/>
        TEmitter Conv_Ovf_I4_Un();

        /// <summary>
        /// Converts the value on the stack to a <see cref="long"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i8"/>
        TEmitter Conv_I8();

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i8"/>
        TEmitter Conv_Ovf_I8();

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i8_un"/>
        TEmitter Conv_Ovf_I8_Un();

        /// <summary>
        /// Converts the value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u"/>
        TEmitter Conv_U();

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u"/>
        TEmitter Conv_Ovf_U();

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u_un"/>
        TEmitter Conv_Ovf_U_Un();

        /// <summary>
        /// Converts the value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u1"/>
        TEmitter Conv_U1();

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u1"/>
        TEmitter Conv_Ovf_U1();

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u1_un"/>
        TEmitter Conv_Ovf_U1_Un();

        /// <summary>
        /// Converts the value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u2"/>
        TEmitter Conv_U2();

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u2"/>
        TEmitter Conv_Ovf_U2();

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u2_un"/>
        TEmitter Conv_Ovf_U2_Un();

        /// <summary>
        /// Converts the value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u4"/>
        TEmitter Conv_U4();

        /// <summary>
        /// Converts the signed value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u4"/>
        TEmitter Conv_Ovf_U4();

        /// <summary>
        /// Converts the unsigned value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u4_un"/>
        TEmitter Conv_Ovf_U4_Un();

        /// <summary>
        /// Converts the value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u8"/>
        TEmitter Conv_U8();

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u8"/>
        TEmitter Conv_Ovf_U8();

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u8_un"/>
        TEmitter Conv_Ovf_U8_Un();

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="float"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r_un"/>
        TEmitter Conv_R_Un();

        /// <summary>
        /// Converts the value on the stack to a <see cref="float"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r4"/>
        TEmitter Conv_R4();

        /// <summary>
        /// Converts the value on the stack to a <see cref="double"/>.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r8"/>
        TEmitter Conv_R8();

        /// <summary>
        /// Compares two values. If they are equal (<see langword="=="/>), (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ceq"/>
        TEmitter Ceq();

        /// <summary>
        /// Compares two values. If the first value is greater than (<see langword="&gt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cgt"/>
        TEmitter Cgt();

        /// <summary>
        /// Compares two unsigned or unordered values. If the first value is greater than (<see langword="&gt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cgt_un"/>
        TEmitter Cgt_Un();

        /// <summary>
        /// Compares two values. If the first value is less than (<see langword="&lt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.clt"/>
        TEmitter Clt();

        /// <summary>
        /// Compares two unsigned or unordered values. If the first value is less than (<see langword="&lt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.clt_un"/>
        TEmitter Clt_Un();

        /// <summary>
        /// Copies a number of bytes from a source address to a destination address.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpblk"/>
        TEmitter Cpblk();

        /// <summary>
        /// Initializes a specified block of memory at a specific address to a given size and initial value.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initblk"/>
        TEmitter Initblk();

        /// <summary>
        /// Allocates a certain number of bytes from the local dynamic memory pool and pushes the address (<see langword="byte*"/>) of the first allocated byte onto the stack.
        /// </summary>
        /// <exception cref="StackOverflowException">If there is insufficient memory to service this request.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.localloc"/>
        TEmitter Localloc();

        /// <summary>
        /// Copies the <see langword="struct"/> located at the <see cref="IntPtr"/> source address to the <see cref="IntPtr"/> destination address.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of <see langword="struct"/> that is to be copied.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpobj"/>
        TEmitter Cpobj(Type type);

        /// <summary>
        /// Copies the <see langword="struct"/> located at the <see cref="IntPtr"/> source address to the <see cref="IntPtr"/> destination address.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see langword="struct"/> that is to be copied.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpobj"/>
        TEmitter Cpobj<T>()
            where T : struct;

        /// <summary>
        /// Copies a value, and then pushes the copy onto the evaluation stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.dup"/>
        TEmitter Dup();

        /// <summary>
        /// Initializes each field of the <see langword="struct"/> at a specified address to a <see langword="null"/> reference or 0 primitive.
        /// </summary>
        /// <param name="type">The <see langword="struct"/> to be initialized.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="type"/> is not a struct.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initobj"/>
        TEmitter Initobj(Type type);

        /// <summary>
        /// Initializes each field of the <see langword="struct"/> at a specified address to a <see langword="null"/> reference or 0 primitive.
        /// </summary>
        /// <typeparam name="T">The <see langword="struct"/> to be initialized.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initobj"/>
        TEmitter Initobj<T>()
            where T : struct;

        /// <summary>
        /// Creates a new <see cref="object"/> or <see langword="struct"/> and pushes it onto the stack.
        /// </summary>
        /// <param name="ctor">The <see cref="ConstructorInfo"/> to use to create the object.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="ctor"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">If there is insufficient memory to satisfy the request.</exception>
        /// <exception cref="MissingMethodException">If the <paramref name="ctor"/> could not be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newobj"/>
        TEmitter Newobj(ConstructorInfo ctor);

        /// <summary>
        /// Removes the value currently on top of the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.pop"/>
        TEmitter Pop();

        TEmitter PopIfNotVoid(Type? type);

        /// <summary>
        /// Pushes the given <see cref="int"/> onto the stack.
        /// </summary>
        /// <param name="value">The value to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4"/>
        TEmitter Ldc_I4(int value);

        /// <summary>
        /// Pushes the given <see cref="sbyte"/> onto the stack.
        /// </summary>
        /// <param name="value">The short-form value to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_s"/>
        TEmitter Ldc_I4_S(sbyte value);

        /// <summary>
        /// Pushes the given <see cref="int"/> value of -1 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_m1"/>
        TEmitter Ldc_I4_M1();

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 0 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_0"/>
        TEmitter Ldc_I4_0();

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 1 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_1"/>
        TEmitter Ldc_I4_1();

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 2 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_2"/>
        TEmitter Ldc_I4_2();

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 3 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_3"/>
        TEmitter Ldc_I4_3();

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 4 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_4"/>
        TEmitter Ldc_I4_4();

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 5 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_5"/>
        TEmitter Ldc_I4_5();

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 6 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_6"/>
        TEmitter Ldc_I4_6();

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 7 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_7"/>
        TEmitter Ldc_I4_7();

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 8 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_8"/>
        TEmitter Ldc_I4_8();

        /// <summary>
        /// Pushes the given <see cref="long"/> onto the stack.
        /// </summary>
        /// <param name="value">The value to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i8"/>
        TEmitter Ldc_I8(long value);

        /// <summary>
        /// Pushes the given <see cref="float"/> onto the stack.
        /// </summary>
        /// <param name="value">The value to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_r4"/>
        TEmitter Ldc_R4(float value);

        /// <summary>
        /// Pushes the given <see cref="double"/> onto the stack.
        /// </summary>
        /// <param name="value">The value to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_r8"/>
        TEmitter Ldc_R8(double value);

        /// <summary>
        /// Pushes a <see langword="null"/> <see cref="object"/> onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldnull"/>
        TEmitter Ldnull();

        /// <summary>
        /// Pushes a <see cref="string"/> onto the stack.
        /// </summary>
        /// <param name="text">The <see cref="string"/> to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldstr"/>
        TEmitter Ldstr(string text);

        /// <summary>
        /// Converts a metadata token to its runtime representation and pushes it onto the stack.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to convert to a <see cref="RuntimeTypeHandle"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
        TEmitter Ldtoken(Type type);

        /// <summary>
        /// Converts a metadata token to its runtime representation and pushes it onto the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> to convert to a <see cref="RuntimeFieldHandle"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
        TEmitter Ldtoken(FieldInfo field);

        /// <summary>
        /// Converts a metadata token to its runtime representation and pushes it onto the stack.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> to convert to a <see cref="RuntimeMethodHandle"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
        TEmitter Ldtoken(MethodInfo method);

        /// <summary>
        /// Pushes the number of elements of a zero-based, one-dimensional <see cref="Array"/> onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldlen"/>
        TEmitter Ldlen();

        /// <summary>
        /// Loads the element from an array index onto the stack as the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the element to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem"/>
        TEmitter Ldelem(Type type);

        /// <summary>
        /// Loads the element from an array index onto the stack as the given <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the element to load.</typeparam>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem"/>
        TEmitter Ldelem<T>();

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="IntPtr"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="IntPtr"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i"/>
        TEmitter Ldelem_I();

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="sbyte"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="sbyte"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i1"/>
        TEmitter Ldelem_I1();

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="short"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="short"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i2"/>
        TEmitter Ldelem_I2();

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="int"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i4"/>
        TEmitter Ldelem_I4();

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="long"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="long"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i8"/>
        TEmitter Ldelem_I8();

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="byte"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="byte"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u1"/>
        TEmitter Ldelem_U1();

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="ushort"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="ushort"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u2"/>
        TEmitter Ldelem_U2();

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="uint"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="uint"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u4"/>
        TEmitter Ldelem_U4();

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="float"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="float"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_r4"/>
        TEmitter Ldelem_R4();

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="double"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="double"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_r8"/>
        TEmitter Ldelem_R8();

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="object"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="object"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_ref"/>
        TEmitter Ldelem_Ref();

        /// <summary>
        /// Loads the element from an array index onto the stack as an address to a value of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the element to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelema"/>
        TEmitter Ldelema(Type type);

        /// <summary>
        /// Loads the element from an array index onto the stack as an address to a value of the given <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the element to load.</typeparam>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelema"/>
        TEmitter Ldelema<T>();

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the value on the stack with the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the element to store.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem"/>
        TEmitter Stelem(Type type);

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the value on the stack with the given <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the element to store.</typeparam>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem"/>
        TEmitter Stelem<T>();

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="IntPtr"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="IntPtr"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i"/>
        TEmitter Stelem_I();

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="sbyte"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="sbyte"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i1"/>
        TEmitter Stelem_I1();

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="short"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="short"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i2"/>
        TEmitter Stelem_I2();

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="int"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="int"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i4"/>
        TEmitter Stelem_I4();

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="long"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="long"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i8"/>
        TEmitter Stelem_I8();

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="float"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="float"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_r4"/>
        TEmitter Stelem_R4();

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="double"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="double"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_r8"/>
        TEmitter Stelem_R8();

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="object"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="object"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_ref"/>
        TEmitter Stelem_Ref();

        /// <summary>
        /// Pushes an <see cref="object"/> reference to a new zero-based, one-dimensional <see cref="Array"/> whose elements are the given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <param name="type">The type of values that can be stored in the array.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newarr"/>
        TEmitter Newarr(Type type);

        /// <summary>
        /// Pushes an <see cref="object"/> reference to a new zero-based, one-dimensional <see cref="Array"/> whose elements are the given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of values that can be stored in the array.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newarr"/>
        TEmitter Newarr<T>();

        /// <summary>
        /// Specifies that the subsequent array address operation performs no type check at run time, and that it returns a managed pointer whose mutability is restricted.
        /// </summary>
        /// <remarks>This instruction can only appear before a <see cref="Jay.Reflection.Emission.Sandbox.FluentILGenerator{TEmitter}.Ldelema"/> instruction.</remarks>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.readonly"/>
        TEmitter Readonly();

        /// <summary>
        /// Loads the value of the given <see cref="FieldInfo"/> onto the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose value to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldfld"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsfld"/>
        TEmitter Ldfld(FieldInfo field);

        /// <summary>
        /// Loads the value of the given <see cref="FieldInfo"/> onto the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose value to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsfld"/>
        TEmitter Ldsfld(FieldInfo field);

        /// <summary>
        /// Loads the address of the given <see cref="FieldInfo"/> onto the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose address to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldflda"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsflda"/>
        TEmitter Ldflda(FieldInfo field);

        /// <summary>
        /// Loads the address of the given <see cref="FieldInfo"/> onto the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose address to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsflda"/>
        TEmitter Ldsflda(FieldInfo field);

        /// <summary>
        /// Replaces the value stored in the given <see cref="FieldInfo"/> with the value on the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose value to replace.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="NullReferenceException">If the instance value/pointer on the stack is <see langword="null"/> and the <paramref name="field"/> is not <see langword="static"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stfld"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stsfld"/>
        TEmitter Stfld(FieldInfo field);

        /// <summary>
        /// Replaces the value stored in the given static <see cref="FieldInfo"/> with the value on the stack.
        /// </summary>
        /// <param name="field">The static <see cref="FieldInfo"/> whose value to replace.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
        /// <exception cref="NullReferenceException">If the instance value/pointer on the stack is <see langword="null"/> and the <paramref name="field"/> is not <see langword="static"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stsfld"/>
        TEmitter Stsfld(FieldInfo field);

        /// <summary>
        /// Loads a value from an address onto the stack.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
        TEmitter Ldobj(Type type);

        /// <summary>
        /// Loads a value from an address onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
        TEmitter Ldobj<T>();

        /// <summary>
        /// Loads a value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
        /// <remarks>This is just an alias for Ldobj</remarks>
        TEmitter Ldind(Type type);

        /// <summary>
        /// Loads a value from an address onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
        /// <remarks>This is just an alias for Ldobj</remarks>
        TEmitter Ldind<T>();

        /// <summary>
        /// Loads a <see cref="IntPtr"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i"/>
        TEmitter Ldind_I();

        /// <summary>
        /// Loads a <see cref="sbyte"/> value from an address onto the stack as an <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i1"/>
        TEmitter Ldind_I1();

        /// <summary>
        /// Loads a <see cref="short"/> value from an address onto the stack as an <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i2"/>
        TEmitter Ldind_I2();

        /// <summary>
        /// Loads a <see cref="int"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i4"/>
        TEmitter Ldind_I4();

        /// <summary>
        /// Loads a <see cref="long"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i8"/>
        TEmitter Ldind_I8();

        /// <summary>
        /// Loads a <see cref="byte"/> value from an address onto the stack as an <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u1"/>
        TEmitter Ldind_U1();

        /// <summary>
        /// Loads a <see cref="ushort"/> value from an address onto the stack as an <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u2"/>
        TEmitter Ldind_U2();

        /// <summary>
        /// Loads a <see cref="uint"/> value from an address onto the stack onto the stack as an <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u4"/>
        TEmitter Ldind_U4();

        /// <summary>
        /// Loads a <see cref="float"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_r4"/>
        TEmitter Ldind_R4();

        /// <summary>
        /// Loads a <see cref="double"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_r8"/>
        TEmitter Ldind_R8();

        /// <summary>
        /// Loads a <see cref="object"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_ref"/>
        TEmitter Ldind_Ref();

        /// <summary>
        /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="TypeLoadException">If <paramref name="type"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
        TEmitter Stobj(Type type);

        /// <summary>
        /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
        /// </summary>
        /// <exception cref="TypeLoadException">If the given <see cref="Type"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
        TEmitter Stobj<T>();

        /// <summary>
        /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="TypeLoadException">If <paramref name="type"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
        /// <remarks>This is just an alias for Stobj</remarks>
        TEmitter Stind(Type type);

        /// <summary>
        /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
        /// </summary>
        /// <exception cref="TypeLoadException">If the given <see cref="Type"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
        /// <remarks>This is just an alias for Stobj</remarks>
        TEmitter Stind<T>();

        /// <summary>
        /// Stores a <see cref="IntPtr"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i"/>
        TEmitter Stind_I();

        /// <summary>
        /// Stores a <see cref="sbyte"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i1"/>
        TEmitter Stind_I1();

        /// <summary>
        /// Stores a <see cref="short"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i2"/>
        TEmitter Stind_I2();

        /// <summary>
        /// Stores a <see cref="int"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i4"/>
        TEmitter Stind_I4();

        /// <summary>
        /// Stores a <see cref="long"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i8"/>
        TEmitter Stind_I8();

        /// <summary>
        /// Stores a <see cref="float"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_r4"/>
        TEmitter Stind_R4();

        /// <summary>
        /// Stores a <see cref="double"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_r8"/>
        TEmitter Stind_R8();

        /// <summary>
        /// Stores a <see cref="object"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_ref"/>
        TEmitter Stind_Ref();

        /// <summary>
        /// Indicates that an address on the stack might not be aligned to the natural size of the immediately following
        /// <see cref="Jay.Reflection.Emission.Sandbox.FluentILGenerator{TEmitter}.Ldind"/>, <see cref="Jay.Reflection.Emission.Sandbox.FluentILGenerator{TEmitter}.Stind"/>, <see cref="Jay.Reflection.Emission.Sandbox.FluentILGenerator{TEmitter}.Ldfld"/>, <see cref="Jay.Reflection.Emission.Sandbox.FluentILGenerator{TEmitter}.Stfld"/>, <see cref="Jay.Reflection.Emission.Sandbox.FluentILGenerator{TEmitter}.Ldobj"/>, <see cref="Jay.Reflection.Emission.Sandbox.FluentILGenerator{TEmitter}.Stobj"/>, <see cref="Jay.Reflection.Emission.Sandbox.FluentILGenerator{TEmitter}.Initblk"/>, or <see cref="Jay.Reflection.Emission.Sandbox.FluentILGenerator{TEmitter}.Cpblk"/> instruction.
        /// </summary>
        /// <param name="alignment">Specifies the generated code should assume the address is <see cref="byte"/>, double-<see cref="byte"/>, or quad-<see cref="byte"/> aligned.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="alignment"/> is not 1, 2, or 4.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unaligned"/>
        TEmitter Unaligned(int alignment);

        /// <summary>
        /// Indicates that an address currently on the stack might be volatile, and the results of reading that location cannot be cached or that multiple stores to that location cannot be suppressed.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.volatile"/>
        TEmitter Volatile();

        /// <summary>
        /// Pushes a typed reference to an instance of a given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of reference to push.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mkrefany"/>
        TEmitter Mkrefany(Type type);

        /// <summary>
        /// Pushes a typed reference to an instance of a given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of reference to push.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mkrefany"/>
        TEmitter Mkrefany<T>();

        /// <summary>
        /// Retrieves the type token embedded in a typed reference.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanytype"/>
        TEmitter Refanytype();

        /// <summary>
        /// Retrieves the address (<see langword="&amp;"/>) embedded in a typed reference.
        /// </summary>
        /// <param name="type">The type of reference to retrieve the address.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        /// <exception cref="InvalidCastException">If <paramref name="type"/> is not the same as the <see cref="Type"/> of the reference.</exception>
        /// <exception cref="TypeLoadException">If <paramref name="type"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanyval"/>
        TEmitter Refanyval(Type type);

        /// <summary>
        /// Retrieves the address (<see langword="&amp;"/>) embedded in a typed reference.
        /// </summary>
        /// <typeparam name="T">The type of reference to retrieve the address.</typeparam>
        /// <exception cref="InvalidCastException">If <typeparamref name="T"/> is not the same as the <see cref="Type"/> of the reference.</exception>
        /// <exception cref="TypeLoadException">If <typeparamref name="T"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanyval"/>
        TEmitter Refanyval<T>();

        /// <summary>
        /// Pushes the size, in <see cref="byte"/>s, of a given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get the size of.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sizeof"/>
        TEmitter Sizeof(Type type);

        /// <summary>
        /// Pushes the size, in <see cref="byte"/>s, of a given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to get the size of.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if the given <see cref="Type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sizeof"/>
        TEmitter Sizeof<T>();

        TEmitter Append(IEnumerable<Instruction> instructions);

        /// <inheritdoc />
        string ToString();

        /// <inheritdoc />
        Instruction this[int index] { get; }

        int Count { get; }

        /// <inheritdoc />
        IEnumerator<Instruction> GetEnumerator();
    }
}