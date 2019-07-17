#region Using Directives
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Model
{
    /// <summary>Represents a sensitivity. This class cannot be derived.</summary>
    public sealed class Sensitivity : DataValue
    {
        #region Members
        private Currency m_Currency;
        private IBucket m_Bucket;
        private IThresholdIdentifier m_ThresholdIdentifier;
        private Product m_Product;
        private SensitivityCategory m_Category;
        private SensitivityRisk m_Risk;
        private SensitivitySubrisk m_Subrisk;
        private String m_Identifier;
        private String m_Label1;
        private String m_Label2;
        private String m_Qualifier;
        private Tenor m_Tenor;
        #endregion

        #region Properties
        /// <summary>Gets or sets the amount of the sensitivity.</summary>
        /// <value>A <see cref="T:InitialMargin.Core.Currency"/> object.</value>
        public Currency Currency
        {
            get => m_Currency;
            private set => m_Currency = value;
        }

        /// <summary>Gets or sets the bucket of the sensitivity.</summary>
        /// <value>An <see cref="T:InitialMargin.Core.IBucket"/> object.</value>
        public IBucket Bucket
        {
            get => m_Bucket;
            private set => m_Bucket = value;
        }

        /// <summary>Gets or sets the threshold identifier of the sensitivity.</summary>
        /// <value>An <see cref="T:InitialMargin.Core.IThresholdIdentifier"/> object.</value>
        public IThresholdIdentifier ThresholdIdentifier
        {
            get => m_ThresholdIdentifier;
            private set => m_ThresholdIdentifier = value;
        }

        /// <summary>Gets or sets the product of the sensitivity.</summary>
        /// <value>An enumerator value of type <see cref="T:InitialMargin.Model.Product"/>.</value>
        public Product Product
        {
            get => m_Product;
            private set => m_Product = value;
        }

        /// <summary>Gets or sets the sensitivity category.</summary>
        /// <value>An enumerator value of type <see cref="T:InitialMargin.Model.SensitivityCategory"/>.</value>
        public SensitivityCategory Category
        {
            get => m_Category;
            private set => m_Category = value;
        }

        /// <summary>Gets or sets the sensitivity risk.</summary>
        /// <value>An enumerator value of type <see cref="T:InitialMargin.Model.SensitivityRisk"/>.</value>
        public SensitivityRisk Risk
        {
            get => m_Risk;
            private set => m_Risk = value;
        }

        /// <summary>Gets or sets the sensitivity sub-risk.</summary>
        /// <value>An enumerator value of type <see cref="T:InitialMargin.Model.SensitivitySubrisk"/>.</value>
        public SensitivitySubrisk Subrisk
        {
            get => m_Subrisk;
            private set => m_Subrisk = value;
        }

        /// <summary>Gets or sets the sensitivity identifier.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Identifier
        {
            get => m_Identifier;
            private set => m_Identifier = value;
        }

        /// <summary>Gets or sets the first label of the sensitivity.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Label1
        {
            get => m_Label1;
            private set => m_Label1 = value;
        }

        /// <summary>Gets or sets the second label of the sensitivity.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Label2
        {
            get => m_Label2;
            private set => m_Label2 = value;
        }

        /// <summary>Gets or sets the qualifier of the sensitivity.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Qualifier
        {
            get => m_Qualifier;
            private set => m_Qualifier = value;
        }

        /// <summary>Gets or sets the tenor of the sensitivity.</summary>
        /// <value>A <see cref="T:InitialMargin.Model.Tenor"/> object.</value>
        public Tenor Tenor
        {
            get => m_Tenor;
            private set => m_Tenor = value;
        }
        #endregion

        #region Constructors
        private Sensitivity(Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo) : base(amount, regulationsInfo, tradeInfo) { }
        #endregion

        #region Methods
        /// <summary>Converts the current instance to a curvature sensitivity.</summary>
        /// <param name="curvatureAmount">The <see cref="T:InitialMargin.Core.Amount"/> of the curvature sensitivity.</param>
        /// <returns>A <see cref="T:InitialMargin.Model.Sensitivity"/> object with the same properties of the current instance and an amount equal to the curvature amount.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the current instance is not a vega sensitivity.</exception>
        public Sensitivity ToCurvature(Amount curvatureAmount)
        {
            if (m_Category != SensitivityCategory.Vega)
                throw new InvalidOperationException("Only vega sensitivities can be converted to curvature sensitivities.");

            Sensitivity clone = (Sensitivity)MemberwiseClone();
            clone.Amount = curvatureAmount;
            clone.Category = SensitivityCategory.Curvature;

            return clone;
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            String reference;

            if (m_Category == SensitivityCategory.BaseCorrelation)
                reference = "BaseCorrelation";
            else
            {
                if (m_Subrisk != SensitivitySubrisk.None)
                    reference = (m_Subrisk == SensitivitySubrisk.CrossCurrencyBasis) ? "CrossCurrencyBasis" : $"{m_Subrisk} {m_Category}";
                else
                    reference = $"{m_Risk} {m_Category}";
            }
            
            StringBuilder builderInfo = new StringBuilder($"Qualifier=\"{Qualifier}\"");

            if ((Bucket != null) && (Bucket.GetType() != typeof(Placeholder)) && !String.IsNullOrWhiteSpace(Bucket.Name))
                builderInfo.Append($" Bucket=\"{Bucket}\"");

            if (!String.IsNullOrEmpty(Label1))
                builderInfo.Append($" Label1=\"{Label1}\"");

            if (!String.IsNullOrEmpty(Label2))
                builderInfo.Append($" Label2=\"{Label2}\"");

            return $"Sensitivity {reference} {builderInfo} Amount=\"{Amount}\"";
        }
        #endregion

        #region Methods (Static)
        internal static Sensitivity BaseCorrelation(String qualifier, Amount amount)
        {
            if (String.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            return (new Sensitivity(amount, RegulationsInfo.Empty, TradeInfo.Empty)
            {
                Currency = amount.Currency,
                Bucket = Placeholder.Instance,
                ThresholdIdentifier = Placeholder.Instance,
                Product = Product.Credit,
                Category = SensitivityCategory.BaseCorrelation,
                Risk = SensitivityRisk.CreditQualifying,
                Subrisk = SensitivitySubrisk.None,
                Identifier = String.Empty,
                Label1 = String.Empty,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = null
            });
        }

        internal static Sensitivity Commodity(SensitivityCategory category, String qualifier, BucketCommodity bucket, Amount amount)
        {
            if (!Enum.IsDefined(typeof(SensitivityCategory), category) || (category == SensitivityCategory.BaseCorrelation))
                throw new ArgumentException("Invalid sensitivity category specified.", nameof(category));

            if (String.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            return (new Sensitivity(amount, RegulationsInfo.Empty, TradeInfo.Empty)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Commodity,
                Category = category,
                Risk = SensitivityRisk.Commodity,
                Subrisk = SensitivitySubrisk.None,
                Identifier = String.Empty,
                Label1 = String.Empty,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = null
            });
        }

        internal static Sensitivity CreditNonQualifying(SensitivityCategory category, String qualifier, BucketCreditNonQualifying bucket, Tenor tenor, Amount amount)
        {
            if (!Enum.IsDefined(typeof(SensitivityCategory), category) || (category == SensitivityCategory.BaseCorrelation))
                throw new ArgumentException("Invalid sensitivity category specified.", nameof(category));

            if (String.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (!tenor.IsCreditTenor)
                throw new ArgumentException($"Invalid tenor specified: accepted tenors are {String.Join(", ", Tenor.Values.Where(x => x.IsCreditTenor).Select(x => x.Name))}.", nameof(tenor));

            return (new Sensitivity(amount, RegulationsInfo.Empty, TradeInfo.Empty)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Credit,
                Category = category,
                Risk = SensitivityRisk.CreditNonQualifying,
                Subrisk = SensitivitySubrisk.None,
                Identifier = String.Empty,
                Label1 = tenor.Name,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = tenor
            });
        }

        internal static Sensitivity CreditQualifying(SensitivityCategory category, String qualifier, BucketCreditQualifying bucket, Tenor tenor, String label2, Amount amount)
        {
            if (!Enum.IsDefined(typeof(SensitivityCategory), category) || (category == SensitivityCategory.BaseCorrelation))
                throw new ArgumentException("Invalid sensitivity category specified.", nameof(category));

            if (String.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (!tenor.IsCreditTenor)
                throw new ArgumentException($"Invalid tenor specified: accepted tenors are {String.Join(", ", Tenor.Values.Where(x => x.IsCreditTenor).Select(x => x.Name))}.", nameof(tenor));

            if (label2 == null)
                throw new ArgumentNullException(nameof(label2));

            if ((label2.Length != 0) && (label2 != "Sec"))
                throw new ArgumentException("Invalid label specified.", nameof(label2));

            return (new Sensitivity(amount, RegulationsInfo.Empty, TradeInfo.Empty)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Credit,
                Category = category,
                Risk = SensitivityRisk.CreditQualifying,
                Subrisk = SensitivitySubrisk.None,
                Identifier = String.Empty,
                Label1 = tenor.Name,
                Label2 = label2,
                Qualifier = qualifier,
                Tenor = tenor
            });
        }

        internal static Sensitivity CrossCurrencyBasis(Currency currency, Amount amount)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            return (new Sensitivity(amount, RegulationsInfo.Empty, TradeInfo.Empty)
            {
                Currency = currency,
                Bucket = currency,
                ThresholdIdentifier = Placeholder.Instance,
                Product = Product.RatesFx,
                Category = SensitivityCategory.Delta,
                Risk = SensitivityRisk.Rates,
                Subrisk = SensitivitySubrisk.CrossCurrencyBasis,
                Identifier = String.Empty,
                Label1 = String.Empty,
                Label2 = String.Empty,
                Qualifier = currency.Name,
                Tenor = null
            });
        }

        internal static Sensitivity Equity(SensitivityCategory category, String qualifier, BucketEquity bucket, Amount amount)
        {
            if (!Enum.IsDefined(typeof(SensitivityCategory), category) || (category == SensitivityCategory.BaseCorrelation))
                throw new ArgumentException("Invalid sensitivity category specified.", nameof(category));

            if (String.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            return (new Sensitivity(amount, RegulationsInfo.Empty, TradeInfo.Empty)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Equity,
                Category = category,
                Risk = SensitivityRisk.Equity,
                Subrisk = SensitivitySubrisk.None,
                Identifier = String.Empty,
                Label1 = String.Empty,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = null
            });
        }

        internal static Sensitivity Fx(SensitivityCategory category, IThresholdIdentifier thresholdIdentifier, Amount amount)
        {
            if (!Enum.IsDefined(typeof(SensitivityCategory), category) || (category == SensitivityCategory.BaseCorrelation))
                throw new ArgumentException("Invalid sensitivity category specified.", nameof(category));

            if (thresholdIdentifier == null)
                throw new ArgumentNullException(nameof(thresholdIdentifier));

            switch (thresholdIdentifier)
            {
                case Currency currency:
                {
                    return (new Sensitivity(amount, RegulationsInfo.Empty, TradeInfo.Empty)
                    {
                        Currency = currency,
                        Bucket = Placeholder.Instance,
                        ThresholdIdentifier = currency,
                        Product = Product.RatesFx,
                        Category = SensitivityCategory.Delta,
                        Risk = SensitivityRisk.Fx,
                        Subrisk = SensitivitySubrisk.None,
                        Identifier = String.Empty,
                        Label1 = String.Empty,
                        Label2 = String.Empty,
                        Qualifier = currency.Name,
                        Tenor = null
                    });
                }

                case CurrencyPair currencyPair:
                {
                    return (new Sensitivity(amount, RegulationsInfo.Empty, TradeInfo.Empty)
                    {
                        Currency = amount.Currency,
                        Bucket = Placeholder.Instance,
                        ThresholdIdentifier = currencyPair,
                        Product = Product.RatesFx,
                        Category = category,
                        Risk = SensitivityRisk.Fx,
                        Subrisk = SensitivitySubrisk.None,
                        Identifier = String.Empty,
                        Label1 = String.Empty,
                        Label2 = String.Empty,
                        Qualifier = currencyPair.ToString(),
                        Tenor = null
                    });
                }

                default:
                    throw new ArgumentException("Invalid threshold identifier specified.", nameof(thresholdIdentifier));
            }
        }

        internal static Sensitivity Inflation(SensitivityCategory category, Currency currency, Amount amount)
        {
            if (!Enum.IsDefined(typeof(SensitivityCategory), category) || (category == SensitivityCategory.BaseCorrelation))
                throw new ArgumentException("Invalid sensitivity category specified.", nameof(category));

            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            return (new Sensitivity(amount, RegulationsInfo.Empty, TradeInfo.Empty)
            {
                Currency = currency,
                Bucket = currency,
                ThresholdIdentifier = currency,
                Product = Product.RatesFx,
                Category = category,
                Risk = SensitivityRisk.Rates,
                Subrisk = SensitivitySubrisk.Inflation,
                Identifier = String.Empty,
                Label1 = String.Empty,
                Label2 = String.Empty,
                Qualifier = currency.Name,
                Tenor = null
            });
        }

        internal static Sensitivity InterestRate(SensitivityCategory category, Currency currency, Tenor tenor, String label2, Amount amount)
        {
            if (!Enum.IsDefined(typeof(SensitivityCategory), category) || (category == SensitivityCategory.BaseCorrelation))
                throw new ArgumentException("Invalid sensitivity category specified.", nameof(category));

            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (label2 == null)
                throw new ArgumentNullException(nameof(label2));

            if ((label2.Length != 0) && !Enum.TryParse(DataValidator.FormatLibor(label2, true), out Curve _))
                throw new ArgumentException("Invalid label specified.", nameof(label2));

            return (new Sensitivity(amount, RegulationsInfo.Empty, TradeInfo.Empty)
            {
                Currency = currency,
                Bucket = currency,
                ThresholdIdentifier = currency,
                Product = Product.RatesFx,
                Category = category,
                Risk = SensitivityRisk.Rates,
                Subrisk = SensitivitySubrisk.InterestRate,
                Identifier = String.Empty,
                Label1 = tenor.Name,
                Label2 = label2,
                Qualifier = currency.Name,
                Tenor = tenor
            });
        }

        /// <summary>Initializes and returns a new base correlation sensitivity using the specified parameters.</summary>
        /// <param name="qualifier">The <see cref="T:System.String"/> representing the qualifier of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new base correlation <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="qualifier">qualifier</paramref> is invalid.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity BaseCorrelation(String qualifier, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (String.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = amount.Currency,
                Bucket = Placeholder.Instance,
                ThresholdIdentifier = Placeholder.Instance,
                Product = Product.Credit,
                Category = SensitivityCategory.BaseCorrelation,
                Risk = SensitivityRisk.CreditQualifying,
                Subrisk = SensitivitySubrisk.None,
                Identifier = "Risk_BaseCorr",
                Label1 = String.Empty,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = null
            });
        }

        /// <summary>Initializes and returns a new commodity delta sensitivity using the specified parameters.</summary>
        /// <param name="qualifier">The <see cref="T:System.String"/> representing the qualifier of the sensitivity.</param>
        /// <param name="bucket">The <see cref="T:InitialMargin.Model.BucketCommodity"/> object representing the bucket of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new commodity delta <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="qualifier">qualifier</paramref> is invalid.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="bucket">bucket</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity CommodityDelta(String qualifier, BucketCommodity bucket, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (String.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Commodity,
                Category = SensitivityCategory.Delta,
                Risk = SensitivityRisk.Commodity,
                Subrisk = SensitivitySubrisk.None,
                Identifier = "Risk_Commodity",
                Label1 = String.Empty,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = null
            });
        }

        /// <summary>Initializes and returns a new commodity vega sensitivity using the specified parameters.</summary>
        /// <param name="qualifier">The <see cref="T:System.String"/> representing the qualifier of the sensitivity.</param>
        /// <param name="bucket">The <see cref="T:InitialMargin.Model.BucketCommodity"/> object representing the bucket of the sensitivity.</param>
        /// <param name="tenor">The <see cref="T:InitialMargin.Model.Tenor"/> object representing the tenor of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new commodity vega <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="qualifier">qualifier</paramref> is invalid.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="bucket">bucket</paramref> is <c>null</c>, when <paramref name="tenor">tenor</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity CommodityVega(String qualifier, BucketCommodity bucket, Tenor tenor, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (String.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Commodity,
                Category = SensitivityCategory.Vega,
                Risk = SensitivityRisk.Commodity,
                Subrisk = SensitivitySubrisk.None,
                Identifier = "Risk_CommodityVol",
                Label1 = tenor.Name,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = tenor
            });
        }

        /// <summary>Initializes and returns a new non-qualifying credit delta sensitivity using the specified parameters.</summary>
        /// <param name="qualifier">The <see cref="T:System.String"/> representing the qualifier of the sensitivity.</param>
        /// <param name="bucket">The <see cref="T:InitialMargin.Model.BucketCreditNonQualifying"/> object representing the bucket of the sensitivity.</param>
        /// <param name="tenor">The <see cref="T:InitialMargin.Model.Tenor"/> object representing the tenor of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new non-qualifying credit delta <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="qualifier">qualifier</paramref> is invalid (see <see cref="M:InitialMargin.Core.DataValidator.IsValidQualifier(System.String,System.Boolean)"/>) or when <paramref name="tenor">tenor</paramref> is inappropriate (see <see cref="P:InitialMargin.Model.Tenor.IsCreditTenor"/>).</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="bucket">bucket</paramref> is <c>null</c>, when <paramref name="tenor">tenor</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity CreditNonQualifyingDelta(String qualifier, BucketCreditNonQualifying bucket, Tenor tenor, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (!DataValidator.IsValidQualifier(qualifier, true))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (!tenor.IsCreditTenor)
                throw new ArgumentException($"Invalid tenor specified: accepted tenors are {String.Join(", ", Tenor.Values.Where(x => x.IsCreditTenor).Select(x => x.Name))}.", nameof(tenor));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Credit,
                Category = SensitivityCategory.Delta,
                Risk = SensitivityRisk.CreditNonQualifying,
                Subrisk = SensitivitySubrisk.None,
                Identifier = "Risk_CreditNonQ",
                Label1 = tenor.Name,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = tenor
            });
        }

        /// <summary>Initializes and returns a new non-qualifying credit vega sensitivity using the specified parameters.</summary>
        /// <param name="qualifier">The <see cref="T:System.String"/> representing the qualifier of the sensitivity.</param>
        /// <param name="bucket">The <see cref="T:InitialMargin.Model.BucketCreditNonQualifying"/> object representing the bucket of the sensitivity.</param>
        /// <param name="tenor">The <see cref="T:InitialMargin.Model.Tenor"/> object representing the tenor of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new non-qualifying credit vega <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="qualifier">qualifier</paramref> is invalid or when <paramref name="tenor">tenor</paramref> is inappropriate (see <see cref="P:InitialMargin.Model.Tenor.IsCreditTenor"/>).</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="bucket">bucket</paramref> is <c>null</c>, when <paramref name="tenor">tenor</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity CreditNonQualifyingVega(String qualifier, BucketCreditNonQualifying bucket, Tenor tenor, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (String.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (!tenor.IsCreditTenor)
                throw new ArgumentException($"Invalid tenor specified: accepted tenors are {String.Join(", ", Tenor.Values.Where(x => x.IsCreditTenor).Select(x => x.Name))}.", nameof(tenor));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Credit,
                Category = SensitivityCategory.Vega,
                Risk = SensitivityRisk.CreditNonQualifying,
                Subrisk = SensitivitySubrisk.None,
                Identifier = "Risk_CreditVolNonQ",
                Label1 = tenor.Name,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = tenor
            });
        }

        /// <summary>Initializes and returns a new qualifying credit delta sensitivity using the specified parameters.</summary>
        /// <param name="qualifier">The <see cref="T:System.String"/> representing the qualifier of the sensitivity.</param>
        /// <param name="bucket">The <see cref="T:InitialMargin.Model.BucketCreditNonQualifying"/> object representing the bucket of the sensitivity.</param>
        /// <param name="tenor">The <see cref="T:InitialMargin.Model.Tenor"/> object representing the tenor of the sensitivity.</param>
        /// <param name="securitization"><c>true</c> if the risk arises from a qualifying securitization; otherwise, <c>false</c>.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new qualifying credit delta <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="qualifier">qualifier</paramref> is invalid (see <see cref="M:InitialMargin.Core.DataValidator.IsValidQualifier(System.String,System.Boolean)"/>) or when <paramref name="tenor">tenor</paramref> is inappropriate (see <see cref="P:InitialMargin.Model.Tenor.IsCreditTenor"/>).</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="bucket">bucket</paramref> is <c>null</c>, when <paramref name="tenor">tenor</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity CreditQualifyingDelta(String qualifier, BucketCreditQualifying bucket, Tenor tenor, Boolean securitization, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (!DataValidator.IsValidQualifier(qualifier, true))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (!tenor.IsCreditTenor)
                throw new ArgumentException($"Invalid tenor specified: accepted tenors are {String.Join(", ", Tenor.Values.Where(x => x.IsCreditTenor).Select(x => x.Name))}.", nameof(tenor));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Credit,
                Category = SensitivityCategory.Delta,
                Risk = SensitivityRisk.CreditQualifying,
                Subrisk = SensitivitySubrisk.None,
                Identifier = "Risk_CreditQ",
                Label1 = tenor.Name,
                Label2 = securitization ? "Sec" : String.Empty,
                Qualifier = qualifier,
                Tenor = tenor
            });
        }

        /// <summary>Initializes and returns a new qualifying credit vega sensitivity using the specified parameters.</summary>
        /// <param name="qualifier">The <see cref="T:System.String"/> representing the qualifier of the sensitivity.</param>
        /// <param name="bucket">The <see cref="T:InitialMargin.Model.BucketCreditNonQualifying"/> object representing the bucket of the sensitivity.</param>
        /// <param name="tenor">The <see cref="T:InitialMargin.Model.Tenor"/> object representing the tenor of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new qualifying credit vega <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="qualifier">qualifier</paramref> is invalid or when <paramref name="tenor">tenor</paramref> is inappropriate (see <see cref="P:InitialMargin.Model.Tenor.IsCreditTenor"/>).</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="bucket">bucket</paramref> is <c>null</c>, when <paramref name="tenor">tenor</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity CreditQualifyingVega(String qualifier, BucketCreditQualifying bucket, Tenor tenor, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (String.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (!tenor.IsCreditTenor)
                throw new ArgumentException($"Invalid tenor specified: accepted tenors are {String.Join(", ", Tenor.Values.Where(x => x.IsCreditTenor).Select(x => x.Name))}.", nameof(tenor));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Credit,
                Category = SensitivityCategory.Vega,
                Risk = SensitivityRisk.CreditQualifying,
                Subrisk = SensitivitySubrisk.None,
                Identifier = "Risk_CreditVol",
                Label1 = tenor.Name,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = tenor
            });
        }

        /// <summary>Initializes and returns a new cross-currency basis sensitivity using the specified parameters.</summary>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the reference currency of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new cross-currency basis <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currency">currency</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity CrossCurrencyBasis(Currency currency, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = currency,
                Bucket = currency,
                ThresholdIdentifier = Placeholder.Instance,
                Product = Product.RatesFx,
                Category = SensitivityCategory.Delta,
                Risk = SensitivityRisk.Rates,
                Subrisk = SensitivitySubrisk.CrossCurrencyBasis,
                Identifier = "Risk_XCcyBasis",
                Label1 = String.Empty,
                Label2 = String.Empty,
                Qualifier = currency.Name,
                Tenor = null
            });
        }

        /// <summary>Initializes and returns a new equity delta sensitivity using the specified parameters.</summary>
        /// <param name="qualifier">The <see cref="T:System.String"/> representing the qualifier of the sensitivity.</param>
        /// <param name="bucket">The <see cref="T:InitialMargin.Model.BucketEquity"/> object representing the bucket of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new equity delta <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="qualifier">qualifier</paramref> is invalid (see <see cref="M:InitialMargin.Core.DataValidator.IsValidQualifier(System.String,System.Boolean)"/>).</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="bucket">bucket</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity EquityDelta(String qualifier, BucketEquity bucket, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (!DataValidator.IsValidQualifier(qualifier, false))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Equity,
                Category = SensitivityCategory.Delta,
                Risk = SensitivityRisk.Equity,
                Subrisk = SensitivitySubrisk.None,
                Identifier = "Risk_Equity",
                Label1 = String.Empty,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = null
            });
        }

        /// <summary>Initializes and returns a new equity vega sensitivity using the specified parameters.</summary>
        /// <param name="qualifier">The <see cref="T:System.String"/> representing the qualifier of the sensitivity.</param>
        /// <param name="bucket">The <see cref="T:InitialMargin.Model.BucketCommodity"/> object representing the bucket of the sensitivity.</param>
        /// <param name="tenor">The <see cref="T:InitialMargin.Model.Tenor"/> object representing the tenor of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new equity vega <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="qualifier">qualifier</paramref> is invalid (see <see cref="M:InitialMargin.Core.DataValidator.IsValidQualifier(System.String,System.Boolean)"/>).</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="bucket">bucket</paramref> is <c>null</c>, when <paramref name="tenor">tenor</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity EquityVega(String qualifier, BucketEquity bucket, Tenor tenor, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (!DataValidator.IsValidQualifier(qualifier, false))
                throw new ArgumentException("Invalid qualifier specified.", nameof(qualifier));

            if (bucket == null)
                throw new ArgumentNullException(nameof(bucket));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = amount.Currency,
                Bucket = bucket,
                ThresholdIdentifier = bucket,
                Product = Product.Equity,
                Category = SensitivityCategory.Vega,
                Risk = SensitivityRisk.Equity,
                Subrisk = SensitivitySubrisk.None,
                Identifier = "Risk_EquityVol",
                Label1 = tenor.Name,
                Label2 = String.Empty,
                Qualifier = qualifier,
                Tenor = tenor
            });
        }

        /// <summary>Initializes and returns a new foreign exchange delta sensitivity using the specified parameters.</summary>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the reference currency of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new foreign exchange delta <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currency">currency</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity FxDelta(Currency currency, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = currency,
                Bucket = Placeholder.Instance,
                ThresholdIdentifier = currency,
                Product = Product.RatesFx,
                Category = SensitivityCategory.Delta,
                Risk = SensitivityRisk.Fx,
                Subrisk = SensitivitySubrisk.None,
                Identifier = "Risk_FX",
                Label1 = String.Empty,
                Label2 = String.Empty,
                Qualifier = currency.Name,
                Tenor = null
            });
        }

        /// <summary>Initializes and returns a new foreign exchange vega sensitivity using the specified parameters.</summary>
        /// <param name="currencyPair">The <see cref="T:InitialMargin.Core.CurrencyPair"/> object representing the reference currency pair of the sensitivity.</param>
        /// <param name="tenor">The <see cref="T:InitialMargin.Model.Tenor"/> object representing the tenor of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new foreign exchange vega <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currencyPair">currencyPair</paramref> is <c>null</c>, when <paramref name="tenor">tenor</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity FxVega(CurrencyPair currencyPair, Tenor tenor, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (currencyPair == null)
                throw new ArgumentNullException(nameof(currencyPair));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = amount.Currency,
                Bucket = Placeholder.Instance,
                ThresholdIdentifier = currencyPair.Sort(),
                Product = Product.RatesFx,
                Category = SensitivityCategory.Vega,
                Risk = SensitivityRisk.Fx,
                Subrisk = SensitivitySubrisk.None,
                Identifier = "Risk_FXVol",
                Label1 = tenor.Name,
                Label2 = String.Empty,
                Qualifier = currencyPair.ToString(false),
                Tenor = tenor
            });
        }

        /// <summary>Initializes and returns a new inflation delta sensitivity using the specified parameters.</summary>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the reference currency of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new inflation delta <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currency">currency</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity InflationDelta(Currency currency, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = currency,
                Bucket = currency,
                ThresholdIdentifier = currency,
                Product = Product.RatesFx,
                Category = SensitivityCategory.Delta,
                Risk = SensitivityRisk.Rates,
                Subrisk = SensitivitySubrisk.Inflation,
                Identifier = "Risk_Inflation",
                Label1 = String.Empty,
                Label2 = String.Empty,
                Qualifier = currency.Name,
                Tenor = null
            });
        }

        /// <summary>Initializes and returns a new inflation vega sensitivity using the specified parameters.</summary>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the reference currency of the sensitivity.</param>
        /// <param name="tenor">The <see cref="T:InitialMargin.Model.Tenor"/> object representing the tenor of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new inflation vega <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currency">currency</paramref> is <c>null</c>, when <paramref name="tenor">tenor</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity InflationVega(Currency currency, Tenor tenor, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = currency,
                Bucket = currency,
                ThresholdIdentifier = currency,
                Product = Product.RatesFx,
                Category = SensitivityCategory.Vega,
                Risk = SensitivityRisk.Rates,
                Subrisk = SensitivitySubrisk.Inflation,
                Identifier = "Risk_InflationVol",
                Label1 = tenor.Name,
                Label2 = String.Empty,
                Qualifier = currency.Name,
                Tenor = tenor
            });
        }

        /// <summary>Initializes and returns a new interest rate delta sensitivity using the specified parameters.</summary>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the reference currency of the sensitivity.</param>
        /// <param name="tenor">The <see cref="T:InitialMargin.Model.Tenor"/> object representing the tenor of the sensitivity.</param>
        /// <param name="curve">The enumerator value of type <see cref="T:InitialMargin.Model.Curve"/> representing the reference curve of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new interest rate delta <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="curve">curve</paramref> is <see cref="F:InitialMargin.Model.Curve.Municipal"/> or <see cref="F:InitialMargin.Model.Curve.Prime"/> and <paramref name="currency">currency</paramref> is not <see cref="F:InitialMargin.Core.Currency.Usd"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currency">currency</paramref> is <c>null</c>, when <paramref name="tenor">tenor</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="curve">curve</paramref> is undefined.</exception>
        public static Sensitivity InterestRateDelta(Currency currency, Tenor tenor, Curve curve, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (!Enum.IsDefined(typeof(Curve), curve))
                throw new InvalidEnumArgumentException("Invalid curve specified.");

            if ((currency != Currency.Usd) && ((curve == Curve.Municipal) || (curve == Curve.Prime)))
                throw new ArgumentException("Invalid curve specified: Municipal and Prime curves can be associated only to USD currency.", nameof(curve));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            String curveName = curve.ToString();

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = currency,
                Bucket = currency,
                ThresholdIdentifier = currency,
                Product = Product.RatesFx,
                Category = SensitivityCategory.Delta,
                Risk = SensitivityRisk.Rates,
                Subrisk = SensitivitySubrisk.InterestRate,
                Identifier = "Risk_IRCurve",
                Label1 = tenor.Name,
                Label2 = String.Concat(curveName.Substring(0, 1), curveName.Substring(1).ToLowerInvariant()),
                Qualifier = currency.Name,
                Tenor = tenor
            });
        }

        /// <summary>Initializes and returns a new interest rate vega sensitivity using the specified parameters.</summary>
        /// <param name="currency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the reference currency of the sensitivity.</param>
        /// <param name="tenor">The <see cref="T:InitialMargin.Model.Tenor"/> object representing the tenor of the sensitivity.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> of the sensitivity.</param>
        /// <param name="regulationsInfo">The <see cref="T:InitialMargin.Core.RegulationsInfo"/> defining collect and post regulations of the sensitivity.</param>
        /// <param name="tradeInfo">The <see cref="T:InitialMargin.Core.TradeInfo"/> defining the underlying trade of the sensitivity.</param>
        /// <returns>A new interest rate vega <see cref="T:InitialMargin.Model.Sensitivity"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="currency">currency</paramref> is <c>null</c>, when <paramref name="tenor">tenor</paramref> is <c>null</c>, when <paramref name="regulationsInfo">regulationsInfo</paramref> is <c>null</c> or when <paramref name="tradeInfo">tradeInfo</paramref> is <c>null</c>.</exception>
        public static Sensitivity InterestRateVega(Currency currency, Tenor tenor, Amount amount, RegulationsInfo regulationsInfo, TradeInfo tradeInfo)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            if (tenor == null)
                throw new ArgumentNullException(nameof(tenor));

            if (regulationsInfo == null)
                throw new ArgumentNullException(nameof(regulationsInfo));

            if (tradeInfo == null)
                throw new ArgumentNullException(nameof(tradeInfo));

            return (new Sensitivity(amount, regulationsInfo, tradeInfo)
            {
                Currency = currency,
                Bucket = currency,
                ThresholdIdentifier = currency,
                Product = Product.RatesFx,
                Category = SensitivityCategory.Vega,
                Subrisk = SensitivitySubrisk.InterestRate,
                Risk = SensitivityRisk.Rates,
                Identifier = "Risk_IRVol",
                Label1 = tenor.Name,
                Label2 = String.Empty,
                Qualifier = currency.Name,
                Tenor = tenor
            });
        }
        #endregion
    }
}