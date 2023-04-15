using AngleDimension.Mno.DataModels;
using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.NetCore.Ussd.Extensions;
using MessageAgent.DataModel.DTOs.TNM;
using System.Text;
using AngleDimension.NetCore.Ussd.Core;
using Ussd.App.BLL.Core;

namespace Ussd.App.BLL.StateMachines.Shared.ExitApp
{
    public class ExitMenuState : MenuState
    {

        public ExitMenuState(MenuState previousState) : base(previousState)
        {
            SessionType = SessionType.End;
        }

        public ExitMenuState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash) 
            : base(handle, currentLanguage, valueStash)
        {
            SessionType = AngleDimension.NetCore.Ussd.Core.SessionType.End;
        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var menuBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
                menuBuilder.AppendNewLine(ErrorMessage);
            }
            menuBuilder.AppendNewLine(CurrentLanguage.ExitServiceMessage);
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
            Handle.CurrentState = this;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return response;
        }
    }
}
