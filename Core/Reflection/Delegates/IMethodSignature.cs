using System;
using System.Reflection;
using Jay.Reflection.Emission;

namespace Jay.Reflection.Delegates
{
    public interface IMethodSignature : IEquatable<IMethodSignature>
    {
        Type OwnerType { get; }
        string Name { get; }
        ArgumentType[] ArgumentTypes { get; }
        ParameterInfo[] Parameters { get; }
        Type[] ParameterTypes { get; }
        int ParameterCount { get; }
        Type ReturnType { get; }
    }
}