#region Using Directives
using System;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Model
{
    public sealed class BucketCommodity : Enumeration<BucketCommodity>, IBucket, IThresholdIdentifier
    {
        #region Properties
        public Boolean IsResidual => false;
        #endregion

        #region Constructors
        private BucketCommodity(String name, String description) : base(name, description) { }
        #endregion
 
        #region Values
        public static readonly BucketCommodity Bucket1 = new BucketCommodity("1", "Coal");
        public static readonly BucketCommodity Bucket2 = new BucketCommodity("2", "Crude Oil");
        public static readonly BucketCommodity Bucket3 = new BucketCommodity("3", "Light Ends");
        public static readonly BucketCommodity Bucket4 = new BucketCommodity("4", "Middle Distillates");
        public static readonly BucketCommodity Bucket5 = new BucketCommodity("5", "Heavy Distillates");
        public static readonly BucketCommodity Bucket6 = new BucketCommodity("6", "North America Natural Gas");
        public static readonly BucketCommodity Bucket7 = new BucketCommodity("7", "European Natural Gas");
        public static readonly BucketCommodity Bucket8 = new BucketCommodity("8", "North American Power");
        public static readonly BucketCommodity Bucket9 = new BucketCommodity("9", "European Power");
        public static readonly BucketCommodity Bucket10 = new BucketCommodity("10", "Freight");
        public static readonly BucketCommodity Bucket11 = new BucketCommodity("11", "Base Metals");
        public static readonly BucketCommodity Bucket12 = new BucketCommodity("12", "Precious Metals");
        public static readonly BucketCommodity Bucket13 = new BucketCommodity("13", "Grains");
        public static readonly BucketCommodity Bucket14 = new BucketCommodity("14", "Softs");
        public static readonly BucketCommodity Bucket15 = new BucketCommodity("15", "Livestock");
        public static readonly BucketCommodity Bucket16 = new BucketCommodity("16", "Other");
        public static readonly BucketCommodity Bucket17 = new BucketCommodity("17", "Indices");
        #endregion
    }
 
    public sealed class BucketCreditNonQualifying : Enumeration<BucketCreditNonQualifying>, IBucket, IThresholdIdentifier
    {
        #region Members
        private readonly Boolean m_IsResidual;
        #endregion

        #region Properties
        public Boolean IsResidual => m_IsResidual;
        #endregion

        #region Constructors
        private BucketCreditNonQualifying(String name, String description, Boolean isResidual) : base(name, description)
        {
            m_IsResidual = isResidual;
        }
        #endregion
 
        #region Values
        public static readonly BucketCreditNonQualifying Bucket1 = new BucketCreditNonQualifying("1", "Investment Grade RMBS/CMBS", false);
        public static readonly BucketCreditNonQualifying Bucket2 = new BucketCreditNonQualifying("2", "High Yield & Non-rated RMBS/CMBS", false);
        public static readonly BucketCreditNonQualifying BucketResidual = new BucketCreditNonQualifying("Residual", "Residual", true);
        #endregion
    }
 
    public sealed class BucketCreditQualifying : Enumeration<BucketCreditQualifying>, IBucket, IThresholdIdentifier
    {
        #region Members
        private readonly Boolean m_IsResidual;
        #endregion

        #region Properties
        public Boolean IsResidual => m_IsResidual;
        #endregion

        #region Constructors
        private BucketCreditQualifying(String name, String description, Boolean isResidual) : base(name, description)
        {
            m_IsResidual = isResidual;
        }
        #endregion
 
        #region Values
        public static readonly BucketCreditQualifying Bucket1 = new BucketCreditQualifying("1", "Investment Grade - Sovereigns & Central Banks", false);
        public static readonly BucketCreditQualifying Bucket2 = new BucketCreditQualifying("2", "Investment Grade - Financials", false);
        public static readonly BucketCreditQualifying Bucket3 = new BucketCreditQualifying("3", "Investment Grade - Basic Materials, Energy & Industrials", false);
        public static readonly BucketCreditQualifying Bucket4 = new BucketCreditQualifying("4", "Investment Grade - Consumer Goods", false);
        public static readonly BucketCreditQualifying Bucket5 = new BucketCreditQualifying("5", "Investment Grade - Technology & Telecommunications", false);
        public static readonly BucketCreditQualifying Bucket6 = new BucketCreditQualifying("6", "Investment Grade - Health Care, Utilities & Parastatals", false);
        public static readonly BucketCreditQualifying Bucket7 = new BucketCreditQualifying("7", "High Yield & Non-rated - Sovereigns & Central Banks", false);
        public static readonly BucketCreditQualifying Bucket8 = new BucketCreditQualifying("8", "High Yield & Non-rated - Financials", false);
        public static readonly BucketCreditQualifying Bucket9 = new BucketCreditQualifying("9", "High Yield & Non-rated - Basic Materials, Energy & Industrials", false);
        public static readonly BucketCreditQualifying Bucket10 = new BucketCreditQualifying("10", "High Yield & Non-rated - Consumer Goods", false);
        public static readonly BucketCreditQualifying Bucket11 = new BucketCreditQualifying("11", "High Yield & Non-rated - Technology & Telecommunications", false);
        public static readonly BucketCreditQualifying Bucket12 = new BucketCreditQualifying("12", "High Yield & Non-rated - Health Care, Utilities & Parastatals", false);
        public static readonly BucketCreditQualifying BucketResidual = new BucketCreditQualifying("Residual", "Residual", true);
        #endregion
    }
 
    public sealed class BucketEquity : Enumeration<BucketEquity>, IBucket, IThresholdIdentifier
    {
        #region Members
        private readonly Boolean m_IsResidual;
        #endregion

        #region Properties
        public Boolean IsResidual => m_IsResidual;
        #endregion

        #region Constructors
        private BucketEquity(String name, String description, Boolean isResidual) : base(name, description)
        {
            m_IsResidual = isResidual;
        }
        #endregion
 
        #region Values
        public static readonly BucketEquity Bucket1 = new BucketEquity("1", "Large Cap - Emerging Markets - Consumer Goods, Services & Utilities", false);
        public static readonly BucketEquity Bucket2 = new BucketEquity("2", "Large Cap - Emerging Markets - Industrials & Telecommunications", false);
        public static readonly BucketEquity Bucket3 = new BucketEquity("3", "Large Cap - Emerging Markets - Agriculture, Basic Materials, Energy & Manufacturing", false);
        public static readonly BucketEquity Bucket4 = new BucketEquity("4", "Large Cap - Emerging Markets - Financials", false);
        public static readonly BucketEquity Bucket5 = new BucketEquity("5", "Large Cap - Developed Markets - Consumer Goods, Services & Utilities", false);
        public static readonly BucketEquity Bucket6 = new BucketEquity("6", "Large Cap - Developed Markets - Industrials & Telecommunications", false);
        public static readonly BucketEquity Bucket7 = new BucketEquity("7", "Large Cap - Developed Markets - Agriculture, Basic Materials, Energy & Manufacturing", false);
        public static readonly BucketEquity Bucket8 = new BucketEquity("8", "Large Cap - Developed Markets - Financials", false);
        public static readonly BucketEquity Bucket9 = new BucketEquity("9", "Small Cap - Emerging Markets - All Sectors", false);
        public static readonly BucketEquity Bucket10 = new BucketEquity("10", "Small Cap - Developed Markets - All Sectors", false);
        public static readonly BucketEquity Bucket11 = new BucketEquity("11", "Indices, Funds & ETFs", false);
        public static readonly BucketEquity Bucket12 = new BucketEquity("12", "Volatility Indices", false);
        public static readonly BucketEquity BucketResidual = new BucketEquity("Residual", "Residual", true);
        #endregion
    }
}