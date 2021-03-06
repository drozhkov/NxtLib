﻿using System.Collections.Generic;
using Newtonsoft.Json;
using NxtLib.Internal;

namespace NxtLib.Accounts
{
    public class AccountBlockIdsReply : BaseReply
    {
        [JsonConverter(typeof(StringToIntegralTypeConverter))]
        public List<ulong> BlockIds { get; set; }
    }
}