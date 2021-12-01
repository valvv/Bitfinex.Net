using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Interfaces;

namespace Bitfinex.Net.Interfaces.Clients
{
    /// <summary>
    /// Interface for the Bitfinex socket client
    /// </summary>
    public interface IBitfinexSocketClient : ISocketClient
    {
        IBitfinexSocketClientSpotStreams SpotStreams { get; }
    }
}