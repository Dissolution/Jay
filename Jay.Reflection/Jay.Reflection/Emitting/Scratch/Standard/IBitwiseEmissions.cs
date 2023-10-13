namespace Jay.Reflection.Emitting.Scratch;

public interface IBitwiseEmissions<out Self>
    where Self : IEmitter<Self>
{
    /// <summary>
    /// Computes the bitwise AND (<see langword="&amp;"/>) of two values and pushes the result onto the stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.and?view=netcore-3.0"/>
    Self And();

    /// <summary>
    /// Negates a value (<see langword="-"/>) and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.neg"/>
    Self Neg();

    /// <summary>
    /// Computes the one's complement (<see langword="~"/>) of a value and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.not"/>
    Self Not();

    /// <summary>
    /// Computes the bitwise OR (<see langword="|"/>) of two values and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.or"/>
    Self Or();

    /// <summary>
    /// Shifts an integer value to the left (<see langword="&lt;&lt;"/>) by a specified number of bits, pushing the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shl"/>
    Self Shl();

    /// <summary>
    /// Shifts an integer value to the right (<see langword="&gt;&gt;"/>) by a specified number of bits, pushing the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shr"/>
    Self Shr();

    /// <summary>
    /// Shifts an unsigned integer value to the right (<see langword="&gt;&gt;"/>) by a specified number of bits, pushing the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shr_un"/>
    Self Shr_Un();

    /// <summary>
    /// Computes the bitwise XOR (<see langword="^"/>) of a value and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.xor"/>
    Self Xor();
}