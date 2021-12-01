using Bitfinex.Net.Interfaces.Clients.General;
using Bitfinex.Net.Interfaces.Clients.Spot;
using CryptoExchange.Net.Interfaces;

namespace Bitfinex.Net.Interfaces.Clients.Rest
{
    /// <summary>
    /// Client for accessing the Bitfinex API. 
    /// </summary>
    public interface IBitfinexClient: IRestClient
    {

        IBitfinexClientGeneral GeneralApi { get; }

        IBitfinexClientSpotMarket SpotApi { get; }
    }
}