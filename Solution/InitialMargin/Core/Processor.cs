#region Using Directives
using System;
using System.Collections.Generic;
#endregion

namespace InitialMargin.Core
{
    public abstract class Processor
    {
        #region Members
        protected readonly Currency m_CalculationCurrency;
        protected readonly DateTime m_ValuationDate;
        protected readonly FxRatesProvider m_RatesProvider;
        #endregion

        #region Constructors
        protected Processor(DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider)
        {
            if (calculationCurrency == null)
                throw new ArgumentNullException(nameof(calculationCurrency));

            if (ratesProvider == null)
                throw new ArgumentNullException(nameof(ratesProvider));

            m_CalculationCurrency = calculationCurrency;
            m_ValuationDate = valuationDate;
            m_RatesProvider = ratesProvider;
        }
        #endregion

        #region Methods (Abstract)
        public abstract MarginTotal Process(List<DataValue> values, List<DataParameter> parameters);
        #endregion
    }
}
