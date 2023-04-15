using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class AuthenticationResponseDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null;
       public string PhoneNumber { get; set; } = null;
    
        public bool? IsBlocked { get; set; }
        public List<CustomerPolicyDto> CustomerPolicies { get; set; } = new List<CustomerPolicyDto>();
    }
}
