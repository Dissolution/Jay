using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {

    })
    .ConfigureLogging(logging => logging.AddConsole())
    .Build();


await host.StartAsync();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var token = lifetime.ApplicationStopping;
var logger = host.Services.GetRequiredService<ILogger<Program>>();


// WORK





lifetime.StopApplication();
await host.WaitForShutdownAsync();
return 0; // OK