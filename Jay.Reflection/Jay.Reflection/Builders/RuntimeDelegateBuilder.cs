using System.Diagnostics;
using Jay.Debugging;
using Jay.Reflection.Emitting;
using Jay.Reflection.Info;


namespace Jay.Reflection.Builders;

public class RuntimeDelegateBuilder : ICodePart
{
    protected readonly DynamicMethod _dynamicMethod;
    protected readonly DelegateInfo _delegateInfo;
    protected readonly FluentGeneratorEmitter _emitter;

    public DynamicMethod DynamicMethod => _dynamicMethod;
    public DelegateInfo DelegateInfo => _delegateInfo;

    public FluentGeneratorEmitter Emitter => _emitter;
    
    public string Name => _delegateInfo.Name;
    public Type ReturnType => _delegateInfo.ReturnType;
    public ParameterInfo[] Parameters => _delegateInfo.Parameters;
    public Type[] ParameterTypes => _delegateInfo.ParameterTypes;
    public int ParameterCount => _delegateInfo.ParameterCount;
    public ParameterInfo? FirstParameter => _delegateInfo.Parameters.FirstOrDefault();
  
    internal RuntimeDelegateBuilder(DynamicMethod dynamicMethod, DelegateInfo delegateInfo)
    {
        _dynamicMethod = dynamicMethod;
        _delegateInfo = delegateInfo;
        _emitter = dynamicMethod.GetEmitter();
    }

    public Delegate CreateDelegate()
    {
        //string il = CodePart.ToDeclaration(this.Emitter);
        //Debugger.Break();
        return _dynamicMethod.CreateDelegate(_delegateInfo.DelegateType);
    }

    public void DeclareTo(CodeBuilder codeBuilder)
    {
        codeBuilder
            .Append("Building a ")
            .Append(_delegateInfo)
            .AppendLine(':')
            .Append(_emitter);
    }
}

public class RuntimeDelegateBuilder<TDelegate> : RuntimeDelegateBuilder
    where TDelegate : Delegate
{
    public RuntimeDelegateBuilder(DynamicMethod dynamicMethod)
        : base(dynamicMethod, DelegateInfo.For<TDelegate>()) { }

    public new TDelegate CreateDelegate()
    {
        string il = CodePart.ToDeclaration(this.Emitter);
        Hold.Onto(il);
        Debugger.Break();
        return _dynamicMethod.CreateDelegate<TDelegate>();
    }
}