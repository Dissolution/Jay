using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Wrap;

namespace Scryfall;


public static class ScryfallServices
{
    
    public static IServiceCollection RegisterServices(IServiceCollection services)
    {
        services.AddHttpClient("Scryfall", httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://api.scryfall.com/");
        });
        services.AddHttpClient("Scryfall_c1", httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://c1.scryfall.com/");
        });
        services.AddHttpClient("Scryfall_c2", httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://c2.scryfall.com/");
        });
        services.AddHttpClient("Scryfall_c3", httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://c3.scryfall.com/");
        });
        return services;
    }
}