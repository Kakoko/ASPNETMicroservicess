using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class SmsResponseDto
    {
        public required string ReferenceId { get; set; }
        public required string MessageId { get; set; }
    }
}
    