using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net.Interfaces.Clients.Rest
{
    /// <summary>
    /// Interface for the Bitfinex client
    /// </summary>
    public interface IBitfinexClient: IRestClient
    {
        public IBitfinexClientAccount Account { get; }
        public IBitfinexClientExchangeData ExchangeData { get; }
        public IBitfinexClientTrading Trading { get; }
        public IBitfinexClientFunding Funding { get; }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        /// <param name="nonceProvider">Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that</param>
        void SetApiCredentials(string apiKey, string apiSecret, INonceProvider? nonceProvider = null);
    }
}