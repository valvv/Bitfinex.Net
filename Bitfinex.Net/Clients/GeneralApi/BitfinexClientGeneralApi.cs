using Bitfinex.Net.Clients.Rest;
using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.ExchangeInterfaces;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.GeneralApi
{
    /// <inheritdoc cref="IBitfinexClientGeneralApi" />
    public class BitfinexClientGeneralApi : RestApiClient, IBitfinexClientGeneralApi
    {
        #region fields
        internal string? AffiliateCode { get; set; }

        private readonly BitfinexClientOptions _options;
        private readonly BitfinexClient _baseClient;
        #endregion

        #region Api clients
        /// <inheritdoc />
        public IBitfinexClientGeneralApiFunding Funding { get; }
        #endregion

        #region ctor

        internal BitfinexClientGeneralApi(BitfinexClient baseClient, BitfinexClientOptions options) :
            base(options, options.SpotApiOptions)
        {
            _baseClient = baseClient;
            _options = options;

            Funding = new BitfinexClientGeneralApiFunding(this);

            AffiliateCode = options.AffiliateCode;
        }

        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
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
