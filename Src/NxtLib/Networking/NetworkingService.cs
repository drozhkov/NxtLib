﻿using System.Collections.Generic;
using System.Threading.Tasks;
using NxtLib.Internal;

namespace NxtLib.Networking
{
    public class NetworkingService : BaseService, INetworkingService
    {
        public NetworkingService(string baseAddress = DefaultBaseUrl)
            : base(new DateTimeConverter(), baseAddress)
        {
        }

        public NetworkingService(IDateTimeConverter dateTimeConverter) 
            : base(dateTimeConverter)
        {
        }

        public async Task<PeerReply> AddPeer(string peer)
        {
            var queryParameters = new Dictionary<string, string> {{"peer", peer}};
            return await Get<PeerReply>("addPeer", queryParameters);
        }

        public async Task<DoneReply> BlacklistPeer(string peer)
        {
            var queryParameters = new Dictionary<string, string> {{"peer", peer}};
            return await Get<DoneReply>("blacklistPeer", queryParameters);
        }

        public async Task<GetMyInfoReply> GetMyInfo()
        {
            return await Get<GetMyInfoReply>("getMyInfo");
        }

        public async Task<PeerReply> GetPeer(string peer)
        {
            var queryParameters = new Dictionary<string, string> {{"peer", peer}};
            return await Get<PeerReply>("getPeer", queryParameters);
        }

        public async Task<GetPeersReply> GetPeers(PeersLocator locator = null)
        {
            if (locator != null)
            {
                return await Get<GetPeersReply>("getPeers", locator.QueryParameters);
            }
            return await Get<GetPeersReply>("getPeers");
        }

        public async Task<GetPeersIncludeInfoReply> GetPeersIncludeInfo(PeersLocator locator = null)
        {
            var queryParameters = locator != null ? locator.QueryParameters : new Dictionary<string, string>();
            queryParameters.Add("includePeerInfo", true.ToString());
            return await Get<GetPeersIncludeInfoReply>("getPeers", queryParameters);
        }
    }
}