#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Schedule
{
    internal sealed class Margin : Margin<MarginProduct>
    {
        #region Constants
        private const Int32 LEVEL = 2;
        private const String NAME = "Model";
        private const String IDENTIFIER = "Schedule";
        #endregion

        #region Constructors
        private Margin(Amount amount, List<MarginProduct> productMargins) : base(LEVEL, NAME, IDENTIFIER, amount, productMargins) { }
        #endregion

        #region Methods (Static)
        public static Margin Of(Amount amount, List<MarginProduct> productMargins)
        {
            if (productMargins == null)
                throw new ArgumentNullException(nameof(productMargins));

            if (productMargins.Count == 0)
                throw new ArgumentException("No product margins have been provided.", nameof(productMargins));

            if (productMargins.Any(x => x == null))
                throw new ArgumentException("One or more product margins are null.", nameof(productMargins));

            List<MarginProduct> productMarginsSorted = productMargins
                .OrderBy(x => (Int32)x.Product)
                .ToList();

            return (new Margin(amount, productMarginsSorted));
        }
        #endregion
    }

    internal sealed class MarginProduct : Margin<IMargin>
    {
        #region Constants
        private const Int32 LEVEL = 3;
        private const String NAME = "Product";
        #endregion

        #region Members
        private readonly Product m_Product;
        #endregion

        #region Properties
        public Product Product => m_Product;
        #endregion

        #region Constructors
        private MarginProduct(Product product, Amount amount) : base(LEVEL, NAME, product.ToString(), amount)
        {
            m_Product = product;
        }
        #endregion

        #region Methods (Static)
        public static MarginProduct Of(Product product, Amount amount)
        {
            if (!Enum.IsDefined(typeof(Product), product))
                throw new InvalidEnumArgumentException("Invalid product specified.");

            return (new MarginProduct(product, amount));
        }
        #endregion
    }
}
