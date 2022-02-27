using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Serialization;
using Scryfall.Json;

namespace Scryfall.Data;

public class ScryfallData
{
    public static async Task<ScryfallData> LoadLatest(CancellationToken token = default)
    {
        // var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)
        //                                  .Parent.Parent.FullName, "Data");
        var path = @"C:\git\Jay\Scryfall\Data";
        if (!Directory.Exists(path))
            throw new NotImplementedException();
        var filePath = Directory.EnumerateFiles(path, "all-cards-*.json.gz")
                                .OrderByDescending(x => x, StringComparer.OrdinalIgnoreCase)
                                .FirstOrDefault();
        if (filePath is null)
            throw new NotImplementedException();
        await using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        await using (var zipStream = new GZipStream(fileStream, CompressionMode.Decompress, true))
        {
            await foreach (var card in JsonSerializer.DeserializeAsyncEnumerable<Card>(zipStream, 
                                                                                       ScryfallOptions.JsonSerializer, 
                                                                                       token))
            {
                Debugger.Break();
            }
        }

        throw new NotImplementedException();
    }
}