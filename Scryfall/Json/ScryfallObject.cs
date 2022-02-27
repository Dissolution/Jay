using System.Text.Json.Serialization;

namespace Scryfall.Json;

public class ScryfallObject : IEquatable<ScryfallObject>
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("object")]
    public string ContentType { get; set; }
    
    [JsonPropertyName("uri")]
    public Uri Uri { get; set; }

    public bool Equals(ScryfallObject? obj)
    {
        return obj != null && obj.Id == this.Id;
    }
}