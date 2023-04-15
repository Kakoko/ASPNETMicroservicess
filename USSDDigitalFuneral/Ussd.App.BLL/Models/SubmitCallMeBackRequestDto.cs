using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class SubmitCallMeBackRequestDto
    {
        public required string CustomerName { get; set; } 
        public required string PhoneNumber { get; set; }
        public required int ProductId { get; set; }
    }
}
