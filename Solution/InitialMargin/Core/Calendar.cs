#region Using Directives
using System;
#endregion

namespace InitialMargin.Core
{
    internal static class Calendar
    {
        #region Constants
        public const Decimal DaysPerWeek = 7m;
        public const Decimal DaysPerMonth = DaysPerYear / 12m;
        public const Decimal DaysPerYear = 365m;
        #endregion
    }
}