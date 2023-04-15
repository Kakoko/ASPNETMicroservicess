using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.KnownValues;
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
using Ussd.App.BLL.StateMachines.Pin.ForgotPin;
using Ussd.App.BLL.StateMachines.Pin.ForgotPin.MainMenu;
using Ussd.App.BLL.StateMachines.Pin.InitialPinChange;
using Ussd.App.BLL.StateMachines.ServiceType;

namespace Ussd.App.BLL.StateMachines.Pin.ConfirmPin
{
    public class PinChangeExitState : MenuState
    {

        public PinChangeExitState(MenuState previousState) : base(previousState)
        {

        }

        public PinChangeExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new PinChangeEntryState(this)
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

            else if (request.Message.Equals(MenuConstants.NextPage))
            {

                menuState = new ForgetPinEntryState(this);
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
                        ValueStash["OldPin"] = request.Message;
                        var responseResult = processResponse.Result as AuthenticationResponseDto;

                        if (processResponse.Message.Equals("ChangePin") || !processResponse.IsErrorOccurred)
                        {


                            ValueStash["CurrentPinMessage"] = "Enter new PIN";

                            menuState = new EnterPinInitialEntryState(this);

                        }

                        else
                        {

                            menuState.ErrorMessage = CurrentLanguage.PinIncorrect;
                        }
                    }
                    catch (Exception Ex)
                    {

                        throw Ex;
                    }





                }








            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
