﻿using CommunityToolkit.Mvvm.Messaging.Messages;
using SharedModel.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicketMobile.Models
{
    internal class ToLateChangeMessage : ValueChangedMessage<LateAndGoHomeEarlyResponse>
    {
        public ToLateChangeMessage(LateAndGoHomeEarlyResponse value) : base(value)
        {
        }
    }



}
