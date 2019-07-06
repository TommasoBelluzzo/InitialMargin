#region Using Directives
using System;
using Xunit;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Tests
{
    [Collection("Tests")]
    public sealed class FxRatesTests
    {
        #region Members
        private readonly TestsFixture m_TestsFixture;
        #endregion

        #region Constructors
        public FxRatesTests(TestsFixture fixture)
        {
            m_TestsFixture = fixture;
        }
        #endregion

        #region Methods
        [Theory]
        [InlineData(0.00d, "CAD", "CAD", 0.00d, "CAD")]
        [InlineData(74250.22d, "CAD", "CAD", 74250.22d, "CAD")]
        [InlineData(138331.00d, "USD", "EUR", 123445.05d, "EUR")]
        [InlineData(123445.05d, "EUR", "USD", 138331.00d, "USD")]
        [InlineData(-8846.77d, "GBP", "JPY", -1247917.17d, "JPY")]
        [InlineData(-1247917.17d, "JPY", "GBP", -8846.77d, "GBP")]
        public void TestConversions(Double valueBase, String currencyBase, String currencyCounter, Double valueExpected, String currencyExpected)
        {
            FxRatesProvider ratesProvider = m_TestsFixture.RatesProvider;

            Amount amountBase = Amount.Of(Currency.Parse(currencyBase), Utilities.Round(valueBase, 2));
            Currency oCurrencyCounter = Currency.Parse(currencyCounter);
            
            Amount expected = Amount.Of(Currency.Parse(currencyExpected), Utilities.Round(valueExpected, 2));
            Amount actual = Amount.Round(ratesProvider.Convert(amountBase, oCurrencyCounter), 2, MidpointRounding.AwayFromZero);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("CAD", "CAD", 1.0000000000d)]
        [InlineData("USD", "EUR", 0.8923889400d)]
        [InlineData("EUR", "USD", 1.1205876218d)]
        public void TestRateNoTriangulation(String currencyBase, String currencyCounter, Double result)
        {
            FxRatesProvider ratesProvider = m_TestsFixture.RatesProvider;

            Currency oCurrencyBase= Currency.Parse(currencyBase);
            Currency oCurrencyCounter = Currency.Parse(currencyCounter);

            Decimal expected = Utilities.Round(Convert.ToDecimal(result), 10);
            Decimal actual = Utilities.Round(ratesProvider.GetRate(oCurrencyBase, oCurrencyCounter, false), 10);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("GBP", "JPY", false)]
        [InlineData("JPY", "GBP", false)]
        [InlineData("ERN", "USD", true)]
        [InlineData("USD", "ERN", true)]
        public void TestRateTriangulationKo(String currencyBase, String currencyCounter, Boolean useTriangulation)
        {
            FxRatesProvider ratesProvider = m_TestsFixture.RatesProvider;

            Currency oCurrencyBase= Currency.Parse(currencyBase);
            Currency oCurrencyCounter = Currency.Parse(currencyCounter);

            Assert.Throws<InvalidOperationException>(() => ratesProvider.GetRate(oCurrencyBase, oCurrencyCounter, useTriangulation));
        }

        [Theory]
        [InlineData("GBP", "JPY", 141.0590728637d)]
        [InlineData("JPY", "GBP", 0.0070892285d)]
        public void TestRateTriangulationOk(String currencyBase, String currencyCounter, Double result)
        {
            FxRatesProvider ratesProvider = m_TestsFixture.RatesProvider;

            Currency oCurrencyBase= Currency.Parse(currencyBase);
            Currency oCurrencyCounter = Currency.Parse(currencyCounter);

            Decimal expected = Utilities.Round(result, 10);
            Decimal actual = Utilities.Round(ratesProvider.GetRate(oCurrencyBase, oCurrencyCounter, true), 10);

            Assert.Equal(expected, actual);
        }
        #endregion
    }
}