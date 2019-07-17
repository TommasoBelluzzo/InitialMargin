namespace InitialMargin.Model
{
    /// <summary>Specifies the curve of an interest date delta sensitivity.</summary>
    public enum Curve
    {
        #region Values
        /// <summary>The 1-month LIBOR curve.</summary>
        Libor1M,
        /// <summary>The 3-months LIBOR curve.</summary>
        Libor3M,
        /// <summary>The 6-months LIBOR curve.</summary>
        Libor6M,
        /// <summary>The 12-months LIBOR curve.</summary>
        Libor12M,
        /// <summary>The municipal curve, which can be used only within the context of US derivatives.</summary>
        Municipal,
        /// <summary>The overnight index swap.</summary>
        Ois,
        /// <summary>The prime curve, which can be used only within the context of US derivatives.</summary>
        Prime
        #endregion
    }

    /// <summary>Specifies the product within the Model IM context.</summary>
    public enum Product
    {
        #region Values
        /// <summary>The commodity product.</summary>
        Commodity = 0,
        /// <summary>The credit product.</summary>
        Credit = 1,
        /// <summary>The equity product.</summary>
        Equity = 2,
        /// <summary>The foreign exchange &amp; interest rate product.</summary>
        RatesFx = 3
        #endregion
    }

    /// <summary>Specifies the sensitivity category.</summary>
    public enum SensitivityCategory
    {
        #region Values
        /// <summary>The base correlation category.</summary>
        BaseCorrelation,
        /// <summary>The curvature category.</summary>
        Curvature,
        /// <summary>The delta category.</summary>
        Delta,
        /// <summary>The vega category.</summary>
        Vega
        #endregion
    }

    /// <summary>Specifies the sensitivity risk.</summary>
    public enum SensitivityRisk
    {
        #region Values
        /// <summary>The commodity risk.</summary>
        Commodity = 0,
        /// <summary>The non-qualifying credit risk.</summary>
        CreditNonQualifying = 2,
        /// <summary>The qualifying credit risk.</summary>
        CreditQualifying = 1,
        /// <summary>The equity risk.</summary>
        Equity = 3,
        /// <summary>The foreign exchange risk.</summary>
        Fx = 4,
        /// <summary>The interest rate risk.</summary>
        Rates = 5
        #endregion
    }

    /// <summary>Specifies the sensitivity sub-risk.</summary>
    public enum SensitivitySubrisk
    {
        #region Values
        /// <summary>The null sub-risk.</summary>
        None,
        /// <summary>The cross-currency basis sub-risk.</summary>
        CrossCurrencyBasis,
        /// <summary>The inflation sub-risk.</summary>
        Inflation,
        /// <summary>The interest rate sub-risk.</summary>
        InterestRate
        #endregion
    }
}