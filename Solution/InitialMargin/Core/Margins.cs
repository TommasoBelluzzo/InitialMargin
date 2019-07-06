#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    public abstract class Margin<T> : IMargin where T : IMargin
    {
        #region Members
        private readonly Amount m_Amount;
        private readonly Int32 m_Level;
        private readonly ReadOnlyCollection<IMargin> m_Children;
        private readonly String m_Identifier;
        private readonly String m_Name;
        #endregion

        #region Properties
        public Amount Value => m_Amount;

        public Int32 ChildrenCount => m_Children.Count;

        public Int32 Level => m_Level;

        public ReadOnlyCollection<IMargin> Children => m_Children;

        public String Identifier => m_Identifier;

        public String Name => m_Name;
        #endregion

        #region Constructors
        protected Margin(Int32 level, String name, String identifier, Amount amount, List<T> children)
        {
            if (level <= 0)
                throw new ArgumentOutOfRangeException(nameof(level), "Invalid level specified, it must be a positive integer.");

            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid name specified.", nameof(name));

            if (String.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Invalid identifier specified.", nameof(identifier));

            if (children == null)
                throw new ArgumentNullException(nameof(children));

            m_Amount = amount;
            m_Level = level;
            m_Children = children.Cast<IMargin>().ToList().AsReadOnly();
            m_Identifier = identifier;
            m_Name = name;
        }

        protected Margin(Int32 level, String name, String identifier, Amount amount) : this(level, name, identifier, amount, (new List<T>(0))) { }
        #endregion

        #region Methods
        public override String ToString()
        {
            return $"{m_Level}) {m_Name} \"{m_Identifier}\" Amount=\"{m_Amount}\" Children=\"{m_Children.Count}\"";
        }
        #endregion
    }

    public sealed class MarginTotal : Margin<IMargin>
    {
        #region Members
        private readonly Currency m_CalculationCurrency;
        private readonly DateTime m_ValuationDate;
        private readonly Regulation? m_Regulation;
        private readonly RegulationRole? m_RegulationRole;
        #endregion

        #region Properties
        public Currency CalculationCurrency => m_CalculationCurrency;

        public DateTime ValuationDate => m_ValuationDate;

        public Regulation? Regulation => m_Regulation;

        public RegulationRole? RegulationRole => m_RegulationRole;
        #endregion

        #region Constructors
        private MarginTotal(RegulationRole? regulationRole, Regulation? regulation, DateTime valuationDate, Currency calculationCurrency, Amount amount, List<IMargin> children) : base(1, "Total", "Total", amount, children)
        {
            m_CalculationCurrency = calculationCurrency;
            m_ValuationDate = valuationDate;
            m_Regulation = regulation;
            m_RegulationRole = regulationRole;
        }
        #endregion

        #region Methods (Static)
        public static MarginTotal Of(DateTime valuationDate, Currency calculationCurrency, params IMargin[] children)
        {
            return Of(null, null, valuationDate, calculationCurrency, children.ToList());
        }

        public static MarginTotal Of(DateTime valuationDate, Currency calculationCurrency, List<IMargin> children)
        {
            return Of(null, null, valuationDate, calculationCurrency, children);
        }

        public static MarginTotal Of(RegulationRole? regulationRole, Regulation? regulation, DateTime valuationDate, Currency calculationCurrency, params IMargin[] children)
        {
            return Of(regulationRole, regulation, valuationDate, calculationCurrency, children.ToList());
        }

        public static MarginTotal Of(RegulationRole? regulationRole, Regulation? regulation, DateTime valuationDate, Currency calculationCurrency, List<IMargin> children)
        {
            if (regulationRole.HasValue && !Enum.IsDefined(typeof(RegulationRole), regulationRole))
                throw new InvalidEnumArgumentException("Invalid regulation role specified.");

            if (regulation.HasValue && !Enum.IsDefined(typeof(Regulation), regulation))
                throw new InvalidEnumArgumentException("Invalid regulation specified.");

            if ((regulationRole.HasValue && !regulation.HasValue) || (!regulationRole.HasValue && regulation.HasValue))
                throw new ArgumentException("Regulation role and regulation must be either both null or both not null.");
            
            if (calculationCurrency == null)
                throw new ArgumentNullException(nameof(calculationCurrency));

            if (children == null)
                throw new ArgumentNullException(nameof(children));

            Amount amount = Amount.Zero(calculationCurrency);
            List<IMargin> childrenFinal = new List<IMargin>();

            foreach (IMargin child in children)
            {
                List<IMargin> marginChildren;

                if (child is MarginTotal)
                    marginChildren = child.Children.ToList();
                else
                    marginChildren = new List<IMargin> { child };

                childrenFinal.AddRange(marginChildren);

                amount += Amount.Sum(marginChildren.Select(x => x.Value), calculationCurrency);
            }

            if (regulationRole.HasValue && (regulationRole == Core.RegulationRole.Pledgor))
                amount = -amount;

            childrenFinal = childrenFinal.OrderBy(x => x.Identifier.ToUpperInvariant()).ToList();

            return (new MarginTotal(regulationRole, regulation, valuationDate, calculationCurrency, amount, childrenFinal));
        }
        #endregion
    }
}