using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.KnownValues;
using MessageAgent.DataModel.DTOs.TNM;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.StateMachines.Balance;
using Ussd.App.BLL.StateMachines.Balance.GreenLife;
using Ussd.App.BLL.StateMachines.Balance.Pension;
using Ussd.App.BLL.StateMachines.Balance.Tsogolo;
using Ussd.App.BLL.StateMachines.CallBack;
using Ussd.App.BLL.StateMachines.FullName;
using Ussd.App.BLL.StateMachines.GeneralEnquiry;
using Ussd.App.BLL.StateMachines.Pin;
using Ussd.App.BLL.StateMachines.Product;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.UnderConstruction;

namespace Ussd.App.BLL.StateMachines.Policies
{
    public class PolicyExitState : MenuState
    {

        public PolicyExitState(MenuState previousState) : base(previousState)
        {

        }

        public PolicyExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new PolicyEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };

            MenuState menuState = errorState;


            if (request.Message.Equals(MenuConstants.Zero))
            {

                menuState = new ChooseServiceEntryState(this);
            }
            else
            {
               var array = ValueStash["policyTypes"];

                var serviceProviderTypes = JsonSerializer.Deserialize<IEnumerable<CustomerPolicyDto>>(array.ToString());

                if (int.TryParse(request.Message, out int option) && option > 0 && option <= serviceProviderTypes.Count())
                {
                    var serviceProviderType = serviceProviderTypes.ElementAt(option - 1);

                    ValueStash["PolicyNumber"] = serviceProviderType.PolicyNumber;
                    ValueStash["PolicyName"] = serviceProviderType.PolicyName;


                    menuState = serviceProviderType.PolicyName switch
                    {
                        "Tsogolo Savings" => new TsogoloBalanceEntryState(this),
                        "Green Life" => new GreenLifeBalanceEntryState(this),
                        "Pension" => new PensionBalanceEntryState(this),
                        _ => new ErrorStateEntryState(this)
                    };
                   


                }

          

            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
