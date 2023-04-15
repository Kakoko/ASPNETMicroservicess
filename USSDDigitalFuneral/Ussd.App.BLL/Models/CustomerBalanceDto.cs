using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class CustomerBalanceDto
    {   
  

        public string PolicyNumber { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal Balance { get; set; } = 0;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal Balance1 { get; set; } = 0;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal AvailableBalance { get; set; } = 0;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal Balance2 { get; set; } = 0;

        public DateTime LastUpdated { get; set; }

    }
}
