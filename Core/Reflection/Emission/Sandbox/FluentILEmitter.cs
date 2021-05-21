using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission.Sandbox
{
    public abstract class FluentILEmitter<TEmitter> : IFluentILEmitter<TEmitter>
        where TEmitter : FluentILEmitter<TEmitter>
    {
        protected readonly TEmitter _emitter;
        protected readonly List<Operation> _operations;

        protected FluentILEmitter()
        {
            _emitter = (this as TEmitter)!;
            _operations = new List<Operation>(16);
        }

        public virtual TEmitter Emit(OpCode opCode)
        {
            _operations.Add(new Operation(opCode));
            return _emitter;
        }
        
        public virtual TEmitter Emit(OpCode opCode, byte arg)
        {
            _operations.Add(new Operation(opCode, arg));
            return _emitter;
        }
        
        public virtual TEmitter Emit(OpCode opCode, sbyte arg)
        {
            _operations.Add(new Operation(opCode, arg));
            return _emitter;
        }
        
        public virtual TEmitter Emit(OpCode opCode, short arg)
        {
            _operations.Add(new Operation(opCode, arg));
            return _emitter;
        }
        
        public virtual TEmitter Emit(OpCode opCode, int arg)
        {
            _operations.Add(new Operation(opCode, arg));
            return _emitter;
        }
        
        public virtual TEmitter Emit(OpCode opCode, MethodInfo method)
        {
            _operations.Add(new Operation(opCode, method));
            return _emitter;
        }
    }
}