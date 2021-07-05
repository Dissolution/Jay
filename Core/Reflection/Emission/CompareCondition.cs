using Jay.Debugging.Dumping;

namespace Jay.Reflection.Emission
{
    public enum CompareCondition
    {
        [DumpAs("== default")]
        Default,
        [DumpAs("!= default")]
        NotDefault,
        [DumpAs("==")]
        Equal,
        [DumpAs("!=")]
        NotEqual,
        [DumpAs(">")]
        GreaterThan,
        [DumpAs(">=")]
        GreaterThanOrEqual,
        [DumpAs("<")]
        LessThan,
        [DumpAs("<=")]
        LessThanOrEqual,
    }
}