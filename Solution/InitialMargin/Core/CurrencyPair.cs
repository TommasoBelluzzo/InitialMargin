#region Using Directives
using System;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    public sealed class CurrencyPair : IEquatable<CurrencyPair>, IThresholdIdentifier
    {
        #region Members
        private readonly Currency m_Currency1;
        private readonly Currency m_Currency2;
        #endregion

        #region Constructors
        private CurrencyPair(Currency currency1, Currency currency2)
        {
            m_Currency1 = currency1;
            m_Currency2 = currency2;
        }
        #endregion

        #region Properties
        public Currency Currency1 => m_Currency1;

        public Currency Currency2 => m_Currency2;

        public String Description => ToString();

        public String Name => ToString();
        #endregion

        #region Methods
        public override Boolean Equals(Object obj)
        {
            return Equals(obj as CurrencyPair);
        }

        public Boolean Equals(CurrencyPair other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return ((m_Currency1 == other.Currency1) && (m_Currency2 == other.Currency2)); 
        }

        public CurrencyPair Invert()
        {
            return Of(m_Currency2, m_Currency1); 
        }

        public CurrencyPair Sort()
        {
            Currency[] currencies = (new[] { m_Currency1, m_Currency2 }).OrderBy(x => x.Name).ToArray();
            return Of(currencies[0], currencies[1]); 
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hash = 17;
                hash = (hash * 23) + m_Currency1.GetHashCode();
                hash = (hash * 23) + m_Currency2.GetHashCode();

                return hash;
            }
        } 

        public override String ToString()
        {
            return ToString(true);
        }

        public String ToString(Boolean separator)
        {
            if (separator)
                return String.Concat(m_Currency1.Name, "/", m_Currency2.Name);

            return String.Concat(m_Currency1.Name, m_Currency2.Name);
        }
        #endregion

        #region Methods (Operators)
        public static Boolean operator ==(CurrencyPair left, CurrencyPair right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        public static Boolean operator !=(CurrencyPair left, CurrencyPair right)
        {
            return !(left == right);
        }
        #endregion

        #region Methods (Static)
        public static Boolean TryParse(String text, out CurrencyPair value)
        {
            return TryParse(text, false, out value);
        }

        public static Boolean TryParse(String text, Boolean ignoreCase, out CurrencyPair value)
        {
            try
            {
                value = Parse(text, ignoreCase);
                return true;
            }
            catch (ArgumentException)
            {
                value = null;
                return false;
            }
        }

        public static CurrencyPair Of(Currency currency1, Currency currency2)
        {
            if (currency1 == null)
                throw new ArgumentNullException(nameof(currency1));

            if (currency2 == null)
                throw new ArgumentNullException(nameof(currency2));

            if (currency1 == currency2)
                throw new ArgumentException("A currency pair must be defined by two different currencies.");

            return (new CurrencyPair(currency1, currency2));
        }

        public static CurrencyPair Parse(String text)
        {
            return Parse(text, false);
        }

        public static CurrencyPair Parse(String text, Boolean ignoreCase)
        {
            if (String.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Invalid text specified.", nameof(text));

            Int32 offset;

            switch (text.Length)
            {
                case 6:
                    offset = 3;
                    break;

                case 7:
                    offset = 4;
                    break;

                default:
                    throw new ArgumentException("Invalid text specified.", nameof(text));
            }

            String currencyText1 = text.Substring(0, 3);

            if (!Currency.TryParse(currencyText1, ignoreCase, out Currency currency1))
                throw new ArgumentException($"The first currency of the pair ({currencyText1}) is invalid.", nameof(text));

            String currencyText2 = text.Substring(offset, 3);

            if (!Currency.TryParse(currencyText2, ignoreCase, out Currency currency2))
                throw new ArgumentException($"The second currency of the pair ({currencyText2}) is invalid.", nameof(text));

            if (currency1 == currency2)
                throw new ArgumentException("A currency pair must be defined by two different currencies.");

            return (new CurrencyPair(currency1, currency2));
        }
        #endregion
    }
}