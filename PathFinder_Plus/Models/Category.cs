using System.Text.Json.Serialization;

namespace PathFinder_Plus.Models
{
    public class Category
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("children")]
        public Dictionary<string, object> Children { get; set; }
    }
}
