﻿using Newtonsoft.Json;
using NxtLib.Internal;

namespace NxtLib.MonetarySystem
{
    public class GetMintingTargetReply : BaseReply
    {
        [JsonConverter(typeof(StringToIntegralTypeConverter))]
        public long Counter { get; set; }

        [JsonConverter(typeof(StringToIntegralTypeConverter))]
        [JsonProperty(PropertyName = Parameters.Currency)]
        public ulong CurrencyId { get; set; }

        [JsonConverter(typeof(StringToIntegralTypeConverter))]
        public int Difficulty { get; set; }
        public string TargetBytes { get; set; }
    }
}