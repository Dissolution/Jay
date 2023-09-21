namespace Jay.Reflection.Emitting.Scratch;

public interface IPrefixEmissions<out Self>
    where Self : IEmitter<Self>
{
    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix1"/>
    [Obsolete("This is a reserved instruction.")]
    Self Prefix1();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix2"/>
    [Obsolete("This is a reserved instruction.")]
    Self Prefix2();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix3"/>
    [Obsolete("This is a reserved instruction.")]
    Self Prefix3();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix4"/>
    [Obsolete("This is a reserved instruction.")]
    Self Prefix4();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix5"/>
    [Obsolete("This is a reserved instruction.")]
    Self Prefix5();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix6"/>
    [Obsolete("This is a reserved instruction.")]
    Self Prefix6();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix7"/>
    [Obsolete("This is a reserved instruction.")]
    Self Prefix7();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefixref"/>
    [Obsolete("This is a reserved instruction.")]
    Self Prefixref();
}