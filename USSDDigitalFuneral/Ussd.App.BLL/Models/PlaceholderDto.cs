using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class PlaceholderDto
    {
        public required string Identifier { get; set; }
        public required string Value { get; set; }
    }
}
