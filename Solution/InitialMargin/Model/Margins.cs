#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Model
{
    internal sealed class Margin : Margin<IMargin>
    {
        #region Constants
        private const Int32 LEVEL = 2;
        private const String NAME = "Model";
        private const String IDENTIFIER = "SIMM";
        #endregion

        #region Constructors
        private Margin(Amount amount, List<IMargin> children) : base(LEVEL, NAME, IDENTIFIER, amount, children) { }
        #endregion

        #region Methods (Static)
        public static Margin Of(Amount amount, List<MarginProduct> productMargins)
        {
            return Of(amount, productMargins, null);
        }

        public static Margin Of(Amount amount, List<MarginProduct> productMargins, MarginAddOn addOnMargin)
        {
            if (productMargins == null)
                throw new ArgumentNullException(nameof(productMargins));

            if (productMargins.Count == 0)
                throw new ArgumentException("No product margins have been provided.", nameof(productMargins));

            if (productMargins.Any(x => x == null))
                throw new ArgumentException("One or more product margins are null.", nameof(productMargins));

            List<IMargin> childrenSorted = productMargins
                .OrderBy(x => (Int32)x.Product)
                .Cast<IMargin>().ToList();

            if (addOnMargin != null)
                childrenSorted.Add(addOnMargin);

            return (new Margin(amount, childrenSorted));
        }
        #endregion
    }

    internal sealed class MarginBucket : Margin<MarginWeighting>
    {
        #region Constants
        private const Int32 LEVEL = 6;
        private const String NAME = "Bucket";
        #endregion

        #region Members
        private readonly IBucket m_Bucket;
        #endregion

        #region Properties
        public IBucket Bucket => m_Bucket;
        #endregion

        #region Constructors
        private MarginBucket(IBucket bucket, Amount amount, List<MarginWeighting> weightingMargins) : base(LEVEL, NAME, CreateBucketIdentifier(bucket), amount, weightingMargins)
        {
            m_Bucket = bucket;
        }
        #endregion

        #region Methods (Static)
        private static String CreateBucketIdentifier(IBucket bucket)
        {
            if (bucket.IsResidual)
                return "Residual";

            if (bucket is Placeholder)
                return "Common";

            if (bucket is Currency currency)
                return $"{currency.Name}";

            return $"{bucket.Description}";
        }

        public static MarginBucket Of(IBucket bucket, Amount amount, List<MarginWeighting> weightingMargins)
        {
            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            if (weightingMargins == null)
                throw new ArgumentNullException(nameof(weightingMargins));

            if (weightingMargins.Count == 0)
                throw new ArgumentException("No weighting margins have been provided.", nameof(weightingMargins));

            if (weightingMargins.Any(x => x == null))
                throw new ArgumentException("One or more weighting margins are null.", nameof(weightingMargins));

            List<MarginWeighting> weightingMarginsSorted = weightingMargins
                .OrderBy(x => (x.Sensitivity.Risk == SensitivityRisk.Rates) ? x.Sensitivity.GetType().Name : x.Sensitivity.Qualifier)
                .ThenBy(x => (x.Sensitivity.Tenor != null) ? x.Sensitivity.Tenor.Days : 0m)
                .ThenBy(x => x.Sensitivity.Label2)
                .ToList();

            return (new MarginBucket(bucket, amount, weightingMarginsSorted));
        }
        #endregion
    }

    internal sealed class MarginProduct : Margin<MarginRisk>
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
        private MarginProduct(Product product, Amount amount, List<MarginRisk> riskMargins) : base(LEVEL, NAME, product.ToString(), amount, riskMargins)
        {
            m_Product = product;
        }
        #endregion

        #region Methods (Static)
        public static MarginProduct Of(Product product, Amount amount, List<MarginRisk> riskMargins)
        {
            if (!Enum.IsDefined(typeof(Product), product))
                throw new InvalidEnumArgumentException("Invalid product specified.");

            if (riskMargins == null)
                throw new ArgumentNullException(nameof(riskMargins));

            if (riskMargins.Count == 0)
                throw new ArgumentException("No risk margins have been provided.", nameof(riskMargins));

            if (riskMargins.Any(x => x == null))
                throw new ArgumentException("One or more risk margins are null.", nameof(riskMargins));

            List<MarginRisk> riskMarginsSorted = riskMargins
                .OrderBy(x => (Int32)x.Risk)
                .ToList();

            return (new MarginProduct(product, amount, riskMarginsSorted));
        }
        #endregion
    }

    internal sealed class MarginRisk : Margin<MarginSensitivity>
    {
        #region Constants
        private const Int32 LEVEL = 4;
        private const String NAME = "Risk";
        #endregion

        #region Members
        private readonly SensitivityRisk m_Risk;
        #endregion

        #region Properties
        public SensitivityRisk Risk => m_Risk;
        #endregion

        #region Constructors
        private MarginRisk(SensitivityRisk risk, Amount amount, List<MarginSensitivity> sensitivityMargins) : base(LEVEL, NAME, risk.ToString(), amount, sensitivityMargins)
        {
            m_Risk = risk;
        }
        #endregion

        #region Methods (Static)
        public static MarginRisk Of(SensitivityRisk risk, Amount amount, List<MarginSensitivity> sensitivityMargins)
        {
            if (!Enum.IsDefined(typeof(SensitivityRisk), risk))
                throw new InvalidEnumArgumentException("Invalid sensitivity risk specified.");

            if (sensitivityMargins == null)
                throw new ArgumentNullException(nameof(sensitivityMargins));

            if (sensitivityMargins.Count == 0)
                throw new ArgumentException("No sensitivity margins have been provided.", nameof(sensitivityMargins));

            if (sensitivityMargins.Any(x => x == null))
                throw new ArgumentException("One or more sensitivity margins are null.", nameof(sensitivityMargins));

            List<MarginSensitivity> sensitivityMarginsSorted = sensitivityMargins
                .OrderBy(x => (Int32)x.Category)
                .ToList();

            return (new MarginRisk(risk, amount, sensitivityMarginsSorted));
        }
        #endregion
    }

    internal sealed class MarginSensitivity : Margin<MarginBucket>
    {
        #region Constants
        private const Int32 LEVEL = 5;
        private const String NAME = "Sensitivity";
        #endregion

        #region Members
        private readonly SensitivityCategory m_Category;
        #endregion

        #region Properties
        public SensitivityCategory Category => m_Category;
        #endregion

        #region Constructors
        private MarginSensitivity(SensitivityCategory category, Amount amount, List<MarginBucket> children) : base(LEVEL, NAME, category.ToString(), amount, children)
        {
            m_Category = category;
        }
        #endregion

        #region Methods (Static)
        public static MarginSensitivity Of(SensitivityCategory category, Amount amount, List<MarginBucket> bucketMargins)
        {
            if (!Enum.IsDefined(typeof(SensitivityCategory), category))
                throw new InvalidEnumArgumentException("Invalid sensitivity category specified.");

            if (bucketMargins == null)
                throw new ArgumentNullException(nameof(bucketMargins));

            if (bucketMargins.Count == 0)
                throw new ArgumentException("No bucket margins have been provided.", nameof(bucketMargins));

            if (bucketMargins.Any(x => x == null))
                throw new ArgumentException("One or more bucket margins are null.", nameof(bucketMargins));

            List<MarginBucket> bucketMarginsSorted = bucketMargins
                .OrderBy(x =>
                {
                    if (x.Bucket.IsResidual)
                        return Int32.MaxValue;

                    if (x.Bucket is Placeholder)
                        return Int32.MinValue;

                    return 0;
                })
                .ThenBy(x =>
                {
                    if (x.Bucket is Currency currency)
                        return currency.Name;

                    return x.Bucket.Description;
                })
                .ToList();

            return (new MarginSensitivity(category, amount, bucketMarginsSorted));
        }
        #endregion
    }

    internal sealed class MarginWeighting : Margin<IMargin>
    {
        #region Constants
        private const Int32 LEVEL = 7;
        private const String NAME = "Weighting";
        #endregion

        #region Members
        private readonly Sensitivity m_Sensitivity;
        #endregion

        #region Properties
        public Sensitivity Sensitivity => m_Sensitivity;
        #endregion

        #region Constructor
        private MarginWeighting(Sensitivity sensitivity, Amount amount) : base(LEVEL, NAME, CreateSensitivityIdentifier(sensitivity), amount)
        {
            m_Sensitivity = sensitivity;
        }
        #endregion

        #region Methods (Static)
        private static String CreateSensitivityIdentifier(Sensitivity sensitivity)
        {
            switch (sensitivity.Product)
            {
                case Product.Commodity:
                case Product.Equity:
                    return $"{sensitivity.Qualifier}";

                case Product.Credit:
                    return $"{sensitivity.Qualifier}{(String.IsNullOrWhiteSpace(sensitivity.Label1) ? String.Empty : $", {sensitivity.Label1}")}{(String.IsNullOrWhiteSpace(sensitivity.Label2) ? String.Empty : $", {sensitivity.Label2}")}";

                default:
                {
                    if (sensitivity.Risk == SensitivityRisk.Fx)
                    {
                        String qualifier = sensitivity.Qualifier;

                        if (qualifier.Length == 6)
                            return $"{qualifier.Substring(0,3)}/{qualifier.Substring(3,3)}";

                        return $"{sensitivity.Qualifier}";
                    }

                    if (String.IsNullOrWhiteSpace(sensitivity.Label1) && String.IsNullOrWhiteSpace(sensitivity.Label2))
                        return $"{sensitivity.Subrisk}";

                    return $"{sensitivity.Subrisk}{(String.IsNullOrWhiteSpace(sensitivity.Label1) ? String.Empty : $", {sensitivity.Label1}")}{(String.IsNullOrWhiteSpace(sensitivity.Label2) ? String.Empty : $", {sensitivity.Label2}")}";
                }
            }
        }

        public static MarginWeighting Of(Sensitivity sensitivity, Amount amount)
        {
            if (sensitivity == null)
                throw new ArgumentNullException(nameof(sensitivity));

            return (new MarginWeighting(sensitivity, amount));
        }
        #endregion
    }

    internal sealed class MarginAddOn : Margin<MarginAddOnComponent>
    {
        #region Constants
        private const Int32 LEVEL = 3;
        private const String IDENTIFIER = "Add-on";
        private const String NAME = "Add-on";
        #endregion

        #region Constructors
        private MarginAddOn(Amount amount, List<MarginAddOnComponent> children) : base(LEVEL, NAME, IDENTIFIER, amount, children) { }
        #endregion

        #region Methods (Static)
        public static MarginAddOn Of(Amount amount, List<MarginAddOnComponent> children)
        {
            if (children == null)
                throw new ArgumentNullException(nameof(children));

            if (children.Count == 0)
                throw new ArgumentException("No child margins have been provided.", nameof(children));

            if (children.Any(x => x == null))
                throw new ArgumentException("One or more child margins are null.", nameof(children));

            List<MarginAddOnComponent> childrenSorted = children
                .OrderBy(x => x.GetType().Name)
                .ThenBy(x => x.Identifier)
                .ToList();

            return (new MarginAddOn(amount, childrenSorted));
        }
        #endregion
    }

    internal abstract class MarginAddOnComponent : Margin<IMargin>
    {
        #region Constants
        private const Int32 LEVEL = 3;
        private const String NAME = "Add-on";
        #endregion

        #region Constructors
        protected MarginAddOnComponent(String identifier, Amount amount) : base(LEVEL, NAME, identifier, amount) { }
        #endregion
    }

    internal sealed class MarginAddOnFixedAmount : MarginAddOnComponent
    {
        #region Constants
        private const String IDENTIFIER = "Fixed";
        #endregion

        #region Constructors
        private MarginAddOnFixedAmount(Amount amount) : base(IDENTIFIER, amount) { }
        #endregion

        #region Methods (Static)
        public static MarginAddOnFixedAmount Of(Amount amount)
        {
            return (new MarginAddOnFixedAmount(amount));
        }
        #endregion
    }

    internal sealed class MarginAddOnNotional : MarginAddOnComponent
    {
        #region Members
        private readonly String m_Qualifier;
        #endregion

        #region Properties
        public String Bucket => m_Qualifier;
        #endregion

        #region Constructors
        private MarginAddOnNotional(String qualifier, Amount amount) : base($"Notional {qualifier}", amount)
        {
            m_Qualifier = qualifier;
        }
        #endregion

        #region Methods (Static)
        public static MarginAddOnNotional Of(String qualifier, Amount amount)
        {
            if (String.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            return (new MarginAddOnNotional(qualifier, amount));
        }
        #endregion
    }

    internal sealed class MarginAddOnProductMultiplier : MarginAddOnComponent
    {
        #region Members
        private readonly Product m_Product;
        #endregion

        #region Properties
        public Product Bucket => m_Product;
        #endregion

        #region Constructors
        private MarginAddOnProductMultiplier(Product product, Amount amount) : base($"Product {product}", amount)
        {
            m_Product = product;
        }
        #endregion

        #region Methods (Static)
        public static MarginAddOnProductMultiplier Of(Product product, Amount amount)
        {
            if (!Enum.IsDefined(typeof(Product), product))
                throw new InvalidEnumArgumentException("Invalid product specified.");

            return (new MarginAddOnProductMultiplier(product, amount));
        }
        #endregion
    }
}
