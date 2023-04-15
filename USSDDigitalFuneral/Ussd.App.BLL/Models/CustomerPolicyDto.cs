using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class CustomerPolicyDto
    {
        public int PolicyId { get; set; }
        public required string PolicyNumber { get; set; }
        public required string PolicyName { get; set; }
        public required int ProductId { get; set; }
    }
}
    