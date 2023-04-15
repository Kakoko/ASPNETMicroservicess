using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class ProductDto
    {
        public required int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string ProductDescription { get; set; }
        public required bool IsGeneralEnquiry { get; set; }
        public required bool IsActive { get; set; }
     
    }
}   
            