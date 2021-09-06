/*using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission
{
    internal sealed class ILEmitter : IFluentILGenerator,
                                      IFluentILEmitter
    {
        /// <summary>
        /// Gets the correct <see cref="OpCode"/> to call the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static OpCode GetCallOpCode(MethodBase method)
        {
            //If the method is static, we know it can never be null, so we can Call
            if (method.IsStatic)
                return OpCodes.Call;
            //If the method owner is a struct, it can also never be null, so we can Call
            if ((method.ReflectedType != null && method.ReflectedType.IsValueType) ||
                (method.DeclaringType != null && method.DeclaringType.IsValueType))
            {
                return OpCodes.Call;
            }
            return OpCodes.Callvirt;
        }
        
        private readonly ILGenerator _ilGenerator;
        private readonly List<Instruction> _instructions;
        
        private void AddInstruction(Instruction instruction)
        {
            if (instruction.ILGeneratorMethod != ILGeneratorMethod.None)
            {
                AddInstruction(instruction.ILGeneratorMethod, instruction.Argument);
            }
            else
            {
                AddInstruction(instruction.OpCode, instruction.Argument);
            }
        }

        private void AddInstruction(OpCode opCode, object? argument = null)
        {
            _instructions.Add(new Instruction(_instructions.Count, opCode, argument));
        }
        private void AddInstruction(ILGeneratorMethod ilGeneratorMethod, params object?[] arguments)
        {
            _instructions.Add(new Instruction(_instructions.Count, ilGeneratorMethod, arguments));
        }
        
      
        
     

        /// <inheritdoc />
        public Instruction this[int index] => _instructions[index];
        
        public int Count => _instructions.Count;

        protected MSILStream()
        {
            _instructions = new List<Instruction>(16);
        }

        /// <inheritdoc />
        public IEnumerator<Instruction> GetEnumerator() => _instructions.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => _instructions.GetEnumerator();
    }
}*/