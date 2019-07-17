#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    /// <summary>Provides information about collect and post regulations, which is aimed at enriching <see cref="T:InitialMargin.Core.DataEntity"/> objects. This class cannot be derived.</summary>
    public sealed class RegulationsInfo
    {
        #region Members
        private readonly ReadOnlyCollection<Regulation> m_CollectRegulations;
        private readonly ReadOnlyCollection<Regulation> m_PostRegulations;
        #endregion

        #region Members (Static)
        /// <summary>Represents an empty <see cref="T:InitialMargin.Core.RegulationsInfo"/>. This field is read-only.</summary>
        public static readonly RegulationsInfo Empty = new RegulationsInfo((new List<Regulation>(0)), (new List<Regulation>(0)));
        #endregion

        #region Properties
        /// <summary>Gets the collect regulations.</summary>
        /// <value>A <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/> containing <c>0</c> or more <see cref="T:InitialMargin.Core.Regulation"/> values.</value>
        public ReadOnlyCollection<Regulation> CollectRegulations => m_CollectRegulations;

        /// <summary>Gets the post regulations.</summary>
        /// <value>A <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/> containing <c>0</c> or more <see cref="T:InitialMargin.Core.Regulation"/> values.</value>
        public ReadOnlyCollection<Regulation> PostRegulations => m_PostRegulations;
        #endregion

        #region Constructors
        private RegulationsInfo(ICollection<Regulation> collectRegulations, ICollection<Regulation> postRegulations)
        {
            m_CollectRegulations = collectRegulations.OrderBy(x => x).ToList().AsReadOnly();
            m_PostRegulations = postRegulations.OrderBy(x => x).ToList().AsReadOnly();
        }
        #endregion

        #region Methods
        /// <summary>Returns the text representation of the current instance.</summary>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public override String ToString()
        {
            String collect = String.Join(", ", m_CollectRegulations.Select(x => x.ToString().ToUpperInvariant()).OrderBy(x => x));
            String post = String.Join(", ", m_PostRegulations.Select(x => x.ToString().ToUpperInvariant()).OrderBy(x => x));

            return $"Regulations Collect=\"{collect}\" Post=\"{post}\"";
        }
        #endregion

        #region Methods (Static)
        internal static RegulationsInfo OfUnchecked(List<Regulation> collectRegulations, List<Regulation> postRegulations)
        {
            return (new RegulationsInfo(collectRegulations, postRegulations));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="collectRegulations">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.Regulation"/> values representing the collect regulations.</param>
        /// <param name="postRegulations">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.Regulation"/> values representing the post regulations.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.RegulationsInfo"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="collectRegulations">collectRegulations</paramref> contains undefined values or when <paramref name="postRegulations">postRegulations</paramref> contains undefined values.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="collectRegulations">collectRegulations</paramref> is <c>null</c> or when <paramref name="postRegulations">postRegulations</paramref> is <c>null</c>.</exception>
        public static RegulationsInfo Of(ICollection<Regulation> collectRegulations, ICollection<Regulation> postRegulations)
        {
            if (collectRegulations == null)
                throw new ArgumentNullException(nameof(collectRegulations));

            if (collectRegulations.Any(x => !Enum.IsDefined(typeof(Regulation), x)))
                throw new ArgumentException("One or more collect regulations are invalid.", nameof(collectRegulations));

            if (postRegulations == null)
                throw new ArgumentNullException(nameof(postRegulations));

            if (postRegulations.Any(x => !Enum.IsDefined(typeof(Regulation), x)))
                throw new ArgumentException("One or more post regulations are invalid.", nameof(collectRegulations));

            return (new RegulationsInfo(collectRegulations, postRegulations));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="regulation">The <see cref="T:InitialMargin.Core.Regulation"/> to use on both regulatory sides.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.RegulationsInfo"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="regulation">regulation</paramref> is undefined.</exception>
        public static RegulationsInfo Of(Regulation regulation)
        {
            return Of((new List<Regulation>(1) { regulation }), (new List<Regulation>(1) { regulation }));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="collectRegulation">The <see cref="T:InitialMargin.Core.Regulation"/> to use on collect side.</param>
        /// <param name="postRegulation">The <see cref="T:InitialMargin.Core.Regulation"/> to use on post side.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.RegulationsInfo"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="collectRegulation">collectRegulation</paramref> is undefined or when <paramref name="postRegulation">postRegulation</paramref> is undefined.</exception>
        public static RegulationsInfo Of(Regulation collectRegulation, Regulation postRegulation)
        {
            return Of((new List<Regulation>(1) { collectRegulation }), (new List<Regulation>(1) { postRegulation }));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="regulations">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.Regulation"/> values representing the collect regulations.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.RegulationsInfo"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="regulations">regulations</paramref> contains undefined values.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="regulations">regulations</paramref> is <c>null</c>.</exception>
        public static RegulationsInfo OfCollect(ICollection<Regulation> regulations)
        {
            return Of(regulations, (new List<Regulation>(0)));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="regulation">The <see cref="T:InitialMargin.Core.Regulation"/> to use on collect side.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.RegulationsInfo"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="regulation">regulation</paramref> is undefined.</exception>
        public static RegulationsInfo OfCollect(Regulation regulation)
        {
            return Of((new List<Regulation>(1) { regulation }), (new List<Regulation>(0)));
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="regulations">The <see cref="System.Collections.Generic.ICollection{T}"/> of <see cref="T:InitialMargin.Core.Regulation"/> values representing the post regulations.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.RegulationsInfo"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="regulations">regulations</paramref> contains undefined values.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="regulations">regulations</paramref> is <c>null</c>.</exception>
        public static RegulationsInfo OfPost(ICollection<Regulation> regulations)
        {
            return Of((new List<Regulation>(0)), regulations);
        }

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="regulation">The <see cref="T:InitialMargin.Core.Regulation"/> to use on post side.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.Core.RegulationsInfo"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="regulation">regulation</paramref> is undefined.</exception>
        public static RegulationsInfo OfPost(Regulation regulation)
        {
            return Of((new List<Regulation>(0)), (new List<Regulation>(1) { regulation }));
        }
        #endregion
    }
}
