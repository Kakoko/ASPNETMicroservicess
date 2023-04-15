using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.KnownValues;
using MessageAgent.DataModel.DTOs.TNM;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.Services.Authentication;
using Ussd.App.BLL.StateMachines.Error;
using Ussd.App.BLL.StateMachines.Pin.InitialPinChange;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.Shared.ExitApp;
using Ussd.App.BLL.StateMachines.Withdraw.Policy;

namespace Ussd.App.BLL.StateMachines.Withdraw.Authenticate
{
    public class WithdrawalAuthenticateExitState : MenuState
    {

        public WithdrawalAuthenticateExitState(MenuState previousState) : base(previousState)
        {

        }

        public WithdrawalAuthenticateExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new WithdrawalAuthenticateEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;


            if (request.Message.Equals(MenuConstants.Nine))
            {

                menuState = new ChooseServiceEntryState(this);
            }
            else if (Validation.ValidateFourDigits(request.Message))
            {

                var service = ServiceProvider.GetService(typeof(IAuthenticationService)) as IAuthenticationService;



                try
                {
                    var processResponse = await service.ValidateCustomer(new AuthenticationDto { PhoneNumber = request.Msisdn, Pin = request.Message });




                    if (processResponse.IsErrorOccurred && processResponse.Message.Equals("ChangePin"))
                    {




                        var responseResult = processResponse.Result as AuthenticationResponseDto;
                        var firstName = responseResult.CustomerName.GetFirstName();
                        ValueStash["CurrentPinMessage"] = string.Format(CurrentLanguage.ActivatePinMessage, firstName);
                        ValueStash["OldPin"] = request.Message;
                        ValueStash["ProceedToPortfolio"] = "withdraw";

                        menuState = new EnterPinInitialEntryState(this);

                    }
                    else if (processResponse.IsErrorOccurred && processResponse.Message.Equals("WrongPin"))
                    {
                        int attempts = 0;
                        if (ValueStash.TryGetValue("PinAttempts", out object attemptsFromStash))
                        {
                            attempts = Convert.ToInt32(attemptsFromStash.ToString());
                        }
                        attempts++;

                        if (attempts >= 3)
                        {

                            ValueStash["MaximumPinEntry"] = "Maximum number of PIN attempts reached. Terminating session.";
                            menuState = new ErrorMenuState(this);
                        }
                        else
                        {

                            errorState.ErrorMessage = $"Invalid PIN code. {3 - attempts} attempt(s) remaining.";

                        }
                        ValueStash["PinAttempts"] = attempts;
                    }
                    else
                    {
                        ValueStash["CustomerInformation"] = processResponse.Result;
                        menuState = new WithdrawalPolicyEntryState(this);
                    }
                }
                catch (Exception Ex)
                {

                    menuState = new GeneralErrorEntryState(this);
                }





            }
            else
            {
                errorState.ErrorMessage = CurrentLanguage.PinRequirementMessage;
            }


            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
