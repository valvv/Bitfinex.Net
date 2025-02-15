﻿using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using CryptoExchange.Net;
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
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.CommonObjects;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class BitfinexRestClientSpotApiTrading : IBitfinexRestClientSpotApiTrading
    {
        private const string OrderHistorySingleEndpoint = "auth/r/orders/hist";
        private const string OrderHistoryEndpoint = "auth/r/orders/{}/hist";
        private const string OrderTradesEndpoint = "auth/r/order/{}:{}/trades";
        private const string MyTradesSingleEndpoint = "auth/r/trades/hist";
        private const string MyTradesEndpoint = "auth/r/trades/{}/hist";
        private const string PlaceOrderEndpoint = "auth/w/order/submit";
        private const string CancelOrderEndpoint = "auth/w/order/cancel";

        private const string PositionHistoryEndpoint = "auth/r/positions/hist";
        private const string PositionAuditEndpoint = "auth/r/positions/audit";
        private const string ActivePositionsEndpoint = "auth/r/positions";

        private readonly BitfinexRestClientSpotApi _baseClient;

        internal BitfinexRestClientSpotApiTrading(BitfinexRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }


        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetOpenOrdersAsync(string? symbol = null, IEnumerable<long>? orderIds = null, CancellationToken ct = default)
        {
            var url = "auth/r/orders";
            if (symbol != null)
                url += "/" + symbol;

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("id", orderIds);

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexOrder>>(_baseClient.GetUrl(url, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetClosedOrdersAsync(string? symbol = null, IEnumerable<long>? orderIds = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            symbol?.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 500);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("id", orderIds);

            var url = string.IsNullOrEmpty(symbol)
                ? OrderHistorySingleEndpoint : OrderHistoryEndpoint.FillPathParameters(symbol!);
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexOrder>>(_baseClient.GetUrl(url, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexTradeDetails>>> GetOrderTradesAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexTradeDetails>>(_baseClient.GetUrl(OrderTradesEndpoint.FillPathParameters(symbol, orderId.ToString(CultureInfo.InvariantCulture)), "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexTradeDetails>>> GetUserTradesAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            symbol?.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var url = string.IsNullOrEmpty(symbol)
                ? MyTradesSingleEndpoint : MyTradesEndpoint.FillPathParameters(symbol!);
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexTradeDetails>>(_baseClient.GetUrl(url, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderType type,
            decimal quantity,
            decimal price,
            OrderFlags? flags = null,
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
                { "aff_code" , affiliateCode ?? _baseClient.AffiliateCode }
            });

            var result = await _baseClient.SendRequestAsync<BitfinexWriteResult<JArray>>(_baseClient.GetUrl(PlaceOrderEndpoint, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexWriteResult<BitfinexOrder>>(default);

            var orderData = result.Data.Data!.First().ToObject<BitfinexOrder>();
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

            _baseClient.InvokeOrderPlaced(new OrderId { SourceObject = result.Data, Id = result.Data.Id.ToString(CultureInfo.InvariantCulture) });
            return result.As(output);
        }


        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> CancelOrderAsync(long? orderId = null, long? clientOrderId = null, DateTime? clientOrderIdDate = null, CancellationToken ct = default)
        {
            if (orderId != null && clientOrderId != null)
                throw new ArgumentException("Either orderId or clientOrderId should be provided, not both");

            if (clientOrderId != null && clientOrderIdDate == null)
                throw new ArgumentException("The date of the order has to be provided if canceling by clientOrderId");

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("id", orderId);
            parameters.AddOptionalParameter("cid", clientOrderId);
            parameters.AddOptionalParameter("cid_date", clientOrderIdDate?.ToString("yyyy-MM-dd"));

            var result = await _baseClient.SendRequestAsync<BitfinexWriteResult<BitfinexOrder>>(_baseClient.GetUrl(CancelOrderEndpoint, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                _baseClient.InvokeOrderCanceled(new OrderId { SourceObject = result.Data, Id = result.Data.Id.ToString(CultureInfo.InvariantCulture) });
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<IEnumerable<BitfinexOrder>>>> CancelOrdersAsync(IEnumerable<long>? orderIds = null, IEnumerable<long>? groupIds = null, Dictionary<long, DateTime>? clientOrderIds = null, bool? all = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("id", orderIds);
            parameters.AddOptionalParameter("gid", groupIds);
            parameters.AddOptionalParameter("all", all == true ? (bool?)true : null);
            if (clientOrderIds != null)
                parameters.Add("cid", clientOrderIds.ToDictionary(c => c.Key, c => c.Value.ToString("yyyy-MM-dd")));

            return await _baseClient.SendRequestAsync<BitfinexWriteResult<IEnumerable<BitfinexOrder>>>(_baseClient.GetUrl("auth/w/order/cancel/multi", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetPositionHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 50);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexPosition>>(_baseClient.GetUrl(PositionHistoryEndpoint, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexPosition>>> ClaimPositionAsync(long id, decimal quantity, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "id", id },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            return await _baseClient.SendRequestAsync<BitfinexWriteResult<BitfinexPosition>>(_baseClient.GetUrl("auth/w/position/claim", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexPositionBasic>>> IncreasePositionAsync(string symbol, decimal quantity, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            return await _baseClient.SendRequestAsync<BitfinexWriteResult<BitfinexPositionBasic>>(_baseClient.GetUrl("auth/w/position/increase", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexIncreasePositionInfo>> GetIncreasePositionInfoAsync(string symbol, decimal quantity, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            return await _baseClient.SendRequestAsync<BitfinexIncreasePositionInfo>(_baseClient.GetUrl("auth/r/position/increase/info", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetPositionsAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexPosition>>(_baseClient.GetUrl(ActivePositionsEndpoint, "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetPositionSnapshotsAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexPosition>>(_baseClient.GetUrl("auth/r/positions/snap", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetPositionsByIdAsync(IEnumerable<string> ids, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            ids.ValidateNotNull(nameof(ids));
            limit?.ValidateIntBetween(nameof(limit), 1, 250);
            var parameters = new Dictionary<string, object>
            {
                { "id", ids }
            };
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexPosition>>(_baseClient.GetUrl(PositionAuditEndpoint, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }
    }
}
