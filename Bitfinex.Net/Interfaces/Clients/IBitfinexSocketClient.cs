using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;

namespace Bitfinex.Net.Interfaces.Clients.Socket
{
    /// <summary>
    /// Interface for the Bitfinex socket client
    /// </summary>
    public interface IBitfinexSocketClient: ISocketClient
    {
        IBitfinexSocketClientSpotMarket SpotStreams { get; }
    }
}