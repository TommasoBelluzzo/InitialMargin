#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    public struct Amount : IComparable, IComparable<Amount>, IEquatable<Amount>, IFormattable
    {
        #region Members
        private readonly Currency m_Currency;
        private readonly Decimal m_Value;
        #endregion

        #region Properties
        public Boolean IsZero => m_Value == 0m;

        public Currency Currency => m_Currency;

        public Decimal Value => m_Value;
        #endregion

        #region Constructors
        private Amount(Currency currency, Decimal value)
        {
            m_Currency = currency;
            m_Value = value;
        }
        #endregion

        #region Methods
        public override Boolean Equals(Object obj)
        {
            if (obj is Amount amount) 
                return Equals(amount);

            return false;
        }

        public Boolean Equals(Amount other)
        {
            return ((m_Currency == other.Currency) && (m_Value == other.Value)); 
        }

        public Int32 CompareTo(Object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (obj is Amount amount) 
                return CompareTo(amount);

            throw new ArgumentException($"Cannot compare type {obj.GetType()} to type {GetType()}.", nameof(obj));
        }

        public Int32 CompareTo(Amount other)
        {
            if (m_Currency == other.Currency)
                return m_Value.CompareTo(other.Value);

            throw new InvalidOperationException("Amounts expressed in different currencies cannot be compared.");
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hash = 17;
                hash = (hash * 23) + m_Currency.GetHashCode();
                hash = (hash * 23) + m_Value.GetHashCode();

                return hash;
            }
        }

        public override String ToString()
        {
            return ToString("N", CultureInfo.CurrentCulture, CurrencyCodeSymbol.Before);
        }

        public String ToString(CurrencyCodeSymbol ccs)
        {
            return ToString("N", CultureInfo.CurrentCulture, ccs);
        }

        public String ToString(IFormatProvider formatProvider)
        {
            return ToString("N", formatProvider, CurrencyCodeSymbol.Before);
        }

        public String ToString(String format)
        {
            return ToString(format, CultureInfo.CurrentCulture, CurrencyCodeSymbol.Before);
        }

        public String ToString(IFormatProvider formatProvider, CurrencyCodeSymbol ccs)
        {
            return ToString("N", formatProvider, ccs);
        }

        public String ToString(String format, CurrencyCodeSymbol ccs)
        {
            return ToString(format, CultureInfo.CurrentCulture, ccs);
        }

        public String ToString(String format, IFormatProvider formatProvider)
        {
            return ToString(format, formatProvider, CurrencyCodeSymbol.Before);
        }

        public String ToString(String format, IFormatProvider formatProvider, CurrencyCodeSymbol ccs)
        {
            if (String.IsNullOrWhiteSpace(format))
                format = "N";

            if (formatProvider == null)
                formatProvider = CultureInfo.CurrentCulture;

            switch (ccs)
            {
                case CurrencyCodeSymbol.After:
                    return $"{m_Value.ToString(format, formatProvider)} {m_Currency}";

                case CurrencyCodeSymbol.Before:
                    return $"{m_Currency} {m_Value.ToString(format, formatProvider)}";

                default:
                    return m_Value.ToString(format, formatProvider);
            }
        }
        #endregion

        #region Methods (Operators)
        public static Boolean operator ==(Amount left, Amount right)
        {
            return left.Equals(right);
        }

        public static Boolean operator !=(Amount left, Amount right)
        {
            return !(left == right);
        }

        public static Boolean operator <(Amount left, Amount right)
        {
            return (left.CompareTo(right) < 0);
        }

        public static Boolean operator <=(Amount left, Amount right)
        {
            return (left.CompareTo(right) <= 0);
        }

        public static Boolean operator >(Amount left, Amount right)
        {
            return (left.CompareTo(right) > 0);
        }

        public static Boolean operator >=(Amount left, Amount right)
        {
            return (left.CompareTo(right) >= 0);
        }

        public static Amount operator +(Amount amount)
        {
            return amount;
        }

        public static Amount operator +(Amount left, Amount right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, left.Value + right.Value));
        }

        public static Amount operator +(Amount left, Decimal right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return (new Amount(left.Currency, left.Value + right));
        }

        public static Amount operator +(Decimal left, Amount right)
        {
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return (new Amount(right.Currency, left + right.Value));
        }

        public static Amount operator -(Amount amount)
        {
            return (new Amount(amount.Currency, -amount.Value));
        }

        public static Amount operator -(Amount left, Amount right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, left.Value - right.Value));
        }

        public static Amount operator -(Amount left, Decimal right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return (new Amount(left.Currency, left.Value - right));
        }

        public static Amount operator -(Decimal left, Amount right)
        {
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return (new Amount(right.Currency, left - right.Value));
        }

        public static Amount operator *(Amount left, Amount right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, left.Value * right.Value));
        }

        public static Amount operator *(Amount left, Decimal right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return (new Amount(left.Currency, left.Value * right));
        }

        public static Amount operator *(Decimal left, Amount right)
        {
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return (new Amount(right.Currency, left * right.Value));
        }

        public static Amount operator /(Amount left, Amount right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, left.Value / right.Value));
        }

        public static Amount operator /(Amount left, Decimal right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return (new Amount(left.Currency, left.Value / right));
        }

        public static Amount operator /(Decimal left, Amount right)
        {
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return (new Amount(right.Currency, left / right.Value));
        }

        public static Amount operator %(Amount left, Amount right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, left.Value % right.Value));
        }

        public static Amount operator %(Amount left, Decimal right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return (new Amount(left.Currency, left.Value % right));
        }
        
        public static Amount operator %(Decimal left, Amount right)
        {
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return (new Amount(right.Currency, left % right.Value));
        }
        #endregion

        #region Methods (Static)
        public static Amount Abs(Amount amount)
        {
            return (new Amount(amount.Currency, Math.Abs(amount.Value)));
        }

        public static Amount Add(Amount left, Amount right)
        { 
            return (left + right);
        }

        public static Amount Add(Amount left, Decimal right)
        { 
            return (left + right);
        }

        public static Amount Add(Decimal left, Amount right)
        { 
            return (left + right);
        }

        public static Amount Divide(Amount left, Amount right)
        { 
            return (left / right);
        }

        public static Amount Divide(Amount left, Decimal right)
        { 
            return (left / right);
        }

        public static Amount Divide(Decimal left, Amount right)
        { 
            return (left / right);
        }

        public static Amount Of(Currency currency, Decimal value)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            return (new Amount(currency, value));
        }

        public static Amount Max(Amount left, Amount right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, Math.Max(left.Value, right.Value)));
        }

        public static Amount Max(Amount left, Decimal right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return (new Amount(left.Currency, Math.Max(left.Value, right)));
        }

        public static Amount Max(Decimal left, Amount right)
        {
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return (new Amount(right.Currency, Math.Max(left, right.Value)));
        }

        public static Amount Min(Amount left, Amount right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, Math.Min(left.Value, right.Value)));
        }

        public static Amount Min(Amount left, Decimal right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return (new Amount(left.Currency, Math.Min(left.Value, right)));
        }

        public static Amount Min(Decimal left, Amount right)
        {
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return (new Amount(right.Currency, Math.Min(left, right.Value)));
        }

        public static Amount Mod(Amount left, Amount right)
        { 
            return (left % right);
        }

        public static Amount Mod(Amount left, Decimal right)
        { 
            return (left % right);
        }

        public static Amount Mod(Decimal left, Amount right)
        { 
            return (left % right);
        }

        public static Amount Multiply(Amount left, Amount right)
        { 
            return (left * right);
        }

        public static Amount Multiply(Amount left, Decimal right)
        { 
            return (left * right);
        }

        public static Amount Multiply(Decimal left, Amount right)
        { 
            return (left * right);
        }

        public static Amount Negate(Amount amount)
        { 
            return -amount;
        }

        public static Amount One(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            return (new Amount(currency, 1m));
        }

        public static Amount Plus(Amount amount)
        { 
            return +amount;
        }

        public static Amount Round(Amount amount)
        { 
            return (new Amount(amount.Currency, Math.Round(amount.Value)));
        }

        public static Amount Round(Amount amount, Int32 digits)
        { 
            if ((digits < 0) || (digits > 15))
                throw new ArgumentOutOfRangeException(nameof(digits), "Invalid number of rounding digits specified.");

            return (new Amount(amount.Currency, Math.Round(amount.Value, digits)));
        }

        public static Amount Round(Amount amount, MidpointRounding mode)
        { 
            return (new Amount(amount.Currency, Math.Round(amount.Value, mode)));
        }

        public static Amount Round(Amount amount, Int32 digits, MidpointRounding mode)
        { 
            if ((digits < 0) || (digits > 15))
                throw new ArgumentOutOfRangeException(nameof(digits), "Invalid number of rounding digits specified.");

            return (new Amount(amount.Currency, Math.Round(amount.Value, digits, mode)));
        }

        public static Amount Square(Amount amount)
        { 
            return (new Amount(amount.Currency, MathUtilities.Square(amount.Value)));
        }

        public static Amount SquareRoot(Amount amount)
        { 
            return (new Amount(amount.Currency, MathUtilities.SquareRoot(amount.Value)));
        }

        public static Amount Subtract(Amount left, Amount right)
        { 
            return (left - right);
        }

        public static Amount Subtract(Amount left, Decimal right)
        { 
            return (left - right);
        }
        
        public static Amount Subtract(Decimal left, Amount right)
        { 
            return (left - right);
        }
        
        public static Amount Sum(IEnumerable<Amount> amounts, Currency currency)
        {
            if (amounts == null)
                throw new ArgumentNullException(nameof(amounts));

            return amounts.Aggregate(Zero(currency), (a1, a2) => a1 + a2);
        }

        public static Amount Zero(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            return (new Amount(currency, 0m));
        }
        #endregion
    }
}
