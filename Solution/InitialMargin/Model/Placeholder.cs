#region Using Directives
using System;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Model
{
    internal sealed class Placeholder : IBucket, IThresholdIdentifier
    {
        #region Members (Static)
        private static readonly Lazy<Placeholder> s_Instance = new Lazy<Placeholder> (() => new Placeholder());
        #endregion

        #region Properties
        public Boolean IsResidual => false;

        public String Description => "Unused";

        public String Name => "Unused";
        #endregion

        #region Properties (Static)
        public static Placeholder Instance => s_Instance.Value;
        #endregion

        #region Constructors
        private Placeholder() { }
        #endregion

        #region Methods
        public override String ToString()
        {
            return Name;
        }
        #endregion
    }
}