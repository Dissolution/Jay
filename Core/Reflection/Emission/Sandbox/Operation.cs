using Jay.Debugging;
using Jay.Debugging.Dumping;
using Jay.Text;
using System;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission.Sandbox
{
    public sealed class Operation : IEquatable<Operation>
    {
        private readonly ILGeneratorMethod? _ilGeneratorMethod;
        private readonly OpCode? _opCode;
        private readonly object? _argument;

        public Operation(ILGeneratorMethod ilGeneratorMethod, object? argument = null)
        {
            _ilGeneratorMethod = ilGeneratorMethod;
            _opCode = null;
            _argument = argument;
        }

        public Operation(OpCode opCode, object? argument = null)
        {
            _ilGeneratorMethod = null;
            _opCode = opCode;
            _argument = argument;
        }

        public bool Equals(Operation? operation)
        {
            if (operation is null) return false;
            return _ilGeneratorMethod == operation._ilGeneratorMethod &&
                   _opCode == operation._opCode &&
                   Comparison.Comparison.Equals(_argument, operation._argument);
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is Operation operation)
                return Equals(operation);
            if (obj is ILGeneratorMethod ilGeneratorMethod)
                return _ilGeneratorMethod == ilGeneratorMethod;
            if (obj is OpCode opCode)
                return _opCode == opCode;
            return false;
        }

        public override int GetHashCode()
        {
            return Hasher.Create(_ilGeneratorMethod, _opCode, _argument);
        }

        public override string ToString()
        {
            return TextBuilder.Build(this, (tb, op) =>
            {
                if (op._ilGeneratorMethod.HasValue)
                {
                    tb.Append(op._ilGeneratorMethod);
                }
                else
                {
                    tb.Append(op._opCode);
                }

                if (op._argument != null)
                {
                    tb.AppendDump(op._argument);
                }
            });
        }

     
    }
}