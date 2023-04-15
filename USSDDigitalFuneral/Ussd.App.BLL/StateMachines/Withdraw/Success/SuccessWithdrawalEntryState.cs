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

namespace Ussd.App.BLL.StateMachines.Withdraw.Success
{
    public class SuccessWithdrawalEntryState : MenuState
    {

        public SuccessWithdrawalEntryState(MenuState previousState) : base(previousState)
        {

        }

        public SuccessWithdrawalEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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


          

            menuBuilder.AppendNewLine("Thank You");
            menuBuilder.AppendNewLine();
            menuBuilder.AppendNewLine("Your request has been submitted successfully");
            menuBuilder.AppendNewLine();
            menuBuilder.AppendNewLine("Our consultant will be in touch within 48 working hours");
            menuBuilder.AppendLine();




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
            Handle.CurrentState = new SuccessWithdrawalExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
