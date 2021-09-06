using System;

namespace Jay.Reflection
{
    public interface IProxyFactory
    {
        Type SourceType { get; }
        Type DestType { get; }

        Type CreateProxyType();
    }
}