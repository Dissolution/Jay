using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace Scryfall.Json;

public class ColorsConverter : JsonConverter<Colors>
{
    
    
    /// <inheritdoc />
    public override Colors Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(Colors));
        Debug.Assert(reader.TokenType == JsonTokenType.StartArray);
        var colors = Colors.Colorless;
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            string color = Encoding.UTF8.GetString(reader.ValueSpan);
            if (color == "W")
                colors |= Colors.White;
            else if (color == "U")
                colors |= Colors.Blue;
            else if (color == "B")
                colors |= Colors.Black;
            else if (color == "R")
                colors |= Colors.Red;
            else if (color == "G")
                colors |= Colors.Green;
            else
            {
                throw new NotImplementedException();
            }
        }

        return colors;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Colors value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}