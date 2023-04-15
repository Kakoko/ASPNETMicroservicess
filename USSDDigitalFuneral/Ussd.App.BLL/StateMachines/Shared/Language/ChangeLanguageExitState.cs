using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.KnownValues;
using MessageAgent.DataModel.DTOs.TNM;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Localization;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.StateMachines.Shared.Success;
using Ussd.App.BLL.StateMachines.ServiceType;

namespace Ussd.App.BLL.StateMachines.Shared.Language
{
    public class ChangeLanguageExitState : MenuState
    {

        public ChangeLanguageExitState(MenuState previousState) : base(previousState)
        {
            
        }

        public ChangeLanguageExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {
            
        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            MenuState menuState;
            var errorState = new ChangeLanguageEntryState(this);
            errorState.ErrorMessage = CurrentLanguage.ErrorMessage;
            menuState = errorState;

            

            if (request.Message.Equals(MenuConstants.Cancel)) //provide valid navigation
            {
                menuState = new ChooseServiceEntryState(this);
            }
            else
            {
                if (int.TryParse(request.Message, out int value))
                {
                    //get language service
                    var languageService = ServiceProvider.GetService(typeof(ILanguageService<LanguageDto>)) as ILanguageService<LanguageDto>;
                    var supportedLanguages = languageService.GetSupportedLanguages();

                    var supportedLanguage = supportedLanguages.FirstOrDefault(x => x.LanguageId == value);
                    if (supportedLanguage is not null)
                    {
                        ValueStash.Remove(DictionaryConstants.LanguageCode);
                        ValueStash.Add(DictionaryConstants.LanguageCode, supportedLanguage.LanguageCode);

                        menuState = new SuccessEntryState(this);
                        menuState.CurrentLanguage = await languageService.GetLanguage(supportedLanguage.LanguageCode);
                        ValueStash.Remove(DictionaryConstants.SuccessMessage);
                        ValueStash.Add(DictionaryConstants.SuccessMessage, menuState.CurrentLanguage.LanguageChangedMessage);
                    }
                }
            }
            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
