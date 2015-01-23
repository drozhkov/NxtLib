﻿using Newtonsoft.Json;

namespace NxtLib.DigitalGoodsStoreOperations
{
    public class Tag
    {
        public int InStockCount { get; set; }

        [JsonProperty(PropertyName = "tag")]
        public string TagName { get; set; }
        public int TotalCount { get; set; }
    }
}