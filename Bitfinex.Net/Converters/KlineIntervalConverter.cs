using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class KlineIntervalConverter: BaseConverter<KlineInterval>
    {
        public KlineIntervalConverter(): this(true) { }
        public KlineIntervalConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<KlineInterval, string>> Mapping => new List<KeyValuePair<KlineInterval, string>>
        {
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneMinute, "1m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FiveMinute, "5m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FifteenMinute, "15m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThirtyMinute, "30m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneHour, "1h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThreeHour, "3h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.SixHour, "6h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.TwelveHour, "12h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneDay, "1D"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.SevenDay, "7D"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FourteenDay, "14D"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneMonth, "1M")
        };
    }
}
