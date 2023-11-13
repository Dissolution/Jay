using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jay.ConsoleApp;

public sealed class LoggingService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _appLifetime;

    public LoggingService(
        ILogger logger,
        IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _appLifetime = appLifetime;
    }

    private void OnStarted()
    {
        _logger.LogDebug("Application Started");
    }
    
    private void OnStopping()
    {
        _logger.LogDebug("Application Stopping");
    }

    private void OnStopped()
    {
        _logger.LogDebug("Application Stopped");
    }

    public Task StartAsync(CancellationToken token)
    {
        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }
}