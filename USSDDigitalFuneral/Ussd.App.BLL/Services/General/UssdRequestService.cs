using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Abstractions;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.KnownValues;
using AngleDimension.NetCore.Ussd.Localization;
using AngleDimension.NetCore.Ussd.SessionManagement;
using MessageAgent.DataModel.DTOs.TNM;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.Shared.ExitApp;

namespace Ussd.App.BLL.Services.Genenal
{
    public class UssdRequestService : IUssdRequestService
    {
        private readonly IServiceProvider _serviceProvider;

        public UssdRequestService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async ValueTask<UssdResponseDTO> GetResponse(UssdRequestDTO request)
        {
            UssdMenu<LanguageDto> menu;
            var valueStash = new Dictionary<string, object>();
            var assembly = Assembly.GetAssembly(typeof(UssdRequestService));
           

          
            var sessionManager = _serviceProvider.GetRequiredService<ISessionManager<LanguageDto>>();
            var sessionResult = await sessionManager.Get(request.SessionId, assembly);

           
            var languageService = _serviceProvider.GetRequiredService<ILanguageService<LanguageDto>>();

            if (sessionResult.IsErrorOccurred)
            {
                
                var currentLanguage = await languageService.GetLanguage("en");

                valueStash.Add(DictionaryConstants.ErrorMessage, sessionResult.Message);
                menu = new UssdMenu<LanguageDto>(typeof(ErrorMenuState), currentLanguage, valueStash);
            }
            else
            {
                var state = sessionResult.Result;
                if (state == null) 
                {
                 

                    
                    var currentLanguage = await languageService.GetLanguage("en");
                    valueStash.Add(DictionaryConstants.LanguageCode, "en");

                   
                    valueStash.Remove(DictionaryConstants.AccessToken);
                    valueStash.Add(DictionaryConstants.AccessToken, "test-access-token"); 

                    
                    menu = new UssdMenu<LanguageDto>(typeof(ChooseServiceEntryState), currentLanguage, valueStash);
                      
                }
                else 
                {
                    menu = new UssdMenu<LanguageDto>(state);
                }
            }

            menu.CurrentState.ServiceProvider = _serviceProvider;
            var response = await menu.HandleRequest(request);
            await sessionManager.AddOrUpdate(request.SessionId, menu.CurrentState);
            return response;
        }
    }
}
