using System.Text.Json.Serialization;

namespace Scryfall.Json;

public class CardFace
{
    [JsonPropertyName("artist")]
    public string? Artist { get; set; }
    
    [JsonPropertyName("cmc")]
    public decimal? ManaValue { get; set; }
    
    [JsonPropertyName("color_indicator")]
    public Colors? ColorIndicator { get; set; }
    
    [JsonPropertyName("colors")]
    public Colors? Colors { get; set; }
    
    [JsonPropertyName("flavor_text")]
    public string? FlavorText { get; set; }
    
    [JsonPropertyName("illustration_id")]
    public Guid? IllustrationId { get; set; }
    
    [JsonPropertyName("image_uris")]
    public Uri[]? ImageUris { get; set; }
    
    [JsonPropertyName("layout")]
    public string? Layout { get; set; }
    
    [JsonPropertyName("loyalty")]
    public string? Loyalty { get; set; }
    
    [JsonPropertyName("mana_cost")]
    public string ManaCost { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("oracle_id")]
    public Guid? OracleId { get; set; }
    
    [JsonPropertyName("oracle_text")]
    public string? OracleText { get; set; }
    
    [JsonPropertyName("power")]
    public string? Power { get; set; }
    
    [JsonPropertyName("printed_name")]
    public string? PrintedName { get; set; }
    
    [JsonPropertyName("printed_text")]
    public string? PrintedText { get; set; }
    
    [JsonPropertyName("printed_type_line")]
    public string? PrintedTypeLine { get; set; }
    
    [JsonPropertyName("toughness")]
    public string? Toughness { get; set; }
    
    [JsonPropertyName("type_line")]
    public string? TypeLine { get; set; }
    
    [JsonPropertyName("watermark")]
    public string? Watermark { get; set; }
}