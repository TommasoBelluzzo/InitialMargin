#region Using Directives
using System;
using System.ComponentModel;
using System.Globalization;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Model
{
    /// <summary>Represents a fixed amount add-on. This class cannot be derived.</summary>
    public sealed class AddOnFixedAmount : DataValue
    {
        #region Constructors
        private AddOnFixedAmount(Amount amount, RegulationsInfo regulationsInfo) : base(amount, regulationsInfo, TradeInfo.Empty) { }
        #endregion

        #region Methods
        /// <inheritdoc/>
        public override String ToString()
        {
            return $"{GetType().Name} Amount=\"{Amount}\"";
        }
        #endregion

        #region Methods (Static)
        internal static AddOnFixedAmount OfUnchecked(Amount amount, RegulationsInfo regulationsInfo)
        {
            return (new AddOnFixedAmount(amount, regulationsInfo));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the entity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the entity.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Model.AddOnFixedAmount"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c>.</exception>
        public static AddOnFixedAmount Of(Amount amount, RegulationsInfo regulationsInfo)
        {
            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            return (new AddOnFixedAmount(amount, regulationsInfo));
        }
        #endregion
    }

    /// <summary>Represents a notional add-on. This class cannot be derived.</summary>
    public sealed class AddOnNotional : DataValue
    {
        #region Members
        private readonly String m_Qualifier;
        #endregion

        #region Properties
        /// <summary>Gets the qualifier of the entity.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Qualifier => m_Qualifier;
        #endregion

        #region Constructors
        private AddOnNotional(String qualifier, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo) : base(amount, regulationsInfo, tradeInfo)
        {
            m_Qualifier = qualifier;
        }
        #endregion

        #region Methods
        /// <inheritdoc/>
        public override String ToString()
        {
            return $"{GetType().Name} Qualifier=\"{m_Qualifier}\" Amount=\"{Amount}\"";
        }
        #endregion

        #region Methods (Static)
        internal static AddOnNotional OfUnchecked(String qualifier, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            return (new AddOnNotional(qualifier, amount, regulationsInfo, tradeInfo));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="qualifier">The <see cref="T:System.String"/> representing the qualifier of the entity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the entity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the entity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the entity.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Model.AddOnNotional"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="qualifier">qualifier</paramref> is invalid. See <see cref="M:InitialMargin.Core.DataValidator.IsValidNotionalQualifier(System.String)"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
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

    /// <summary>Represents a notional add-on factor. This class cannot be derived.</summary>
    public sealed class AddOnNotionalFactor : DataParameter
    {
        #region Members
        private readonly String m_Qualifier; 
        #endregion

        #region Properties
        /// <summary>Gets the qualifier of the entity.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Qualifier => m_Qualifier;
        #endregion

        #region Constructors
        private AddOnNotionalFactor(String qualifier, Decimal factor, RegulationsInfo regulationsInfo) : base(factor, regulationsInfo)
        {
            m_Qualifier = qualifier;
        }
        #endregion

        #region Methods
        /// <inheritdoc/>
        public override String ToString()
        {
            return $"{GetType().Name} Qualifier=\"{m_Qualifier}\" Factor=\"{Parameter.ToString(CultureInfo.CurrentCulture)}\"";
        }
        #endregion

        #region Methods (Static)
        internal static AddOnNotionalFactor OfUnchecked(String qualifier, Decimal factor, RegulationsInfo regulationsInfo)
        {
            return (new AddOnNotionalFactor(qualifier, factor, regulationsInfo));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="qualifier">The <see cref="T:System.String"/> representing the qualifier of the entity.</param>
        /// <param name="factor">The <see cref="T:System.Decimal"/> representing the parameter of the entity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the entity.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Model.AddOnNotionalFactor"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="qualifier">qualifier</paramref> is invalid (see <see cref="M:InitialMargin.Core.DataValidator.IsValidNotionalQualifier(System.String)"/>) or when <paramref name="factor">factor</paramref> does not belong to the interval <c>(0,1]</c>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c>.</exception>
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

    /// <summary>Represents a product multiplier add-on. This class cannot be derived.</summary>
    public sealed class AddOnProductMultiplier : DataParameter
    {
        #region Members
        private readonly Product m_Product;
        #endregion

        #region Properties
        /// <summary>Gets the product of the entity.</summary>
        /// <value>An enumerator value of type <see cref="T:InitialMargin.Model.Product"/>.</value>
        public Product Product => m_Product;
        #endregion

        #region Constructors
        private AddOnProductMultiplier(Product product, Decimal multiplier, RegulationsInfo regulationsInfo) : base (multiplier, regulationsInfo)
        {
            m_Product = product;
        }
        #endregion

        #region Methods
        /// <inheritdoc/>
        public override String ToString()
        {
            return $"{GetType().Name} Product=\"{m_Product}\" Multiplier=\"{Parameter.ToString(CultureInfo.CurrentCulture)}\"";
        }
        #endregion

        #region Methods (Static)
        internal static AddOnProductMultiplier OfUnchecked(Product product, Decimal multiplier, RegulationsInfo regulationsInfo)
        {
            return (new AddOnProductMultiplier(product, multiplier, regulationsInfo));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="product">The enumerator value of type <see cref="T:InitialMargin.Model.Product"/> representing the product of the entity.</param>
        /// <param name="multiplier">The <see cref="T:System.Decimal"/> representing the parameter of the entity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the entity.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Model.AddOnProductMultiplier"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="multiplier">multiplier</paramref> is less than or equal to <c>0</c>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="product">product</paramref> is undefined.</exception>
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