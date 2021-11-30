using Bitfinex.Net.Interfaces.Clients.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Interfaces.Clients.Spot
{
    public interface IBitfinexClientSpotMarket : IDisposable
    {
        /// <summary>
        /// Endpoints related to account settings, info or actions
        /// </summary>
        public IBitfinexClientSpotMarketAccount Account { get; }

        /// <summary>
        /// Endpoints related to retrieving market and system data
        /// </summary>
        public IBitfinexClientSpotMarketExchangeData ExchangeData { get; }

        /// <summary>
        /// Endpoints related to orders and trades
        /// </summary>
        public IBitfinexClientSpotMarketTrading Trading { get; }
    }
}
