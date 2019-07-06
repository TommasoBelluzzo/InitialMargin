#region Using Directives
using System.ComponentModel;
#endregion

namespace InitialMargin.Model
{
    public enum Curve
    {
        #region Values
        [Description("1-Month LIBOR")] Libor1M,
        [Description("3-Month LIBOR")] Libor3M,
        [Description("6-Month LIBOR")] Libor6M,
        [Description("12-Month LIBOR")] Libor12M,
        [Description("Municipal")] Municipal,
        [Description("Overnight Index Swap")] Ois,
        [Description("Prime")] Prime
        #endregion
    }

    public enum Product
    {
        #region Values
        [Description("Commodity")] Commodity = 0,
        [Description("Credit")] Credit = 1,
        [Description("Equity")] Equity = 2,
        [Description("Foreign Exchange & Interest Rates")] RatesFx = 3
        #endregion
    }

    public enum SensitivityCategory
    {
        #region Values
        [Description("Base Correlation")] BaseCorrelation,
        [Description("Curvature")] Curvature,
        [Description("Delta")] Delta,
        [Description("Vega")] Vega
        #endregion
    }

    public enum SensitivityRisk
    {
        #region Values
        [Description("Commodity")] Commodity = 0,
        [Description("Credit Non-qualifying")] CreditNonQualifying = 2,
        [Description("Credit Qualifying")] CreditQualifying = 1,
        [Description("Equity")] Equity = 3,
        [Description("FX")] Fx = 4,
        [Description("Rates")] Rates = 5
        #endregion
    }

    public enum SensitivitySubrisk
    {
        #region Values
        [Description("None")] None,
        [Description("Cross-Currency Basis")] CrossCurrencyBasis,
        [Description("Inflation")] Inflation,
        [Description("Interest Rate")] InterestRate
        #endregion
    }
}