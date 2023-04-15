using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.KnownValues;
using AngleDimension.Standard.Core.Validations;
using MessageAgent.DataModel.DTOs.TNM;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.Services.Authentication;
using Ussd.App.BLL.Services.Notification;
using Ussd.App.BLL.Services.Products;
using Ussd.App.BLL.StateMachines.CallBack;
using Ussd.App.BLL.StateMachines.FullName;
using Ussd.App.BLL.StateMachines.Pin.InitialPinChange;
using Ussd.App.BLL.StateMachines.Pin.SuccessfulPinChange;
using Ussd.App.BLL.StateMachines.Policies;
using Ussd.App.BLL.StateMachines.Product;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.Withdraw.Policy;

namespace Ussd.App.BLL.StateMachines.Pin.ConfirmPin
{
    public class EnterPinConfirmExitState : MenuState
    {

        public EnterPinConfirmExitState(MenuState previousState) : base(previousState)
        {

        }

        public EnterPinConfirmExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new EnterPinConfirmEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;


            if (request.Message.Equals(MenuConstants.Zero))
            {
                menuState = new ChooseServiceEntryState(this);
            }
            else if (request.Message.Equals(MenuConstants.Nine))
            {

                menuState = new EnterPinInitialEntryState(this);
            }
            else
            {


                var service = ServiceProvider.GetService(typeof(IAuthenticationService)) as IAuthenticationService;



                var initialPin = ValueStash["NewPin"].ToString()!;
                var confirmPin = request.Message;
                var oldPin = ValueStash["OldPin"].ToString();



                var proceedToPortfolio = string.Empty;

                if (ValueStash.TryGetValue("ProceedToPortfolio", out object infoFromStash))
                {
                    proceedToPortfolio = Convert.ToString(infoFromStash);
                }


                bool validPin = Validation.ValidateFourDigits(request.Message);
                bool pinsMatch = initialPin.Equals(confirmPin);

                switch ((validPin, pinsMatch))
                {
                    case (true, true):

                        var processResponse = await service.ChangePin(new ChangePinDto { NewPin = confirmPin, OldPin = oldPin, PhoneNumber = request.Msisdn });
                        if(processResponse.IsErrorOccurred && Convert.ToBoolean(processResponse.Result) == Convert.ToBoolean(false))
                        {
                            menuState.ErrorMessage = CurrentLanguage.PasswordObvious;
                            menuState = new EnterPinInitialEntryState(this);
                        }
                        else if(proceedToPortfolio.Equals("portfolio"))
                        {
                            var processResponsePin = await service.ValidateCustomer(new AuthenticationDto { PhoneNumber = request.Msisdn, Pin = request.Message });
                            if (!processResponsePin.IsErrorOccurred)
                            {
                                ValueStash["CustomerInformation"] = processResponsePin.Result;
                                menuState = new PolicyEntryState(this);
                            }
                           
                        }
                        else if (proceedToPortfolio.Equals("withdraw"))
                        {
                            var processResponsePin = await service.ValidateCustomer(new AuthenticationDto { PhoneNumber = request.Msisdn, Pin = request.Message });
                            if (!processResponsePin.IsErrorOccurred)
                            {
                                ValueStash["CustomerInformation"] = processResponsePin.Result;
                                menuState = new WithdrawalPolicyEntryState(this);
                            }

                        }

                        else if (!processResponse.IsErrorOccurred)
                        {
                            menuState = new PinChangedSuccessfullyEntryState(this);
                        }
                        break;
                    case (false, false):
                        menuState.ErrorMessage = CurrentLanguage.PinNotMatchingPinInvalidMessage;
                        break;
                    case (false, true):
                        menuState.ErrorMessage = CurrentLanguage.PinRequirementMessage;
                        break;
                    case (true, false):
                        menuState.ErrorMessage = CurrentLanguage.PinNotMatching;
                        break;
                }





            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
