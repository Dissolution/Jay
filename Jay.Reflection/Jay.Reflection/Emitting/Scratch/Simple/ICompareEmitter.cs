namespace Jay.Reflection.Emitting.Scratch.Simple;

public interface ICompareEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self Equal();
    Self NotEqual();
    Self GreaterThan();
    Self GreaterOrEqualThan();
    Self LessThan();
    Self LessOrEqualThan();
}