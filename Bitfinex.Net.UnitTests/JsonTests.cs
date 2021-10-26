using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Objects;
using Bitfinex.Net.UnitTests;
using Bitfinex.Net.UnitTests.TestImplementations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture]
    public class JsonTests
    {
        private JsonToObjectComparer<IBitfinexClient> _comparer = new JsonToObjectComparer<IBitfinexClient>((json) => TestHelpers.CreateResponseClient(json, new BitfinexClientOptions()
        { ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "123"), OutputOriginalData = true }, System.Net.HttpStatusCode.OK));

        [Test]
        public async Task ValidateSpotCalls()
        {   
            await _comparer.ProcessSubject(c => c,
                parametersToSetNull: new[] { "limit", "clientOrderId" },
                ignoreProperties: new Dictionary<string, List<string>>
                {
                    { "GetOrderAsync", new List<string> { "meta" } }
                },
                takeFirstItemForCompare: new List<string> { "WithdrawAsync", "GetAccountInfoAsync", "WalletTransferAsync" }
                );
        }  
    }
}
