using System.Reflection.Emit;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Global

namespace Jay.Reflection.Emission;

public interface IILGenerator<out TThis>
    where TThis : class, IILGenerator<TThis>
{
    int ILOffset { get; }

    InstructionStream Instructions { get; }

    /// <summary>
    /// Begins an exception block for a non-filtered exception.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> for the end of the block. This will leave you in the correct place to execute <see langword="finally"/> blocks or to finish the <see langword="try"/>.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptionblock?view=netcore-3.0"/>
    TThis BeginExceptionBlock(out Label label);
    /// <summary>
    /// Begins a <see langword="catch"/> block.
    /// </summary>
    /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="exceptionType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> is not an <see cref="Exception"/> type.</exception>
    /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock?view=netcore-3.0"/>
    TThis BeginCatchBlock(Type exceptionType);
    /// <summary>
    /// Begins a <see langword="catch"/> block.
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</typeparam>
    /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock?view=netcore-3.0"/>
    TThis BeginCatchBlock<TException>() where TException : Exception => BeginCatchBlock(typeof(TException));
    /// <summary>
    /// Begins a <see langword="finally"/> block in the stream.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfinallyblock?view=netcore-3.0"/>
    TThis BeginFinallyBlock();
    /// <summary>
    /// Begins an exception block for a filtered exception.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <exception cref="NotSupportedException">This <see cref="IILGenerator{T}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptfilterblock?view=netcore-3.0"/>
    TThis BeginExceptFilterBlock();
    /// <summary>
    /// Begins an exception fault block in the stream.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <exception cref="NotSupportedException">This <see cref="IILGenerator{T}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfaultblock?view=netcore-3.0"/>
    TThis BeginFaultBlock();
    /// <summary>
    /// Ends an exception block.
    /// </summary>
    /// <exception cref="InvalidOperationException">If this operation occurs in an unexpected place in the stream.</exception>
    /// <exception cref="NotSupportedException">If the stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endexceptionblock?view=netcore-3.0"/>
    TThis EndExceptionBlock();

    /// <summary>
    /// Begins a lexical scope.
    /// </summary>
    /// <exception cref="NotSupportedException">This <see cref="IILGenerator{T}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginscope?view=netcore-3.0"/>
    TThis BeginScope();
    /// <summary>
    /// Ends a lexical scope.
    /// </summary>
    /// <exception cref="NotSupportedException">If this <see cref="IILGenerator{T}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endscope?view=netcore-3.0"/>
    TThis EndScope();

    /// <summary>
    /// Specifies the <see langword="namespace"/> to be used in evaluating locals and watches for the current active lexical scope.
    /// </summary>
    /// <param name="usingNamespace">The namespace to be used in evaluating locals and watches for the current active lexical scope.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="usingNamespace"/> is <see langword="null"/> or has a Length of 0.</exception>
    /// <exception cref="NotSupportedException">If this <see cref="IILGenerator{T}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.usingnamespace?view=netcore-3.0"/>
    TThis UsingNamespace(string usingNamespace);

    /// <summary>
    /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="localType">The type of the <see cref="LocalBuilder"/>.</param>
    /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="localType"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="localType"/> was created with <see cref="TypeBuilder.CreateType"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=netcore-3.0#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
    TThis DeclareLocal(Type localType, out LocalBuilder local);
    /// <summary>
    /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="LocalBuilder"/>.</typeparam>
    /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=netcore-3.0#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
    TThis DeclareLocal<T>(out LocalBuilder local) => DeclareLocal(typeof(T), out local);
    /// <summary>
    /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="localType">The type of the <see cref="LocalBuilder"/>.</param>
    /// <param name="pinned">Whether or not the <see cref="LocalBuilder"/> should be pinned in memory.</param>
    /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="localType"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="localType"/> was created with <see cref="TypeBuilder.CreateType"/>.</exception>
    /// <exception cref="InvalidOperationException">If the method body of the enclosing method was created with <see cref="M:MethodBuilder.CreateMethodBody"/>.</exception>
    /// <exception cref="NotSupportedException">If the method this <see cref="IILGenerator{T}"/> is associated with is not wrapping a <see cref="MethodBuilder"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=netcore-3.0#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_System_Boolean_"/>
    TThis DeclareLocal(Type localType, bool pinned, out LocalBuilder local);
    /// <summary>
    /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="LocalBuilder"/>.</typeparam>
    /// <param name="pinned">Whether or not the <see cref="LocalBuilder"/> should be pinned in memory.</param>
    /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
    /// <exception cref="InvalidOperationException">If the method body of the enclosing method was created with <see cref="M:MethodBuilder.CreateMethodBody"/>.</exception>
    /// <exception cref="NotSupportedException">If the method this <see cref="IILGenerator{T}"/> is associated with is not wrapping a <see cref="MethodBuilder"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=netcore-3.0#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_System_Boolean_"/>
    TThis DeclareLocal<T>(bool pinned, out LocalBuilder local) => DeclareLocal(typeof(T), pinned, out local);

    /// <summary>
    /// Declares a new <see cref="Label"/>.
    /// </summary>
    /// <param name="label">Returns the new <see cref="Label"/> that can be used for branching.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.definelabel?view=netcore-3.0"/>
    TThis DefineLabel(out Label label);
    /// <summary>
    /// Marks the stream's current position with the given <see cref="Label"/>.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> for which to set an index.</param>
    /// <exception cref="ArgumentException">If the <paramref name="label"/> has an invalid index.</exception>
    /// <exception cref="ArgumentException">If the <paramref name="label"/> has already been marked.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.marklabel?view=netcore-3.0"/>
    TThis MarkLabel(Label label);

    /// <summary>
    /// Puts a <see cref="OpCodes.Call"/>, <see cref="OpCodes.Callvirt"/>, or <see cref="OpCodes.Newobj"/> instruction onto the stream to call a <see langword="varargs"/> <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="methodInfo">The <see langword="varargs"/> <see cref="MethodInfo"/> to be called.</param>
    /// <param name="optionalParameterTypes">The types of the Option arguments if the method is a <see langword="varargs"/> method; otherwise, <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="methodInfo"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcall?view=netcore-3.0"/>
    TThis EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes);
    /// <summary>
    /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call.
    /// </summary>
    /// <param name="callingConvention">The managed calling conventions to be used.</param>
    /// <param name="returnType">The <see cref="Type"/> of the result.</param>
    /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
    /// <param name="optionalParameterTypes">The types of the Option arguments for <see langword="varargs"/> calls.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="returnType"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="optionalParameterTypes"/> is not <see langword="null"/> or empty but <paramref name="callingConvention"/> does not include the <see cref="CallingConventions.VarArgs"/> flag.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli?view=netcore-3.0#System_Reflection_Emit_ILGenerator_EmitCalli_System_Reflection_Emit_OpCode_System_Reflection_CallingConventions_System_Type_System_Type___System_Type___"/>
    TThis EmitCalli(CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, params Type[]? optionalParameterTypes);

    /// <summary>
    /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call.
    /// </summary>
    /// <param name="unmanagedCallConv">The unmanaged calling convention to be used.</param>
    /// <param name="returnType">The <see cref="Type"/> of the result.</param>
    /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="returnType"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli?view=netcore-3.0#System_Reflection_Emit_ILGenerator_EmitCalli_System_Reflection_Emit_OpCode_System_Runtime_InteropServices_CallingConvention_System_Type_System_Type___"/>
    TThis EmitCalli(CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_"/>
    TThis Emit(OpCode opCode);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="byte"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Byte_"/>
    TThis Emit(OpCode opCode, byte arg);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="sbyte"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_SByte_"/>
    TThis Emit(OpCode opCode, sbyte arg);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="short"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int16_"/>
    TThis Emit(OpCode opCode, short arg);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="int"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int32_"/>
    TThis Emit(OpCode opCode, int arg);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="long"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int64_"/>
    TThis Emit(OpCode opCode, long arg);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="float"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Single_"/>
    TThis Emit(OpCode opCode, float arg);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="double"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="arg">The numeric value to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Double_"/>
    TThis Emit(OpCode opCode, double arg);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="string"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="str">The <see cref="string"/>to emit.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_String_"/>
    TThis Emit(OpCode opCode, string str);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream and leaves space to include a <see cref="Label"/> when fixes are done.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="label">The <see cref="Label"/> to branch from this location.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_Label_"/>
    TThis Emit(OpCode opCode, Label label);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream and leaves space to include a <see cref="Label"/> when fixes are done.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="labels">The <see cref="Label"/>s of which to branch to from this locations.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="labels"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException">If <paramref name="labels"/> is empty.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_Label___"/>
    TThis Emit(OpCode opCode, params Label[] labels);
    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the index of the given <see cref="LocalBuilder"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="local">The <see cref="LocalBuilder"/> to emit the index of.</param>
    /// <exception cref="InvalidOperationException">If <paramref name="opCode"/> is a single-byte instruction and <paramref name="local"/> has an index greater than <see cref="byte.MaxValue"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_LocalBuilder_"/>
    TThis Emit(OpCode opCode, LocalBuilder local);


    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="FieldInfo"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="field">The <see cref="ArgumentNullException"/> to emit.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_FieldInfo_"/>
    TThis Emit(OpCode opCode, FieldInfo field);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="ConstructorInfo"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="ctor">The <see cref="ConstructorInfo"/> to emit.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="ctor"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_ConstructorInfo_"/>
    TThis Emit(OpCode opCode, ConstructorInfo ctor);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="method">The <see cref="MethodInfo"/> to emit.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException">If <paramref name="method"/> is a generic method for which <see cref="MethodBase.IsGenericMethodDefinition"/> is <see langword="false"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_MethodInfo_"/>
    TThis Emit(OpCode opCode, MethodInfo method);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="Type"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="type">The <see cref="Type"/> to emit.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Type_"/>
    TThis Emit(OpCode opCode, Type type);

    /// <summary>
    /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="SignatureHelper"/>.
    /// </summary>
    /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
    /// <param name="signature">A helper for constructing a signature token.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="signature"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=netcore-3.0#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_SignatureHelper_"/>
    TThis Emit(OpCode opCode, SignatureHelper signature);
}