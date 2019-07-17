namespace InitialMargin.Schedule
{
    /// <summary>Specifies the product within the Schedule IM context.</summary>
    public enum Product
    {
        #region Values
        /// <summary>The commodity product.</summary>
        Commodity = 0,
        /// <summary>The credit product.</summary>
        Credit = 1,
        /// <summary>The equity product.</summary>
        Equity = 2,
        /// <summary>The foreign exchange product.</summary>
        Fx = 3,
        /// <summary>The interest rate product.</summary>
        Rates = 4,
        /// <summary>The residual category of products.</summary>
        Other = 5
        #endregion
    }
}