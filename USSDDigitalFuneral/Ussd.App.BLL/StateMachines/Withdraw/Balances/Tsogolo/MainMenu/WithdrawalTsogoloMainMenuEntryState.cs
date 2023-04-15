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
using Ussd.App.BLL.StateMachines.Product.Description;

namespace Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.MainMenu
{
    public class WithdrawalTsogoloMainMenuEntryState : MenuState
    {

        public WithdrawalTsogoloMainMenuEntryState(MenuState previousState) : base(previousState)
        {

        }

        public WithdrawalTsogoloMainMenuEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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




                menuBuilder.AppendLine(CurrentLanguage.TsogoloProductSelectecion);
                menuBuilder.AppendNewLine();
                menuBuilder.AppendLine(CurrentLanguage.LongTermPocket);
                menuBuilder.AppendLine(CurrentLanguage.ShortTermPocket);
                menuBuilder.AppendLine(CurrentLanguage.Both);
                menuBuilder.AppendLine();





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
            Handle.CurrentState = new WithdrawalTsogoloMainMenuExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
