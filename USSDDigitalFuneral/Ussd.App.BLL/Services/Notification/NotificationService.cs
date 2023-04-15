using AngleDimension.NetCore.Logging;
using AngleDimension.Standard.Core.General;
using AngleDimension.Standard.Http.HttpServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.Services.Products;

namespace Ussd.App.BLL.Services.Notification
{
    public class NotificationService : INotificationService
    {

        private readonly string _enquiriesUrl;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(string enquiriesUrl, ILogger<NotificationService> logger)
        {
            _enquiriesUrl = enquiriesUrl;
            _logger = logger;
        }

        public async Task<ProcessResponse> SendCallMeBackNotification(SubmitCallMeBackRequestDto submitCallMeBackRequestDto)
        {
            try
            {

                var response = await HttpRequestFactory.Post($"{_enquiriesUrl}/Enquiries/SubmitCallMeBack", submitCallMeBackRequestDto);


                if (response.StatusCode == HttpStatusCode.Accepted)
                {

                    return new ProcessResponse
                    {
                        IsErrorOccurred = false,
                        Result = "Notification Sent"
                    };
                }

                var httpProcessMessage = await HttpErrorResponseService.SanitizeAsync(response);
                return new ProcessResponse
                {
                    IsErrorOccurred = true,
                    Message = httpProcessMessage.Message,
                    ResponseCode = "ERROR"
                };
            }
            catch (Exception Ex)
            {

                var logWriter = new LogWriter();
                logWriter.Exception(Ex);

                return new ProcessResponse
                {
                    IsErrorOccurred = true,
                    ResponseCode = "APP_ERROR"
                };
            }
        }

        public async Task<ProcessResponse> SendGeneralEnquiryNotification(SubmitGeneralEnquiryRequestDto generalEnquiryRequestDto)
        {
            try
            {

                var response = await HttpRequestFactory.Post($"{_enquiriesUrl}/Enquiries/SubmitGeneralEnquiry", generalEnquiryRequestDto);


                if (response.StatusCode == HttpStatusCode.Accepted)
                {

                    return new ProcessResponse
                    {
                        IsErrorOccurred = false,
                        Result = "Notification Sent"
                    };
                }

                var httpProcessMessage = await HttpErrorResponseService.SanitizeAsync(response);
                return new ProcessResponse
                {
                    IsErrorOccurred = true,
                    Message = httpProcessMessage.Message,
                    ResponseCode = "ERROR"
                };
            }
            catch (Exception Ex)
            {

                var logWriter = new LogWriter();
                // logWriter.Exception(ex);

                return new ProcessResponse
                {
                    IsErrorOccurred = true,
                    ResponseCode = "APP_ERROR"
                };
            }

        }

        public async Task<ProcessResponse> SendNotification(NotificationDto notificationDto)
        {
            _logger.LogDebug("Sending Notification {NotificationDto}", notificationDto);


            try
            {

              
                var response = await HttpRequestFactory.Post($"{_enquiriesUrl}/SubmitEnquiry", notificationDto);

                if (response.StatusCode == HttpStatusCode.Accepted)
                {

                    return new ProcessResponse
                    {
                        IsErrorOccurred = false,
                        Result = "Notification Sent"
                    };
                }

                var httpProcessMessage = await HttpErrorResponseService.SanitizeAsync(response);
                return new ProcessResponse
                {
                    IsErrorOccurred = true,
                    Message = httpProcessMessage.Message,
                    ResponseCode = "ERROR"
                };

            }
            catch (Exception ex)
            {

                var logWriter = new LogWriter();
               // logWriter.Exception(ex);

                return new ProcessResponse
                {
                    IsErrorOccurred = true,
                    ResponseCode = "APP_ERROR"
                };
            }
        }

        public async Task<ProcessResponse> SendWithdrawRequest(SubmitPartialWithdrawalDto submitPartialWithdrawalDto)
        {
            try
            {
                var response = await HttpRequestFactory.Post($"{_enquiriesUrl}/PartialWithdrawals/SubmitEnquiry", submitPartialWithdrawalDto);

                if (response.StatusCode == HttpStatusCode.Accepted)
                {

                    return new ProcessResponse
                    {
                        IsErrorOccurred = false,
                        Result = "Notification Sent"
                    };
                }

                var httpProcessMessage = await HttpErrorResponseService.SanitizeAsync(response);
                return new ProcessResponse
                {
                    IsErrorOccurred = true,
                    Message = httpProcessMessage.Message,
                    ResponseCode = "ERROR"
                };
            }
            catch (Exception)
            {

                var logWriter = new LogWriter();
                // logWriter.Exception(ex);

                return new ProcessResponse
                {
                    IsErrorOccurred = true,
                    ResponseCode = "APP_ERROR"
                };
            }
        }
    }
}
