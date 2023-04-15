using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Extensions;
using AngleDimension.NetCore.Ussd.KnownValues;
using MessageAgent.DataModel.DTOs.TNM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.StateMachines.Shared.Success;
using AngleDimension.Standard.Core.General;
using PhoneNumbers;
using Microsoft.Extensions.DependencyInjection;
using Ussd.App.BLL.Models;

namespace Ussd.App.BLL.StateMachines.CallBack
{
    public class CallBackEntryState : MenuState
    {

        public CallBackEntryState(MenuState previousState) : base(previousState)
        {
           
        }

        public CallBackEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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




          


            var firstName = ValueStash["FirstName"];
        
            
            menuBuilder.AppendNewLine(CurrentLanguage.ThankYou);
            menuBuilder.AppendNewLine();
            menuBuilder.AppendNewLine(CurrentLanguage.CallBack);
            menuBuilder.AppendNewLine();
            menuBuilder.AppendNewLine(string.Format(CurrentLanguage.MoreAssistance! , firstName));
            menuBuilder.AppendLine();
        

      

            menuBuilder.AppendNewLine();
            menuBuilder.AppendNewLine(CurrentLanguage.Yes);
            menuBuilder.AppendNewLine(CurrentLanguage.No);
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
            Handle.CurrentState = new CallBackExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
