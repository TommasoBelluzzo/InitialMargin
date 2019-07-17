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
    /// <summary>Represents the core class of the library, in charge of processing entities and producing calculation results. This class cannot be derived.</summary>
    public sealed class Engine
    {
        #region Members
        private readonly Currency m_CalculationCurrency;
        private readonly DateTime m_ValuationDate;
        private readonly FxRatesProvider m_RatesProvider;
        private readonly List<Processor> m_Processors;
        #endregion

        #region Properties
        /// <summary>Gets the calculation currency of the engine.</summary>
        /// <value>A <see cref="T:InitialMargin.Core.Currency"/> object.</value>
        public Currency CalculationCurrency => m_CalculationCurrency;

        /// <summary>Gets the valuation date of the engine.</summary>
        /// <value>A <see cref="T:System.DateTime"/> without time component.</value>
        public DateTime ValuationDate => m_ValuationDate;

        /// <summary>Gets the rates provider of the engine.</summary>
        /// <value>An <see cref="T:InitialMargin.Core.FxRatesProvider"/> object.</value>
        public FxRatesProvider RatesProvider => m_RatesProvider;
        #endregion

        #region Constructors
        private Engine(DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider)
        {
            m_CalculationCurrency = calculationCurrency;
            m_ValuationDate = valuationDate;
            m_RatesProvider = ratesProvider;

            List<Type> processorTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.IsSubclassOf(typeof(Processor)))
                .ToList();

            m_Processors = new List<Processor>(processorTypes.Count);

            foreach (Type processorType in processorTypes)
                m_Processors.Add((Processor)Activator.CreateInstance(processorType, valuationDate, calculationCurrency, ratesProvider));
        }
        #endregion

        #region Methods
        /// <summary>Calculates the total margin amount for the specified regulation role and entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="entities">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataEntity"/> objects representing the target entities.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object representing the total margin amount.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="entities">entities</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="entities">entities</paramref> contains elements defined on different regulations.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public Amount Calculate(RegulationRole regulationRole, ICollection<DataEntity> entities)
        {
            return CalculateDetailed(regulationRole, entities).Value;
        }

        /// <summary>Calculates the total margin amount for the specified regulation role and value entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object representing the total margin amount.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="values">values</paramref> contains elements defined on different regulations.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public Amount Calculate(RegulationRole regulationRole, ICollection<DataValue> values)
        {
            return CalculateDetailed(regulationRole, values, (new List<DataParameter>(0))).Value;
        }

        /// <summary>Calculates the total margin amount for the specified regulation role, value entities and parameter entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <param name="parameters">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataParameter"/> objects representing the target parameter entities.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object representing the total margin amount.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c> or when <paramref name="parameters">parameters</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="values">values</paramref> contains elements defined on different regulations or when <paramref name="parameters">parameters</paramref> contains elements defined on different regulations.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public Amount Calculate(RegulationRole regulationRole, ICollection<DataValue> values, ICollection<DataParameter> parameters)
        {
            return CalculateDetailed(regulationRole, values, parameters).Value;
        }

        /// <summary>Calculates the total margin amount for the specified regulation role and for the entities defined in the specified CRIF file.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="crifPath">The <see cref="T:System.String"/> representing the CRIF file that defines the target entities.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object representing the total margin amount.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="crifPath">crifPath</paramref> is invalid or does not refer to a CSV file.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the CRIF file contains entities defined on different regulations.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">Thrown when <paramref name="crifPath">crifPath</paramref> could not be found.</exception>
        /// <exception cref="T:System.IO.InvalidDataException">Thrown when the CRIF file contains invalid or malformed data.</exception>
        public Amount Calculate(RegulationRole regulationRole, String crifPath)
        {
            return CalculateDetailed(regulationRole, crifPath).Value;
        }

        /// <summary>Calculates the worst total margin amount among all the regulations for the specified regulation role and entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="entities">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataEntity"/> objects representing the target entities.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object representing the worst total margin amount among all the regulations.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="entities">entities</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public Amount CalculateWorstOf(RegulationRole regulationRole, ICollection<DataEntity> entities)
        {
            return CalculateDetailedWorstOf(regulationRole, entities).Value;
        }

        /// <summary>Calculates the worst total margin amount among all the regulations for the specified regulation role and value entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object representing the worst total margin amount among all the regulations.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public Amount CalculateWorstOf(RegulationRole regulationRole, ICollection<DataValue> values)
        {
            return CalculateDetailedWorstOf(regulationRole, values, (new List<DataParameter>(0))).Value;
        }

        /// <summary>Calculates the worst total margin amount among all the regulations for the specified regulation role, value entities and parameter entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <param name="parameters">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataParameter"/> objects representing the target parameter entities.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object representing the worst total margin amount among all the regulations.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c> or when <paramref name="parameters">parameters</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public Amount CalculateWorstOf(RegulationRole regulationRole, ICollection<DataValue> values, ICollection<DataParameter> parameters)
        {
            return CalculateDetailedWorstOf(regulationRole, values, parameters).Value;
        }

        /// <summary>Calculates the worst total margin amount among all the regulations for the specified regulation role and for the entities defined in the specified CRIF file.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="crifPath">The <see cref="T:System.String"/> representing the CRIF file that defines the target entities.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.Amount"/> object representing the worst total margin amount among all the regulations.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="crifPath">crifPath</paramref> is invalid or does not refer to a CSV file.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">Thrown when <paramref name="crifPath">crifPath</paramref> could not be found.</exception>
        /// <exception cref="T:System.IO.InvalidDataException">Thrown when the CRIF file contains invalid or malformed data.</exception>
        public Amount CalculateWorstOf(RegulationRole regulationRole, String crifPath)
        {
            return CalculateDetailedWorstOf(regulationRole, crifPath).Value;
        }

        /// <summary>Calculates the total margin amount of each regulation for the specified regulation role and entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="entities">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataEntity"/> objects representing the target entities.</param>
        /// <returns>An <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/> of pairs defined by <see cref="T:InitialMargin.Core.Regulation"/> keys and <see cref="T:InitialMargin.Core.Amount"/> values representing the regulation/amount couples.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="entities">entities</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public IDictionary<Regulation,Amount> CalculateByRole(RegulationRole regulationRole, ICollection<DataEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateByRole(regulationRole, values, parameters);
        }

        /// <summary>Calculates the total margin amount of each regulation for the specified regulation role and value entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <returns>An <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/> of pairs defined by <see cref="T:InitialMargin.Core.Regulation"/> keys and <see cref="T:InitialMargin.Core.Amount"/> values representing the regulation/amount couples.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public IDictionary<Regulation,Amount> CalculateByRole(RegulationRole regulationRole, ICollection<DataValue> values)
        {
            return CalculateByRole(regulationRole, values, (new List<DataParameter>(0)));
        }

        /// <summary>Calculates the total margin amount of each regulation for the specified regulation role, value entities and parameter entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <param name="parameters">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataParameter"/> objects representing the target parameter entities.</param>
        /// <returns>An <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/> of pairs defined by <see cref="T:InitialMargin.Core.Regulation"/> keys and <see cref="T:InitialMargin.Core.Amount"/> values representing the regulation/amount couples.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c> or when <paramref name="parameters">parameters</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public IDictionary<Regulation,Amount> CalculateByRole(RegulationRole regulationRole, ICollection<DataValue> values, ICollection<DataParameter> parameters)
        {
            IDictionary<Regulation,MarginTotal> result = CalculateDetailedByRole(regulationRole, values, parameters);
            return result.ToDictionary(x => x.Key, x => x.Value.Value);
        }

        /// <summary>Calculates the total margin amount of each regulation for the specified regulation role and for the entities defined in the specified CRIF file.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="crifPath">The <see cref="T:System.String"/> representing the CRIF file that defines the target entities.</param>
        /// <returns>An <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/> of pairs defined by <see cref="T:InitialMargin.Core.Regulation"/> keys and <see cref="T:InitialMargin.Core.Amount"/> values representing the regulation/amount couples.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="crifPath">crifPath</paramref> is invalid or does not refer to a CSV file.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">Thrown when <paramref name="crifPath">crifPath</paramref> could not be found.</exception>
        /// <exception cref="T:System.IO.InvalidDataException">Thrown when the CRIF file contains invalid or malformed data.</exception>
        public IDictionary<Regulation,Amount> CalculateByRole(RegulationRole regulationRole, String crifPath)
        {
            ICollection<DataEntity> entities = CrifManager.Read(crifPath);
            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateByRole(regulationRole, values, parameters);
        }

        /// <summary>Calculates the total margin of each regulation for the specified regulation role and entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="entities">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataEntity"/> objects representing the target entities.</param>
        /// <returns>An <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/> of pairs defined by <see cref="T:InitialMargin.Core.Regulation"/> keys and <see cref="T:InitialMargin.Core.Amount"/> values representing the regulation/margin couples.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="entities">entities</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public IDictionary<Regulation,MarginTotal> CalculateDetailedByRole(RegulationRole regulationRole, ICollection<DataEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            ICollection<DataValue> values = entities.OfType<DataValue>().ToList();
            ICollection<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailedByRole(regulationRole, values, parameters);
        }

        /// <summary>Calculates the total margin of each regulation for the specified regulation role and value entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <returns>An <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/> of pairs defined by <see cref="T:InitialMargin.Core.Regulation"/> keys and <see cref="T:InitialMargin.Core.Amount"/> values representing the regulation/margin couples.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public IDictionary<Regulation,MarginTotal> CalculateDetailedByRole(RegulationRole regulationRole, ICollection<DataValue> values)
        {
            return CalculateDetailedByRole(regulationRole, values, (new List<DataParameter>(0)));
        }

        /// <summary>Calculates the total margin of each regulation for the specified regulation role, value entities and parameter entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <param name="parameters">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataParameter"/> objects representing the target parameter entities.</param>
        /// <returns>An <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/> of pairs defined by <see cref="T:InitialMargin.Core.Regulation"/> keys and <see cref="T:InitialMargin.Core.Amount"/> values representing the regulation/margin couples.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c> or when <paramref name="parameters">parameters</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public IDictionary<Regulation,MarginTotal> CalculateDetailedByRole(RegulationRole regulationRole, ICollection<DataValue> values, ICollection<DataParameter> parameters)
        {
            if (!Enum.IsDefined(typeof(RegulationRole), regulationRole))
                throw new InvalidEnumArgumentException("Invalid regulation role specified.");

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

            Dictionary<Regulation,MarginTotal> result = new Dictionary<Regulation,MarginTotal>();

            foreach (Regulation regulation in regulations)
            {
                List<DataValue> valuesByRegulation = valuesFinal.Where(x => SelectRegulations(x, regulationRole).Contains(regulation)).ToList();
                List<DataParameter> parametersByRegulation = parametersFinal.Where(x => SelectRegulations(x, regulationRole).Contains(regulation)).ToList();

                List<IMargin> margins = new List<IMargin>(m_Processors.Count);

                foreach (Processor processor in m_Processors)
                    margins.Add(processor.Process(valuesByRegulation, parametersByRegulation));

                result[regulation] = MarginTotal.Of(regulationRole, regulation, m_ValuationDate, m_CalculationCurrency, margins);
            }

            return result;
        }

        /// <summary>Calculates the total margin of each regulation for the specified regulation role and for the entities defined in the specified CRIF file.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="crifPath">The <see cref="T:System.String"/> representing the CRIF file that defines the target entities.</param>
        /// <returns>An <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/> of pairs defined by <see cref="T:InitialMargin.Core.Regulation"/> keys and <see cref="T:InitialMargin.Core.Amount"/> values representing the regulation/margin couples.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="crifPath">crifPath</paramref> is invalid or does not refer to a CSV file.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">Thrown when <paramref name="crifPath">crifPath</paramref> could not be found.</exception>
        /// <exception cref="T:System.IO.InvalidDataException">Thrown when the CRIF file contains invalid or malformed data.</exception>
        public IDictionary<Regulation,MarginTotal> CalculateDetailedByRole(RegulationRole regulationRole, String crifPath)
        {
            ICollection<DataEntity> entities = CrifManager.Read(crifPath);
            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailedByRole(regulationRole, values, parameters);
        }

        /// <summary>Calculates the total margin for the specified regulation role and entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="entities">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataEntity"/> objects representing the target entities.</param>
        /// <returns>A <see cref="T:InitialMargin.Core.MarginTotal"/> object representing the total margin.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="entities">entities</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="entities">entities</paramref> contains elements defined on different regulations.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public MarginTotal CalculateDetailed(RegulationRole regulationRole, ICollection<DataEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailed(regulationRole, values, parameters);
        }

        /// <summary>Calculates the total margin for the specified regulation role and value entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <returns>A <see cref="T:InitialMargin.Core.MarginTotal"/> object representing the total margin.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="values">values</paramref> contains elements defined on different regulations.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public MarginTotal CalculateDetailed(RegulationRole regulationRole, ICollection<DataValue> values)
        {
            return CalculateDetailed(regulationRole, values, (new List<DataParameter>(0)));
        }

        /// <summary>Calculates the total margin for the specified regulation role, value entities and parameter entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <param name="parameters">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataParameter"/> objects representing the target parameter entities.</param>
        /// <returns>A <see cref="T:InitialMargin.Core.MarginTotal"/> object representing the total margin.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c> or when <paramref name="parameters">parameters</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when <paramref name="values">values</paramref> contains elements defined on different regulations or when <paramref name="parameters">parameters</paramref> contains elements defined on different regulations.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public MarginTotal CalculateDetailed(RegulationRole regulationRole, ICollection<DataValue> values, ICollection<DataParameter> parameters)
        {
            if (!Enum.IsDefined(typeof(RegulationRole), regulationRole))
                throw new InvalidEnumArgumentException("Invalid regulation role specified.");

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

            List<IMargin> margins = new List<IMargin>(m_Processors.Count);

            foreach (Processor processor in m_Processors)
                margins.Add(processor.Process(valuesFinal, parametersFinal));

            return MarginTotal.Of(regulationRole, regulations.Single(), m_ValuationDate, m_CalculationCurrency, margins);
        }

        /// <summary>Calculates the total margin for the specified regulation role and for the entities defined in the specified CRIF file.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="crifPath">The <see cref="T:System.String"/> representing the CRIF file that defines the target entities.</param>
        /// <returns>A <see cref="T:InitialMargin.Core.MarginTotal"/> object representing the total margin.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="crifPath">crifPath</paramref> is invalid or does not refer to a CSV file.</exception>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the CRIF file contains entities defined on different regulations.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">Thrown when <paramref name="crifPath">crifPath</paramref> could not be found.</exception>
        /// <exception cref="T:System.IO.InvalidDataException">Thrown when the CRIF file contains invalid or malformed data.</exception>
        public MarginTotal CalculateDetailed(RegulationRole regulationRole, String crifPath)
        {
            ICollection<DataEntity> entities = CrifManager.Read(crifPath);
            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailed(regulationRole, values, parameters);
        }
        
        /// <summary>Calculates the worst total margin among all the regulations for the specified regulation role and entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="entities">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataEntity"/> objects representing the target entities.</param>
        /// <returns>A <see cref="T:InitialMargin.Core.MarginTotal"/> object representing the worst total margin among all the regulations.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="entities">entities</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public MarginTotal CalculateDetailedWorstOf(RegulationRole regulationRole, ICollection<DataEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailedWorstOf(regulationRole, values, parameters);
        }

        /// <summary>Calculates the worst total margin among all the regulations for the specified regulation role and value entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <returns>A <see cref="T:InitialMargin.Core.MarginTotal"/> object representing the worst total margin among all the regulations.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public MarginTotal CalculateDetailedWorstOf(RegulationRole regulationRole, ICollection<DataValue> values)
        {
            return CalculateDetailedWorstOf(regulationRole, values, (new List<DataParameter>(0)));
        }

        /// <summary>Calculates the worst total margin among all the regulations for the specified regulation role, value entities and parameter entities.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="values">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataValue"/> objects representing the target value entities.</param>
        /// <param name="parameters">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.DataParameter"/> objects representing the target parameter entities.</param>
        /// <returns>A <see cref="T:InitialMargin.Core.MarginTotal"/> object representing the worst total margin among all the regulations.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values">values</paramref> is <c>null</c> or when <paramref name="parameters">parameters</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        public MarginTotal CalculateDetailedWorstOf(RegulationRole regulationRole, ICollection<DataValue> values, ICollection<DataParameter> parameters)
        {
            IDictionary<Regulation,MarginTotal> result = CalculateDetailedByRole(regulationRole, values, parameters);
            return result.Select(x => x.Value).OrderByDescending(x => Amount.Abs(x.Value)).ThenBy(x => x.Regulation).FirstOrDefault();
        }

        /// <summary>Calculates the worst total margin among all the regulations for the specified regulation role and for the entities defined in the specified CRIF file.</summary>
        /// <param name="regulationRole">An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> that specifies the target regulation role.</param>
        /// <param name="crifPath">The <see cref="T:System.String"/> representing the CRIF file that defines the target entities.</param>
        /// <returns>A <see cref="T:InitialMargin.Core.MarginTotal"/> object representing the worst total margin among all the regulations.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="crifPath">crifPath</paramref> is invalid or does not refer to a CSV file.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is undefined.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">Thrown when <paramref name="crifPath">crifPath</paramref> could not be found.</exception>
        /// <exception cref="T:System.IO.InvalidDataException">Thrown when the CRIF file contains invalid or malformed data.</exception>
        public MarginTotal CalculateDetailedWorstOf(RegulationRole regulationRole, String crifPath)
        {
            ICollection<DataEntity> entities = CrifManager.Read(crifPath);
            List<DataValue> values = entities.OfType<DataValue>().ToList();
            List<DataParameter> parameters = entities.OfType<DataParameter>().ToList();

            return CalculateDetailedWorstOf(regulationRole, values, parameters);
        }
        #endregion

        #region Methods (Static)
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

        /// <summary>Initializes and returns a new instance using the specified parameters and assuming that the valuation takes place today (see <see cref="P:System.DateTime.Today"/>).</summary>
        /// <param name="calculationCurrency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the calculation currency of the engine.</param>
        /// <param name="ratesProvider">The <see cref="T:InitialMargin.Core.FxRatesProvider"/> object supplying the rates.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Engine"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="calculationCurrency">calculationCurrency</paramref> is <c>null</c> or when <paramref name="ratesProvider">ratesProvider</paramref> is <c>null</c>.</exception>
        public static Engine Of(Currency calculationCurrency, FxRatesProvider ratesProvider)
        {
            return Of(DateTime.Today, calculationCurrency, ratesProvider);
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="valuationDate">The <see cref="T:System.DateTime"/> representing the valuation date of the engine.</param>
        /// <param name="calculationCurrency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the calculation currency of the engine.</param>
        /// <param name="ratesProvider">The <see cref="T:InitialMargin.Core.FxRatesProvider"/> object supplying the rates.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Engine"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="calculationCurrency">calculationCurrency</paramref> is <c>null</c> or when <paramref name="ratesProvider">ratesProvider</paramref> is <c>null</c>.</exception>
        public static Engine Of(DateTime valuationDate, Currency calculationCurrency, FxRatesProvider ratesProvider)
        {
            if (calculationCurrency == null)
                throw new ArgumentNullException(nameof(calculationCurrency));

            if (ratesProvider == null)
                throw new ArgumentNullException(nameof(ratesProvider));

            return (new Engine(valuationDate.Date, calculationCurrency, ratesProvider));
        }
        #endregion
    }
}