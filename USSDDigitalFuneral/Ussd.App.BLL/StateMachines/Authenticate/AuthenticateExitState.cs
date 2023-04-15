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
using Ussd.App.BLL.StateMachines.Policies;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.Shared.ExitApp;

namespace Ussd.App.BLL.StateMachines.Pin
{
    public class AuthenticateExitState : MenuState
    {

        public AuthenticateExitState(MenuState previousState) : base(previousState)
        {

        }

        public AuthenticateExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new AuthenticateEntryState(this)
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
                        ValueStash["ProceedToPortfolio"] = "portfolio";
                       

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

                            errorState.ErrorMessage = $"Incorrect PIN code. {3 - attempts} attempt(s) remaining.";

                        }
                        ValueStash["PinAttempts"] = attempts;

                    }
                    else
                    {
                        ValueStash["CustomerInformation"] = processResponse.Result;
                        menuState = new PolicyEntryState(this);
                    }
                }
                catch (Exception Ex)
                {

                    Handle.CurrentState = new GeneralErrorEntryState(this)
                    {
                        ServiceProvider = ServiceProvider
                    };
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
