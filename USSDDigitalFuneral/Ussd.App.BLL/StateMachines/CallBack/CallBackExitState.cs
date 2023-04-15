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
using Ussd.App.BLL.Helper;
using Ussd.App.BLL.Models;
using Ussd.App.BLL.StateMachines.FullName;
using Ussd.App.BLL.StateMachines.Product;
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.Shared.ExitApp;

namespace Ussd.App.BLL.StateMachines.CallBack
{
    public class CallBackExitState : MenuState
    {

        public CallBackExitState(MenuState previousState) : base(previousState)
        {

        }

        public CallBackExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new CallBackEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;
          

            if (request.Message.Equals(MenuConstants.One))
            {
                menuState = new ChooseServiceEntryState(this);
            }
            if (request.Message.Equals(MenuConstants.Two))
            {
                menuState = new ExitMenuState(this);
            }


            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}
