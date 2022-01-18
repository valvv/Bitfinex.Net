## Creating client
There are 2 clients available to interact with the Bitfinex API, the `BitfinexClient` and `BitfinexSocketClient`.

*Create a new rest client*
````C#
var bitfinexClient = new BitfinexClient(new BitfinexClientOptions()
{
	// Set options here for this client
});
````

*Create a new socket client*
````C#
var bitfinexSocketClient = new BitfinexSocketClient(new BitfinexSocketClientOptions()
{
	// Set options here for this client
});
````

Different options are available to set on the clients, see this example
````C#
var bitfinexClient = new BitfinexClient(new BitfinexClientOptions()
{
	ApiCredentials = new ApiCredentials("API-KEY", "API-SECRET"),
	LogLevel = LogLevel.Trace,
	RequestTimeout = TimeSpan.FromSeconds(60)
});
````
Alternatively, options can be provided before creating clients by using `SetDefaultOptions`:
````C#
BitfinexClient.SetDefaultOptions(new BitfinexClientOptions{
	// Set options here for all new clients
});
var bitfinexClient = new BitfinexClient();
````
More info on the specific options can be found on the [CryptoExchange.Net wiki](https://github.com/JKorf/CryptoExchange.Net/wiki/Options)

## Usage
Make sure to read the [CryptoExchange.Net wiki](https://github.com/JKorf/CryptoExchange.Net/wiki/Clients#processing-request-responses) on processing responses.

### Dependency injection
See [CryptoExchange.Net wiki](https://github.com/JKorf/CryptoExchange.Net/wiki/Clients#dependency-injection)

#### Get market data
````C#
// Getting info on all symbols
var symbolData = await bitfinexClient.SpotApi.ExchangeData.GetSymbolsAsync();

// Getting tickers for all symbols
var tickerData = await bitfinexClient.SpotApi.ExchangeData.GetTickersAsync();

// Getting the order book of a symbol
var orderBookData = await bitfinexClient.SpotApi.ExchangeData.GetOrderBookAsync("tBTCUST", Precision.PrecisionLevel0);

// Getting recent trades of a symbol
var tradeHistoryData = await bitfinexClient.SpotApi.ExchangeData.GetTradeHistoryAsync("tBTCUST");
````

#### Requesting balances
````C#
var accountData = await bitfinexClient.SpotApi.Account.GetBalancesAsync();
````
#### Placing order
````C#
// Placing a buy limit order for 0.001 BTC at a price of 50000USDT each
var symbolData = await bitfinexClient.SpotApi.Trading.PlaceOrderAsync(
                "tBTCUST",
                OrderSide.Buy,
                OrderType.ExchangeLimit,
                0.001m,
                50000);
													
// Place a stop loss order, place a limit order of 0.001 BTC at 39000USDT each when the last trade price drops below 40000USDT
var orderData = await bitfinexClient.SpotApi.Trading.PlaceOrderAsync(
                "tBTCUST",
                OrderSide.Sell,
                OrderType.ExchangeStopLimit,
                0.001m,
                39000,
                priceAuxLimit: 40000);
````

#### Requesting a specific order
````C#
// Request info on order with id `1234`
var orderData = await bitfinexClient.SpotApi.Trading.GetOrderAsync(1234);
````

#### Requesting order history
````C#
// Get all orders conform the parameters
 var ordersData = await bitfinexClient.SpotApi.Trading.GetClosedOrdersAsync();
````

#### Cancel order
````C#
// Cancel order with id `1234`
var orderData = await bitfinexClient.SpotApi.Trading.CancelOrderAsync(1234);
````

#### Get user trades
````C#
var userTradesResult = await bitfinexClient.SpotApi.Trading.GetUserTradesAsync();
````

#### Subscribing to market data updates
````C#
var subscribeResult =  bitfinexSocket.SpotStreams.SubscribeToTickerUpdatesAsync("tBTCUST", data =>
{
	// Handle ticker data
});
````

#### Subscribing to order updates
````C#
// Any handler can be passed `null` if you're not interested in that type of update
var subscribeResult = await bitfinexSocket.SpotStreams.SubscribeToUserTradeUpdatesAsync(
	data =>
	{
	  // Handle order updates
	},
	data =>
	{
	  // Handle trade updates
	}, 
	data =>
	{
	  // Handle position updates
	});
````
