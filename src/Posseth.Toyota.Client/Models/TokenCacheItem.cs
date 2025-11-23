using System;

namespace Posseth.Toyota.Client.Models
{
    public class TokenCacheItem
    {
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }
        public string? uuid { get; set; }
        public DateTime expiration { get; set; }
        public string? username { get; set; }
    }
}