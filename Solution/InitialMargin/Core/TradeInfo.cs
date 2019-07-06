#region Using Directives
using System;
#endregion

namespace InitialMargin.Core
{
    public sealed class TradeInfo : IEquatable<TradeInfo>
    {
        #region Members
        private readonly DateTime m_EndDate;
        private readonly String m_PortfolioId;
        private readonly String m_TradeId;
        #endregion

        #region Members (Static)
        public static readonly TradeInfo Empty = new TradeInfo("~", "~", DateTime.Today);
        #endregion

        #region Properties
        public DateTime EndDate => m_EndDate;
        
        public String PortfolioId => m_PortfolioId;

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
        public override Boolean Equals(Object obj)
        {
            return Equals(obj as TradeInfo);
        }

        public Boolean Equals(TradeInfo other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return ((m_PortfolioId == other.PortfolioId) && (m_TradeId == other.TradeId) && (m_EndDate == other.EndDate)); 
        }

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

        public override String ToString()
        {
            return $"Trade TradeID=\"{m_TradeId}\" PortfolioID=\"{m_PortfolioId}\" EndDate=\"{m_EndDate:yyyy-MM-dd}\"";
        }
        #endregion

        #region Methods (Static)
        public static TradeInfo Of(String portfolioId, String tradeId, DateTime endDate)
        {
            if (String.IsNullOrWhiteSpace(tradeId))
                throw new ArgumentException("Invalid trade identifier specified.", nameof(tradeId));

            if (String.IsNullOrWhiteSpace(portfolioId))
                throw new ArgumentException("Invalid portfolio identifier specified.", nameof(portfolioId));

            return (new TradeInfo(portfolioId, tradeId, endDate));
        }
        #endregion
    }
}
