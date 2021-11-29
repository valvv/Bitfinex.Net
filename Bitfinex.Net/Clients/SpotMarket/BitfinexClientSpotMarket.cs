using Bitfinex.Net.Clients.Rest;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.Rest;
using Bitfinex.Net.Interfaces.Clients.Spot;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.ExchangeInterfaces;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients
{
    public class BitfinexClientSpotMarket: RestSubClient, IBitfinexClientSpotMarket, IExchangeClient
    {
        #region fields
        internal string? AffiliateCode { get; set; }
        private readonly BitfinexClient _baseClient;
        #endregion

        #region Subclient
        public IBitfinexClientSpotMarketAccount Account { get; }
        public IBitfinexClientSpotMarketExchangeData ExchangeData { get; }
        public IBitfinexClientSpotMarketTrading Trading { get; }
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

        internal BitfinexClientSpotMarket(BitfinexClient baseClient,  BitfinexClientOptions options): 
            base(options.OptionsSpot, options.OptionsSpot.ApiCredentials == null ? null : new BitfinexAuthenticationProvider(options.OptionsSpot.ApiCredentials, options.NonceProvider))
        {
            _baseClient = baseClient;

            Account = new BitfinexClientSpotMarketAccount(this);
            ExchangeData = new BitfinexClientSpotMarketExchangeData(this);
            Trading = new BitfinexClientSpotMarketTrading(this);

            AffiliateCode = options.AffiliateCode;
        }

        #endregion

        #region common interface

        /// <summary>
        /// Get the name of a symbol for Bitfinex based on the base and quote asset
        /// </summary>
        /// <param name="baseAsset"></param>
        /// <param name="quoteAsset"></param>
        /// <returns></returns>
        public string GetSymbolName(string baseAsset, string quoteAsset) =>
            "t" + (baseAsset + quoteAsset).ToUpper(CultureInfo.InvariantCulture);

#pragma warning disable 1066
        async Task<WebCallResult<IEnumerable<ICommonSymbol>>> IExchangeClient.GetSymbolsAsync()
        {
            var symbols = await ExchangeData.GetSymbolDetailsAsync().ConfigureAwait(false);
            return symbols.As<IEnumerable<ICommonSymbol>>(symbols.Data);
        }

        async Task<WebCallResult<ICommonTicker>> IExchangeClient.GetTickerAsync(string symbol)
        {
            var tickersResult = await ExchangeData.GetTickerAsync(symbol).ConfigureAwait(false);
            return tickersResult.As<ICommonTicker>(tickersResult.Data?.FirstOrDefault());
        }

        async Task<WebCallResult<IEnumerable<ICommonTicker>>> IExchangeClient.GetTickersAsync()
        {
            var tickersResult = await ExchangeData.GetTickersAsync().ConfigureAwait(false);
            return tickersResult.As<IEnumerable<ICommonTicker>>(tickersResult.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonRecentTrade>>> IExchangeClient.GetRecentTradesAsync(string symbol)
        {
            var tradesResult = await ExchangeData.GetTradeHistoryAsync(symbol).ConfigureAwait(false);
            return tradesResult.As<IEnumerable<ICommonRecentTrade>>(tradesResult.Data);
        }

        async Task<WebCallResult<ICommonOrderBook>> IExchangeClient.GetOrderBookAsync(string symbol)
        {
            var orderBookResult = await ExchangeData.GetOrderBookAsync(symbol, Precision.PrecisionLevel0).ConfigureAwait(false);
            if (!orderBookResult)
                return WebCallResult<ICommonOrderBook>.CreateErrorResult(orderBookResult.ResponseStatusCode, orderBookResult.ResponseHeaders, orderBookResult.Error!);

            var isFunding = symbol.StartsWith("f");
            var result = new BitfinexOrderBook
            {
                Asks = orderBookResult.Data.Where(d => isFunding ? d.Quantity < 0 : d.Quantity > 0),
                Bids = orderBookResult.Data.Where(d => isFunding ? d.Quantity > 0 : d.Quantity < 0)
            };

            return orderBookResult.As<ICommonOrderBook>(result);
        }

        async Task<WebCallResult<IEnumerable<ICommonKline>>> IExchangeClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var klines = await ExchangeData.GetKlinesAsync(symbol, GetTimeFrameFromTimeSpan(timespan), startTime: startTime, endTime: endTime, limit: limit).ConfigureAwait(false);
            return klines.As<IEnumerable<ICommonKline>>(klines.Data);
        }

        async Task<WebCallResult<ICommonOrder>> IExchangeClient.GetOrderAsync(string orderId, string? symbol)
        {
            var result = await Trading.GetOrderAsync(long.Parse(orderId)).ConfigureAwait(false);
            return result.As<ICommonOrder>(result.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonTrade>>> IExchangeClient.GetTradesAsync(string orderId, string? symbol = null)
        {
            if (string.IsNullOrEmpty(symbol))
                return WebCallResult<IEnumerable<ICommonTrade>>.CreateErrorResult(new ArgumentError(nameof(symbol) + " required for Bitfinex " + nameof(IExchangeClient.GetTradesAsync)));

            var result = await Trading.GetOrderTradesAsync(symbol!, long.Parse(orderId)).ConfigureAwait(false);
            return result.As<IEnumerable<ICommonTrade>>(result.Data);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.PlaceOrderAsync(string symbol, IExchangeClient.OrderSide side, IExchangeClient.OrderType type, decimal quantity, decimal? price = null, string? accountId = null)
        {
            var result = await Trading.PlaceOrderAsync(symbol, GetOrderSide(side), GetOrderType(type), quantity, price ?? 0).ConfigureAwait(false);
            return result.As<ICommonOrderId>(result.Data?.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetOpenOrdersAsync(string? symbol)
        {
            var result = await Trading.GetOpenOrdersAsync().ConfigureAwait(false);
            return result.As<IEnumerable<ICommonOrder>>(result.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetClosedOrdersAsync(string? symbol)
        {
            var result = await Trading.GetOrdersAsync(symbol).ConfigureAwait(false);
            return result.As<IEnumerable<ICommonOrder>>(result.Data);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.CancelOrderAsync(string orderId, string? symbol)
        {
            var result = await Trading.CancelOrderAsync(long.Parse(orderId)).ConfigureAwait(false);
            return result.As<ICommonOrderId>(result.Data?.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonBalance>>> IExchangeClient.GetBalancesAsync(string? accountId = null)
        {
            var result = await Account.GetBalancesAsync().ConfigureAwait(false);
            return result.As<IEnumerable<ICommonBalance>>(result.Data.Where(d => d.Type == WalletType.Exchange));
        }
#pragma warning restore 1066

        internal void InvokeOrderPlaced(ICommonOrderId id)
        {
            OnOrderPlaced?.Invoke(id);
        }

        internal void InvokeOrderCanceled(ICommonOrderId id)
        {
            OnOrderCanceled?.Invoke(id);
        }

        private static KlineInterval GetTimeFrameFromTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.FromMinutes(1)) return KlineInterval.OneMinute;
            if (timeSpan == TimeSpan.FromMinutes(5)) return KlineInterval.FiveMinute;
            if (timeSpan == TimeSpan.FromMinutes(15)) return KlineInterval.FifteenMinute;
            if (timeSpan == TimeSpan.FromMinutes(30)) return KlineInterval.ThirtyMinute;
            if (timeSpan == TimeSpan.FromHours(1)) return KlineInterval.OneHour;
            if (timeSpan == TimeSpan.FromHours(3)) return KlineInterval.ThreeHour;
            if (timeSpan == TimeSpan.FromHours(6)) return KlineInterval.SixHour;
            if (timeSpan == TimeSpan.FromHours(12)) return KlineInterval.TwelveHour;
            if (timeSpan == TimeSpan.FromDays(1)) return KlineInterval.OneDay;
            if (timeSpan == TimeSpan.FromDays(7)) return KlineInterval.SevenDay;
            if (timeSpan == TimeSpan.FromDays(14)) return KlineInterval.FourteenDay;
            if (timeSpan == TimeSpan.FromDays(30) || timeSpan == TimeSpan.FromDays(31)) return KlineInterval.OneMonth;

            throw new ArgumentException("Unsupported timespan for Bitfinex Klines, check supported intervals using Bitfinex.Net.Objects.TimeFrame");
        }

        private static OrderSide GetOrderSide(IExchangeClient.OrderSide side)
        {
            if (side == IExchangeClient.OrderSide.Sell) return OrderSide.Sell;
            if (side == IExchangeClient.OrderSide.Buy) return OrderSide.Buy;

            throw new ArgumentException("Unsupported order side for Bitfinex order: " + side);
        }

        private static OrderType GetOrderType(IExchangeClient.OrderType type)
        {
            if (type == IExchangeClient.OrderType.Limit) return OrderType.ExchangeLimit;
            if (type == IExchangeClient.OrderType.Market) return OrderType.ExchangeMarket;

            throw new ArgumentException("Unsupported order type for Bitfinex order: " + type);
        }

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
        #endregion
    }
}
