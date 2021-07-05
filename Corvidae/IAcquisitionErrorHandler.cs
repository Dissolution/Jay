using System;
using Jay;
using Jay.Debugging.Dumping;
using Microsoft.Extensions.Logging;

namespace Corvidae
{
    public interface IAcquisitionErrorHandler : IDisposable
    {
        Result TryAcquire(ISource source,
                          Type valueType,
                          out object? value);

        Result TryAcquire<TValue>(ISource source,
                                  out TValue? value)
        {
            Result result = TryAcquire(source, typeof(TValue), out object? obj);
            if (result)
            {
                if (!obj.Is<TValue>(out value))
                {
                    result = new InvalidOperationException(Dumper.Format($"TryResolve(ISource, {typeof(TValue)}, out {value}) did not produce a {typeof(TValue)} value"));
                }
            }
            else
            {
                value = default;
            }
            return result;
        }
    }

    public sealed class LoggerAcquisitionErrorHandler : IAcquisitionErrorHandler
    {
        private readonly ILogger<ISource> _logger;

        public LoggerAcquisitionErrorHandler(ILogger<ISource> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <inheritdoc />
        public Result TryAcquire(ISource source, Type valueType, out object? value)
        {
            // We log and then fail
            _logger.LogError("{Source} could not acquire a {ValueType} value", source, valueType);
            value = default;
            return false;
        }
        
        /// <inheritdoc />
        public void Dispose()
        {
           // Do nothing
        }
    }
}