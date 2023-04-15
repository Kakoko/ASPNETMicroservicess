using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Extensions;
using MessageAgent.DataModel.DTOs.TNM;
using System.Text;
using Ussd.App.BLL.Core;

namespace Ussd.App.BLL.StateMachines.Pin
{
    public class AuthenticateEntryState : MenuState
    {

        public AuthenticateEntryState(MenuState previousState) : base(previousState)
        {

        }

        public AuthenticateEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
            : base(handle, language, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var menuBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
               
                menuBuilder.AppendNewLine(ErrorMessage);
             
            }


           

         
            menuBuilder.AppendNewLine(CurrentLanguage.EnterPinMessage);
            menuBuilder.AppendLine();

            menuBuilder.AppendNewLine();
            menuBuilder.AppendNewLine(CurrentLanguage.Back);
          
            var response = new UssdResponseDTO
            {
                Message = menuBuilder.ToString(),
                Premium = new Premium
                {
                    Cost = 0,
                    Reference = Guid.NewGuid().ToString()
                },
                Type = SessionType.ToInt().ToString()
            };
            Handle.CurrentState = new AuthenticateExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
