using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.ComonObjects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc cref="IBitfinexClientSpotApi" />
    public class BitfinexClientSpotApi : RestApiClient, IBitfinexClientSpotApi, ISpotClient
    {
        #region fields
        internal string? AffiliateCode { get; set; }

        private readonly BitfinexClient _baseClient;
        private readonly Log _log;
        private readonly BitfinexClientOptions _options;

        internal static TimeSpan TimeOffset;
        internal static SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);
        internal static DateTime LastTimeSync;

        internal static TimeSyncState TimeSyncState = new TimeSyncState();
        #endregion

        /// <inheritdoc />
        public string ExchangeName => "Bitfinex";

        #region Api clients
        /// <inheritdoc />
        public IBitfinexClientSpotApiAccount Account { get; }
        /// <inheritdoc />
        public IBitfinexClientSpotApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public IBitfinexClientSpotApiTrading Trading { get; }
        #endregion

        /// <summary>
        /// Event triggered when an order is placed via this client
        /// </summary>
        public event Action<OrderId>? OnOrderPlaced;
        /// <summary>
        /// Event triggered when an order is canceled via this client
        /// </summary>
        public event Action<OrderId>? OnOrderCanceled;

        #region ctor

        internal BitfinexClientSpotApi(Log log, BitfinexClient baseClient, BitfinexClientOptions options) :
            base(options, options.SpotApiOptions)
        {
            _baseClient = baseClient;
            _log = log;
            _options = options;

            Account = new BitfinexClientSpotApiAccount(this);
            ExchangeData = new BitfinexClientSpotApiExchangeData(this);
            Trading = new BitfinexClientSpotApiTrading(this);

            AffiliateCode = options.AffiliateCode;
        }

        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, _options.NonceProvider ?? new BitfinexNonceProvider());

        #region common interface

        /// <summary>
        /// Get the name of a symbol for Bitfinex based on the base and quote asset
        /// </summary>
        /// <param name="baseAsset"></param>
        /// <param name="quoteAsset"></param>
        /// <returns></returns>
        public string GetSymbolName(string baseAsset, string quoteAsset) =>
            "t" + (baseAsset + quoteAsset).ToUpper(CultureInfo.InvariantCulture);

        internal void InvokeOrderPlaced(OrderId id)
        {
            OnOrderPlaced?.Invoke(id);
        }

        internal void InvokeOrderCanceled(OrderId id)
        {
            OnOrderCanceled?.Invoke(id);
        }

        private static KlineInterval GetTimeFrameFromTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.FromMinutes(1)) return KlineInterval.OneMinute;
            if (timeSpan == TimeSpan.FromMinutes(5)) return KlineInterval.FiveMinutes;
            if (timeSpan == TimeSpan.FromMinutes(15)) return KlineInterval.FifteenMinutes;
            if (timeSpan == TimeSpan.FromMinutes(30)) return KlineInterval.ThirtyMinutes;
            if (timeSpan == TimeSpan.FromHours(1)) return KlineInterval.OneHour;
            if (timeSpan == TimeSpan.FromHours(3)) return KlineInterval.ThreeHours;
            if (timeSpan == TimeSpan.FromHours(6)) return KlineInterval.SixHours;
            if (timeSpan == TimeSpan.FromHours(12)) return KlineInterval.TwelveHours;
            if (timeSpan == TimeSpan.FromDays(1)) return KlineInterval.OneDay;
            if (timeSpan == TimeSpan.FromDays(7)) return KlineInterval.SevenDays;
            if (timeSpan == TimeSpan.FromDays(14)) return KlineInterval.FourteenDays;
            if (timeSpan == TimeSpan.FromDays(30) || timeSpan == TimeSpan.FromDays(31)) return KlineInterval.OneMonth;

            throw new ArgumentException("Unsupported timespan for Bitfinex Klines, check supported intervals using Bitfinex.Net.Objects.TimeFrame");
        }

        private static Enums.OrderSide GetOrderSide(CryptoExchange.Net.ComonObjects.OrderSide side)
        {
            if (side == CryptoExchange.Net.ComonObjects.OrderSide.Sell) return Enums.OrderSide.Sell;
            if (side == CryptoExchange.Net.ComonObjects.OrderSide.Buy) return Enums.OrderSide.Buy;

            throw new ArgumentException("Unsupported order side for Bitfinex order: " + side);
        }

        private static Enums.OrderType GetOrderType(CryptoExchange.Net.ComonObjects.OrderType type)
        {
            if (type == CryptoExchange.Net.ComonObjects.OrderType.Limit) return Enums.OrderType.ExchangeLimit;
            if (type == CryptoExchange.Net.ComonObjects.OrderType.Market) return Enums.OrderType.ExchangeMarket;

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

        /// <inheritdoc />
        protected override Task<WebCallResult<DateTime>> GetServerTimestampAsync()
            => Task.FromResult(new WebCallResult<DateTime>(null, null, DateTime.UtcNow, null));

        /// <inheritdoc />
        protected override TimeSyncInfo GetTimeSyncInfo()
            => new TimeSyncInfo(_log, _options.SpotApiOptions.AutoTimestamp, TimeSyncState);

        /// <inheritdoc />
        public override TimeSpan GetTimeOffset()
            => TimeSyncState.TimeOffset;

        /// <inheritdoc />
        public ISpotClient ComonSpotClient => this;

        async Task<WebCallResult<OrderId>> ISpotClient.PlaceOrderAsync(string symbol, CryptoExchange.Net.ComonObjects.OrderSide side, CryptoExchange.Net.ComonObjects.OrderType type, decimal quantity, decimal? price, string? accountId)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.PlaceOrderAsync), nameof(symbol));

            var result = await Trading.PlaceOrderAsync(symbol, GetOrderSide(side), GetOrderType(type), quantity, price ?? 0).ConfigureAwait(false);
            if (!result)
                return result.As<OrderId>(null);

            return result.As(new OrderId
            {
                SourceObject = result.Data,
                Id = result.Data.Id.ToString(CultureInfo.InvariantCulture)
            });
        }

        async Task<WebCallResult<Order>> IBaseRestClient.GetOrderAsync(string orderId, string? symbol)
        {
            if (!long.TryParse(orderId, out var id))
                throw new ArgumentException($"Invalid orderId provided for Bitfinex {nameof(ISpotClient.GetOrderAsync)}", nameof(orderId));

            var result = await Trading.GetOrderAsync(id).ConfigureAwait(false);
            if (!result)
                return result.As<Order>(null);

            return result.As(new Order
            {
                SourceObject = result.Data,
                Id = result.Data.Id.ToString(CultureInfo.InvariantCulture),
                Symbol = result.Data.Symbol,
                Timestamp = result.Data.Timestamp,
                Price = result.Data.Price,
                Quantity = result.Data.Quantity,
                QuantityFilled = result.Data.QuantityFilled,                
                Side = result.Data.Side == Enums.OrderSide.Buy ? CryptoExchange.Net.ComonObjects.OrderSide.Buy: CryptoExchange.Net.ComonObjects.OrderSide.Sell,
                Status = result.Data.Canceled ? CryptoExchange.Net.ComonObjects.OrderStatus.Canceled: result.Data.QuantityRemaining == 0 ? CryptoExchange.Net.ComonObjects.OrderStatus.Filled: CryptoExchange.Net.ComonObjects.OrderStatus.Active,
                Type = result.Data.Type == Enums.OrderTypeV1.ExchangeLimit ? CryptoExchange.Net.ComonObjects.OrderType.Limit : result.Data.Type == Enums.OrderTypeV1.ExchangeMarket ? CryptoExchange.Net.ComonObjects.OrderType.Market: CryptoExchange.Net.ComonObjects.OrderType.Other
            });
        }

        async Task<WebCallResult<IEnumerable<UserTrade>>> IBaseRestClient.GetOrderTradesAsync(string orderId, string? symbol)
        {
            if (!long.TryParse(orderId, out var id))
                throw new ArgumentException($"Invalid orderId provided for Bitfinex {nameof(ISpotClient.GetOrderAsync)}", nameof(orderId));

            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.GetOrderTradesAsync), nameof(symbol));

            var result = await Trading.GetOrderTradesAsync(symbol!, id).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<UserTrade>>(null);

            return result.As(result.Data.Select(d => new UserTrade
            {
                SourceObject = d,
                Id = d.Id.ToString(CultureInfo.InvariantCulture),
                OrderId = d.OrderId.ToString(CultureInfo.InvariantCulture),
                Price = d.Price,
                Quantity = d.Quantity,
                Symbol = d.Symbol,
                Fee = d.Fee,
                FeeAsset = d.FeeAsset,
                Timestamp = d.Timestamp
            }));
        }

        async Task<WebCallResult<IEnumerable<Order>>> IBaseRestClient.GetOpenOrdersAsync(string? symbol)
        {
            var result = await Trading.GetOpenOrdersAsync().ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<Order>>(null);

            return result.As(result.Data.Select(o =>
                new Order
                {
                    SourceObject = o,
                    Id = o.Id.ToString(CultureInfo.InvariantCulture),
                    Symbol = o.Symbol,
                    Timestamp = o.CreateTime,
                    Price = o.Price,
                    Quantity = o.Quantity,
                    QuantityFilled = o.Quantity - o.QuantityRemaining,
                    Side = o.Side == Enums.OrderSide.Buy ? CryptoExchange.Net.ComonObjects.OrderSide.Buy : CryptoExchange.Net.ComonObjects.OrderSide.Sell,
                    Status = o.Status == Enums.OrderStatus.Canceled ? CryptoExchange.Net.ComonObjects.OrderStatus.Canceled: o.Status == Enums.OrderStatus.Executed? CryptoExchange.Net.ComonObjects.OrderStatus.Filled: CryptoExchange.Net.ComonObjects.OrderStatus.Active,
                    Type = o.Type == Enums.OrderType.ExchangeLimit ? CryptoExchange.Net.ComonObjects.OrderType.Limit: o.Type == Enums.OrderType.ExchangeMarket ? CryptoExchange.Net.ComonObjects.OrderType.Market: CryptoExchange.Net.ComonObjects.OrderType.Other
                }
            ));
        }

        async Task<WebCallResult<IEnumerable<Order>>> IBaseRestClient.GetClosedOrdersAsync(string? symbol)
        {
            var result = await Trading.GetClosedOrdersAsync(symbol).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<Order>>(null);

            return result.As(result.Data.Select(o =>
                new Order
                {
                    SourceObject = o,
                    Id = o.Id.ToString(CultureInfo.InvariantCulture),
                    Symbol = o.Symbol,
                    Timestamp = o.CreateTime,
                    Price = o.Price,
                    Quantity = o.Quantity,
                    QuantityFilled = o.Quantity - o.QuantityRemaining,
                    Side = o.Side == Enums.OrderSide.Buy ? CryptoExchange.Net.ComonObjects.OrderSide.Buy : CryptoExchange.Net.ComonObjects.OrderSide.Sell,
                    Status = o.Status == Enums.OrderStatus.Canceled ? CryptoExchange.Net.ComonObjects.OrderStatus.Canceled : o.Status == Enums.OrderStatus.Executed ? CryptoExchange.Net.ComonObjects.OrderStatus.Filled : CryptoExchange.Net.ComonObjects.OrderStatus.Active,
                    Type = o.Type == Enums.OrderType.ExchangeLimit ? CryptoExchange.Net.ComonObjects.OrderType.Limit : o.Type == Enums.OrderType.ExchangeMarket ? CryptoExchange.Net.ComonObjects.OrderType.Market : CryptoExchange.Net.ComonObjects.OrderType.Other
                }
            ));
        }

        async Task<WebCallResult<OrderId>> IBaseRestClient.CancelOrderAsync(string orderId, string? symbol)
        {
            if (!long.TryParse(orderId, out var id))
                throw new ArgumentException($"Invalid orderId provided for Bitfinex {nameof(ISpotClient.GetOrderAsync)}", nameof(orderId));

            var result = await Trading.CancelOrderAsync(id).ConfigureAwait(false);
            if (!result)
                return result.As<OrderId>(null);

            return result.As(new OrderId
            {
                SourceObject = result.Data,
                Id = result.Data.Id.ToString(CultureInfo.InvariantCulture)
            });
        }

        async Task<WebCallResult<IEnumerable<Symbol>>> IBaseRestClient.GetSymbolsAsync()
        {
            var symbols = await ExchangeData.GetSymbolDetailsAsync().ConfigureAwait(false);
            if (!symbols)
                return symbols.As<IEnumerable<Symbol>>(null);

            return symbols.As(symbols.Data.Select(s =>
                new Symbol
                {
                    SourceObject = s,
                    Name = s.Symbol,
                    PriceDecimals = s.PricePrecision,
                    MinTradeQuantity = s.MinimumOrderQuantity
                }));
        }

        async Task<WebCallResult<Ticker>> IBaseRestClient.GetTickerAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.GetTickerAsync), nameof(symbol));

            var tickersResult = await ExchangeData.GetTickerAsync(symbol).ConfigureAwait(false);
            if (!tickersResult)
                return tickersResult.As<Ticker>(null);

            return tickersResult.As(new Ticker
            {
                SourceObject = tickersResult.Data,
                HighPrice = tickersResult.Data.HighPrice,
                LowPrice = tickersResult.Data.LowPrice,
                LastPrice = tickersResult.Data.LastPrice,
                Price24H = tickersResult.Data.LastPrice - tickersResult.Data.DailyChange,
                Symbol = tickersResult.Data.Symbol,
                Volume = tickersResult.Data.Volume
            });
        }

        async Task<WebCallResult<IEnumerable<Ticker>>> IBaseRestClient.GetTickersAsync()
        {
            var tickersResult = await ExchangeData.GetTickersAsync().ConfigureAwait(false);
            if (!tickersResult)
                return tickersResult.As<IEnumerable<Ticker>>(null);

            return tickersResult.As(tickersResult.Data.Select(t =>
            new Ticker
            {
                SourceObject = t,
                HighPrice = t.HighPrice,
                LowPrice = t.LowPrice,
                LastPrice = t.LastPrice,
                Price24H = t.LastPrice - t.DailyChange,
                Symbol = t.Symbol,
                Volume = t.Volume
            }));
        }

        async Task<WebCallResult<IEnumerable<Kline>>> IBaseRestClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime, DateTime? endTime, int? limit)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.GetKlinesAsync), nameof(symbol));

            var klines = await ExchangeData.GetKlinesAsync(symbol, GetTimeFrameFromTimeSpan(timespan), startTime: startTime, endTime: endTime, limit: limit).ConfigureAwait(false);
            if (!klines)
                return klines.As<IEnumerable<Kline>>(null);

            return klines.As(klines.Data.Select(k =>
            new Kline
            {
                SourceObject = k,
                OpenPrice = k.OpenPrice,
                OpenTime = k.OpenTime,
                ClosePrice = k.ClosePrice,
                HighPrice = k.HighPrice,
                LowPrice = k.LowPrice,
                Volume = k.Volume
            }));
        }

        async Task<WebCallResult<OrderBook>> IBaseRestClient.GetOrderBookAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.GetOrderAsync), nameof(symbol));

            var orderBookResult = await ExchangeData.GetOrderBookAsync(symbol, Precision.PrecisionLevel0).ConfigureAwait(false);
            if (!orderBookResult)
                return orderBookResult.As<OrderBook>(null);

            return orderBookResult.As(new OrderBook
            {
                SourceObject = orderBookResult.Data,
                Asks = orderBookResult.Data.Asks.Select(a => new OrderBookEntry() { Price = a.Price, Quantity = a.Quantity }),
                Bids = orderBookResult.Data.Bids.Select(b => new OrderBookEntry() { Price = b.Price, Quantity = b.Quantity }),
            });
        }

        async Task<WebCallResult<IEnumerable<Trade>>> IBaseRestClient.GetRecentTradesAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.GetRecentTradesAsync), nameof(symbol));

            var tradesResult = await ExchangeData.GetTradeHistoryAsync(symbol).ConfigureAwait(false);
            if (!tradesResult)
                return tradesResult.As<IEnumerable<Trade>>(null);

            return tradesResult.As(tradesResult.Data.Select(t => new Trade
            {
                SourceObject = t,
                Price = t.Price,
                Quantity = t.Quantity,
                Symbol = symbol,
                Timestamp = t.Timestamp
            }));
        }

        async Task<WebCallResult<IEnumerable<Balance>>> IBaseRestClient.GetBalancesAsync(string? accountId)
        {
            var result = await Account.GetBalancesAsync().ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<Balance>>(null);

            return result.As(result.Data.Select(b => new Balance
            {
                SourceObject = b,
                Asset = b.Asset,
                Available = b.Available,
                Total = b.Total
            }));
        }
        #endregion
    }
}
