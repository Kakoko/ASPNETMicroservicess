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
using Ussd.App.BLL.StateMachines.Error;
using Ussd.App.BLL.StateMachines.Pin;
using Ussd.App.BLL.StateMachines.Policies;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.UnderConstruction;
using Ussd.App.BLL.StateMachines.Withdraw.Authenticate;
using Ussd.App.BLL.StateMachines.Withdraw.Balances;
using Ussd.App.BLL.StateMachines.Withdraw.Balances.GreenLife;
using Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo;
using Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.MainMenu;

namespace Ussd.App.BLL.StateMachines.Withdraw.Policy
{
    public class WithdrawalPolicyExitState : MenuState
    {

        public WithdrawalPolicyExitState(MenuState previousState) : base(previousState)
        {

        }

        public WithdrawalPolicyExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new WithdrawalPolicyEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };

            MenuState menuState = errorState;

            if (request.Message.Equals(MenuConstants.Zero))
            {

                menuState = new WithdrawalAuthenticateEntryState(this);
            }
            else
            {
                var array = ValueStash["policyTypes"];

                var serviceProviderTypes = JsonSerializer.Deserialize<IEnumerable<CustomerPolicyDto>>(array.ToString()).Where(u => u.PolicyName == "Tsogolo Savings" || u.PolicyName == "Green Life").ToList();

                if (int.TryParse(request.Message, out int option) && option > 0 && option <= serviceProviderTypes.Count())
                {
                   var serviceProviderType = serviceProviderTypes.ElementAt(option-1);

                    ValueStash["PolicyNumber"] = serviceProviderType.PolicyNumber;
                    ValueStash["PolicyName"] = serviceProviderType.PolicyName;
                    ValueStash["ProductId"] = serviceProviderType.ProductId;
                  


                    menuState = serviceProviderType.PolicyName switch
                    {
                        "Tsogolo Savings" => new WithdrawalTsogoloMainMenuEntryState(this),
                        "Green Life" => new WithdrawGreenLifeBalanceEntryState(this),
                        _ => new GeneralErrorEntryState(this)
                    };
    
                }
            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
