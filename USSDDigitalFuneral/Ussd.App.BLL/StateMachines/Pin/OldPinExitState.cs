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

namespace Ussd.App.BLL.StateMachines.Pin
{
    public class OldPinExitState : MenuState
    {

        public OldPinExitState(MenuState previousState) : base(previousState)
        {

        }

        public OldPinExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new OldPinEntryState(this)
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

                menuState = new ChooseServiceEntryState(this);
            }
            else
            {




                if (!Validation.ValidateFourDigits(request.Message))
                {
                    menuState.ErrorMessage = CurrentLanguage.PinRequirementMessage;
                }
                else
                {

                    var service = ServiceProvider.GetService(typeof(IAuthenticationService)) as IAuthenticationService;


                    var processResponse = await service.ValidateCustomer(new Models.AuthenticationDto { PhoneNumber = request.Msisdn, Pin = request.Message });



                    try
                    {
                        ValueStash["NewPin"] = request.Message;
                        var responseResult = processResponse.Result as AuthenticationResponseDto;

                        if (processResponse.Message.Equals("ChangePin") || !processResponse.IsErrorOccurred)
                        {


                            ValueStash["FirstName"] = responseResult.CustomerName.GetFirstName();
                           
                            menuState = new EnterPinInitialEntryState(this);

                        }


                        else
                        {

                            menuState.ErrorMessage = CurrentLanguage.PinIncorrect;
                        }
                    }
                    catch (Exception Ex)
                    {

                        menuState = new GeneralErrorEntryState(this);
                    }


                    
                   
                   
                }








            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
