#region Using Directives
using System;
#endregion

namespace InitialMargin.Core
{
    /// <summary>Provides information about a specific trade, which is aimed at enriching <see cref="T:InitialMargin.Core.DataEntity"/> objects. This class cannot be derived.</summary>
    public sealed class TradeInfo : IEquatable<TradeInfo>
    {
        #region Members
        private readonly DateTime m_EndDate;
        private readonly String m_PortfolioId;
        private readonly String m_TradeId;
        #endregion

        #region Members (Static)
        /// <summary>Represents an empty <see cref="T:InitialMargin.Core.TradeInfo"/>. This field is read-only.</summary>
        public static readonly TradeInfo Empty = new TradeInfo("~", "~", DateTime.Today);
        #endregion

        #region Properties
        /// <summary>Gets the end date of the trade.</summary>
        /// <value>A <see cref="T:System.DateTime"/> without time component.</value>
        public DateTime EndDate => m_EndDate;
        
        /// <summary>Gets the portfolio identifier of the trade.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String PortfolioId => m_PortfolioId;

        /// <summary>Gets the identifier of the trade.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String TradeId => m_TradeId;
        #endregion

        #region Constructors
        private TradeInfo(String portfolioId, String tradeId, DateTime endDate)
        {
            m_EndDate = endDate;
            m_PortfolioId = portfolioId;
            m_TradeId = tradeId;
        }
        #endregion

        #region Methods
        /// <summary>Indicates whether the current instance is equal to the specified object.</summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current instance.</param>
        /// <returns><c>true</c> if <paramref name="obj">obj</paramref> is an instance of <see cref="T:InitialMargin.Core.TradeInfo"/> and is equal to the current instance; otherwise, <c>false</c>.</returns>
        public override Boolean Equals(Object obj)
        {
            return Equals(obj as TradeInfo);
        }

        /// <summary>Indicates whether the current instance is equal to the specified object of the same type.</summary>
        /// <param name="other">The <see cref="T:InitialMargin.Core.TradeInfo"/> to compare with the current instance.</param>
        /// <returns><c>true</c> if <paramref name="other">other</paramref> is equal to the current instance; otherwise, <c>false</c>.</returns>
        public Boolean Equals(TradeInfo other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return ((m_PortfolioId == other.PortfolioId) && (m_TradeId == other.TradeId) && (m_EndDate == other.EndDate)); 
        }

        /// <summary>Returns the hash code of the current instance.</summary>
        /// <returns>An <see cref="T:System.Int32"/> representing the hash code of the current instance.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hash = 17;
                hash = (hash * 23) + m_PortfolioId.GetHashCode();
                hash = (hash * 23) + m_TradeId.GetHashCode();
                hash = (hash * 23) + m_EndDate.GetHashCode();

                return hash;
            }
        } 

        /// <summary>Returns the text representation of the current instance.</summary>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public override String ToString()
        {
            return $"Trade TradeID=\"{m_TradeId}\" PortfolioID=\"{m_PortfolioId}\" EndDate=\"{m_EndDate:yyyy-MM-dd}\"";
        }
        #endregion

        #region Methods (Operators)
        /// <summary>Returns a value indicating whether two trade information are equal.</summary>
        /// <param name="left">The first <see cref="T:InitialMargin.Core.TradeInfo"/> object to compare.</param>
        /// <param name="right">The second <see cref="T:InitialMargin.Core.TradeInfo"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are equal; otherwise, <c>false</c>.</returns>
        public static Boolean operator ==(TradeInfo left, TradeInfo right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        /// <summary>Returns a value indicating whether two trade information are not equal.</summary>
        /// <param name="left">The first <see cref="T:InitialMargin.Core.TradeInfo"/> object to compare.</param>
        /// <param name="right">The second <see cref="T:InitialMargin.Core.TradeInfo"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are not equal; otherwise, <c>false</c>.</returns>
        public static Boolean operator !=(TradeInfo left, TradeInfo right)
        {
            return !(left == right);
        }
        #endregion

        #region Methods (Static)
        internal static TradeInfo OfUnchecked(String portfolioId, String tradeId, DateTime endDate)
        {
            return (new TradeInfo(portfolioId, tradeId, endDate));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="portfolioId">The <see cref="T:System.String"/> representing the portfolio identifier of the trade.</param>
        /// <param name="tradeId">The <see cref="T:System.String"/> representing the identifier of the trade.</param>
        /// <param name="endDate">The <see cref="T:System.DateTime"/> representing the end date of the trade.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.TradeInfo"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="portfolioId">portfolioId</paramref> is invalid or when <paramref name="tradeId">tradeId</paramref> is invalid. See <see cref="M:InitialMargin.Core.DataValidator.IsValidTradeReference(System.String)"/>.</exception>
        public static TradeInfo Of(String portfolioId, String tradeId, DateTime endDate)
        {
            if (!DataValidator.IsValidTradeReference(portfolioId))
                throw new ArgumentException("Invalid portfolio identifier specified.", nameof(portfolioId));

            if (!DataValidator.IsValidTradeReference(tradeId))
                throw new ArgumentException("Invalid trade identifier specified.", nameof(tradeId));

            return (new TradeInfo(portfolioId, tradeId, endDate.Date));
        }
        #endregion
    }
}
