#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Model
{
    internal static class ModelCalculations
    {
        #region Members
        private static readonly Dictionary<SensitivityCategory,MarginCalculator> s_MarginCalculators = new Dictionary<SensitivityCategory,MarginCalculator>
        {
            [SensitivityCategory.BaseCorrelation] = new MarginCalculatorBaseCorrelation(),
            [SensitivityCategory.Curvature] = new MarginCalculatorCurvature(),
            [SensitivityCategory.Delta] = new MarginCalculatorOther(),
            [SensitivityCategory.Vega] = new MarginCalculatorOther()
        };
        #endregion

        #region Methods
        private static Amount CalculateCorrelatedSumRisks(Currency calculationCurrency, List<MarginRisk> riskMargins)
        {
            Amount sum = Amount.OfZero(calculationCurrency);

            foreach (MarginRisk marginRisk1 in riskMargins)
            {
                foreach (MarginRisk marginRisk2 in riskMargins)
                {
                    SensitivityRisk risk1 = marginRisk1.Risk;
                    SensitivityRisk risk2 = marginRisk2.Risk;

                    if (risk1 != risk2)
                    {
                        Decimal correlation = ModelParameters.GetCorrelationRisk(risk1, risk2);
                        sum += marginRisk1.Value * marginRisk2.Value * correlation;
                    }
                }
            }

            return sum;
        }

        private static Decimal CalculateThresholdFactor(Currency calculationCurrency, FxRatesProvider ratesProvider, SensitivityRisk risk, SensitivityCategory category, List<Sensitivity> sensitivities)
        {
            if ((risk == SensitivityRisk.Rates) && (category == SensitivityCategory.Delta))
            {
                sensitivities = sensitivities.Where(x => x.Subrisk != SensitivitySubrisk.CrossCurrencyBasis).ToList();
                
                if (sensitivities.Count == 0)
                    return 1m;
            }

            Amount sum = Amount.Abs(Amount.Sum(sensitivities.Select(x => x.Amount), calculationCurrency));

            IThresholdIdentifier thresholdIdentifier = sensitivities.Select(y => y.ThresholdIdentifier).Distinct().Single();
            Amount threshold = ratesProvider.Convert(ModelParameters.GetThreshold(risk, category, thresholdIdentifier), calculationCurrency);

            return Math.Max(1m, MathUtilities.SquareRoot((sum / threshold).Value));
        }

        private static List<MarginAddOnComponent> CalculateMarginsAddOnNotional(Currency calculationCurrency, List<AddOnNotional> notionals, List<AddOnNotionalFactor> notionalFactors)
        {
            if ((notionals.Count == 0) || (notionalFactors.Count == 0))
                return (new List<MarginAddOnComponent>(0));

            List <MarginAddOnComponent> margins = new List<MarginAddOnComponent>();

            Dictionary<String,Decimal> notionalFactorsMap = notionalFactors
                .GroupBy(x => x.Qualifier)
                .ToDictionary(x => x.Key, x => x.Single().Parameter);

            Dictionary<String,Amount> notionalsMap = notionals
                .Where(x => notionalFactorsMap.ContainsKey(x.Qualifier))
                .GroupBy(x => x.Qualifier)
                .ToDictionary(x => x.Key, x => Amount.Sum(x.Select(n => Amount.Abs(n.Amount)), calculationCurrency));

            foreach (KeyValuePair<String,Decimal> entry in notionalFactorsMap)
            {
                if (!notionalsMap.ContainsKey(entry.Key))
                    continue;

                Amount notionalAmount = notionalsMap[entry.Key] * entry.Value;
                margins.Add(MarginAddOnNotional.Of(entry.Key, notionalAmount));
            }

            return margins;
        }

        private static List<MarginAddOnComponent> CalculateMarginsAddOnProduct(List<MarginProduct> productMargins, List<AddOnProductMultiplier> productMultipliers)
        {
            if ((productMargins.Count == 0) || (productMultipliers.Count == 0))
                return (new List<MarginAddOnComponent>(0));

            List <MarginAddOnComponent> margins = new List<MarginAddOnComponent>();

            Dictionary<Product,Decimal> productMultipliersMap = productMultipliers
                .GroupBy(x => x.Product)
                .ToDictionary(x => x.Key, x => x.Single().Parameter);

            Dictionary<Product,Amount> productMarginsMap = productMargins
                .Where(x => productMultipliersMap.ContainsKey(x.Product))
                .GroupBy(x => x.Product)
                .ToDictionary(x => x.Key, x => x.Single().Value);

            foreach (KeyValuePair<Product,Decimal> entry in productMultipliersMap)
            {
                if (!productMarginsMap.ContainsKey(entry.Key))
                    continue;

                Amount productMultiplierAmount = productMarginsMap[entry.Key] * entry.Value;
                margins.Add(MarginAddOnProductMultiplier.Of(entry.Key, productMultiplierAmount));
            }

            return margins;
        }

        private static List<MarginRisk> CalculateRiskMargins(Currency calculationCurrency, FxRatesProvider ratesProvider, List<Sensitivity> sensitivities)
        {
            List<SensitivityRisk> risks = sensitivities.Select(x => x.Risk).Distinct().OrderBy(x => x.ToString()).ToList();
            List<MarginRisk> riskMargins = new List<MarginRisk>(risks.Count);

            foreach (SensitivityRisk risk in risks)
            {
                List<Sensitivity> sensitivitiesByRisk = sensitivities.Where(x => x.Risk == risk).ToList();
                
                List<SensitivityCategory> sensitivityCategories = sensitivitiesByRisk.Select(x => x.Category).Distinct().OrderBy(x => x.ToString()).ToList();
                List<MarginSensitivity> sensitivityMargins = new List<MarginSensitivity>(sensitivityCategories.Count);

                foreach (SensitivityCategory category in sensitivityCategories)
                {
                    List<Sensitivity> sensitivitiesByClass = sensitivitiesByRisk.Where(x => x.Category == category).ToList();
                    MarginSensitivity sensitivityMargin = s_MarginCalculators[category].CalculateMargin(calculationCurrency, ratesProvider, risk, category, sensitivitiesByClass);

                    sensitivityMargins.Add(sensitivityMargin);
                }

                Amount riskMarginAmount = Amount.Sum(sensitivityMargins.Select(x => x.Value), calculationCurrency);

                riskMargins.Add(MarginRisk.Of(risk, riskMarginAmount, sensitivityMargins));
            }

            return riskMargins;
        }

        public static Amount CalculateCurvatureAmount(Sensitivity sensitivity)
        {
            if (sensitivity == null)
                throw new ArgumentNullException(nameof(sensitivity));

            if (sensitivity.Category != SensitivityCategory.Vega)
                throw new ArgumentException("Invalid sensitivity specified.", nameof(sensitivity));

            Decimal days = sensitivity.Tenor.Days;
            Decimal scaledDays = ModelParameters.GetCurvatureScaledDays(days);

            return (sensitivity.Amount * scaledDays);
        }

        public static List<MarginProduct> CalculateMarginsProduct(Currency calculationCurrency, FxRatesProvider ratesProvider, List<Sensitivity> sensitivities)
        {
            if (calculationCurrency == null)
                throw new ArgumentNullException(nameof(calculationCurrency));

            if (ratesProvider == null)
                throw new ArgumentNullException(nameof(ratesProvider));

            if (sensitivities == null)
                throw new ArgumentNullException(nameof(sensitivities));

            List<Product> products = sensitivities.Select(x => x.Product).Distinct().OrderBy(x => x).ToList();
            List<MarginProduct> productMargins = new List<MarginProduct>(products.Count);

            foreach (Product product in products)
            {
                List<Sensitivity> sensitivitiesByProduct = sensitivities.Where(x => x.Product == product).ToList();

                List<MarginRisk> riskMargins = CalculateRiskMargins(calculationCurrency, ratesProvider, sensitivitiesByProduct);
                Amount sumSquared = Amount.Sum(riskMargins.Select(x => Amount.Square(x.Value)), calculationCurrency);
                Amount sumCorrelated = CalculateCorrelatedSumRisks(calculationCurrency, riskMargins);
                Amount productMarginAmount = Amount.SquareRoot(sumSquared + sumCorrelated);

                productMargins.Add(MarginProduct.Of(product, productMarginAmount, riskMargins));
            }

            return productMargins;
        }

        public static List<Sensitivity> NetSensitivities(Currency calculationCurrency, List<Sensitivity> sensitivities)
        {
            if (calculationCurrency == null)
                throw new ArgumentNullException(nameof(calculationCurrency));

            if (sensitivities == null)
                throw new ArgumentNullException(nameof(sensitivities));

            if (sensitivities.Count == 0)
                return (new List<Sensitivity>(0));

            List<Sensitivity> sensitivitiesNetted = new List<Sensitivity>(0)
                .Concat
                (
                    sensitivities
                        .Where(x => x.Category == SensitivityCategory.BaseCorrelation)
                        .GroupBy(x => x.Qualifier)
                        .Select(x => Sensitivity.BaseCorrelation(x.Key, Amount.Sum(x.Select(s => s.Amount), calculationCurrency)))
                )
                .Concat
                (
                    sensitivities
                        .Where(x => x.Risk == SensitivityRisk.Commodity)
                        .GroupBy(x => new { x.Category, x.Qualifier, Bucket = (BucketCommodity)x.Bucket })
                        .Select(x => Sensitivity.Commodity(x.Key.Category, x.Key.Qualifier, x.Key.Bucket, Amount.Sum(x.Select(s => s.Amount), calculationCurrency)))
                )
                .Concat
                (
                    sensitivities
                        .Where(x => x.Risk == SensitivityRisk.CreditNonQualifying)
                        .GroupBy(x => new { x.Category, x.Qualifier, Bucket = (BucketCreditNonQualifying)x.Bucket, x.Tenor })
                        .Select(x => Sensitivity.CreditNonQualifying(x.Key.Category, x.Key.Qualifier, x.Key.Bucket, x.Key.Tenor, Amount.Sum(x.Select(s => s.Amount), calculationCurrency)))
                )
                .Concat
                (
                    sensitivities
                        .Where(x => (x.Risk == SensitivityRisk.CreditQualifying) && (x.Category != SensitivityCategory.BaseCorrelation))
                        .GroupBy(x => new { x.Category, x.Qualifier, Bucket = (BucketCreditQualifying)x.Bucket, x.Tenor, x.Label2 })
                        .Select(x => Sensitivity.CreditQualifying(x.Key.Category, x.Key.Qualifier, x.Key.Bucket, x.Key.Tenor, x.Key.Label2, Amount.Sum(x.Select(s => s.Amount), calculationCurrency)))
                )
                .Concat
                (
                    sensitivities
                        .Where(x => x.Subrisk == SensitivitySubrisk.CrossCurrencyBasis)
                        .GroupBy(x => (Currency)x.Bucket)
                        .Select(x => Sensitivity.CrossCurrencyBasis(x.Key, Amount.Sum(x.Select(s => s.Amount), calculationCurrency)))
                )
                .Concat
                (
                    sensitivities
                        .Where(x => x.Risk == SensitivityRisk.Equity)
                        .GroupBy(x => new { x.Category, x.Qualifier, Bucket = (BucketEquity)x.Bucket })
                        .Select(x => Sensitivity.Equity(x.Key.Category, x.Key.Qualifier, x.Key.Bucket, Amount.Sum(x.Select(s => s.Amount), calculationCurrency)))
                )
                .Concat
                (
                    sensitivities
                        .Where(x => x.Risk == SensitivityRisk.Fx)
                        .GroupBy(x => new { x.Category, x.ThresholdIdentifier })
                        .Select(x => Sensitivity.Fx(x.Key.Category, x.Key.ThresholdIdentifier, Amount.Sum(x.Select(s => s.Amount), calculationCurrency)))
                )
                .Concat
                (
                    sensitivities
                        .Where(x => x.Subrisk == SensitivitySubrisk.Inflation)
                        .GroupBy(x => new { x.Category, Bucket = (Currency)x.Bucket })
                        .Select(x => Sensitivity.Inflation(x.Key.Category, x.Key.Bucket, Amount.Sum(x.Select(s => s.Amount), calculationCurrency)))
                )
                .Concat
                (
                    sensitivities
                        .Where(x => x.Subrisk == SensitivitySubrisk.InterestRate)
                        .GroupBy(x => new { x.Category, Bucket = (Currency)x.Bucket, x.Label2, x.Tenor })
                        .Select(x => Sensitivity.InterestRate(x.Key.Category, x.Key.Bucket, x.Key.Tenor, x.Key.Label2, Amount.Sum(x.Select(s => s.Amount), calculationCurrency)))
                )
                .ToList();

            return sensitivitiesNetted;
        }

        public static MarginAddOn CalculateMarginAddOn(Currency calculationCurrency, List<MarginProduct> productMargins, List<AddOnProductMultiplier> productMultipliers, List<AddOnNotional> notionals, List<AddOnNotionalFactor> notionalFactors, List<AddOnFixedAmount> fixedAmounts)
        {
            if (calculationCurrency == null)
                throw new ArgumentNullException(nameof(calculationCurrency));

            if (productMargins == null)
                throw new ArgumentNullException(nameof(productMargins));

            if (productMultipliers == null)
                throw new ArgumentNullException(nameof(productMultipliers));

            if (notionals == null)
                throw new ArgumentNullException(nameof(notionals));

            if (notionalFactors == null)
                throw new ArgumentNullException(nameof(notionalFactors));

            if (fixedAmounts == null)
                throw new ArgumentNullException(nameof(fixedAmounts));

            List<MarginAddOnComponent> margins = new List<MarginAddOnComponent>();
            margins.AddRange(CalculateMarginsAddOnProduct(productMargins, productMultipliers));
            margins.AddRange(CalculateMarginsAddOnNotional(calculationCurrency, notionals, notionalFactors));

            if (fixedAmounts.Count > 0)
            {
                Amount fixedAmountAmount = Amount.Sum(fixedAmounts.Select(x => x.Amount), calculationCurrency);
                margins.Add(MarginAddOnFixedAmount.Of(fixedAmountAmount));
            }

            if (margins.Count == 0)
                return null;

            Amount addOnMarginAmount = Amount.Sum(margins.Select(x => x.Value), calculationCurrency);
            MarginAddOn addOnMargin = MarginAddOn.Of(addOnMarginAmount, margins);

            return addOnMargin;
        }
        #endregion

        #region Nesting (Classes)
        private abstract class MarginCalculator
        {
            #region Methods
            protected static List<MarginWeighting> CalculateMarginsWeighting(SensitivityCategory category, List<Sensitivity> sensitivities)
            {
                if (sensitivities == null)
                    throw new ArgumentNullException(nameof(sensitivities));

                List<MarginWeighting> weightingMargins = new List<MarginWeighting>(sensitivities.Count);

                foreach (Sensitivity sensitivity in sensitivities)
                {
                    Decimal riskWeight = ModelParameters.GetWeightRisk(category, sensitivity);
                    Amount weightingMarginAmount = sensitivity.Amount * riskWeight;
                    
                    weightingMargins.Add(MarginWeighting.Of(sensitivity, weightingMarginAmount));
                }

                return weightingMargins;
            }

            protected static List<MarginWeighting> CalculateMarginsWeighting(SensitivityCategory category, Dictionary<String,Decimal> thresholdFactors, List<Sensitivity> sensitivities)
            {
                if (sensitivities == null)
                    throw new ArgumentNullException(nameof(sensitivities));

                if (thresholdFactors == null)
                    throw new ArgumentNullException(nameof(thresholdFactors));

                List<MarginWeighting> weightingMargins = new List<MarginWeighting>(sensitivities.Count);

                foreach (Sensitivity sensitivity in sensitivities)
                {
                    Decimal riskWeight = ModelParameters.GetWeightRisk(category, sensitivity);
                    Decimal thresholdFactor = (sensitivity.Subrisk == SensitivitySubrisk.CrossCurrencyBasis) ? 1m : thresholdFactors[sensitivity.Qualifier];
                    Amount weightingMarginAmount = sensitivity.Amount * riskWeight * thresholdFactor;

                    weightingMargins.Add(MarginWeighting.Of(sensitivity, weightingMarginAmount));
                }

                return weightingMargins;
            }
            #endregion

            #region Methods (Abstract)
            public abstract MarginSensitivity CalculateMargin(Currency calculationCurrency, FxRatesProvider ratesProvider, SensitivityRisk risk, SensitivityCategory category, List<Sensitivity> sensitivities);
            #endregion
        }

        private sealed class MarginCalculatorBaseCorrelation : MarginCalculator
        {
            #region Methods
            public override MarginSensitivity CalculateMargin(Currency calculationCurrency, FxRatesProvider ratesProvider, SensitivityRisk risk, SensitivityCategory category, List<Sensitivity> sensitivities)
            {
                if (calculationCurrency == null)
                    throw new ArgumentNullException(nameof(calculationCurrency));

                if (ratesProvider == null)
                    throw new ArgumentNullException(nameof(ratesProvider));

                if (!Enum.IsDefined(typeof(SensitivityRisk), risk))
                    throw new InvalidEnumArgumentException("Invalid sensitivity risk specified.");

                if (!Enum.IsDefined(typeof(SensitivityCategory), category))
                    throw new InvalidEnumArgumentException("Invalid sensitivity category specified.");
                    
                if (sensitivities == null)
                    throw new ArgumentNullException(nameof(sensitivities));

                List<MarginWeighting> weightingMargins = CalculateMarginsWeighting(SensitivityCategory.BaseCorrelation, sensitivities);
                Amount sumSquared = Amount.Sum(weightingMargins.Select(x => Amount.Square(x.Value)), calculationCurrency);
                Amount sumCorrelated = CalculateCorrelatedSumWeights(calculationCurrency, weightingMargins);
                Amount amount = Amount.SquareRoot(sumSquared + sumCorrelated);

                MarginBucket bucketMargin = MarginBucket.Of(Placeholder.Instance, amount, weightingMargins);

                return MarginSensitivity.Of(SensitivityCategory.BaseCorrelation, amount, (new List<MarginBucket>{ bucketMargin }));
            }
            #endregion

            #region Methods (Static)
            private static Amount CalculateCorrelatedSumWeights(Currency calculationCurrency, List<MarginWeighting> weightingMargins)
            {
                Amount sum = Amount.OfZero(calculationCurrency);

                foreach (MarginWeighting marginWeighting1 in weightingMargins)
                {
                    foreach (MarginWeighting marginWeighting2 in weightingMargins)
                    {
                        Sensitivity sensitivity1 = marginWeighting1.Sensitivity;
                        Sensitivity sensitivity2 = marginWeighting2.Sensitivity;

                        if (sensitivity1 != sensitivity2)
                        {
                            Decimal correlation = ModelParameters.GetCorrelationBase(sensitivity1, sensitivity2);
                            sum += marginWeighting1.Value * marginWeighting2.Value * correlation;
                        }
                    }
                }

                return sum;
            }
            #endregion
        }

        private sealed class MarginCalculatorCurvature : MarginCalculator
        {
            #region Methods
            public override MarginSensitivity CalculateMargin(Currency calculationCurrency, FxRatesProvider ratesProvider, SensitivityRisk risk, SensitivityCategory category, List<Sensitivity> sensitivities)
            {
                if (calculationCurrency == null)
                    throw new ArgumentNullException(nameof(calculationCurrency));

                if (ratesProvider == null)
                    throw new ArgumentNullException(nameof(ratesProvider));

                if (!Enum.IsDefined(typeof(SensitivityRisk), risk))
                    throw new InvalidEnumArgumentException("Invalid sensitivity risk specified.");

                if (!Enum.IsDefined(typeof(SensitivityCategory), category))
                    throw new InvalidEnumArgumentException("Invalid sensitivity category specified.");

                if (sensitivities == null)
                    throw new ArgumentNullException(nameof(sensitivities));

                List<IBucket> buckets = sensitivities.Select(x => x.Bucket).Distinct().OrderBy(x => x.Name).ToList();
                List<MarginBucket> bucketMargins = new List<MarginBucket>(buckets.Count);

                foreach (IBucket bucket in buckets)
                {
                    List<Sensitivity> sensitivitiesByBucket = sensitivities.Where(x => x.Bucket == bucket).ToList();

                    List<MarginWeighting> weightingMargins = CalculateMarginsWeighting(SensitivityCategory.Curvature, sensitivitiesByBucket);
                    Amount sumSquared = Amount.Sum(weightingMargins.Select(x => Amount.Square(x.Value)), calculationCurrency);
                    Amount sumCorrelated = CalculateCorrelatedSumWeights(calculationCurrency, risk, weightingMargins);
                    Amount bucketMarginAmount = Amount.SquareRoot(sumSquared + sumCorrelated);

                    bucketMargins.Add(MarginBucket.Of(bucket, bucketMarginAmount, weightingMargins));
                }

                Amount marginNonResidual = CalculateMarginByBuckets(calculationCurrency, risk, false, bucketMargins);
                Amount marginResidual = CalculateMarginByBuckets(calculationCurrency, risk, true, bucketMargins);
                Amount sensitivityMarginAmount = ModelParameters.GetCurvatureScaleFactor(risk) * (marginNonResidual + marginResidual);

                return MarginSensitivity.Of(SensitivityCategory.Curvature, sensitivityMarginAmount, bucketMargins);
            }
            #endregion

            #region Methods (Static)
            private static Amount CalculateCorrelatedSumBuckets(Currency calculationCurrency, SensitivityRisk risk, List<MarginBucket> bucketMargins)
            {
                Dictionary<IBucket,Amount> weightingMarginSums = new Dictionary<IBucket,Amount>();

                foreach (MarginBucket bucketMargin in bucketMargins)
                {
                    Amount weightingMarginsSum = Amount.Sum(bucketMargin.Children.Select(x => x.Value), calculationCurrency);
                    Amount bucketMarginAmount = Amount.Max(Amount.Min(weightingMarginsSum, bucketMargin.Value), -bucketMargin.Value);
                    weightingMarginSums[bucketMargin.Bucket] = bucketMarginAmount;
                }

                Amount sum = Amount.OfZero(calculationCurrency);

                foreach (IBucket bucket1 in weightingMarginSums.Keys)
                {
                    foreach (IBucket bucket2 in weightingMarginSums.Keys)
                    {
                        if (bucket1 != bucket2)
                        {
                            Decimal correlation = MathUtilities.Square(ModelParameters.GetCorrelationBucket(risk, bucket1, bucket2));
                            sum += weightingMarginSums[bucket1] * weightingMarginSums[bucket2] * correlation;
                        }
                    }
                }

                return sum;
            }

            private static Amount CalculateCorrelatedSumWeights(Currency calculationCurrency, SensitivityRisk risk, List<MarginWeighting> weightingMargins)
            {
                Amount sum = Amount.OfZero(calculationCurrency);

                foreach (MarginWeighting marginWeighting1 in weightingMargins)
                {
                    foreach (MarginWeighting marginWeighting2 in weightingMargins)
                    {
                        Sensitivity sensitivity1 = marginWeighting1.Sensitivity;
                        Sensitivity sensitivity2 = marginWeighting2.Sensitivity;

                        if (sensitivity1 != sensitivity2)
                        {
                            Decimal correlation = MathUtilities.Square(ModelParameters.GetCorrelationSensitivity(risk, sensitivity1, sensitivity2));
                            sum += marginWeighting1.Value * marginWeighting2.Value * correlation;
                        }
                    }
                }

                return sum;
            }

            private static Amount CalculateMarginByBuckets(Currency calculationCurrency, SensitivityRisk risk, Boolean residuals, List<MarginBucket> bucketMargins)
            {
                if (residuals)
                    bucketMargins = bucketMargins.Where(x => x.Bucket.IsResidual).ToList();
                else
                    bucketMargins = bucketMargins.Where(x => !x.Bucket.IsResidual).ToList();

                Amount crossBucketSum = Amount.Sum(bucketMargins.Select(x => Amount.Square(x.Value)), calculationCurrency);
                Amount crossBucketCorrelation = residuals ? Amount.OfZero(calculationCurrency) : CalculateCorrelatedSumBuckets(calculationCurrency, risk, bucketMargins);

                List<IMargin> weightingMargins = bucketMargins.SelectMany(x => x.Children).ToList();
                Amount cvr = Amount.Sum(weightingMargins.Select(x => x.Value), calculationCurrency);
                Amount cvrAbsolute = Amount.Sum(weightingMargins.Select(x => Amount.Abs(x.Value)), calculationCurrency);
                
                Decimal theta = cvrAbsolute.IsZero ? 0m : Amount.Min(cvr / cvrAbsolute, 0m).Value;
                Decimal lambda = ModelParameters.GetCurvatureLambda(theta);
                Amount kappa = Amount.SquareRoot(crossBucketSum + crossBucketCorrelation);

                return Amount.Max(cvr + (lambda * kappa), 0m);
            }
            #endregion
        }

        private sealed class MarginCalculatorOther : MarginCalculator
        {
            #region Methods
            public override MarginSensitivity CalculateMargin(Currency calculationCurrency, FxRatesProvider ratesProvider, SensitivityRisk risk, SensitivityCategory category, List<Sensitivity> sensitivities)
            {
                if (calculationCurrency == null)
                    throw new ArgumentNullException(nameof(calculationCurrency));

                if (ratesProvider == null)
                    throw new ArgumentNullException(nameof(ratesProvider));

                if (!Enum.IsDefined(typeof(SensitivityRisk), risk))
                    throw new InvalidEnumArgumentException("Invalid sensitivity risk specified.");

                if (!Enum.IsDefined(typeof(SensitivityCategory), category))
                    throw new InvalidEnumArgumentException("Invalid sensitivity category specified.");

                if (sensitivities == null)
                    throw new ArgumentNullException(nameof(sensitivities));

                List<IBucket> buckets = sensitivities.Select(x => x.Bucket).Distinct().OrderBy(x => x.Name).ToList();
                List<MarginBucket> bucketMargins = new List<MarginBucket>(buckets.Count);
                Dictionary<IBucket,Dictionary<String,Decimal>> thresholdFactors = new Dictionary<IBucket,Dictionary<String,Decimal>>();

                foreach (IBucket bucket in buckets)
                {
                    List<Sensitivity> sensitivitiesByBucket = sensitivities.Where(x => x.Bucket == bucket).ToList();

                    thresholdFactors[bucket] = sensitivitiesByBucket
                        .GroupBy(x => new {x.Bucket, x.Qualifier})
                        .ToDictionary(x => x.Key.Qualifier, x => CalculateThresholdFactor(calculationCurrency, ratesProvider, risk, category, x.ToList()));

                    List<MarginWeighting> weightingMargins = CalculateMarginsWeighting(category, thresholdFactors[bucket], sensitivitiesByBucket);
                    Amount sumSquared = Amount.Sum(weightingMargins.Select(x => Amount.Square(x.Value)), calculationCurrency);
                    Amount sumCorrelated = CalculateCorrelatedSumWeights(calculationCurrency, risk, weightingMargins, thresholdFactors[bucket]);
                    Amount bucketMarginAmount = Amount.SquareRoot(sumSquared + sumCorrelated);

                    bucketMargins.Add(MarginBucket.Of(bucket, bucketMarginAmount, weightingMargins));
                }

                List<MarginBucket> bucketMarginsNonResidual = bucketMargins.Where(x => !x.Bucket.IsResidual).ToList();
                Amount sumSquaredNonResidual = Amount.Sum(bucketMarginsNonResidual.Select(x => Amount.Square(x.Value)), calculationCurrency);

                List<MarginBucket> bucketMarginsResidual = bucketMargins.Where(x => x.Bucket.IsResidual).ToList();
                Amount sumSquaredResidual = Amount.Sum(bucketMarginsResidual.Select(x => Amount.Square(x.Value)), calculationCurrency);

                Amount sumCorrelatedNonResidual;

                if ((risk == SensitivityRisk.Rates) && (category == SensitivityCategory.Delta))
                {
                    Dictionary<IBucket,Decimal> bucketThresholdFactors = thresholdFactors.ToDictionary(x => x.Key, x => x.Value.Values.Single());
                    sumCorrelatedNonResidual = CalculateCorrelatedSumBuckets(calculationCurrency, risk, bucketThresholdFactors, bucketMarginsNonResidual);
                }
                else
                    sumCorrelatedNonResidual = CalculateCorrelatedSumBuckets(calculationCurrency, risk, bucketMarginsNonResidual);

                Amount sensitivityMarginAmount = Amount.SquareRoot(sumSquaredNonResidual + sumCorrelatedNonResidual) + Amount.SquareRoot(sumSquaredResidual);

                return MarginSensitivity.Of(category, sensitivityMarginAmount, bucketMargins);
            }
            #endregion

            #region Methods (Static)
            private static Amount CalculateCorrelatedSumBuckets(Currency calculationCurrency, SensitivityRisk risk, List<MarginBucket> bucketMargins)
            {
                Dictionary<IBucket,Amount> weightingMarginSums = new Dictionary<IBucket,Amount>();

                foreach (MarginBucket bucketMargin in bucketMargins)
                {
                    Amount weightingMarginsSum = Amount.Sum(bucketMargin.Children.Select(x => x.Value), calculationCurrency);
                    Amount bucketMarginValue = Amount.Max(Amount.Min(weightingMarginsSum, bucketMargin.Value), -bucketMargin.Value);
                    weightingMarginSums[bucketMargin.Bucket] = bucketMarginValue;
                }

                Amount sum = Amount.OfZero(calculationCurrency);

                foreach (IBucket bucket1 in weightingMarginSums.Keys)
                {
                    foreach (IBucket bucket2 in weightingMarginSums.Keys)
                    {
                        if (bucket1 != bucket2)
                        {
                            Decimal correlation = ModelParameters.GetCorrelationBucket(risk, bucket1, bucket2);
                            sum += weightingMarginSums[bucket1] * weightingMarginSums[bucket2] * correlation;
                        }
                    }
                }

                return sum;
            }

            private static Amount CalculateCorrelatedSumBuckets(Currency calculationCurrency, SensitivityRisk risk, Dictionary<IBucket,Decimal> thresholdFactors, List<MarginBucket> bucketMargins)
            {
                Dictionary<IBucket,Amount> weightingMarginSums = new Dictionary<IBucket,Amount>();

                foreach (MarginBucket bucketMargin in bucketMargins)
                {
                    Amount weightingMarginsSum = Amount.Sum(bucketMargin.Children.Select(x => x.Value), calculationCurrency);
                    Amount bucketMarginValue = Amount.Max(Amount.Min(weightingMarginsSum, bucketMargin.Value), -bucketMargin.Value);
                    weightingMarginSums[bucketMargin.Bucket] = bucketMarginValue;
                }

                Amount sum = Amount.OfZero(calculationCurrency);

                foreach (IBucket bucket1 in weightingMarginSums.Keys)
                {
                    foreach (IBucket bucket2 in weightingMarginSums.Keys)
                    {
                        if (bucket1 != bucket2)
                        {
                            Decimal thresholdFactor1 = thresholdFactors[bucket1];
                            Decimal thresholdFactor2 = thresholdFactors[bucket2];
                            Decimal concentration = Math.Min(thresholdFactor1, thresholdFactor2) / Math.Max(thresholdFactor1, thresholdFactor2);

                            Decimal correlationBucket = ModelParameters.GetCorrelationBucket(risk, bucket1, bucket2);
                            Decimal correlation = correlationBucket * concentration;

                            sum += weightingMarginSums[bucket1] * weightingMarginSums[bucket2] * correlation;
                        }
                    }
                }

                return sum;
            }

            private static Amount CalculateCorrelatedSumWeights(Currency calculationCurrency, SensitivityRisk risk, List<MarginWeighting> weightingMargins, Dictionary<String,Decimal> thresholdFactors)
            {
                Amount sum = Amount.OfZero(calculationCurrency);

                foreach (MarginWeighting marginWeighting1 in weightingMargins)
                {
                    foreach (MarginWeighting marginWeighting2 in weightingMargins)
                    {
                        Sensitivity sensitivity1 = marginWeighting1.Sensitivity;
                        Sensitivity sensitivity2 = marginWeighting2.Sensitivity;

                        if (sensitivity1 != sensitivity2)
                        {
                            Decimal thresholdFactor1 = thresholdFactors[sensitivity1.Qualifier];
                            Decimal thresholdFactor2 = thresholdFactors[sensitivity2.Qualifier];
                            Decimal concentration = Math.Min(thresholdFactor1, thresholdFactor2) / Math.Max(thresholdFactor1, thresholdFactor2);

                            Decimal correlationSensitivity = ModelParameters.GetCorrelationSensitivity(risk, sensitivity1, sensitivity2);
                            Decimal correlation = correlationSensitivity * concentration;

                            sum += marginWeighting1.Value * marginWeighting2.Value * correlation;
                        }
                    }
                }

                return sum;
            }
            #endregion
        }
        #endregion
    }
}