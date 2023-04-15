using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.KnownValues;
using MessageAgent.DataModel.DTOs.TNM;

using AngleDimension.NetCore.Ussd.Core;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.StateMachines.Shared.ExitApp;

namespace Ussd.App.BLL.StateMachines.Shared.Success
{
    public class SuccessExitState : MenuState
    {

        public SuccessExitState(MenuState previousState) : base(previousState)
        {
            
        }

        public SuccessExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {
            
        }

        public override ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            MenuState menuState;

            var errorState = new SuccessEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            menuState = errorState;

            switch (request.Message)
            {
                case MenuConstants.SignOut:
                    menuState = new ExitMenuState(this);
                    break;
                case MenuConstants.GoHome:
                    var actionRequest = ValueStash[AppKeyConstants.ServiceProviderTypeCode].ToString();
                    switch (actionRequest)
                    {
                        case "E":
                            break;
                        case "W":
                            break;
                        case "A":
                            break;
                        case "CL":
                            break;
                    }
                    break;
            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return menuState.ProccessRequest(request);
        }
    }
}
