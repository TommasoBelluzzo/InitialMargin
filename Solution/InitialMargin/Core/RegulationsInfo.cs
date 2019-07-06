#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    public sealed class RegulationsInfo
    {
        #region Members
        private readonly ReadOnlyCollection<Regulation> m_CollectRegulations;
        private readonly ReadOnlyCollection<Regulation> m_PostRegulations;
        #endregion

        #region Members (Static)
        public static readonly RegulationsInfo Empty = new RegulationsInfo((new List<Regulation>(0)), (new List<Regulation>(0)));
        #endregion

        #region Properties
        public ReadOnlyCollection<Regulation> CollectRegulations => m_CollectRegulations;
        public ReadOnlyCollection<Regulation> PostRegulations => m_PostRegulations;
        #endregion

        #region Constructors
        private RegulationsInfo(List<Regulation> collectRegulations, List<Regulation> postRegulations)
        {
            m_CollectRegulations = collectRegulations.OrderBy(x => x).ToList().AsReadOnly();
            m_PostRegulations = postRegulations.OrderBy(x => x).ToList().AsReadOnly();
        }
        #endregion

        #region Methods
        public override String ToString()
        {
            String collect = String.Join(", ", m_CollectRegulations.Select(x => x.ToString().ToUpperInvariant()).OrderBy(x => x));
            String post = String.Join(", ", m_PostRegulations.Select(x => x.ToString().ToUpperInvariant()).OrderBy(x => x));

            return $"Regulations Collect=\"{collect}\" Post=\"{post}\"";
        }
        #endregion

        #region Methods (Static)
        public static RegulationsInfo Of(List<Regulation> collectRegulations, List<Regulation> postRegulations)
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

        public static RegulationsInfo Of(Regulation regulation)
        {
            return Of((new List<Regulation>(1) { regulation }), (new List<Regulation>(1) { regulation }));
        }

        public static RegulationsInfo Of(Regulation collectRegulation, Regulation postRegulation)
        {
            return Of((new List<Regulation>(1) { collectRegulation }), (new List<Regulation>(1) { postRegulation }));
        }

        public static RegulationsInfo OfCollect(List<Regulation> regulations)
        {
            return Of(regulations, (new List<Regulation>(0)));
        }

        public static RegulationsInfo OfCollect(Regulation regulation)
        {
            return Of((new List<Regulation>(1) { regulation }), (new List<Regulation>(0)));
        }

        public static RegulationsInfo OfPost(List<Regulation> regulations)
        {
            return Of((new List<Regulation>(0)), regulations);
        }

        public static RegulationsInfo OfPost(Regulation regulation)
        {
            return Of((new List<Regulation>(0)), (new List<Regulation>(1) { regulation }));
        }
        #endregion
    }
}
