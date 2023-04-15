using AngleDimension.Standard.Core.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Models;

namespace Ussd.App.BLL.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<ProcessResponse> ValidateCustomer(AuthenticationDto authenticationDto);
        Task<ProcessResponse> ChangePin(ChangePinDto changePinDto);
        Task<ProcessResponse> CheckPhoneNumber(CheckNumberDto checkNumberDto);
        Task<ProcessResponse> RecoverPin(RecoverPinDto recoverPinDto);
    }
}
        