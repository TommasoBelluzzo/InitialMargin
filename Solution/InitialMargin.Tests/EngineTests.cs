#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Tests
{
    [Collection("Tests")]
    public sealed class EngineTests
    {
        #region Members
        private readonly TestsFixture m_TestsFixture;
        #endregion

        #region Constructors
        public EngineTests(TestsFixture fixture)
        {
            m_TestsFixture = fixture;
        }
        #endregion

        #region Methods
        [Fact]
        public void TestConsistency()
        {
            FxRatesProvider ratesProvider = m_TestsFixture.RatesProvider;
            List<DataEntity> dataEntities = new List<DataEntity>(m_TestsFixture.DataEntities);

            Amount amount = Engine.Calculate(RegulationRole.Secured, DateTime.Today, Currency.Usd, ratesProvider, dataEntities);

            List<Amount> amounts = new List<Amount>(3)
            {
                Engine.CalculateByRole(RegulationRole.Secured, DateTime.Today, Currency.Usd, ratesProvider, dataEntities).Single().Value,
                Engine.CalculateWorstOf(RegulationRole.Secured, DateTime.Today, Currency.Usd, ratesProvider, dataEntities)
            };

            Assert.True(amounts.TrueForAll(x => x.Equals(amount)));
        }

        [Theory]
        [InlineData(RegulationRole.Pledgor, -9168183562.00d)]
        [InlineData(RegulationRole.Secured, 11204843707.00d)]
        public void TestResult(RegulationRole regulationRole, Double result)
        {
            FxRatesProvider ratesProvider = m_TestsFixture.RatesProvider;
            List<DataEntity> dataEntities = new List<DataEntity>(m_TestsFixture.DataEntities);

            Decimal expected = Convert.ToDecimal(result);
            Decimal actual = Math.Round(Engine.Calculate(regulationRole, DateTime.Today, Currency.Usd, ratesProvider, dataEntities).Value, 0, MidpointRounding.AwayFromZero);

            Assert.Equal(expected, actual);
        }
        #endregion
    }
}