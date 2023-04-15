using AngleDimension.Mno.DataModels;
using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.NetCore.Ussd.Extensions;
using AngleDimension.NetCore.Ussd.KnownValues;
using MessageAgent.DataModel.DTOs.TNM;
using System.Text;
using AngleDimension.NetCore.Ussd.Core;
using Ussd.App.BLL.Core;

namespace Ussd.App.BLL.StateMachines.Shared.Success
{
    public class SuccessEntryState : MenuState
    {
     

        public SuccessEntryState(MenuState previousState) : base(previousState)
        {
            
        }

        public SuccessEntryState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {
            
        }

        public override ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var menuBuilder = new StringBuilder();

            menuBuilder.AppendNewLine(ValueStash[DictionaryConstants.SuccessMessage].ToString());
            menuBuilder.AppendNewLine(CurrentLanguage.MenuActions);

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
            Handle.CurrentState = new SuccessExitState(this);
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return new ValueTask<UssdResponseDTO>(response);
        }
    }
}
