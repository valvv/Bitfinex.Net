using System;
using System.Collections.Generic;
using System.Net.Http;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Interfaces.Clients.Socket;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Options for the BitfinexClient
    /// </summary>
    public class BitfinexClientOptions : RestClientOptions
    {
        /// <summary>
        /// Default options for the client
        /// </summary>
        public static BitfinexClientOptions Default { get; set; } = new BitfinexClientOptions()
        {
            BaseAddress = "https://api.bitfinex.com"
        };

        /// <summary>
        /// Default affiliate code to use when placing orders
        /// </summary>
        public string? AffiliateCode { get; set; } = "kCCe-CNBO";

        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public BitfinexClientOptions()
        {
            if (Default == null)
                return;

            Copy(this, Default);
        }

        /// <summary>
        /// Copy the values of the def to the input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="def"></param>
        public new void Copy<T>(T input, T def) where T : BitfinexClientOptions
        {
            base.Copy(input, def);

            input.AffiliateCode = def.AffiliateCode;
            input.NonceProvider = def.NonceProvider;
        }
    }

    /// <summary>
    /// Options for the BitfinexSocketClient
    /// </summary>
    public class BitfinexSocketClientOptions: SocketClientOptions
    {
        /// <summary>
        /// Default options for the client
        /// </summary>
        public static BitfinexSocketClientOptions Default { get; set; } = new BitfinexSocketClientOptions()
        {
            BaseAddress = "wss://api.bitfinex.com/ws/2",
            SocketSubscriptionsCombineTarget = 10,
            SocketNoDataTimeout = TimeSpan.FromSeconds(30)
        };

        /// <summary>
        /// Default affiliate code to use when placing orders
        /// </summary>
        public string? AffiliateCode { get; set; } = "kCCe-CNBO";

        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public BitfinexSocketClientOptions()
        {
            if (Default == null)
                return;

            Copy(this, Default);
        }

        /// <summary>
        /// Copy the values of the def to the input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="def"></param>
        public new void Copy<T>(T input, T def) where T : BitfinexSocketClientOptions
        {
            base.Copy(input, def);

            input.AffiliateCode = def.AffiliateCode;
            input.NonceProvider = def.NonceProvider;
        }
    }

    /// <summary>
    /// Options for the BitfinexSymbolOrderBook
    /// </summary>
    public class BitfinexOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.
        /// </summary>
        public IBitfinexSocketClient? SocketClient { get; }

        /// <summary>
        /// </summary>
        /// <param name="client">The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.</param>
        public BitfinexOrderBookOptions(IBitfinexSocketClient? client = null)
        {
            SocketClient = client;
        }
    }
}
