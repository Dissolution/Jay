/*using System.Reflection.Emit;
using Jay.Reflection.Emission;

namespace Jay.Reflection.Deconstruction;

public abstract class ILPattern
{
    protected static Instruction? GetLastMatchingInstruction(MatchContext context)
    {
        return context._instructionNode?.Previous?.Value;
    }

    public static ILPattern Optional(OpCode opCode)
    {
        return Optional(OpCode(opCode));
    }

    public static ILPattern Optional(params OpCode[] opCodes)
    {
        var len = opCodes.Length;
        var patterns = new ILPattern[len];
        for (var i = 0; i < len; i++)
        {
            patterns[i] = OpCode(opCodes[i]);
        }
        return Optional(Sequence(patterns));
    }

    public static ILPattern Optional(ILPattern pattern)
    {
        return new OptionalPattern(pattern);
    }

    public static MatchContext Match(MethodBase method, ILPattern pattern)
    {
        var instructions = method.GetInstructions();
        if (instructions.Count == 0)
            throw new ArgumentException("Method does not provide any IL Instructions", nameof(method));
        var context = new MatchContext(instructions);
        pattern.Match(context);
        return context;
    }

    internal class OptionalPattern : ILPattern
    {
        private readonly ILPattern _pattern;

        public OptionalPattern(ILPattern optional)
        {
            _pattern = optional;
        }

        public override void Match(MatchContext context)
        {
            _pattern.TryMatch(context);
        }
    }

    public static ILPattern Sequence(params ILPattern[] patterns)
    {
        return new SequencePattern(patterns);
    }

    internal class SequencePattern : ILPattern
    {
        private readonly ILPattern[] _patterns;

        public SequencePattern(ILPattern[] patterns)
        {
            _patterns = patterns;
        }

        public override void Match(MatchContext context)
        {
            foreach (var pattern in _patterns)
            {
                pattern.Match(context);
                if (!context.IsMatch)
                    break;
            }
        }
    }

    public static ILPattern OpCode(OpCode opCode)
    {
        return new OpCodePattern(opCode);
    }

    internal class OpCodePattern : ILPattern
    {
        readonly OpCode _opCode;

        public OpCodePattern(OpCode opCode)
        {
            _opCode = opCode;
        }

        public override void Match(MatchContext context)
        {
            if (context.Instruction == null)
            {
                context.IsMatch = false;
                return;
            }

            context.IsMatch = context.Instruction.OpCode == _opCode;
            context.Advance();
        }
    }

    public static ILPattern Either(ILPattern a, ILPattern b)
    {
        return new EitherPattern(a, b);
    }

    internal class EitherPattern : ILPattern
    {
        private readonly ILPattern _a;
        private readonly ILPattern _b;

        public EitherPattern(ILPattern a, ILPattern b)
        {
            _a = a;
            _b = b;
        }

        public override void Match(MatchContext context)
        {
            if (!_a.TryMatch(context))
                _b.Match(context);
        }
    }

    public abstract void Match(MatchContext context);

    public bool TryMatch(MatchContext context)
    {
        var instructions = context._instructions;
        Match(context);

        if (context.IsMatch)
            return true;

        context.Reset(instructions);
        return false;
    }
}

public sealed class MatchContext
{
    private readonly Dictionary<object, object?> _data = new(0);
    internal InstructionStream _instructions;
    internal LinkedListNode<Instruction>? _instructionNode;
    private bool _success;

    public bool IsMatch
    {
        get => _success;
        set => _success = true;
    }

    public Instruction? Instruction => _instructionNode?.Value;

    internal MatchContext(InstructionStream instructions)
    {
        Reset(instructions);
    }

    public bool TryGetData(object key, out object? value)
    {
        return _data.TryGetValue(key, out value);
    }

    public void AddData(object key, object? value)
    {
        _data.Add(key, value);
    }

    internal void Reset(InstructionStream instructions)
    {
        _instructions = instructions;
        _instructionNode = _instructions.First;
        _success = true;
    }

    internal void Advance()
    {
        _instructionNode = _instructionNode?.Next;
    }
}*/