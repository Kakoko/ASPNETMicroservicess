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
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.StateMachines.Balance.GreenLife;
using Ussd.App.BLL.StateMachines.Policies;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.Pockets.LongTerm;
using Ussd.App.BLL.StateMachines.Withdraw.Policy;
using Ussd.App.BLL.StateMachines.Withdraw.Summary;
using Ussd.App.BLL.StateMachines.Withdraw.Summary.GreenLife;

namespace Ussd.App.BLL.StateMachines.Withdraw.Balances.GreenLife
{
    public class WithdrawGreenLifeBalanceExitState : MenuState
    {

        public WithdrawGreenLifeBalanceExitState(MenuState previousState) : base(previousState)
        {

        }

        public WithdrawGreenLifeBalanceExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new WithdrawGreenLifeBalanceEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };


            MenuState menuState = errorState;

            var withdrawBalance = Convert.ToDecimal(ValueStash["TsogoloWithdrawBalance"].ToString());

            if (request.Message.Equals(MenuConstants.Zero))
            {

                menuState = new ChooseServiceEntryState(this);
            }
            else if (request.Message.Equals(MenuConstants.Nine))
            {

                menuState = new WithdrawalPolicyEntryState(this);
            }
            else if (Validation.IsPositiveNumber(request.Message))
            {

                if (Validation.ValidateNumber(Convert.ToDecimal(request.Message), withdrawBalance, CurrentLanguage.ThresholdValue, out string msg))
                {
                    ValueStash["WithdrawalAmount"] = request.Message;
                    ValueStash["Pocket"] = "Both";
                    ValueStash["WithdrawProduct"] = "GreenLife Savings Plan";
                    menuState = new WithdrawalGreenLifeSummaryEntryState(this);
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
