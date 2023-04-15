using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.KnownValues;
using MessageAgent.DataModel.DTOs.TNM;
using System.Text.Json;
using AngleDimension.NetCore.Ussd.Core;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.StateMachines.Shared.ExitApp;
using Ussd.App.BLL.StateMachines.Shared.Language;
using Ussd.App.BLL.StateMachines.Shared.Success;
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.StateMachines.Product;
using Ussd.App.BLL.StateMachines.GeneralEnquiry;
using System.ComponentModel.Design;
using Ussd.App.BLL.StateMachines.UnderConstruction;
using Ussd.App.BLL.StateMachines.Pin;
using Ussd.App.BLL.StateMachines.Withdraw.Authenticate;

namespace Ussd.App.BLL.StateMachines.ServiceType
{
    public class ChooseServiceExitState : MenuState
    {

        public ChooseServiceExitState(MenuState previousState) : base(previousState)
        {
            
        }

        public ChooseServiceExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {
            
        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new ChooseServiceEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;



           


            var array = ValueStash["ServiceProviderTypes"];
            var serviceProviderTypes = JsonSerializer.Deserialize<IEnumerable<StaticDataStore>>(array.ToString());

            if (int.TryParse(request.Message, out int option) && option > 0 && option <= serviceProviderTypes.Count())
            {
                var serviceProviderType = serviceProviderTypes.ElementAt(option - 1);

                if (serviceProviderType.ProductCode == "A")
                {
                    menuState = new ProductEntryState(this);
                }
                else if (serviceProviderType.ProductCode == "G")
                {
                    menuState = new GeneralEnquiryEntryState(this);
                }
                else if (serviceProviderType.ProductCode == "V")
                {
                    
                    
                    if (ValueStash.TryGetValue("NewOrOldCustomer" , out object value))
                    {
                        if (value.ToString().Equals(CurrentLanguage.NewOrOldCustomer))
                        {
                            menuState = new AuthenticateEntryState(this);
                        }
                       
                    }
                    else
                    {
                      
                        menuState = new UnregisteredCustomerEntryState(this);
                    }

                    
                    
                }
                else if (serviceProviderType.ProductCode == "C")
                {
                    menuState = new PinChangeEntryState(this);
                }
                else if (serviceProviderType.ProductCode == "M")
                {

                    if (ValueStash.TryGetValue("NewOrOldCustomer", out object value))
                    {
                        if (value.ToString().Equals(CurrentLanguage.NewOrOldCustomer))
                        {
                            menuState = new WithdrawalAuthenticateEntryState(this);
                        }

                    }
                    else
                    {

                        menuState = new UnregisteredCustomerEntryState(this);
                    }
                
                }
                else
                {
                    menuState = new ErrorStateEntryState(this);
                }


            }


            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
