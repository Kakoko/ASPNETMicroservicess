using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class EmailDto
    {
     

        public string Subject { get; set; } = null!;
        public string FullName { get; set; } = null!;   
        public string PhoneNumber { get; set; } = null!;
        public string Enquiry { get; set; } = null!;
        public string Message { get; set; } = null!;    
        public string Flag { get; set; } = null!;


    }   
}
