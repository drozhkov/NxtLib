﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NxtLib.Internal;
using NxtLib.Local;

namespace NxtLib.Forging
{
    public class ForgingService : BaseService, IForgingService
    {
        public ForgingService(string baseAddress = Constants.DefaultNxtUrl)
            : base(baseAddress)
        {
        }

        public async Task<GetForgingReply> GetForging(SecretPhraseOrAdminPassword secretPhraseOrAdminPassword)
        {
            var queryParameters = secretPhraseOrAdminPassword.QueryParameters;
            return await Post<GetForgingReply>("getForging", queryParameters);
        }

        public async Task<GetNextBlockGeneratorsReply> GetNextBlockGenerators(int? limit = default(int?))
        {
            var queryParameters = new Dictionary<string, string>();
            queryParameters.AddIfHasValue(Parameters.Limit, limit);
            return await Get<GetNextBlockGeneratorsReply>("getNextBlockGenerators", queryParameters);
        }

        public async Task<TransactionCreatedReply> LeaseBalance(int period, Account recipient,
            CreateTransactionParameters parameters)
        {
            var queryParameters = new Dictionary<string, string>
            {
                {Parameters.Period, period.ToString()},
                {Parameters.Recipient, recipient.AccountId.ToString()}
            };
            parameters.AppendToQueryParameters(queryParameters);
            return await Post<TransactionCreatedReply>("leaseBalance", queryParameters);
        }

        public async Task<StartForgingReply> StartForging(string secretPhrase)
        {
            var queryParameters = new Dictionary<string, string> {{Parameters.SecretPhrase, secretPhrase}};
            return await Post<StartForgingReply>("startForging", queryParameters);
        }

        public async Task<StopForgingReply> StopForging(SecretPhraseOrAdminPassword secretPhraseOrAdminPassword)
        {
            var queryParameters = secretPhraseOrAdminPassword.QueryParameters;
            return await Post<StopForgingReply>("stopForging", queryParameters);
        }
    }
}