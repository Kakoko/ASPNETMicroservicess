using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Extensions;
using AngleDimension.Standard.Core.General;
using MessageAgent.DataModel.DTOs.TNM;
using System.Text;
using Ussd.App.BLL.Core;

namespace Ussd.App.BLL.StateMachines.UnderConstruction
{
    public class ErrorStateEntryState : MenuState
    {

        public ErrorStateEntryState(MenuState previousState) : base(previousState)
        {
           
        }

        public ErrorStateEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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


         

            menuBuilder.AppendNewLine(CurrentLanguage.ServicesDown);
            menuBuilder.AppendNewLine(CurrentLanguage.PleaseTryAgainLater);



            menuBuilder.AppendNewLine();
            menuBuilder.AppendNewLine(CurrentLanguage.Home);


            var processResponse = new ProcessResponse
            {
                IsErrorOccurred = false
            };

            if (processResponse.IsErrorOccurred)
            {
                menuBuilder.AppendNewLine(processResponse.Message);
            }
            
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

            Handle.CurrentState = new ErrorStateExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
