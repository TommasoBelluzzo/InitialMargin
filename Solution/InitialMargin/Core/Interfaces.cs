#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion

namespace InitialMargin.Core
{
    public interface IBucket
    {
        #region Properties
        Boolean IsResidual { get; }
        String Description { get; }
        String Name { get; }
        #endregion
    }

    public interface IMargin
    {
        #region Properties
        Amount Value { get; }
        Int32 ChildrenCount { get; }
        Int32 Level { get; }
        ReadOnlyCollection<IMargin> Children { get; }
        String Identifier { get; }
        String Name { get; }
        #endregion
    }

    public interface IProcessor
    {
        #region Properties
        Currency CalculationCurrency { get; }
        DateTime ValuationDate { get; }
        FxRatesProvider RatesProvider { get; }
        #endregion

        #region Methods
        MarginTotal Process(List<DataValue> values, List<DataParameter> parameters);
        #endregion
    }

    public interface IThresholdIdentifier
    {
        #region Properties
        String Description { get; }
        String Name { get; }
        #endregion
    }
}