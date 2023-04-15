using AngleDimension.Mno.DataModels;
using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.NetCore.Ussd.Extensions;
using MessageAgent.DataModel.DTOs.TNM;
using System.Text;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Localization;
using Ussd.App.BLL.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ussd.App.BLL.StateMachines.Shared.Language
{
    public class ChangeLanguageEntryState : MenuState
    {

        public ChangeLanguageEntryState(MenuState previousState) : base(previousState)
        {
            
        }

        public ChangeLanguageEntryState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {
            
        }

        public override ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var menuBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
                menuBuilder.AppendNewLine(ErrorMessage);
            }
            menuBuilder.AppendNewLine(CurrentLanguage.LanguageTitle);

            //get language service
            var languageService = ServiceProvider.GetRequiredService<ILanguageService<LanguageDto>>();
            var supportedLanguages = languageService.GetSupportedLanguages();

            foreach (var language in supportedLanguages)
            {
                menuBuilder.AppendNewLine($"{language.LanguageId}.{language.LanguageName}");
            }
            menuBuilder.AppendNewLine(CurrentLanguage.Cancel);
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
            Handle.CurrentState = new ChangeLanguageExitState(this);
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return new ValueTask<UssdResponseDTO>(response);
        }
    }
}
