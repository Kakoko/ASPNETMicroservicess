using AngleDimension.NetCore.Logging;
using AngleDimension.Standard.Core.General;
using AngleDimension.Standard.Http.HttpServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Models;

namespace Ussd.App.BLL.Services.Balance
{
    public class BalanceService : IBalanceService
    {

        private readonly string _balanceUrl;
        private readonly ILogger<BalanceService> _logger;

        public BalanceService(string balanceUrl , ILogger<BalanceService> logger)
        {
            _logger = logger;
            _balanceUrl = balanceUrl;
        }

        public async Task<ProcessResponse> GetCustomerBalance(string policyNumber, string phoneNumber)
        {
            try
            {
             
                var response = await HttpRequestFactory.Get($"{_balanceUrl}?PhoneNumber={phoneNumber}&PolicyNumber={policyNumber}");
                
 



                if (response.StatusCode == HttpStatusCode.OK)
                {

                    var test = await response.Content.ReadAsStringAsync();

                  //  var result = await response.ContentAsTypeAsync<CustomerBalanceDto>();
                    CustomerBalanceDto result = JsonConvert.DeserializeObject<CustomerBalanceDto>(test);
                    return new ProcessResponse
                    {
                        IsErrorOccurred = false,
                        Result = result
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
                logWriter.Exception(ex);

                return new ProcessResponse
                {
                    IsErrorOccurred = true,
                    ResponseCode = "APP_ERROR"
                };
            }

        }
    }
}
