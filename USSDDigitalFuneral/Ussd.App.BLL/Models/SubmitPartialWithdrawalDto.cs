using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class SubmitPartialWithdrawalDto
    {
        public string PhoneNumber { get; set; } = null!;
        public string PolicyNumber { get; set; } = null!;
        public int ProductId { get; set; }
        public decimal Balance { get; set; }
        public decimal Balance1 { get; set; }
        public decimal Balance2 { get; set; }
    }
}
