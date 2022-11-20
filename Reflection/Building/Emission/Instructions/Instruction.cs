using Jay.Dumping;
using Jay.Dumping.Interpolated;
using Jay.Reflection.Comparison;

namespace Jay.Reflection.Building.Emission.Instructions;

public abstract class Instruction : IEquatable<Instruction>, IDumpable
{
    static Instruction()
    {
        DefaultComparers.Replace<LocalBuilder>(new LocalBuilderEqualityComparer());
    }
    
    public abstract bool Equals(Instruction? instruction);

    public abstract void DumpTo(ref DumpStringHandler dumpHandler, DumpFormat dumpFormat = default);
}