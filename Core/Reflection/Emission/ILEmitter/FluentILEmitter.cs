using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission
{
    public class FluentILEmitter : IFluentILEmitter,
                                   IMSILStream
    {
        internal readonly AttachedFluentILGenerator _ilGenerator;
        internal readonly IFluentILEmitter This;

        /// <inheritdoc />
        public int Count => _ilGenerator.Count;

        /// <inheritdoc />
        public Instruction this[int index] => _ilGenerator[index];
        
        /// <inheritdoc />
        public IFluentILGenerator Generator => _ilGenerator;

        /// <inheritdoc />
        public IWriter<IFluentILEmitter> Console { get; }

        /// <inheritdoc />
        public IWriter<IFluentILEmitter> Debug { get; }

        /// <inheritdoc />
        public IArrayInteraction<IFluentILEmitter> Array { get; }
        
        public FluentILEmitter(DynamicMethod dynamicMethod)
        {
            _ilGenerator = new AttachedFluentILGenerator(dynamicMethod.GetILGenerator());
            this.This = this;

            this.Console = new Writer(this);
            this.Debug = new Writer(this);
            this.Array = new ArrayInteraction(this);
        }
        
        public FluentILEmitter(ILGenerator ilGenerator)
        {
            _ilGenerator = new AttachedFluentILGenerator(ilGenerator);
            this.This = this;
            this.Console = new Writer(this);
            this.Debug = new Writer(this);
            this.Array = new ArrayInteraction(this);
        }
        
        public FluentILEmitter(AttachedFluentILGenerator ilGenerator)
        {
            _ilGenerator = ilGenerator ?? throw new ArgumentNullException(nameof(ilGenerator));
            this.This = this;
            this.Console = new Writer(this);
            this.Debug = new Writer(this);
            this.Array = new ArrayInteraction(this);
        }

        /// <inheritdoc />
        public IFluentILEmitter Generate(Action<AttachedFluentILGenerator> generate)
        {
            generate(_ilGenerator);
            return this;
        }


        /// <inheritdoc />
        public IFluentILEmitter Append(IMSILStream ilStream)
        {
            _ilGenerator.Append(ilStream);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Try(Action<ITryBuilder<IFluentILEmitter>> buildTry)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IFluentILEmitter Scoped(Action<IFluentILEmitter> scopedBlock)
        {
            _ilGenerator.BeginScope();
            scopedBlock(this);
            _ilGenerator.EndScope();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Define(out Label label)
        {
            _ilGenerator.DefineLabel(out label);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter MarkLabel(Label label)
        {
            _ilGenerator.MarkLabel(label);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Branch(Label label)
        {
            _ilGenerator.Br(label);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter BranchIf(CompareCondition condition, Label label)
        {
            switch (condition)
            {
                case CompareCondition.Default:
                {
                    _ilGenerator.Brfalse(label);
                    break;
                }
                case CompareCondition.NotDefault:
                {
                    _ilGenerator.Brtrue(label);
                    break;
                }
                case CompareCondition.Equal:
                {
                    _ilGenerator.Beq(label);
                    break;
                }
                case CompareCondition.NotEqual:
                {
                    _ilGenerator.Bne_Un(label);
                    break;
                }
                case CompareCondition.GreaterThan:
                {
                    _ilGenerator.Bgt(label);
                    break;
                }
                case CompareCondition.GreaterThanOrEqual:
                {
                    _ilGenerator.Bge(label);
                    break;
                }
                case CompareCondition.LessThan:
                {
                    _ilGenerator.Blt(label);
                    break;
                }
                case CompareCondition.LessThanOrEqual:
                {
                    _ilGenerator.Ble(label);
                    break;
                }
                default:
                    break;
            }
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Return()
        {
            _ilGenerator.Ret();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter DeclareLocal(Type localType, out LocalBuilder local)
        {
            _ilGenerator.DeclareLocal(localType, out local);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter DeclareLocal(Type localType, bool pinned, out LocalBuilder local)
        {
            _ilGenerator.DeclareLocal(localType, pinned, out local);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadFromLocal(LocalBuilder local)
        {
            _ilGenerator.Ldloc(local);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadLocalAddress(LocalBuilder local)
        {
            _ilGenerator.Ldloca(local);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter StoreInLocal(LocalBuilder local)
        {
            _ilGenerator.Stloc(local);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadArgument(int index)
        {
            _ilGenerator.Ldarg(index);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadArgumentAddress(int index)
        {
            _ilGenerator.Ldarga(index);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter StoreArgument(int index)
        {
            _ilGenerator.Starg(index);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadConstant(int value)
        {
            _ilGenerator.Ldc_I4(value);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadConstant(long value)
        {
            _ilGenerator.Ldc_I8(value);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadConstant(float value)
        {
            _ilGenerator.Ldc_R4(value);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadConstant(double value)
        {
            _ilGenerator.Ldc_R8(value);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadNull()
        {
            _ilGenerator.Ldnull();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadString(string? text)
        {
            _ilGenerator.Ldstr(text);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadToken(Type type)
        {
            _ilGenerator.Ldtoken(type);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadToken(FieldInfo field)
        {
            _ilGenerator.Ldtoken(field);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadToken(MethodInfo method)
        {
            _ilGenerator.Ldtoken(method);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadValue(object? value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadValue<T>([AllowNull] T value)
        {
            if (value is null)
                return LoadNull();
            switch (value)
            {
                case bool boolean:
                    return boolean ? LoadConstant(1) : LoadConstant(0);
                case byte b:
                    return LoadConstant((int) b);
                case sbyte sb:
                    return LoadConstant((int) sb);
                case short sh:
                    return LoadConstant((int) sh);
                case ushort ush:
                    return LoadConstant((int) ush);
                case char c:
                    return LoadConstant((int) c);
                case int i:
                    return LoadConstant(i);
                case uint ui:
                    return LoadConstant((int) ui);
                case long l:
                    return LoadConstant(l);
                case ulong ul:
                    return LoadConstant((long) ul);
                case float f:
                    return LoadConstant(f);
                case double d:
                    return LoadConstant(d);
                case string s:
                    return LoadString(s);
            }

            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadField(FieldInfo field)
        {
            _ilGenerator.Ldfld(field);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadFieldAddress(FieldInfo field)
        {
            _ilGenerator.Ldflda(field);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter StoreField(FieldInfo field)
        {
            _ilGenerator.Stfld(field);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadFromAddress(Type type)
        {
            _ilGenerator.Ldind(type);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter StoreToAddress(Type type)
        {
            _ilGenerator.Stind(type);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Break()
        {
            _ilGenerator.Break();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter ReThrow()
        {
            _ilGenerator.Rethrow();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Throw<TException>() where TException : Exception, new()
        {
            _ilGenerator.ThrowException<TException>();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Throw<TException>(string? message) where TException : Exception
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IFluentILEmitter Throw<TException>(params object?[] exceptionConstructorParameters) where TException : Exception
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IFluentILEmitter Add()
        {
            _ilGenerator.Add();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Divide()
        {
            _ilGenerator.Div();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Multiply()
        {
            _ilGenerator.Mul();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Remainder()
        {
            _ilGenerator.Rem();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Subtract()
        {
            _ilGenerator.Sub();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter And()
        {
            _ilGenerator.And();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Negate()
        {
            _ilGenerator.Neg();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Not()
        {
            _ilGenerator.Not();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Or()
        {
            _ilGenerator.Or();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter ShiftLeft()
        {
            _ilGenerator.Shl();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter ShiftRight()
        {
            _ilGenerator.Shr();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Xor()
        {
            _ilGenerator.Xor();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Box(Type type)
        {
            _ilGenerator.Box(type);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Unbox(Type type)
        {
            _ilGenerator.Unbox_Any(type);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter UnboxToPointer(Type type)
        {
            _ilGenerator.Unbox(type);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter CastClass(Type type)
        {
            _ilGenerator.Castclass(type);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter IsInstance(Type type)
        {
            _ilGenerator.Isinst(type);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Compare(CompareCondition condition)
        {
            switch (condition)
            {
                case CompareCondition.Default:
                {
                    throw new NotImplementedException();
                }
                case CompareCondition.NotDefault:
                {
                    throw new NotImplementedException();
                }
                case CompareCondition.Equal:
                {
                    _ilGenerator.Ceq();
                    break;
                }
                case CompareCondition.NotEqual:
                {
                    _ilGenerator.Ceq();
                    _ilGenerator.Not();
                    break;
                }
                case CompareCondition.GreaterThan:
                {
                    _ilGenerator.Cgt();
                    break;
                }
                case CompareCondition.GreaterThanOrEqual:
                {
                    _ilGenerator.Clt();
                    _ilGenerator.Not();
                    break;
                }
                case CompareCondition.LessThan:
                {
                    _ilGenerator.Clt();
                    break;
                }
                case CompareCondition.LessThanOrEqual:
                {
                    _ilGenerator.Cgt();
                    _ilGenerator.Neg();
                    break;
                }
                default:
                    break;
            }
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter SizeOf(Type type)
        {
            _ilGenerator.Sizeof(type);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Call(MethodInfo method)
        {
            _ilGenerator.Call(method);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter New(ConstructorInfo constructor)
        {
            _ilGenerator.Newobj(constructor);
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Pop()
        {
            _ilGenerator.Pop();
            return This;
        }

        /// <inheritdoc />
        public IFluentILEmitter Duplicate()
        {
            _ilGenerator.Dup();
            return This;
        }

        /// <inheritdoc />
        public IEnumerator<Instruction> GetEnumerator()
        {
            return _ilGenerator.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => _ilGenerator.GetEnumerator();

        /// <inheritdoc />
        public override string ToString()
        {
            return _ilGenerator.ToString();
        }
    }
}