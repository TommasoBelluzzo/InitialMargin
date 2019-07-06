#region Using Directives
using System;
#endregion

namespace InitialMargin.Schedule
{
    internal static class ScheduleParameters
    {
        #region Parameters
        private const Decimal MARGINFACTOR_NGR = 0.60m;
        private const Decimal MARGINFACTOR_STRAIGHT = 0.40m;
        private const Decimal NOTIONALFACTOR_COMMODITIES = 0.15m;
        private const Decimal NOTIONALFACTOR_CREDIT_UNDER2_Y = 0.02m;
        private const Decimal NOTIONALFACTOR_CREDIT_2YTO5Y = 0.05m;
        private const Decimal NOTIONALFACTOR_CREDIT_OVER5Y = 0.10m;
        private const Decimal NOTIONALFACTOR_EQUITY = 0.15m;
        private const Decimal NOTIONALFACTOR_FX = 0.06m;
        private const Decimal NOTIONALFACTOR_OTHER = 0.15m;
        private const Decimal NOTIONALFACTOR_RATES_UNDER2Y = 0.01m;
        private const Decimal NOTIONALFACTOR_RATES_2YTO5Y = 0.02m;
        private const Decimal NOTIONALFACTOR_RATES_OVER5Y = 0.04m;
        #endregion

        #region Methods
        public static Decimal GetMarginFactor(Decimal ngRatio)
        {
            if (ngRatio < 0m)
                throw new ArgumentOutOfRangeException(nameof(ngRatio), "Invalid net/gross ratio specified.");

            return (MARGINFACTOR_STRAIGHT + (MARGINFACTOR_NGR * ngRatio));
        }

        public static Decimal GetNotionalFactor(DateTime valuationDate, Notional notional)
        {
            if (notional == null)
                throw new ArgumentNullException(nameof(notional));

            switch (notional.Product)
            {
                case Product.Commodity:
                    return NOTIONALFACTOR_COMMODITIES;

                case Product.Credit:
                {
                    DateTime endDate = notional.EndDate;

                    if (endDate.AddYears(-2) < valuationDate)
                        return NOTIONALFACTOR_CREDIT_UNDER2_Y;
            
                    if (endDate.AddYears(-5) <= valuationDate)
                        return NOTIONALFACTOR_CREDIT_2YTO5Y;

                    return NOTIONALFACTOR_CREDIT_OVER5Y;
                }

                case Product.Equity:
                    return NOTIONALFACTOR_EQUITY;

                case Product.Fx:
                    return NOTIONALFACTOR_FX;

                case Product.Rates:
                {
                    DateTime endDate = notional.EndDate;

                    if (endDate.AddYears(-2) < valuationDate)
                        return NOTIONALFACTOR_RATES_UNDER2Y;

                    if (endDate.AddYears(-5) <= valuationDate)
                        return NOTIONALFACTOR_RATES_2YTO5Y;

                    return NOTIONALFACTOR_RATES_OVER5Y;
                }

                default:
                    return NOTIONALFACTOR_OTHER;
            }
        }
        #endregion
    }
}
