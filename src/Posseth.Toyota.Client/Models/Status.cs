using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Posseth.Toyota.Client.Models
{
    public class Status
    {
        [JsonPropertyName("messages")]
        public List<Message>? Messages { get; set; }

        // Add other properties from the JSON if they exist, for example:
        [JsonPropertyName("statusCode")]
        public string? StatusCode { get; set; }

        [JsonPropertyName("request")]
        public string? Request { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
}