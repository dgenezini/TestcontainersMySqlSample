using System.Text.Json.Serialization;

namespace TestcontainersSample.DatabaseContext;

public class TodoItem
{
    [JsonPropertyName("itemId")]
    public int ItemId { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
