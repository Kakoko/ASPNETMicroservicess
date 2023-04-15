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
using Ussd.App.BLL.StateMachines.Error;
using Ussd.App.BLL.StateMachines.Pin.ForgotPin.ForgetPinChange;
using Ussd.App.BLL.StateMachines.Pin.InitialPinChange;
using Ussd.App.BLL.StateMachines.Pin.SuccessfulPinChange;
using Ussd.App.BLL.StateMachines.ServiceType;

namespace Ussd.App.BLL.StateMachines.Pin.ForgotPin.MainMenu
{
    public class ForgetPinExitState : MenuState
    {

        public ForgetPinExitState(MenuState previousState) : base(previousState)
        {

        }

        public ForgetPinExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new ForgetPinEntryState(this)
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




                if (Validation.ValidateFourDigits(request.Message))
                {

                    var service = ServiceProvider.GetService(typeof(IAuthenticationService)) as IAuthenticationService;

                    try
                    {
                        var response = await service.RecoverPin(new RecoverPinDto { PartPolicyNumber = request.Message, PhoneNumber = request.Msisdn });

                        if(!response.IsErrorOccurred)
                        {
                            menuState = new ForgetPinChangeMessageEntryState(this);
                        }
                        else
                        {
                            menuState.ErrorMessage = "Pin reset failed";
                        }
                    }
                    catch (Exception)
                    {

                        menuState = new GeneralErrorEntryState(this);
                        
                    }
                 
                }
                else
                {

                    menuState = new GeneralErrorEntryState(this);
                }


            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
