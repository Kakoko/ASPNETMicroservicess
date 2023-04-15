using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Extensions;
using AngleDimension.Standard.Core.General;
using MessageAgent.DataModel.DTOs.TNM;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.StateMachines.Pin;
using Ussd.App.BLL.Services.Data;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.Helper;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Ussd.App.BLL.StateMachines.Policies
{
    public class PolicyEntryState : MenuState
    {

        public PolicyEntryState(MenuState previousState) : base(previousState)
        {

        }

        public PolicyEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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


            var customerInformation = ValueStash["CustomerInformation"] as AuthenticationResponseDto;



            if (ValueStash.TryGetValue("CustomerInformationBack", out object customerResponse))
            {
                var customerInformationFromNextState = customerResponse.ToString();
                


                if (customerInformationFromNextState != null)
                {
                    customerInformation = JsonConvert.DeserializeObject<AuthenticationResponseDto>(customerInformationFromNextState);
                }
                
            }
           

            menuBuilder.AppendNewLine(string.Format(CurrentLanguage.PolicyMainMessage! , customerInformation!.CustomerName.GetFirstName()));
            menuBuilder.AppendLine();



           

            int menuNumber = 1;

            if (customerInformation is not null)
            {
                foreach (var customerPolicy in customerInformation.CustomerPolicies)
                {
                    menuBuilder.AppendNewLine($"{menuNumber}.{customerPolicy.PolicyName}");
                    Interlocked.Increment(ref menuNumber);
                }
                ValueStash["policyTypes"] = customerInformation.CustomerPolicies;
               
                
                ValueStash["CustomerFirstName"] = customerInformation.CustomerName.GetFirstName();
                ValueStash["CustomerInformationBack"] = customerInformation;

            }

           




            menuBuilder.AppendNewLine();
            menuBuilder.AppendNewLine(CurrentLanguage.Home);

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
            Handle.CurrentState = new PolicyExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
