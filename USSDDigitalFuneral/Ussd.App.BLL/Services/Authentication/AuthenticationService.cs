using AngleDimension.NetCore.Logging;
using AngleDimension.Standard.Core.General;
using AngleDimension.Standard.Http.HttpServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.Services.Products;

namespace Ussd.App.BLL.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly string _authenticationUrl;
        private readonly ILogger<AuthenticationService> _logger;



        public AuthenticationService(string authenticationUrl, ILogger<AuthenticationService> logger)
        {
            _authenticationUrl = authenticationUrl;
            _logger = logger;
        }

        public async Task<ProcessResponse> ChangePin(ChangePinDto changePinDto)
        {
            var changePin = new ChangePinDto
            {
                NewPin = changePinDto.NewPin,
                OldPin = changePinDto.OldPin,
                PhoneNumber = changePinDto.PhoneNumber,
            };

            try
            {

              
                var response = await HttpRequestFactory.Post($"{_authenticationUrl}/Authentication/ChangePin", changePin);


                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    var result = await response.ContentAsTypeAsync<AuthenticationResponseDto>();
                    return new ProcessResponse
                    {
                        IsErrorOccurred = false,
                        Result = result
                    };
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
               
                    return new ProcessResponse
                    {
                        IsErrorOccurred = true,
                        Result = false
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

        public async Task<ProcessResponse> CheckPhoneNumber(CheckNumberDto checkNumberDto)
        {

            var checkNumber = new CheckNumberDto
            {
                PhoneNumber = checkNumberDto.PhoneNumber
            };

            try
            {
                var response = await HttpRequestFactory.Post($"{_authenticationUrl}/Authentication/CheckPhoneNumber", checkNumber);

                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    var result = await response.ContentAsTypeAsync<AuthenticationResponseDto>();
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

        public async Task<ProcessResponse> RecoverPin(RecoverPinDto recoverPinDto)
        {
            var recoverPin = new RecoverPinDto
            {
                PartPolicyNumber = recoverPinDto.PartPolicyNumber,
                PhoneNumber = recoverPinDto.PhoneNumber
            };


            try
            {
                var response = await HttpRequestFactory.Post($"{_authenticationUrl}/Authentication/RecoverPin", recoverPin);

                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                  
                    return new ProcessResponse
                    {
                        IsErrorOccurred = false
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

        public async Task<ProcessResponse> ValidateCustomer(AuthenticationDto authenticationDto)
        {


            var authentication = new AuthenticationDto
            {
                PhoneNumber = authenticationDto.PhoneNumber,
                Pin = authenticationDto.Pin
            };

            try
            {
                var response = await HttpRequestFactory.Post($"{_authenticationUrl}/Authentication/AuthenticateCustomer", authentication);


                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    var result = await response.ContentAsTypeAsync<AuthenticationResponseDto>();
                    return new ProcessResponse
                    {
                        IsErrorOccurred = false,
                        Result = result,
                        Message = "Allowed"
                    };
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {

                    var result = await response.ContentAsTypeAsync<AuthenticationResponseDto>();

                    return new ProcessResponse
                    {
                        IsErrorOccurred = true,
                        Result = result,
                        Message = "ChangePin"
                    };
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                   
                    return new ProcessResponse
                    {
                        IsErrorOccurred = true,
                        Message = "WrongPin"
                        
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
    }
}
