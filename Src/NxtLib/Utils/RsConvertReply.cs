﻿using Newtonsoft.Json;
using NxtLib.Internal;

namespace NxtLib.Utils
{
    public class RsConvertReply : BaseReply
    {
        [JsonConverter(typeof(StringToIntegralTypeConverter))]
        [JsonProperty(PropertyName = Parameters.Account)]
        public ulong AccountId { get; set; }
        public string AccountRs { get; set; }
    }
}