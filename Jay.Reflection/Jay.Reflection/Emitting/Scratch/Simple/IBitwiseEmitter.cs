namespace Jay.Reflection.Emitting.Scratch;

public interface IBitwiseEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self And();
    Self Negate();
    Self Not();
    Self Or();
    Self ShiftLeft();
    Self ShiftRight(bool unsigned = false);
    Self Xor();
}