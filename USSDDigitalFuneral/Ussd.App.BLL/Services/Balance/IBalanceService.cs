using AngleDimension.Standard.Core.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Services.Balance
{
    public interface IBalanceService
    {
        Task<ProcessResponse> GetCustomerBalance(string policyNumber , string phoneNumber);
    }
}
    