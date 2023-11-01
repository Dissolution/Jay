namespace Jay.Reflection.Emitting.Scratch.Simple;

public interface IMemoryEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self CopyBlock();
    Self InitBlock();
    Self LocalAlloc();
}