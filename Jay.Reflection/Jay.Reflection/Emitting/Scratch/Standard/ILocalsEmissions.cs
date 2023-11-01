using Jay.Reflection.Emitting.Scratch.Simple;

namespace Jay.Reflection.Emitting.Scratch.Standard;

public interface ILocalsEmissions<out Self>
    where Self : IEmitter<Self>
{
#region Declare
    /// <summary>
    /// Declares a <see cref="EmitterLocal"/> variable of the specified <see cref="Type"/>
    /// </summary>
    /// <param name="type">The type of the local variable</param>
    /// <param name="local">Outputs the declared local variable</param>
    /// <param name="localName">The name of the local variable</param>
    /// <exception cref="ArgumentNullException">
    /// If <paramref name="type"/> is <see langword="null"/>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// If <paramref name="type"/> was created with <see cref="M:TypeBuilder.CreateType"/>
    /// </exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
    Self DeclareLocal(
        Type type,
        out EmitterLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null);

    /// <summary>
    /// Declares a <see cref="EmitterLocal"/> variable of the generic type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of the local variable</typeparam>
    /// <param name="local">Outputs the declared local variable</param>
    /// <param name="localName">The name of the local variable</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
    Self DeclareLocal<T>(
        out EmitterLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null);

    /// <summary>
    /// Declares a <see cref="EmitterLocal"/> variable of the specified <see cref="Type"/>
    /// </summary>
    /// <param name="type">The type of the local variable</param>
    /// <param name="isPinned">Whether or not the local variable should be pinned in memory</param>
    /// <param name="local">Outputs the declared local variable</param>
    /// <param name="localName">The name of the local variable</param>
    /// <exception cref="ArgumentNullException">
    /// If <paramref name="type"/> is <see langword="null"/>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// If <paramref name="type"/> was created with <see cref="M:TypeBuilder.CreateType"/>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// If the method body of the enclosing method was created with <see cref="M:MethodBuilder.CreateMethodBody"/>
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// If the method this Emitter is associated with is not wrapping a <see cref="MethodBuilder"/>
    /// </exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_System_Boolean_"/>
    Self DeclareLocal(
        Type type,
        bool isPinned,
        out EmitterLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null);

    /// <summary>
    /// Declares a <see cref="EmitterLocal"/> variable of the generic type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of the local variable</typeparam>
    /// <param name="isPinned">Whether or not the local variable should be pinned in memory</param>
    /// <param name="local">Outputs the declared local variable</param>
    /// <param name="localName">The name of the local variable</param>
    /// <exception cref="InvalidOperationException">
    /// If <typeparamref name="T"/> was created with <see cref="M:TypeBuilder.CreateType"/>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// If the method body of the enclosing method was created with <see cref="M:MethodBuilder.CreateMethodBody"/>
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// If the method this Emitter is associated with is not wrapping a <see cref="MethodBuilder"/>
    /// </exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_System_Boolean_"/>
    Self DeclareLocal<T>(
        bool isPinned,
        out EmitterLocal local,
        [CallerArgumentExpression(nameof(local))]
        string? localName = null);
#endregion

#region Load
    /// <summary>
    /// Loads the given <see cref="EmitterLocal"/>'s value onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
    Self Ldloc(EmitterLocal emitterLocal);

    /// <summary>
    /// Loads the given short-form <see cref="EmitterLocal"/>'s value onto the stack.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="local"/> is not short-form.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
    Self Ldloc_S(EmitterLocal local);

    /// <summary>
    /// Loads the given <see cref="EmitterLocal"/>'s value onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
    Self Ldloc(int index);

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 0 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_0"/>
    Self Ldloc_0();

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 1 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_1"/>
    Self Ldloc_1();

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 2 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_2"/>
    Self Ldloc_2();

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 3 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_3"/>
    Self Ldloc_3();

    /// <summary>
    /// Loads the address of the given <see cref="EmitterLocal"/> variable.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca_s"/>
    Self Ldloca(EmitterLocal local);

    /// <summary>
    /// Loads the address of the given short-form <see cref="EmitterLocal"/> variable.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="local"/> is not short-form.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca_s"/>
    Self Ldloca_S(EmitterLocal local);

    /// <summary>
    /// Loads the address of the given <see cref="EmitterLocal"/> variable.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca_s"/>
    Self Ldloca(int index);
#endregion

#region Store
    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the given <see cref="EmitterLocal"/>.
    /// </summary>
    /// <param name="local">The <see cref="EmitterLocal"/> to store the value in.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
    Self Stloc(EmitterLocal local);

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the given short-form <see cref="EmitterLocal"/>.
    /// </summary>
    /// <param name="local">The short-form <see cref="EmitterLocal"/> to store the value in.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
    Self Stloc_S(EmitterLocal local);

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the given <see cref="EmitterLocal"/>.
    /// </summary>
    /// <param name="index">The <see cref="EmitterLocal"/> to store the value in.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
    Self Stloc(int index);

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 0.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_0"/>
    Self Stloc_0();

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 1.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_1"/>
    Self Stloc_1();

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 2.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_2"/>
    Self Stloc_2();

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 3.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_3"/>
    Self Stloc_3();
#endregion
}