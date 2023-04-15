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
using Ussd.App.BLL.Services.Data;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.Services.Authentication;
using Ussd.App.BLL.Services.Balance;
using System.Globalization;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.StateMachines.Shared.ExitApp;
using Ussd.App.BLL.StateMachines.Error;

namespace Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.Pockets.LongTerm
{
    public class LongTermPocketEntryState : MenuState
    {

        public LongTermPocketEntryState(MenuState previousState) : base(previousState)
        {

        }

        public LongTermPocketEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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
                    var culture = CultureInfo.GetCultureInfo(CurrentLanguage.CountryCulture);
                    var balanceInfo = processResponse.Result as CustomerBalanceDto;
                    menuBuilder.AppendNewLine(string.Format(CurrentLanguage.LongTermWithdrawalPocket , customerFirstName));
                    menuBuilder.AppendNewLine();
                    menuBuilder.AppendNewLine("You will be charged for withdrawing");

                    menuBuilder.AppendNewLine(string.Format(CurrentLanguage.CurrentBalance , balanceInfo!.Balance1.ToString("C2", culture)));
                    ValueStash["TsogoloWithdrawBalance"] = balanceInfo.Balance1;

                    menuBuilder.AppendLine();
                }



            }
            catch (Exception)
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
            Handle.CurrentState = new LongTermPocketExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
