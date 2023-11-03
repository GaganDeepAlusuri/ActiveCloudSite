using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoPulse.Models
{
    public class Coin
    {
        [Key]
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("price_usd")]
        public decimal? PriceUSD { get; set; }

        [JsonProperty("market_cap_usd")]
        public decimal? MarketCapUSD { get; set; }

        [JsonProperty("volume24")]
        public decimal? Volume24h { get; set; }

        [JsonProperty("csupply")]
        public decimal? SupplyCurrent { get; set; }

        [JsonProperty("tsupply")]
        public decimal? SupplyTotal { get; set; }

        [JsonProperty("msupply")]
        public decimal? SupplyMax { get; set; } // Use nullable decimal?

        [JsonProperty("percent_change_1h")]
        public decimal? PercentChange1h { get; set; }

        [JsonProperty("percent_change_24h")]
        public decimal? PercentChange24h { get; set; }

        [JsonProperty("percent_change_7d")]
        public decimal? PercentChange7d { get; set; }

        // Existing Relationship (One-to-Many with MarketDto)
        public List<Market> Markets { get; set; }
    }

    public class CoinWatchList
    {
        [Key]
        public int ID { get; set; }

        public string Symbol { get; set; }

        public string Name { get; set; }

        public int Rank { get; set; }

        public decimal? PriceUSD { get; set; }

        public decimal? MarketCapUSD { get; set; }

        public decimal? Volume24h { get; set; }

        public decimal? SupplyCurrent { get; set; }

        public decimal? SupplyTotal { get; set; }

        public decimal? SupplyMax { get; set; }

        public decimal? PercentChange1h { get; set; }

        public decimal? PercentChange24h { get; set; }

        public decimal? PercentChange7d { get; set; }

        // Existing Relationship (One-to-Many with MarketDto)
        public List<Market> Markets { get; set; }
    }

}
