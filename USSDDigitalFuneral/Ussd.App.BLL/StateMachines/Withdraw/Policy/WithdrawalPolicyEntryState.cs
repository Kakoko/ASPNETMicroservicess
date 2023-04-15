using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Extensions;
using MessageAgent.DataModel.DTOs.TNM;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.Services.Balance;
using Ussd.App.BLL.StateMachines.Balance;
using Newtonsoft.Json;
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.StateMachines.Policies;
using Ussd.App.BLL.Services.Products;

namespace Ussd.App.BLL.StateMachines.Withdraw.Policy
{
    public class WithdrawalPolicyEntryState : MenuState
    {

        public WithdrawalPolicyEntryState(MenuState previousState) : base(previousState)
        {

        }   

        public WithdrawalPolicyEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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


            var service = ServiceProvider.GetService(typeof(IProductsService)) as IProductsService;

            if (ValueStash.TryGetValue("CustomerInformationBack", out object customerResponse))
            {
                var customerInformationFromNextState = customerResponse.ToString();



                if (customerInformationFromNextState != null)
                {
                    customerInformation = JsonConvert.DeserializeObject<AuthenticationResponseDto>(customerInformationFromNextState);
                }

            }


            menuBuilder.AppendNewLine(CurrentLanguage.WithdrawMainMessage);
            menuBuilder.AppendLine();





            int menuNumber = 1;

            if (customerInformation is not null)
            {

                var filteredList = customerInformation.CustomerPolicies.Where(c => c.PolicyName == "Tsogolo Savings" || c.PolicyName == "Green Life").ToList();
              //  var filteredList = customerInformation.CustomerPolicies.Where(c => c. == "Tsogolo Savings" || c.PolicyName == "Green Life").ToList();



                foreach (var customerPolicy in filteredList)
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
            Handle.CurrentState = new WithdrawalPolicyExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
