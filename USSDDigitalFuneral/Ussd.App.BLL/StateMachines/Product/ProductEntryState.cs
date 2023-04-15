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
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.Services.Data;
using Ussd.App.BLL.Services.Products;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.StateMachines.Error;

namespace Ussd.App.BLL.StateMachines.Product
{
    public class ProductEntryState : MenuState
    {

        public ProductEntryState(MenuState previousState) : base(previousState)
        {

        }

        public ProductEntryState(UssdMenu<LanguageDto> handle, LanguageDto language, IDictionary<string, object> valueStash)
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

            menuBuilder.AppendNewLine(CurrentLanguage.ProductSelection);
            menuBuilder.AppendLine();


            var service = ServiceProvider.GetService(typeof(IProductsService)) as IProductsService;




            try
            {
                var processResponse = await service.GetProducts(false);


                if (processResponse.IsErrorOccurred)
                {
                    menuBuilder.AppendNewLine(processResponse.Message);
                }
                else
                {
                    var productTypes = processResponse.Result as List<ProductDto>;

                    int menuNumber = 1;

                    if (productTypes is not null)
                    {
                        foreach (var serviceProviderType in productTypes)
                        {
                            menuBuilder.AppendNewLine($"{menuNumber}.{serviceProviderType.ProductName}");
                            Interlocked.Increment(ref menuNumber);
                        }
                        ValueStash["productTypes"] = productTypes;
                    }
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
            Handle.CurrentState = new ProductExitState(this)
            {
                ServiceProvider = ServiceProvider
            };
            return response;
        }
    }
}
