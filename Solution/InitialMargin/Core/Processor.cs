#region Using Directives
using System;
using System.Collections.Generic;
#endregion

namespace InitialMargin.Core
{
    /// <summary>Represents the base class from which all the processors must derive. This class is abstract.</summary>
    public abstract class Processor
    {
        #region Members
        /// <summary>Represents the calculation currency of the processor. This field is read-only.</summary>
        protected readonly Currency m_CalculationCurrency;

        /// <summary>Represents the valuation date of the processor. This field is read-only.</summary>
        protected readonly DateTime m_ValuationDate;

        /// <summary>Represents the rates provider of the processor. This field is read-only.</summary>
        protected readonly FxRatesProvider m_RatesProvider;
        #endregion

        #region Properties
        /// <summary>Gets the calculation currency of the processor.</summary>
        /// <value>A <see cref="T:InitialMargin.Core.Currency"/> object.</value>
        public Currency CalculationCurrency => m_CalculationCurrency;

        /// <summary>Gets the valuation date of the processor.</summary>
        /// <value>A <see cref="T:System.DateTime"/> without time component.</value>
        public DateTime ValuationDate => m_ValuationDate;

        /// <summary>Gets the rates provider of the processor.</summary>
        /// <value>An <see cref="T:InitialMargin.Core.FxRatesProvider"/> object.</value>
        public FxRatesProvider RatesProvider => m_RatesProvider;
        #endregion

        #region Constructors
        /// <summary>Represents the base constructor used by derived classes.</summary>
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
        /// <summary>Performs calculations on the specified set of value and parameter entities.</summary>
        /// <param name="values">A <see cref="System.Collections.Generic.List{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the value entities.</param>
        /// <param name="parameters">A <see cref="System.Collections.Generic.List{T}"/> of <see cref="T:InitialMargin.Core.DataParameter"/> objects representing the parameter entities.</param>
        /// <returns>A <see cref="T:InitialMargin.Core.MarginTotal"/> object representing the result of the calculations.</returns>
        public abstract MarginTotal Process(List<DataValue> values, List<DataParameter> parameters);
        #endregion
    }
}
