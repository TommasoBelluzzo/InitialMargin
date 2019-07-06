#region Using Directives
using System;
using System.ComponentModel;
using System.Globalization;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Model
{
    public sealed class AddOnFixedAmount : DataValue
    {
        #region Constructors
        private AddOnFixedAmount(Amount amount, RegulationsInfo regulationsInfo) : base(amount, regulationsInfo, TradeInfo.Empty) { }
        #endregion

        #region Methods
        public override String ToString()
        {
            return $"{GetType().Name} Amount=\"{Amount}\"";
        }
        #endregion

        #region Methods (Static)
        public static AddOnFixedAmount Of(Amount amount, RegulationsInfo regulationsInfo)
        {
            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            return (new AddOnFixedAmount(amount, regulationsInfo));
        }
        #endregion
    }

    public sealed class AddOnNotional : DataValue
    {
        #region Members
        private readonly String m_Qualifier;
        #endregion

        #region Properties
        public String Qualifier => m_Qualifier;
        #endregion

        #region Constructors
        private AddOnNotional(String qualifier, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo) : base(amount, regulationsInfo, tradeInfo)
        {
            m_Qualifier = qualifier;
        }
        #endregion

        #region Methods
        public override String ToString()
        {
            return $"{GetType().Name} Qualifier=\"{m_Qualifier}\" Amount=\"{Amount}\"";
        }
        #endregion

        #region Methods (Static)
        public static AddOnNotional Of(String qualifier, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (!DataValidator.IsValidNotionalQualifier(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new AddOnNotional(qualifier, amount, regulationsInfo, tradeInfo));
        }
        #endregion
    }

    public sealed class AddOnNotionalFactor : DataParameter
    {
        #region Members
        private readonly String m_Qualifier; 
        #endregion

        #region Properties
        public String Qualifier => m_Qualifier;
        #endregion

        #region Constructors
        private AddOnNotionalFactor(String qualifier, Decimal factor, RegulationsInfo regulationsInfo) : base(factor, regulationsInfo)
        {
            m_Qualifier = qualifier;
        }
        #endregion

        #region Methods
        public override String ToString()
        {
            return $"{GetType().Name} Qualifier=\"{m_Qualifier}\" Factor=\"{Parameter.ToString(CultureInfo.CurrentCulture)}\"";
        }
        #endregion

        #region Methods (Static)
        public static AddOnNotionalFactor Of(String qualifier, Decimal factor, RegulationsInfo regulationsInfo)
        {
            if (!DataValidator.IsValidNotionalQualifier(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if ((factor <= 0m) || (factor > 1m))
                throw new ArgumentException("Invalid factor specified.", nameof(factor));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            return (new AddOnNotionalFactor(qualifier, factor, regulationsInfo));
        }
        #endregion
    }

    public sealed class AddOnProductMultiplier : DataParameter
    {
        #region Members
        private readonly Product m_Product;
        #endregion

        #region Properties
        public Product Product => m_Product;
        #endregion

        #region Constructors
        private AddOnProductMultiplier(Product product, Decimal multiplier, RegulationsInfo regulationsInfo) : base (multiplier, regulationsInfo)
        {
            m_Product = product;
        }
        #endregion

        #region Methods
        public override String ToString()
        {
            return $"{GetType().Name} Product=\"{m_Product}\" Multiplier=\"{Parameter.ToString(CultureInfo.CurrentCulture)}\"";
        }
        #endregion

        #region Methods (Static)
        public static AddOnProductMultiplier Of(Product product, Decimal multiplier, RegulationsInfo regulationsInfo)
        {
            if (!Enum.IsDefined(typeof(Product), product))
                throw new InvalidEnumArgumentException("Invalid product specified.");

            if (multiplier <= 0m)
                throw new ArgumentException("Invalid multiplier specified.", nameof(multiplier));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            return (new AddOnProductMultiplier(product, multiplier, regulationsInfo));
        }
        #endregion
    }
}