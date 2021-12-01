using System;

namespace Bitfinex.Net.Interfaces.Clients.GeneralApi
{
    public interface IBitfinexClientGeneralApi : IDisposable
    {
        /// <summary>
        /// Endpoints related to funding
        /// </summary>
        public IBitfinexClientGeneralApiFunding Funding { get; }
    }
}
