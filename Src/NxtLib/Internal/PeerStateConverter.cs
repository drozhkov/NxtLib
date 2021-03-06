﻿using System;
using Newtonsoft.Json;
using NxtLib.Networking;

namespace NxtLib.Internal
{
    internal class PeerStateConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if ((reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Integer) && objectType == typeof(PeerInfo.PeerState))
            {
                var stateInt = Convert.ToInt32(reader.Value.ToString());
                switch (stateInt)
                {
                    case 0:
                        return PeerInfo.PeerState.NonConnected;
                    case 1:
                        return PeerInfo.PeerState.Connected;
                    case 2:
                        return PeerInfo.PeerState.Disconnected;
                }
            }
            throw new NotSupportedException($"objectType {objectType} and TokenType {reader.TokenType} is not supported");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}