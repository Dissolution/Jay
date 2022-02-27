using System.Text.Json.Serialization;

namespace Scryfall.Json;

public static class ScryfallOptions
{
    public static JsonSerializerOptions JsonSerializer { get; }
    
    static ScryfallOptions()
    {
        JsonSerializer = new()
        {
            WriteIndented = true,
            Converters =
            {
                new ColorsConverter(),
                new JsonStringEnumConverter(),
            },
        };
    }
}