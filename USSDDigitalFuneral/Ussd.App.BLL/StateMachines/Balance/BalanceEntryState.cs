using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Extensions;
using AngleDimension.Standard.Core.General;
using MessageAgent.DataModel.DTOs.TNM;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.StateMachines.Pin;
using Ussd.App.BLL.Services.Products;
using Ussd.App.BLL.Services.Balance;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.StateMachines.Error;

namespace Ussd.App.BLL.StateMachines.Balance
{
    public class BalanceEntryState : MenuState
    {

        public BalanceEntryState(MenuState previousState) : base(previousState)
        {

        }
            
        public BalanceEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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
                    menuBuilder.AppendNewLine(string.Format(CurrentLanguage.PolicyPlanDetails, customerFirstName));
                    menuBuilder.AppendNewLine($"Policy Balance: {balanceInfo.Balance1}");
                    menuBuilder.AppendNewLine($"NFA Loan Balance: {balanceInfo.Balance2}");
                    menuBuilder.AppendNewLine($"Premium Balance: {balanceInfo.Balance}");
                    menuBuilder.AppendLine();
                }
            }
            catch (Exception Ex)
            {

                Handle.CurrentState = new GeneralErrorEntryState(this) { ServiceProvider = ServiceProvider };
            }
            

         





            menuBuilder.AppendNewLine();
            menuBuilder.AppendNewLine(CurrentLanguage.Back);

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
            Handle.CurrentState = new BalanceExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
