#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    /// <summary>Represents the generalized base class from which all the margins must derive. This class is abstract.</summary>
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
        /// <summary>Gets the amount of the margin.</summary>
        /// <value>An <see cref="T:InitialMargin.Core.Amount"/> object.</value>
        public Amount Value => m_Amount;

        /// <summary>Gets the number of margin children.</summary>
        /// <value>An <see cref="T:System.Int32"/> value.</value>
        public Int32 ChildrenCount => m_Children.Count;

        /// <summary>Gets the hierarchical level of the margin.</summary>
        /// <value>An <see cref="T:System.Int32"/> value.</value>
        public Int32 Level => m_Level;

        /// <summary>Gets the children of the margin.</summary>
        /// <value>A <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/> of <see cref="T:InitialMargin.Core.IMargin"/> objects.</value>
        public ReadOnlyCollection<IMargin> Children => m_Children;

        /// <summary>Gets the identifier of the margin.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Identifier => m_Identifier;

        /// <summary>Gets the name of the margin.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Name => m_Name;
        #endregion

        #region Constructors
        /// <summary>Represents the base constructor, for a margin with children, used by derived classes.</summary>
        /// <param name="level">The <see cref="T:System.Int32"/> value representing the hierarchical level of the margin.</param>
        /// <param name="name">The <see cref="T:System.String"/> represeting the name of the margin.</param>
        /// <param name="identifier">The <see cref="T:System.String"/> represeting the identifier of the margin.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> object representing the amount of the margin.</param>
        /// <param name="children">The <see cref="System.Collections.Generic.List{T}"/> of <see cref="T:InitialMargin.Core.IMargin"/> objects representing the margin children.</param>
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

        /// <summary>Represents the base constructor, for a margin without children, used by derived classes.</summary>
        /// <param name="level">The <see cref="T:System.Int32"/> value representing the hierarchical level of the margin.</param>
        /// <param name="name">The <see cref="T:System.String"/> represeting the name of the margin.</param>
        /// <param name="identifier">The <see cref="T:System.String"/> represeting the identifier of the margin.</param>
        /// <param name="amount">The <see cref="T:InitialMargin.Core.Amount"/> object representing the amount of the margin.</param>
        protected Margin(Int32 level, String name, String identifier, Amount amount) : this(level, name, identifier, amount, (new List<T>(0))) { }
        #endregion

        #region Methods
        /// <summary>Returns the text representation of the current instance.</summary>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public override String ToString()
        {
            return $"{m_Level}) {m_Name} \"{m_Identifier}\" Amount=\"{m_Amount}\" Children=\"{m_Children.Count}\"";
        }
        #endregion
    }

    /// <summary>Represents a total margin, which is the standard result of an engine calculation. This class cannot be derived.</summary>
    public sealed class MarginTotal : Margin<IMargin>
    {
        #region Members
        private readonly Currency m_CalculationCurrency;
        private readonly DateTime m_ValuationDate;
        private readonly Regulation? m_Regulation;
        private readonly RegulationRole? m_RegulationRole;
        #endregion

        #region Properties
        /// <summary>Gets the reference calculation currency of the margin.</summary>
        /// <value>A <see cref="T:InitialMargin.Core.Currency"/> object.</value>
        public Currency CalculationCurrency => m_CalculationCurrency;

        /// <summary>Gets the reference valuation date of the margin.</summary>
        /// <value>A <see cref="T:System.DateTime"/> without time component.</value>
        public DateTime ValuationDate => m_ValuationDate;

        /// <summary>Gets the reference regulation of the margin, if defined.</summary>
        /// <value>An enumerator value of type <see cref="T:InitialMargin.Core.Regulation"/> or <c>null</c>.</value>
        public Regulation? Regulation => m_Regulation;

        /// <summary>Gets the reference regulation role of the margin, if defined.</summary>
        /// <value>An enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> or <c>null</c>.</value>
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
        /// <summary>Initializes and returns a new total margin using the specified parameters.</summary>
        /// <param name="valuationDate">The <see cref="T:System.DateTime"/> representing the reference valuation date.</param>
        /// <param name="calculationCurrency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the reference calculation currency.</param>
        /// <param name="children">An <see cref="T:InitialMargin.Core.IMargin"/>[] representing the margin children</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.MarginTotal"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="calculationCurrency">calculationCurrency</paramref> is <c>null</c>.</exception>
        public static MarginTotal Of(DateTime valuationDate, Currency calculationCurrency, params IMargin[] children)
        {
            return Of(null, null, valuationDate, calculationCurrency, children.ToList());
        }

        /// <summary>Initializes and returns a new total margin using the specified parameters.</summary>
        /// <param name="valuationDate">The <see cref="T:System.DateTime"/> representing the reference valuation date.</param>
        /// <param name="calculationCurrency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the reference calculation currency.</param>
        /// <param name="children">An <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.IMargin"/> objects representing the margin children.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.MarginTotal"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="calculationCurrency">calculationCurrency</paramref> is <c>null</c> or when <paramref name="children">children</paramref> is <c>null</c>.</exception>
        public static MarginTotal Of(DateTime valuationDate, Currency calculationCurrency, ICollection<IMargin> children)
        {
            return Of(null, null, valuationDate, calculationCurrency, children);
        }

        /// <summary>Initializes and returns a new total margin using the specified parameters.</summary>
        /// <param name="regulationRole">An nullable enumerator value of type <see cref="T:InitialMargin.Core.RegulationRole"/> representing the reference regulation role, if defined.</param>
        /// <param name="regulation">An nullable enumerator value of type <see cref="T:InitialMargin.Core.Regulation"/> representing the reference regulation, if defined.</param>
        /// <param name="valuationDate">The <see cref="T:System.DateTime"/> representing the reference valuation date.</param>
        /// <param name="calculationCurrency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the reference calculation currency.</param>
        /// <param name="children">An <see cref="T:InitialMargin.Core.IMargin"/>[] representing the margin children</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.MarginTotal"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is <c>null</c> and <paramref name="regulation">regulation</paramref> is not <c>null</c> or when <paramref name="regulationRole">regulationRole</paramref> is not <c>null</c> and <paramref name="regulation">regulation</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="calculationCurrency">calculationCurrency</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> has an undefined value or when <paramref name="regulation">regulation</paramref> has an undefined value.</exception>
        public static MarginTotal Of(RegulationRole? regulationRole, Regulation? regulation, DateTime valuationDate, Currency calculationCurrency, params IMargin[] children)
        {
            return Of(regulationRole, regulation, valuationDate, calculationCurrency, children.ToList());
        }

        /// <summary>Initializes and returns a new total margin using the specified parameters.</summary>
        /// <param name="regulationRole"></param>
        /// <param name="regulation"></param>
        /// <param name="valuationDate">The <see cref="T:System.DateTime"/> representing the reference valuation date.</param>
        /// <param name="calculationCurrency">The <see cref="T:InitialMargin.Core.Currency"/> object representing the reference calculation currency.</param>
        /// <param name="children">An <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.IMargin"/> objects representing the margin children.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.MarginTotal"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> is <c>null</c> and <paramref name="regulation">regulation</paramref> is not <c>null</c> or when <paramref name="regulationRole">regulationRole</paramref> is not <c>null</c> and <paramref name="regulation">regulation</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="calculationCurrency">calculationCurrency</paramref> is <c>null</c> or when <paramref name="children">children</paramref> is <c>null</c>.</exception>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Thrown when <paramref name="regulationRole">regulationRole</paramref> has an undefined value or when <paramref name="regulation">regulation</paramref> has an undefined value.</exception>
        public static MarginTotal Of(RegulationRole? regulationRole, Regulation? regulation, DateTime valuationDate, Currency calculationCurrency, ICollection<IMargin> children)
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

            Amount amount = Amount.OfZero(calculationCurrency);
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