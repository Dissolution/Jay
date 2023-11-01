namespace Jay.Reflection.Emitting.Scratch.Simple;

public interface IBreakEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self IfEqual(EmitterLabel label);
    Self IfEqual(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    Self IfNotEqual(EmitterLabel label, bool unsigned = false);
    Self IfNotEqual(out EmitterLabel label, bool unsigned = false, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    Self IfGreaterThan(EmitterLabel label, bool unsigned = false);
    Self IfGreaterThan(out EmitterLabel label, bool unsigned = false, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    Self IfGreaterOrEqualThan(EmitterLabel label, bool unsigned = false);
    Self IfGreaterOrEqualThan(out EmitterLabel label, bool unsigned = false, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    Self IfLessThan(EmitterLabel label, bool unsigned = false);
    Self IfLessThan(out EmitterLabel label, bool unsigned = false, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    Self IfLessOrEqualThan(EmitterLabel label, bool unsigned = false);
    Self IfLessOrEqualThan(out EmitterLabel label, bool unsigned = false, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    Self If(bool b, EmitterLabel label);
    Self If(bool b, out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    Self To(EmitterLabel label);
    Self To(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    Self JumpTo(MethodInfo method);
    Self LeaveTo(EmitterLabel label);
    Self LeaveTo(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
}