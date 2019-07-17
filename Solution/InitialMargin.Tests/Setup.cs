#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Xunit;
using InitialMargin.Core;
using InitialMargin.IO;
using InitialMargin.Model;
using InitialMargin.Schedule;
#endregion

namespace InitialMargin.Tests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class TestsFixture : IDisposable
    {
        #region Members
        private readonly FxRatesManager m_RatesManager;
        private readonly OutputWriterCsv m_WriterCsv;
        private readonly OutputWriterTree m_WriterTree;
        private readonly ReadOnlyCollection<DataEntity> m_DataEntities;
        #endregion

        #region Properties
        public FxRatesManager RatesManager => m_RatesManager;

        public FxRatesProvider RatesProvider => CreateRatesProvider();

        public OutputWriterCsv WriterCsv => m_WriterCsv;

        public OutputWriterTree WriterTree => m_WriterTree;

        public ReadOnlyCollection<DataEntity> DataEntities => m_DataEntities;
        #endregion

        #region Constructors
        public TestsFixture()
        {
            m_RatesManager = FxRatesManager.Of(Encoding.UTF8, CultureInfo.InvariantCulture);
            m_WriterCsv = OutputWriterCsv.Of(Encoding.UTF8, CultureInfo.InvariantCulture);
            m_WriterTree = OutputWriterTree.Of(Encoding.UTF8, CultureInfo.InvariantCulture);
            m_DataEntities = CreateDataEntities();
        }
        #endregion

        #region Methods
        public void Dispose() { }
        #endregion

        #region Methods (Static)
        private static FxRatesProvider CreateRatesProvider()
        {
            FxRatesProvider ratesProvider = new FxRatesProvider();
            ratesProvider.AddRate(Currency.Usd, Currency.Aed, 3.6725m);
            ratesProvider.AddRate(Currency.Usd, Currency.Afn, 78.5310333944m);
            ratesProvider.AddRate(Currency.Usd, Currency.All, 109.9428418518m);
            ratesProvider.AddRate(Currency.Usd, Currency.Amd, 480.5582473128m);
            ratesProvider.AddRate(Currency.Usd, Currency.Ang, 1.7899999938m);
            ratesProvider.AddRate(Currency.Usd, Currency.Aoa, 326.6060255636m);
            ratesProvider.AddRate(Currency.Usd, Currency.Ars, 45.0412132395m);
            ratesProvider.AddRate(Currency.Usd, Currency.Aud, 1.4432844156m);
            ratesProvider.AddRate(Currency.Usd, Currency.Awg, 1.79m);
            ratesProvider.AddRate(Currency.Usd, Currency.Azn, 1.696492363m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bam, 1.7453610606m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bbd, 2m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bdt, 84.4767286935m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bgn, 1.7453610606m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bhd, 0.376m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bif, 1836.0857892173m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bmd, 1m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bnd, 1.3687250827m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bob, 6.906313269m);
            ratesProvider.AddRate(Currency.Usd, Currency.Brl, 3.9871915722m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bsd, 1m);
            ratesProvider.AddRate(Currency.Usd, Currency.Btn, 70.2600231444m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bwp, 10.6937002014m);
            ratesProvider.AddRate(Currency.Usd, Currency.Byn, 2.0891453577m);
            ratesProvider.AddRate(Currency.Usd, Currency.Bzd, 2.0141543654m);
            ratesProvider.AddRate(Currency.Usd, Currency.Cad, 1.3436510634m);
            ratesProvider.AddRate(Currency.Usd, Currency.Cdf, 1648.3875977128m);
            ratesProvider.AddRate(Currency.Usd, Currency.Chf, 1.009007713m);
            ratesProvider.AddRate(Currency.Usd, Currency.Clp, 692.3058554072m);
            ratesProvider.AddRate(Currency.Usd, Currency.Cny, 6.8754746235m);
            ratesProvider.AddRate(Currency.Usd, Currency.Cop, 3291.62306082m);
            ratesProvider.AddRate(Currency.Usd, Currency.Crc, 594.6918709224m);
            ratesProvider.AddRate(Currency.Usd, Currency.Cup, 26.5m);
            ratesProvider.AddRate(Currency.Usd, Currency.Cve, 98.4037284167m);
            ratesProvider.AddRate(Currency.Usd, Currency.Czk, 22.9538661477m);
            ratesProvider.AddRate(Currency.Usd, Currency.Djf, 177.7698994864m);
            ratesProvider.AddRate(Currency.Usd, Currency.Dkk, 6.6630941262m);
            ratesProvider.AddRate(Currency.Usd, Currency.Dop, 50.67066023m);
            ratesProvider.AddRate(Currency.Usd, Currency.Dzd, 119.753693494m);
            ratesProvider.AddRate(Currency.Usd, Currency.Egp, 17.0798292155m);
            ratesProvider.AddRate(Currency.Usd, Currency.Etb, 28.9968088106m);
            ratesProvider.AddRate(Currency.Usd, Currency.Eur, 0.89238894m);
            ratesProvider.AddRate(Currency.Usd, Currency.Fjd, 2.1568427232m);
            ratesProvider.AddRate(Currency.Usd, Currency.Fkp, 0.776923679m);
            ratesProvider.AddRate(Currency.Usd, Currency.Gbp, 0.776923679m);
            ratesProvider.AddRate(Currency.Usd, Currency.Gel, 2.7439449439m);
            ratesProvider.AddRate(Currency.Usd, Currency.Ghs, 5.1602827518m);
            ratesProvider.AddRate(Currency.Usd, Currency.Gip, 0.776923679m);
            ratesProvider.AddRate(Currency.Usd, Currency.Gmd, 49.6033761727m);
            ratesProvider.AddRate(Currency.Usd, Currency.Gnf, 9223.6167422552m);
            ratesProvider.AddRate(Currency.Usd, Currency.Gtq, 7.6534246994m);
            ratesProvider.AddRate(Currency.Usd, Currency.Gyd, 209.4767888422m);
            ratesProvider.AddRate(Currency.Usd, Currency.Hkd, 7.8495450225m);
            ratesProvider.AddRate(Currency.Usd, Currency.Hnl, 24.4354235709m);
            ratesProvider.AddRate(Currency.Usd, Currency.Hrk, 6.6231045402m);
            ratesProvider.AddRate(Currency.Usd, Currency.Htg, 87.2867040791m);
            ratesProvider.AddRate(Currency.Usd, Currency.Huf, 289.739854453m);
            ratesProvider.AddRate(Currency.Usd, Currency.Idr, 14449.5907749033m);
            ratesProvider.AddRate(Currency.Usd, Currency.Ils, 3.5698607201m);
            ratesProvider.AddRate(Currency.Usd, Currency.Inr, 70.2600231444m);
            ratesProvider.AddRate(Currency.Usd, Currency.Iqd, 1195.9945328397m);
            ratesProvider.AddRate(Currency.Usd, Currency.Irr, 42103.6181049991m);
            ratesProvider.AddRate(Currency.Usd, Currency.Isk, 122.4311723316m);
            ratesProvider.AddRate(Currency.Usd, Currency.Jmd, 135.6515136135m);
            ratesProvider.AddRate(Currency.Usd, Currency.Jod, 0.709m);
            ratesProvider.AddRate(Currency.Usd, Currency.Jpy, 109.5921338456m);
            ratesProvider.AddRate(Currency.Usd, Currency.Kes, 101.0653013309m);
            ratesProvider.AddRate(Currency.Usd, Currency.Kgs, 69.8502684734m);
            ratesProvider.AddRate(Currency.Usd, Currency.Khr, 4061.2711289118m);
            ratesProvider.AddRate(Currency.Usd, Currency.Kmf, 439.0265789494m);
            ratesProvider.AddRate(Currency.Usd, Currency.Kpw, 900.0815039499m);
            ratesProvider.AddRate(Currency.Usd, Currency.Krw, 1187.6063062343m);
            ratesProvider.AddRate(Currency.Usd, Currency.Kwd, 0.3042077699m);
            ratesProvider.AddRate(Currency.Usd, Currency.Kyd, 0.8199999711m);
            ratesProvider.AddRate(Currency.Usd, Currency.Kzt, 379.5988461204m);
            ratesProvider.AddRate(Currency.Usd, Currency.Lak, 8684.956674892m);
            ratesProvider.AddRate(Currency.Usd, Currency.Lbp, 1507.5m);
            ratesProvider.AddRate(Currency.Usd, Currency.Lkr, 176.2598885557m);
            ratesProvider.AddRate(Currency.Usd, Currency.Lrd, 179.1975234039m);
            ratesProvider.AddRate(Currency.Usd, Currency.Lsl, 14.199591614m);
            ratesProvider.AddRate(Currency.Usd, Currency.Lyd, 1.3996581554m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mad, 9.6455562128m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mdl, 17.8999249372m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mga, 3603.3783484999m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mkd, 54.9892842944m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mmk, 1527.3447085636m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mnt, 2641.5839544202m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mop, 8.0850313732m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mru, 36.6981098071m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mur, 35.1553187233m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mvr, 15.3886642328m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mwk, 724.7354535121m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mxn, 19.0718694767m);
            ratesProvider.AddRate(Currency.Usd, Currency.Myr, 4.173265079m);
            ratesProvider.AddRate(Currency.Usd, Currency.Mzn, 63.6898302664m);
            ratesProvider.AddRate(Currency.Usd, Currency.Nad, 14.199591614m);
            ratesProvider.AddRate(Currency.Usd, Currency.Ngn, 359.9966790011m);
            ratesProvider.AddRate(Currency.Usd, Currency.Nio, 33.1270570298m);
            ratesProvider.AddRate(Currency.Usd, Currency.Nok, 8.7204439365m);
            ratesProvider.AddRate(Currency.Usd, Currency.Npr, 112.9429872046m);
            ratesProvider.AddRate(Currency.Usd, Currency.Nzd, 1.5242499724m);
            ratesProvider.AddRate(Currency.Usd, Currency.Omr, 0.3845m);
            ratesProvider.AddRate(Currency.Usd, Currency.Pab, 1m);
            ratesProvider.AddRate(Currency.Usd, Currency.Pen, 3.3231897163m);
            ratesProvider.AddRate(Currency.Usd, Currency.Pgk, 3.3783976041m);
            ratesProvider.AddRate(Currency.Usd, Currency.Php, 52.3089323526m);
            ratesProvider.AddRate(Currency.Usd, Currency.Pkr, 141.613183941m);
            ratesProvider.AddRate(Currency.Usd, Currency.Pln, 3.8402645992m);
            ratesProvider.AddRate(Currency.Usd, Currency.Pyg, 6356.2253938512m);
            ratesProvider.AddRate(Currency.Usd, Currency.Qar, 3.64m);
            ratesProvider.AddRate(Currency.Usd, Currency.Ron, 4.2488317391m);
            ratesProvider.AddRate(Currency.Usd, Currency.Rsd, 105.2421166253m);
            ratesProvider.AddRate(Currency.Usd, Currency.Rub, 64.5309626263m);
            ratesProvider.AddRate(Currency.Usd, Currency.Rwf, 905.9739553945m);
            ratesProvider.AddRate(Currency.Usd, Currency.Sar, 3.75m);
            ratesProvider.AddRate(Currency.Usd, Currency.Sbd, 8.0413360072m);
            ratesProvider.AddRate(Currency.Usd, Currency.Scr, 13.6115590477m);
            ratesProvider.AddRate(Currency.Usd, Currency.Sdg, 45.0960499485m);
            ratesProvider.AddRate(Currency.Usd, Currency.Sek, 9.6029787646m);
            ratesProvider.AddRate(Currency.Usd, Currency.Sgd, 1.3687250827m);
            ratesProvider.AddRate(Currency.Usd, Currency.Shp, 0.776923679m);
            ratesProvider.AddRate(Currency.Usd, Currency.Sll, 8874.1549767724m);
            ratesProvider.AddRate(Currency.Usd, Currency.Sos, 580.4139856458m);
            ratesProvider.AddRate(Currency.Usd, Currency.Srd, 7.4574277811m);
            ratesProvider.AddRate(Currency.Usd, Currency.Stn, 21.8786640996m);
            ratesProvider.AddRate(Currency.Usd, Currency.Svc, 8.75m);
            ratesProvider.AddRate(Currency.Usd, Currency.Syp, 514.9989266974m);
            ratesProvider.AddRate(Currency.Usd, Currency.Szl, 14.199591614m);
            ratesProvider.AddRate(Currency.Usd, Currency.Thb, 31.5504179073m);
            ratesProvider.AddRate(Currency.Usd, Currency.Tjs, 9.4394984415m);
            ratesProvider.AddRate(Currency.Usd, Currency.Tmt, 3.5000004519m);
            ratesProvider.AddRate(Currency.Usd, Currency.Tnd, 2.993029268m);
            ratesProvider.AddRate(Currency.Usd, Currency.Top, 2.2760793695m);
            ratesProvider.AddRate(Currency.Usd, Currency.Try, 6.0045772074m);
            ratesProvider.AddRate(Currency.Usd, Currency.Ttd, 6.7718865774m);
            ratesProvider.AddRate(Currency.Usd, Currency.Tvd, 1.4432844156m);
            ratesProvider.AddRate(Currency.Usd, Currency.Twd, 31.0892037106m);
            ratesProvider.AddRate(Currency.Usd, Currency.Tzs, 2298.999049136m);
            ratesProvider.AddRate(Currency.Usd, Currency.Uah, 26.3635670612m);
            ratesProvider.AddRate(Currency.Usd, Currency.Ugx, 3772.9608374011m);
            ratesProvider.AddRate(Currency.Usd, Currency.Uyu, 35.2496745779m);
            ratesProvider.AddRate(Currency.Usd, Currency.Uzs, 8458.1403796178m);
            ratesProvider.AddRate(Currency.Usd, Currency.Ves, 5245.403525587m);
            ratesProvider.AddRate(Currency.Usd, Currency.Vnd, 23316.2645357692m);
            ratesProvider.AddRate(Currency.Usd, Currency.Vuv, 115.9205515476m);
            ratesProvider.AddRate(Currency.Usd, Currency.Wst, 2.634341304m);
            ratesProvider.AddRate(Currency.Usd, Currency.Yer, 250.3138618604m);
            ratesProvider.AddRate(Currency.Usd, Currency.Zar, 14.199591614m);
            ratesProvider.AddRate(Currency.Usd, Currency.Zmw, 13.4769973161m);
            ratesProvider.AddRate(Currency.Usd, Currency.Zwl, 361.9m);

            return ratesProvider;
        }

        private static ReadOnlyCollection<DataEntity> CreateDataEntities()
        {
            DateTime today = DateTime.Today;
            DateTime todayPlus3M = today.AddMonths(3);
            DateTime todayPlus4Y = today.AddYears(4);
            DateTime todayPlus10Y = today.AddYears(10);

            Int32 tradeId = 0;

            RegulationsInfo ri = RegulationsInfo.Of(Regulation.Cftc);

            List<DataEntity> dataEntities = new List<DataEntity>
            {
                // Model - Sensitivities - Base Correlation
                Sensitivity.BaseCorrelation("CDX HY", Amount.Of(Currency.Usd, 200000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.BaseCorrelation("CDX HY", Amount.Of(Currency.Usd, -300000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.BaseCorrelation("CDX IG", Amount.Of(Currency.Usd, 100000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.BaseCorrelation("iTraxx XO", Amount.Of(Currency.Usd, 450000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Sensitivities - Delta - Commodity
                Sensitivity.CommodityDelta("Coal Americas", BucketCommodity.Bucket1, Amount.Of(Currency.Usd, 3000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityDelta("Coal Europe", BucketCommodity.Bucket1, Amount.Of(Currency.Usd, -5000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityDelta("Middle Distillates America", BucketCommodity.Bucket3, Amount.Of(Currency.Usd, 15000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityDelta("Middle Distillates Europe", BucketCommodity.Bucket3, Amount.Of(Currency.Usd, -20000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityDelta("Middle Distillates Asia", BucketCommodity.Bucket3, Amount.Of(Currency.Usd, 32000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityDelta("NA Natural Gas Gulf Coast", BucketCommodity.Bucket6, Amount.Of(Currency.Usd, 6500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityDelta("NA Natural Gas West", BucketCommodity.Bucket6, Amount.Of(Currency.Usd, 4000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityDelta("Freight Wet", BucketCommodity.Bucket10, Amount.Of(Currency.Usd, 35000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityDelta("Freight Dry", BucketCommodity.Bucket10, Amount.Of(Currency.Usd, -10000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityDelta("Softs Coffee", BucketCommodity.Bucket14, Amount.Of(Currency.Usd, 2500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityDelta("Livestock Feeder Cattle", BucketCommodity.Bucket15, Amount.Of(Currency.Usd, -1200000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Sensitivities - Delta - Credit Qualifying
                Sensitivity.CreditQualifyingDelta("ISIN:US3949181045", BucketCreditQualifying.Bucket1, Tenor.Y2, false, Amount.Of(Currency.Usd, 100000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:US3949181045", BucketCreditQualifying.Bucket1, Tenor.Y5, false, Amount.Of(Currency.Usd, -1500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:US3949181045", BucketCreditQualifying.Bucket1, Tenor.Y3, false, Amount.Of(Currency.Usd, 1450000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:XS1061333921", BucketCreditQualifying.Bucket1, Tenor.Y10, false, Amount.Of(Currency.Usd, 400000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:CH0419041295", BucketCreditQualifying.Bucket4, Tenor.Y1, true, Amount.Of(Currency.Usd, -650000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:JP1718441K71", BucketCreditQualifying.Bucket4, Tenor.Y1, false, Amount.Of(Currency.Usd, 200000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:RU000A100H37", BucketCreditQualifying.Bucket7, Tenor.Y2, false, Amount.Of(Currency.Usd, -100000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:RU000A100H37", BucketCreditQualifying.Bucket7, Tenor.Y3, false, Amount.Of(Currency.Usd, 150000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:XS1886258598", BucketCreditQualifying.Bucket7, Tenor.Y1, false, Amount.Of(Currency.Usd, -120000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:XS1120672716", BucketCreditQualifying.Bucket7, Tenor.Y2, false, Amount.Of(Currency.Usd, 330000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:GB0702134646", BucketCreditQualifying.Bucket11, Tenor.Y2, false, Amount.Of(Currency.Usd, 1000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:GB0702134646", BucketCreditQualifying.Bucket11, Tenor.Y10, false, Amount.Of(Currency.Usd, -475000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:GB0702134646", BucketCreditQualifying.Bucket11, Tenor.Y3, false, Amount.Of(Currency.Usd, -500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:FR0003504418", BucketCreditQualifying.BucketResidual, Tenor.Y2, false, Amount.Of(Currency.Usd, 400000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingDelta("ISIN:XS2023221601", BucketCreditQualifying.BucketResidual, Tenor.Y10, false, Amount.Of(Currency.Usd, -300000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Sensitivities - Delta - Credit Non-qualifying
                Sensitivity.CreditNonQualifyingDelta("ISIN:US3949181045", BucketCreditNonQualifying.Bucket1, Tenor.Y3, Amount.Of(Currency.Usd, 300000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditNonQualifyingDelta("ISIN:US3949181045", BucketCreditNonQualifying.Bucket1, Tenor.Y5, Amount.Of(Currency.Usd, 700000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditNonQualifyingDelta("ISIN:XS1061333921", BucketCreditNonQualifying.Bucket1, Tenor.Y2, Amount.Of(Currency.Usd, -500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditNonQualifyingDelta("ISIN:XS1980681200", BucketCreditNonQualifying.Bucket2, Tenor.Y1, Amount.Of(Currency.Usd, -220000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditNonQualifyingDelta("ISIN:XS1980681200", BucketCreditNonQualifying.Bucket2, Tenor.Y10, Amount.Of(Currency.Usd, 200000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditNonQualifyingDelta("ISIN:DE000HVB34J9", BucketCreditNonQualifying.Bucket2, Tenor.Y2, Amount.Of(Currency.Usd, 800000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditNonQualifyingDelta("ISIN:FR0003507418", BucketCreditNonQualifying.BucketResidual, Tenor.Y5, Amount.Of(Currency.Usd, 300000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditNonQualifyingDelta("ISIN:XS2023221601", BucketCreditNonQualifying.BucketResidual, Tenor.Y3, Amount.Of(Currency.Usd, -270000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Sensitivities - Delta - Equity
                Sensitivity.EquityDelta("ISIN:XS1980681200", BucketEquity.Bucket2, Amount.Of(Currency.Usd, 200000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityDelta("ISIN:DE000HVB34J9", BucketEquity.Bucket2, Amount.Of(Currency.Usd, 500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityDelta("ISIN:DE000TK0T291", BucketEquity.Bucket5, Amount.Of(Currency.Usd, -1000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityDelta("ISIN:DE000FF2AKA7", BucketEquity.Bucket5, Amount.Of(Currency.Usd, 3300000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityDelta("ISIN:XS1896686544", BucketEquity.Bucket5, Amount.Of(Currency.Usd, 500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityDelta("ISIN:BE5631439402", BucketEquity.Bucket9, Amount.Of(Currency.Usd, 100000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityDelta("ISIN:XS1006556794", BucketEquity.Bucket10, Amount.Of(Currency.Usd, 50000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityDelta("ISIN:GB02BALKJB79", BucketEquity.Bucket10, Amount.Of(Currency.Usd, -150000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityDelta("FTSE100", BucketEquity.Bucket11, Amount.Of(Currency.Usd, 600000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityDelta("S&P500", BucketEquity.Bucket11, Amount.Of(Currency.Usd, -450000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityDelta("ISIN:FR0003504418", BucketEquity.BucketResidual, Amount.Of(Currency.Usd, -75000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityDelta("ISIN:XS2023221601", BucketEquity.BucketResidual, Amount.Of(Currency.Usd, 56000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Sensitivities - Delta - FX
                Sensitivity.FxDelta(Currency.Eur, Amount.Of(Currency.Usd, 10000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.FxDelta(Currency.Eur, Amount.Of(Currency.Usd, -10000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.FxDelta(Currency.Nok, Amount.Of(Currency.Usd, 200000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.FxDelta(Currency.Mru, Amount.Of(Currency.Usd, 60000000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Sensitivities - Delta - Rates - Cross-currency Basis
                Sensitivity.CrossCurrencyBasis(Currency.Gbp, Amount.Of(Currency.Usd, -15000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CrossCurrencyBasis(Currency.Usd, Amount.Of(Currency.Usd, 10000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CrossCurrencyBasis(Currency.Usd, Amount.Of(Currency.Usd, 10000000m), ri, CreateTradeInfoModel(++tradeId)),

                // Sensitivities - Delta - Rates - Inflation
                Sensitivity.InflationDelta(Currency.Cad, Amount.Of(Currency.Usd, 3000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.InflationDelta(Currency.Gbp, Amount.Of(Currency.Usd, -10000000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Sensitivities - Delta - Rates - Interest Rate
                Sensitivity.InterestRateDelta(Currency.Gbp, Tenor.Y5, Curve.Libor6M, Amount.Of(Currency.Usd, -5000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.InterestRateDelta(Currency.Gbp, Tenor.M6, Curve.Ois, Amount.Of(Currency.Usd, 20000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.InterestRateDelta(Currency.Gbp, Tenor.Y2, Curve.Ois, Amount.Of(Currency.Usd, 30000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.InterestRateDelta(Currency.Jpy, Tenor.Y10, Curve.Libor3M, Amount.Of(Currency.Usd, 9000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.InterestRateDelta(Currency.Jpy, Tenor.Y20, Curve.Libor3M, Amount.Of(Currency.Usd, 1000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.InterestRateDelta(Currency.Inr, Tenor.W2, Curve.Ois, Amount.Of(Currency.Usd, -2000000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Sensitivities - Vega - Commodity
                Sensitivity.CommodityVega("Precious Metals Silver", BucketCommodity.Bucket12, Tenor.W2, Amount.Of(Currency.Usd, 3500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityVega("Precious Metals Silver", BucketCommodity.Bucket12, Tenor.Y5, Amount.Of(Currency.Usd, -3200000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityVega("Precious Metals Gold", BucketCommodity.Bucket12, Tenor.M6, Amount.Of(Currency.Usd, 1500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityVega("EU Power Germany", BucketCommodity.Bucket9, Tenor.Y1, Amount.Of(Currency.Usd, 1000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityVega("EU Power Germany", BucketCommodity.Bucket9, Tenor.W2, Amount.Of(Currency.Usd, 4500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CommodityVega("Crude Oil Americas", BucketCommodity.Bucket2, Tenor.Y15, Amount.Of(Currency.Usd, -1500000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Sensitivities - Vega - Credit Qualifying
                Sensitivity.CreditQualifyingVega("EU.HY", BucketCreditQualifying.BucketResidual, Tenor.Y5, Amount.Of(Currency.Usd, -4000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingVega("EU.IG", BucketCreditQualifying.BucketResidual, Tenor.Y1, Amount.Of(Currency.Usd, 1500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingVega("EU.IG", BucketCreditQualifying.Bucket2, Tenor.Y2, Amount.Of(Currency.Usd, 3000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingVega("EU.IG", BucketCreditQualifying.Bucket2, Tenor.Y2, Amount.Of(Currency.Usd, -2000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditQualifyingVega("US.HY", BucketCreditQualifying.Bucket7, Tenor.Y10, Amount.Of(Currency.Usd, 3500000m), ri, CreateTradeInfoModel(++tradeId)),
                
                // Model - Sensitivities - Vega - Credit Non-qualifying
                Sensitivity.CreditNonQualifyingVega("EU.HY", BucketCreditNonQualifying.BucketResidual, Tenor.Y3, Amount.Of(Currency.Usd, 1200000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditNonQualifyingVega("EU.IG", BucketCreditNonQualifying.Bucket1, Tenor.Y2, Amount.Of(Currency.Usd, -2300000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditNonQualifyingVega("EU.IG", BucketCreditNonQualifying.BucketResidual, Tenor.Y3, Amount.Of(Currency.Usd, 4000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditNonQualifyingVega("EU.IG", BucketCreditNonQualifying.BucketResidual, Tenor.Y1, Amount.Of(Currency.Usd, -3500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.CreditNonQualifyingVega("US.HY", BucketCreditNonQualifying.BucketResidual, Tenor.Y10, Amount.Of(Currency.Usd, -2600000m), ri, CreateTradeInfoModel(++tradeId)),
                
                // Model - Sensitivities - Vega - Equity
                Sensitivity.EquityVega("ISIN:US3949181045", BucketEquity.Bucket1, Tenor.Y5, Amount.Of(Currency.Usd, 2000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityVega("ISIN:US3949181045", BucketEquity.Bucket1, Tenor.Y2, Amount.Of(Currency.Usd, -1800000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityVega("ISIN:US3949181045", BucketEquity.Bucket1, Tenor.W2, Amount.Of(Currency.Usd, 6000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityVega("ISIN:CH0419041295", BucketEquity.Bucket4, Tenor.Y3, Amount.Of(Currency.Usd, 5000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityVega("ISIN:DE000HVB34J9", BucketEquity.Bucket4, Tenor.Y10, Amount.Of(Currency.Usd, -4500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityVega("ISIN:RU000A100H37", BucketEquity.Bucket7, Tenor.Y20, Amount.Of(Currency.Usd, -2700000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityVega("VIX", BucketEquity.Bucket12, Tenor.Y15, Amount.Of(Currency.Usd, -1500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityVega("ISIN:FR0003504418", BucketEquity.BucketResidual, Tenor.M6, Amount.Of(Currency.Usd, 3000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityVega("ISIN:FR0003504418", BucketEquity.BucketResidual, Tenor.M1, Amount.Of(Currency.Usd, -3200000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.EquityVega("ISIN:XS2023221601", BucketEquity.BucketResidual, Tenor.M3, Amount.Of(Currency.Usd, 4400000m), ri, CreateTradeInfoModel(++tradeId)),
                
                // Model - Sensitivities - Vega - FX
                Sensitivity.FxVega(CurrencyPair.Of(Currency.Usd, Currency.Eur), Tenor.M6, Amount.Of(Currency.Usd, 2500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.FxVega(CurrencyPair.Of(Currency.Eur, Currency.Usd), Tenor.Y3, Amount.Of(Currency.Usd, 8500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.FxVega(CurrencyPair.Of(Currency.Usd, Currency.Eur), Tenor.Y20, Amount.Of(Currency.Usd, -10000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.FxVega(CurrencyPair.Of(Currency.Brl, Currency.Cny), Tenor.Y2, Amount.Of(Currency.Usd, 6500000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.FxVega(CurrencyPair.Of(Currency.Jpy, Currency.Sgd), Tenor.M6, Amount.Of(Currency.Usd, -2500000m), ri, CreateTradeInfoModel(++tradeId)),
                
                // Sensitivities - Delta - Vega - Inflation
                Sensitivity.InflationVega(Currency.Eur, Tenor.Y5, Amount.Of(Currency.Usd, 15000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.InflationVega(Currency.Eur, Tenor.Y15, Amount.Of(Currency.Usd, -50000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.InflationVega(Currency.Usd, Tenor.Y20, Amount.Of(Currency.Usd, 100000000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Sensitivities - Vega - Rates - Interest Rate
                Sensitivity.InterestRateVega(Currency.Usd, Tenor.M3, Amount.Of(Currency.Usd, 20000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.InterestRateVega(Currency.Usd, Tenor.Y1, Amount.Of(Currency.Usd, -30000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.InterestRateVega(Currency.Aud, Tenor.W2, Amount.Of(Currency.Usd, -13000000m), ri, CreateTradeInfoModel(++tradeId)),
                Sensitivity.InterestRateVega(Currency.Aud, Tenor.Y2, Amount.Of(Currency.Usd, -2500000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Add-on - Fixed Amounts
                AddOnFixedAmount.Of(Amount.Of(Currency.Usd, 1650000m), ri),
                AddOnFixedAmount.Of(Amount.Of(Currency.Usd, 2000000m), ri),

                // Model - Add-on - Notionals
                AddOnNotional.Of("FlexiCallOption", Amount.Of(Currency.Usd, 1000000m), ri, CreateTradeInfoModel(++tradeId)),
                AddOnNotional.Of("FlexiCallOption", Amount.Of(Currency.Usd, -2000000m), ri, CreateTradeInfoModel(++tradeId)),
                AddOnNotional.Of("FlexiCallOption", Amount.Of(Currency.Usd, 1500000m), ri, CreateTradeInfoModel(++tradeId)),
                AddOnNotional.Of("FlexiPutOption", Amount.Of(Currency.Usd, -800000m), ri, CreateTradeInfoModel(++tradeId)),
                AddOnNotional.Of("FlexiPutOption", Amount.Of(Currency.Usd, -2200000m), ri, CreateTradeInfoModel(++tradeId)),

                // Model - Add-on - Notional Factors
                AddOnNotionalFactor.Of("FlexiCallOption", 0.06105m, ri),
                AddOnNotionalFactor.Of("FlexiPutOption", 0.02055m, ri),

                // Model - Add-on - Product Multipliers
                AddOnProductMultiplier.Of(Model.Product.Commodity, 0.075m, ri),
                AddOnProductMultiplier.Of(Model.Product.Credit, 0.11m, ri),
                AddOnProductMultiplier.Of(Model.Product.Equity, 0.015m, ri),
                AddOnProductMultiplier.Of(Model.Product.RatesFx, 0.205m, ri),

                // Schedule - Notionals
                Notional.Of(Schedule.Product.Commodity, Amount.Of(Currency.Cad, 1850000m), ri, CreateTradeInfoSchedule(1, todayPlus3M)),
                Notional.Of(Schedule.Product.Credit, Amount.Of(Currency.Usd, 2300000m), ri, CreateTradeInfoSchedule(2, todayPlus3M)),
                Notional.Of(Schedule.Product.Credit, Amount.Of(Currency.Usd, -900000m), ri, CreateTradeInfoSchedule(3, todayPlus10Y)),
                Notional.Of(Schedule.Product.Fx, Amount.Of(Currency.Usd, -5200000m), ri, CreateTradeInfoSchedule(4, todayPlus3M)),
                Notional.Of(Schedule.Product.Equity, Amount.Of(Currency.Usd, 1200000m), ri, CreateTradeInfoSchedule(5, todayPlus4Y)),
                Notional.Of(Schedule.Product.Rates, Amount.Of(Currency.Usd, 3700000m), ri, CreateTradeInfoSchedule(6, todayPlus3M)),
                Notional.Of(Schedule.Product.Rates, Amount.Of(Currency.Usd, -2000000m), ri, CreateTradeInfoSchedule(7, todayPlus4Y)),
                Notional.Of(Schedule.Product.Rates, Amount.Of(Currency.Gbp, -1750000m), ri, CreateTradeInfoSchedule(8, todayPlus10Y)),
                Notional.Of(Schedule.Product.Rates, Amount.Of(Currency.Eur, 1620000m), ri, CreateTradeInfoSchedule(8, todayPlus10Y)),
                Notional.Of(Schedule.Product.Other, Amount.Of(Currency.Usd, -120000m), ri, CreateTradeInfoSchedule(9, todayPlus3M)),
                Notional.Of(Schedule.Product.Other, Amount.Of(Currency.Usd, 2140000m), ri, CreateTradeInfoSchedule(9, todayPlus3M)),
                Notional.Of(Schedule.Product.Other, Amount.Of(Currency.Usd, 4350000m), ri, CreateTradeInfoSchedule(10, todayPlus4Y)),
                Notional.Of(Schedule.Product.Other, Amount.Of(Currency.Usd, 4100000m), ri, CreateTradeInfoSchedule(11, todayPlus10Y)),
                Notional.Of(Schedule.Product.Other, Amount.Of(Currency.Usd, -5120000m), ri, CreateTradeInfoSchedule(11, todayPlus10Y)),

                // Schedule - Present Values
                PresentValue.Of(Schedule.Product.Commodity, Amount.Of(Currency.Usd, 490000m), ri, CreateTradeInfoSchedule(1, todayPlus3M)),
                PresentValue.Of(Schedule.Product.Credit, Amount.Of(Currency.Usd, -60000m), ri, CreateTradeInfoSchedule(2, todayPlus3M)),
                PresentValue.Of(Schedule.Product.Credit, Amount.Of(Currency.Usd, 610000m), ri, CreateTradeInfoSchedule(3, todayPlus10Y)),
                PresentValue.Of(Schedule.Product.Fx, Amount.Of(Currency.Usd, 320000m), ri, CreateTradeInfoSchedule(4, todayPlus3M)),
                PresentValue.Of(Schedule.Product.Equity, Amount.Of(Currency.Usd, -230000m), ri, CreateTradeInfoSchedule(5, todayPlus4Y)),
                PresentValue.Of(Schedule.Product.Rates, Amount.Of(Currency.Usd, 1850000m), ri, CreateTradeInfoSchedule(6, todayPlus3M)),
                PresentValue.Of(Schedule.Product.Rates, Amount.Of(Currency.Usd, -450000m), ri, CreateTradeInfoSchedule(7, todayPlus4Y)),
                PresentValue.Of(Schedule.Product.Rates, Amount.Of(Currency.Gbp, -170000m), ri, CreateTradeInfoSchedule(8, todayPlus10Y)),
                PresentValue.Of(Schedule.Product.Rates, Amount.Of(Currency.Eur, 260000m), ri, CreateTradeInfoSchedule(8, todayPlus10Y)),
                PresentValue.Of(Schedule.Product.Other, Amount.Of(Currency.Usd, -60000m), ri, CreateTradeInfoSchedule(9, todayPlus3M)),
                PresentValue.Of(Schedule.Product.Other, Amount.Of(Currency.Usd, -240000m), ri, CreateTradeInfoSchedule(11, todayPlus10Y)),
            };

            return dataEntities.AsReadOnly();
        }

        private static TradeInfo CreateTradeInfoModel(Int32 tradeId)
        {
            return TradeInfo.Of("Global", String.Concat("M", tradeId.ToString()), DateTime.Today.AddYears(2));
        }

        private static TradeInfo CreateTradeInfoSchedule(Int32 tradeId, DateTime date)
        {
            return TradeInfo.Of("Global", String.Concat("S", tradeId.ToString()), date);
        }
        #endregion
    }

    [CollectionDefinition("Tests")]
    public sealed class TestsCollection : ICollectionFixture<TestsFixture> { }
}