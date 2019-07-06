#region Using Directives
using System.ComponentModel;
#endregion

namespace InitialMargin.Core
{
    public enum CurrencyCategory
    {
        #region Values
        [Description("Frequently Traded")] FrequentlyTraded,
        [Description("Significantly Material")] SignificantlyMaterial,
        [Description("Other")] Other
        #endregion
    }

    public enum CurrencyCodeSymbol
    {
        #region Values
        [Description("None")] None,
        [Description("After")] After,
        [Description("Before")] Before
        #endregion
    }

    public enum CurrencyLiquidity
    {
        #region Values
        [Description("Undefined")] Undefined,
        [Description("Medium")] Medium,
        [Description("High")] High,
        #endregion
    }

    public enum CurrencyVolatility
    {
        #region Values
        [Description("Low Volatility")] Low = 1,
        [Description("Regular Volatility")] Regular = 0,
        [Description("High Volatility")] High = 2
        #endregion
    }

    public enum Regulation
    {
        #region Values
        [Description("Australian Prudential Regulation Authority")] Apra,
        [Description("US Commodity Futures Trading Commission")] Cftc,
        [Description("European Supervisory Authorities")] Esa,
        [Description("Swiss Financial Market Supervisory Authority")] Finma,
        [Description("Korean Financial Services Commission")] Kfsc,
        [Description("Hong Kong Monetary Authority")] Hkma,
        [Description("Japanese Financial Services Authority")] Jfsa,
        [Description("Monetary Authority of Singapore")] Mas,
        [Description("Canadian Office of the Superintendent of Financial Institutions")] Osfi,
        [Description("Reserve Bank of India")] Rbi,
        [Description("US Securities Exchange Commission")] Sec,
        [Description("South African National Treasury")] Sant,
        [Description("US Prudential Regulators")] Uspr
        #endregion
    }

    public enum RegulationRole
    {
        #region Values
        [Description("Pledgor")] Pledgor,
        [Description("Secured")] Secured
        #endregion
    }
}