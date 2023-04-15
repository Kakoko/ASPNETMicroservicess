using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.KnownValues;
using MessageAgent.DataModel.DTOs.TNM;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.StateMachines.Pin.ConfirmPin;
using Ussd.App.BLL.StateMachines.ServiceType;

namespace Ussd.App.BLL.StateMachines.Pin.InitialPinChange
{
    public class EnterPinInitialExitState : MenuState
    {

        public EnterPinInitialExitState(MenuState previousState) : base(previousState)
        {

        }

        public EnterPinInitialExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new EnterPinInitialEntryState(this)
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


                var restrictedPINs = new List<string>() { "1234", "0000", "1111", "2222", "3333", "4444", "5555", "6666", "7777", "8888", "9999" };
                var found = restrictedPINs.FirstOrDefault(u => u.Equals(request.Message));
                if (found != null)
                    menuState.ErrorMessage = CurrentLanguage.PasswordObvious;

                else if (!Validation.ValidateFourDigits(request.Message))
                {
                    menuState.ErrorMessage = CurrentLanguage.PinRequirementMessage;
                }
                else
                {

                    ValueStash["NewPin"] = request.Message;
                    menuState = new EnterPinConfirmEntryState(this);
                }








            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
