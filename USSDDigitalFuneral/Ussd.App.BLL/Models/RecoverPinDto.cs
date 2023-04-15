using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class RecoverPinDto
    {
        public required string PhoneNumber { get; set; }
        public required string PartPolicyNumber { get; set; }
    }
}
