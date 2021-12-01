using Bitfinex.Net.Clients.Rest;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.General;
using Bitfinex.Net.Interfaces.Clients.Rest;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.ExchangeInterfaces;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients
{
    public class BitfinexClientGeneral : RestApiClient, IBitfinexClientGeneral
    {
        #region fields
        internal string? AffiliateCode { get; set; }

        private readonly BitfinexClientOptions _options;
        private readonly BitfinexClient _baseClient;
        #endregion

        #region Api clients
        public IBitfinexClientGeneralFunding Funding { get; }
        #endregion

        /// <summary>
        /// Event triggered when an order is placed via this client
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderPlaced;
        /// <summary>
        /// Event triggered when an order is canceled via this client
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderCanceled;

        #region ctor

        internal BitfinexClientGeneral(BitfinexClient baseClient, BitfinexClientOptions options) :
            base(options, options.SpotApiOptions)
        {
            _baseClient = baseClient;
            _options = options;

            Funding = new BitfinexClientGeneralFunding(this);

            AffiliateCode = options.AffiliateCode;
        }

        #endregion

        public override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, _options.NonceProvider ?? new BitfinexNonceProvider());

        internal Task<WebCallResult<T>> SendRequestAsync<T>(
            Uri uri,
            HttpMethod method,
            CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null,
            bool signed = false) where T : class
                => _baseClient.SendRequestAsync<T>(this, uri, method, cancellationToken, parameters, signed);

        internal Uri GetUrl(string endpoint, string version)
        {
            return new Uri(BaseAddress.AppendPath($"v{version}", endpoint));
        }
    }
}
