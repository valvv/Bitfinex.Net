using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.RestV1Objects;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Interfaces.Clients.Rest
{
    public interface IBitfinexClientTrading
    {
        /// <summary>
        /// Get the active orders
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetOpenOrdersAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the order history for a symbol for this account
        /// </summary>
        /// <param name="symbol">The symbol to get the history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetOrdersAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the individual trades for an order
        /// </summary>
        /// <param name="symbol">The symbol of the order</param>
        /// <param name="orderId">The order Id</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexTradeDetails>>> GetOrderTradesAsync(string symbol, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Get the trade history for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexTradeDetails>>> GetUserTradesAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);


        /// <summary>
        /// Get a list of historical positions
        /// </summary>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexPositionExtended>>> GetPositionHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Place a new order
        /// </summary>
        /// <param name="symbol">Symbol to place order for</param>
        /// <param name="side">Side of the order</param>
        /// <param name="type">Type of the order</param>
        /// <param name="quantity">The quantity of the order</param>
        /// <param name="price">The price for the order</param>
        /// <param name="affiliateCode">Affiliate code for the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <param name="flags"></param>
        /// <param name="leverage">Set the leverage for a derivative order, supported by derivative symbol orders only. The value should be between 1 and 100 inclusive. The field is optional, if omitted the default leverage value of 10 will be used.</param>
        /// <param name="groupId">Group id</param>
        /// <param name="clientOrderId">Client order id</param>
        /// <param name="priceTrailing">The trailing price for a trailing stop order</param>
        /// <param name="priceAuxLimit">Auxiliary Limit price (for STOP LIMIT)</param>
        /// <param name="priceOcoStop">OCO stop price</param>
        /// <param name="cancelTime">datetime for automatic order cancelation</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> PlaceOrderAsync(
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
            CancellationToken ct = default);

        /// <summary>
        /// Cancel a specific order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="clientOrderId">The client order id of the order to cancel</param>
        /// <param name="clientOrderIdDate">The date of the client order (year month and day)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> CancelOrderAsync(long? orderId = null, long? clientOrderId = null, DateTime? clientOrderIdDate = null, CancellationToken ct = default);

        /// <summary>
        /// Cancels all open orders
        /// </summary>
        /// <param name="ct">Cancellation token</param><returns></returns>
        Task<WebCallResult<BitfinexResult>> CancelAllOrdersAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the status of a specific order
        /// </summary>
        /// <param name="orderId">The order id of the order to get</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPlacedOrder>> GetOrderAsync(long orderId, CancellationToken ct = default);

        /// <summary>
        /// Claim a position
        /// </summary>
        /// <param name="id">The id of the position to claim</param>
        /// <param name="quantity">The (partial) quantity to be claimed</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPositionV1>> ClaimPositionAsync(long id, decimal quantity, CancellationToken ct = default);

        /// <summary>
        /// Close a position
        /// </summary>
        /// <param name="positionId">The id to close</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexClosePositionResult>> ClosePositionAsync(long positionId, CancellationToken ct = default);

        /// <summary>
        /// Get the active positions
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetActivePositionsAsync(CancellationToken ct = default);


        /// <summary>
        /// Get positions by id
        /// </summary>
        /// <param name="ids">The id's of positions to return</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexPositionExtended>>> GetPositionsByIdAsync(IEnumerable<string> ids, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

    }
}
