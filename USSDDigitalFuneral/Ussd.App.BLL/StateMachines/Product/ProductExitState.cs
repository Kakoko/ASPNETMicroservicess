using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.KnownValues;
using MessageAgent.DataModel.DTOs.TNM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.StateMachines.FullName;
using Ussd.App.BLL.StateMachines.Product.Description;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.Shared.ExitApp;
using Ussd.App.BLL.StateMachines.Shared.Language;

namespace Ussd.App.BLL.StateMachines.Product
{
    public class ProductExitState : MenuState
    {

        public ProductExitState(MenuState previousState) : base(previousState)
        {

        }

        public ProductExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new ProductEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;
            ValueStash.Remove(DictionaryConstants.ActionRequest);

            if (request.Message.Equals(MenuConstants.Nine))
            {
         
                menuState = new ChooseServiceEntryState(this);
            }
            else
            {
                var array = ValueStash["productTypes"];
                var serviceProviderTypes = JsonSerializer.Deserialize<IEnumerable<ProductDto>>(array.ToString());

                if (int.TryParse(request.Message, out int option) && option > 0 && option <= serviceProviderTypes.Count())
                {
                    var serviceProviderType = serviceProviderTypes.ElementAt(option - 1);

                    ValueStash["ProductName"] = serviceProviderType.ProductName;
                    ValueStash["ProductId"] = serviceProviderType.ProductId;
                    ValueStash["ProductDescription"] = serviceProviderType.ProductDescription;
             
                    menuState = new ProductDescriptionEntryState(this);
                    

                }
            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
