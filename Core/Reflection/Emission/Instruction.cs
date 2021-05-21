using Jay.Debugging;
using Jay.Debugging.Dumping;
using Jay.Text;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission 
{
	public sealed class Instruction : IEquatable<Instruction>
	{
		public int Offset { get; internal set; }
		public ILGeneratorMethod ILGeneratorMethod { get; internal set; }
		public OpCode OpCode { get; internal set; }
		public object? Operand { get; internal set; }

		public int Size 
		{
			get
			{
				if (ILGeneratorMethod != ILGeneratorMethod.None)
					return 0;
				int size = OpCode.Size;

				switch (OpCode.OperandType) 
				{
					case OperandType.InlineSwitch:
					{
						if (Operand is Instruction[] instructionArray)
						{
							size += (1 + instructionArray.Length) * 4;
						}
						else
						{
							throw new InvalidOperationException("InlineSwitch operand must be an Instruction[]");
						}
						break;
					}
					case OperandType.InlineI8:
					case OperandType.InlineR:
						size += 8;
						break;
					case OperandType.InlineBrTarget:
					case OperandType.InlineField:
					case OperandType.InlineI:
					case OperandType.InlineMethod:
					case OperandType.InlineString:
					case OperandType.InlineTok:
					case OperandType.InlineType:
					case OperandType.ShortInlineR:
						size += 4;
						break;
					case OperandType.InlineVar:
						size += 2;
						break;
					case OperandType.ShortInlineBrTarget:
					case OperandType.ShortInlineI:
					case OperandType.ShortInlineVar:
						size += 1;
						break;
					case OperandType.InlineNone:
					case OperandType.InlinePhi:
					case OperandType.InlineSig:
					default:
						// Do nothing to size
						break;
				}

				return size;
			}
		}

		internal Instruction(int offset,
		                     OpCode opcode, 
		                     object? operand)
		{
			this.Offset = offset;
			this.ILGeneratorMethod = ILGeneratorMethod.None;
			this.OpCode = opcode;
			this.Operand = operand;
		}

		internal Instruction(int offset,
		                     ILGeneratorMethod ilGeneratorMethod,
		                     params object[] methodArgs)
		{
			this.Offset = offset;
			this.ILGeneratorMethod = ilGeneratorMethod;
			this.OpCode = default;
			this.Operand = methodArgs;
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
			       Comparison.Comparison.Equals(instruction.Operand, Operand);
		}
		
		public override bool Equals(object? obj)
		{
			if (obj is Instruction instruction)
				return Equals(instruction);
			return false;
		}

		public override int GetHashCode()
		{
			return Hasher.Create(Offset, ILGeneratorMethod, OpCode, Operand);
		}

		private static void AppendLabel(TextBuilder builder, Instruction instruction)
		{
			builder.Append("IL_")
			       .AppendFormat(instruction.Offset, "x4");
		}

		private static void AppendOperand(TextBuilder text, OperandType operandType, object operand)
		{
			text.Append(' ');
			switch (operandType)
			{
			    case OperandType.ShortInlineBrTarget:
			    case OperandType.InlineBrTarget:
			    {
			        if (operand is Instruction opInst)
			        {
			            AppendLabel(text, opInst);
			        }
			        else
			        {
			            throw new InvalidOperationException();
			        }

			        break;
			    }
			    case OperandType.InlineSwitch:
			    {
			        if (operand is Instruction[] labels)
			        {
			            text.AppendDelimit(',', labels, AppendLabel);
			        }
			        else
			        {
			            throw new InvalidOperationException();
			        }

			        break;
			    }
			    case OperandType.InlineString:
			    {
			        if (!(operand is string str))
			        {
			            str = operand.ToString() ?? string.Empty;
			        }

			        text.Append('"')
			            .Append(str)
			            .Append('"');
			        break;
			    }
			    case OperandType.InlineField:
			    {
			        if (operand is FieldInfo field)
			        {
			            text.AppendDump(field);
			        }
			        else
			        {
			            throw new InvalidOperationException();
			        }
			        break;
			    }
			    case OperandType.InlineI:
			    case OperandType.ShortInlineI:
			    {
			        if (operand is IntPtr intPtr)
			        {
			            text.AppendFormat(intPtr, "X");
			        }
			        else if (operand is int integer)
			        {
				        text.Append(integer);
			        }
			        else if (operand is sbyte signedByte)
			        {
				        text.Append(signedByte);
			        }
			        else
			        {
			            throw new InvalidOperationException();
			        }

			        break;
			    }
			    case OperandType.InlineI8:
			    {
			        if (operand is byte b)
			        {
			            text.Append(b)
			                .Append('b');
			        }
			        else
			        {
			            throw new InvalidOperationException();
			        }
			        break;
			    }
			    case OperandType.InlineMethod:
			    {
			        if (operand is MethodBase methodBase)
			        {
			            text.AppendDump(methodBase);
			        }
			        else
			        {
			            throw new InvalidOperationException();
			        }
			        break;
			    }
			    case OperandType.InlineR:
			    case OperandType.ShortInlineR:
			    {
			        if (operand is float f)
			        {
			            text.Append(f)
			                .Append('f');
			        }
			        else if (operand is double d)
			        {
			            text.Append(d)
			                .Append('d');
			        }
			        else if (operand is decimal m)
			        {
			            text.Append(m)
			                .Append('m');
			        }
			        else
			        {
			            throw new InvalidOperationException();
			        }
			        break;
			    }
			    case OperandType.InlineType:
			    {
			        if (operand is Type type)
			        {
			            text.AppendDump(type);
			        }
			        else
			        {
			            throw new InvalidOperationException();
			        }
			        break;
			    }
			    case OperandType.InlineTok:
			    {
				    if (operand is MemberInfo member)
				    {
					    text.AppendDump(member);
				    }
				    else
				    {
					    throw new InvalidOperationException();
				    }
				    break;
			    }
			    case OperandType.InlineVar:
			    case OperandType.ShortInlineVar:
			    {
				    // Variables?
				    text.AppendDump(operand);
				    break;
			    }
			    case OperandType.InlineNone:
			    //case OperandType.InlinePhi:
			    case OperandType.InlineSig:
			    default:
			    {
			        Hold.Debug(operandType, operand);
			        text.Append(operand);
			        break;
			    }
			}
		}

		internal void ToString(TextBuilder text)
		{
			if (this.ILGeneratorMethod != ILGeneratorMethod.None)
			{
				text.Append(this.ILGeneratorMethod)
				    .Append('(');
				if (this.Operand is object[] args)
				{
					text.AppendDelimit(',', args, (tb, a) => tb.AppendDump(a));
				}
				else
				{
					text.AppendDump(this.Operand);
				}

				text.Append(')');
			}
			else
			{
				var opCode = this.OpCode;
				AppendLabel(text, this);
				text.Append(": ")
				    .Append(opCode.Name);
				var operand = this.Operand;
				if (operand is null)
					return;
				AppendOperand(text, opCode.OperandType, operand);
			}
		}
		
		public override string ToString() => TextBuilder.Build(ToString);
	}
}