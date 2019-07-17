#region Using Directives
using System;
using System.Collections.ObjectModel;
#endregion

namespace InitialMargin.Core
{
    /// <summary>Defines the contract of a bucket.</summary>
    public interface IBucket
    {
        #region Properties
        /// <summary>Gets a value indicating whether the bucket is residual.</summary>
        /// <value><c>true</c> if the bucket is residual; otherwise, <c>false</c>.</value>
        Boolean IsResidual { get; }

        /// <summary>Gets the description of the bucket.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        String Description { get; }

        /// <summary>Gets the name of the bucket.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        String Name { get; }
        #endregion
    }

    /// <summary>Defines the contract of a margin.</summary>
    public interface IMargin
    {
        #region Properties
        /// <summary>Gets the amount of the margin.</summary>
        /// <value>An <see cref="T:InitialMargin.Core.Amount"/> object.</value>
        Amount Value { get; }

        /// <summary>Gets the number of margin children.</summary>
        /// <value>An <see cref="T:System.Int32"/> value.</value>
        Int32 ChildrenCount { get; }

        /// <summary>Gets the hierarchical level of the margin.</summary>
        /// <value>An <see cref="T:System.Int32"/> value.</value>
        Int32 Level { get; }

        /// <summary>Gets the children of the margin.</summary>
        /// <value>A <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/> of <see cref="T:InitialMargin.Core.IMargin"/> objects.</value>
        ReadOnlyCollection<IMargin> Children { get; }

        /// <summary>Gets the identifier of the margin.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        String Identifier { get; }

        /// <summary>Gets the name of the margin.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        String Name { get; }
        #endregion
    }

    /// <summary>Defines the contract of a threshold identifier.</summary>
    public interface IThresholdIdentifier
    {
        #region Properties
        /// <summary>Gets the description of the threshold identifier.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        String Description { get; }

        /// <summary>Gets the name of the threshold identifier.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        String Name { get; }
        #endregion
    }
}