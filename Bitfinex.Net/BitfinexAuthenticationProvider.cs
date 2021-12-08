using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net
{
    internal class BitfinexAuthenticationProvider: AuthenticationProvider
    {
        private readonly INonceProvider _nonceProvider;

        public long GetNonce() => _nonceProvider.GetNonce();

        public BitfinexAuthenticationProvider(ApiCredentials credentials, INonceProvider? nonceProvider) : base(credentials)
        {
            if (credentials.Secret == null)
                throw new ArgumentException("ApiKey/Secret needed");

            _nonceProvider = nonceProvider ?? new BitfinexNonceProvider();
        }

        public override void AuthenticateUriRequest(RestApiClient apiClient, Uri uri, HttpMethod method, SortedDictionary<string, object> parameters, Dictionary<string, string> headers, bool auth, ArrayParametersSerialization arraySerialization)
        {
            // Only public endpoints are Uri requests, so no need to do anything here
            return;
        }

        public override void AuthenticateBodyRequest(RestApiClient apiClient, Uri uri, HttpMethod method, SortedDictionary<string, object> parameters, Dictionary<string, string> headers, bool auth, ArrayParametersSerialization arraySerialization)
        {
            if (!auth)
                return;

            if (uri.AbsolutePath.Contains("v1"))
            {
                parameters.Add("request", uri.AbsolutePath);
                parameters.Add("nonce", _nonceProvider.GetNonce().ToString());

                var signature = JsonConvert.SerializeObject(parameters);
                var payload = Convert.ToBase64String(Encoding.ASCII.GetBytes(signature));
                var signedData = Sign(payload);

                headers.Add("X-BFX-APIKEY", Credentials.Key!.GetString());
                headers.Add("X-BFX-PAYLOAD", payload);
                headers.Add("X-BFX-SIGNATURE", signedData.ToLower(CultureInfo.InvariantCulture));
            }
            else if (uri.AbsolutePath.Contains("v2"))
            {
                var json = JsonConvert.SerializeObject(parameters);
                var n = _nonceProvider.GetNonce().ToString();
                var signature = $"/api{uri.AbsolutePath}{n}{json}";
                var signedData = SignHMACSHA384(signature);

                headers.Add("bfx-apikey", Credentials.Key!.GetString());
                headers.Add("bfx-nonce", n);
                headers.Add("bfx-signature", signedData.ToLower(CultureInfo.InvariantCulture));
            }
        }

        public override string Sign(string toSign) => SignHMACSHA384(toSign);
    }
}
