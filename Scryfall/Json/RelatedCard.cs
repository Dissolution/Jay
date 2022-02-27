using System.Text.Json.Serialization;

namespace Scryfall.Json;

public class RelatedCard : ScryfallObject
{
    [JsonPropertyName("component")]
    public string Component { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("type_line")]
    public string TypeLine { get; set; }
}