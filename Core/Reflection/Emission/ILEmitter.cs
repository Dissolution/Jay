using Jay.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Reflection.Emission
{
    
    
    /// <summary>
    /// A fluent emitter of IL <see cref="OpCode"/>s and operations.
    /// </summary>
    /// <remarks>
    /// This is a fluent wrapper around an <see cref="ILGenerator"/> or mimics an underlying generator so operations can be batched and applied in different sequences.
    /// </remarks>
    public partial class ILEmitter
    {
        private readonly ILGenerator _generator;
        private readonly List<Instruction> _operations;

        private readonly List<Label> _labels;
        private readonly List<LocalBuilder> _locals;
        
        /// <summary>
        /// Gets the current offset, in bytes, of the stream that is being emitted.
        /// </summary>
        public int ILOffset => _generator.ILOffset;

        /// <summary>
        /// Gets the number of operations that have been emitted.
        /// </summary>
        public int Count => _operations.Count;
        
        /// <summary>
        /// Constructs a new, empty <see cref="ILEmitter"/>.
        /// </summary>
        public ILEmitter(ILGenerator generator)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _operations = new List<Instruction>(8);
            _labels = new List<Label>(0);
            _locals = new List<LocalBuilder>(0);
        }

        /// <summary>
        /// Validates a <see cref="label"/> and returns information about it.
        /// </summary>
        /// <param name="label">The <see cref="label"/> to validate.</param>
        /// <param name="isShort">Whether or not the label qualifies for short-form operations.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="label"/>'s index is invalid</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateLabel(Label label, out bool isShort)
        {
            var index = label.GetHashCode();
            if (index < 0 || index >= _labels.Count)
                throw new ArgumentOutOfRangeException(nameof(label), index, $"Label index must be between 0 and {_labels.Count - 1}");
            isShort = index <= sbyte.MaxValue;
        }
        
        /// <summary>
        /// Validates a <see cref="Label"/> and ensures that it qualifies for short-form operations.
        /// </summary>
        /// <param name="label">The <see cref="label"/> to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="label"/>'s index is invalid</exception>
        /// <exception cref="ArgumentException">If <paramref name="label"/> does not qualify for short-form operations.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateShortFormLabel(Label label)
        {
            var index = label.GetHashCode();
            if (index < 0 || index >= _labels.Count)
                throw new ArgumentOutOfRangeException(nameof(label), index, $"Label index must be between 0 and {_labels.Count - 1}");
            if (index > sbyte.MaxValue)
                throw new ArgumentException("The label does not quality for short-form operations.", nameof(label));
        }

        /// <summary>
        /// Validates a <see cref="LocalBuilder"/> and returns whether it qualifies for short-form operations.
        /// </summary>
        /// <param name="local">The <see cref="LocalBuilder"/> to validate.</param>
        /// <param name="isShort">Whether or not the local qualifies for short-form operations.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="local"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="local"/>'s index is invalid</exception>
        private void ValidateLocal(LocalBuilder local, out bool isShort)
        {
            if (local is null)
                throw new ArgumentNullException(nameof(local));
            var index = local.LocalIndex;
            if (index < 0 || index > _locals.Count || index > 65534)
                throw new ArgumentOutOfRangeException(nameof(local), index, $"LocalBuilder index must be between 0 and {System.Math.Min(_locals.Count, 65534)}");
            isShort = index <= byte.MaxValue;
        }

        /// <summary>
        /// Validates a <see cref="LocalBuilder"/> and ensures that it qualifies for short-form operations.
        /// </summary>
        /// <param name="local">The <see cref="LocalBuilder"/> to validate.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="local"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="local"/>'s index is invalid</exception>
        /// <exception cref="ArgumentException">If <paramref name="local"/> does not qualify for short-form operations.</exception>
        private void ValidateShortFormLocal(LocalBuilder local)
        {
            if (local is null)
                throw new ArgumentNullException(nameof(local));
            var index = local.LocalIndex;
            if (index < 0 || index > _locals.Count || index > 65534)
                throw new ArgumentOutOfRangeException(nameof(local), index, $"LocalBuilder index must be between 0 and {System.Math.Min(_locals.Count, 65534)}");
            if (index > byte.MaxValue)
                throw new ArgumentException("The local does not quality for short-form operations.", nameof(local));
        }

        #region Try / Catch / Finally
        /// <summary>
        /// Begins a <see langword="catch"/> block.
        /// </summary>
        /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="exceptionType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> is not an <see cref="Exception"/> type.</exception>
        /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock"/>
        public ILEmitter BeginCatchBlock(Type exceptionType)
        {
            if (exceptionType is null)
                throw new ArgumentNullException(nameof(exceptionType));
            if (!typeof(Exception).IsAssignableFrom(exceptionType))
                throw new ArgumentException("The given type must be an Exception type", nameof(exceptionType));
            _generator.BeginCatchBlock(exceptionType);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.BeginCatchBlock, exceptionType));
            return this;
        }

        /// <summary>
        /// Begins a <see langword="catch"/> block.
        /// </summary>
        /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</typeparam>
        /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock"/>
        public ILEmitter BeginCatchBlock<TException>()
            where TException : Exception
            => BeginCatchBlock(typeof(TException));

        /// <summary>
        /// Begins an exception block for a filtered exception.
        /// </summary>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
        /// <exception cref="NotSupportedException">This <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptfilterblock"/>
        public ILEmitter BeginExceptFilterBlock()
        {
            _generator.BeginExceptFilterBlock();
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.BeginExceptFilterBlock));
            return this;
        }

        /// <summary>
        /// Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfilter"/>
        public ILEmitter Endfilter() => Emit(OpCodes.Endfilter);

        /// <summary>
        /// Begins an exception block for a non-filtered exception.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> for the end of the block. This will leave you in the correct place to execute <see langword="finally"/> blocks or to finish the <see langword="try"/>.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptionblock"/>
        public ILEmitter BeginExceptionBlock(out Label label)
        {
            label = _generator.BeginExceptionBlock();
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.BeginExceptionBlock, label));
            return this;
        }

        /// <summary>
        /// Ends an exception block.
        /// </summary>
        /// <exception cref="InvalidOperationException">If this operation occurs in an unexpected place in the stream.</exception>
        /// <exception cref="NotSupportedException">If the stream being emitted is not currently in an exception block.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endexceptionblock"/>
        public ILEmitter EndExceptionBlock()
        {
            _generator.EndExceptionBlock();
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.EndExceptionBlock));
            return this;
        }

        /// <summary>
        /// Begins an exception fault block in the stream.
        /// </summary>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
        /// <exception cref="NotSupportedException">This <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfaultblock"/>
        public ILEmitter BeginFaultBlock()
        {
            _generator.BeginFaultBlock();
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.BeginFaultBlock));
            return this;
        }

        /// <summary>
        /// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
        /// </summary>
        /// <remarks>Note that the Endfault and Endfinally instructions are aliases - they correspond to the same opcode.</remarks>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfinally"/>
        public ILEmitter Endfault() => Emit(OpCodes.Endfinally);

        /// <summary>
        /// Begins a <see langword="finally"/> block in the stream.
        /// </summary>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfinallyblock"/>
        public ILEmitter BeginFinallyBlock()
        {
            _generator.BeginFinallyBlock();
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.BeginFinallyBlock));
            return this;
        }

        /// <summary>
        /// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfinally"/>
        public ILEmitter Endfinally() => Emit(OpCodes.Endfinally);
        #endregion

        #region Scope

        /// <summary>
        /// Begins a lexical scope.
        /// </summary>
        /// <exception cref="NotSupportedException">This <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginscope"/>
        public ILEmitter BeginScope()
        {
            _generator.BeginScope();
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.BeginScope));
            return this;
        }

        /// <summary>
        /// Ends a lexical scope.
        /// </summary>
        /// <exception cref="NotSupportedException">If this <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endscope"/>
        public ILEmitter EndScope()
        {
            _generator.EndScope();
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.EndScope));
            return this;
        }

        /// <summary>
        /// Specifies the <see langword="namespace"/> to be used in evaluating locals and watches for the current active lexical scope.
        /// </summary>
        /// <param name="namespace">The namespace to be used in evaluating locals and watches for the current active lexical scope.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="namespace"/> is <see langword="null"/> or has a Length of 0.</exception>
        /// <exception cref="NotSupportedException">If this <see cref="ILEmitter"/> belongs to a <see cref="DynamicMethod"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.usingnamespace"/>
        public ILEmitter UsingNamespace(string @namespace)
        {
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentNullException(nameof(@namespace));
            var exists = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Select(t => t.Namespace)
                .Any(n => n == @namespace);
            //TODO: Search for higher-level namespaces (system.text.xyz should match system, system.text, and system.text.xyz)
            if (!exists)
                throw new ArgumentException($"The specified namespace '{@namespace}' is invalid", nameof(@namespace));
            _generator.UsingNamespace(@namespace);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.UsingNamespace, @namespace));
            return this;
        }
        #endregion

        #region Locals
        #region Declare

        /// <summary>
        /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="localType">The type of the <see cref="LocalBuilder"/>.</param>
        /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="localType"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="localType"/> was created with <see cref="TypeBuilder.CreateType"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
        public ILEmitter DeclareLocal(Type localType, out LocalBuilder local)
        {
            if (localType is null)
                throw new ArgumentNullException(nameof(localType));
            local = _generator.DeclareLocal(localType);
            Debug.Assert(local.LocalIndex == _locals.Count);
            _locals.Add(local);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.DeclareLocal, local));
            return this;
        }

        /// <summary>
        /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="LocalBuilder"/>.</typeparam>
        /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_"/>
        public ILEmitter DeclareLocal<T>(out LocalBuilder local)
            => DeclareLocal(typeof(T), out local);

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
        public ILEmitter DeclareLocal(Type localType, bool pinned, out LocalBuilder local)
        {
            if (localType is null)
                throw new ArgumentNullException(nameof(localType));
            local = _generator.DeclareLocal(localType, pinned);
            Debug.Assert(local.LocalIndex == _locals.Count);
            _locals.Add(local);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.DeclareLocal, local));
            return this;
        }

        /// <summary>
        /// Declares a <see cref="LocalBuilder"/> variable of the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="LocalBuilder"/>.</typeparam>
        /// <param name="pinned">Whether or not the <see cref="LocalBuilder"/> should be pinned in memory.</param>
        /// <param name="local">Returns the declared <see cref="LocalBuilder"/>.</param>
        /// <exception cref="InvalidOperationException">If the method body of the enclosing method was created with <see cref="M:MethodBuilder.CreateMethodBody"/>.</exception>
        /// <exception cref="NotSupportedException">If the method this <see cref="ILEmitter"/> is associated with is not wrapping a <see cref="MethodBuilder"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal#System_Reflection_Emit_ILGenerator_DeclareLocal_System_Type_System_Boolean_"/>
        public ILEmitter DeclareLocal<T>(bool pinned, out LocalBuilder local)
            => DeclareLocal(typeof(T), pinned, out local);
        #endregion

        #region Load
        /// <summary>
        /// Loads the given <see cref="LocalBuilder"/>'s value onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
        public ILEmitter Ldloc(LocalBuilder local)
        {
            ValidateLocal(local, out var isShort);
            switch (local.LocalIndex)
            {
                case 0:
                    return Emit(OpCodes.Ldloc_0);
                case 1:
                    return Emit(OpCodes.Ldloc_1);
                case 2:
                    return Emit(OpCodes.Ldloc_2);
                case 3:
                    return Emit(OpCodes.Ldloc_3);
                default:
                {
                    if (isShort)
                        return Emit(OpCodes.Ldloc_S, local);
                    return Emit(OpCodes.Ldloc, local);
                }
            }
        }

        /// <summary>
        /// Loads the given short-form <see cref="LocalBuilder"/>'s value onto the stack.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="local"/> is not short-form.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
        [Obsolete("Use " + nameof(Ldloc) + " instead")]
        public ILEmitter Ldloc_S(LocalBuilder local)
        {
            ValidateShortFormLocal(local);
            switch (local.LocalIndex)
            {
                case 0:
                    return Emit(OpCodes.Ldloc_0);
                case 1:
                    return Emit(OpCodes.Ldloc_1);
                case 2:
                    return Emit(OpCodes.Ldloc_2);
                case 3:
                    return Emit(OpCodes.Ldloc_3);
                default:
                    return Emit(OpCodes.Ldloc_S, local);
            }
        }

        /// <summary>
        /// Loads the value of the <see cref="LocalBuilder"/> variable at index 0 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_0"/>
        [Obsolete("Use " + nameof(Ldloc) + "(0) instead")]
        public ILEmitter Ldloc_0() => Emit(OpCodes.Ldloc_0);

        /// <summary>
        /// Loads the value of the <see cref="LocalBuilder"/> variable at index 1 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_1"/>
        [Obsolete("Use " + nameof(Ldloc) + "(1) instead")]
        public ILEmitter Ldloc_1() => Emit(OpCodes.Ldloc_1);

        /// <summary>
        /// Loads the value of the <see cref="LocalBuilder"/> variable at index 2 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_2"/>
        [Obsolete("Use " + nameof(Ldloc) + "(2) instead")]
        public ILEmitter Ldloc_2() => Emit(OpCodes.Ldloc_2);

        /// <summary>
        /// Loads the value of the <see cref="LocalBuilder"/> variable at index 3 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_3"/>
        [Obsolete("Use " + nameof(Ldloc) + "(3) instead")]
        public ILEmitter Ldloc_3() => Emit(OpCodes.Ldloc_3);

        /// <summary>
        /// Loads the address of the given <see cref="LocalBuilder"/> variable.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca_s"/>
        public ILEmitter Ldloca(LocalBuilder local)
        {
            ValidateLocal(local, out var isShort);
            if (isShort)
                return Emit(OpCodes.Ldloca_S, local);
            return Emit(OpCodes.Ldloca, local);
        }

        /// <summary>
        /// Loads the address of the given short-form <see cref="LocalBuilder"/> variable.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="local"/> is not short-form.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca_s"/>
        [Obsolete("Use " + nameof(Ldloca) + " instead")]
        public ILEmitter Ldloca_S(LocalBuilder local)
        {
            ValidateShortFormLocal(local);
            return Emit(OpCodes.Ldloca_S, local);
        }
        #endregion

        #region Store
        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the given <see cref="LocalBuilder"/>.
        /// </summary>
        /// <param name="local">The <see cref="LocalBuilder"/> to store the value in.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
        public ILEmitter Stloc(LocalBuilder local)
        {
            ValidateLocal(local, out var isShort);
            switch (local.LocalIndex)
            {
                case 0:
                    return Emit(OpCodes.Stloc_0);
                case 1:
                    return Emit(OpCodes.Stloc_1);
                case 2:
                    return Emit(OpCodes.Stloc_2);
                case 3:
                    return Emit(OpCodes.Stloc_3);
                default:
                {
                    if (isShort)
                        return Emit(OpCodes.Stloc_S, local);
                    return Emit(OpCodes.Stloc, local);
                }
            }
        }

        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the given short-form <see cref="LocalBuilder"/>.
        /// </summary>
        /// <param name="local">The short-form <see cref="LocalBuilder"/> to store the value in.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
        [Obsolete("Use " + nameof(Stloc) + " instead")]
        public ILEmitter Stloc_S(LocalBuilder local)
        {
            ValidateShortFormLocal(local);
            switch (local.LocalIndex)
            {
                case 0:
                    return Emit(OpCodes.Stloc_0);
                case 1:
                    return Emit(OpCodes.Stloc_1);
                case 2:
                    return Emit(OpCodes.Stloc_2);
                case 3:
                    return Emit(OpCodes.Stloc_3);
                default:
                    return Emit(OpCodes.Stloc_S, local);
            }
        }

        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the <see cref="LocalBuilder"/> at index 0.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_0"/>
        [Obsolete("Use " + nameof(Stloc) + "(0) instead")]
        public ILEmitter Stloc_0() => Emit(OpCodes.Stloc_0);

        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the <see cref="LocalBuilder"/> at index 1.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_1"/>
        [Obsolete("Use " + nameof(Stloc) + "(1) instead")]
        public ILEmitter Stloc_1() => Emit(OpCodes.Stloc_1);

        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the <see cref="LocalBuilder"/> at index 2.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_2"/>
        [Obsolete("Use " + nameof(Stloc) + "(2) instead")]
        public ILEmitter Stloc_2() => Emit(OpCodes.Stloc_2);

        /// <summary>
        /// Pops the value from the top of the stack and stores it in a the <see cref="LocalBuilder"/> at index 3.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_3"/>
        [Obsolete("Use " + nameof(Stloc) + "(3) instead")]
        public ILEmitter Stloc_3() => Emit(OpCodes.Stloc_3);
        #endregion
        #endregion

        #region Labels

        /// <summary>
        /// Declares a new <see cref="Label"/>.
        /// </summary>
        /// <param name="label">Returns the new <see cref="Label"/> that can be used for branching.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.definelabel"/>
        public ILEmitter DefineLabel(out Label label)
        {
            label = _generator.DefineLabel();
            Debug.Assert(label.GetHashCode() == _labels.Count);
            _labels.Add(label);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.DefineLabel, label));
            return this;
        }

        /// <summary>
        /// Marks the stream's current position with the given <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> for which to set an index.</param>
        /// <exception cref="ArgumentException">If the <paramref name="label"/> has an invalid index.</exception>
        /// <exception cref="ArgumentException">If the <paramref name="label"/> has already been marked.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.marklabel"/>
        public ILEmitter MarkLabel(Label label)
        {
            _generator.MarkLabel(label);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.MarkLabel, label));
            return this;
        }

        /// <summary>
        /// Declares a new <see cref="Label"/> and marks the stream's current position with it.
        /// </summary>
        /// <param name="label">Returns the new <see cref="Label"/> marked with the stream's current position.</param>
        public ILEmitter DefineAndMarkLabel(out Label label) => DefineLabel(out label).MarkLabel(label);

        /// <summary>
        /// Implements a jump table.
        /// </summary>
        /// <param name="labels">The labels for the jumptable.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="labels"/> is <see langword="null"/> or empty.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.switch"/>
        public ILEmitter Switch(params Label[] labels)
        {
            if (labels is null || labels.Length == 0)
                throw new ArgumentNullException(nameof(labels));
            for (var i = 0; i < labels.Length; i++)
            {
                ValidateLabel(labels[i], out _);
            }
            return Emit(OpCodes.Switch, labels);
        }
        #endregion

        #region Emit
        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_"/>
        public ILEmitter Emit(OpCode opCode)
        {
            _generator.Emit(opCode);
            _operations.Add(new Instruction(ILOffset, opCode, null));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="byte"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Byte_"/>
        public ILEmitter Emit(OpCode opCode, byte value)
        {
            _generator.Emit(opCode, value);
            _operations.Add(new Instruction(ILOffset, opCode, value));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="sbyte"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_SByte_"/>
        public ILEmitter Emit(OpCode opCode, sbyte value)
        {
            _generator.Emit(opCode, value);
            _operations.Add(new Instruction(ILOffset, opCode, value));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="short"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int16_"/>
        public ILEmitter Emit(OpCode opCode, short value)
        {
            _generator.Emit(opCode, value);
            _operations.Add(new Instruction(ILOffset, opCode, value));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="int"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int32_"/>
        public ILEmitter Emit(OpCode opCode, int value)
        {
            _generator.Emit(opCode, value);
            _operations.Add(new Instruction(ILOffset, opCode, value));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="long"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Int64_"/>
        public ILEmitter Emit(OpCode opCode, long value)
        {
            _generator.Emit(opCode, value);
            _operations.Add(new Instruction(ILOffset, opCode, value));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="float"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Single_"/>
        public ILEmitter Emit(OpCode opCode, float value)
        {
            _generator.Emit(opCode, value);
            _operations.Add(new Instruction(ILOffset, opCode, value));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="double"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="value">The numeric value to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Double_"/>
        public ILEmitter Emit(OpCode opCode, double value)
        {
            _generator.Emit(opCode, value);
            _operations.Add(new Instruction(ILOffset, opCode, value));
            return this;
        }
        
        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="string"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="str">The <see cref="string"/>to emit.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_String_"/>
        public ILEmitter Emit(OpCode opCode, string str)
        {
            if (str is null)
                str = string.Empty;
            _generator.Emit(opCode, str);
            _operations.Add(new Instruction(ILOffset, opCode, str));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="FieldInfo"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="field">The <see cref="FieldInfo"/> to emit.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_FieldInfo_"/>
        public ILEmitter Emit(OpCode opCode, FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            _generator.Emit(opCode, field);
            _operations.Add(new Instruction(ILOffset, opCode, field));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="method">The <see cref="MethodInfo"/> to emit.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is a generic method for which <see cref="MethodBase.IsGenericMethodDefinition"/> is <see langword="false"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_MethodInfo_"/>
        public ILEmitter Emit(OpCode opCode, MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            _generator.Emit(opCode, method);
            _operations.Add(new Instruction(ILOffset, opCode, method));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="ConstructorInfo"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="ctor">The <see cref="ConstructorInfo"/> to emit.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="ctor"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_ConstructorInfo_"/>
        public ILEmitter Emit(OpCode opCode, ConstructorInfo ctor)
        {
            if (ctor is null)
                throw new ArgumentNullException(nameof(ctor));
            _generator.Emit(opCode, ctor);
            _operations.Add(new Instruction(ILOffset, opCode, ctor));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the given <see cref="SignatureHelper"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="signature">A helper for constructing a signature token.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="signature"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_SignatureHelper_"/>
        public ILEmitter Emit(OpCode opCode, SignatureHelper signature)
        {
            if (signature is null)
                throw new ArgumentNullException(nameof(signature));
            _generator.Emit(opCode, signature);
            _operations.Add(new Instruction(ILOffset, opCode, signature));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the metadata token for the given <see cref="Type"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="type">The <see cref="Type"/> to emit.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Type_"/>
        public ILEmitter Emit(OpCode opCode, Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            _generator.Emit(opCode, type);
            _operations.Add(new Instruction(ILOffset, opCode, type));
            return this;
        }

        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream followed by the index of the given <see cref="LocalBuilder"/>.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="local">The <see cref="LocalBuilder"/> to emit the index of.</param>
        /// <exception cref="InvalidOperationException">If <paramref name="opCode"/> is a single-byte instruction and <paramref name="local"/> has an index greater than <see cref="byte.MaxValue"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_LocalBuilder_"/>
        public ILEmitter Emit(OpCode opCode, LocalBuilder local)
        {
            if (local is null)
                throw new ArgumentNullException(nameof(local));
            _generator.Emit(opCode, local);
            _operations.Add(new Instruction(ILOffset, opCode, local));
            return this;
        }
        
        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream and leaves space to include a <see cref="Label"/> when fixes are done.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="label">The <see cref="Label"/> to branch from this location.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_Label_"/>
        public ILEmitter Emit(OpCode opCode, Label label)
        {
            _generator.Emit(opCode, label);
            _operations.Add(new Instruction(ILOffset, opCode, label));
            return this;
        }
        
        /// <summary>
        /// Emits an <see cref="OpCode"/> onto the stream and leaves space to include a <see cref="Label"/> when fixes are done.
        /// </summary>
        /// <param name="opCode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="labels">The <see cref="Label"/>s of which to branch to from this locations.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="labels"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="labels"/> is empty.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#System_Reflection_Emit_ILGenerator_Emit_System_Reflection_Emit_OpCode_System_Reflection_Emit_Label___"/>
        public ILEmitter Emit(OpCode opCode, params Label[] labels)
        {
            if (labels is null || labels.Length == 0)
                throw new ArgumentNullException(nameof(labels));
            _generator.Emit(opCode, labels);
            _operations.Add(new Instruction(ILOffset, opCode, labels));
            return this;
        }
        #endregion

        #region Method-Related (Call)

        /// <summary>
        /// Puts a <see cref="OpCodes.Call"/>, <see cref="OpCodes.Callvirt"/>, or <see cref="OpCodes.Newobj"/> instruction onto the stream to call a <see langword="varargs"/> <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The <see langword="varargs"/> <see cref="MethodInfo"/> to be called.</param>
        /// <param name="optionParameterTypes">The types of the Option arguments if the method is a <see langword="varargs"/> method; otherwise, <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcall"/>
        public ILEmitter Call(MethodInfo method, params Type[] optionParameterTypes)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            if (optionParameterTypes is null)
                optionParameterTypes = Type.EmptyTypes;
            var callCode = GetCallOpCode(method);
            _generator.EmitCall(callCode, method, optionParameterTypes);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.EmitCall, callCode, method, optionParameterTypes));
            return this;
        }

        /// <summary>
        /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call.
        /// </summary>
        /// <param name="convention">The unmanaged calling convention to be used.</param>
        /// <param name="returnType">The <see cref="Type"/> of the result.</param>
        /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="returnType"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli#System_Reflection_Emit_ILGenerator_EmitCalli_System_Reflection_Emit_OpCode_System_Runtime_InteropServices_CallingConvention_System_Type_System_Type___"/>
        public ILEmitter Calli(CallingConvention convention, Type returnType, params Type[] parameterTypes)
        {
            if (returnType is null)
                returnType = typeof(void);
            if (parameterTypes is null)
                parameterTypes = Type.EmptyTypes;
            _generator.EmitCalli(OpCodes.Calli, convention, returnType, parameterTypes);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.Calli, convention, returnType, parameterTypes));
            return this;
        }

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
        public ILEmitter Calli(CallingConventions conventions, Type returnType, Type[] parameterTypes, params Type[] optionParameterTypes)
        {
            if (returnType is null)
                returnType = typeof(void);
            if (parameterTypes is null)
                parameterTypes = Type.EmptyTypes;
            if (optionParameterTypes is null)
                optionParameterTypes = Type.EmptyTypes;
            _generator.EmitCalli(OpCodes.Calli, conventions, returnType, parameterTypes, optionParameterTypes);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.Calli, conventions, returnType, parameterTypes, optionParameterTypes));
            return this;
        }

        /// <summary>
        /// Calls the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> that will be called.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.call"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.callvirt"/>
        public ILEmitter Call(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return Emit(GetCallOpCode(method), method);

            /* Have to figure out exactly what methods can be safely called versus callvirted
             * ~~Also need to figure out how to let them be specific (override guessing) because either can be used for either in specific cases (see doc)~~
             * Solved by Emit(OpCodes.Call/Virt, method) directly
             *
             * Callvirt calls a late-bound method on an object, eg method chosen on runtime type of obj rather than compile-time class visible in method pointer. Can be used for virtual and instance methods
             * Call instruction calls the method indicated by the method descriptor passed with the instruction. The method descriptor is a metadata token that indicates the method to call and the number, type, and order of args on the stack and calling conventions
             *
             * TO  DO:
             * Note :
             * When calling methods of System.Object on value types, consider using the constrained prefix with the callvirt instruction instead of emitting a call instruction.
             * This removes the need to emit different IL depending on whether or not the value type overrides the method, avoiding a potential versioning problem.
             * Consider using the constrained prefix when invoking interface methods on value types, since the value type method implementing the interface method can be changed using a MethodImpl.
             * These issues are described in more detail in the Constrained opcode.
             */
        }

        /// <summary>
        /// Calls the given late-bound <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> that will be called.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.callvirt"/>
        [Obsolete("Use " + nameof(Call) + " Instead")]
        public ILEmitter Callvirt(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            return Emit(OpCodes.Callvirt, method);
        }

        /// <summary>
        /// Constrains the <see cref="Type"/> on which a virtual method call (<see cref="OpCodes.Callvirt"/>) is made.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to constrain the <see cref="OpCodes.Callvirt"/> upon.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained"/>
        public ILEmitter Constrained(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            return Emit(OpCodes.Constrained, type);
        }

        /// <summary>
        /// Constrains the <see cref="Type"/> on which a virtual method call (<see cref="OpCodes.Callvirt"/>) is made.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to constrain the <see cref="OpCodes.Callvirt"/> upon.</typeparam>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained"/>
        public ILEmitter Constrained<T>() => Emit(OpCodes.Constrained, typeof(T));

        /// <summary>
        /// Pushes an unmanaged pointer (<see cref="IntPtr"/>) to the native code implementing the given <see cref="MethodInfo"/> onto the stack.
        /// </summary>
        /// <param name="method">The method to get pointer to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldftn"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldvirtftn"/>
        public ILEmitter Ldftn(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            return Emit(OpCodes.Ldftn, method);
        }

        /// <summary>
        /// Pushes an unmanaged pointer (<see cref="IntPtr"/>) to the native code implementing the given virtual <see cref="MethodInfo"/> onto the stack.
        /// </summary>
        /// <param name="method">The method to get pointer to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldvirtftn"/>
        public ILEmitter Ldvirtftn(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            return Emit(OpCodes.Ldvirtftn, method);

        }

        /// <summary>
        /// Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual call instruction is executed.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.tailcall"/>
        public ILEmitter Tailcall() => Emit(OpCodes.Tailcall);
        #endregion

        #region Debugging
        /// <summary>
        /// Signals the Common Language Infrastructure (CLI) to inform the debugger that a breakpoint has been tripped.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.break"/>
        public ILEmitter Break() => Emit(OpCodes.Break);

        /// <summary>
        /// Fills space if opcodes are patched. No meaningful operation is performed, although a processing cycle can be consumed.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.nop"/>
        public ILEmitter Nop() => Emit(OpCodes.Nop);

        #region WriteLine

        /// <summary>
        /// Emits the instructions to call <see cref="Console.WriteLine(string)"/>.
        /// </summary>
        /// <param name="text">The <see cref="string"/> to write to the console.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitwriteline#System_Reflection_Emit_ILGenerator_EmitWriteLine_System_String_"/>
        public ILEmitter WriteLine(string text)
        {
            if (text is null)
                text = string.Empty;
            _generator.EmitWriteLine(text);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.WriteLine, text));
            return this;
        }

        /// <summary>
        /// Emits the instructions to call <see cref="Console.WriteLine(object)"/> with the value of the given <see cref="FieldInfo"/>.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose value is to be written to the console.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">If the <paramref name="field"/> contains a <see cref="TypeBuilder"/> or <see cref="EnumBuilder"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitwriteline#System_Reflection_Emit_ILGenerator_EmitWriteLine_System_Reflection_FieldInfo_"/>
        public ILEmitter WriteLine(FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            _generator.EmitWriteLine(field);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.WriteLine, field));
            return this;
        }

        /// <summary>
        /// Emits the instructions to call <see cref="Console.WriteLine(object)"/> with the value of the given <see cref="LocalBuilder"/>.
        /// </summary>
        /// <param name="local">The <see cref="LocalBuilder"/> whose value is to be written to the console.</param>
        /// <exception cref="ArgumentException">If the <paramref name="local"/> contains a <see cref="TypeBuilder"/> or <see cref="EnumBuilder"/>.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitwriteline#System_Reflection_Emit_ILGenerator_EmitWriteLine_System_Reflection_Emit_LocalBuilder_"/>
        public ILEmitter WriteLine(LocalBuilder local)
        {
            if (local is null)
                throw new ArgumentNullException(nameof(local));
            _generator.EmitWriteLine(local);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.WriteLine, local));
            return this;
        }
        #endregion

        #region Prefix
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix1"/>
        [Obsolete("This is a reserved instruction.", true)]
        public ILEmitter Prefix1() => Emit(OpCodes.Prefix1);

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix2"/>
        [Obsolete("This is a reserved instruction.", true)]
        public ILEmitter Prefix2() => Emit(OpCodes.Prefix2);

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix3"/>
        [Obsolete("This is a reserved instruction.", true)]
        public ILEmitter Prefix3() => Emit(OpCodes.Prefix1);
        
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix4"/>
        [Obsolete("This is a reserved instruction.", true)]
        public ILEmitter Prefix4() => Emit(OpCodes.Prefix4);
        
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix5"/>
        [Obsolete("This is a reserved instruction.", true)]
        public ILEmitter Prefix5() => Emit(OpCodes.Prefix5);
        
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix6"/>
        [Obsolete("This is a reserved instruction.", true)]
        public ILEmitter Prefix6() => Emit(OpCodes.Prefix6);
        
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix7"/>
        [Obsolete("This is a reserved instruction.", true)]
        public ILEmitter Prefix7() => Emit(OpCodes.Prefix7);
        
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefixref"/>
        [Obsolete("This is a reserved instruction.", true)]
        public ILEmitter Prefixref() => Emit(OpCodes.Prefixref);
        #endregion
        #endregion

        #region Exceptions

        /// <summary>
        /// Emits the instructions to throw an <see cref="Exception"/>.
        /// </summary>
        /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/> to throw.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="exceptionType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> is not an <see cref="Exception"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> does not have a default constructor.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.throwexception"/>
        public ILEmitter ThrowException(Type exceptionType)
        {
            if (exceptionType is null)
                throw new ArgumentNullException(nameof(exceptionType));
            if (!exceptionType.Implements<Exception>())
                throw new ArgumentException("The given type is not an Exception type", nameof(exceptionType));
            if (exceptionType.GetConstructor(Type.EmptyTypes) is null)
                throw new ArgumentException("The given Exception type does not have a default constructor", nameof(exceptionType));
            _generator.ThrowException(exceptionType);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.ThrowException, exceptionType));
            return this;
        }

        /// <summary>
        /// Emits the instructions to throw an <see cref="Exception"/>.
        /// </summary>
        /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/> to throw.</typeparam>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.throwexception"/>
        public ILEmitter ThrowException<TException>()
            where TException : Exception, new()
        {
            var exceptionType = typeof(TException);
            _generator.ThrowException(exceptionType);
            _operations.Add(new Instruction(ILOffset, ILGeneratorMethod.ThrowException, exceptionType));
            return this;
        }

        /// <summary>
        /// Emits the instructions to throw an <see cref="ArithmeticException"/> if the value on the stack is not a finite number.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ckfinite"/>
        public ILEmitter Ckfinite() => Emit(OpCodes.Ckfinite);

        /// <summary>
        /// Rethrows the current exception.
        /// </summary>
        /// <exception cref="NotSupportedException">The stream being emitted is not currently in an <see langword="catch"/> block.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rethrow"/>
        public ILEmitter Rethrow() => Emit(OpCodes.Rethrow);

        /// <summary>
        /// Throws the <see cref="Exception"/> currently on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Exception"/> <see cref="object"/> on the stack is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.throw"/>
        public ILEmitter Throw() => Emit(OpCodes.Throw);
        #endregion

        #region Math
        /// <summary>
        /// Adds two values and pushes the result onto the stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add"/>
        public ILEmitter Add() => Emit(OpCodes.Add);

        /// <summary>
        /// Adds two <see cref="int"/>s, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add_ovf"/>
        public ILEmitter Add_Ovf() => Emit(OpCodes.Add_Ovf);

        /// <summary>
        /// Adds two <see cref="uint"/>s, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add_ovf_un"/>
        public ILEmitter Add_Ovf_Un() => Emit(OpCodes.Add_Ovf_Un);

        /// <summary>
        /// Divides two values and pushes the result as a <see cref="float"/> or <see cref="int"/> quotient onto the evaluation stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.div"/>
        public ILEmitter Div() => Emit(OpCodes.Div);

        /// <summary>
        /// Divides two unsigned values and pushes the result as a <see cref="int"/> quotient onto the evaluation stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.div_un"/>
        public ILEmitter Div_Un() => Emit(OpCodes.Div_Un);

        /// <summary>
        /// Multiplies two values and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul"/>
        public ILEmitter Mul() => Emit(OpCodes.Mul);

        /// <summary>
        /// Multiplies two integer values, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul_ovf"/>
        public ILEmitter Mul_Ovf() => Emit(OpCodes.Mul_Ovf);

        /// <summary>
        /// Multiplies two unsigned integer values, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul_ovf_un"/>
        public ILEmitter Mul_Ovf_Un() => Emit(OpCodes.Mul_Ovf_Un);

        /// <summary>
        /// Divides two values and pushes the remainder onto the evaluation stack.
        /// </summary>
        /// <exception cref="DivideByZeroException">If the second value is zero.</exception>
        /// <exception cref="OverflowException">If computing the remainder between <see cref="int.MinValue"/> and <see langword="-1"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rem"/>
        public ILEmitter Rem() => Emit(OpCodes.Rem);

        /// <summary>
        /// Divides two unsigned values and pushes the remainder onto the evaluation stack.
        /// </summary>
        /// <exception cref="DivideByZeroException">If the second value is zero.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rem_un"/>
        public ILEmitter Rem_Un() => Emit(OpCodes.Rem_Un);

        /// <summary>
        /// Subtracts one value from another and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub"/>
        public ILEmitter Sub() => Emit(OpCodes.Sub);

        /// <summary>
        /// Subtracts one integer value from another, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub_ovf"/>
        public ILEmitter Sub_Ovf() => Emit(OpCodes.Sub_Ovf);

        /// <summary>
        /// Subtracts one unsigned integer value from another, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub_ovf_un"/>
        public ILEmitter Sub_Ovf_Un() => Emit(OpCodes.Sub_Ovf_Un);
        #endregion

        #region Bitwise
        /// <summary>
        /// Computes the bitwise AND (<see langword="&amp;"/>) of two values and pushes the result onto the stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.and"/>
        public ILEmitter And() => Emit(OpCodes.And);

        /// <summary>
        /// Negates a value (<see langword="-"/>) and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.neg"/>
        public ILEmitter Neg() => Emit(OpCodes.Neg);

        /// <summary>
        /// Computes the bitwise complement (<see langword="!"/>) of a value and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.not"/>
        public ILEmitter Not() => Emit(OpCodes.Not);

        /// <summary>
        /// Computes the bitwise OR (<see langword="|"/>) of two values and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.or"/>
        public ILEmitter Or() => Emit(OpCodes.Or);

        /// <summary>
        /// Shifts an integer value to the left (<see langword="&lt;&lt;"/>) by a specified number of bits, pushing the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shl"/>
        public ILEmitter Shl() => Emit(OpCodes.Shl);

        /// <summary>
        /// Shifts an integer value to the right (<see langword="&gt;&gt;"/>) by a specified number of bits, pushing the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shr"/>
        public ILEmitter Shr() => Emit(OpCodes.Shr);

        /// <summary>
        /// Shifts an unsigned integer value to the right (<see langword="&gt;&gt;"/>) by a specified number of bits, pushing the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shr_un"/>
        public ILEmitter Shr_Un() => Emit(OpCodes.Shr_Un);

        /// <summary>
        /// Computes the bitwise XOR (<see langword="^"/>) of a value and pushes the result onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.xor"/>
        public ILEmitter Xor() => Emit(OpCodes.Xor);
        #endregion

        #region Arguments
        /// <summary>
        /// Returns an unmanaged pointer to the argument list of the current method.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.arglist"/>
        public ILEmitter Arglist() => Emit(OpCodes.Arglist);

        #region Ldarg
        /// <summary>
        /// Loads the argument with the specified <paramref name="index"/> onto the stack.
        /// </summary>
        /// <param name="index">The index of the argument to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg"/>
        public ILEmitter Ldarg(int index)
        {
            if (index < 0 || index > short.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Argument index must be between 0 and {short.MaxValue}");
            if (index == 0)
                return Emit(OpCodes.Ldarg_0);
            if (index == 1)
                return Emit(OpCodes.Ldarg_1);
            if (index == 2)
                return Emit(OpCodes.Ldarg_2);
            if (index == 3)
                return Emit(OpCodes.Ldarg_3);
            if (index <= byte.MaxValue)
                return Emit(OpCodes.Ldarg_S, (byte) index);
            return Emit(OpCodes.Ldarg, (short) index);
        }

        /// <summary>
        /// Loads the argument with the specified short-form <paramref name="index"/> onto the stack.
        /// </summary>
        /// <param name="index">The short-form index of the argument to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_s"/>
        [Obsolete("Use " + nameof(Ldarg) + " Instead")]
        public ILEmitter Ldarg_S(int index)
        {
            if (index < 0 || index > byte.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Argument index must be between 0 and {byte.MaxValue}");
            if (index == 0)
                return Emit(OpCodes.Ldarg_0);
            if (index == 1)
                return Emit(OpCodes.Ldarg_1);
            if (index == 2)
                return Emit(OpCodes.Ldarg_2);
            if (index == 3)
                return Emit(OpCodes.Ldarg_3);
            return Emit(OpCodes.Ldarg_S, (byte)index);
        }

        /// <summary>
        /// Loads the argument at index 0 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_0"/>
        [Obsolete("Use " + nameof(Ldarg) + "(0) instead")]
        public ILEmitter Ldarg_0() => Emit(OpCodes.Ldarg_0);

        /// <summary>
        /// Loads the argument at index 1 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_1"/>
        [Obsolete("Use " + nameof(Ldarg) + "(1) instead")]
        public ILEmitter Ldarg_1() => Emit(OpCodes.Ldarg_1);

        /// <summary>
        /// Loads the argument at index 2 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_2"/>
        [Obsolete("Use " + nameof(Ldarg) + "(2) instead")]
        public ILEmitter Ldarg_2() => Emit(OpCodes.Ldarg_2);

        /// <summary>
        /// Loads the argument at index 3 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_3"/>
        [Obsolete("Use " + nameof(Ldarg) + "(3) instead")]
        public ILEmitter Ldarg_3() => Emit(OpCodes.Ldarg_3);

        /// <summary>
        /// Loads the address of the argument with the specified <paramref name="index"/> onto the stack.
        /// </summary>
        /// <param name="index">The index of the argument address to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarga"/>
        public ILEmitter Ldarga(int index)
        {
            if (index < 0 || index > short.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Argument index must be between 0 and {short.MaxValue}");
            if (index <= byte.MaxValue)
                return Emit(OpCodes.Ldarga_S, (byte)index);
            return Emit(OpCodes.Ldarga, (short)index);
        }

        /// <summary>
        /// Loads the address of the argument with the specified short-form <paramref name="index"/> onto the stack.
        /// </summary>
        /// <param name="index">The short-form index of the argument address to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarga_s"/>
        [Obsolete("Use " + nameof(Ldarga) + " Instead")]
        public ILEmitter Ldarga_S(int index)
        {
            if (index < 0 || index > byte.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Argument index must be between 0 and {byte.MaxValue}");
            return Emit(OpCodes.Ldarg_S, (byte)index);
        }
        #endregion
        #region Starg
        /// <summary>
        /// Stores the value on top of the stack in the argument at the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the argument.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.starg"/>
        public ILEmitter Starg(int index)
        {
            if (index < 0 || index > short.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Argument index must be between 0 and {short.MaxValue}");
            if (index <= byte.MaxValue)
                return Emit(OpCodes.Starg_S, (byte)index);
            return Emit(OpCodes.Starg, (short)index);
        }

        /// <summary>
        /// Stores the value on top of the stack in the argument at the given short-form <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The short-form index of the argument.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.starg_s"/>
        [Obsolete("Use " + nameof(Starg) + " instead")]
        public ILEmitter Starg_S(int index)
        {
            if (index < 0 || index > byte.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Short-form argument index must be between 0 and {byte.MaxValue}");
            return Emit(OpCodes.Starg_S, (byte) index);
        }
        #endregion
        #endregion

        #region Branching
        #region Unconditional
        /// <summary>
        /// Unconditionally transfers control to the given <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br_s"/>
        public ILEmitter Br(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Br_S, label);
            return Emit(OpCodes.Br, label);
        }

        /// <summary>
        /// Defines a <see cref="Label"/> and then unconditionally transfers control to it.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to define and then trasnfer to.</param>
        public ILEmitter Br(out Label label) => DefineLabel(out label).Br(label);

        /// <summary>
        /// Unconditionally transfers control to the given short-form <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br_s"/>
        [Obsolete("Use " + nameof(Br) + " instead")]
        public ILEmitter Br_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Beq_S, label);
        }

        /// <summary>
        /// Exits the current method and jumps to the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The metadata token for a <see cref="MethodInfo"/> to jump to.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="method"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.jmp"/>
        public ILEmitter Jmp(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            return Emit(OpCodes.Jmp, method);
        }

        /// <summary>
        /// Exits a internal region of code, unconditionally transferring control to the given <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave_s"/>
        public ILEmitter Leave(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Leave_S, label);
            return Emit(OpCodes.Leave, label);
        }

        /// <summary>
        /// Exits a internal region of code, unconditionally transferring control to the given short-form <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> is not short-form.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave_s"/>
        [Obsolete("Use " + nameof(Leave) + " instead")]
        public ILEmitter Leave_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Leave_S, label);
        }

        /// <summary>
        /// Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.
        /// </summary>
        public ILEmitter Ret() => Emit(OpCodes.Ret);
        #endregion
        #region True
        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if value is <see langword="true"/>, not-<see langword="null"/>, or non-zero.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue_s"/>
        public ILEmitter Brtrue(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Brtrue_S, label);
            return Emit(OpCodes.Brtrue, label);
        }

        public ILEmitter Brtrue(out Label label) => DefineLabel(out label).Brtrue(label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if value is <see langword="true"/>, not-<see langword="null"/>, or non-zero.
        /// </summary>
        /// <param name="label">The short-form<see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue_s"/>
        [Obsolete("Use " + nameof(Brtrue) + " instead")]
        public ILEmitter Brtrue_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Brtrue_S, label);
        }
        #endregion
        #region False
        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if value is <see langword="false"/>, <see langword="null"/>, or zero.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse_s"/>
        public ILEmitter Brfalse(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Brfalse_S, label);
            return Emit(OpCodes.Brfalse, label);
        }

        public ILEmitter Brfalse(out Label label)
        {
            return DefineLabel(out label).Brfalse(label);
        }

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if value is <see langword="false"/>, <see langword="null"/>, or zero.
        /// </summary>
        /// <param name="label">The short-form<see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse_s"/>
        [Obsolete("Use " + nameof(Brfalse) + " instead")]
        public ILEmitter Brfalse_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Brfalse_S, label);
        }
        #endregion
        #region ==
        public ILEmitter Beq(out Label label)
            => DefineLabel(out label)
                .Beq(label);
        
        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if two values are equal.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq_s"/>
        public ILEmitter Beq(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Beq_S, label);
            return Emit(OpCodes.Beq, label);
        }

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if two values are equal.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq_s"/>
        [Obsolete("Use " + nameof(Beq) + " instead")]
        public ILEmitter Beq_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Beq_S, label);
        }
        #endregion
        #region !=
        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if two unsigned or unordered values are not equal (<see langword="!="/>).
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un_s"/>
        public ILEmitter Bne_Un(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Bne_Un_S, label);
            return Emit(OpCodes.Bne_Un, label);
        }

        public ILEmitter Bne_Un(out Label label)
            => DefineLabel(out label)
                .Bne_Un(label);

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if two unsigned or unordered values are not equal (<see langword="!="/>).
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un_s"/>
        [Obsolete("Use " + nameof(Bne_Un) + " instead")]
        public ILEmitter Bne_Un_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Bne_Un_S, label);
        }
        #endregion
        #region >=
        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_s"/>
        public ILEmitter Bge(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Bge_S, label);
            return Emit(OpCodes.Bge, label);
        }

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_s"/>
        [Obsolete("Use " + nameof(Bge) + " instead")]
        public ILEmitter Bge_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Bge_S, label);
        }

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un_s"/>
        public ILEmitter Bge_Un(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Bge_Un_S, label);
            return Emit(OpCodes.Bge_Un, label);
        }

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un_s"/>
        [Obsolete("Use " + nameof(Bge_Un) + " instead")]
        public ILEmitter Bge_Un_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Bge_Un_S, label);
        }
        #endregion
        #region >
        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_s"/>
        public ILEmitter Bgt(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Bgt_S, label);
            return Emit(OpCodes.Bgt, label);
        }

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_s"/>
        [Obsolete("Use " + nameof(Bgt) + " instead")]
        public ILEmitter Bgt_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Bgt_S, label);
        }

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un_s"/>
        public ILEmitter Bgt_Un(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Bgt_Un_S, label);
            return Emit(OpCodes.Bgt_Un, label);
        }

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un_s"/>
        [Obsolete("Use " + nameof(Bgt_Un) + " instead")]
        public ILEmitter Bgt_Un_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Bgt_Un_S, label);
        }
        #endregion
        #region <=
        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_s"/>
        public ILEmitter Ble(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Ble_S, label);
            return Emit(OpCodes.Ble, label);
        }

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_s"/>
        [Obsolete("Use " + nameof(Ble) + " instead")]
        public ILEmitter Ble_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Ble_S, label);
        }

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un_s"/>
        public ILEmitter Ble_Un(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Ble_Un_S, label);
            return Emit(OpCodes.Ble_Un, label);
        }

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un_s"/>
        [Obsolete("Use " + nameof(Ble_Un) + " instead")]
        public ILEmitter Ble_Un_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Ble_Un_S, label);
        }
        #endregion
        #region <
        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_s"/>
        public ILEmitter Blt(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Blt_S, label);
            return Emit(OpCodes.Blt, label);
        }

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_s"/>
        [Obsolete("Use " + nameof(Blt) + " instead")]
        public ILEmitter Blt_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Blt_S, label);
        }

        /// <summary>
        /// Transfers control to the given <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to transfer to.</param>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un"/>
        /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un_s"/>
        public ILEmitter Blt_Un(Label label)
        {
            ValidateLabel(label, out var isShort);
            if (isShort)
                return Emit(OpCodes.Blt_Un_S, label);
            return Emit(OpCodes.Blt_Un, label);
        }

        /// <summary>
        /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value when comparing unsigned integer values or unordered float values.
        /// </summary>
        /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un_s"/>
        [Obsolete("Use " + nameof(Blt_Un) + " instead")]
        public ILEmitter Blt_Un_S(Label label)
        {
            ValidateShortFormLabel(label);
            return Emit(OpCodes.Blt_Un_S, label);
        }
        #endregion
        #endregion

        #region Boxing / Unboxing / Casting
        /// <summary>
        /// Converts a <see langword="struct"/> into an <see cref="object"/> reference.
        /// </summary>
        /// <param name="valueType">The <see cref="Type"/> of <see langword="struct"/> that is to be boxed.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="valueType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="valueType"/> is not a value type.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.box"/>
        public ILEmitter Box(Type valueType)
        {
            AssertIsValueType(valueType);
            return Emit(OpCodes.Box, valueType);
        }

        /// <summary>
        /// Converts a <see langword="struct"/> into an <see cref="object"/> reference.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see langword="struct"/> that is to be boxed.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.box"/>
        public ILEmitter Box<T>()
            where T : struct 
            => Emit(OpCodes.Box, typeof(T));

        public ILEmitter BoxIfNeeded(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsValueType)
                return Emit(OpCodes.Box, type);
            return this;
        }
        
        public ILEmitter BoxIfNeeded<T>()
        {
            var type = typeof(T);
            if (type.IsValueType)
                return Emit(OpCodes.Box, type);
            return this;
        }
        
        /// <summary>
        /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to a value-type pointer.
        /// </summary>
        /// <param name="valueType">The value type that is to be unboxed.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="valueType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="valueType"/> is not a value type.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox"/>
        public ILEmitter Unbox(Type valueType)
        {
            AssertIsValueType(valueType);
            return Emit(OpCodes.Unbox, valueType);
        }

        /// <summary>
        /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to a value-type pointer.
        /// </summary>
        /// <typeparam name="T">The value type that is to be unboxed.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox"/>
        public ILEmitter Unbox<T>()
            where T : struct
            => Unbox(typeof(T));

        /// <summary>
        /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to its unboxed value.
        /// </summary>
        /// <param name="valueType">The value type that is to be unboxed.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="valueType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="valueType"/> is not a value type.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox_any"/>
        public ILEmitter Unbox_Any(Type valueType)
        {
            AssertIsValueType(valueType);
            return Emit(OpCodes.Unbox_Any, valueType);
        }

        /// <summary>
        /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to its unboxed value.
        /// </summary>
        /// <typeparam name="T">The value type that is to be unboxed.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox_any"/>
        public ILEmitter Unbox_Any<T>()
            where T : struct
            => Unbox_Any(typeof(T));

        /// <summary>
        /// Casts an <see cref="object"/> into the given <see langword="class"/>.
        /// </summary>
        /// <param name="classType">The <see cref="Type"/> of <see langword="class"/> to cast to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="classType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="classType"/> is not a <see langword="class"/> type.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.castclass"/>
        public ILEmitter Castclass(Type classType)
        {
            AssertIsClassType(classType);
            return Emit(OpCodes.Castclass, classType);
        }

        /// <summary>
        /// Casts an <see cref="object"/> into the given <see langword="class"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see langword="class"/> to cast to.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.castclass"/>
        public ILEmitter Castclass<T>()
            where T : class
            => Emit(OpCodes.Castclass, typeof(T));

        public ILEmitter UnboxOrCastclass(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsValueType)
                return Unbox_Any(type);
            return Castclass(type);
        }
        
        /// <summary>
        /// Tests whether an <see cref="object"/> is an instance of a given <see langword="class"/> <see cref="Type"/>.
        /// </summary>
        /// <param name="classType">The <see cref="Type"/> of <see langword="class"/> to cast to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="classType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="classType"/> is not a <see langword="class"/> type.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.isinst"/>
        public ILEmitter Isinst(Type classType)
        {
            AssertIsClassType(classType);
            return Emit(OpCodes.Isinst, classType);
        }

        /// <summary>
        /// Tests whether an <see cref="object"/> is an instance of a given <see langword="class"/> <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see langword="class"/> to cast to.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.isinst"/>
        public ILEmitter Isinst<T>()
            where T : class
            => Emit(OpCodes.Isinst, typeof(T));
        #endregion

        #region Conv
        /// <summary>
        /// Operation flags for generic emission methods.
        /// </summary>
        [Flags]
        public enum ConvOptions
        {
            /// <summary>
            /// Throw an <see cref="OverflowException"/> on overflow.
            /// </summary>
            Ovf = 1,
            /// <summary>
            /// The value on the stack is unsigned.
            /// </summary>
            Un = 1 | 2,
        }

        /// <summary>
        /// Converts the value on the stack to the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to convert the value to.</typeparam>
        /// <param name="options">Option options for conversion.</param>
        public ILEmitter Conv<T>(ConvOptions options = default) => Conv(typeof(T), options);

        /// <summary>
        /// Converts the value on the stack to the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to convert the value to.</param>
        /// <param name="options">Option options for conversion.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="type"/> is not valid for Conv_ operations.</exception>
        public ILEmitter Conv(Type type, ConvOptions options = default)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type == typeof(IntPtr))
            {
                if (options.HasFlag(ConvOptions.Un))
                    return Emit(OpCodes.Conv_Ovf_I_Un);
                if (options.HasFlag(ConvOptions.Ovf))
                    return Emit(OpCodes.Conv_Ovf_I);
                return Emit(OpCodes.Conv_I);
            }
            if (type == typeof(sbyte))
            {
                if (options.HasFlag(ConvOptions.Un))
                    return Emit(OpCodes.Conv_Ovf_I1_Un);
                if (options.HasFlag(ConvOptions.Ovf))
                    return Emit(OpCodes.Conv_Ovf_I1);
                return Emit(OpCodes.Conv_I1);
            }
            if (type == typeof(short))
            {
                if (options.HasFlag(ConvOptions.Un))
                    return Emit(OpCodes.Conv_Ovf_I2_Un);
                if (options.HasFlag(ConvOptions.Ovf))
                    return Emit(OpCodes.Conv_Ovf_I2);
                return Emit(OpCodes.Conv_I2);
            }
            if (type == typeof(int))
            {
                if (options.HasFlag(ConvOptions.Un))
                    return Emit(OpCodes.Conv_Ovf_I4_Un);
                if (options.HasFlag(ConvOptions.Ovf))
                    return Emit(OpCodes.Conv_Ovf_I4);
                return Emit(OpCodes.Conv_I4);
            }
            if (type == typeof(long))
            {
                if (options.HasFlag(ConvOptions.Un))
                    return Emit(OpCodes.Conv_Ovf_I8_Un);
                if (options.HasFlag(ConvOptions.Ovf))
                    return Emit(OpCodes.Conv_Ovf_I8);
                return Emit(OpCodes.Conv_I8);
            }
            if (type == typeof(UIntPtr))
            {
                if (options.HasFlag(ConvOptions.Un))
                    return Emit(OpCodes.Conv_Ovf_U_Un);
                if (options.HasFlag(ConvOptions.Ovf))
                    return Emit(OpCodes.Conv_Ovf_U);
                return Emit(OpCodes.Conv_U);
            }
            if (type == typeof(byte))
            {
                if (options.HasFlag(ConvOptions.Un))
                    return Emit(OpCodes.Conv_Ovf_U1_Un);
                if (options.HasFlag(ConvOptions.Ovf))
                    return Emit(OpCodes.Conv_Ovf_U1);
                return Emit(OpCodes.Conv_U1);
            }
            if (type == typeof(ushort))
            {
                if (options.HasFlag(ConvOptions.Un))
                    return Emit(OpCodes.Conv_Ovf_U2_Un);
                if (options.HasFlag(ConvOptions.Ovf))
                    return Emit(OpCodes.Conv_Ovf_U2);
                return Emit(OpCodes.Conv_U2);
            }
            if (type == typeof(uint))
            {
                if (options.HasFlag(ConvOptions.Un))
                    return Emit(OpCodes.Conv_Ovf_U4_Un);
                if (options.HasFlag(ConvOptions.Ovf))
                    return Emit(OpCodes.Conv_Ovf_U4);
                return Emit(OpCodes.Conv_U4);
            }
            if (type == typeof(ulong))
            {
                if (options.HasFlag(ConvOptions.Un))
                    return Emit(OpCodes.Conv_Ovf_U8_Un);
                if (options.HasFlag(ConvOptions.Ovf))
                    return Emit(OpCodes.Conv_Ovf_U8);
                return Emit(OpCodes.Conv_U8);
            }
            if (type == typeof(float))
            {
                if (options.HasFlag(ConvOptions.Un))
                    return Emit(OpCodes.Conv_R_Un);
                return Emit(OpCodes.Conv_R4);
            }
            if (type == typeof(double))
                return Emit(OpCodes.Conv_R8);
            throw new ArgumentException($"The specified type '{type}' is not a valid type for Conv operations", nameof(type));
        }
        
        #region nativeint
        /// <summary>
        /// Converts the value on the stack to a <see cref="IntPtr"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i"/>
        public ILEmitter Conv_I() => Emit(OpCodes.Conv_I);

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i"/>
        public ILEmitter Conv_Ovf_I() => Emit(OpCodes.Conv_Ovf_I);

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i_un"/>
        public ILEmitter Conv_Ovf_I_Un() => Emit(OpCodes.Conv_Ovf_I_Un);
        #endregion
        #region sbyte
        /// <summary>
        /// Converts the value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i1"/>
        public ILEmitter Conv_I1() => Emit(OpCodes.Conv_I1);

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i1"/>
        public ILEmitter Conv_Ovf_I1() => Emit(OpCodes.Conv_Ovf_I1);

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i1_un"/>
        public ILEmitter Conv_Ovf_I1_Un() => Emit(OpCodes.Conv_Ovf_I1_Un);
        #endregion
        #region short
        /// <summary>
        /// Converts the value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i2"/>
        public ILEmitter Conv_I2() => Emit(OpCodes.Conv_I2);

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i2"/>
        public ILEmitter Conv_Ovf_I2() => Emit(OpCodes.Conv_Ovf_I2);

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i2_un"/>
        public ILEmitter Conv_Ovf_I2_Un() => Emit(OpCodes.Conv_Ovf_I2_Un);
        #endregion
        #region int
        /// <summary>
        /// Converts the value on the stack to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i4"/>
        public ILEmitter Conv_I4() => Emit(OpCodes.Conv_I4);

        /// <summary>
        /// Converts the signed value on the stack to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i4"/>
        public ILEmitter Conv_Ovf_I4() => Emit(OpCodes.Conv_Ovf_I4);

        /// <summary>
        /// Converts the unsigned value on the stack to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i4_un"/>
        public ILEmitter Conv_Ovf_I4_Un() => Emit(OpCodes.Conv_Ovf_I4_Un);
        #endregion
        #region long
        /// <summary>
        /// Converts the value on the stack to a <see cref="long"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i8"/>
        public ILEmitter Conv_I8() => Emit(OpCodes.Conv_I8);

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i8"/>
        public ILEmitter Conv_Ovf_I8() => Emit(OpCodes.Conv_Ovf_I8);

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i8_un"/>
        public ILEmitter Conv_Ovf_I8_Un() => Emit(OpCodes.Conv_Ovf_I8_Un);
        #endregion

        #region nativeuuint
        /// <summary>
        /// Converts the value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u"/>
        public ILEmitter Conv_U() => Emit(OpCodes.Conv_U);

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u"/>
        public ILEmitter Conv_Ovf_U() => Emit(OpCodes.Conv_Ovf_U);

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u_un"/>
        public ILEmitter Conv_Ovf_U_Un() => Emit(OpCodes.Conv_Ovf_U_Un);
        #endregion
        #region byte
        /// <summary>
        /// Converts the value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u1"/>
        public ILEmitter Conv_U1() => Emit(OpCodes.Conv_U1);

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u1"/>
        public ILEmitter Conv_Ovf_U1() => Emit(OpCodes.Conv_Ovf_U1);

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u1_un"/>
        public ILEmitter Conv_Ovf_U1_Un() => Emit(OpCodes.Conv_Ovf_U1_Un);
        #endregion
        #region uushort
        /// <summary>
        /// Converts the value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u2"/>
        public ILEmitter Conv_U2() => Emit(OpCodes.Conv_U2);

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u2"/>
        public ILEmitter Conv_Ovf_U2() => Emit(OpCodes.Conv_Ovf_U2);

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u2_un"/>
        public ILEmitter Conv_Ovf_U2_Un() => Emit(OpCodes.Conv_Ovf_U2_Un);
        #endregion
        #region uuint
        /// <summary>
        /// Converts the value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u4"/>
        public ILEmitter Conv_U4() => Emit(OpCodes.Conv_U4);

        /// <summary>
        /// Converts the signed value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u4"/>
        public ILEmitter Conv_Ovf_U4() => Emit(OpCodes.Conv_Ovf_U4);

        /// <summary>
        /// Converts the unsigned value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u4_un"/>
        public ILEmitter Conv_Ovf_U4_Un() => Emit(OpCodes.Conv_Ovf_U4_Un);
        #endregion
        #region uulong
        /// <summary>
        /// Converts the value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u8"/>
        public ILEmitter Conv_U8() => Emit(OpCodes.Conv_U8);

        /// <summary>
        /// Converts the signed value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u8"/>
        public ILEmitter Conv_Ovf_U8() => Emit(OpCodes.Conv_Ovf_U8);

        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u8_un"/>
        public ILEmitter Conv_Ovf_U8_Un() => Emit(OpCodes.Conv_Ovf_U8_Un);
        #endregion
        #region float / double
        /// <summary>
        /// Converts the unsigned value on the stack to a <see cref="float"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r_un"/>
        public ILEmitter Conv_R_Un() => Emit(OpCodes.Conv_R_Un);

        /// <summary>
        /// Converts the value on the stack to a <see cref="float"/>.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r4"/>
        public ILEmitter Conv_R4() => Emit(OpCodes.Conv_R4);

        /// <summary>
        /// Converts the value on the stack to a <see cref="double"/>.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r8"/>
        public ILEmitter Conv_R8() => Emit(OpCodes.Conv_R8);
        #endregion
        #endregion

        #region Comparison
        /// <summary>
        /// Compares two values. If they are equal (<see langword="=="/>), (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ceq"/>
        public ILEmitter Ceq() => Emit(OpCodes.Ceq);

        /// <summary>
        /// Compares two values. If the first value is greater than (<see langword="&gt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cgt"/>
        public ILEmitter Cgt() => Emit(OpCodes.Cgt);

        /// <summary>
        /// Compares two unsigned or unordered values. If the first value is greater than (<see langword="&gt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cgt_un"/>
        public ILEmitter Cgt_Un() => Emit(OpCodes.Cgt_Un);

        /// <summary>
        /// Compares two values. If the first value is less than (<see langword="&lt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.clt"/>
        public ILEmitter Clt() => Emit(OpCodes.Clt);

        /// <summary>
        /// Compares two unsigned or unordered values. If the first value is less than (<see langword="&lt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.clt_un"/>
        public ILEmitter Clt_Un() => Emit(OpCodes.Clt_Un);
        #endregion

        #region byte*  /  byte[]  /  ref byte
        /// <summary>
        /// Copies a number of bytes from a source address to a destination address.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpblk"/>
        public ILEmitter Cpblk() => Emit(OpCodes.Cpblk);

        /// <summary>
        /// Initializes a specified block of memory at a specific address to a given size and initial value.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initblk"/>
        public ILEmitter Initblk() => Emit(OpCodes.Initblk);

        /// <summary>
        /// Allocates a certain number of bytes from the local dynamic memory pool and pushes the address (<see langword="byte*"/>) of the first allocated byte onto the stack.
        /// </summary>
        /// <exception cref="StackOverflowException">If there is insufficient memory to service this request.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.localloc"/>
        public ILEmitter Localloc() => Emit(OpCodes.Localloc);
        #endregion

        #region Copy / Duplicate
        /// <summary>
        /// Copies the <see langword="struct"/> located at the <see cref="IntPtr"/> source address to the <see cref="IntPtr"/> destination address.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of <see langword="struct"/> that is to be copied.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpobj"/>
        public ILEmitter Cpobj(Type type)
        {
            AssertIsValueType(type);
            return Emit(OpCodes.Cpobj, type);
        }

        /// <summary>
        /// Copies the <see langword="struct"/> located at the <see cref="IntPtr"/> source address to the <see cref="IntPtr"/> destination address.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see langword="struct"/> that is to be copied.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpobj"/>
        public ILEmitter Cpobj<T>()
            where T : struct
            => Emit(OpCodes.Cpobj, typeof(T));

        /// <summary>
        /// Copies a value, and then pushes the copy onto the evaluation stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.dup"/>
        public ILEmitter Dup() => Emit(OpCodes.Dup);
        #endregion

        #region Value Transformation / Creation
        /// <summary>
        /// Initializes each field of the <see langword="struct"/> at a specified address to a <see langword="null"/> reference or 0 primitive.
        /// </summary>
        /// <param name="type">The <see langword="struct"/> to be initialized.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="type"/> is not a struct.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initobj"/>
        public ILEmitter Initobj(Type type)
        {
            AssertIsValueType(type);
            return Emit(OpCodes.Initobj, type);
        }

        /// <summary>
        /// Initializes each field of the <see langword="struct"/> at a specified address to a <see langword="null"/> reference or 0 primitive.
        /// </summary>
        /// <typeparam name="T">The <see langword="struct"/> to be initialized.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initobj"/>
        public ILEmitter Initobj<T>()
            where T : struct
            => Emit(OpCodes.Initobj, typeof(T));
        
        /// <summary>
        /// Creates a new <see cref="object"/> or <see langword="struct"/> and pushes it onto the stack.
        /// </summary>
        /// <param name="ctor">The <see cref="ConstructorInfo"/> to use to create the object.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="ctor"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">If there is insufficient memory to satisfy the request.</exception>
        /// <exception cref="MissingMethodException">If the <paramref name="ctor"/> could not be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newobj"/>
        public ILEmitter Newobj(ConstructorInfo ctor)
        {
            if (ctor is null)
                throw new ArgumentNullException(nameof(ctor));
            return Emit(OpCodes.Newobj, ctor);
        }

        /// <summary>
        /// Removes the value currently on top of the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.pop"/>
        public ILEmitter Pop() => Emit(OpCodes.Pop);

        public ILEmitter PopIfNotVoid(Type? type)
        {
            if (type is null || type == typeof(void))
                return this;
            return Pop();
        }
        #endregion

        #region Load Value

        #region LoaD Constant (LDC)

        /*public ILEmitter Ldc<T>(T value)
        {
            throw new NotImplementedException();
        }*/
        
        /// <summary>
        /// Pushes the given <see cref="int"/> onto the stack.
        /// </summary>
        /// <param name="value">The value to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4"/>
        public ILEmitter Ldc_I4(int value)
        {
            if (value == -1)
                return Emit(OpCodes.Ldc_I4_M1);
            if (value == 0)
                return Emit(OpCodes.Ldc_I4_0);
            if (value == 1)
                return Emit(OpCodes.Ldc_I4_1);
            if (value == 2)
                return Emit(OpCodes.Ldc_I4_2);
            if (value == 3)
                return Emit(OpCodes.Ldc_I4_3);
            if (value == 4)
                return Emit(OpCodes.Ldc_I4_4);
            if (value == 5)
                return Emit(OpCodes.Ldc_I4_5);
            if (value == 6)
                return Emit(OpCodes.Ldc_I4_6);
            if (value == 7)
                return Emit(OpCodes.Ldc_I4_7);
            if (value == 8)
                return Emit(OpCodes.Ldc_I4_8);
            if (value >= sbyte.MinValue || value <= sbyte.MaxValue)
                return Emit(OpCodes.Ldc_I4_S, (sbyte) value);
            return Emit(OpCodes.Ldc_I4, value);
        }

        /// <summary>
        /// Pushes the given <see cref="sbyte"/> onto the stack.
        /// </summary>
        /// <param name="value">The short-form value to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_s"/>
        [Obsolete("Use " + nameof(Ldc_I4) + "() instead")]
        public ILEmitter Ldc_I4_S(sbyte value)
        {
            if (value == -1)
                return Emit(OpCodes.Ldc_I4_M1);
            if (value == 0)
                return Emit(OpCodes.Ldc_I4_0);
            if (value == 1)
                return Emit(OpCodes.Ldc_I4_1);
            if (value == 2)
                return Emit(OpCodes.Ldc_I4_2);
            if (value == 3)
                return Emit(OpCodes.Ldc_I4_3);
            if (value == 4)
                return Emit(OpCodes.Ldc_I4_4);
            if (value == 5)
                return Emit(OpCodes.Ldc_I4_5);
            if (value == 6)
                return Emit(OpCodes.Ldc_I4_6);
            if (value == 7)
                return Emit(OpCodes.Ldc_I4_7);
            if (value == 8)
                return Emit(OpCodes.Ldc_I4_8);
            return Emit(OpCodes.Ldc_I4_S, value);
        }
        
        /// <summary>
        /// Pushes the given <see cref="int"/> value of -1 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_m1"/>
        [Obsolete("Use " + nameof(Ldc_I4) + "(-1) instead")]
        public ILEmitter Ldc_I4_M1() => Emit(OpCodes.Ldc_I4_M1);

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 0 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_0"/>
        [Obsolete("Use " + nameof(Ldc_I4) + "(0) instead")]
        public ILEmitter Ldc_I4_0() => Emit(OpCodes.Ldc_I4_0);

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 1 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_1"/>
        [Obsolete("Use " + nameof(Ldc_I4) + "(1) instead")]
        public ILEmitter Ldc_I4_1() => Emit(OpCodes.Ldc_I4_1);

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 2 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_2"/>
        [Obsolete("Use " + nameof(Ldc_I4) + "(2) instead")]
        public ILEmitter Ldc_I4_2() => Emit(OpCodes.Ldc_I4_2);

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 3 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_3"/>
        [Obsolete("Use " + nameof(Ldc_I4) + "(3) instead")]
        public ILEmitter Ldc_I4_3() => Emit(OpCodes.Ldc_I4_3);

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 4 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_4"/>
        [Obsolete("Use " + nameof(Ldc_I4) + "(4) instead")]
        public ILEmitter Ldc_I4_4() => Emit(OpCodes.Ldc_I4_4);

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 5 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_5"/>
        [Obsolete("Use " + nameof(Ldc_I4) + "(5) instead")]
        public ILEmitter Ldc_I4_5() => Emit(OpCodes.Ldc_I4_5);

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 6 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_6"/>
        [Obsolete("Use " + nameof(Ldc_I4) + "(6) instead")]
        public ILEmitter Ldc_I4_6() => Emit(OpCodes.Ldc_I4_6);

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 7 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_7"/>
        [Obsolete("Use " + nameof(Ldc_I4) + "(7) instead")]
        public ILEmitter Ldc_I4_7() => Emit(OpCodes.Ldc_I4_7);

        /// <summary>
        /// Pushes the given <see cref="int"/> value of 8 onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_8"/>
        [Obsolete("Use " + nameof(Ldc_I4) + "(8) instead")]
        public ILEmitter Ldc_I4_8() => Emit(OpCodes.Ldc_I4_8);

        /// <summary>
        /// Pushes the given <see cref="long"/> onto the stack.
        /// </summary>
        /// <param name="value">The value to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i8"/>
        public ILEmitter Ldc_I8(long value) => Emit(OpCodes.Ldc_I8, value);

        /// <summary>
        /// Pushes the given <see cref="float"/> onto the stack.
        /// </summary>
        /// <param name="value">The value to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_r4"/>
        public ILEmitter Ldc_R4(float value) => Emit(OpCodes.Ldc_R4, value);

        /// <summary>
        /// Pushes the given <see cref="double"/> onto the stack.
        /// </summary>
        /// <param name="value">The value to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_r8"/>
        public ILEmitter Ldc_R8(double value) => Emit(OpCodes.Ldc_R8, value);
        #endregion

        /// <summary>
        /// Pushes a <see langword="null"/> <see cref="object"/> onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldnull"/>
        public ILEmitter Ldnull() => Emit(OpCodes.Ldnull);

        /// <summary>
        /// Pushes a <see cref="string"/> onto the stack.
        /// </summary>
        /// <param name="text">The <see cref="string"/> to push onto the stack.</param>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldstr"/>
        public ILEmitter Ldstr(string text) => Emit(OpCodes.Ldstr, text ?? string.Empty);

        #region Ldtoken
        /// <summary>
        /// Converts a metadata token to its runtime representation and pushes it onto the stack.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to convert to a <see cref="RuntimeTypeHandle"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
        public ILEmitter Ldtoken(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            return Emit(OpCodes.Ldtoken, type);
        }

        /// <summary>
        /// Converts a metadata token to its runtime representation and pushes it onto the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> to convert to a <see cref="RuntimeFieldHandle"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
        public ILEmitter Ldtoken(FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            return Emit(OpCodes.Ldtoken, field);
        }

        /// <summary>
        /// Converts a metadata token to its runtime representation and pushes it onto the stack.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> to convert to a <see cref="RuntimeMethodHandle"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
        public ILEmitter Ldtoken(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            return Emit(OpCodes.Ldtoken, method);
        }
        #endregion
        #endregion

        #region Arrays
        /// <summary>
        /// Pushes the number of elements of a zero-based, one-dimensional <see cref="Array"/> onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldlen"/>
        public ILEmitter Ldlen() => Emit(OpCodes.Ldlen);

        #region Load Element
        /// <summary>
        /// Loads the element from an array index onto the stack as the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the element to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem"/>
        public ILEmitter Ldelem(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type == typeof(IntPtr))
                return Emit(OpCodes.Ldelem_I);
            if (type == typeof(sbyte))
                return Emit(OpCodes.Ldelem_I1);
            if (type == typeof(short))
                return Emit(OpCodes.Ldelem_I2);
            if (type == typeof(int))
                return Emit(OpCodes.Ldelem_I4);
            if (type == typeof(long))
                return Emit(OpCodes.Ldelem_I8);
            if (type == typeof(byte))
                return Emit(OpCodes.Ldelem_U1);
            if (type == typeof(ushort))
                return Emit(OpCodes.Ldelem_U2);
            if (type == typeof(uint))
                return Emit(OpCodes.Ldelem_U4);
            if (type == typeof(float))
                return Emit(OpCodes.Ldelem_R4);
            if (type == typeof(double))
                return Emit(OpCodes.Ldelem_R8);
            if (type == typeof(object))
                return Emit(OpCodes.Ldelem_Ref);
            return Emit(OpCodes.Ldelem, type);
        }

        /// <summary>
        /// Loads the element from an array index onto the stack as the given <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the element to load.</typeparam>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem"/>
        public ILEmitter Ldelem<T>() => Ldelem(typeof(T));

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="IntPtr"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="IntPtr"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i"/>
        public ILEmitter Ldelem_I() => Emit(OpCodes.Ldelem_I);

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="sbyte"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="sbyte"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i1"/>
        public ILEmitter Ldelem_I1() => Emit(OpCodes.Ldelem_I1);

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="short"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="short"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i2"/>
        public ILEmitter Ldelem_I2() => Emit(OpCodes.Ldelem_I2);

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="int"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i4"/>
        public ILEmitter Ldelem_I4() => Emit(OpCodes.Ldelem_I4);

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="long"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="long"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i8"/>
        public ILEmitter Ldelem_I8() => Emit(OpCodes.Ldelem_I8);

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="byte"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="byte"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u1"/>
        public ILEmitter Ldelem_U1() => Emit(OpCodes.Ldelem_U1);

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="ushort"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="ushort"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u2"/>
        public ILEmitter Ldelem_U2() => Emit(OpCodes.Ldelem_U2);

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="uint"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="uint"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u4"/>
        public ILEmitter Ldelem_U4() => Emit(OpCodes.Ldelem_U4);

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="float"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="float"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_r4"/>
        public ILEmitter Ldelem_R4() => Emit(OpCodes.Ldelem_R4);

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="double"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="double"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_r8"/>
        public ILEmitter Ldelem_R8() => Emit(OpCodes.Ldelem_R8);

        /// <summary>
        /// Loads the element from an array index onto the stack as a <see cref="object"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="object"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_ref"/>
        public ILEmitter Ldelem_Ref() => Emit(OpCodes.Ldelem_Ref);

        /// <summary>
        /// Loads the element from an array index onto the stack as an address to a value of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the element to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelema"/>
        public ILEmitter Ldelema(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            return Emit(OpCodes.Ldelema, type);
        }

        /// <summary>
        /// Loads the element from an array index onto the stack as an address to a value of the given <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the element to load.</typeparam>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelema"/>
        public ILEmitter Ldelema<T>() => Emit(OpCodes.Ldelema, typeof(T));
        #endregion
        #region Store Element
        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the value on the stack with the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the element to store.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem"/>
        public ILEmitter Stelem(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type == typeof(IntPtr))
                return Emit(OpCodes.Stelem_I);
            if (type == typeof(sbyte))
                return Emit(OpCodes.Stelem_I1);
            if (type == typeof(short))
                return Emit(OpCodes.Stelem_I2);
            if (type == typeof(int))
                return Emit(OpCodes.Stelem_I4);
            if (type == typeof(long))
                return Emit(OpCodes.Stelem_I8);
            if (type == typeof(float))
                return Emit(OpCodes.Stelem_R4);
            if (type == typeof(double))
                return Emit(OpCodes.Stelem_R8);
            if (type == typeof(object))
                return Emit(OpCodes.Stelem_Ref);
            return Emit(OpCodes.Stelem, type);
        }

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the value on the stack with the given <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the element to store.</typeparam>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem"/>
        public ILEmitter Stelem<T>() => Stelem(typeof(T));

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="IntPtr"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="IntPtr"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i"/>
        public ILEmitter Stelem_I() => Emit(OpCodes.Stelem_I);

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="sbyte"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="sbyte"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i1"/>
        public ILEmitter Stelem_I1() => Emit(OpCodes.Stelem_I1);

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="short"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="short"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i2"/>
        public ILEmitter Stelem_I2() => Emit(OpCodes.Stelem_I2);

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="int"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="int"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i4"/>
        public ILEmitter Stelem_I4() => Emit(OpCodes.Stelem_I4);

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="long"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="long"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i8"/>
        public ILEmitter Stelem_I8() => Emit(OpCodes.Stelem_I8);

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="float"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="float"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_r4"/>
        public ILEmitter Stelem_R4() => Emit(OpCodes.Stelem_R4);

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="double"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="double"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_r8"/>
        public ILEmitter Stelem_R8() => Emit(OpCodes.Stelem_R8);

        /// <summary>
        /// Replaces the <see cref="Array"/> element at a given index with the <see cref="object"/> value on the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
        /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="object"/> elements.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_ref"/>
        public ILEmitter Stelem_Ref() => Emit(OpCodes.Stelem_Ref);
        #endregion

        /// <summary>
        /// Pushes an <see cref="object"/> reference to a new zero-based, one-dimensional <see cref="Array"/> whose elements are the given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <param name="type">The type of values that can be stored in the array.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newarr"/>
        public ILEmitter Newarr(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            return Emit(OpCodes.Newarr, type);
        }

        /// <summary>
        /// Pushes an <see cref="object"/> reference to a new zero-based, one-dimensional <see cref="Array"/> whose elements are the given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of values that can be stored in the array.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newarr"/>
        public ILEmitter Newarr<T>() => Emit(OpCodes.Newarr, typeof(T));

        /// <summary>
        /// Specifies that the subsequent array address operation performs no type check at run time, and that it returns a managed pointer whose mutability is restricted.
        /// </summary>
        /// <remarks>This instruction can only appear before a <see cref="Ldelema"/> instruction.</remarks>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.readonly"/>
        public ILEmitter Readonly() => Emit(OpCodes.Readonly);
        #endregion

        #region Fields
        /// <summary>
        /// Loads the value of the given <see cref="FieldInfo"/> onto the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose value to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldfld"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsfld"/>
        public ILEmitter Ldfld(FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (field.IsStatic)
                return Emit(OpCodes.Ldsfld, field);
            return Emit(OpCodes.Ldfld, field);
        }

        /// <summary>
        /// Loads the value of the given <see cref="FieldInfo"/> onto the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose value to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsfld"/>
        [Obsolete("Use " + nameof(Ldfld) + " instead")]
        public ILEmitter Ldsfld(FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (field.IsStatic)
                return Emit(OpCodes.Ldsfld, field);
            throw new ArgumentException($"The specified field '{field}' must be static", nameof(field));
        }

        /// <summary>
        /// Loads the address of the given <see cref="FieldInfo"/> onto the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose address to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldflda"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsflda"/>
        public ILEmitter Ldflda(FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (field.IsStatic)
                return Emit(OpCodes.Ldsflda, field);
            return Emit(OpCodes.Ldflda, field);
        }

        /// <summary>
        /// Loads the address of the given <see cref="FieldInfo"/> onto the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose address to load.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsflda"/>
        [Obsolete("Use " + nameof(Ldflda) + " instead")]
        public ILEmitter Ldsflda(FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (field.IsStatic)
                return Emit(OpCodes.Ldsflda, field);
            throw new ArgumentException($"The specified field '{field}' must be static", nameof(field));
        }

        /// <summary>
        /// Replaces the value stored in the given <see cref="FieldInfo"/> with the value on the stack.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> whose value to replace.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="NullReferenceException">If the instance value/pointer on the stack is <see langword="null"/> and the <paramref name="field"/> is not <see langword="static"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stfld"/>
        /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stsfld"/>
        public ILEmitter Stfld(FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (field.IsStatic)
                return Emit(OpCodes.Stsfld, field);
            return Emit(OpCodes.Stfld, field);
        }

        /// <summary>
        /// Replaces the value stored in the given static <see cref="FieldInfo"/> with the value on the stack.
        /// </summary>
        /// <param name="field">The static <see cref="FieldInfo"/> whose value to replace.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
        /// <exception cref="NullReferenceException">If the instance value/pointer on the stack is <see langword="null"/> and the <paramref name="field"/> is not <see langword="static"/>.</exception>
        /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stsfld"/>
        [Obsolete("Use " + nameof(Stfld) + " instead")]
        public ILEmitter Stsfld(FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (field.IsStatic)
                return Emit(OpCodes.Ldsfld, field);
            throw new ArgumentException($"The specified field '{field}' must be static", nameof(field));
        }
        #endregion

        #region Load / Store via Address
        /// <summary>
        /// Loads a value from an address onto the stack.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
        public ILEmitter Ldobj(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type == typeof(IntPtr))
                return Ldind_I();
            if (type == typeof(byte) || type == typeof(bool))
                return Ldind_I1();
            if (type == typeof(short))
                return Ldind_I2();
            if (type == typeof(int))
                return Ldind_I4();
            if (type == typeof(long) || type == typeof(ulong))
                return Ldind_I8();
            if (type == typeof(byte))
                return Ldind_U1();
            if (type == typeof(ushort))
                return Ldind_U2();
            if (type == typeof(uint))
                return Ldind_U4();
            if (type == typeof(float))
                return Ldind_R4();
            if (type == typeof(double))
                return Ldind_R8();
            if (type == typeof(object) || type.IsClass)
                return Ldind_Ref();
            return Emit(OpCodes.Ldobj, type);
        }

        /// <summary>
        /// Loads a value from an address onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
        public ILEmitter Ldobj<T>() => Ldobj(typeof(T));

        #region Ldind
        /// <summary>
        /// Loads a value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
        /// <remarks>This is just an alias for Ldobj</remarks>
        public ILEmitter Ldind(Type type) => Ldobj(type);

        /// <summary>
        /// Loads a value from an address onto the stack.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
        /// <remarks>This is just an alias for Ldobj</remarks>
        public ILEmitter Ldind<T>() => Ldobj<T>();

        /// <summary>
        /// Loads a <see cref="IntPtr"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i"/>
        public ILEmitter Ldind_I() => Emit(OpCodes.Ldind_I);

        /// <summary>
        /// Loads a <see cref="sbyte"/> value from an address onto the stack as an <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i1"/>
        public ILEmitter Ldind_I1() => Emit(OpCodes.Ldind_I1);

        /// <summary>
        /// Loads a <see cref="short"/> value from an address onto the stack as an <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i2"/>
        public ILEmitter Ldind_I2() => Emit(OpCodes.Ldind_I2);

        /// <summary>
        /// Loads a <see cref="int"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i4"/>
        public ILEmitter Ldind_I4() => Emit(OpCodes.Ldind_I4);

        /// <summary>
        /// Loads a <see cref="long"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i8"/>
        public ILEmitter Ldind_I8() => Emit(OpCodes.Ldind_I8);

        /// <summary>
        /// Loads a <see cref="byte"/> value from an address onto the stack as an <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u1"/>
        public ILEmitter Ldind_U1() => Emit(OpCodes.Ldind_U1);

        /// <summary>
        /// Loads a <see cref="ushort"/> value from an address onto the stack as an <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u2"/>
        public ILEmitter Ldind_U2() => Emit(OpCodes.Ldind_U2);

        /// <summary>
        /// Loads a <see cref="uint"/> value from an address onto the stack onto the stack as an <see cref="int"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u4"/>
        public ILEmitter Ldind_U4() => Emit(OpCodes.Ldind_U4);

        /// <summary>
        /// Loads a <see cref="float"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_r4"/>
        public ILEmitter Ldind_R4() => Emit(OpCodes.Ldind_R4);

        /// <summary>
        /// Loads a <see cref="double"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_r8"/>
        public ILEmitter Ldind_R8() => Emit(OpCodes.Ldind_R8);

        /// <summary>
        /// Loads a <see cref="object"/> value from an address onto the stack.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_ref"/>
        public ILEmitter Ldind_Ref() => Emit(OpCodes.Ldind_Ref);
        #endregion

        /// <summary>
        /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="TypeLoadException">If <paramref name="type"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
        public ILEmitter Stobj(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type == typeof(IntPtr))
                return Stind_I();
            if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(bool))
                return Stind_I1();
            if (type == typeof(short) || type == typeof(ushort))
                return Stind_I2();
            if (type == typeof(int) || type == typeof(uint))
                return Stind_I4();
            if (type == typeof(long) || type == typeof(ulong))
                return Stind_I8();
            if (type == typeof(float))
                return Stind_R4();
            if (type == typeof(double))
                return Stind_R8();
            if (type == typeof(object) || type.IsClass)
                return Stind_Ref();
            return Emit(OpCodes.Stobj, type);
        }

        /// <summary>
        /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
        /// </summary>
        /// <exception cref="TypeLoadException">If the given <see cref="Type"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
        public ILEmitter Stobj<T>() => Stobj(typeof(T));

        #region Stind
        /// <summary>
        /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="TypeLoadException">If <paramref name="type"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
        /// <remarks>This is just an alias for Stobj</remarks>
        public ILEmitter Stind(Type type) => Stobj(type);

        /// <summary>
        /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
        /// </summary>
        /// <exception cref="TypeLoadException">If the given <see cref="Type"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
        /// <remarks>This is just an alias for Stobj</remarks>
        public ILEmitter Stind<T>() => Stobj(typeof(T));

        /// <summary>
        /// Stores a <see cref="IntPtr"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i"/>
        public ILEmitter Stind_I() => Emit(OpCodes.Stind_I);

        /// <summary>
        /// Stores a <see cref="sbyte"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i1"/>
        public ILEmitter Stind_I1() => Emit(OpCodes.Stind_I1);

        /// <summary>
        /// Stores a <see cref="short"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i2"/>
        public ILEmitter Stind_I2() => Emit(OpCodes.Stind_I2);

        /// <summary>
        /// Stores a <see cref="int"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i4"/>
        public ILEmitter Stind_I4() => Emit(OpCodes.Stind_I4);

        /// <summary>
        /// Stores a <see cref="long"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i8"/>
        public ILEmitter Stind_I8() => Emit(OpCodes.Stind_I8);

        /// <summary>
        /// Stores a <see cref="float"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_r4"/>
        public ILEmitter Stind_R4() => Emit(OpCodes.Stind_R4);

        /// <summary>
        /// Stores a <see cref="double"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_r8"/>
        public ILEmitter Stind_R8() => Emit(OpCodes.Stind_R8);

        /// <summary>
        /// Stores a <see cref="object"/> value in a supplied address.
        /// </summary>
        /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_ref"/>
        public ILEmitter Stind_Ref() => Emit(OpCodes.Stind_Ref);
        #endregion


        /// <summary>
        /// Indicates that an address on the stack might not be aligned to the natural size of the immediately following
        /// <see cref="Ldind"/>, <see cref="Stind"/>, <see cref="Ldfld"/>, <see cref="Stfld"/>, <see cref="Ldobj"/>, <see cref="Stobj"/>, <see cref="Initblk"/>, or <see cref="Cpblk"/> instruction.
        /// </summary>
        /// <param name="alignment">Specifies the generated code should assume the address is <see cref="byte"/>, double-<see cref="byte"/>, or quad-<see cref="byte"/> aligned.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="alignment"/> is not 1, 2, or 4.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unaligned"/>
        public ILEmitter Unaligned(int alignment)
        {
            if (alignment != 1 && alignment != 2 && alignment != 4)
                throw new ArgumentOutOfRangeException(nameof(alignment), alignment, "Alignment can only be 1, 2, or 4");
            return Emit(OpCodes.Unaligned, (byte)alignment);
        }

        /// <summary>
        /// Indicates that an address currently on the stack might be volatile, and the results of reading that location cannot be cached or that multiple stores to that location cannot be suppressed.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.volatile"/>
        public ILEmitter Volatile() => Emit(OpCodes.Volatile);
        #endregion

        #region Upon Type
        /// <summary>
        /// Pushes a typed reference to an instance of a given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of reference to push.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mkrefany"/>
        public ILEmitter Mkrefany(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            return Emit(OpCodes.Mkrefany, type);
        }

        /// <summary>
        /// Pushes a typed reference to an instance of a given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of reference to push.</typeparam>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mkrefany"/>
        public ILEmitter Mkrefany<T>()
            => Emit(OpCodes.Mkrefany, typeof(T));

        /// <summary>
        /// Retrieves the type token embedded in a typed reference.
        /// </summary>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanytype"/>
        public ILEmitter Refanytype() => Emit(OpCodes.Refanytype);

        /// <summary>
        /// Retrieves the address (<see langword="&amp;"/>) embedded in a typed reference.
        /// </summary>
        /// <param name="type">The type of reference to retrieve the address.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        /// <exception cref="InvalidCastException">If <paramref name="type"/> is not the same as the <see cref="Type"/> of the reference.</exception>
        /// <exception cref="TypeLoadException">If <paramref name="type"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanyval"/>
        public ILEmitter Refanyval(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            return Emit(OpCodes.Refanyval, type);
        }

        /// <summary>
        /// Retrieves the address (<see langword="&amp;"/>) embedded in a typed reference.
        /// </summary>
        /// <typeparam name="T">The type of reference to retrieve the address.</typeparam>
        /// <exception cref="InvalidCastException">If <typeparamref name="T"/> is not the same as the <see cref="Type"/> of the reference.</exception>
        /// <exception cref="TypeLoadException">If <typeparamref name="T"/> cannot be found.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanyval"/>
        public ILEmitter Refanyval<T>() => Emit(OpCodes.Refanyval, typeof(T));

        /// <summary>
        /// Pushes the size, in <see cref="byte"/>s, of a given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get the size of.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sizeof"/>
        public ILEmitter Sizeof(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            return Emit(OpCodes.Sizeof, type);
        }

        /// <summary>
        /// Pushes the size, in <see cref="byte"/>s, of a given <see cref="Type"/> onto the stack.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to get the size of.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if the given <see cref="Type"/> is <see langword="null"/>.</exception>
        /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sizeof"/>
        public ILEmitter Sizeof<T>() => Emit(OpCodes.Sizeof, typeof(T));
            
        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return TextBuilder.Build(text => text.AppendDelimit(Environment.NewLine, _operations, (tb, instruction) => instruction.ToString(tb)));
        }
    }
}
