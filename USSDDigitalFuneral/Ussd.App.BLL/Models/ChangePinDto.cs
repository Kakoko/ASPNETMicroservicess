﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Models
{
    public class ChangePinDto
    {
        public required string PhoneNumber { get; set; }
        public required string OldPin { get; set; }
        public required string NewPin { get; set; }
    }
}