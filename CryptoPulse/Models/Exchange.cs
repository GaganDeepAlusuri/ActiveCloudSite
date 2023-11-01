using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoPulse.Models
{
    public class Exchange
    {
        [Key]
        public int ExchangeID { get; set; }
        public string Name { get; set; }
        public decimal? VolumeUSD { get; set; }
        public int ActivePairs { get; set; }
        public string URL { get; set; }
        public string Country { get; set; }

       
        public List<Market> Markets { get; set; }
    }

}
