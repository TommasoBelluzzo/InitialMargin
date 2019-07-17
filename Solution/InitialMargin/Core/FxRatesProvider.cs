#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    /// <summary>Represents a rates provider. This class cannot be derived.</summary>
    public sealed class FxRatesProvider
    {
        #region Members
        private readonly Dictionary<CurrencyPair,Decimal> m_Rates;
        private readonly HashSet<CurrencyPair> m_OriginalCurrencyPairs;
        #endregion

        #region Properties
        /// <summary>Gets the number of original rates, the ones not implicitly calculated by the rates provider.</summary>
        /// <value>A <see cref="T:System.Int32"/> value.</value>
        public Int32 OriginalRatesCount => m_OriginalCurrencyPairs.Count;

        /// <summary>Gets the number of rates.</summary>
        /// <value>A <see cref="T:System.Int32"/> value.</value>
        public Int32 RatesCount => m_Rates.Count;
        
        /// <summary>Gets the original rates, the ones not implicitly calculated by the rates provider.</summary>
        /// <value>A <see cref="System.Collections.ObjectModel.ReadOnlyDictionary{TKey,TValue}"/> of pairs defined by <see cref="T:InitialMargin.Core.CurrencyPair"/> keys and <see cref="T:System.Decimal"/> values.</value>
        public ReadOnlyDictionary<CurrencyPair,Decimal> OriginalRates => GetOriginalRates();

        /// <summary>Gets the rates.</summary>
        /// <value>A <see cref="System.Collections.ObjectModel.ReadOnlyDictionary{TKey,TValue}"/> of pairs defined by <see cref="T:InitialMargin.Core.CurrencyPair"/> keys and <see cref="T:System.Decimal"/> values.</value>
        public ReadOnlyDictionary<CurrencyPair,Decimal> Rates => (new ReadOnlyDictionary<CurrencyPair,Decimal>(m_Rates));
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of <see cref="T:InitialMargin.Core.FxRatesProvider"/>.</summary>
        public FxRatesProvider()
        {
            m_Rates = new Dictionary<CurrencyPair,Decimal>();
            m_OriginalCurrencyPairs = new HashSet<CurrencyPair>();
        }
        #endregion

        #region Methods
        private Boolean PerformTriangulation(CurrencyPair currencyPair, out Decimal rate)
        {
            rate = 0m;

            foreach (CurrencyPair key1 in m_Rates.Keys)
            {
                if (key1.CurrencyBase != currencyPair.CurrencyBase)
                    continue;

                foreach (CurrencyPair key2 in m_Rates.Keys)
                {
                    if (key2.CurrencyCounter != currencyPair.CurrencyCounter)
                        continue;

                    if (key1.CurrencyCounter != key2.CurrencyBase)
                        continue;

                    rate = m_Rates[key1] * m_Rates[key2];
                    m_Rates[currencyPair] = rate;
                    m_Rates[currencyPair.Invert()] = 1m / rate;

                    return true;
                }
            }

            return false;
        }

        private ReadOnlyDictionary<CurrencyPair,Decimal> GetOriginalRates()
        {
            Dictionary<CurrencyPair, Decimal> originalRates = m_Rates
                .Where(x => m_OriginalCurrencyPairs.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            return (new ReadOnlyDictionary<CurrencyPair,Decimal>(originalRates));
        }

        /// <summary>Converts the specified amount to the specified currency, performing a triangulation if necessary.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> to convert.</param>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the target currency.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.Amount"/> representing the conversion result.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currency">currency</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the conversion cannot be performed.</exception>
        public Amount Convert(Amount amount, Currency currency)
        {
            return Convert(amount, currency, true);
        }

        /// <summary>Converts the specified amount to the specified currency, using the specified triangulation approach.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> to convert.</param>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the target currency.</param>
        /// <param name="useTriangulation"><c>true</c> to perform a triangulation if necessary; otherwise, <c>false</c>.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.Amount"/> representing the conversion result.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currency">currency</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the conversion cannot be performed.</exception>
        public Amount Convert(Amount amount, Currency currency, Boolean useTriangulation)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            Decimal amountValue = amount.Value;

            if (amountValue == 0m)
                return Amount.Of(currency, amountValue);

            Currency amountCurrency = amount.Currency;

            if (amountCurrency == currency)
                return amount;

            Decimal convertedValue = amountValue * GetRate(amountCurrency, currency, useTriangulation);

            return Amount.Of(currency, convertedValue);
        }

        /// <summary>Retrieves the rate for the specified base and counter currencies, performing a triangulation if necessary.</summary>
        /// <param name="currencyBase">The <see cref="T:InitialMargin.Core.Currency"/> object representing the base currency.</param>
        /// <param name="currencyCounter">The <see cref="T:InitialMargin.Core.Currency"/> object representing the counter currency.</param>
        /// <returns>A <see cref="T:System.Decimal"/> representing the rate.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currencyBase">currencyBase</paramref> is <c>null</c> or when <paramref name="currencyCounter">currencyCounter</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the rate cannot be retrieved.</exception>
        public Decimal GetRate(Currency currencyBase, Currency currencyCounter)
        {
            return GetRate(currencyBase, currencyCounter, true);
        }

        /// <summary>Retrieves the rate for the specified base and counter currencies, using the specified triangulation approach.</summary>
        /// <param name="currencyBase">The <see cref="T:InitialMargin.Core.Currency"/> object representing the base currency.</param>
        /// <param name="currencyCounter">The <see cref="T:InitialMargin.Core.Currency"/> object representing the counter currency.</param>
        /// <param name="useTriangulation"><c>true</c> to perform a triangulation if necessary; otherwise, <c>false</c>.</param>
        /// <returns>A <see cref="T:System.Decimal"/> representing the rate.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currencyBase">currencyBase</paramref> is <c>null</c> or when <paramref name="currencyCounter">currencyCounter</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the rate cannot be retrieved.</exception>
        public Decimal GetRate(Currency currencyBase, Currency currencyCounter, Boolean useTriangulation)
        {
            if (currencyBase == null)
                throw new ArgumentNullException(nameof(currencyBase));

            if (currencyCounter == null)
                throw new ArgumentNullException(nameof(currencyCounter));

            if (currencyBase == currencyCounter)
                return 1m;

            return GetRate(CurrencyPair.Of(currencyBase, currencyCounter), useTriangulation);
        }

        /// <summary>Retrieves the rate for the specified currency pair, performing a triangulation if necessary.</summary>
        /// <param name="currencyPair">The <see cref="T:InitialMargin.Core.CurrencyPair"/> object defining the base and counter currencies.</param>
        /// <returns>A <see cref="T:System.Decimal"/> representing the rate.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currencyPair">currencyPair</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the rate cannot be retrieved.</exception>
        public Decimal GetRate(CurrencyPair currencyPair)
        {
            return GetRate(currencyPair, true);
        }

        /// <summary>Retrieves the rate for the specified currency pair, using the specified triangulation approach.</summary>
        /// <param name="currencyPair">The <see cref="T:InitialMargin.Core.CurrencyPair"/> object defining the base and counter currencies.</param>
        /// <param name="useTriangulation"><c>true</c> to perform a triangulation if necessary; otherwise, <c>false</c>.</param>
        /// <returns>A <see cref="T:System.Decimal"/> representing the rate.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currencyPair">currencyPair</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the rate cannot be retrieved.</exception>
        public Decimal GetRate(CurrencyPair currencyPair, Boolean useTriangulation)
        {
            if (currencyPair == null)
                throw new ArgumentNullException(nameof(currencyPair));

            if (m_Rates.TryGetValue(currencyPair, out Decimal rate))
                return rate;

            if (useTriangulation)
            {
                if (PerformTriangulation(currencyPair, out rate))
                    return rate;

                throw new InvalidOperationException($"The {currencyPair} rate is not defined and cannot be obtained through triangulation.");
            }
            else
                throw new InvalidOperationException($"The {currencyPair} rate is not defined.");
        }

        /// <summary>Tries to retrieve the rate for the specified base and counter currencies, performing a triangulation if necessary.</summary>
        /// <param name="currencyBase">The <see cref="T:InitialMargin.Core.Currency"/> object representing the base currency.</param>
        /// <param name="currencyCounter">The <see cref="T:InitialMargin.Core.Currency"/> object representing the counter currency.</param>
        /// <param name="rate">A <see cref="T:System.Decimal"/> representing the rate or <c>0</c>.</param>
        /// <returns><c>true</c> if the rate was successfully retrieved; otherwise, <c>false</c>.</returns>
        public Boolean TryGetRate(Currency currencyBase, Currency currencyCounter, out Decimal rate)
        {
            return TryGetRate(currencyBase, currencyCounter, true, out rate);
        }

        /// <summary>Tries to retrieve the rate for the specified base and counter currencies, using the specified triangulation approach.</summary>
        /// <param name="currencyBase">The <see cref="T:InitialMargin.Core.Currency"/> object representing the base currency.</param>
        /// <param name="currencyCounter">The <see cref="T:InitialMargin.Core.Currency"/> object representing the counter currency.</param>
        /// <param name="useTriangulation"><c>true</c> to perform a triangulation if necessary; otherwise, <c>false</c>.</param>
        /// <param name="rate">A <see cref="T:System.Decimal"/> representing the rate or <c>0</c>.</param>
        /// <returns><c>true</c> if the rate was successfully retrieved; otherwise, <c>false</c>.</returns>
        public Boolean TryGetRate(Currency currencyBase, Currency currencyCounter, Boolean useTriangulation, out Decimal rate)
        {
            if (currencyBase == null)
                throw new ArgumentNullException(nameof(currencyBase));

            if (currencyCounter == null)
                throw new ArgumentNullException(nameof(currencyCounter));

            if (currencyBase == currencyCounter)
            {
                rate = 1m;
                return true;
            }

            return TryGetRate(CurrencyPair.Of(currencyBase, currencyCounter), useTriangulation, out rate);
        }

        /// <summary>Tries to retrieve the rate for the specified currency pair, performing a triangulation if necessary.</summary>
        /// <param name="currencyPair">The <see cref="T:InitialMargin.Core.CurrencyPair"/> object defining the base and counter currencies.</param>
        /// <param name="rate">A <see cref="T:System.Decimal"/> representing the rate or <c>0</c>.</param>
        /// <returns><c>true</c> if the rate was successfully retrieved; otherwise, <c>false</c>.</returns>
        public Boolean TryGetRate(CurrencyPair currencyPair, out Decimal rate)
        {
            return TryGetRate(currencyPair, true, out rate);
        }

        /// <summary>Tries to retrieve the rate for the specified currency pair, using the specified triangulation approach.</summary>
        /// <param name="currencyPair">The <see cref="T:InitialMargin.Core.CurrencyPair"/> object defining the base and counter currencies.</param>
        /// <param name="useTriangulation"><c>true</c> to perform a triangulation if necessary; otherwise, <c>false</c>.</param>
        /// <param name="rate">A <see cref="T:System.Decimal"/> representing the rate or <c>0</c>.</param>
        /// <returns><c>true</c> if the rate was successfully retrieved; otherwise, <c>false</c>.</returns>
        public Boolean TryGetRate(CurrencyPair currencyPair, Boolean useTriangulation, out Decimal rate)
        {
            try
            {
                rate = GetRate(currencyPair, useTriangulation);
                return true;
            }
            catch
            {
                rate = 0m;
                return false;
            }
        }

        /// <summary>Adds the specified rate for the specified base and counter currencies, updating the lookup table if the currency pair already exists.</summary>
        /// <param name="currencyBase">The <see cref="T:InitialMargin.Core.Currency"/> object representing the base currency.</param>
        /// <param name="currencyCounter">The <see cref="T:InitialMargin.Core.Currency"/> object representing the counter currency.</param>
        /// <param name="rate">A <see cref="T:System.Decimal"/> representing the rate.</param>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="currencyBase">currencyBase</paramref> is equal to <paramref name="currencyCounter">currencyCounter</paramref> or when <paramref name="rate">rate</paramref> is less than or equal to <c>0</c>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currencyBase">currencyBase</paramref> is <c>null</c> or when <paramref name="currencyCounter">currencyCounter</paramref> is <c>null</c>.</exception>
        public void AddRate(Currency currencyBase, Currency currencyCounter, Decimal rate)
        {
            AddRate(currencyBase, currencyCounter, rate, true);
        }

        /// <summary>Adds the specified rate for the specified base and counter currencies, using the specified update criterion.</summary>
        /// <param name="currencyBase">The <see cref="T:InitialMargin.Core.Currency"/> object representing the base currency.</param>
        /// <param name="currencyCounter">The <see cref="T:InitialMargin.Core.Currency"/> object representing the counter currency.</param>
        /// <param name="rate">A <see cref="T:System.Decimal"/> representing the rate.</param>
        /// <param name="update"><c>true</c> to update the lookup table if the currency pair implicitly defined by the currencies already exists; otherwise, <c>false</c>.</param>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="currencyBase">currencyBase</paramref> is equal to <paramref name="currencyCounter">currencyCounter</paramref> or when <paramref name="rate">rate</paramref> is less than or equal to <c>0</c>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currencyBase">currencyBase</paramref> is <c>null</c> or when <paramref name="currencyCounter">currencyCounter</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the currency pair implicitly defined by <paramref name="currencyBase">currencyBase</paramref> and <paramref name="currencyCounter">currencyCounter</paramref> already exists in the lookup table and <paramref name="update">update</paramref> is <c>false</c>.</exception>
        public void AddRate(Currency currencyBase, Currency currencyCounter, Decimal rate, Boolean update)
        {
            if (currencyBase == null)
                throw new ArgumentNullException(nameof(currencyBase));

            if (currencyCounter == null)
                throw new ArgumentNullException(nameof(currencyCounter));

            if (currencyBase == currencyCounter)
                throw new ArgumentException("The two currencies must be different.");

            AddRate(CurrencyPair.Of(currencyBase, currencyCounter), rate, update);
        }

        /// <summary>Adds the specified rate for the specified currency pair, updating the lookup table if the currency pair already exists.</summary>
        /// <param name="currencyPair">The <see cref="T:InitialMargin.Core.CurrencyPair"/> object defining the base and counter currencies.</param>
        /// <param name="rate">A <see cref="T:System.Decimal"/> representing the rate.</param>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="rate">rate</paramref> is less than or equal to <c>0</c>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currencyPair">currencyPair</paramref> is <c>null</c>.</exception>
        public void AddRate(CurrencyPair currencyPair, Decimal rate)
        {
            AddRate(currencyPair, rate, true);
        }

        /// <summary>Adds the specified rate for the specified currency pair, using the specified update criterion.</summary>
        /// <param name="currencyPair">The <see cref="T:InitialMargin.Core.CurrencyPair"/> object defining the base and counter currencies.</param>
        /// <param name="rate">A <see cref="T:System.Decimal"/> representing the rate.</param>
        /// <param name="update"><c>true</c> to update the lookup table if the currency pair already exists; otherwise, <c>false</c>.</param>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="rate">rate</paramref> is less than or equal to <c>0</c>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currencyPair">currencyPair</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="currencyPair">currencyPair</paramref> already exists in the lookup table and <paramref name="update">update</paramref> is <c>false</c>.</exception>
        public void AddRate(CurrencyPair currencyPair, Decimal rate, Boolean update)
        {
            if (currencyPair == null)
                throw new ArgumentNullException(nameof(currencyPair));

            if (rate <= 0m)
                throw new ArgumentException("Invalid rate specified.", nameof(rate));

            if (update || !m_Rates.ContainsKey(currencyPair))
            {
                CurrencyPair currencyPairInverse = currencyPair.Invert();

                m_Rates[currencyPair] = rate;
                m_OriginalCurrencyPairs.Add(currencyPair);

                m_Rates[currencyPairInverse] = 1m / rate;
                m_OriginalCurrencyPairs.Remove(currencyPairInverse);
                
                return;
            }

            throw new InvalidDataException("The specified rate has already been defined, directly or implicitly.");
        }
        #endregion
    }
}
