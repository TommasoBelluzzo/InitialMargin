#region Using Directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Schedule
{
    internal sealed class ScheduleProcessor : Processor
    {
        #region Constructors
        public ScheduleProcessor(DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider) : base(valuationDate, calculationCurrency, ratesProvider) { }
        #endregion

        #region Methods
        private List<ScheduleObject> SanitizeData(List<Notional> notionals, List<PresentValue> presentValues)
        {
            notionals.RemoveAll(x => x.EndDate < m_ValuationDate);
            presentValues.RemoveAll(x => x.EndDate < m_ValuationDate);

            if (notionals.Count == 0)
                return (new List<ScheduleObject>(0));

            Dictionary<(String,String),List<DataValue>> dataValues = notionals.Select(x => x.ChangeAmount(m_RatesProvider.Convert(x.Amount, m_CalculationCurrency)))
                .Concat(presentValues.Select(x => x.ChangeAmount(m_RatesProvider.Convert(x.Amount, m_CalculationCurrency))))
                .GroupBy(x => (x.PortfolioId,x.TradeId))
                .ToDictionary(x => x.Key, x => x.ToList());

            List<ScheduleObject> scheduleObjects = new List<ScheduleObject>(dataValues.Keys.Count);

            foreach ((String,String) key in dataValues.Keys.OrderBy(x => x))
            {
                String portfolioId = key.Item1;
                String tradeId = key.Item2;

                List<DataValue> dataValuesByKey = dataValues[key];
                List<Notional> notionalsByKey = dataValuesByKey.OfType<Notional>().ToList();
                List<PresentValue> presentValuesByKey = dataValuesByKey.OfType<PresentValue>().ToList();

                if (notionalsByKey.Count == 0)
                    throw new InvalidDataException($"The trade \"{tradeId}\" (portfolio \"{portfolioId}\") defines no notionals.");

                if ((presentValuesByKey.Count != 0) && (presentValuesByKey.Count != 1) && (presentValuesByKey.Count != notionalsByKey.Count))
                    throw new InvalidDataException($"The trade \"{tradeId}\" (portfolio \"{portfolioId}\") defines an invalid number of present values.");

                List<Product> productsByKey = notionalsByKey.Select(x => x.Product)
                    .Union(presentValuesByKey.Select(x => x.Product))
                    .ToList();

                if (productsByKey.Count != 1)
                    throw new InvalidDataException($"The trade \"{tradeId}\" (portfolio \"{portfolioId}\") defines notionals and present values having different products.");

                List<DateTime> endDatesByKey = dataValuesByKey.Select(x => x.EndDate)
                    .Distinct().ToList();

                if (endDatesByKey.Count != 1)
                    throw new InvalidDataException($"The trade \"{tradeId}\" (portfolio \"{portfolioId}\") defines notionals and present values having different end dates.");

                scheduleObjects.Add(new ScheduleObject
                {
                    Product = productsByKey.Single(),
                    Notionals = notionalsByKey,
                    PresentValues = presentValuesByKey,
                    TradeInfo = TradeInfo.Of(portfolioId, tradeId, endDatesByKey.Single())
                });
            }

            return scheduleObjects;
        }

        private MarginTotal Process(List<Notional> notionals, List<PresentValue> presentValues)
        {
            if (notionals == null)
                throw new ArgumentNullException(nameof(notionals));

            if (presentValues == null)
                throw new ArgumentNullException(nameof(presentValues));

            List<ScheduleObject> scheduleObjects = SanitizeData(notionals, presentValues);

            if (scheduleObjects.Count == 0)
                return MarginTotal.Of(m_ValuationDate, m_CalculationCurrency);

            HashSet<Product> products = new HashSet<Product>();
            List<Notional> notionalsNetted = new List<Notional>(scheduleObjects.Count);
            List<PresentValue> presentValuesNetted = new List<PresentValue>(scheduleObjects.Count);

            foreach (ScheduleObject obj in scheduleObjects)
            {
                Amount notionalsSum = Amount.Abs(Amount.Sum(obj.Notionals.Select(x => x.Amount), m_CalculationCurrency));
                notionalsNetted.Add(Notional.Of(obj.Product, notionalsSum, RegulationsInfo.Empty, obj.TradeInfo));

                Amount presentValuesSum = Amount.Sum(obj.PresentValues.Select(x => x.Amount), m_CalculationCurrency);
                presentValuesNetted.Add(PresentValue.Of(obj.Product, presentValuesSum, RegulationsInfo.Empty, obj.TradeInfo));

                products.Add(obj.Product);
            }

            List<MarginProduct> productMargins = new List<MarginProduct>(products.Count);

            foreach (Product product in products.OrderBy(x => x))
            {
                List<Notional> notionalsByProduct = notionalsNetted.Where(x => x.Product == product).ToList();
                List<PresentValue> presentValuesByProduct = presentValuesNetted.Where(x => x.Product == product).ToList();
                MarginProduct productMargin = ScheduleCalculations.CalculateMarginProduct(m_ValuationDate, m_CalculationCurrency, product, notionalsByProduct, presentValuesByProduct);

                productMargins.Add(productMargin);
            }

            Margin scheduleMargin = ScheduleCalculations.CalculateMargin(m_ValuationDate, m_CalculationCurrency, productMargins, notionals, presentValues);

            return MarginTotal.Of(m_ValuationDate, m_CalculationCurrency, scheduleMargin);
        }

        public override MarginTotal Process(List<DataValue> values, List<DataParameter> parameters)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            List<Notional> notionals = values.OfType<Notional>().ToList();
            List<PresentValue> presentValues = values.OfType<PresentValue>().ToList();

            return Process(notionals, presentValues);
        }
        #endregion

        #region Nesting (Structures)
        private struct ScheduleObject
        {
            #region Members
            public Product Product;
            public List<Notional> Notionals;
            public List<PresentValue> PresentValues;
            public TradeInfo TradeInfo;
            #endregion
        }
        #endregion
    }
}