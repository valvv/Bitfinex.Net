using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.RestV1Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Interfaces;
using CryptoExchange.Net.ExchangeInterfaces;
using CryptoExchange.Net.Interfaces;
using Bitfinex.Net.Enums;

namespace Bitfinex.Net
{
    /// <summary>
    /// Client for the Bitfinex API
    /// </summary>
    public class BitfinexClient : RestClient, IBitfinexClient, IExchangeClient
    {
        #region fields

        private static BitfinexClientOptions defaultOptions = new BitfinexClientOptions();
        private static BitfinexClientOptions DefaultOptions => defaultOptions.Copy<BitfinexClientOptions>();

        private const string ApiVersion1 = "1";
        private const string ApiVersion2 = "2";

        private const string StatusEndpoint = "platform/status";
        private const string FundingBookEndpoint = "lendbook/{}";
        private const string LendsEndpoint = "lends/{}";
        private const string SymbolsEndpoint = "symbols";
        private const string SymbolDetailsEndpoint = "symbols_details";
        private const string CurrenciesEndpoint = "conf/pub:map:currency:label";
        private const string TickersEndpoint = "tickers";
        private const string TradesEndpoint = "trades/{}/hist";
        private const string OrderBookEndpoint = "book/{}/{}";
        private const string StatsEndpoint = "stats1/{}:1m:{}:{}/{}";
        private const string LastCandleEndpoint = "candles/trade:{}:{}/last";
        private const string LastFundingCandleEndpoint = "candles/trade:{}:{}:{}/last";
        private const string CandlesEndpoint = "candles/trade:{}:{}/hist";
        private const string FundingCandlesEndpoint = "candles/trade:{}:{}:{}/hist";
        private const string MarketAverageEndpoint = "calc/trade/avg";
        private const string ForeignExchangeEndpoint = "calc/fx";

        private const string WalletsEndpoint = "auth/r/wallets";
        private const string CalcAvailableBalanceEndpoint = "auth/calc/order/avail";
        private const string OpenOrdersEndpoint = "auth/r/orders";
        private const string OrderHistorySingleEndpoint = "auth/r/orders/hist";
        private const string OrderHistoryEndpoint = "auth/r/orders/{}/hist";
        private const string OrderTradesEndpoint = "auth/r/order/{}:{}/trades";
        private const string MyTradesSingleEndpoint = "auth/r/trades/hist";
        private const string MyTradesEndpoint = "auth/r/trades/{}/hist";
        private const string UserInfoEndpoint = "auth/r/info/user";
        private const string LedgerEntriesSingleEndpoint = "auth/r/ledgers/hist";
        private const string LedgerEntriesEndpoint = "auth/r/ledgers/{}/hist";

        private const string ActivePositionsEndpoint = "auth/r/positions";
        private const string PositionHistoryEndpoint = "auth/r/positions/hist";
        private const string PositionAuditEndpoint = "auth/r/positions/audit";
        private const string ActiveFundingOffersEndpoint = "auth/r/funding/offers/{}";
        private const string FundingOfferHistoryEndpoint = "auth/r/funding/offers/{}/hist";
        private const string FundingLoansEndpoint = "auth/r/funding/loans/{}";
        private const string FundingLoansHistoryEndpoint = "auth/r/funding/loans/{}/hist";
        private const string FundingCreditsEndpoint = "auth/r/funding/credits/{}";
        private const string FundingCreditsHistoryEndpoint = "auth/r/funding/credits/{}/hist";
        private const string FundingTradesEndpoint = "auth/r/funding/trades/{}/hist";
        private const string MarginInfoBaseEndpoint = "auth/r/info/margin/base";
        private const string MarginInfoSymbolEndpoint = "auth/r/info/margin/{}";
        private const string FundingInfoEndpoint = "auth/r/info/funding/{}";
        private const string FundingOfferSubmitEndpoint = "auth/w/funding/offer/submit";
        private const string FundingOfferCancelEndpoint = "auth/w/funding/offer/cancel";

        private const string AllMovementsEndpoint = "auth/r/movements/hist";
        private const string MovementsEndpoint = "auth/r/movements/{}/hist";
        private const string DailyPerformanceEndpoint = "auth/r/stats/perf:1D/hist";

        private const string AlertListEndpoint = "auth/r/alerts";
        private const string SetAlertEndpoint = "auth/w/alert/set";
        private const string DeleteAlertEndpoint = "auth/w/alert/price:{}:{}/del";

        private const string PlaceOrderEndpoint = "auth/w/order/submit";
        private const string CancelOrderEndpoint = "auth/w/order/cancel";

        private const string AccountInfoEndpoint = "account_infos";
        private const string SummaryEndpoint = "summary";
        private const string WithdrawalFeeEndpoint = "account_fees";
        private const string CancelAllOrderEndpoint = "order/cancel/all";
        private const string OrderStatusEndpoint = "order/status";

        private const string DepositAddressEndpoint = "deposit/new";
        private const string TransferEndpoint = "transfer";
        private const string WithdrawEndpoint = "withdraw";

        private const string ClaimPositionEndpoint = "position/claim";
        private const string NewOfferEndpoint = "offer/new";
        private const string CancelOfferEndpoint = "offer/cancel";
        private const string GetOfferEndpoint = "offer/status";
        private const string CloseMarginFundingEndpoint = "funding/close";
        private const string ClosePositionEndpoint = "position/close";

        private readonly string? _affCode;
        #endregion

        /// <summary>
        /// Event triggered when an order is placed via this client
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderPlaced;
        /// <summary>
        /// Event triggered when an order is canceled via this client
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderCanceled;

        #region constructor/destructor
        /// <summary>
        /// Create a new instance of BitfinexClient using the default options
        /// </summary>
        public BitfinexClient() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of BitfinexClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public BitfinexClient(BitfinexClientOptions options) : base("Bitfinex", options, options.ApiCredentials == null ? null : new BitfinexAuthenticationProvider(options.ApiCredentials, options.NonceProvider))
        {
            if (options == null)
                throw new ArgumentException("Cant pass null options, use empty constructor for default");

            _affCode = options.AffiliateCode;
        }
        #endregion

        #region methods
        /// <summary>
        /// Sets the default options to use for new clients
        /// </summary>
        /// <param name="options">The options to use for new clients</param>
        public static void SetDefaultOptions(BitfinexClientOptions options)
        {
            defaultOptions = options;
        }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        /// <param name="nonceProvider">Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that</param>
        public void SetApiCredentials(string apiKey, string apiSecret, INonceProvider? nonceProvider = null)
        {
            SetAuthenticationProvider(new BitfinexAuthenticationProvider(new ApiCredentials(apiKey, apiSecret), nonceProvider));
        }

        #region Version2
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<BitfinexPlatformStatus>(GetUrl(StatusEndpoint, ApiVersion2), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexAsset>>> GetAssetAsync(CancellationToken ct = default)
        {
            var result = await SendRequestAsync<IEnumerable<IEnumerable<BitfinexAsset>>>(GetUrl(CurrenciesEndpoint, ApiVersion2), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return WebCallResult<IEnumerable<BitfinexAsset>>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);
            return result.As(result.Data.First());
        }

        /// <inheritdoc />
        public Task<WebCallResult<IEnumerable<BitfinexSymbolOverview>>> GetTickerAsync(string symbol, CancellationToken ct = default)
            => GetTickersAsync(new[] { symbol }, ct);

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexSymbolOverview>>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                {"symbols", symbols?.Any() == true ? string.Join(",", symbols): "ALL"}
            };

            return await SendRequestAsync<IEnumerable<BitfinexSymbolOverview>>(GetUrl(TickersEndpoint, ApiVersion2), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexTradeSimple>>> GetTradeHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 5000);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            return await SendRequestAsync<IEnumerable<BitfinexTradeSimple>>(GetUrl(FillPathParameter(TradesEndpoint, symbol), ApiVersion2), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexOrderBookEntry>>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntValues("limit", 25, 100);
            if (precision == Precision.R0)
                throw new ArgumentException("Precision can not be R0. Use PrecisionLevel0 to get aggregated trades for each price point or GetRawOrderBook to get the raw order book instead");

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));
            var prec = JsonConvert.SerializeObject(precision, new PrecisionConverter(false));

            return await SendRequestAsync<IEnumerable<BitfinexOrderBookEntry>>(GetUrl(FillPathParameter(OrderBookEndpoint, symbol, prec), ApiVersion2), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexRawOrderBookEntry>>> GetRawOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntValues("limit", 25, 100);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));
            var prec = JsonConvert.SerializeObject(Precision.R0, new PrecisionConverter(false));

            return await SendRequestAsync<IEnumerable<BitfinexRawOrderBookEntry>>(GetUrl(FillPathParameter(OrderBookEndpoint, symbol, prec), ApiVersion2), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexStats>>> GetStatsAsync(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = FillPathParameter(StatsEndpoint,
                JsonConvert.SerializeObject(key, new StatKeyConverter(false)),
                symbol,
                JsonConvert.SerializeObject(side, new StatSideConverter(false)),
                JsonConvert.SerializeObject(section, new StatSectionConverter(false)));

            return await SendRequestAsync<IEnumerable<BitfinexStats>>(GetUrl(endpoint, ApiVersion2), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexKline>> GetLastKlineAsync(string symbol, KlineInterval interval, string? fundingPeriod = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();


            string endpoint;
            if (fundingPeriod != null)
            {
                endpoint = FillPathParameter(LastFundingCandleEndpoint,
                    JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)),
                    symbol,
                    fundingPeriod);
            }
            else
            {
                endpoint = FillPathParameter(LastCandleEndpoint,
                    JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)),
                    symbol);
            }

            return await SendRequestAsync<BitfinexKline>(GetUrl(endpoint, ApiVersion2), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexKline>>> GetKlinesAsync(string symbol, KlineInterval interval, string? fundingPeriod = null, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 5000);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            string endpoint;
            if (fundingPeriod != null)
            {
                endpoint = FillPathParameter(FundingCandlesEndpoint,
                    JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)),
                    symbol,
                    fundingPeriod);
            }
            else
            {
                endpoint = FillPathParameter(CandlesEndpoint,
                    JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)),
                    symbol);
            }

            return await SendRequestAsync<IEnumerable<BitfinexKline>>(GetUrl(endpoint, ApiVersion2), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAveragePrice>> GetAveragePriceAsync(string symbol, decimal quantity, decimal? rateLimit = null, int? period = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("period", period?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("rate_limit", rateLimit?.ToString(CultureInfo.InvariantCulture));

            return await SendRequestAsync<BitfinexAveragePrice>(GetUrl(MarketAverageEndpoint, ApiVersion2), HttpMethod.Post, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexForeignExchangeRate>> GetForeignExchangeRateAsync(string asset1, string asset2, CancellationToken ct = default)
        {
            asset1.ValidateNotNull(nameof(asset1));
            asset2.ValidateNotNull(nameof(asset2));

            var parameters = new Dictionary<string, object>
            {
                { "ccy1", asset1 },
                { "ccy2", asset2 }
            };

            return await SendRequestAsync<BitfinexForeignExchangeRate>(GetUrl(ForeignExchangeEndpoint, ApiVersion2), HttpMethod.Post, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexWallet>>> GetBalancesAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<IEnumerable<BitfinexWallet>>(GetUrl(WalletsEndpoint, ApiVersion2), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetActiveOrdersAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<IEnumerable<BitfinexOrder>>(GetUrl(OpenOrdersEndpoint, ApiVersion2), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetOrdersAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            symbol?.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 500);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            var url = string.IsNullOrEmpty(symbol)
                ? OrderHistorySingleEndpoint : FillPathParameter(OrderHistoryEndpoint, symbol!);
            return await SendRequestAsync<IEnumerable<BitfinexOrder>>(GetUrl(url, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexTradeDetails>>> GetOrderTradesAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();

            return await SendRequestAsync<IEnumerable<BitfinexTradeDetails>>(GetUrl(FillPathParameter(OrderTradesEndpoint, symbol, orderId.ToString(CultureInfo.InvariantCulture)), ApiVersion2), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexTradeDetails>>> GetUserTradesAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            symbol?.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);
            
            var url = string.IsNullOrEmpty(symbol)
                ? MyTradesSingleEndpoint : FillPathParameter(MyTradesEndpoint, symbol!);
            return await SendRequestAsync<IEnumerable<BitfinexTradeDetails>>(GetUrl(url, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetActivePositionsAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<IEnumerable<BitfinexPosition>>(GetUrl(ActivePositionsEndpoint, ApiVersion2), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexPositionExtended>>> GetPositionHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 50);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await SendRequestAsync<IEnumerable<BitfinexPositionExtended>>(GetUrl(PositionHistoryEndpoint, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexPositionExtended>>> GetPositionsByIdAsync(IEnumerable<string> ids, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            ids.ValidateNotNull(nameof(ids));
            limit?.ValidateIntBetween(nameof(limit), 1, 250);
            var parameters = new Dictionary<string, object>
            {
                { "id", ids }
            };
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await SendRequestAsync<IEnumerable<BitfinexPositionExtended>>(GetUrl(PositionAuditEndpoint, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingOffer>>> GetActiveFundingOffersAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            return await SendRequestAsync<IEnumerable<BitfinexFundingOffer>>(GetUrl(FillPathParameter(ActiveFundingOffersEndpoint, symbol), ApiVersion2), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingOffer>>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await SendRequestAsync<IEnumerable<BitfinexFundingOffer>>(GetUrl(FillPathParameter(FundingOfferHistoryEndpoint, symbol), ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexFundingOffer>>> SubmitFundingOfferAsync(FundingOrderType fundingOrderType, string symbol, decimal quantity, decimal rate, int period, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            var parameters = new Dictionary<string, object>()
            {
                { "type", JsonConvert.SerializeObject(fundingOrderType, new FundingOrderTypeConverter(false)) },
                { "symbol", symbol },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) },
                { "rate", rate.ToString(CultureInfo.InvariantCulture) },
                { "period", period },
            };
            
            return await SendRequestAsync<BitfinexWriteResult<BitfinexFundingOffer>>(GetUrl(FundingOfferSubmitEndpoint, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexFundingOffer>>> CancelFundingOfferAsync(long offerId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "id", offerId }
            };

            return await SendRequestAsync<BitfinexWriteResult<BitfinexFundingOffer>>(GetUrl(FundingOfferCancelEndpoint, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFunding>>> GetFundingLoansAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            return await SendRequestAsync<IEnumerable<BitfinexFunding>>(GetUrl(FillPathParameter(FundingLoansEndpoint, symbol), ApiVersion2), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFunding>>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await SendRequestAsync<IEnumerable<BitfinexFunding>>(GetUrl(FillPathParameter(FundingLoansHistoryEndpoint, symbol), ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingCredit>>> GetFundingCreditsAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            return await SendRequestAsync<IEnumerable<BitfinexFundingCredit>>(GetUrl(FillPathParameter(FundingCreditsEndpoint, symbol), ApiVersion2), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingCredit>>> GetFundingCreditsHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await SendRequestAsync<IEnumerable<BitfinexFundingCredit>>(GetUrl(FillPathParameter(FundingCreditsHistoryEndpoint, symbol), ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingTrade>>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await SendRequestAsync<IEnumerable<BitfinexFundingTrade>>(GetUrl(FillPathParameter(FundingTradesEndpoint, symbol), ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMarginBase>> GetBaseMarginInfoAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<BitfinexMarginBase>(GetUrl(MarginInfoBaseEndpoint, ApiVersion2), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            return await SendRequestAsync<BitfinexMarginSymbol>(GetUrl(FillPathParameter(MarginInfoSymbolEndpoint, symbol), ApiVersion2), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            return await SendRequestAsync<BitfinexFundingInfo>(GetUrl(FillPathParameter(FundingInfoEndpoint, symbol), ApiVersion2), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexMovement>>> GetMovementsAsync(string? symbol = null, CancellationToken ct = default)
        {
            var url = GetUrl(symbol == null ? AllMovementsEndpoint : FillPathParameter(MovementsEndpoint, symbol), ApiVersion2);
            return await SendRequestAsync<IEnumerable<BitfinexMovement>>(url, HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexAlert>>> GetAlertListAsync(CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "type", "price" }
            };

            return await SendRequestAsync< IEnumerable<BitfinexAlert>>(GetUrl(AlertListEndpoint, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();

            var parameters = new Dictionary<string, object>
            {
                { "type", "price" },
                { "symbol", symbol },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };

            return await SendRequestAsync<BitfinexAlert>(GetUrl(SetAlertEndpoint, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();

            return await SendRequestAsync<BitfinexSuccessResult>(GetUrl(FillPathParameter(DeleteAlertEndpoint, symbol, price.ToString(CultureInfo.InvariantCulture)), ApiVersion2), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal price, WalletType type, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "dir", side == OrderSide.Buy ? 1: -1 },
                { "rate", price.ToString(CultureInfo.InvariantCulture) },
                { "type", JsonConvert.SerializeObject(type, new WalletTypeConverter(false)).ToUpper() }
            };

            return await SendRequestAsync<BitfinexAvailableBalance>(GetUrl(CalcAvailableBalanceEndpoint, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexLedgerEntry>>> GetLedgerEntriesAsync(string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? category = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("category", category);
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            var url = string.IsNullOrEmpty(asset)
                ? LedgerEntriesSingleEndpoint : FillPathParameter(LedgerEntriesEndpoint, asset!);

            return await SendRequestAsync<IEnumerable<BitfinexLedgerEntry>>(GetUrl(url, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexUserInfo>> GetUserInfoAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<BitfinexUserInfo>(GetUrl(UserInfoEndpoint, ApiVersion2), HttpMethod.Post, ct, signed: true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderType type,
            decimal price,
            decimal quantity,
            int? flags = null,
            int? leverage = null,
            int? groupId = null,
            int? clientOrderId = null,
            decimal? priceTrailing = null,
            decimal? priceAuxLimit = null,
            decimal? priceOcoStop = null,
            DateTime? cancelTime = null,
            string? affiliateCode = null,
            CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();

            if (side == OrderSide.Sell)
                quantity = -quantity;

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "type", JsonConvert.SerializeObject(type, new OrderTypeConverter(false)) },
                { "price", price.ToString(CultureInfo.InvariantCulture) },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };

            parameters.AddOptionalParameter("gid", groupId);
            parameters.AddOptionalParameter("cid", clientOrderId);
            parameters.AddOptionalParameter("flags", flags);
            parameters.AddOptionalParameter("lev", leverage);
            parameters.AddOptionalParameter("price_trailing", priceTrailing?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("price_aux_limit", priceAuxLimit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("price_oco_stop", priceOcoStop?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("tif", cancelTime?.ToString("yyyy-MM-dd HH:mm:ss"));
            parameters.AddOptionalParameter("meta", new Dictionary<string, string?>()
            {
                { "aff_code" , affiliateCode ?? _affCode }
            });

            var result = await SendRequestAsync<BitfinexWriteResult<JArray>>(GetUrl(PlaceOrderEndpoint, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (!result)
                return WebCallResult<BitfinexWriteResult<BitfinexOrder>>.CreateErrorResult(result.ResponseStatusCode,
                    result.ResponseHeaders, result.Error!);

            var orderData = result.Data.Data.First().ToObject<BitfinexOrder>();
            var output = new BitfinexWriteResult<BitfinexOrder>()
            {
                Code = result.Data.Code,
                Id = result.Data.Id,
                Status = result.Data.Status,
                Text = result.Data.Text,
                Timestamp = result.Data.Timestamp,
                Type = result.Data.Type,
                Data = orderData
            };

            OnOrderPlaced?.Invoke(orderData!);
            return result.As(output);
        }
        #endregion

        #region Version1

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingBook>> GetFundingBookAsync(string asset, int? limit = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit_bids", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit_asks", limit?.ToString(CultureInfo.InvariantCulture));
            return await SendRequestAsync<BitfinexFundingBook>(GetUrl(FillPathParameter(FundingBookEndpoint, asset), ApiVersion1), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexLend>>> GetLendsAsync(string asset, DateTime? startTime = null, int? limit = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit_lends", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("timestamp", startTime == null ? null: JsonConvert.SerializeObject(startTime, new TimestampSecondsConverter()));
            return await SendRequestAsync<IEnumerable<BitfinexLend>>(GetUrl(FillPathParameter(LendsEndpoint, asset), ApiVersion1), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<IEnumerable<string>>(GetUrl(SymbolsEndpoint, ApiVersion1), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexSymbolDetails>>> GetSymbolDetailsAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<IEnumerable<BitfinexSymbolDetails>>(GetUrl(SymbolDetailsEndpoint, ApiVersion1), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAccountInfo>> GetAccountInfoAsync(CancellationToken ct = default)
        {
            var result = await SendRequestAsync<IEnumerable<BitfinexAccountInfo>>(GetUrl(AccountInfoEndpoint, ApiVersion1), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
            return result ? result.As(result.Data.First()) : WebCallResult<BitfinexAccountInfo>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWithdrawalFees>> GetWithdrawalFeesAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<BitfinexWithdrawalFees>(GetUrl(WithdrawalFeeEndpoint, ApiVersion1), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Bitfinex30DaySummary>> Get30DaySummaryAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<Bitfinex30DaySummary>(GetUrl(SummaryEndpoint, ApiVersion1), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> CancelOrderAsync(long? orderId = null, long? clientOrderId = null, DateTime? clientOrderIdDate = null, CancellationToken ct = default)
        {
            if(orderId != null && clientOrderId != null)
                throw new ArgumentException("Either orderId or clientOrderId should be provided, not both");

            if (clientOrderId != null && clientOrderIdDate == null)
                throw new ArgumentException("The date of the order has to be provided if canceling by clientOrderId");

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("id", orderId);
            parameters.AddOptionalParameter("cid", clientOrderId);
            parameters.AddOptionalParameter("cid_date", clientOrderIdDate?.ToString("yyyy-MM-dd"));

            var result = await SendRequestAsync<BitfinexWriteResult<BitfinexOrder>>(GetUrl(CancelOrderEndpoint, ApiVersion2), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if(result)
                OnOrderCanceled?.Invoke(result.Data.Data!);
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexResult>> CancelAllOrdersAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<BitfinexResult>(GetUrl(CancelAllOrderEndpoint, ApiVersion1), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexPlacedOrder>> GetOrderAsync(long orderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "order_id", orderId }
            };

            return await SendRequestAsync<BitfinexPlacedOrder>(GetUrl(OrderStatusEndpoint, ApiVersion1), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexDepositAddress>> GetDepositAddressAsync(string asset, WithdrawWallet toWallet, bool? forceNew = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new Dictionary<string, object>
            {
                { "method", asset },
                { "wallet_name", JsonConvert.SerializeObject(toWallet, new WithdrawWalletConverter(false)) }
            };
            parameters.AddOptionalParameter("renew", forceNew.HasValue ? JsonConvert.SerializeObject(toWallet, new BoolToIntConverter(false)) : null);

            return await SendRequestAsync<BitfinexDepositAddress>(GetUrl(DepositAddressEndpoint, ApiVersion1), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexTransferResult>> WalletTransferAsync(string asset, decimal quantity, WithdrawWallet fromWallet, WithdrawWallet toWallet, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new Dictionary<string, object>
            {
                { "asset", asset },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) },
                { "walletfrom", JsonConvert.SerializeObject(fromWallet, new WithdrawWalletConverter(false)) },
                { "walletto", JsonConvert.SerializeObject(toWallet, new WithdrawWalletConverter(false)) },
            };
            var result =  await SendRequestAsync<BitfinexTransferResult[]>(GetUrl(TransferEndpoint, ApiVersion1), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (!result)
                return result.As((BitfinexTransferResult)null!);
            
            return result.As(result.Data.First());
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWithdrawalResult>> WithdrawAsync(string withdrawType,
                                                                         WithdrawWallet wallet,
                                                                         decimal quantity,
                                                                         string? address = null,
                                                                         string? accountNumber = null,
                                                                         string? bankSwift = null,
                                                                         string? bankName = null,
                                                                         string? bankAddress = null,
                                                                         string? bankCity = null,
                                                                         string? bankCountry = null,
                                                                         string? paymentDetails = null,
                                                                         bool? expressWire = null,
                                                                         string? intermediaryBankName = null,
                                                                         string? intermediaryBankAddress = null,
                                                                         string? intermediaryBankCity = null,
                                                                         string? intermediaryBankCountry = null,
                                                                         string? intermediaryBankAccount = null,
                                                                         string? intermediaryBankSwift = null,
                                                                         string? accountName = null,
                                                                         string? paymentId = null,
                                                                         CancellationToken ct = default)
        {
            withdrawType.ValidateNotNull(nameof(withdrawType));
            var parameters = new Dictionary<string, object>
            {
                { "withdraw_type", withdrawType },
                { "walletselected", JsonConvert.SerializeObject(wallet, new WithdrawWalletConverter(false)) },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("address", address);
            parameters.AddOptionalParameter("payment_id", paymentId);
            parameters.AddOptionalParameter("account_name", accountName);
            parameters.AddOptionalParameter("account_number", accountNumber);
            parameters.AddOptionalParameter("swift", bankSwift);
            parameters.AddOptionalParameter("bank_name", bankName);
            parameters.AddOptionalParameter("bank_address", bankAddress);
            parameters.AddOptionalParameter("bank_city", bankCity);
            parameters.AddOptionalParameter("bank_country", bankCountry);
            parameters.AddOptionalParameter("detail_payment", paymentDetails);
            parameters.AddOptionalParameter("expressWire", expressWire == null ? null : JsonConvert.SerializeObject(expressWire, new BoolToIntConverter(false)));
            parameters.AddOptionalParameter("intermediary_bank_name", intermediaryBankName);
            parameters.AddOptionalParameter("intermediary_bank_address", intermediaryBankAddress);
            parameters.AddOptionalParameter("intermediary_bank_city", intermediaryBankCity);
            parameters.AddOptionalParameter("intermediary_bank_country", intermediaryBankCountry);
            parameters.AddOptionalParameter("intermediary_bank_account", intermediaryBankAccount);
            parameters.AddOptionalParameter("intermediary_bank_swift", intermediaryBankSwift);

            var result = await SendRequestAsync<IEnumerable<BitfinexWithdrawalResult>>(GetUrl(WithdrawEndpoint, ApiVersion1), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (!result)
                return WebCallResult<BitfinexWithdrawalResult>.CreateErrorResult(result.ResponseStatusCode, null, result.Error!);

            var data = result.Data.First();
            if (!data.Success)
                return WebCallResult<BitfinexWithdrawalResult>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, new ServerError(data.Message));
            return result.As(data);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexPositionV1>> ClaimPositionAsync(long id, decimal quantity, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "position_id", id },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            return await SendRequestAsync<BitfinexPositionV1>(GetUrl(ClaimPositionEndpoint, ApiVersion1), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexOffer>> NewOfferAsync(string asset, decimal quantity, decimal price, int period, FundingType direction, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new Dictionary<string, object>
            {
                { "currency", asset },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) },
                { "rate", price.ToString(CultureInfo.InvariantCulture) },
                { "period", period },
                { "direction", JsonConvert.SerializeObject(direction, new FundingTypeConverter(false)) },
            };
            return await SendRequestAsync<BitfinexOffer>(GetUrl(NewOfferEndpoint, ApiVersion1), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexOffer>> CancelOfferAsync(long offerId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "offer_id", offerId }
            };
            return await SendRequestAsync<BitfinexOffer>(GetUrl(CancelOfferEndpoint, ApiVersion1), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexOffer>> GetOfferAsync(long offerId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "offer_id", offerId }
            };
            return await SendRequestAsync<BitfinexOffer>(GetUrl(GetOfferEndpoint, ApiVersion1), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingContract>> CloseMarginFundingAsync(long swapId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "swap_id", swapId }
            };
            return await SendRequestAsync<BitfinexFundingContract>(GetUrl(CloseMarginFundingEndpoint, ApiVersion1), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexClosePositionResult>> ClosePositionAsync(long positionId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "position_id", positionId }
            };
            return await SendRequestAsync<BitfinexClosePositionResult>(GetUrl(ClosePositionEndpoint, ApiVersion1), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }
        
        private static string ParseAsset(string assetName)
        {
            assetName = assetName.ToLowerInvariant();
            if (assetName == "usdt") return "ust";

            return assetName;
        }
        #endregion

        #region common interface
#pragma warning disable 1066
        async Task<WebCallResult<IEnumerable<ICommonSymbol>>> IExchangeClient.GetSymbolsAsync()
        {
            var symbols = await GetSymbolDetailsAsync().ConfigureAwait(false);
            return symbols.As<IEnumerable<ICommonSymbol>>(symbols.Data);
        }

        async Task<WebCallResult<ICommonTicker>> IExchangeClient.GetTickerAsync(string symbol)
        {
            var tickersResult = await GetTickerAsync(symbol).ConfigureAwait(false);
            return tickersResult.As<ICommonTicker>(tickersResult.Data?.FirstOrDefault());
        }

        async Task<WebCallResult<IEnumerable<ICommonTicker>>> IExchangeClient.GetTickersAsync()
        {
            var tickersResult = await GetTickersAsync().ConfigureAwait(false);
            return tickersResult.As<IEnumerable<ICommonTicker>>(tickersResult.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonRecentTrade>>> IExchangeClient.GetRecentTradesAsync(string symbol)
        {
            var tradesResult = await GetTradeHistoryAsync(symbol).ConfigureAwait(false);
            return tradesResult.As<IEnumerable<ICommonRecentTrade>>(tradesResult.Data);
        }

        async Task<WebCallResult<ICommonOrderBook>> IExchangeClient.GetOrderBookAsync(string symbol)
        {
            var orderBookResult = await GetOrderBookAsync(symbol, Precision.PrecisionLevel0).ConfigureAwait(false);
            if (!orderBookResult)
                return WebCallResult<ICommonOrderBook>.CreateErrorResult(orderBookResult.ResponseStatusCode, orderBookResult.ResponseHeaders, orderBookResult.Error!);

            var isFunding = symbol.StartsWith("f");
            var result = new BitfinexOrderBook
            {
                Asks = orderBookResult.Data.Where(d => isFunding ? d.Quantity < 0: d.Quantity > 0),
                Bids = orderBookResult.Data.Where(d => isFunding? d.Quantity > 0: d.Quantity < 0)
            };

            return orderBookResult.As<ICommonOrderBook>(result);
        }
        
        async Task<WebCallResult<IEnumerable<ICommonKline>>> IExchangeClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var klines = await GetKlinesAsync(symbol, GetTimeFrameFromTimeSpan(timespan), startTime: startTime, endTime: endTime, limit: limit).ConfigureAwait(false);
            return klines.As<IEnumerable<ICommonKline>>(klines.Data);
        }

        async Task<WebCallResult<ICommonOrder>> IExchangeClient.GetOrderAsync(string orderId, string? symbol)
        {
            var result = await GetOrderAsync(long.Parse(orderId)).ConfigureAwait(false);
            return result.As<ICommonOrder>(result.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonTrade>>> IExchangeClient.GetTradesAsync(string orderId, string? symbol = null)
        {
            if (string.IsNullOrEmpty(symbol))
                return WebCallResult<IEnumerable<ICommonTrade>>.CreateErrorResult(new ArgumentError(nameof(symbol) + " required for Bitfinex " + nameof(IExchangeClient.GetTradesAsync)));

            var result = await GetOrderTradesAsync(symbol!, long.Parse(orderId)).ConfigureAwait(false);
            return result.As<IEnumerable<ICommonTrade>>(result.Data);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.PlaceOrderAsync(string symbol, IExchangeClient.OrderSide side, IExchangeClient.OrderType type, decimal quantity, decimal? price = null, string? accountId = null)
        {
            var result = await PlaceOrderAsync(symbol, GetOrderSide(side), GetOrderType(type), quantity, price ?? 0).ConfigureAwait(false);
            return result.As<ICommonOrderId>(result.Data?.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetOpenOrdersAsync(string? symbol)
        {
            var result = await GetActiveOrdersAsync().ConfigureAwait(false);
            return result.As<IEnumerable<ICommonOrder>>(result.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetClosedOrdersAsync(string? symbol)
        {
            var result = await GetOrdersAsync(symbol).ConfigureAwait(false);
            return result.As<IEnumerable<ICommonOrder>>(result.Data);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.CancelOrderAsync(string orderId, string? symbol)
        {
            var result = await CancelOrderAsync(long.Parse(orderId)).ConfigureAwait(false);
            return result.As<ICommonOrderId>(result.Data?.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonBalance>>> IExchangeClient.GetBalancesAsync(string? accountId = null)
        {
            var result = await GetBalancesAsync().ConfigureAwait(false);
            return result.As<IEnumerable<ICommonBalance>>(result.Data.Where(d => d.Type == WalletType.Exchange));
        }
#pragma warning restore 1066

        /// <summary>
        /// Get the name of a symbol for Bitfinex based on the base and quote asset
        /// </summary>
        /// <param name="baseAsset"></param>
        /// <param name="quoteAsset"></param>
        /// <returns></returns>
        public string GetSymbolName(string baseAsset, string quoteAsset) =>
            "t" + (ParseAsset(baseAsset) + ParseAsset(quoteAsset)).ToUpper(CultureInfo.InvariantCulture);

        #endregion


        #region private methods
        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override Error ParseErrorResponse(JToken data)
        {
            if (!(data is JArray))
            {
                if (data["error"] != null && data["code"] != null && data["error_description"] != null)
                    return new ServerError((int)data["code"]!, data["error"] + ": " + data["error_description"]);
                if (data["message"] != null)
                    return new ServerError(data["message"]!.ToString());
                else
                    return new ServerError(data.ToString());
            }

            var error = data.ToObject<BitfinexError>();
            return new ServerError(error!.ErrorCode, error.ErrorMessage);

        }

        private Uri GetUrl(string endpoint, string version)
        {
            var result = $"{BaseAddress}v{version}/{endpoint}";
            return new Uri(result);
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
        #endregion
        #endregion
    }
}
