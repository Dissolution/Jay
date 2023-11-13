using Jay.Concurrency;
using Jay.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

using var _ = OnlyApplication.Acquire();
var hostBuilder = new HostApplicationBuilder(args);
hostBuilder.Logging.ClearProviders();
hostBuilder.Services
    .AddHostedService<LoggingService>()
    .AddSerilog(new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
        .CreateLogger());
using var host = hostBuilder.Build();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var token = lifetime.ApplicationStopping;
await host.StartAsync(token);













await host.StopAsync(token);
return 0;