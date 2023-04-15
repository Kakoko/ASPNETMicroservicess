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
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.StateMachines.GeneralEnquiry;
using Ussd.App.BLL.StateMachines.Pin;
using Ussd.App.BLL.StateMachines.Product;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.UnderConstruction;
using Ussd.App.BLL.StateMachines.Withdraw.Authenticate;
using Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.MainMenu;
using Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.Pockets.LongTerm;
using Ussd.App.BLL.StateMachines.Withdraw.Summary;
using Ussd.App.BLL.StateMachines.Withdraw.Summary.Tsogolo;

namespace Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.Pockets.ShortTerm
{
    public class ShortTermPocketExitState : MenuState
    {

        public ShortTermPocketExitState(MenuState previousState) : base(previousState)
        {

        }

        public ShortTermPocketExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new ShortTermPocketEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };

            MenuState menuState = errorState;
            var flag = string.Empty;
            var withdrawBalance = Convert.ToDecimal(ValueStash["TsogoloWithdrawBalance"].ToString());


            if (ValueStash.TryGetValue("flag", out object flagResponse))
            {
                flag = flagResponse.ToString();
       
            }

            


            if (request.Message.Equals(MenuConstants.Zero))
            {

                menuState = new ChooseServiceEntryState(this);
            }
            else if (request.Message.Equals(MenuConstants.Nine))
            {
                menuState = new WithdrawalTsogoloMainMenuEntryState(this);
            }


            else if (!string.IsNullOrWhiteSpace(flag) && Validation.IsPositiveNumber(request.Message))
            {
             

                if (Validation.ValidateNumber(Convert.ToDecimal(request.Message), withdrawBalance, CurrentLanguage.ThresholdValue, out string msg))
                {
                    ValueStash["WithdrawalAmount"] = request.Message;
                    ValueStash["WithdrawalAmountShortTerm"] = request.Message;
                    ValueStash["Pocket"] = "Both";
                    ValueStash["WithdrawType"] = "Both";
                    ValueStash["WithdrawProduct"] = "Tsogolo Savings Plan";
                    menuState = new LongTermPocketEntryState(this);
                }


                else
                {
                    errorState.ErrorMessage = msg;
                }
            }



            else if (Validation.IsPositiveNumber(request.Message))
            {


                if(Validation.ValidateNumber(Convert.ToDecimal(request.Message) , withdrawBalance, CurrentLanguage.ThresholdValue , out string msg))
                {
                    ValueStash["WithdrawalAmount"] = request.Message;
                    ValueStash["Pocket"] = "Short Term";
                    ValueStash["WithdrawType"] = "ShortTerm";
                    ValueStash["WithdrawProduct"] = "Tsogolo Savings Plan";
                    menuState = new WithdrawalTsogoloSummaryEntryState(this);
                }


                else
                {
                    errorState.ErrorMessage = msg;
                }
               
            }

           



            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
