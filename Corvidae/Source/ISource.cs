using System;

namespace Corvidae
{
    public interface ISource : IDisposable
    {
        IReadOnlyData Data { get; }

        TValue? Get<TValue>();
    }
}