using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Extensions;
using AngleDimension.Standard.Core.General;
using MessageAgent.DataModel.DTOs.TNM;
using Microsoft.Extensions.DependencyInjection;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.StateMachines.FullName;

namespace Ussd.App.BLL.StateMachines.GeneralEnquiryFullName
{
    public class GeneralEnquiryFullNameEntryState : MenuState
    {

        public GeneralEnquiryFullNameEntryState(MenuState previousState) : base(previousState)
        {

        }

        public GeneralEnquiryFullNameEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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


            var productName = ValueStash["ProductName"];


        
            menuBuilder.AppendNewLine(CurrentLanguage.GeneralEnquiryFullNameTitle);
            menuBuilder.AppendLine();
            menuBuilder.AppendLine(CurrentLanguage.FullNameExample);

          
          

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
            Handle.CurrentState = new GeneralEnquiryFullNameExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
