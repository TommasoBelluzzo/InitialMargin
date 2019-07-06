#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Schedule
{
    internal static class ScheduleCalculations
    {
        #region Methods
        public static Margin CalculateMargin(DateTime valuationDate, Currency calculationCurrency, List<MarginProduct> productMargins, List<Notional> notionals, List<PresentValue> presentValues)
        {
            if (calculationCurrency == null)
                throw new ArgumentNullException(nameof(calculationCurrency));

            if (productMargins == null)
                throw new ArgumentNullException(nameof(productMargins));

            if (notionals == null)
                throw new ArgumentNullException(nameof(notionals));

            if (presentValues == null)
                throw new ArgumentNullException(nameof(presentValues));

            Amount netReplacementCost = Amount.Max(Amount.Sum(presentValues.Select(x => -x.Amount), calculationCurrency), 0m);
            Amount grossReplacementCost = Amount.Sum(presentValues.Select(x => Amount.Max(-x.Amount, 0m)), calculationCurrency);
            Decimal ngRatio = grossReplacementCost.IsZero ? 1m : (netReplacementCost / grossReplacementCost).Value;

            Amount marginAmountGross = Amount.Sum(notionals.Select(x => Amount.Abs(x.Amount) * ScheduleParameters.GetNotionalFactor(valuationDate, x)), calculationCurrency);
            Decimal marginFactor = ScheduleParameters.GetMarginFactor(ngRatio);
            Amount marginAmountNet = marginAmountGross * marginFactor;

            return Margin.Of(marginAmountNet, productMargins);
        }

        public static MarginProduct CalculateMarginProduct(DateTime valuationDate, Currency calculationCurrency, Product product, List<Notional> notionals, List<PresentValue> presentValues)
        {
            if (calculationCurrency == null)
                throw new ArgumentNullException(nameof(calculationCurrency));

            if (!Enum.IsDefined(typeof(Product), product))
                throw new InvalidEnumArgumentException("Invalid product specified.");

            if (notionals == null)
                throw new ArgumentNullException(nameof(notionals));

            if (presentValues == null)
                throw new ArgumentNullException(nameof(presentValues));

            Amount netReplacementCost = Amount.Max(Amount.Sum(presentValues.Select(x => -x.Amount), calculationCurrency), 0m);
            Amount grossReplacementCost = Amount.Sum(presentValues.Select(x => Amount.Max(-x.Amount, 0m)), calculationCurrency);
            Decimal ngRatio = grossReplacementCost.IsZero ? 1m : (netReplacementCost / grossReplacementCost).Value;

            Amount marginAmountGross = Amount.Sum(notionals.Select(x => Amount.Abs(x.Amount) * ScheduleParameters.GetNotionalFactor(valuationDate, x)), calculationCurrency);
            Decimal marginFactor = ScheduleParameters.GetMarginFactor(ngRatio);
            Amount marginAmountNet = marginAmountGross * marginFactor;

            return MarginProduct.Of(product, marginAmountNet);
        }
        #endregion
    }
}
