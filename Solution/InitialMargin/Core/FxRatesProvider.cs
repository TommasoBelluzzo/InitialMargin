#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    public sealed class FxRatesProvider
    {
        #region Members
        private readonly Dictionary<CurrencyPair,Decimal> m_Rates;
        private readonly HashSet<CurrencyPair> m_OriginalCurrencyPairs;
        #endregion

        #region Properties
        public Int32 OriginalRatesCount => m_OriginalCurrencyPairs.Count;

        public Int32 RatesCount => m_Rates.Count;
        
        public ReadOnlyDictionary<CurrencyPair,Decimal> OriginalRates => GetOriginalRates();

        public ReadOnlyDictionary<CurrencyPair,Decimal> Rates => (new ReadOnlyDictionary<CurrencyPair,Decimal>(m_Rates));
        #endregion

        #region Constructors
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
                if (key1.Currency1 != currencyPair.Currency1)
                    continue;

                foreach (CurrencyPair key2 in m_Rates.Keys)
                {
                    if (key2.Currency2 != currencyPair.Currency2)
                        continue;

                    if (key1.Currency2 != key2.Currency1)
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

        public Amount Convert(Amount amount, Currency currency)
        {
            return Convert(amount, currency, true);
        }

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

        public Decimal GetRate(Currency currencyBase, Currency currencyCounter)
        {
            return GetRate(currencyBase, currencyCounter, true);
        }

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

        public Decimal GetRate(CurrencyPair currencyPair)
        {
            return GetRate(currencyPair, true);
        }

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

                throw new InvalidOperationException($"The {currencyPair} exchange rate is not defined and cannot be obtained through triangulation.");
            }
            else
                throw new InvalidOperationException($"The {currencyPair} exchange rate is not defined.");
        }

        public void AddRate(Currency currencyBase, Currency currencyCounter, Decimal rate)
        {
            AddRate(currencyBase, currencyCounter, rate, true);
        }

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

        public void AddRate(CurrencyPair currencyPair, Decimal rate)
        {
            AddRate(currencyPair, rate, true);
        }

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

            throw new ArgumentException("The specified rate has already been defined, directly or by implicit inversion.", nameof(rate));
        }
        #endregion
    }
}
