#region Using Directives
using System;
using System.Collections.ObjectModel;
#endregion

namespace InitialMargin.Core
{
    /// <summary>Represents the base class from which all entities must derive. This class is abstract.</summary>
    public abstract class DataEntity : ICloneable
    {
        #region Members
        private readonly RegulationsInfo m_RegulationsInfo;
        #endregion

        #region Properties
        /// <summary>Gets the collect regulations of the entity.</summary>
        /// <value>A <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/> containing <c>0</c> or more <see cref="T:InitialMargin.Core.Regulation"/> values.</value>
        public ReadOnlyCollection<Regulation> CollectRegulations => m_RegulationsInfo.CollectRegulations;

        /// <summary>Gets the post regulations of the entity.</summary>
        /// <value>A <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/> containing <c>0</c> or more <see cref="T:InitialMargin.Core.Regulation"/> values.</value>
        public ReadOnlyCollection<Regulation> PostRegulations => m_RegulationsInfo.PostRegulations;
        #endregion

        #region Constructors
        /// <summary>Represents the base constructor used by derived classes.</summary>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the entity.</param>
        protected DataEntity(RegulationsInfo regulationsInfo)
        {
            m_RegulationsInfo = regulationsInfo;
        }
        #endregion

        #region Methods
        /// <summary>Creates a copy of the current instance.</summary>
        /// <returns>An <see cref="T:System.Object"/> representing a copy of the current instance.</returns>
        public Object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>Returns the text representation of the current instance.</summary>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public override String ToString()
        {
            return GetType().Name;
        }

        #endregion
    }

    /// <summary>Represents the base class from which all parameter entities must derive. This class is abstract.</summary>
    public abstract class DataParameter : DataEntity
    {
        #region Members
        private readonly Decimal m_Parameter;
        #endregion

        #region Properties
        /// <summary>Gets the parameter of the entity.</summary>
        /// <value>A <see cref="T:System.Decimal"/> value.</value>
        public Decimal Parameter => m_Parameter;
        #endregion

        #region Constructors
        /// <summary>Represents the base constructor used by derived classes.</summary>
        /// <param name="parameter">The <see cref="T:System.Decimal"/> representing the parameter of the entity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the entity.</param>
        protected DataParameter(Decimal parameter, RegulationsInfo regulationsInfo) : base(regulationsInfo)
        {
            m_Parameter = parameter;
        }
        #endregion
    }

    /// <summary>Represents the base class from which all value entities must derive. This class is abstract.</summary>
    public abstract class DataValue : DataEntity
    {
        #region Members
        private Amount m_Amount;
        private readonly TradeInfo m_TradeInfo;
        #endregion

        #region Properties
        /// <summary>Gets or sets the entity amount.</summary>
        /// <value>An <see cref="T:InitialMargin.Core.Amount"/> object.</value>
        public Amount Amount
        {
            get => m_Amount;
            protected set => m_Amount = value; 
        }

        /// <summary>Gets the end date of the underlying trade of the entity.</summary>
        /// <value>A <see cref="T:System.DateTime"/> without time component.</value>
        public DateTime EndDate => m_TradeInfo.EndDate;

        /// <summary>Gets the portfolio identifier of the underlying trade of the entity.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String PortfolioId => m_TradeInfo.PortfolioId;

        /// <summary>Gets the identifier of the underlying trade of the entity.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String TradeId => m_TradeInfo.TradeId;
        #endregion

        #region Constructors
        /// <summary>Represents the base constructor used by derived classes.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the entity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the entity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the entity.</param>
        protected DataValue(Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo) : base(regulationsInfo)
        {
            m_TradeInfo = tradeInfo;
            m_Amount = amount;
        }
        #endregion

        #region Methods
        /// <summary>Changes the amount of the current instance and returns it.</summary>
        /// <param name="amount">The new <see cref="T:InitialMargin.Core.Amount"/> of the current instance.</param>
        /// <returns>A reference to the current instance.</returns>
        public DataValue ChangeAmount(Amount amount)
        {
            Amount = amount;
            return this;
        }
        #endregion
    }
}