#region Using Directives
using System;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    /// <summary>Represents a currency pair. This class cannot be derived.</summary>
    public sealed class CurrencyPair : IEquatable<CurrencyPair>, IThresholdIdentifier
    {
        #region Members
        private readonly Currency m_CurrencyBase;
        private readonly Currency m_CurrencyCounter;
        #endregion

        #region Constructors
        private CurrencyPair(Currency currencyBase, Currency currencyCounter)
        {
            m_CurrencyBase = currencyBase;
            m_CurrencyCounter = currencyCounter;
        }
        #endregion

        #region Properties
        /// <summary>Gets the base currency of the currency pair.</summary>
        /// <value>A <see cref="T:InitialMargin.Core.Currency"/> object.</value>
        public Currency CurrencyBase => m_CurrencyBase;

        /// <summary>Gets the counter currency of the currency pair.</summary>
        /// <value>A <see cref="T:InitialMargin.Core.Currency"/> object.</value>
        public Currency CurrencyCounter => m_CurrencyCounter;

        /// <summary>Gets the description of the currency pair.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Description => ToString();

        /// <summary>Gets the name of the currency pair.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Name => ToString();
        #endregion

        #region Methods
        /// <summary>Indicates whether the current instance is equal to the specified object.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if <paramref name="obj">obj</paramref> is an instance of <see cref="T:InitialMargin.Core.CurrencyPair"/> and is equal to the current instance; otherwise, <c>false</c>.</returns>
        public override Boolean Equals(Object obj)
        {
            return Equals(obj as CurrencyPair);
        }

        /// <summary>Indicates whether the current instance is equal to the specified object of the same type.</summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if <paramref name="other">other</paramref> is equal to the current instance; otherwise, <c>false</c>.</returns>
        public Boolean Equals(CurrencyPair other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return ((m_CurrencyBase == other.CurrencyBase) && (m_CurrencyCounter == other.CurrencyCounter)); 
        }

        /// <summary>Returns a new currency pair with inverted currencies.</summary>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.CurrencyPair"/> with inverted currencies.</returns>
        public CurrencyPair Invert()
        {
            return (new CurrencyPair(m_CurrencyCounter, m_CurrencyBase)); 
        }

        /// <summary>Returns a new currency pair with alphabetically sorted currencies.</summary>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.CurrencyPair"/> with alphabetically sorted currencies.</returns>
        public CurrencyPair Sort()
        {
            Currency[] currencies = (new[] { m_CurrencyBase, m_CurrencyCounter }).OrderBy(x => x.Name).ToArray();
            return (new CurrencyPair(currencies[0], currencies[1]));
        }

        /// <summary>Returns the hash code of the current instance.</summary>
        /// <returns>An <see cref="T:System.Int32"/> representing the hash code of the current instance.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hash = 17;
                hash = (hash * 23) + m_CurrencyBase.GetHashCode();
                hash = (hash * 23) + m_CurrencyCounter.GetHashCode();

                return hash;
            }
        } 

        /// <summary>Returns the text representation of the current instance, using a separator between the two currencies.</summary>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public override String ToString()
        {
            return ToString(true);
        }

        /// <summary>Returns the text representation of the current instance. A parameter specifies whether to use a separator between the two currencies.</summary>
        /// <param name="separator"><c>true</c> to include a separator between the currency codes; otherwise, <c>false</c>.</param>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public String ToString(Boolean separator)
        {
            if (separator)
                return String.Concat(m_CurrencyBase.Name, "/", m_CurrencyCounter.Name);

            return String.Concat(m_CurrencyBase.Name, m_CurrencyCounter.Name);
        }
        #endregion

        #region Methods (Operators)
        /// <summary>Returns a value indicating whether two currency pairs are equal.</summary>
        /// <param name="left">The first <see cref="T:InitialMargin.Core.CurrencyPair"/> object to compare.</param>
        /// <param name="right">The second <see cref="T:InitialMargin.Core.CurrencyPair"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are equal; otherwise, <c>false</c>.</returns>
        public static Boolean operator ==(CurrencyPair left, CurrencyPair right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        /// <summary>Returns a value indicating whether two currency pairs are not equal.</summary>
        /// <param name="left">The first <see cref="T:InitialMargin.Core.CurrencyPair"/> object to compare.</param>
        /// <param name="right">The second <see cref="T:InitialMargin.Core.CurrencyPair"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are not equal; otherwise, <c>false</c>.</returns>
        public static Boolean operator !=(CurrencyPair left, CurrencyPair right)
        {
            return !(left == right);
        }
        #endregion

        #region Methods (Static)
        /// <summary>Tries to convert the specified text representation of a currency pair to the equivalent instance.</summary>
        /// <param name="text">The <see cref="T:System.String"/> representing the currency pair.</param>
        /// <param name="result">A new instance of <see cref="T:InitialMargin.Core.CurrencyPair"/> whose properties are represented by <paramref name="text">text</paramref> or <c>null</c>.</param>
        /// <returns><c>true</c> if <paramref name="text">text</paramref> was successfully converted; otherwise, <c>false</c>.</returns>
        public static Boolean TryParse(String text, out CurrencyPair result)
        {
            return TryParse(text, false, out result);
        }

        /// <summary>Tries to convert the specified text representation of a currency pair to the equivalent instance.</summary>
        /// <param name="text">The <see cref="T:System.String"/> representing the currency pair.</param>
        /// <param name="ignoreCase"><c>true</c> to perform a case-insensitive parsing; otherwise, <c>false</c>.</param>
        /// <param name="result">A new instance of <see cref="T:InitialMargin.Core.CurrencyPair"/> whose properties are represented by <paramref name="text">text</paramref> or <c>null</c>.</param>
        /// <returns><c>true</c> if <paramref name="text">text</paramref> was successfully converted; otherwise, <c>false</c>.</returns>
        public static Boolean TryParse(String text, Boolean ignoreCase, out CurrencyPair result)
        {
            try
            {
                result = Parse(text, ignoreCase);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        /// <summary>Initializes and returns a new instance using the specified currencies.</summary>
        /// <param name="currencyBase">The <see cref="T:InitialMargin.Core.Currency"/> object representing the base currency.</param>
        /// <param name="currencyCounter">The <see cref="T:InitialMargin.Core.Currency"/> object representing the counter currency.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.CurrencyPair"/> initialized with the specified currencies.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="currencyBase">currencyBase</paramref> equals <paramref name="currencyCounter">currencyCounter</paramref>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currencyBase">currencyBase</paramref> is <c>null</c> or when <paramref name="currencyCounter">currencyCounter</paramref> is <c>null</c>.</exception>
        public static CurrencyPair Of(Currency currencyBase, Currency currencyCounter)
        {
            if (currencyBase == null)
                throw new ArgumentNullException(nameof(currencyBase));

            if (currencyCounter == null)
                throw new ArgumentNullException(nameof(currencyCounter));

            if (currencyBase == currencyCounter)
                throw new ArgumentException("A currency pair must be defined by two different currencies.");

            return (new CurrencyPair(currencyBase, currencyCounter));
        }

        /// <summary>Converts the specified string representation of a currency pair to the equivalent instance.</summary>
        /// <param name="text">The <see cref="T:System.String"/> representing the currency pair.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.CurrencyPair"/> whose properties are represented by <paramref name="text">text</paramref>.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="text">text</paramref> is invalid or when <paramref name="text">text</paramref> defines two identical currencies.</exception>
        public static CurrencyPair Parse(String text)
        {
            return Parse(text, false);
        }

        /// <summary>Converts the specified string representation of a currency pair to the equivalent instance.</summary>
        /// <param name="text">The <see cref="T:System.String"/> representing the currency pair.</param>
        /// <param name="ignoreCase"><c>true</c> to perform a case-insensitive parsing; otherwise, <c>false</c>.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.CurrencyPair"/> whose properties are represented by <paramref name="text">text</paramref>.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="text">text</paramref> is invalid or when <paramref name="text">text</paramref> defines two identical currencies.</exception>
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