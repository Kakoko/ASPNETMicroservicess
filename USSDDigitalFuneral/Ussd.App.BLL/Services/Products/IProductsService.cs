using AngleDimension.Standard.Core.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Models;

namespace Ussd.App.BLL.Services.Products
{
    public interface IProductsService
    {
        Task<ProcessResponse> GetProducts(bool isGeneralEnquiry);
        Task<ProcessResponse> GetProductsByCategory(int productCategoryId);
    }
}
            