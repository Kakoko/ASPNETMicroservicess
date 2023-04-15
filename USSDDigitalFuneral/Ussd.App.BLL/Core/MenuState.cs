using AngleDimension.NetCore.Ussd.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Core
{
    public class MenuState : MenuStateBase<LanguageDto>
    {
        public MenuState(MenuStateBase<LanguageDto> previousState) : base(previousState)
        {
        }

        public MenuState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash) : base(handle, currentLanguage, valueStash)
        {
        }
    }
}
