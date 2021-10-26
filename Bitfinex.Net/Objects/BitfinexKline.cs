﻿using System;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.ExchangeInterfaces;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Kline info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexKline: ICommonKline
    {
        /// <summary>
        /// The timestamp of the kline
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(TimestampConverter))]
        public DateTime OpenTime { get; set; }
        /// <summary>
        /// The opening price
        /// </summary>
        [ArrayProperty(1)]
        public decimal OpenPrice { get; set; }
        /// <summary>
        /// The closing price
        /// </summary>
        [ArrayProperty(2)]
        public decimal ClosePrice { get; set; }
        /// <summary>
        /// The highest price
        /// </summary>
        [ArrayProperty(3)]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// The lowest price
        /// </summary>
        [ArrayProperty(4)]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// The volume for this kline
        /// </summary>
        [ArrayProperty(5)]
        public decimal Volume { get; set; }

        decimal ICommonKline.CommonHighPrice => HighPrice;
        decimal ICommonKline.CommonLowPrice => LowPrice;
        decimal ICommonKline.CommonOpenPrice => OpenPrice;
        decimal ICommonKline.CommonClosePrice => ClosePrice;
        DateTime ICommonKline.CommonOpenTime => OpenTime;
        decimal ICommonKline.CommonVolume => Volume;
    }
}
