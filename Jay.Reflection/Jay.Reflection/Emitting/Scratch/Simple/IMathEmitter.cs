namespace Jay.Reflection.Emitting.Scratch;

public interface IMathEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self Add(bool overflowCheck = false, bool unsigned = false);
    Self Divide(bool unsigned = false);
    Self Multiply(bool overflowCheck = false, bool unsigned = false);
    Self Modulo(bool unsigned = false);
    Self Subtract(bool overflowCheck = false, bool unsigned = false);
}