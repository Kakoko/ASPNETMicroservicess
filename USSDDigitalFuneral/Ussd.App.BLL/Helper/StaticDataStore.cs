using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Helper
{
    public  class StaticDataStore
    {
        public  string? ProductName { get; set;}
        public  string? ProductCode { get; set; }
        public  string? ProductId { get; set; }
    }

}
