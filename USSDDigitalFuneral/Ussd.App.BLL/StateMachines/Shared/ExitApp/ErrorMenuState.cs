using AngleDimension.Mno.DataModels;
using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.NetCore.Ussd.Extensions;
using AngleDimension.NetCore.Ussd.KnownValues;
using MessageAgent.DataModel.DTOs.TNM;
using System.Text;

using AngleDimension.NetCore.Ussd.Core;
using Ussd.App.BLL.Core;

namespace Ussd.App.BLL.StateMachines.Shared.ExitApp
{
    public class ErrorMenuState : MenuState
    {
        
        public ErrorMenuState(MenuState previousState) : base(previousState)
        {
            SessionType = SessionType.End;
        }

        public ErrorMenuState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash) 
            : base(handle, currentLanguage, valueStash)
        {
            SessionType = SessionType.End;
        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var menuBuilder = new StringBuilder();
            
            menuBuilder.AppendNewLine(ValueStash["MaximumPinEntry"].ToString());
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
