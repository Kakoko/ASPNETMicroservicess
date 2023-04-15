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
using Ussd.App.BLL.StateMachines.Withdraw.Summary;
using Ussd.App.BLL.StateMachines.Withdraw.Summary.Tsogolo;

namespace Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.Pockets.LongTerm
{
    public class LongTermPocketExitState : MenuState
    {

        public LongTermPocketExitState(MenuState previousState) : base(previousState)
        {

        }

        public LongTermPocketExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new LongTermPocketEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;
            var withdrawBalance = Convert.ToDecimal(ValueStash["TsogoloWithdrawBalance"].ToString());

            var flag = string.Empty;

            decimal withdrawAmount = 0;



            if (ValueStash.TryGetValue("WithdrawalAmount", out object amount) && amount != null)
            {
                withdrawAmount = Convert.ToDecimal(amount.ToString());

               
            }

            if (ValueStash.TryGetValue("flag", out object flagResponse) && flagResponse != null)
            {
                flag = flagResponse.ToString();
                //ValueStash.Remove("flag");
            }

            if (request.Message.Equals(MenuConstants.Zero))
            {

                menuState = new ChooseServiceEntryState(this);
            }
            else if (request.Message.Equals(MenuConstants.Nine))
            {
                menuState = new WithdrawalTsogoloMainMenuEntryState(this);
            }
            else if (Validation.IsPositiveNumber(request.Message))
            {
                ValueStash["WithdrawalAmount"] = withdrawAmount + Convert.ToInt32(request.Message);
                ValueStash["WithdrawProduct"] = "Tsogolo Savings Plan";



                if (Validation.ValidateNumber(Convert.ToDecimal(request.Message), withdrawBalance, CurrentLanguage.ThresholdValue, out string msg))
                {

                    if (!string.IsNullOrEmpty(flag))
                    {
                        ValueStash["Pocket"] = "Both";
                        ValueStash["WithdrawType"] = "Both";
                        ValueStash["WithdrawalAmountLongTerm"] = request.Message;
                    }
                    else
                    {
                        ValueStash["Pocket"] = "Long Term";
                        ValueStash["WithdrawType"] = "LongTerm";
                    }

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
