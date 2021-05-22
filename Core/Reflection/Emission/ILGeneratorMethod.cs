using System.Reflection.Emit;

namespace Jay.Reflection.Emission
{
    /// <summary>
    /// Specifies a method on an <see cref="ILGenerator"/>.
    /// </summary>
    public enum ILGeneratorMethod
    {
        None = 0,
        /// <summary>
        /// Begins a <see langword="catch"/> block.
        /// </summary>
        BeginCatchBlock,
        /// <summary>
        /// Begins an exception block for a filtered exception.
        /// </summary>
        BeginExceptFilterBlock,
        /// <summary>
        /// Begins an exception block for a non-filtered exception.
        /// </summary>
        BeginExceptionBlock,
        /// <summary>
        /// Begins an exception fault block in the stream.
        /// </summary>
        BeginFaultBlock,
        /// <summary>
        /// Begins a <see langword="finally"/> block in the stream.
        /// </summary>
        BeginFinallyBlock,
        /// <summary>
        /// Begins a lexical scope.
        /// </summary>
        BeginScope,
        /// <summary>
        /// Writing a value with <see cref="M:System.Console.WriteLine"/>.
        /// </summary>
        WriteLine,
        /// <summary>
        /// Ends an exception block.
        /// </summary>
        EndExceptionBlock,
        /// <summary>
        /// Ends a lexical scope.
        /// </summary>
        EndScope,
        /// <summary>
        /// Declares a <see cref="LocalBuilder"/>.
        /// </summary>
        DeclareLocal,
        /// <summary>
        /// Declares a new <see cref="Label"/>.
        /// </summary>
        DefineLabel,
        /// <summary>
        /// Marks the stream's current position with a <see cref="Label"/>.
        /// </summary>
        MarkLabel,
        /// <summary>
        /// Emits the instructions to throw an &lt;see cref="Exception"/&gt;.
        /// </summary>
        ThrowException,
        /// <summary>
        /// Specifies the <see langword="namespace"/> to be used in evaluating locals and watches for the current active lexical scope.
        /// </summary>
        UsingNamespace,
        /// <summary>
        /// Emitting a varargs call
        /// </summary>
        EmitCall,
        /// <summary>
        /// Emitting a <see cref="M:ILGenerator.Calli"/> call
        /// </summary>
        EmitCalli,
    }
}