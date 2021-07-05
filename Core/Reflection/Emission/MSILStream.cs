using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace Jay.Reflection.Emission
{
    public abstract class MSILStream : IMSILStream
    {
        /// <summary>
        /// Gets the correct <see cref="OpCode"/> to call the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        protected static OpCode GetCallOpCode(MethodBase method)
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
        
        
        protected readonly List<Instruction> _instructions;

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
}