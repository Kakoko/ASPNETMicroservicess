using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class SmsDto
    {
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Type { get; set; }
        public required string CallToAction { get; set; }
        public required string Enquiry { get; set; }

    }   
}
        