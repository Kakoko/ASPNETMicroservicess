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
using Ussd.App.BLL.StateMachines.Pin;

namespace Ussd.App.BLL.StateMachines.Withdraw.Authenticate
{
    public class WithdrawalAuthenticateEntryState : MenuState
    {

        public WithdrawalAuthenticateEntryState(MenuState previousState) : base(previousState)
        {

        }

        public WithdrawalAuthenticateEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
            : base(handle, language, valueStash)    
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var menuBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {

                menuBuilder.AppendNewLine(ErrorMessage);
                menuBuilder.AppendNewLine();
            }

            menuBuilder.AppendNewLine(CurrentLanguage.EnterPinMessageForWithdraw);
            menuBuilder.AppendLine();

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
            Handle.CurrentState = new WithdrawalAuthenticateExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
