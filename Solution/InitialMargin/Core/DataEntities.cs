#region Using Directives
using System;
using System.Collections.ObjectModel;
#endregion

namespace InitialMargin.Core
{
    public abstract class DataEntity : ICloneable
    {
        #region Members
        private readonly RegulationsInfo m_RegulationsInfo;
        #endregion

        #region Properties
        public ReadOnlyCollection<Regulation> CollectRegulations => m_RegulationsInfo.CollectRegulations;

        public ReadOnlyCollection<Regulation> PostRegulations => m_RegulationsInfo.PostRegulations;
        #endregion

        #region Constructors
        protected DataEntity(RegulationsInfo regulationsInfo)
        {
            m_RegulationsInfo = regulationsInfo;
        }
        #endregion

        #region Methods
        public Object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }

    public abstract class DataParameter : DataEntity
    {
        #region Members
        private readonly Decimal m_Parameter;
        #endregion

        #region Properties
        public Decimal Parameter => m_Parameter;
        #endregion

        #region Constructors
        protected DataParameter(Decimal parameter, RegulationsInfo regulationsInfo) : base(regulationsInfo)
        {
            m_Parameter = parameter;
        }
        #endregion
    }

    public abstract class DataValue : DataEntity
    {
        #region Members
        private Amount m_Amount;
        private readonly TradeInfo m_TradeInfo;
        #endregion

        #region Properties
        public Amount Amount
        {
            get => m_Amount;
            protected set => m_Amount = value; 
        }

        public DateTime EndDate => m_TradeInfo.EndDate;

        public String PortfolioId => m_TradeInfo.PortfolioId;

        public String TradeId => m_TradeInfo.TradeId;
        #endregion

        #region Constructors
        protected DataValue(Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo) : base(regulationsInfo)
        {
            m_TradeInfo = tradeInfo;
            m_Amount = amount;
        }
        #endregion

        #region Methods
        public DataValue ChangeAmount(Amount amount)
        {
            Amount = amount;
            return this;
        }
        #endregion
    }
}