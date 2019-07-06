#region Using Directives
using System.ComponentModel;
#endregion

namespace InitialMargin.Schedule
{
    public enum Product
    {
        #region Values
        [Description("Commodity")] Commodity = 0,
        [Description("Credit")] Credit = 1,
        [Description("Equity")] Equity = 2,
        [Description("Foreign Exchange")] Fx = 3,
        [Description("Other")] Other = 5,
        [Description("Interest Rates")] Rates = 4
        #endregion
    }
}