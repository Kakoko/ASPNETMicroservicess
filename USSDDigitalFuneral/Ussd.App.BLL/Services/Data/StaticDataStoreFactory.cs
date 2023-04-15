using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Helper;

namespace Ussd.App.BLL.Services.Data
{
    public static class StaticDataStoreFactory
    {
        private static readonly List<StaticDataStore> _menuProducts = new()
            {
                new StaticDataStore { ProductName = "Apply for products" , ProductCode = "A" },
                new StaticDataStore { ProductName = "View my portfolio", ProductCode = "V" },
                new StaticDataStore { ProductName = "Make a withdrawal", ProductCode = "M" },
                new StaticDataStore { ProductName = "Get in touch", ProductCode = "G" },
                new StaticDataStore { ProductName = "Change PIN", ProductCode = "C" }
            };

        private static readonly List<StaticDataStore> _menuPolicies = new()
            {
                new StaticDataStore { ProductName = "Tsogolo Savings Policy" },
                new StaticDataStore { ProductName = "Pensions" },
                new StaticDataStore { ProductName = "Green Life Policy" }
            };



        private static readonly List<StaticDataStore> _menuGeneralEnquiry = new()
            {
                new StaticDataStore { ProductName = "Funeral cover" },
                new StaticDataStore { ProductName = "Insure" },
                new StaticDataStore { ProductName = "Save and Invest" },
                new StaticDataStore { ProductName = "Claims" },
                new StaticDataStore { ProductName = "Other" }
            };

        public static IReadOnlyList<StaticDataStore> GetMainMenu()
        {
            return _menuProducts.AsReadOnly();
        }

        public static IReadOnlyList<StaticDataStore> GetPolicies()
        {
            return _menuPolicies.AsReadOnly();
        }

        public static IReadOnlyList<StaticDataStore> CreateGeneralEnquiryMenuProducts()
        {
            return _menuGeneralEnquiry.AsReadOnly();
        }

    }
}
