namespace Jay.Reflection.Emitting.Scratch;

public interface IStandardEmitter<TSelf> :
    IEmitter<TSelf>,
    IArgumentEmissions<TSelf>,
    IBitwiseEmissions<TSelf>,
    IDebuggingEmissions<TSelf>,
    IExceptionEmissions<TSelf>,
    ILabelsEmissions<TSelf>,
    ILocalsEmissions<TSelf>,
    IMathEmissions<TSelf>,
    IMethodEmissions<TSelf>,
    IOpCodeEmissions<TSelf>,
    IPrefixEmissions<TSelf>,
    IScopeEmissions<TSelf>,
    ITryCatchFinallyEmissions<TSelf>
    where TSelf : IStandardEmitter<TSelf>
{
    
}