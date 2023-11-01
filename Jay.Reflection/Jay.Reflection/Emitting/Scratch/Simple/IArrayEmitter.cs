namespace Jay.Reflection.Emitting.Scratch.Simple;

public interface IArrayEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self LoadLength();
    Self ReadonlyOp();
    
    Self GetItem<TItem>();
    Self GetItem(Type itemType);

    Self SetItem<TItem>();
    Self SetItem(Type itemType);

    Self New<TItem>();
    Self New(Type itemType);
}