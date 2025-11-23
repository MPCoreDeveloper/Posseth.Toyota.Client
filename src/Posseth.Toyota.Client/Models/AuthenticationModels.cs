using System.Collections.Generic;

namespace Posseth.Toyota.Client.Models
{
    public class AuthenticationModel
    {
        public string? authId { get; set; }
        public string? template { get; set; }
        public string? stage { get; set; }
        public string? header { get; set; }
        public List<Callback>? callbacks { get; set; }
    }

    public class Callback
    {
        public string? type { get; set; }
        public List<Input>? input { get; set; }
        public List<Output>? output { get; set; }
        public int? _id { get; set; }
    }

    public class Input
    {
        public string? name { get; set; }
        public object? value { get; set; }
    }

    public class Output
    {
        public string? name { get; set; }
        public object? value { get; set; }
    }

    public class AuthenticationModel2
    {
        public string? tokenId { get; set; }
        public string? successUrl { get; set; }
        public string? realm { get; set; }
    }

    public class AccessTokenEndpointResponse
    {
        public string access_token { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public string id_token { get; set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
        public int expires_in { get; set; }
        public string scope { get; set; } = string.Empty;
    }
}