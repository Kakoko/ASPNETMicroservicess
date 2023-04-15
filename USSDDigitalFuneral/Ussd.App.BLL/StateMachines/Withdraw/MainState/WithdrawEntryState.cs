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
using Ussd.App.BLL.Models;
using Ussd.App.BLL.Services.Balance;
using Ussd.App.BLL.StateMachines.Balance;
using Ussd.App.BLL.StateMachines.Error;

namespace Ussd.App.BLL.StateMachines.Withdraw.MainState
{
    public class WithdrawEntryState : MenuState
    {

        public WithdrawEntryState(MenuState previousState) : base(previousState)
        {

        }

        public WithdrawEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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





          
            var service = ServiceProvider.GetService(typeof(IBalanceService)) as IBalanceService;





            try
            {
              
                    menuBuilder.AppendLine("Got it. Which product would you like to make a withdrawal from?");
                    menuBuilder.AppendLine();

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
            Handle.CurrentState = new WithdrawExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
