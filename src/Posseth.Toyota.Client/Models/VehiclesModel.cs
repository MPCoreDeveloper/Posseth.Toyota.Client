using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Posseth.Toyota.Client.Models
{
    public class VehiclesModel
    {
        [JsonPropertyName("status")]
        public Status? Status { get; set; }
        [JsonPropertyName("payload")]
        public List<Vehicle>? Payload { get; set; }
    }
}