namespace Jay.Reflection.Emitting.Scratch.Simple;

public interface IDebugEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self Breakpoint();
    Self NoOperation();
}