using System.Runtime.InteropServices;

namespace Jay.Reflection.Emitting.Scratch;

public interface IMethodEmissions<out Self>
    where Self : IEmitter<Self>
{
    /// <summary>
    /// Puts a <see cref="OpCodes.Call"/>, <see cref="OpCodes.Callvirt"/>, or <see cref="OpCodes.Newobj"/> instruction onto the stream to call a <see langword="varargs"/> <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="methodInfo">The <see langword="varargs"/> <see cref="MethodInfo"/> to be called.</param>
    /// <param name="optionalParameterTypes">The types of the Option arguments if the method is a <see langword="varargs"/> method; otherwise, <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="methodInfo"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcall"/>
    Self EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes);

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
    Self EmitCalli(CallingConventions callingConventions, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes);

    /// <summary>
    /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call.
    /// </summary>
    /// <param name="callingConvention">The unmanaged calling convention to be used.</param>
    /// <param name="returnType">The <see cref="Type"/> of the result.</param>
    /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="returnType"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli#System_Reflection_Emit_ILGenerator_EmitCalli_System_Reflection_Emit_OpCode_System_Runtime_InteropServices_CallingConvention_System_Type_System_Type___"/>
    Self EmitCalli(CallingConvention callingConvention, Type? returnType, Type[]? parameterTypes);

    /// <summary>
    /// Calls the given <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> that will be called.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.call"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.callvirt"/>
    Self Call(MethodBase method);

    /// <summary>
    /// Calls the given late-bound <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> that will be called.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.callvirt"/>
    Self Callvirt(MethodInfo method);

    /// <summary>
    /// Constrains the <see cref="Type"/> on which a virtual method call (<see cref="OpCodes.Callvirt"/>) is made.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to constrain the <see cref="OpCodes.Callvirt"/> upon.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained?view=netcore-3.0"/>
    Self Constrained(Type type);

    /// <summary>
    /// Constrains the <see cref="Type"/> on which a virtual method call (<see cref="OpCodes.Callvirt"/>) is made.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> to constrain the <see cref="OpCodes.Callvirt"/> upon.</typeparam>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained?view=netcore-3.0"/>
    Self Constrained<T>();

    /// <summary>
    /// Pushes an unmanaged pointer (<see cref="IntPtr"/>) to the native code implementing the given <see cref="MethodInfo"/> onto the stack.
    /// </summary>
    /// <param name="method">The method to get pointer to.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldftn"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldvirtftn"/>
    Self Ldftn(MethodInfo method);

    /// <summary>
    /// Pushes an unmanaged pointer (<see cref="IntPtr"/>) to the native code implementing the given virtual <see cref="MethodInfo"/> onto the stack.
    /// </summary>
    /// <param name="method">The method to get pointer to.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldvirtftn"/>
    Self Ldvirtftn(MethodInfo method);

    /// <summary>
    /// Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual call instruction is executed.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.tailcall"/>
    Self Tailcall();
}