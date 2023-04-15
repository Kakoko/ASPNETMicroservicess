using AngleDimension.Mno.DataModels;
using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Extensions;
using AngleDimension.Standard.Core.General;
using MessageAgent.DataModel.DTOs.TNM;
using System.Text;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.Services.Authentication;
using Ussd.App.BLL.Services.Data;
using Ussd.App.BLL.StateMachines.Error;

namespace Ussd.App.BLL.StateMachines.ServiceType
{
    public class ChooseServiceEntryState : MenuState
    {

        public ChooseServiceEntryState(MenuState previousState) : base(previousState)
        {

        }

        public ChooseServiceEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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


            var service = ServiceProvider.GetService(typeof(IAuthenticationService)) as IAuthenticationService;






            try
            {
               
                menuBuilder.AppendNewLine(CurrentLanguage.WelcomeMenu);
                menuBuilder.AppendNewLine(CurrentLanguage.WelcomeMenuSub);
                menuBuilder.AppendLine();


                var serviceProviderTypes = StaticDataStoreFactory.GetMainMenu();

                int menuNumber = 1;

                if (serviceProviderTypes is not null)
                {
                    foreach (var serviceProviderType in serviceProviderTypes)
                    {
                        menuBuilder.AppendNewLine($"{menuNumber}.{serviceProviderType.ProductName}");
                        Interlocked.Increment(ref menuNumber);
                    }

                    
                    ValueStash["ServiceProviderTypes"] = serviceProviderTypes;
                }



            }
            catch (Exception)
            {

                Handle.CurrentState = new GeneralErrorEntryState(this)
                {
                    ServiceProvider = ServiceProvider
                };
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
            Handle.CurrentState = new ChooseServiceExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
