using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

// ReSharper disable IdentifierTypo

namespace Jay.Reflection.Emission
{
    internal class FluentOpEmitter<TEmitter> : IFluentOpEmitter<TEmitter>
        where TEmitter : FluentOpEmitter<TEmitter>
    {
        protected readonly List<Instruction> _instructions;

        public int Count => _instructions.Count;
        public TEmitter Emit(OpCode opCode)
        {
            throw new NotImplementedException();
        }

        public FluentOpEmitter()
        {
            _instructions = new List<Instruction>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool UseShort(Label label)
        {
            int index = label.GetHashCode();
            return index <= sbyte.MaxValue &&
                   index >= sbyte.MinValue;
        }
    }
    
    public interface IFluentOpEmitter<TEmitter>
        where TEmitter : IFluentOpEmitter<TEmitter>
    {
        /// <summary>
        /// Gets the number of <see cref="OpCode"/>s that have been emitted
        /// </summary>
        int Count { get; }

        TEmitter Emit(OpCode opCode);
        
    }

    public interface IFluentILEmitter<TEmitter> : IFluentOpEmitter<TEmitter>
        where TEmitter : IFluentILEmitter<TEmitter>
    {
        #region Try/Catch/Finally
        /// <summary>
        /// Begins a <see langword="catch"/> block.
        /// </summary>
        /// <param name="exceptionType">
        /// The <see cref="Type"/> of <see cref="Exception"/>s to catch.
        /// </param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock"/>
        TEmitter BeginCatchBlock(Type exceptionType);

        /// <summary>
        /// Begins a <see langword="catch"/> block.
        /// </summary>
        /// <typeparam name="TException">
        /// The <see cref="Type"/> of <see cref="Exception"/>s to catch.
        /// </param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock"/>
        TEmitter BeginCatchBlock<TException>()
            where TException : Exception
            => BeginCatchBlock(typeof(TException));

        /// <summary>
        /// Begins an exception block for a filtered exception.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The stream being emitted is not currently in an exception block.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// This <typeparamref name="TEmitter"/> belongs to a <see cref="DynamicMethod"/>.
        /// </exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptfilterblock"/>
        TEmitter BeginExceptFilterBlock();

        /// <summary>
        /// Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfilter"/>
        TEmitter Endfilter() => Emit(OpCodes.Endfilter);

        /// <summary>
        /// Begins an exception block for a non-filtered exception.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> for the end of the block. This will leave you in the correct place to execute <see langword="finally"/> blocks or to finish the <see langword="try"/>.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptionblock"/>
        TEmitter BeginExceptionBlock(out Label label);

        /// <summary>
        /// Ends an exception block.
        /// </summary>
        /// <exception cref="InvalidOperationException">If this operation occurs in an unexpected place in the stream.</exception>
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
        TEmitter Endfault() => Emit(OpCodes.Endfinally);

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
        TEmitter Endfinally() => Emit(OpCodes.Endfinally);
        #endregion
        
        #region Scope

        /// <summary>
        /// Begins a lexical scope.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// This <typeparamref name="TEmitter"/> belongs to a <see cref="DynamicMethod"/>.
        /// </exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginscope"/>
        TEmitter BeginScope();

        /// <summary>
        /// Ends a lexical scope.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// If this <typeparamref name="TEmitter"/> belongs to a <see cref="DynamicMethod"/>.
        /// </exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endscope"/>
        TEmitter EndScope();

        /// <summary>
        /// Specifies the <see langword="namespace"/> to be used in evaluating locals and watches for the current active lexical scope.
        /// </summary>
        /// <param name="namespace">
        /// The namespace to be used in evaluating locals and watches for the current active lexical scope.</param>
        /// <exception cref="ArgumentNullException">
        ///
        /// If <paramref name="namespace"/> is <see langword="null"/> or has a Length of 0.</exception>
        /// <exception cref="NotSupportedException">
        /// If this <typeparamref name="TEmitter"/> belongs to a <see cref="DynamicMethod"/>.
        /// </exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.usingnamespace"/>
        TEmitter UsingNamespace(string @namespace);

        #endregion
    }

    public interface IFluentExtendedILEmitter<TEmitter> : IFluentILEmitter<TEmitter>
        where TEmitter : IFluentExtendedILEmitter<TEmitter>
    {
        TEmitter Scoped(Action<TEmitter> scopedEmission)
        {
            scopedEmission(BeginScope());
            return EndScope();
        }
    }
}