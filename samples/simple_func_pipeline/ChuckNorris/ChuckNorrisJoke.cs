using System.Text.Json.Serialization;

namespace simple_func_pipeline.ChuckNorris;


public class ChuckNorrisJoke
{
    [JsonPropertyName("id")] public string Id { get; set; } = null!;
    [JsonPropertyName("value")] public string Value { get; set; } = null!;
    [JsonPropertyName("url")] public string Url { get; set; } = null!;
    [JsonPropertyName("categories")] public List<string> Categories { get; set; } = new();
}
