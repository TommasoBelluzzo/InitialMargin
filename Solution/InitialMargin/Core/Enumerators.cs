namespace InitialMargin.Core
{
    /// <summary>Specifies the currency category.</summary>
    public enum CurrencyCategory
    {
        #region Values
        /// <summary>The frequently traded category.</summary>
        FrequentlyTraded,
        /// <summary>The significantly material category.</summary>
        SignificantlyMaterial,
        /// <summary>The residual category.</summary>
        Other
        #endregion
    }

    /// <summary>Specifies the location of the currency symbol.</summary>
    public enum CurrencyCodeSymbol
    {
        #region Values
        /// <summary>The currency symbol is not displayed.</summary>
        None,
        /// <summary>The currency symbol is displayed after the currency amount.</summary>
        After,
        /// <summary>The currency symbol is displayed before the currency amount.</summary>
        Before
        #endregion
    }

    /// <summary>Specifies the currency liquidity.</summary>
    public enum CurrencyLiquidity
    {
        #region Values
        /// <summary>The undefined liquidity.</summary>
        Undefined,
        /// <summary>The medium liquidity.</summary>
        Medium,
        /// <summary>The high liquidity.</summary>
        High,
        #endregion
    }

    /// <summary>Specifies the currency volatility.</summary>
    public enum CurrencyVolatility
    {
        #region Values
        /// <summary>The low volatility.</summary>
        Low = 1,
        /// <summary>The regular volatility.</summary>
        Regular = 0,
        /// <summary>The high volatility.</summary>
        High = 2
        #endregion
    }

    /// <summary>Specifies the regulation.</summary>
    public enum Regulation
    {
        #region Values
        /// <summary>The regulation of the Australian Prudential Regulation Authority.</summary>
        Apra,
        /// <summary>The regulation of the US Commodity Futures Trading Commission.</summary>
        Cftc,
        /// <summary>The regulation of the European Supervisory Authority.</summary>
        Esa,
        /// <summary>The regulation of the Swiss Financial Market Supervisory Authority.</summary>
        Finma,
        /// <summary>The regulation of the Korean Financial Services Commission.</summary>
        Kfsc,
        /// <summary>The regulation of the HongKong Monetary Authority.</summary>
        Hkma,
        /// <summary>The regulation of the Japanese Financial Services Authority.</summary>
        Jfsa,
        /// <summary>The regulation of the Monetary Authority of Singapore.</summary>
        Mas,
        /// <summary>The regulation of the Canadian Office of the Superintendent of Financial Institutions.</summary>
        Osfi,
        /// <summary>The regulation of the Reserve Bank of India.</summary>
        Rbi,
        /// <summary>The regulation of the US Securities Exchange Commission.</summary>
        Sec,
        /// <summary>The regulation of the South African National Treasury.</summary>
        Sant,
        /// <summary>The regulation of the US Prudential Regulators.</summary>
        Uspr
        #endregion
    }

    /// <summary>Specifies the regulation role.</summary>
    public enum RegulationRole
    {
        #region Values
        /// <summary>The pledgor role, subject to post regulations.</summary>
        Pledgor,
        /// <summary>The secured role, subject to collect regulations.</summary>
        Secured
        #endregion
    }
}