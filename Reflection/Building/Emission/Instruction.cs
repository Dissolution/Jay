using System.Reflection.Emit;

namespace Jay.Reflection.Emission;

public abstract class Instruction : IEquatable<Instruction>
{
    protected static bool Equals(LocalBuilder x, LocalBuilder y)
    {
        return x.IsPinned == y.IsPinned &&
               x.LocalIndex == y.LocalIndex &&
               x.LocalType == y.LocalType;
    }

    protected Instruction() : base() { }

    public abstract bool Equals(Instruction? instruction);
}