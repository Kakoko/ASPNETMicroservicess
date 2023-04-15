using AngleDimension.Mno.DataModels.TNM;
using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.Extensions;
using AngleDimension.Standard.Core.General;
using MessageAgent.DataModel.DTOs.TNM;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.Services.Products;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.Services.Data;

namespace Ussd.App.BLL.StateMachines.GeneralEnquiry
{
    public class GeneralEnquiryEntryState : MenuState
    {

        public GeneralEnquiryEntryState(MenuState previousState) : base(previousState)
        {

        }

        public GeneralEnquiryEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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
            menuBuilder.AppendNewLine(CurrentLanguage.GeneralEnquiryMenu);
            menuBuilder.AppendLine();


            var generalEnquiryTypes = StaticDataStoreFactory.CreateGeneralEnquiryMenuProducts();


            int menuNumber = 1;

            if (generalEnquiryTypes is not null)
            {
                foreach (var serviceProviderType in generalEnquiryTypes)
                {
                    menuBuilder.AppendNewLine($"{menuNumber}.{serviceProviderType.ProductName}");
                    Interlocked.Increment(ref menuNumber);
                }




                ValueStash["GeneralEnquiryTypes"] = generalEnquiryTypes;
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
            Handle.CurrentState = new GeneralEnquiryExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
