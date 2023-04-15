using AngleDimension.Mno.DataModels;
using AngleDimension.NetCore.Ussd.Core;
using AngleDimension.NetCore.Ussd.KnownValues;
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
using Ussd.App.BLL.Services.Notification;
using Ussd.App.BLL.StateMachines.CallBack;
using Ussd.App.BLL.StateMachines.Error;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.Withdraw.Balances.Tsogolo.MainMenu;
using Ussd.App.BLL.StateMachines.Withdraw.Policy;
using Ussd.App.BLL.StateMachines.Withdraw.Success;

namespace Ussd.App.BLL.StateMachines.Withdraw.Summary.Tsogolo
{
    public class WithdrawalTsogoloSummaryExitState : MenuState
    {

        public WithdrawalTsogoloSummaryExitState(MenuState previousState) : base(previousState)
        {

        }

        public WithdrawalTsogoloSummaryExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new WithdrawalTsogoloSummaryEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;


            if (request.Message.Equals(MenuConstants.Zero))
            {
                menuState = new ChooseServiceEntryState(this);
            }
            else if (request.Message.Equals(MenuConstants.One))
            {


                //Check if it single or both
                var withdrawAmount = ValueStash["WithdrawalAmount"].ToString();
                var policyNumber = ValueStash["PolicyNumber"].ToString();
                var productId = ValueStash["ProductId"].ToString();
                //  var p = ValueStash["ProductId"].ToString();
               
                var service = ServiceProvider.GetService(typeof(INotificationService)) as INotificationService;


                if (ValueStash.TryGetValue("WithdrawType", out object flagResponse))
                {





                    if (flagResponse.ToString().Equals("ShortTerm"))

                    {


                        var withdrawalRequest = new SubmitPartialWithdrawalDto
                        {
                            Balance = Convert.ToDecimal(withdrawAmount),
                            Balance1 = 0,
                            Balance2 = 0,
                            PhoneNumber = request.Msisdn,
                            PolicyNumber = policyNumber,
                            ProductId = Convert.ToInt32(productId)
                        };

                        try
                        {
                            var processResponse = await service.SendWithdrawRequest(withdrawalRequest);


                            if (processResponse.IsErrorOccurred)
                            {
                                //menuBuilder.AppendNewLine(processResponse.Message);
                                errorState.ErrorMessage = "Action Not Completed";
                            }
                            else
                            {
                                //We need to make the Request Here
                                menuState = new SuccessWithdrawalEntryState(this);
                            }
                        }
                        catch (Exception)
                        {

                            menuState = new GeneralErrorEntryState(this);
                        }
                    }
                    else if (flagResponse.ToString().Equals("LongTerm"))

                    {

                        var withdrawalRequest = new SubmitPartialWithdrawalDto
                        {
                            Balance = 0,
                            Balance1 = Convert.ToDecimal(withdrawAmount),
                            Balance2 = 0,
                            PhoneNumber = request.Msisdn,
                            PolicyNumber = policyNumber,
                            ProductId = Convert.ToInt32(productId)
                        };

                        try
                        {
                            var processResponse = await service.SendWithdrawRequest(withdrawalRequest);


                            if (processResponse.IsErrorOccurred)
                            {
                                //menuBuilder.AppendNewLine(processResponse.Message);
                                errorState.ErrorMessage = "Action Not Completed";
                            }
                            else
                            {
                                //We need to make the Request Here
                                menuState = new SuccessWithdrawalEntryState(this);
                            }
                        }
                        catch (Exception)
                        {

                            menuState = new GeneralErrorEntryState(this);
                        }



                    }
                    else if (flagResponse.ToString().Equals("Both"))

                    {
                        var shortTermAmount = ValueStash["WithdrawalAmountShortTerm"].ToString();
                        var longTermAmount = ValueStash["WithdrawalAmountLongTerm"].ToString();
                       // var withdrawType = ValueStash["WithdrawType"].ToString();



                        var withdrawalRequest = new SubmitPartialWithdrawalDto
                        {
                            Balance = Convert.ToDecimal(shortTermAmount),
                            Balance1 = Convert.ToDecimal(longTermAmount),
                            Balance2 = 0,
                            PhoneNumber = request.Msisdn,
                            PolicyNumber = policyNumber,
                            ProductId = Convert.ToInt32(productId)
                        };

                        try
                        {
                            var processResponse = await service.SendWithdrawRequest(withdrawalRequest);


                            if (processResponse.IsErrorOccurred)
                            {
                                //menuBuilder.AppendNewLine(processResponse.Message);
                                errorState.ErrorMessage = "Action Not Completed";
                            }
                            else
                            {
                                //We need to make the Request Here
                                menuState = new SuccessWithdrawalEntryState(this);
                            }
                        }
                        catch (Exception)
                        {

                            menuState = new GeneralErrorEntryState(this);
                        }
                      
                    }
                }
                       
            
            }
            else if (request.Message.Equals(MenuConstants.Two))
            {

                //Change Amount
                menuState = new WithdrawalTsogoloMainMenuEntryState(this);
            }
            else if (request.Message.Equals(MenuConstants.Three))
            {
                //Change Product
                menuState = new WithdrawalPolicyEntryState(this);
            }



            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
