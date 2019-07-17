#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            ReadOnlyCollection<DataEntity> dataEntities = m_TestsFixture.DataEntities;

            Engine engine = Engine.Of(Currency.Usd, ratesProvider);

            Amount amount = engine.Calculate(RegulationRole.Secured, dataEntities);

            List<Amount> amounts = new List<Amount>(3)
            {
                engine.CalculateByRole(RegulationRole.Secured, dataEntities).Single().Value,
                engine.CalculateWorstOf(RegulationRole.Secured, dataEntities)
            };

            Assert.True(amounts.TrueForAll(x => x.Equals(amount)));
        }

        [Theory]
        [InlineData(RegulationRole.Pledgor, -9168183562.00d)]
        [InlineData(RegulationRole.Secured, 11204843707.00d)]
        public void TestResult(RegulationRole regulationRole, Double result)
        {
            FxRatesProvider ratesProvider = m_TestsFixture.RatesProvider;
            ReadOnlyCollection<DataEntity> dataEntities = m_TestsFixture.DataEntities;

            Engine engine = Engine.Of(Currency.Usd, ratesProvider);

            Decimal expected = Convert.ToDecimal(result);
            Decimal actual = Math.Round(engine.Calculate(regulationRole, dataEntities).Value, 0, MidpointRounding.AwayFromZero);

            Assert.Equal(expected, actual);
        }
        #endregion
    }
}