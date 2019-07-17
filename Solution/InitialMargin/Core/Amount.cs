#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    /// <summary>Represents a monetary amount defined by a currency and a value.</summary>
    public struct Amount : IComparable, IComparable<Amount>, IEquatable<Amount>, IFormattable
    {
        #region Members
        private readonly Currency m_Currency;
        private readonly Decimal m_Value;
        #endregion

        #region Properties
        /// <summary>Gets a value indicating whether the amount is equal to <c>0</c>.</summary>
        /// <value><c>true</c> if the amount is equal to <c>0</c>; otherwise, <c>false</c>.</value>
        public Boolean IsZero => m_Value == 0m;

        /// <summary>Gets the currency of the amount.</summary>
        /// <value>A <see cref="T:InitialMargin.Core.Currency"/> object.</value>
        public Currency Currency => m_Currency;
        
        /// <summary>Gets the value of the amount.</summary>
        /// <value>A <see cref="T:System.Decimal"/> value.</value>
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
        /// <summary>Indicates whether the current instance is equal to the specified object.</summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current instance.</param>
        /// <returns><c>true</c> if <paramref name="obj">obj</paramref> is an instance of <see cref="T:InitialMargin.Core.Amount"/> and is equal to the current instance; otherwise, <c>false</c>.</returns>
        public override Boolean Equals(Object obj)
        {
            if (obj is Amount amount) 
                return Equals(amount);

            return false;
        }

        /// <summary>Indicates whether the current instance is equal to the specified object of the same type.</summary>
        /// <param name="other">The <see cref="T:InitialMargin.Core.Amount"/> to compare with the current instance.</param>
        /// <returns><c>true</c> if <paramref name="other">other</paramref> is equal to the current instance; otherwise, <c>false</c>.</returns>
        public Boolean Equals(Amount other)
        {
            return ((m_Currency == other.Currency) && (m_Value == other.Value)); 
        }

        /// <summary>Compares the current instance to the specified object and returns an indication of their relative value.</summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current instance.</param>
        /// <returns>An <see cref="T:System.Int32"/> representing the relative value between the current instance and <paramref name="obj">obj</paramref>.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="obj">obj</paramref> is not of type <see cref="T:InitialMargin.Core.Amount"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="obj">obj</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="obj">obj</paramref> is of type <see cref="T:InitialMargin.Core.Amount"/> but expressed in a different currency.</exception>
        public Int32 CompareTo(Object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (obj is Amount amount) 
                return CompareTo(amount);

            throw new ArgumentException($"Cannot compare type {obj.GetType()} to type {GetType()}.", nameof(obj));
        }

        /// <summary>Compares the current instance to the specified object of the same type and returns an indication of their relative value.</summary>
        /// <param name="other">The <see cref="T:InitialMargin.Core.Amount"/> to compare with the current instance.</param>
        /// <returns>An <see cref="T:System.Int32"/> representing the relative value between the current instance and <paramref name="other">other</paramref>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="other">other</paramref> is expressed in a different currency.</exception>
        public Int32 CompareTo(Amount other)
        {
            if (m_Currency == other.Currency)
                return m_Value.CompareTo(other.Value);

            throw new InvalidOperationException("Amounts expressed in different currencies cannot be compared.");
        }
        
        /// <summary>Returns the hash code of the current instance.</summary>
        /// <returns>An <see cref="T:System.Int32"/> representing the hash code of the current instance.</returns>
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

        /// <summary>Returns the text representation of the current instance using <c>N</c> as value format, <see cref="P:System.Globalization.CultureInfo.CurrentCulture"/> as culture-specific format and <see cref="F:InitialMargin.Core.CurrencyCodeSymbol.Before"/> as currency symbol location.</summary>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public override String ToString()
        {
            return ToString("N", CultureInfo.CurrentCulture, CurrencyCodeSymbol.Before);
        }

        /// <summary>Returns the text representation of the current instance using <c>N</c> as value format, <see cref="P:System.Globalization.CultureInfo.CurrentCulture"/> as culture-specific format and the specified currency symbol location.</summary>
        /// <param name="ccs">The enumerator value of type <see cref="T:InitialMargin.Core.CurrencyCodeSymbol"/> representing the currency symbol location.</param>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public String ToString(CurrencyCodeSymbol ccs)
        {
            return ToString("N", CultureInfo.CurrentCulture, ccs);
        }

        /// <summary>Returns the text representation of the current instance using <c>N</c> as value format, the specified culture-specific format and <see cref="F:InitialMargin.Core.CurrencyCodeSymbol.Before"/> as currency symbol location.</summary>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider"/> supplying the culture-specific format.</param>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public String ToString(IFormatProvider formatProvider)
        {
            return ToString("N", formatProvider, CurrencyCodeSymbol.Before);
        }

        /// <summary>Returns the text representation of the current instance using the specified value format, <see cref="P:System.Globalization.CultureInfo.CurrentCulture"/> as culture-specific format and <see cref="F:InitialMargin.Core.CurrencyCodeSymbol.Before"/> as currency symbol location.</summary>
        /// <param name="format">The <see cref="T:System.String"/> representing the value format.</param>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public String ToString(String format)
        {
            return ToString(format, CultureInfo.CurrentCulture, CurrencyCodeSymbol.Before);
        }

        /// <summary>Returns the text representation of the current instance using <c>N</c> as value format, the specified culture-specific format and the specified currency symbol location.</summary>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider"/> supplying the culture-specific format.</param>
        /// <param name="ccs">The enumerator value of type <see cref="T:InitialMargin.Core.CurrencyCodeSymbol"/> representing the currency symbol location.</param>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public String ToString(IFormatProvider formatProvider, CurrencyCodeSymbol ccs)
        {
            return ToString("N", formatProvider, ccs);
        }

        /// <summary>Returns the text representation of the current instance using the specified value format, <see cref="P:System.Globalization.CultureInfo.CurrentCulture"/> as culture-specific format and the specified currency symbol location.</summary>
        /// <param name="format">The <see cref="T:System.String"/> representing the value format.</param>
        /// <param name="ccs">The enumerator value of type <see cref="T:InitialMargin.Core.CurrencyCodeSymbol"/> representing the currency symbol location.</param>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public String ToString(String format, CurrencyCodeSymbol ccs)
        {
            return ToString(format, CultureInfo.CurrentCulture, ccs);
        }

        /// <summary>Returns the text representation of the current instance using the specified value format, the specified culture-specific format and <see cref="F:InitialMargin.Core.CurrencyCodeSymbol.Before"/> as currency symbol location.</summary>
        /// <param name="format">The <see cref="T:System.String"/> representing the value format.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider"/> supplying the culture-specific format.</param>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public String ToString(String format, IFormatProvider formatProvider)
        {
            return ToString(format, formatProvider, CurrencyCodeSymbol.Before);
        }

        /// <summary>Returns the text representation of the current instance using the specified value format, culture-specific format and currency symbol location.</summary>
        /// <param name="format">The <see cref="T:System.String"/> representing the value format.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider"/> supplying the culture-specific format.</param>
        /// <param name="ccs">The enumerator value of type <see cref="T:InitialMargin.Core.CurrencyCodeSymbol"/> representing the currency symbol location.</param>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
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
        /// <summary>Returns a value indicating whether two amounts are equal.</summary>
        /// <param name="left">The first <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <param name="right">The second <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are equal; otherwise, <c>false</c>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        public static Boolean operator ==(Amount left, Amount right)
        {
            return left.Equals(right);
        }

        /// <summary>Returns a value indicating whether two amounts are not equal.</summary>
        /// <param name="left">The first <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <param name="right">The second <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are not equal; otherwise, <c>false</c>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        public static Boolean operator !=(Amount left, Amount right)
        {
            return !(left == right);
        }

        /// <summary>Returns a value indicating whether an amount is less than another amount.</summary>
        /// <param name="left">The first <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <param name="right">The second <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> is less than <paramref name="right">right</paramref>; otherwise, <c>false</c>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        public static Boolean operator <(Amount left, Amount right)
        {
            return (left.CompareTo(right) < 0);
        }

        /// <summary>Returns a value indicating whether an amount is less than or equal to another amount.</summary>
        /// <param name="left">The first <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <param name="right">The second <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> is less than or equal to <paramref name="right">right</paramref>; otherwise, <c>false</c>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        public static Boolean operator <=(Amount left, Amount right)
        {
            return (left.CompareTo(right) <= 0);
        }

        /// <summary>Returns a value indicating whether an amount is greater than another amount.</summary>
        /// <param name="left">The first <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <param name="right">The second <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> is greater than <paramref name="right">right</paramref>; otherwise, <c>false</c>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        public static Boolean operator >(Amount left, Amount right)
        {
            return (left.CompareTo(right) > 0);
        }

        /// <summary>Returns a value indicating whether an amount is greater than or equal to another amount.</summary>
        /// <param name="left">The first <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <param name="right">The second <see cref="T:InitialMargin.Core.Amount"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> is greater than or equal to <paramref name="right">right</paramref>; otherwise, <c>false</c>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        public static Boolean operator >=(Amount left, Amount right)
        {
            return (left.CompareTo(right) >= 0);
        }

        /// <summary>Returns the specified amount with unchanged sign.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> object representing the operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> equal to <paramref name="amount">amount</paramref>.</returns>
        public static Amount operator +(Amount amount)
        {
            return (new Amount(amount.Currency, amount.Value));
        }

        /// <summary>Performs an addition between two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object representing the sum between <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the sum between the values of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator +(Amount left, Amount right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, left.Value + right.Value));
        }

        /// <summary>Performs an addition between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object representing the sum between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the sum between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator +(Amount left, Decimal right)
        {
            return (new Amount(left.Currency, left.Value + right));
        }

        /// <summary>Performs an addition between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object representing the sum between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the sum between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator +(Decimal left, Amount right)
        {
            return (new Amount(right.Currency, left + right.Value));
        }

        /// <summary>Returns the specified amount with changed sign.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> representing the operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object equal to <paramref name="amount">amount</paramref> but with opposite sign, unless the value of <paramref name="amount">amount</paramref> is equal to <c>0</c>.</returns>
        public static Amount operator -(Amount amount)
        {
            if (amount.IsZero)
                return (new Amount(amount.Currency, 0m));

            return (new Amount(amount.Currency, -amount.Value));
        }

        /// <summary>Performs a subtraction between two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object resulting from the subtraction of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the subtraction between the values of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator -(Amount left, Amount right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, left.Value - right.Value));
        }

        /// <summary>Performs a subtraction between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object resulting from the subtraction between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the subtraction between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator -(Amount left, Decimal right)
        {
            return (new Amount(left.Currency, left.Value - right));
        }

        /// <summary>Performs a subtraction between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object resulting from the subtraction between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the subtraction between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator -(Decimal left, Amount right)
        {
            return (new Amount(right.Currency, left - right.Value));
        }

        /// <summary>Performs a multiplication between two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object resulting from the multiplication of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the multiplication between the values of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator *(Amount left, Amount right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, left.Value * right.Value));
        }

        /// <summary>Performs a multiplication between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the multiplication between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the multiplication between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator *(Amount left, Decimal right)
        {
            return (new Amount(left.Currency, left.Value * right));
        }

        /// <summary>Performs a multiplication between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the multiplication between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the multiplication between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator *(Decimal left, Amount right)
        {
            return (new Amount(right.Currency, left * right.Value));
        }

        /// <summary>Performs a division between two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the division of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when the value of <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the division between the values of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator /(Amount left, Amount right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, left.Value / right.Value));
        }

        /// <summary>Performs a division between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the division between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the division between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator /(Amount left, Decimal right)
        {
            return (new Amount(left.Currency, left.Value / right));
        }

        /// <summary>Performs a division between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the division between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when the value of <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the division between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator /(Decimal left, Amount right)
        {
            return (new Amount(right.Currency, left / right.Value));
        }

        /// <summary>Returns the remainder of a division between two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the remainder resulting from the division of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when the value of <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the remainder is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator %(Amount left, Amount right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, left.Value % right.Value));
        }

        /// <summary>Returns the remainder of a division between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the remainder resulting from the division of the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the remainder is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator %(Amount left, Decimal right)
        {
            return (new Amount(left.Currency, left.Value % right));
        }
        
        /// <summary>Returns the remainder of a division between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the remainder resulting from the division of <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when the value of <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the remainder is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount operator %(Decimal left, Amount right)
        {
            return (new Amount(right.Currency, left % right.Value));
        }
        #endregion

        #region Methods (Static)
        internal static Amount OfUnchecked(Currency currency, Decimal value)
        {
            return (new Amount(currency, value));
        }

        /// <summary>Returns the absolute value of the specified amount.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> whose absolute value must be computed.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the absolute value of <paramref name="amount">amount</paramref>.</returns>
        public static Amount Abs(Amount amount)
        {
            return (new Amount(amount.Currency, Math.Abs(amount.Value)));
        }

        /// <summary>Performs an addition between two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the sum between <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the sum between the values of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Add(Amount left, Amount right)
        {
            return (left + right);
        }

        /// <summary>Performs an addition between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the sum between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the sum between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Add(Amount left, Decimal right)
        { 
            return (left + right);
        }

        /// <summary>Performs an addition between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the sum between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the sum between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Add(Decimal left, Amount right)
        { 
            return (left + right);
        }

        /// <summary>Performs a division between two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the division of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when the value of <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the division between the values of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Divide(Amount left, Amount right)
        { 
            return (left / right);
        }

        /// <summary>Performs a division between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the division between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the division between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Divide(Amount left, Decimal right)
        { 
            return (left / right);
        }

        /// <summary>Performs a division between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the division between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when the value of <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the division between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Divide(Decimal left, Amount right)
        { 
            return (left / right);
        }

        /// <summary>Returns the largest of two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the largest amount between <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        public static Amount Max(Amount left, Amount right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, Math.Max(left.Value, right.Value)));
        }

        /// <summary>Returns the largest between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> whose value is the largest between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        public static Amount Max(Amount left, Decimal right)
        {
            return (new Amount(left.Currency, Math.Max(left.Value, right)));
        }

        /// <summary>Returns the largest between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> whose value is the largest between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        public static Amount Max(Decimal left, Amount right)
        {
            return (new Amount(right.Currency, Math.Max(left, right.Value)));
        }

        /// <summary>Returns the smallest of two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the smallest amount between <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        public static Amount Min(Amount left, Amount right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Arithmetic operations cannot be performed on amounts expressed in different currencies.");

            return (new Amount(left.Currency, Math.Min(left.Value, right.Value)));
        }

        /// <summary>Returns the smallest between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> whose value is the smallest between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        public static Amount Min(Amount left, Decimal right)
        {
            return (new Amount(left.Currency, Math.Min(left.Value, right)));
        }

        /// <summary>Returns the smallest between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> whose value is the smallest between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        public static Amount Min(Decimal left, Amount right)
        {
            return (new Amount(right.Currency, Math.Min(left, right.Value)));
        }

        /// <summary>Returns the remainder of a division between two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the remainder resulting from the division of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when the value of <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the remainder is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Mod(Amount left, Amount right)
        { 
            return (left % right);
        }

        /// <summary>Returns the remainder of a division between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the remainder resulting from the division of the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the remainder is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Mod(Amount left, Decimal right)
        { 
            return (left % right);
        }

        /// <summary>Returns the remainder of a division between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> representing the remainder resulting from the division of <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.DivideByZeroException">Thrown when the value of <paramref name="right">right</paramref> is equal to <c>0</c>.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the remainder is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Mod(Decimal left, Amount right)
        { 
            return (left % right);
        }

        /// <summary>Performs a multiplication between two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the multiplication of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the multiplication between the values of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Multiply(Amount left, Amount right)
        { 
            return (left * right);
        }

        /// <summary>Performs a multiplication between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the multiplication between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the multiplication between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Multiply(Amount left, Decimal right)
        { 
            return (left * right);
        }

        /// <summary>Performs a multiplication between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the multiplication between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the multiplication between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Multiply(Decimal left, Amount right)
        { 
            return (left * right);
        }

        /// <summary>Returns the specified amount with changed sign.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> representing the operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> equal to <paramref name="amount">amount</paramref> but with opposite sign, unless the value of <paramref name="amount">amount</paramref> is equal to <c>0</c>.</returns>
        public static Amount Negate(Amount amount)
        {
            return -amount;
        }

        /// <summary>Initializes and returns a new instance using the specified currency and value.</summary>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the amount currency.</param>
        /// <param name="value">The <see cref="T:System.Decimal"/> representing the amount value.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.Amount"/> initialized with the specified currency and amount.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currency">currency</paramref> is <c>null</c>.</exception>
        public static Amount Of(Currency currency, Decimal value)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            return (new Amount(currency, value));
        }

        /// <summary>Initializes and returns a new instance using the specified currency and a value equal to <c>1</c>.</summary>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the amount currency.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.Amount"/> initialized with the specified currency and a value equal to <c>1</c>.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currency">currency</paramref> is <c>null</c>.</exception>
        public static Amount OfOne(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            return (new Amount(currency, 1m));
        }

        /// <summary>Initializes and returns a new instance using the specified currency and a value equal to <c>0</c>.</summary>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the amount currency.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.Amount"/> initialized with the specified currency and a value equal to <c>0</c>.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currency">currency</paramref> is <c>null</c>.</exception>
        public static Amount OfZero(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            return (new Amount(currency, 0m));
        }

        /// <summary>Returns the specified amount with unchanged sign.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> representing the operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> equal to <paramref name="amount">amount</paramref>.</returns>
        public static Amount Plus(Amount amount)
        { 
            return +amount;
        }

        /// <summary>Rounds the specified amount to the nearest integer.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> to round.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> whose value is the integer that is nearest to the value of <paramref name="amount">amount</paramref>. If the value of <paramref name="amount">amount</paramref> is halfway between two integers, one of which is even and the other odd, the even number is taken.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the result is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Round(Amount amount)
        {
            return (new Amount(amount.Currency, Decimal.Round(amount.Value)));
        }

        /// <summary>Rounds the specified amount to the specified number of decimal places.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> to round.</param>
        /// <param name="decimals">The <see cref="T:System.Int32"/> value that specifies the number of decimal places to round to.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> whose value is equivalent to the value of <paramref name="amount">amount</paramref> rounded to <paramref name="decimals">decimals</paramref> number of decimal places.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when <paramref name="decimals">decimals</paramref> is less than <c>0</c> or greater than <c>28</c>.</exception>
        public static Amount Round(Amount amount, Int32 decimals)
        {
            return (new Amount(amount.Currency, Decimal.Round(amount.Value, decimals)));
        }

        /// <summary>Rounds the specified amount to the nearest integer. A parameter specifies how to round its value if it is midway between two other numbers.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> to round.</param>
        /// <param name="mode">An enumerator value of type <see cref="T:System.MidpointRounding"/> that specifies how to round the value if it is midway between two other numbers.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> whose value is the integer that is nearest to the value of <paramref name="amount">amount</paramref>. If the value of <paramref name="amount">amount</paramref> is halfway between two numbers, one of which is even and the other odd, <paramref name="mode">mode</paramref> determines which of the two numbers is taken.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the result is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="mode">mode</paramref> is undefined.</exception>
        public static Amount Round(Amount amount, MidpointRounding mode)
        {
            if (!Enum.IsDefined(typeof(MidpointRounding), mode))
                throw new InvalidEnumArgumentException("Invalid mode specified.");

            return (new Amount(amount.Currency, Decimal.Round(amount.Value, mode)));
        }

        /// <summary>Rounds the specified amount to the specified number of decimal places. A parameter specifies how to round its value if it is midway between two other numbers.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> to round.</param>
        /// <param name="decimals">The <see cref="T:System.Int32"/> value that specifies the number of decimal places to round to.</param>
        /// <param name="mode">An enumerator value of type <see cref="T:System.MidpointRounding"/> that specifies how to round the value if it is midway between two other numbers.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> whose value is equivalent to the value of <paramref name="amount">amount</paramref> rounded to <paramref name="decimals">decimals</paramref> number of decimal places. If the value of <paramref name="amount">amount</paramref> is halfway between two numbers, one of which is even and the other odd, <paramref name="mode">mode</paramref> determines which of the two numbers is taken.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when <paramref name="decimals">decimals</paramref> is less than <c>0</c> or greater than <c>28</c>.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the result is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="mode">mode</paramref> is undefined.</exception>
        public static Amount Round(Amount amount, Int32 decimals, MidpointRounding mode)
        {
            if (!Enum.IsDefined(typeof(MidpointRounding), mode))
                throw new InvalidEnumArgumentException("Invalid mode specified.");

            return (new Amount(amount.Currency, Decimal.Round(amount.Value, decimals, mode)));
        }

        /// <summary>Returns the specified amount raised to the power of <c>2</c>.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> representing the operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> equal to <paramref name="amount">amount</paramref> raised to the power of <c>2</c>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the result is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Square(Amount amount)
        {
            return (new Amount(amount.Currency, MathUtilities.Square(amount.Value)));
        }

        /// <summary>Returns the square root of the specified amount.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> representing the operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> equal to the square root of <paramref name="amount">amount</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the result is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount SquareRoot(Amount amount)
        {
            return (new Amount(amount.Currency, MathUtilities.SquareRoot(amount.Value)));
        }

        /// <summary>Performs a subtraction between two amounts.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the subtraction of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are expressed in different currencies.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the subtraction between the values of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Subtract(Amount left, Amount right)
        { 
            return (left - right);
        }

        /// <summary>Performs a subtraction between an amount and a decimal value.</summary>
        /// <param name="left">The <see cref="T:InitialMargin.Core.Amount"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:System.Decimal"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the subtraction between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the subtraction between the value of <paramref name="left">left</paramref> and <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Subtract(Amount left, Decimal right)
        { 
            return (left - right);
        }
        
        /// <summary>Performs a subtraction between a decimal value and an amount.</summary>
        /// <param name="left">The <see cref="T:System.Decimal"/> representing the first operand.</param>
        /// <param name="right">The <see cref="T:InitialMargin.Core.Amount"/> representing the second operand.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> resulting from the subtraction between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref>.</returns>
        /// <exception cref="T:System.OverflowException">Thrown when the subtraction between <paramref name="left">left</paramref> and the value of <paramref name="right">right</paramref> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Subtract(Decimal left, Amount right)
        { 
            return (left - right);
        }
        
        /// <summary>Computes the sum of a sequence of amounts.</summary>
        /// <param name="amounts">A sequence of <see cref="T:InitialMargin.Core.Amount"/> objects to calculate the sum of.</param>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the fallback currency used for returning an amount with a value equal to <c>0</c> when the sequence is empty.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="amounts">amounts</paramref> is <c>null</c> or when <paramref name="currency">currency</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.OverflowException">Thrown when the sum is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.</exception>
        public static Amount Sum(IEnumerable<Amount> amounts, Currency currency)
        {
            if (amounts == null)
                throw new ArgumentNullException(nameof(amounts));

            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            return amounts.Aggregate(OfZero(currency), (a1, a2) => a1 + a2);
        }
        #endregion
    }
}
