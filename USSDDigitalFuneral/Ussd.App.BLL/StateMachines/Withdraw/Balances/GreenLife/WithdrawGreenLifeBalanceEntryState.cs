using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Extensions;
using MessageAgent.DataModel.DTOs.TNM;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.Services.Balance;
using Ussd.App.BLL.StateMachines.Balance;
using Ussd.App.BLL.StateMachines.Error;

namespace Ussd.App.BLL.StateMachines.Withdraw.Balances.GreenLife
{
    public class WithdrawGreenLifeBalanceEntryState : MenuState
    {

        public WithdrawGreenLifeBalanceEntryState(MenuState previousState) : base(previousState)
        {

        }

        public WithdrawGreenLifeBalanceEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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





            var customerBalance = ValueStash["PolicyNumber"].ToString();
            var customerFirstName = ValueStash["CustomerFirstName"].ToString();
            var service = ServiceProvider.GetService(typeof(IBalanceService)) as IBalanceService;



            try
            {
                var processResponse = await service.GetCustomerBalance(customerBalance, request.Msisdn.ToString());




                if (processResponse.IsErrorOccurred)
                {
                    menuBuilder.AppendNewLine(processResponse.Message);
                }


                else
                {


                    var balanceInfo = processResponse.Result as CustomerBalanceDto;

                    var culture = CultureInfo.GetCultureInfo(CurrentLanguage.CountryCulture);



                    string currencySymbol = "K";
                    CultureInfo customCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    customCulture.NumberFormat.CurrencySymbol = currencySymbol;
                   




                    menuBuilder.AppendNewLine($"{customerFirstName}, how much would you like to withdraw from your Green Life?");
                    menuBuilder.AppendNewLine();
                    menuBuilder.AppendNewLine($"Current Balance: {balanceInfo.Balance.ToString("C2", customCulture)}");
                    ValueStash["TsogoloWithdrawBalance"] = balanceInfo.Balance;
                    menuBuilder.AppendLine();
                }
            }
            catch (Exception Ex)
            {

                Handle.CurrentState = new GeneralErrorEntryState(this)
                {
                    ServiceProvider = ServiceProvider
                };
            }


            menuBuilder.AppendNewLine();
            menuBuilder.AppendNewLine(CurrentLanguage.Back);
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
            Handle.CurrentState = new WithdrawGreenLifeBalanceExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
