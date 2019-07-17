#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using InitialMargin.Core;
using InitialMargin.IO;
#endregion

namespace InitialMargin.Tests
{
    [Collection("Tests")]
    public sealed class InputOutputTests
    {
        #region Members
        private readonly TestsFixture m_TestsFixture;
        #endregion

        #region Constructors
        public InputOutputTests(TestsFixture fixture)
        {
            m_TestsFixture = fixture;
        }
        #endregion

        #region Methods
        [Theory]
        [InlineData("Crif.csv")]
        public void TestInputCrif(String crifFile)
        {
            String crifFilePath = Utilities.GetStaticFilePath(crifFile);
            List<String> expected = CrifManager.Read(crifFilePath).Select(x => x.ToString()).ToList();
            List<String> actual = m_TestsFixture.DataEntities.Select(x => x.ToString()).ToList();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("FxRates.csv")]
        public void TestInputRates(String ratesFile)
        {
            FxRatesManager ratesManager = m_TestsFixture.RatesManager;
            FxRatesProvider ratesProvider = m_TestsFixture.RatesProvider;

            String ratesFilePath = Utilities.GetStaticFilePath(ratesFile);
            FxRatesProvider ratesProviderParsed = ratesManager.Read(ratesFilePath);

            Dictionary<CurrencyPair,Decimal> expected = ratesProvider.Rates
                .ToDictionary(x => x.Key, x => x.Value);

            Dictionary<CurrencyPair,Decimal> actual = ratesProviderParsed.Rates
                .ToDictionary(x => x.Key, x => x.Value);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Crif.csv")]
        public void TestOutputCrif(String crifFile)
        {
            String crifExpectedFile = Utilities.GetStaticFilePath(crifFile);
            String crifActualFile = Utilities.GetRandomFilePath(".csv");

            CrifManager.Write(crifActualFile, m_TestsFixture.DataEntities);

            String crifExpectedContent = Regex.Replace(File.ReadAllText(crifExpectedFile), @"\d{4}-\d{2}-\d{2}", String.Empty);
            String expected = Utilities.ComputeHash(crifExpectedContent);

            String crifActualContent = Regex.Replace(File.ReadAllText(crifActualFile), @"\d{4}-\d{2}-\d{2}", String.Empty);
            String actual = Utilities.ComputeHash(crifActualContent);

            try
            {
                File.Delete(crifActualFile);
            }
            catch { }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(RegulationRole.Pledgor, "CsvPledgor.csv")]
        [InlineData(RegulationRole.Secured, "CsvSecured.csv")]
        public void TestOutputCsv(RegulationRole regulationRole, String csvFile)
        {
            FxRatesProvider ratesProvider = m_TestsFixture.RatesProvider;
            ReadOnlyCollection<DataEntity> dataEntities = m_TestsFixture.DataEntities;
            OutputWriterCsv writer = m_TestsFixture.WriterCsv;

            Engine engine = Engine.Of(Currency.Usd, ratesProvider);

            String csvExpectedFile = Utilities.GetStaticFilePath(csvFile);
            String csvActualFile = Utilities.GetRandomFilePath(".csv");

            MarginTotal margin = engine.CalculateDetailed(regulationRole, dataEntities);
            writer.Write(csvActualFile, margin);

            String expected = Utilities.ComputeHash(File.ReadAllText(csvExpectedFile));
            String actual = Utilities.ComputeHash(File.ReadAllText(csvActualFile));

            try
            {
                File.Delete(csvActualFile);
            }
            catch { }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("FxRates.csv")]
        public void TestOutputRates(String ratesFile)
        {
            FxRatesManager ratesManager = m_TestsFixture.RatesManager;
            FxRatesProvider ratesProvider = m_TestsFixture.RatesProvider;

            String ratesExpectedFile = Utilities.GetStaticFilePath(ratesFile);
            String ratesActualFile = Utilities.GetRandomFilePath(".csv");

            ratesManager.Write(ratesActualFile, ratesProvider);

            String expected = Utilities.ComputeHash(File.ReadAllText(ratesExpectedFile));
            String actual = Utilities.ComputeHash(File.ReadAllText(ratesActualFile));

            try
            {
                File.Delete(ratesActualFile);
            }
            catch { }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(RegulationRole.Pledgor, "TreePledgor.txt")]
        [InlineData(RegulationRole.Secured, "TreeSecured.txt")]
        public void TestOutputTree(RegulationRole regulationRole, String treeFile)
        {
            FxRatesProvider ratesProvider = m_TestsFixture.RatesProvider;
            ReadOnlyCollection<DataEntity> dataEntities = m_TestsFixture.DataEntities;
            OutputWriterTree writer = m_TestsFixture.WriterTree;

            Engine engine = Engine.Of(Currency.Usd, ratesProvider);

            String treeExpectedFile = Utilities.GetStaticFilePath(treeFile);
            String treeActualFile = Utilities.GetRandomFilePath(".txt");
            
            MarginTotal margin = engine.CalculateDetailed(regulationRole, dataEntities);
            writer.Write(treeActualFile, margin);

            String expected = Utilities.ComputeHash(File.ReadAllText(treeExpectedFile));
            String actual = Utilities.ComputeHash(File.ReadAllText(treeActualFile));

            try
            {
                File.Delete(treeActualFile);
            }
            catch { }

            Assert.Equal(expected, actual);
        }
        #endregion
    }
}