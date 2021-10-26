using System;
using System.Collections.Generic;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.RestV1Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net;
using NUnit.Framework;
using System.Linq;
using Bitfinex.Net.UnitTests.TestImplementations;
using Moq;
using System.Net;
using System.Threading.Tasks;
using Bitfinex.Net.Enums;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture()]
    public class BitfinexClientTests
    {
        [TestCase]
        public async Task ReceivingHttpError_Should_ResultInErrorResult()
        {
            // arrange
            var client = TestHelpers.CreateResponseClient("Error message", null, HttpStatusCode.BadRequest);

            // act
            var result = await client.GetAssetAsync();

            // assert
            Assert.AreEqual(false, result.Success);
            Assert.IsTrue(result.Error.ToString().Contains("Error message"));
            Assert.AreEqual(HttpStatusCode.BadRequest, result.ResponseStatusCode);
        }


        [Test]
        public void ProvidingApiCredentials_Should_SaveApiCredentials()
        {
            // arrange
            // act
            var authProvider = new BitfinexAuthenticationProvider(new ApiCredentials("TestKey", "TestSecret"), null);

            // assert
            Assert.AreEqual(authProvider.Credentials.Key.GetString(), "TestKey");
            Assert.AreEqual(authProvider.Credentials.Secret.GetString(), "TestSecret");
        }

        [Test]
        public void SigningString_Should_ReturnCorrectString()
        {
            // arrange
            var authProvider = new BitfinexAuthenticationProvider(new ApiCredentials("TestKey", "TestSecret"), null);

            // act
            string signed = authProvider.Sign("SomeTestString");

            // assert
            Assert.AreEqual(signed, "9052C73092B21B945BC5859CADBA6A5658E142F021FCB092A72F68E8A0D5E6351CFEBAE52DB9067D4360F796CB520960");
        }

        [Test]
        public async Task MakingAuthv2Call_Should_SendAuthHeaders()
        {
            // arrange
            var client = TestHelpers.CreateClient(new BitfinexClientOptions(){ ApiCredentials = new ApiCredentials("TestKey", "t")});
            var request = TestHelpers.SetResponse((RestClient)client, "{}");

            // act
            await client.GetActiveOrdersAsync();

            // assert
            request.Verify(r => r.AddHeader("bfx-nonce", It.IsAny<string>()));
            request.Verify(r => r.AddHeader("bfx-signature", It.IsAny<string>()));
            request.Verify(r => r.AddHeader("bfx-apikey", "TestKey"));
        }

        [Test]
        public async Task MakingAuthv1Call_Should_SendAuthHeaders()
        {
            // arrange
            var client = TestHelpers.CreateClient(new BitfinexClientOptions() { ApiCredentials = new ApiCredentials("TestKey", "t") });
            var request = TestHelpers.SetResponse((RestClient)client, "{}");

            // act
            await client.GetAccountInfoAsync();

            // assert
            request.Verify(r => r.AddHeader("X-BFX-SIGNATURE", It.IsAny<string>()));
            request.Verify(r => r.AddHeader("X-BFX-PAYLOAD", It.IsAny<string>()));
            request.Verify(r => r.AddHeader("X-BFX-APIKEY", "TestKey"));
        }

        [TestCase("tBTCUSD", true)]
        [TestCase("tNANOUSD", true)]
        [TestCase("tNANOBTC", true)]
        [TestCase("tETHBTC", true)]
        [TestCase("dETHBTC", false)]
        [TestCase("tBEETC", false)]
        [TestCase("BTC-USDT", false)]
        [TestCase("BTC-USD", false)]
        [TestCase("tBTC-USD", false)]
        [TestCase("fBTC-USD", false)]
        [TestCase("fBTC", true)]
        [TestCase("fNANO", true)]
        [TestCase("fNA", false)]
        public void CheckValidBitfinexSymbol(string symbol, bool isValid)
        {
            if (isValid)
                Assert.DoesNotThrow(symbol.ValidateBitfinexSymbol);
            else
                Assert.Throws(typeof(ArgumentException), symbol.ValidateBitfinexSymbol);
        }
    }
}
