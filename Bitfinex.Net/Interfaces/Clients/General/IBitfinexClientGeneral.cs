using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Interfaces.Clients.General
{
    public interface IBitfinexClientGeneral : IDisposable
    {
        /// <summary>
        /// Endpoints related to funding
        /// </summary>
        public IBitfinexClientGeneralFunding Funding { get; }
    }
}
