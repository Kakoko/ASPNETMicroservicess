using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.KnownValues;
using MessageAgent.DataModel.DTOs.TNM;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
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
using Ussd.App.BLL.Services.Notification;
using Ussd.App.BLL.StateMachines.CallBack;
using Ussd.App.BLL.StateMachines.Error;
using Ussd.App.BLL.StateMachines.FullName;
using Ussd.App.BLL.StateMachines.Pin;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.UnderConstruction;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ussd.App.BLL.StateMachines.Product.Description
{
    public class ProductDescriptionExitState : MenuState
    {

        public ProductDescriptionExitState(MenuState previousState) : base(previousState)
        {

        }

        public ProductDescriptionExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new ProductDescriptionEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;
            ValueStash.Remove(DictionaryConstants.ActionRequest);

            if (request.Message.Equals(MenuConstants.Nine))
            {

                menuState = new ProductEntryState(this);
            }
            else if (request.Message.Equals(MenuConstants.Zero))
            {

                menuState = new ChooseServiceEntryState(this);
            }
            else
            {
                if (int.TryParse(request.Message, out int option) && option > 0)
                {




                    if (ValueStash.TryGetValue("CustomerInformationCheckNumber", out object value))
                    {
                        var customerInformation = ValueStash["CustomerInformationCheckNumber"].ToString();
                        var customerInformationCheckNumber = JsonSerializer.Deserialize<CheckNumberDto>(customerInformation);

                        ValueStash["FirstName" ] = customerInformationCheckNumber!.CustomerName!.GetFirstName();
                        var productId = Convert.ToInt32(ValueStash["ProductId"].ToString())!;


                        try
                        {
                            var callMeBackNotificationsDetail = new SubmitCallMeBackRequestDto
                            {
                                CustomerName = request.Message,
                                PhoneNumber = request.Msisdn,
                                ProductId = productId
                            };


                            var notificationService = ServiceProvider.GetRequiredService<INotificationService>();


                            var response = await notificationService.SendCallMeBackNotification(callMeBackNotificationsDetail);



                            if(response.IsErrorOccurred)
                            {
                                menuState = new GeneralErrorEntryState(this);
                            }
                            menuState = new CallBackEntryState(this);

                        }
                        catch (Exception Ex)
                        {

                            menuState = new GeneralErrorEntryState(this);
                        }
                       
                    }

                    else if (option == 1)
                    {
                        menuState = new FullNameEntryState(this);
                    }
                    else
                    {
                        menuState.ErrorMessage = CurrentLanguage.InvalidMessage;
                    }

                }
            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
