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
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.StateMachines.FullName;
using Ussd.App.BLL.StateMachines.Product.Description;
using Ussd.App.BLL.StateMachines.Product;
using Ussd.App.BLL.StateMachines.Withdraw.Authenticate;
using Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.Pockets.ShortTerm;
using Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.Pockets.LongTerm;
using Ussd.App.BLL.StateMachines.Withdraw.Policy;
using Ussd.App.BLL.StateMachines.ServiceType;

namespace Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.MainMenu
{
    public class WithdrawalTsogoloMainMenuExitState : MenuState
    {

        public WithdrawalTsogoloMainMenuExitState(MenuState previousState) : base(previousState)
        {

        }

        public WithdrawalTsogoloMainMenuExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new WithdrawalTsogoloMainMenuEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;
            ValueStash.Remove(DictionaryConstants.ActionRequest);

            if (request.Message.Equals(MenuConstants.Nine))
            {

                menuState = new WithdrawalPolicyEntryState(this);
            }
            else if (request.Message.Equals(MenuConstants.Zero))
            {

                menuState = new ChooseServiceEntryState(this);
            }
            else
            {
                if (int.TryParse(request.Message, out int option) && option > 0)
                {



                    if (option == Convert.ToUInt32(MenuConstants.One))
                    {

                        menuState = new LongTermPocketEntryState(this);
                    }
                    else if (option == Convert.ToUInt32(MenuConstants.Two))
                    {
                        
                        
                        menuState = new ShortTermPocketEntryState(this);
                    }
                    else if (option == Convert.ToUInt32(MenuConstants.Three))
                    {
                        ValueStash["flag"] = "continuos";
                        menuState = new ShortTermPocketEntryState(this);
                    }
                    else
                    {
                        menuState.ErrorMessage = CurrentLanguage.InvalidMessage;
                    }

                }
            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
