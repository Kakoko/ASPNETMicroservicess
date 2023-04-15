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
using Ussd.App.BLL.StateMachines.CallBack;
using System.Globalization;

namespace Ussd.App.BLL.StateMachines.Withdraw.Summary.Tsogolo
{
    public class WithdrawalTsogoloSummaryEntryState : MenuState
    {

        public WithdrawalTsogoloSummaryEntryState(MenuState previousState) : base(previousState)
        {

        }

        public WithdrawalTsogoloSummaryEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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



            var withdrawAmount = ValueStash["WithdrawalAmount"];
            var withdrawProduct = ValueStash["WithdrawProduct"];
            var pocket = ValueStash["Pocket"];



            string currencySymbol = "K";

            CultureInfo customCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            customCulture.NumberFormat.CurrencySymbol = currencySymbol;
            decimal amount = Convert.ToDecimal(withdrawAmount);
            string formattedAmount = amount.ToString("C", customCulture);



            menuBuilder.AppendNewLine("Here's your withdrawal summary ");
            menuBuilder.AppendNewLine();
            menuBuilder.AppendNewLine($"You're withdrawing: {formattedAmount}");
            menuBuilder.AppendNewLine($"Product: {withdrawProduct}");
            menuBuilder.AppendNewLine($"Pocket: {pocket}");
            menuBuilder.AppendNewLine();


            menuBuilder.AppendNewLine("1. Yes, continue");
            menuBuilder.AppendNewLine("2. Change amount");
            menuBuilder.AppendNewLine("3. Change product");






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
            Handle.CurrentState = new WithdrawalTsogoloSummaryExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
