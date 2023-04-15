using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ussd.App.BLL.Helper;

namespace Ussd.App.BLL.Services.Data
{
    public interface IStaticDataStoreFactory
    {
        IReadOnlyList<StaticDataStore> CreateMenuProducts();
        IReadOnlyList<StaticDataStore> CreateMenuSubProducts();
    }
}
