using AngleDimension.Standard.Core.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Models;

namespace Ussd.App.BLL.Services.Notification
{
    public interface INotificationService   
    {
        Task<ProcessResponse> SendNotification(NotificationDto notificationDto);
        Task<ProcessResponse> SendWithdrawRequest(SubmitPartialWithdrawalDto submitPartialWithdrawalDto);
        Task<ProcessResponse> SendCallMeBackNotification(SubmitCallMeBackRequestDto submitCallMeBackRequestDto);
        Task<ProcessResponse> SendGeneralEnquiryNotification(SubmitGeneralEnquiryRequestDto generalEnquiryRequestDto);
    }
}
    