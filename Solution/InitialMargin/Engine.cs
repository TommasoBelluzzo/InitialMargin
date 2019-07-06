#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using InitialMargin.Core;
using InitialMargin.IO;
using InitialMargin.Model;
using InitialMargin.Schedule;
#endregion

namespace InitialMargin
{
    public static class Engine
    {
        #region Methods
        private static List<Processor> InitializeProcessors(DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider)
        {
            List<Type> processorTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.IsSubclassOf(typeof(Processor)))
                .ToList();

            List<Processor> processors = new List<Processor>(processorTypes.Count);

            foreach (Type processorType in processorTypes)
                processors.Add((Processor)Activator.CreateInstance(processorType, valuationDate, calculationCurrency, ratesProvider));

            return processors;
        }

        private static List<Regulation> SelectRegulations(DataEntity entity, RegulationRole regulationRole)
        {
            return ((regulationRole == RegulationRole.Pledgor) ? entity.PostRegulations : entity.CollectRegulations).ToList();
        }

        private static DataValue AdjustedClone(PresentValue presentValue, RegulationRole regulationRole)
        {
            DataValue clone = (DataValue)presentValue.Clone();

            if (regulationRole == RegulationRole.Secured)
                clone = clone.ChangeAmount(-clone.Amount);

            return clone;
        }

        private static DataValue AdjustedClone(Sensitivity sensitivity, RegulationRole regulationRole)
        {
            DataValue clone = (DataValue)sensitivity.Clone();

            if (regulationRole == RegulationRole.Pledgor)
                clone = clone.ChangeAmount(-clone.Amount);

            return clone;
        }

        public static Amount Calculate(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataEntity> entities)
        {
            return CalculateDetailed(regulationRole, valuationDate, calculationCurrency, ratesProvider, entities).Value;
        }

        public static Amount Calculate(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values)
        {
            return CalculateDetailed(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, (new List<DataParameter>(0))).Value;
        }

        public static Amount Calculate(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values, List<DataParameter> parameters)
        {
            return CalculateDetailed(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters).Value;
        }

        public static Amount Calculate(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, String crifPath)
        {
            return CalculateDetailed(regulationRole, valuationDate, calculationCurrency, ratesProvider, crifPath).Value;
        }

        public static Amount CalculateWorstOf(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataEntity> entities)
        {
            return CalculateDetailedWorstOf(regulationRole, valuationDate, calculationCurrency, ratesProvider, entities).Value;
        }

        public static Amount CalculateWorstOf(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values)
        {
            return CalculateDetailedWorstOf(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, (new List<DataParameter>(0))).Value;
        }

        public static Amount CalculateWorstOf(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values, List<DataParameter> parameters)
        {
            return CalculateDetailedWorstOf(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters).Value;
        }

        public static Amount CalculateWorstOf(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, String crifPath)
        {
            return CalculateDetailedWorstOf(regulationRole, valuationDate, calculationCurrency, ratesProvider, crifPath).Value;
        }

        public static Dictionary<Regulation,Amount> CalculateByRole(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateByRole(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters);
        }

        public static Dictionary<Regulation,Amount> CalculateByRole(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values)
        {
            return CalculateByRole(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, (new List<DataParameter>(0)));
        }

        public static Dictionary<Regulation,Amount> CalculateByRole(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values, List<DataParameter> parameters)
        {
            Dictionary<Regulation,MarginTotal> result = CalculateDetailedByRole(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters);
            return result.ToDictionary(x => x.Key, x => x.Value.Value);
        }

        public static Dictionary<Regulation,Amount> CalculateByRole(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, String crifPath)
        {
            List<DataEntity> entities = CrifManager.Read(crifPath);
            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateByRole(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters);
        }

        public static Dictionary<Regulation,MarginTotal> CalculateDetailedByRole(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailedByRole(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters);
        }

        public static Dictionary<Regulation,MarginTotal> CalculateDetailedByRole(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values)
        {
            return CalculateDetailedByRole(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, (new List<DataParameter>(0)));
        }

        public static Dictionary<Regulation,MarginTotal> CalculateDetailedByRole(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values, List<DataParameter> parameters)
        {
            if (!Enum.IsDefined(typeof(RegulationRole), regulationRole))
                throw new InvalidEnumArgumentException("Invalid regulation role specified.");

            if (calculationCurrency == null)
                throw new ArgumentNullException(nameof(calculationCurrency));

            if (ratesProvider == null)
                throw new ArgumentNullException(nameof(ratesProvider));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            List<Regulation> regulations = values.SelectMany(x => SelectRegulations(x, regulationRole))
                .Concat(parameters.SelectMany(x => SelectRegulations(x, regulationRole)))
                .OrderBy(x => x)
                .Distinct().ToList();

            if (regulations.Count == 0)
                return (new Dictionary<Regulation,MarginTotal>());

            List<DataValue> valuesFinal = values.Where(x => !(x is PresentValue) && !(x is Sensitivity)).Select(x => (DataValue)x.Clone())
                .Concat(values.OfType<PresentValue>().Select(x => AdjustedClone(x, regulationRole)))
                .Concat(values.OfType<Sensitivity>().Select(x => AdjustedClone(x, regulationRole)))
                .ToList();

            List<DataParameter> parametersFinal = parameters
                .Select(x => (DataParameter)x.Clone())
                .ToList();

            List<Processor> processors = InitializeProcessors(valuationDate, calculationCurrency, ratesProvider);
            Dictionary<Regulation,MarginTotal> result = new Dictionary<Regulation,MarginTotal>();

            foreach (Regulation regulation in regulations)
            {
                List<DataValue> valuesByRegulation = valuesFinal.Where(x => SelectRegulations(x, regulationRole).Contains(regulation)).ToList();
                List<DataParameter> parametersByRegulation = parametersFinal.Where(x => SelectRegulations(x, regulationRole).Contains(regulation)).ToList();

                List<IMargin> margins = new List<IMargin>(processors.Count);

                foreach (Processor processor in processors)
                    margins.Add(processor.Process(valuesByRegulation, parametersByRegulation));

                result[regulation] = MarginTotal.Of(regulationRole, regulation, valuationDate, calculationCurrency, margins);
            }

            return result;
        }

        public static Dictionary<Regulation,MarginTotal> CalculateDetailedByRole(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, String crifPath)
        {
            List<DataEntity> entities = CrifManager.Read(crifPath);
            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailedByRole(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters);
        }

        public static MarginTotal CalculateDetailed(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailed(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters);
        }

        public static MarginTotal CalculateDetailed(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values)
        {
            return CalculateDetailed(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, (new List<DataParameter>(0)));
        }

        public static MarginTotal CalculateDetailed(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values, List<DataParameter> parameters)
        {
            if (calculationCurrency == null)
                throw new ArgumentNullException(nameof(calculationCurrency));

            if (ratesProvider == null)
                throw new ArgumentNullException(nameof(ratesProvider));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            List<Regulation> regulations = values.SelectMany(x => SelectRegulations(x, regulationRole))
                .Concat(parameters.SelectMany(x => SelectRegulations(x, regulationRole)))
                .OrderBy(x => x)
                .Distinct().ToList();

            if (regulations.Count > 1)
                throw new InvalidOperationException($"All data entities must either have no regulations defined or belong to a single {((regulationRole == RegulationRole.Pledgor) ? "post" : "collect")} regulation.");

            List<DataValue> valuesFinal = values.Where(x => !(x is PresentValue) && !(x is Sensitivity)).Select(x => (DataValue)x.Clone())
                .Concat(values.OfType<PresentValue>().Select(x => AdjustedClone(x, regulationRole)))
                .Concat(values.OfType<Sensitivity>().Select(x => AdjustedClone(x, regulationRole)))
                .ToList();

            List<DataParameter> parametersFinal = parameters
                .Select(x => (DataParameter)x.Clone())
                .ToList();

            List<Processor> processors = InitializeProcessors(valuationDate, calculationCurrency, ratesProvider);
            List<IMargin> margins = new List<IMargin>(processors.Count);

            foreach (Processor processor in processors)
                margins.Add(processor.Process(valuesFinal, parametersFinal));

            return MarginTotal.Of(regulationRole, regulations.SingleOrDefault(), valuationDate, calculationCurrency, margins);
        }

        public static MarginTotal CalculateDetailed(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, String crifPath)
        {
            List<DataEntity> entities = CrifManager.Read(crifPath);
            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailed(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters);
        }
        
        public static MarginTotal CalculateDetailedWorstOf(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailedWorstOf(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters);
        }

        public static MarginTotal CalculateDetailedWorstOf(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values)
        {
            return CalculateDetailedWorstOf(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, (new List<DataParameter>(0)));
        }

        public static MarginTotal CalculateDetailedWorstOf(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, List<DataValue> values, List<DataParameter> parameters)
        {
            Dictionary<Regulation,MarginTotal> result = CalculateDetailedByRole(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters);
            return result.Select(x => x.Value).OrderByDescending(x => Amount.Abs(x.Value)).ThenBy(x => x.Regulation).FirstOrDefault();
        }

        public static MarginTotal CalculateDetailedWorstOf(RegulationRole regulationRole, DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider, String crifPath)
        {
            List<DataEntity> entities = CrifManager.Read(crifPath);
            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailedWorstOf(regulationRole, valuationDate, calculationCurrency, ratesProvider, values, parameters);
        }
        #endregion
    }
}