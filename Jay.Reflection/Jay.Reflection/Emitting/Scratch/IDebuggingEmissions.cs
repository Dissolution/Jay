namespace Jay.Reflection.Emitting.Scratch;

public interface IDebuggingEmissions<out Self>
    where Self : IEmitter<Self>
{
    /// <summary>
    /// Signals the Common Language Infrastructure (CLI) to inform the debugger that a breakpoint has been tripped.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.break"/>
    Self Break();

    /// <summary>
    /// Fills space if opcodes are patched. No meaningful operation is performed, although a processing cycle can be consumed.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.nop"/>
    Self Nop();
}