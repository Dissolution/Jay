#region SETUP

using System.Diagnostics;
using Jay.Dumping.Scratch;
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
#endregion
// WORK

var arg = 147;

string str = DumpHome.DumpToString(arg);

Debugger.Break();





#region TEARDOWN
lifetime.StopApplication();
await host.WaitForShutdownAsync();
return 0; // OK
#endregion