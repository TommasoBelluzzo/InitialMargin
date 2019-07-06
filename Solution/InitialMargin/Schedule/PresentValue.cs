#region Using Directives
using System;
using System.ComponentModel;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Schedule
{
    public sealed class PresentValue : DataValue
    {
        #region Members
        private readonly Product m_Product;
        #endregion
 
        #region Properties
        public Product Product => m_Product;
        #endregion

        #region Constructors
        private PresentValue(Product product, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo) : base(amount, regulationsInfo, tradeInfo)
        {
            m_Product = product;
        }
        #endregion

        #region Methods
        public override String ToString()
        {
            return $"{GetType().Name} Product=\"{m_Product}\" Amount=\"{Amount}\"";
        }
        #endregion

        #region Methods (Static)
        public static PresentValue Of(Product product, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (!Enum.IsDefined(typeof(Product), product))
                throw new InvalidEnumArgumentException("Invalid product specified.");

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new PresentValue(product, amount, regulationsInfo, tradeInfo));
        }
        #endregion
    }
}