using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Jay.Reflection.Emission;

public interface IGenEmitter<TEmitter> : IEmitter
    where TEmitter : IGenEmitter<TEmitter>
{
    int ILOffset { get; }

    #region Try / Catch / Finally
    /// <summary>
    /// Begins a <see langword="catch"/> block.
    /// </summary>
    /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="exceptionType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> is not an <see cref="Exception"/> type.</exception>
    /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock?view=netcore-3.0"/>
    TEmitter BeginCatchBlock(Type exceptionType);

    /// <summary>
    /// Begins a <see langword="catch"/> block.
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</typeparam>
    /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock?view=netcore-3.0"/>
    TEmitter BeginCatchBlock<TException>() where TException : Exception => BeginCatchBlock(typeof(TException));

    /// <summary>
    /// Begins an exception block for a filtered exception.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <exception cref="NotSupportedException">This <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptfilterblock?view=netcore-3.0"/>
    TEmitter BeginExceptFilterBlock();

    /// <summary>
    /// Begins an exception block for a non-filtered exception.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> for the end of the block. This will leave you in the correct place to execute <see langword="finally"/> blocks or to finish the <see langword="try"/>.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptionblock?view=netcore-3.0"/>
    TEmitter BeginExceptionBlock(out Label label);

    /// <summary>
    /// Ends an exception block.
    /// </summary>
    /// <exception cref="InvalidOperationException">If this operation occurs in an unexpected place in the stream.</exception>
    /// <exception cref="NotSupportedException">If the stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endexceptionblock?view=netcore-3.0"/>
    TEmitter EndExceptionBlock();

    /// <summary>
    /// Begins an exception fault block in the stream.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <exception cref="NotSupportedException">This <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfaultblock?view=netcore-3.0"/>
    TEmitter BeginFaultBlock();

    /// <summary>
    /// Begins a <see langword="finally"/> block in the stream.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfinallyblock?view=netcore-3.0"/>
    TEmitter BeginFinallyBlock();
    #endregion

    #region Scope

    /// <summary>
    /// Begins a lexical scope.
    /// </summary>
    /// <exception cref="NotSupportedException">This <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginscope?view=netcore-3.0"/>
    TEmitter BeginScope();

    /// <summary>
    /// Ends a lexical scope.
    /// </summary>
    /// <exception cref="NotSupportedException">If this <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endscope?view=netcore-3.0"/>
    TEmitter EndScope();

    /// <summary>
    /// Specifies the <see langword="namespace"/> to be used in evaluating locals and watches for the current active lexical scope.
    /// </summary>
    /// <param name="namespace">The namespace to be used in evaluating locals and watches for the current active lexical scope.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="namespace"/> is <see langword="null"/> or has a Length of 0.</exception>
    /// <exception cref="NotSupportedException">If this <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.usingnamespace?view=netcore-3.0"/>
    TEmitter UsingNamespace(string @namespace);

    #endregion

    #region Locals
    /// <summary>
    /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="localType">The type of the <see cref="LocalBuilder"/>.</param>
    /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="localType"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="localType"/> was created with <see cref="TypeBuilder.CreateType"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=netcore-3.0#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
    TEmitter DeclareLocal(Type localType, out LocalBuilder local);

    /// <summary>
    /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="LocalBuilder"/>.</typeparam>
    /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=netcore-3.0#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
    TEmitter DeclareLocal<T>(out LocalBuilder local) => DeclareLocal(typeof(T), out local);

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
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=netcore-3.0#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_System_Boolean_"/>
    TEmitter DeclareLocal(Type localType, bool pinned, out LocalBuilder local);

    /// <summary>
    /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="LocalBuilder"/>.</typeparam>
    /// <param name="pinned">Whether or not the <see cref="LocalBuilder"/> should be pinned in memory.</param>
    /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
    /// <exception cref="InvalidOperationException">If the method body of the enclosing method was created with <see cref="M:MethodBuilder.CreateMethodBody"/>.</exception>
    /// <exception cref="NotSupportedException">If the method this <see cref="ILEmitter"/> is associated with is not wrapping a <see cref="MethodBuilder"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=netcore-3.0#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_System_Boolean_"/>
    TEmitter DeclareLocal<T>(bool pinned, out LocalBuilder local) => DeclareLocal(typeof(T), pinned, out local);
    #endregion

    #region Labels

    /// <summary>
    /// Declares a new <see cref="Label"/>.
    /// </summary>
    /// <param name="label">Returns the new <see cref="Label"/> that can be used for branching.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.definelabel?view=netcore-3.0"/>
    TEmitter DefineLabel(out Label label);

    /// <summary>
    /// Marks the stream's current position with the given <see cref="Label"/>.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> for which to set an index.</param>
    /// <exception cref="ArgumentException">If the <paramref name="label"/> has an invalid index.</exception>
    /// <exception cref="ArgumentException">If the <paramref name="label"/> has already been marked.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.marklabel?view=netcore-3.0"/>
    TEmitter MarkLabel(Label label);
    //
    // /// <summary>
    // /// Declares a new <see cref="Label"/> and marks the stream's current position with it.
    // /// </summary>
    // /// <param name="label">Returns the new <see cref="Label"/> marked with the stream's current position.</param>
    // TEmitter DefineAndMarkLabel(out Label label) => DefineLabel(out label).MarkLabel(label);
    #endregion

    #region Method-Related (Call)

    /// <summary>
    /// Puts a <see cref="OpCodes.Call"/>, <see cref="OpCodes.Callvirt"/>, or <see cref="OpCodes.Newobj"/> instruction onto the stream to call a <see langword="varargs"/> <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="method">The <see langword="varargs"/> <see cref="MethodInfo"/> to be called.</param>
    /// <param name="optionParameterTypes">The types of the Option arguments if the method is a <see langword="varargs"/> method; otherwise, <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcall?view=netcore-3.0"/>
    TEmitter Call(MethodInfo method, params Type[] optionParameterTypes);

    /// <summary>
    /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call.
    /// </summary>
    /// <param name="convention">The unmanaged calling convention to be used.</param>
    /// <param name="returnType">The <see cref="Type"/> of the result.</param>
    /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="returnType"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli?view=netcore-3.0#System_Reflection_Emit_ILGenerator_EmitCalli_System_Reflection_Emit_OpCode_System_Runtime_InteropServices_CallingConvention_System_Type_System_Type___"/>
    TEmitter Calli(CallingConvention convention, Type returnType, params Type[] parameterTypes);

    /// <summary>
    /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call.
    /// </summary>
    /// <param name="conventions">The managed calling conventions to be used.</param>
    /// <param name="returnType">The <see cref="Type"/> of the result.</param>
    /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
    /// <param name="optionParameterTypes">The types of the Option arguments for <see langword="varargs"/> calls.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="returnType"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="optionParameterTypes"/> is not <see langword="null"/> or empty but <paramref name="conventions"/> does not include the <see cref="CallingConventions.VarArgs"/> flag.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli?view=netcore-3.0#System_Reflection_Emit_ILGenerator_EmitCalli_System_Reflection_Emit_OpCode_System_Reflection_CallingConventions_System_Type_System_Type___System_Type___"/>
    TEmitter Calli(CallingConventions conventions, Type returnType, Type[] parameterTypes, params Type[] optionParameterTypes);
    #endregion

    IWriter<TEmitter> Write { get; }

    #region Exceptions

    /// <summary>
    /// Emits the instructions to throw an <see cref="Exception"/>.
    /// </summary>
    /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/> to throw.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="exceptionType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> is not an <see cref="Exception"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> does not have a default constructor.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.throwexception?view=netcore-3.0"/>
    TEmitter ThrowException(Type exceptionType);

    /// <summary>
    /// Emits the instructions to throw an <see cref="Exception"/>.
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/> to throw.</typeparam>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.throwexception?view=netcore-3.0"/>
    TEmitter ThrowException<TException>()
        where TException : Exception, new() => ThrowException(typeof(TException));
    #endregion
}