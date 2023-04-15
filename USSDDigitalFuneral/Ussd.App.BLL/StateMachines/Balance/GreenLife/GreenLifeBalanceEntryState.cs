using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Extensions;
using MessageAgent.DataModel.DTOs.TNM;
using System.Text;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.Services.Balance;
using System.Globalization;
using Ussd.App.BLL.StateMachines.Error;

namespace Ussd.App.BLL.StateMachines.Balance.GreenLife
{
    public class GreenLifeBalanceEntryState : MenuState
    {

        public GreenLifeBalanceEntryState(MenuState previousState) : base(previousState)
        {

        }

        public GreenLifeBalanceEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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
                    menuBuilder.AppendNewLine(string.Format(CurrentLanguage.PolicyPlanDetails , "Green Life Savings"));
                    menuBuilder.AppendNewLine();
                    menuBuilder.AppendNewLine($"{CurrentLanguage.PolicyNumber} {balanceInfo.Balance1.ToString("C2", culture)}");
                    menuBuilder.AppendNewLine($"{CurrentLanguage.NFALoanBalance} {balanceInfo.Balance2.ToString("C2", culture)}");
                    menuBuilder.AppendNewLine($"{CurrentLanguage.UnpaidPremium} {balanceInfo.Balance.ToString("C2", culture)}");
                    menuBuilder.AppendLine();
                }
            }
            catch (Exception Ex)
            {

                Handle.CurrentState = new GeneralErrorEntryState(this) { ServiceProvider = ServiceProvider };
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
            Handle.CurrentState = new GreenLifeBalanceExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
