using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.KnownValues;
using AngleDimension.Standard.Core.Validations;
using MessageAgent.DataModel.DTOs.TNM;
using Microsoft.Extensions.DependencyInjection;
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
using Ussd.App.BLL.StateMachines.Product;
using Ussd.App.BLL.StateMachines.ServiceType;

namespace Ussd.App.BLL.StateMachines.FullName
{
    public class FullNameExitState : MenuState
    {

        public FullNameExitState(MenuState previousState) : base(previousState)
        {

        }

        public FullNameExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new FullNameEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;
          

            if (request.Message.Equals(MenuConstants.Zero))
            {
                menuState = new ChooseServiceEntryState(this);
            }
            else if (request.Message.Equals(MenuConstants.Nine))
            {

                menuState = new ProductEntryState(this);
            }
            else
            {



                if (Validation.ContainsTwoOrMoreNames(request.Message))
                {

                    ValueStash["FullName"] = request.Message;
                    ValueStash["FirstName"] = request.Message.GetFirstName();
                    var productId = Convert.ToInt32(ValueStash["ProductId"].ToString())!;


                    var callMeBackNotificationsDetail = new SubmitCallMeBackRequestDto
                    {
                        CustomerName = request.Message,
                        PhoneNumber = request.Msisdn,
                        ProductId = productId
                    };



                    try
                    {

                        var notificationService = ServiceProvider.GetRequiredService<INotificationService>();


                        var response = await notificationService.SendCallMeBackNotification(callMeBackNotificationsDetail);



                        if (response.IsErrorOccurred)
                        {
                            menuState = new GeneralErrorEntryState(this);
                        }
                        menuState = new CallBackEntryState(this);

                    }
                    catch (Exception ex)
                    {
                        Console.Out.WriteLine($"Email Exception {ex.Message} ");
                        menuState = new ChooseServiceEntryState(this);
                    }


                }
            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
