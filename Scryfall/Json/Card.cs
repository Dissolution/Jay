using System.Drawing;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Scryfall.Json;

//https://scryfall.com/docs/api/cards
public class Card : ScryfallObject, IEquatable<Card>
{
    [JsonPropertyName("arena_id")]
    public int? ArenaId { get; set; }
  
    [JsonPropertyName("lang")]
    public string LanguageCode { get; set; }
    
    [JsonPropertyName("mtgo_id")]
    public int? MtgoId { get; set; }
    
    [JsonPropertyName("mtgo_foil_id")]
    public int? MtgoFoilId { get; set; }
    
    [JsonPropertyName("multiverse_ids")]
    public int[]? MultiverseIds { get; set; }
    
    [JsonPropertyName("tcgplayer_id")]
    public int? TcgPlayerProductId { get; set; }
    
    [JsonPropertyName("tcgplayer_etched_id")]
    public int? TcgPlayerEtchedProductId { get; set; }
    
    [JsonPropertyName("cardmarket_id")]
    public int? CardMarketProductId { get; set; }

    [JsonPropertyName("oracle_id")]
    public Guid OracleId { get; set; }
    
    [JsonPropertyName("prints_search_uri")]
    public Uri PrintsSearchUri { get; set; }
    
    [JsonPropertyName("rulings_uri")]
    public Uri RulingsUri { get; set; }
    
    [JsonPropertyName("scryfall_uri")]
    public Uri ScryfallUri { get; set; }

    #region Rules Related
    [JsonPropertyName("all_parts")]
    public RelatedCard[]? RelatedCards { get; set; }
    
    [JsonPropertyName("card_faces")]
    public CardFace[]? CardFaces { get; set; }
    
    [JsonPropertyName("cmc")]
    public decimal ManaValue { get; set; }
    
    [JsonPropertyName("color_identity")]
    public Colors ColorIdentity { get; set; }
    
    [JsonPropertyName("color_indicator")]
    public Colors? ColorIndicator { get; set; }
    
    [JsonPropertyName("colors")]
    public Colors? Colors { get; set; }
    
    [JsonPropertyName("edhrec_rank")]
    public int? EdhRecRank { get; set; }
    
    [JsonPropertyName("hand_modifier")]
    public string? HandModifier { get; set; }
    
    [JsonPropertyName("keywords")]
    public string[] Keywords { get; set; }
    
    [JsonPropertyName("layout")]
    public string Layout { get; set; }
    
    [JsonPropertyName("legalities")]
    public Dictionary<string, string> Legalities { get; set; }
    
    [JsonPropertyName("life_modifier")]
    public string? LifeModifier { get; set; }
    
    [JsonPropertyName("loyalty")]
    public string? Loyalty { get; set; }
    
    [JsonPropertyName("mana_cost")]
    public string? ManaCost { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("oracle_text")]
    public string? OracleText { get; set; }
    
    [JsonPropertyName("oversized")]
    public bool Oversized { get; set; }
    
    [JsonPropertyName("power")]
    public string? Power { get; set; }
    
    [JsonPropertyName("produced_mana")]
    public Colors? ProducedMana { get; set; }
    
    [JsonPropertyName("reserved")]
    public bool Reserved { get; set; }
    
    [JsonPropertyName("toughness")]
    public string? Toughness { get; set; }
    
    [JsonPropertyName("type_line")]
    public string TypeLine { get; set; }
    #endregion
    
    #region Unique Per Print
    [JsonPropertyName("artist")]
    public string? Artist { get; set; }
    
    [JsonPropertyName("booster")]
    public bool InBoosters { get; set; }
    
    [JsonPropertyName("border_color")]
    public string BorderColor { get; set; }
    
    [JsonPropertyName("card_back_id")]
    public Guid CardBackId { get; set; }
    
    [JsonPropertyName("collector_number")]
    public string CollectorNumber { get; set; }
    
    [JsonPropertyName("content_warning")]
    public bool? ContentWarning { get; set; }
    
    [JsonPropertyName("digital")]
    public bool IsDigital { get; set; }
    
    [JsonPropertyName("finishes")]
    public string[] Finishes { get; set; }
    
    [JsonPropertyName("flavor_name")]
    public string? FlavorName { get; set; }
    
    [JsonPropertyName("flavor_text")]
    public string? FlavorText { get; set; }
    
    [JsonPropertyName("frame_effects")]
    public string[]? FrameEffects { get; set; }
    
    [JsonPropertyName("frame")]
    public string Frame { get; set; }
    
    [JsonPropertyName("full_art")]
    public bool IsFullArt { get; set; }
    
    [JsonPropertyName("games")]
    public string[] Games { get; set; }
    
    [JsonPropertyName("highres_image")]
    public bool IsHighResImage { get; set; }
    
    [JsonPropertyName("illustration_id")]
    public Guid? IllustrationId { get; set; }
    
    [JsonPropertyName("image_status")]
    public string ImageStatus { get; set; }
    
    [JsonPropertyName("image_uris")]
    public Dictionary<string, Uri>? ImageUris { get; set; }
    
    [JsonPropertyName("prices")]
    public Dictionary<string, string> Prices { get; set; }
    
    [JsonPropertyName("printed_name")]
    public string? PrintedName { get; set; }
    
    [JsonPropertyName("printed_text")]
    public string? PrintedText { get; set; }
    
    [JsonPropertyName("printed_type_line")]
    public string? PrintedTypeLine { get; set; }
    
    [JsonPropertyName("promo")]
    public bool IsPromo { get; set; }
    
    [JsonPropertyName("promo_types")]
    public string[]? PromoTypes { get; set; }
    
    [JsonPropertyName("purchase_uris")]
    public Dictionary<string, Uri>? PurchaseUris { get; set; }
    
    [JsonPropertyName("rarity")]
    public Rarity Rarity { get; set; }
    
    [JsonPropertyName("related_uris")]
    public Dictionary<string, Uri> RelatedUris { get; set; }
    
    [JsonPropertyName("released_at")]
    public DateTime ReleasedAt { get; set; }
    
    [JsonPropertyName("reprint")]
    public bool IsReprint { get; set; }
    
    [JsonPropertyName("scryfall_set_uri")]
    public Uri ScryfallSetUri { get; set; }
    
    [JsonPropertyName("set_name")]
    public string SetName { get; set; }
    
    [JsonPropertyName("set_search_uri")]
    public Uri SetSearchUri { get; set; }
    
    [JsonPropertyName("set_type")]
    public string SetType { get; set; }
    
    [JsonPropertyName("set_uri")]
    public Uri SetUri { get; set; }
    
    [JsonPropertyName("set")]
    public string Set { get; set; }
    
    [JsonPropertyName("set_id")]
    public Guid SetId { get; set; }
    
    [JsonPropertyName("story_spotlight")]
    public bool IsStorySpotlight { get; set; }
    
    [JsonPropertyName("textless")]
    public bool IsTextless { get; set; }
    
    [JsonPropertyName("variation")]
    public bool IsVariation { get; set; }
    
    [JsonPropertyName("variation_of")]
    public Guid? VariationOf { get; set; }
    
    [JsonPropertyName("security_stamp")]
    public string? SecurityStamp { get; set; }
    
    [JsonPropertyName("watermark")]
    public string? Watermark { get; set; }
    
    [JsonPropertyName("preview.previewed_at")]
    public DateTime? PreviewDate { get; set; }
    
    [JsonPropertyName("preview.source_uri")]
    public Uri? PreviewUri { get; set; }
    
    [JsonPropertyName("preview.source")]
    public string? PreviewSource { get; set; }
    #endregion
    
    /// <inheritdoc />
    public bool Equals(Card? card)
    {
        return card != null && this.Id == card.Id;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Card card && this.Id == card.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}