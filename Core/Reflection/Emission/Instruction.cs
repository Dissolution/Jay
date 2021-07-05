using Jay.Debugging;
using Jay.Debugging.Dumping;
using Jay.Text;
using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Reflection.Cloning;

#pragma warning disable 618

namespace Jay.Reflection.Emission 
{
	public sealed class Instruction : IEquatable<Instruction>,
	                                  ICloneable<Instruction>
	{
		public int Offset { get; internal set; }
		public ILGeneratorMethod ILGeneratorMethod { get; }
		public OpCode OpCode { get; }
		public object? Argument { get; }

		public int Size 
		{
			get
			{
				if (ILGeneratorMethod != ILGeneratorMethod.None)
				{
					return 0;
				}
				switch (OpCode.OperandType) 
				{
					case OperandType.InlineSwitch:
					{
						if (Argument is Instruction[] instructionArray)
						{
							return OpCode.Size + ((1 + instructionArray.Length) * 4);
						}

						throw new InvalidOperationException("InlineSwitch operand must be an Instruction[]");
					}
					case OperandType.InlineI8:
					case OperandType.InlineR:
						return OpCode.Size + 8;
					case OperandType.InlineBrTarget:
					case OperandType.InlineField:
					case OperandType.InlineI:
					case OperandType.InlineMethod:
					case OperandType.InlineString:
					case OperandType.InlineTok:
					case OperandType.InlineType:
					case OperandType.ShortInlineR:
						return OpCode.Size + 4;
					case OperandType.InlineVar:
						return OpCode.Size + 2;
					case OperandType.ShortInlineBrTarget:
					case OperandType.ShortInlineI:
					case OperandType.ShortInlineVar:
						return OpCode.Size + 1;
					case OperandType.InlineNone:
					case OperandType.InlinePhi:
					case OperandType.InlineSig:
					default:
						return OpCode.Size;
				}
			}
		}

		private Instruction(int offset,
		                    ILGeneratorMethod ilGeneratorMethod,
		                    OpCode opCode,
		                    object? argument)
		{
			this.Offset = offset;
			this.ILGeneratorMethod = ilGeneratorMethod;
			this.OpCode = opCode;
			this.Argument = argument;
		}
		
		internal Instruction(int offset,
		                     OpCode opcode, 
		                     object? argument)
		{
			this.Offset = offset;
			this.ILGeneratorMethod = ILGeneratorMethod.None;
			this.OpCode = opcode;
			this.Argument = argument;
		}

		internal Instruction(int offset,
		                     ILGeneratorMethod ilGeneratorMethod,
		                     params object?[] methodArgs)
		{
			this.Offset = offset;
			this.ILGeneratorMethod = ilGeneratorMethod;
			this.OpCode = default;
			this.Argument = methodArgs;
		}

		public bool Equals(Instruction? instruction)
		{
			if (ReferenceEquals(this, instruction))
				return true;
			if (instruction is null)
				return false;
			return instruction.Offset == Offset &&
			       instruction.ILGeneratorMethod == ILGeneratorMethod &&
			       instruction.OpCode == OpCode &&
			       Jay.Comparison.Comparison.Equals(instruction.Argument, Argument);
		}
		
		public override bool Equals(object? obj)
		{
			if (obj is Instruction instruction)
				return Equals(instruction);
			return false;
		}

		public override int GetHashCode()
		{
			return Hasher.Create(Offset, ILGeneratorMethod, OpCode, Argument);
		}

		private static TextBuilder AppendILOffset(TextBuilder builder, Instruction instruction)
		{
			return builder.Append("IL_")
			       .AppendFormat(instruction.Offset, "X4");
		}
		
		private static TextBuilder AppendLabel(TextBuilder builder, Label label)
		{
			return builder.Append("LABEL_")
			       .Append(label.GetHashCode())
			       .Append(':');
		}

		private static TextBuilder AppendArgument(TextBuilder text, object? operand)
		{
			if (operand is null) return text;
			text.Append(" '");
			switch (operand)
			{
				case Instruction instruction:
					AppendILOffset(text, instruction);
					break;
				case Label label1:
					AppendLabel(text, label1);
					break;
				case Instruction[] instructions:
					text.AppendDelimit(", ", instructions, (tb, instr) => AppendILOffset(tb, instr));
					break;
				case Label[] labels:
					text.AppendDelimit(", ", labels, (tb, label) => AppendLabel(tb, label));
					break;
				case string str:
					text.Append('"').Append(str).Append('"');
					break;
				case MemberInfo memberInfo:
					text.AppendDump(memberInfo, MemberDumpOptions.Default);
					break;
				case IntPtr intPtr:
					text.AppendFormat(intPtr, "X");
					break;
				case int integer:
					text.Append(integer);
					break;
				case sbyte signedByte:
					text.Append(signedByte);
					break;
				case byte b:
					text.Append(b);
					break;
				case float f:
					text.Append(f).Append('f');
					break;
				case double d:
					text.Append(d).Append('d');
					break;
				case decimal m:
					text.Append(m).Append('m');
					break;
				case IEnumerable enumerable:
					text.AppendDelimit(", ", enumerable.AsObjectEnumerable(), (tb, obj) => AppendArgument(tb, obj));
					break;
				default:
					text.Append(operand);
					break;
			}

			return text.Append('\'');
		}

		internal void Append(TextBuilder text)
		{
			if (this.ILGeneratorMethod != ILGeneratorMethod.None)
			{
				text.Append(this.ILGeneratorMethod).Append('(');
				AppendArgument(text, this.Argument).Append(')');
			}
			else
			{
				var opCode = this.OpCode;
				AppendILOffset(text, this);
				text.Append(": ").Append(opCode.Name);
				AppendArgument(text, this.Argument);
			}
		}

		/// <inheritdoc />
		public Instruction Clone(CloneType cloneType)
		{
			if (cloneType == CloneType.Shallow)
			{
				return new Instruction(Offset, ILGeneratorMethod, OpCode, Argument);
			}
			else
			{
				return new Instruction(Offset, ILGeneratorMethod, OpCode, Cloner.Clone(Argument));
			}
		}

		public override string ToString() => TextBuilder.Build(Append!);
	}
}