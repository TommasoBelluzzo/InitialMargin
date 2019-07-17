#region Using Directives
using System;
using System.ComponentModel;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Schedule
{
    /// <summary>Represents a notional. This class cannot be derived.</summary>
    public sealed class Notional : DataValue
    {
        #region Members
        private readonly Product m_Product;
        #endregion

        #region Properties
        /// <summary>Gets the product of the entity.</summary>
        /// <value>An enumerator value of type <see cref="T:InitialMargin.Schedule.Product"/>.</value>
        public Product Product => m_Product;
        #endregion

        #region Constructors
        private Notional(Product product, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo) : base(amount, regulationsInfo, tradeInfo)
        {
            m_Product = product;
        }
        #endregion

        #region Methods
        /// <inheritdoc/>
        public override String ToString()
        {
            return $"{GetType().Name} Product=\"{m_Product}\" Amount=\"{Amount}\"";
        }
        #endregion

        #region Methods (Static)
        internal static Notional OfUnchecked(Product product, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            return (new Notional(product, amount, regulationsInfo, tradeInfo));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="product">The enumerator value of type <see cref="T:InitialMargin.Schedule.Product"/> representing the product of the entity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the entity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the entity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the entity.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Schedule.Notional"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="product">product</paramref> is undefined.</exception>
        public static Notional Of(Product product, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (!Enum.IsDefined(typeof(Product), product))
                throw new InvalidEnumArgumentException("Invalid product specified.");

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Notional(product, amount, regulationsInfo, tradeInfo));
        }
        #endregion
    }
}