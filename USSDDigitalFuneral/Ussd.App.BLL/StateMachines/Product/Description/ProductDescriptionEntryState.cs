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
using Ussd.App.BLL.Services.Products;
using Ussd.App.BLL.StateMachines.Error;

namespace Ussd.App.BLL.StateMachines.Product.Description
{
    public class ProductDescriptionEntryState : MenuState
    {

        public ProductDescriptionEntryState(MenuState previousState) : base(previousState)
        {

        }   

        public ProductDescriptionEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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

       


            try
            {

              
                var productDescription = ValueStash["ProductDescription"];


                menuBuilder.AppendNewLine(string.Format(CurrentLanguage.ProductDescriptionMessage!, productDescription));
                menuBuilder.AppendLine();



                menuBuilder.AppendLine(CurrentLanguage.PleaseCallMe);


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
            Handle.CurrentState = new ProductDescriptionExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
