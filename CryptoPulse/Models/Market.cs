using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoPulse.Models
{
    public class Market
    {
        [Key]
        public int MarketID { get; set; }
        public string ExchangeName { get; set; }
        public string BaseCurrency { get; set; }
        public string QuoteCurrency { get; set; }
        public decimal? PriceUSD { get; set; }
        public decimal? VolumeUSD { get; set; }



    }
}
