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

namespace Ussd.App.BLL.Services.Products
{
    public class ProductsService : IProductsService
    {

        private readonly string _productsUrl;
        private readonly string _savingsProductsUrl;
        private readonly ILogger<ProductsService> _logger;

        public ProductsService(string productsUrl, ILogger<ProductsService> logger, string savingsProductsUrl)
        {
            _productsUrl = productsUrl;
            _logger = logger;
            _savingsProductsUrl = savingsProductsUrl;
        }


        public async Task<ProcessResponse> GetProducts(bool isGeneralEnquiry)
        {

            _logger.LogDebug("Get all Products");
            try
            {
                var response = await HttpRequestFactory.Get($"{_productsUrl}/GetProducts?isGeneralEnquiry={isGeneralEnquiry}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = await response.ContentAsTypeAsync<List<ProductDto>>();
                    result = result.Where(u => u.IsActive).OrderBy(x => x.ProductId).ToList();

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

        public async Task<ProcessResponse> GetProductsByCategory(int productCategoryId)
        {
            _logger.LogDebug("Get Products by Category Id");
            try
            {
                var response = await HttpRequestFactory.Get(_savingsProductsUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = await response.ContentAsTypeAsync<List<ProductDto>>();
                    result = result.Where(u => u.IsActive).OrderBy(x => x.ProductId).ToList();

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
