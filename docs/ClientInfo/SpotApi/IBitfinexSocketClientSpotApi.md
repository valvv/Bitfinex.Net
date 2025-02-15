---
title: IBitfinexSocketClientSpotApi
has_children: true
parent: Socket API documentation
---
*[generated documentation]*  
`BitfinexSocketClient > SpotApi`  
*Bitfinex spot streams*
  

***

## CancelFundingOfferAsync  

[https://docs.bitfinex.com/reference/ws-auth-input-offer-cancel](https://docs.bitfinex.com/reference/ws-auth-input-offer-cancel)  
<p>

*Cancel a funding offer*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.CancelFundingOfferAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<BitfinexFundingOffer>> CancelFundingOfferAsync(long id);  
```  

|Parameter|Description|
|---|---|
|id|Id of the offer to cancel|

</p>

***

## CancelOrderAsync  

[https://docs.bitfinex.com/reference#ws-auth-input-order-cancel](https://docs.bitfinex.com/reference#ws-auth-input-order-cancel)  
<p>

*Cancels an order*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.CancelOrderAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<BitfinexOrder>> CancelOrderAsync(long orderId);  
```  

|Parameter|Description|
|---|---|
|orderId|The id of the order to cancel|

</p>

***

## CancelOrdersAsync  

[https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi](https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi)  
<p>

*Cancels multiple orders based on their order ids*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.CancelOrdersAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<bool>> CancelOrdersAsync(IEnumerable<long> orderIds);  
```  

|Parameter|Description|
|---|---|
|orderIds|The order ids to cancel|

</p>

***

## CancelOrdersByClientOrderIdsAsync  

[https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi](https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi)  
<p>

*Cancels multiple orders based on their clientOrderIds*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.CancelOrdersByClientOrderIdsAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<bool>> CancelOrdersByClientOrderIdsAsync(Dictionary<long,DateTime> clientOrderIds);  
```  

|Parameter|Description|
|---|---|
|clientOrderIds|The client order ids to cancel, listed as (clientOrderId, Day) pair. ClientOrderIds are unique per day, so timestamp should be provided|

</p>

***

## CancelOrdersByGroupIdAsync  

[https://docs.bitfinex.com/reference#ws-auth-input-order-cancel](https://docs.bitfinex.com/reference#ws-auth-input-order-cancel)  
<p>

*Cancels multiple orders based on their groupId*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.CancelOrdersByGroupIdAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<bool>> CancelOrdersByGroupIdAsync(long groupOrderId);  
```  

|Parameter|Description|
|---|---|
|groupOrderId|The group id to cancel|

</p>

***

## CancelOrdersByGroupIdsAsync  

[https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi](https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi)  
<p>

*Cancels multiple orders based on their groupIds*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.CancelOrdersByGroupIdsAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<bool>> CancelOrdersByGroupIdsAsync(IEnumerable<long> groupOrderIds);  
```  

|Parameter|Description|
|---|---|
|groupOrderIds|The group ids to cancel|

</p>

***

## PlaceOrderAsync  

[https://docs.bitfinex.com/reference#ws-auth-input-order-new](https://docs.bitfinex.com/reference#ws-auth-input-order-new)  
<p>

*Places a new order*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.PlaceOrderAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderSide side, OrderType type, string symbol, decimal quantity, long? groupId = default, long? clientOrderId = default, decimal? price = default, decimal? priceTrailing = default, decimal? priceAuxiliaryLimit = default, decimal? priceOcoStop = default, OrderFlags? flags = default, int? leverage = default, DateTime? cancelTime = default, string? affiliateCode = default);  
```  

|Parameter|Description|
|---|---|
|side|The order side|
|type|The type of the order|
|symbol|The symbol the order is for|
|quantity|The quantity of the order, positive for buying, negative for selling|
|_[Optional]_ groupId|Group id to assign to the order|
|_[Optional]_ clientOrderId|Client order id to assign to the order|
|_[Optional]_ price|Price of the order|
|_[Optional]_ priceTrailing|Trailing price of the order|
|_[Optional]_ priceAuxiliaryLimit|Auxiliary limit price of the order|
|_[Optional]_ priceOcoStop|Oco stop price of the order|
|_[Optional]_ flags|Additional flags|
|_[Optional]_ leverage|Leverage|
|_[Optional]_ cancelTime|Automatically cancel the order after this time|
|_[Optional]_ affiliateCode|Affiliate code for the order|

</p>

***

## SubmitFundingOfferAsync  

[https://docs.bitfinex.com/reference/ws-auth-input-offer-new](https://docs.bitfinex.com/reference/ws-auth-input-offer-new)  
<p>

*Submit a new funding offer*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubmitFundingOfferAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<BitfinexFundingOffer>> SubmitFundingOfferAsync(FundingOfferType type, string symbol, decimal quantity, decimal price, int period, int? flags = default);  
```  

|Parameter|Description|
|---|---|
|type|Offer type|
|symbol|Symbol|
|quantity|Amount (more than 0 for offer, less than 0 for bid)|
|price|Rate (or offset for FRRDELTA offers)|
|period|Time period of offer. Minimum 2 days. Maximum 120 days.|
|_[Optional]_ flags|Flags|

</p>

***

## SubscribeToBalanceUpdatesAsync  

[https://docs.bitfinex.com/reference#ws-auth-wallets](https://docs.bitfinex.com/reference#ws-auth-wallets)  
<p>

*Subscribe to wallet information updates*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToBalanceUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexWallet>>>> walletHandler, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|walletHandler|Data handler for wallet updates|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToDerivativesUpdatesAsync  

[https://docs.bitfinex.com/reference/ws-public-status](https://docs.bitfinex.com/reference/ws-public-status)  
<p>

*Subscribe to derivatives status updates*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToDerivativesUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToDerivativesUpdatesAsync(string symbol, Action<DataEvent<BitfinexDerivativesStatusUpdate>> handler, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The derivatives symbol, tBTCF0:USTF0 for example|
|handler|The handler for the data|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToFundingOrderBookUpdatesAsync  

[https://docs.bitfinex.com/reference#ws-public-books](https://docs.bitfinex.com/reference#ws-public-books)  
<p>

*Subscribes to funding order book updates for a symbol. Use SubscribeToOrderBookUpdatesAsync for trade symbol ticker updates*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToFundingOrderBookUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToFundingOrderBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<IEnumerable<BitfinexOrderBookFundingEntry>>> handler, Action<DataEvent<int>>? checksumHandler = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to subscribe to|
|precision|The precision of the updates|
|frequency|The frequency of updates|
|length|The range for the order book updates, either 25 or 100|
|handler|The handler for the data|
|_[Optional]_ checksumHandler|The handler for the checksum, can be used to validate a order book implementation|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToFundingTickerUpdatesAsync  

[https://docs.bitfinex.com/reference#ws-public-ticker](https://docs.bitfinex.com/reference#ws-public-ticker)  
<p>

*Subscribes to funding ticker updates a symbol. Use SubscribeToTickerUpdatesAsync for trade symbol ticker updates*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToFundingTickerUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToFundingTickerUpdatesAsync(string symbol, Action<DataEvent<BitfinexStreamFundingTicker>> handler, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to subscribe to|
|handler|The handler for the data|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToFundingUpdatesAsync  

[https://docs.bitfinex.com/reference#ws-auth-funding-offers](https://docs.bitfinex.com/reference#ws-auth-funding-offers)  
[https://docs.bitfinex.com/reference#ws-auth-funding-credits](https://docs.bitfinex.com/reference#ws-auth-funding-credits)  
[https://docs.bitfinex.com/reference#ws-auth-funding-loans](https://docs.bitfinex.com/reference#ws-auth-funding-loans)  
<p>

*Subscribe to funding information updates*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToFundingUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToFundingUpdatesAsync(Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFundingOffer>>>> fundingOfferHandler, Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFundingCredit>>>> fundingCreditHandler, Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFunding>>>> fundingLoanHandler, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|fundingOfferHandler|Subscribe to funding offer updates. Can be null if not interested|
|fundingCreditHandler|Subscribe to funding credit updates. Can be null if not interested|
|fundingLoanHandler|Subscribe to funding loan updates. Can be null if not interested|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToKlineUpdatesAsync  

[https://docs.bitfinex.com/reference#ws-public-candles](https://docs.bitfinex.com/reference#ws-public-candles)  
<p>

*Subscribes to kline updates for a symbol*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToKlineUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<BitfinexKline>>> handler, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to subscribe to. For funding klines use {symbol}:p{period}, for example fUSD:p30|
|interval|The interval of the klines|
|handler|The handler for the data|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToLiquidationUpdatesAsync  

[https://docs.bitfinex.com/reference/ws-public-status](https://docs.bitfinex.com/reference/ws-public-status)  
<p>

*Subscribe to liquidation updates*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToLiquidationUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToLiquidationUpdatesAsync(Action<DataEvent<IEnumerable<BitfinexLiquidation>>> handler, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|handler|The handler for the data|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToOrderBookUpdatesAsync  

[https://docs.bitfinex.com/reference#ws-public-books](https://docs.bitfinex.com/reference#ws-public-books)  
<p>

*Subscribes to order book updates for a symbol. Use SubscribeToFundingOrderBookUpdatesAsync for funding symbol ticker updates*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToOrderBookUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<IEnumerable<BitfinexOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to subscribe to|
|precision|The precision of the updates|
|frequency|The frequency of updates|
|length|The range for the order book updates, either 25 or 100|
|handler|The handler for the data|
|_[Optional]_ checksumHandler|The handler for the checksum, can be used to validate a order book implementation|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToRawFundingOrderBookUpdatesAsync  

[https://docs.bitfinex.com/reference#ws-public-raw-books](https://docs.bitfinex.com/reference#ws-public-raw-books)  
<p>

*Subscribes to raw order book updates for a symbol. Use SubscribeToRawOrderBookUpdatesAsync for trade symbol ticker updates*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToRawFundingOrderBookUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToRawFundingOrderBookUpdatesAsync(string symbol, int limit, Action<DataEvent<IEnumerable<BitfinexRawOrderBookFundingEntry>>> handler, Action<DataEvent<int>>? checksumHandler = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to subscribe to|
|limit|The range for the order book updates|
|handler|The handler for the data|
|_[Optional]_ checksumHandler|The handler for the checksum, can be used to validate a order book implementation|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToRawOrderBookUpdatesAsync  

[https://docs.bitfinex.com/reference#ws-public-raw-books](https://docs.bitfinex.com/reference#ws-public-raw-books)  
<p>

*Subscribes to raw order book updates for a symbol. Use SubscribeToRawFundingOrderBookUpdatesAsync for funding symbol ticker updates*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToRawOrderBookUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToRawOrderBookUpdatesAsync(string symbol, int limit, Action<DataEvent<IEnumerable<BitfinexRawOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to subscribe to|
|limit|The range for the order book updates|
|handler|The handler for the data|
|_[Optional]_ checksumHandler|The handler for the checksum, can be used to validate a order book implementation|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToTickerUpdatesAsync  

[https://docs.bitfinex.com/reference#ws-public-ticker](https://docs.bitfinex.com/reference#ws-public-ticker)  
<p>

*Subscribes to ticker updates for a symbol. Use SubscribeToFundingTickerUpdatesAsync for funding symbol ticker updates*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToTickerUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<BitfinexTicker>> handler, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to subscribe to|
|handler|The handler for the data|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToTradeUpdatesAsync  

[https://docs.bitfinex.com/reference#ws-public-trades](https://docs.bitfinex.com/reference#ws-public-trades)  
<p>

*Subscribes to public trade updates for a symbol*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToTradeUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<IEnumerable<BitfinexTradeSimple>>> handler, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to subscribe to|
|handler|The handler for the data|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToUserTradeUpdatesAsync  

[https://docs.bitfinex.com/reference#ws-auth-trades](https://docs.bitfinex.com/reference#ws-auth-trades)  
<p>

*Subscribe to trading information updates*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.SubscribeToUserTradeUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexOrder>>>> orderHandler, Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexTradeDetails>>>> tradeHandler, Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexPosition>>>> positionHandler, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|orderHandler|Data handler for order updates. Can be null if not interested|
|tradeHandler|Data handler for trade execution updates. Can be null if not interested|
|positionHandler|Data handler for position updates. Can be null if not interested|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## UpdateOrderAsync  

[https://docs.bitfinex.com/reference#ws-auth-input-order-update](https://docs.bitfinex.com/reference#ws-auth-input-order-update)  
<p>

*Updates an order*  

```csharp  
var client = new BitfinexSocketClient();  
var result = await client.SpotApi.UpdateOrderAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<BitfinexOrder>> UpdateOrderAsync(long orderId, decimal? price = default, decimal? quantity = default, decimal? delta = default, decimal? priceAuxiliaryLimit = default, decimal? priceTrailing = default, OrderFlags? flags = default);  
```  

|Parameter|Description|
|---|---|
|orderId|The id of the order to update|
|_[Optional]_ price|The new price of the order|
|_[Optional]_ quantity|The new quantity of the order|
|_[Optional]_ delta|The delta to change|
|_[Optional]_ priceAuxiliaryLimit|the new aux limit price|
|_[Optional]_ priceTrailing|The new trailing price|
|_[Optional]_ flags|The new flags|

</p>
