using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoPulse.Models
{
    public class Exchange
    {
        [Key]
        [JsonProperty("id")]
        public string ExchangeID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_id")]
        public string NameId { get; set; }

        [JsonProperty("volume_usd")]
        public double VolumeUSD { get; set; }

        [JsonProperty("active_pairs")]
        public int ActivePairs { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        public List<Market> Markets { get; set; }
    }
}