using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.KnownValues;
using AngleDimension.Standard.Core.General;
using AngleDimension.Standard.Core.Validations;
using MessageAgent.DataModel.DTOs.TNM;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Core;
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.Services.Notification;
using Ussd.App.BLL.Services.Products;
using Ussd.App.BLL.StateMachines.CallBack;
using Ussd.App.BLL.StateMachines.Error;
using Ussd.App.BLL.StateMachines.GeneralEnquiry;
using Ussd.App.BLL.StateMachines.Product;
using Ussd.App.BLL.StateMachines.ServiceType;

namespace Ussd.App.BLL.StateMachines.GeneralEnquiryFullName
{
    public class GeneralEnquiryFullNameExitState : MenuState
    {



        public GeneralEnquiryFullNameExitState(MenuState previousState) : base(previousState)
        {

        }

        public GeneralEnquiryFullNameExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {
           
        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new GeneralEnquiryFullNameEntryState(this)
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

                menuState = new GeneralEnquiryEntryState(this);
            }
            else
            {


                var service = ServiceProvider.GetService(typeof(IProductsService)) as IProductsService;

                var processResponse = await service.GetProducts(true);
                var productTypes = processResponse.Result as List<ProductDto>;
                var productsWithoutEnquiry = productTypes.Where(u => u.IsGeneralEnquiry).AsEnumerable().First();

                if (processResponse.IsErrorOccurred)
                {
                    menuState = new GeneralErrorEntryState(this);
                }

                if (Validation.ContainsTwoOrMoreNames((request.Message)))
                {

                    ValueStash["FullName"] = request.Message;
                    ValueStash["FirstName"] = request.Message.GetFirstName();


                    var generalEnquiryDetails = new SubmitGeneralEnquiryRequestDto
                    {
                        CustomerName = request.Message,
                        GeneralEnquiryQuestion = ValueStash["EnquiryName"].ToString()!,
                        PhoneNumber = request.Msisdn
                    };

                    try
                    {
                
                        var notificationService = ServiceProvider.GetRequiredService<INotificationService>();

                    
                        await notificationService.SendGeneralEnquiryNotification(generalEnquiryDetails);


                        menuState = new CallBackEntryState(this);

                    }
                    catch (Exception ex)
                    {
                        Console.Out.WriteLine($"Email Exception {ex.Message} ");
                        menuState = new GeneralErrorEntryState(this);
                    }
                    
                }
            }

            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
