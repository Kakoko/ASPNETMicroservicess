﻿using AngleDimension.Mno.DataModels;
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
using Ussd.App.BLL.StateMachines.ServiceType;
using Ussd.App.BLL.StateMachines.Withdraw.Success;

namespace Ussd.App.BLL.StateMachines.Error
{
    public class GeneralErrorExitState : MenuState
    {

        public GeneralErrorExitState(MenuState previousState) : base(previousState)
        {

        }

        public GeneralErrorExitState(UssdMenu<LanguageDto> handle, LanguageDto currentLanguage, IDictionary<string, object> valueStash)
            : base(handle, currentLanguage, valueStash)
        {

        }

        public override async ValueTask<UssdResponseDTO> ProccessRequest(UssdRequestDTO request)
        {
            var errorState = new GeneralErrorEntryState(this)
            {
                ErrorMessage = CurrentLanguage.ErrorMessage
            };
            MenuState menuState = errorState;


            if (request.Message.Equals(MenuConstants.Zero))
            {
                menuState = new ChooseServiceEntryState(this);
            }


            Handle.CurrentState = menuState;
            Handle.CurrentState.ServiceProvider = ServiceProvider;
            return await menuState.ProccessRequest(request);
        }
    }
}