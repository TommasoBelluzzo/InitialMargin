#region Using Directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Model
{
    internal sealed class ModelProcessor : Processor
    {
        #region Constructors
        public ModelProcessor(DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider) : base(valuationDate, calculationCurrency, ratesProvider) { }
        #endregion

        #region Methods
        private MarginTotal Process(List<Sensitivity> sensitivities, List<AddOnProductMultiplier> productMultipliers, List<AddOnNotional> notionals, List<AddOnNotionalFactor> notionalFactors, List<AddOnFixedAmount> fixedAmounts)
        {
            if (sensitivities == null)
                throw new ArgumentNullException(nameof(sensitivities));

            if (productMultipliers == null)
                throw new ArgumentNullException(nameof(productMultipliers));

            if (notionals == null)
                throw new ArgumentNullException(nameof(notionals));

            if (notionalFactors == null)
                throw new ArgumentNullException(nameof(notionalFactors));

            if (fixedAmounts == null)
                throw new ArgumentNullException(nameof(fixedAmounts));

            ModelObject? modelObject = SanitizeData(sensitivities, productMultipliers, notionals, notionalFactors, fixedAmounts);
            
            if (!modelObject.HasValue)
                return MarginTotal.Of(m_ValuationDate, m_CalculationCurrency);

            ModelObject modelObjectValue = modelObject.Value;
            sensitivities = modelObjectValue.Sensitivities;
            notionals = modelObjectValue.Notionals;
            notionalFactors = modelObjectValue.NotionalFactors;
            productMultipliers = modelObjectValue.ProductMultipliers;
            fixedAmounts = modelObjectValue.FixedAmounts;

            List<Sensitivity> sensitivitiesVega = sensitivities
                .RemoveAllAndGet(x => x.Category == SensitivityCategory.Vega)
                .ToList();

            if (sensitivitiesVega.Count > 0)
            {
                List<Sensitivity> sensitivitiesCurvature = sensitivitiesVega
                    .Select(x => x.ToCurvature(ModelCalculations.CalculateCurvatureAmount(x)))
                    .ToList();

                List<Sensitivity> sensitivitiesVolatilityWeighted = sensitivitiesVega
                    .Concat(sensitivitiesCurvature)
                    .ToList();

                foreach (Sensitivity sensitivity in sensitivitiesVolatilityWeighted)
                    sensitivity.ChangeAmount(sensitivity.Amount * ModelParameters.GetWeightVolatility(sensitivity));
                
                sensitivities.AddRange(sensitivitiesVolatilityWeighted);
            }

            List<Sensitivity> sensitivitiesNetted = ModelCalculations.NetSensitivities(m_CalculationCurrency, sensitivities);
            
            List<MarginProduct> productMargins = ModelCalculations.CalculateMarginsProduct(m_CalculationCurrency, m_RatesProvider, sensitivitiesNetted);
            Amount productMarginsAmount = Amount.Sum(productMargins.Select(x => x.Value), m_CalculationCurrency);

            MarginAddOn addOnMargin = ModelCalculations.CalculateMarginAddOn(m_CalculationCurrency, productMargins, productMultipliers, notionals, notionalFactors, fixedAmounts);
            Amount addOnMarginAmount = addOnMargin?.Value ?? Amount.OfZero(m_CalculationCurrency);

            Amount modelMarginAmount = productMarginsAmount + addOnMarginAmount;
            Margin modelMargin = Margin.Of(modelMarginAmount, productMargins, addOnMargin);

            return MarginTotal.Of(m_ValuationDate, m_CalculationCurrency, modelMargin);
        }

        private ModelObject? SanitizeData(List<Sensitivity> sensitivities, List<AddOnProductMultiplier> productMultipliers, List<AddOnNotional> notionals, List<AddOnNotionalFactor> notionalFactors, List<AddOnFixedAmount> fixedAmounts)
        {
            sensitivities.RemoveAll(x => x.EndDate < m_ValuationDate);
            sensitivities.RemoveAll(x => (x.Category == SensitivityCategory.Delta) && (x.Risk == SensitivityRisk.Fx) && ((x.ThresholdIdentifier as Currency) == m_CalculationCurrency));

            notionals.RemoveAll(x => x.EndDate < m_ValuationDate);

            if ((sensitivities.Count == 0) && (notionals.Count == 0))
                return null;

            Dictionary<String,List<DataValue>> dataValues = sensitivities.Select(x => x.ChangeAmount(m_RatesProvider.Convert(x.Amount, m_CalculationCurrency)))
                .Concat(notionals.Select(x => x.ChangeAmount(m_RatesProvider.Convert(x.Amount, m_CalculationCurrency))))
                .Concat(fixedAmounts.Select(x => x.ChangeAmount(m_RatesProvider.Convert(x.Amount, m_CalculationCurrency))))
                .GroupBy(x => String.Concat(x.PortfolioId, "/", x.TradeId))
                .ToDictionary(x => x.Key, x => x.ToList());

            HashSet<Product> products = new HashSet<Product>();
            HashSet<String> qualifiers = new HashSet<String>();

            foreach (String key in dataValues.Keys.OrderBy(x => x))
            {
                List<DataValue> dataValuesByKey = dataValues[key];

                List<DateTime> endDatesByKey = dataValuesByKey.Select(x => x.EndDate)
                    .Distinct().ToList();

                if (endDatesByKey.Count != 1)
                    throw new InvalidDataException($"The trade \"{key}\" defines notionals and present values having different end dates.");

                List<Product> productsByKey = dataValuesByKey
                    .OfType<Sensitivity>()
                    .Select(x => x.Product)
                    .Distinct().ToList();

                if (productsByKey.Count != 0)
                {
                    if (productsByKey.Count != 1)
                        throw new InvalidDataException($"The trade \"{key}\" defines sensitivities having different products.");

                    products.Add(productsByKey.Single());
                }

                List<String> qualifiersByKey = dataValuesByKey
                    .OfType<AddOnNotional>()
                    .Select(x => x.Qualifier)
                    .Distinct().ToList();

                if (qualifiersByKey.Count != 0)
                {
                    if (qualifiersByKey.Count != 1)
                        throw new InvalidDataException($"The trade \"{key}\" defines notionals having different qualifiers.");

                    qualifiers.Add(qualifiersByKey.Single());
                }
            }

            Dictionary<Product,List<AddOnProductMultiplier>> productMultipliersGrouped = productMultipliers
                .GroupBy(x => x.Product)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (KeyValuePair<Product,List<AddOnProductMultiplier>> kvp in productMultipliersGrouped)
            {
                if (kvp.Value.Count > 1)
                    throw new InvalidDataException($"The dataset defines multiple product multipliers for product \"{kvp.Key}\".");

                if (!products.Contains(kvp.Key))
                    throw new InvalidDataException($"The dataset defines a product multiplier for product \"{kvp.Key}\", but no sensitivities are associated to it.");
            }

            Dictionary<String,List<AddOnNotionalFactor>> notionalFactorsGrouped = notionalFactors
                .GroupBy(x => x.Qualifier)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (KeyValuePair<String,List<AddOnNotionalFactor>> kvp in notionalFactorsGrouped)
            {
                if (kvp.Value.Count > 1)
                    throw new InvalidDataException($"The dataset defines multiple notional factors for qualifier \"{kvp.Key}\".");

                if (!qualifiers.Contains(kvp.Key))
                    throw new InvalidDataException($"The dataset defines a notional factor for qualifier \"{kvp.Key}\", but no notionals are associated to it.");

                qualifiers.Remove(kvp.Key);
            }

            if (qualifiers.Count > 0)
                throw new InvalidDataException($"The dataset defines notionals without a respective notional factor on the following qualifiers: {String.Join(", ", qualifiers.Select(x => $"\"{x}\""))}.");

            return (new ModelObject
            {
                Sensitivities = sensitivities,
                ProductMultipliers = productMultipliers,
                Notionals = notionals,
                NotionalFactors = notionalFactors,
                FixedAmounts = fixedAmounts
            });
        }

        public override MarginTotal Process(List<DataValue> values, List<DataParameter> parameters)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            List<Sensitivity> sensitivities = values.OfType<Sensitivity>().ToList();
            List<AddOnProductMultiplier> productMultipliers = parameters.OfType<AddOnProductMultiplier>().ToList();
            List<AddOnNotional> notionals = values.OfType<AddOnNotional>().ToList();
            List<AddOnNotionalFactor> notionalFactors = parameters.OfType<AddOnNotionalFactor>().ToList();
            List<AddOnFixedAmount> fixedAmounts = values.OfType<AddOnFixedAmount>().ToList();

            return Process(sensitivities, productMultipliers, notionals, notionalFactors, fixedAmounts);
        }
        #endregion

        #region Nesting (Structures)
        private struct ModelObject
        {
            #region Members
            public List<Sensitivity> Sensitivities;
            public List<AddOnProductMultiplier> ProductMultipliers;
            public List<AddOnNotional> Notionals;
            public List<AddOnNotionalFactor> NotionalFactors;
            public List<AddOnFixedAmount> FixedAmounts;
            #endregion
        }
        #endregion
    }
}