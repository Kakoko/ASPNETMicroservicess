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
using AngleDimension.NetCore.Ussd.KnownValues;
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.Shared.ExitApp;
using System.Text.Json;
using Ussd.App.BLL.StateMachines.FullName;
using Ussd.App.BLL.StateMachines.GeneralEnquiryFullName;
using Ussd.App.BLL.Models;
using Microsoft.Extensions.DependencyInjection;
using Ussd.App.BLL.Services.Notification;
using Ussd.App.BLL.StateMachines.CallBack;
using Ussd.App.BLL.StateMachines.Error;

namespace Ussd.App.BLL.StateMachines.GeneralEnquiry
{
    public class GeneralEnquiryExitState : MenuState
    {

        public GeneralEnquiryExitState(MenuState previousState) : base(previousState)
        {

        }

        public GeneralEnquiryExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new GeneralEnquiryEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;


            if (request.Message.Equals(MenuConstants.Nine))
            {

                menuState = new ChooseServiceEntryState(this);
            }
            else
            {


                var array = ValueStash["GeneralEnquiryTypes"];
                var serviceProviderTypes = JsonSerializer.Deserialize<IEnumerable<StaticDataStore>>(array.ToString());

                if (int.TryParse(request.Message, out int option) && option > 0 && option <= serviceProviderTypes.Count())
                {
                    var serviceProviderType = serviceProviderTypes.ElementAt(option - 1);


                    ValueStash["EnquiryName"] = serviceProviderType.ProductName;
                    ValueStash["ProductName"] = "";


                    if (ValueStash.TryGetValue("CustomerInformationCheckNumber", out object value))
                    {
                        var customerInformation = ValueStash["CustomerInformationCheckNumber"].ToString();
                        var customerInformationCheckNumber = JsonSerializer.Deserialize<CheckNumberDto>(customerInformation);

                        ValueStash["FirstName"] = customerInformationCheckNumber!.CustomerName!.GetFirstName();

                        var generalEnquiryDetails = new SubmitGeneralEnquiryRequestDto
                        {
                            CustomerName = customerInformationCheckNumber!.CustomerName!,
                            GeneralEnquiryQuestion = ValueStash["EnquiryName"].ToString()!,
                            PhoneNumber = customerInformationCheckNumber.PhoneNumber
                        };

                        try
                        {
                            var notificationService = ServiceProvider.GetRequiredService<INotificationService>();


                            await notificationService.SendGeneralEnquiryNotification(generalEnquiryDetails);


                            menuState = new CallBackEntryState(this);
                        }
                        catch (Exception Ex)
                        {

                            Console.Out.WriteLine($"Email Exception {Ex.Message} ");
                            menuState = new GeneralErrorEntryState(this);
                        }

                    }
                    else
                    {
                        menuState = new GeneralEnquiryFullNameEntryState(this);
                    }

                      


                }
            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
