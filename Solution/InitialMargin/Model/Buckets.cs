#region Using Directives
using System;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Model
{
    /// <summary>Specifies the bucket of a commodity sensitivity. This class cannot be derived.</summary>
    public sealed class BucketCommodity : Enumeration<BucketCommodity>, IBucket, IThresholdIdentifier
    {
        #region Properties
        /// <summary>Gets a value indicating whether the bucket is residual.</summary>
        /// <value><c>true</c> if the bucket is residual; otherwise, <c>false</c>.</value>
        public Boolean IsResidual => false;
        #endregion

        #region Constructors
        private BucketCommodity(String name, String description) : base(name, description) { }
        #endregion
 
        #region Values
        /// <summary>Represents the commodity bucket 1 (Coal). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket1 = new BucketCommodity("1", "Coal");

        /// <summary>Represents the commodity bucket 2 (Crude Oil). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket2 = new BucketCommodity("2", "Crude Oil");

        /// <summary>Represents the commodity bucket 3 (Light Ends). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket3 = new BucketCommodity("3", "Light Ends");

        /// <summary>Represents the commodity bucket 4 (Middle Distillates). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket4 = new BucketCommodity("4", "Middle Distillates");

        /// <summary>Represents the commodity bucket 5 (Heavy Distillates). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket5 = new BucketCommodity("5", "Heavy Distillates");

        /// <summary>Represents the commodity bucket 6 (North America Natural Gas). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket6 = new BucketCommodity("6", "North America Natural Gas");

        /// <summary>Represents the commodity bucket 7 (European Natural Gas). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket7 = new BucketCommodity("7", "European Natural Gas");

        /// <summary>Represents the commodity bucket 8 (North American Power). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket8 = new BucketCommodity("8", "North American Power");

        /// <summary>Represents the commodity bucket 9 (European Power). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket9 = new BucketCommodity("9", "European Power");

        /// <summary>Represents the commodity bucket 10 (Freight). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket10 = new BucketCommodity("10", "Freight");

        /// <summary>Represents the commodity bucket 11 (Base Metals). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket11 = new BucketCommodity("11", "Base Metals");

        /// <summary>Represents the commodity bucket 12 (Precious Metals). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket12 = new BucketCommodity("12", "Precious Metals");

        /// <summary>Represents the commodity bucket 13 (Grains). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket13 = new BucketCommodity("13", "Grains");

        /// <summary>Represents the commodity bucket 14 (Softs). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket14 = new BucketCommodity("14", "Softs");

        /// <summary>Represents the commodity bucket 15 (Livestock). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket15 = new BucketCommodity("15", "Livestock");

        /// <summary>Represents the commodity bucket 16 (Other). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket16 = new BucketCommodity("16", "Other");

        /// <summary>Represents the commodity bucket 17 (Indices). This field is read-only.</summary>
        public static readonly BucketCommodity Bucket17 = new BucketCommodity("17", "Indices");
        #endregion
    }
 
    /// <summary>Specifies the bucket of a non-qualifying credit sensitivity. This class cannot be derived.</summary>
    public sealed class BucketCreditNonQualifying : Enumeration<BucketCreditNonQualifying>, IBucket, IThresholdIdentifier
    {
        #region Members
        private readonly Boolean m_IsResidual;
        #endregion

        #region Properties
        /// <summary>Gets a value indicating whether the bucket is residual.</summary>
        /// <value><c>true</c> if the bucket is residual; otherwise, <c>false</c>.</value>
        public Boolean IsResidual => m_IsResidual;
        #endregion

        #region Constructors
        private BucketCreditNonQualifying(String name, String description, Boolean isResidual) : base(name, description)
        {
            m_IsResidual = isResidual;
        }
        #endregion
 
        #region Values
        /// <summary>Represents the non-qualifying credit bucket 1 (Investment Grade RMBS/CMBS). This field is read-only.</summary>
        public static readonly BucketCreditNonQualifying Bucket1 = new BucketCreditNonQualifying("1", "Investment Grade RMBS/CMBS", false);
        
        /// <summary>Represents the non-qualifying credit bucket 2 (High Yield &amp; Non-rated RMBS/CMBS). This field is read-only.</summary>
        public static readonly BucketCreditNonQualifying Bucket2 = new BucketCreditNonQualifying("2", "High Yield & Non-rated RMBS/CMBS", false);
        
        /// <summary>Represents the residual non-qualifying credit bucket. This field is read-only.</summary>
        public static readonly BucketCreditNonQualifying BucketResidual = new BucketCreditNonQualifying("Residual", "Residual", true);
        #endregion
    }
 
    /// <summary>Specifies the bucket of a qualifying credit sensitivity. This class cannot be derived.</summary>
    public sealed class BucketCreditQualifying : Enumeration<BucketCreditQualifying>, IBucket, IThresholdIdentifier
    {
        #region Members
        private readonly Boolean m_IsResidual;
        #endregion

        #region Properties
        /// <summary>Gets a value indicating whether the bucket is residual.</summary>
        /// <value><c>true</c> if the bucket is residual; otherwise, <c>false</c>.</value>
        public Boolean IsResidual => m_IsResidual;
        #endregion

        #region Constructors
        private BucketCreditQualifying(String name, String description, Boolean isResidual) : base(name, description)
        {
            m_IsResidual = isResidual;
        }
        #endregion
 
        #region Values
        /// <summary>Represents the qualifying credit bucket 1 ("Investment Grade - Sovereigns &amp; Central Banks"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket1 = new BucketCreditQualifying("1", "Investment Grade - Sovereigns & Central Banks", false);

        /// <summary>Represents the qualifying credit bucket 2 ("Investment Grade - Financials"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket2 = new BucketCreditQualifying("2", "Investment Grade - Financials", false);

        /// <summary>Represents the qualifying credit bucket 3 ("Investment Grade - Basic Materials, Energy &amp; Industrials"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket3 = new BucketCreditQualifying("3", "Investment Grade - Basic Materials, Energy & Industrials", false);

        /// <summary>Represents the qualifying credit bucket 4 ("Investment Grade - Consumer Goods"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket4 = new BucketCreditQualifying("4", "Investment Grade - Consumer Goods", false);

        /// <summary>Represents the qualifying credit bucket 5 ("Investment Grade - Technology &amp; Telecommunications"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket5 = new BucketCreditQualifying("5", "Investment Grade - Technology & Telecommunications", false);

        /// <summary>Represents the qualifying credit bucket 6 ("Investment Grade - Health Care, Utilities &amp; Parastatals"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket6 = new BucketCreditQualifying("6", "Investment Grade - Health Care, Utilities & Parastatals", false);

        /// <summary>Represents the qualifying credit bucket 7 ("High Yield &amp; Non-rated - Sovereigns &amp; Central Banks"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket7 = new BucketCreditQualifying("7", "High Yield & Non-rated - Sovereigns & Central Banks", false);

        /// <summary>Represents the qualifying credit bucket 8 ("High Yield &amp; Non-rated - Financials"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket8 = new BucketCreditQualifying("8", "High Yield & Non-rated - Financials", false);

        /// <summary>Represents the qualifying credit bucket 9 ("High Yield &amp; Non-rated - Basic Materials, Energy &amp; Industrials"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket9 = new BucketCreditQualifying("9", "High Yield & Non-rated - Basic Materials, Energy & Industrials", false);

        /// <summary>Represents the qualifying credit bucket 10 ("High Yield &amp; Non-rated - Consumer Goods"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket10 = new BucketCreditQualifying("10", "High Yield & Non-rated - Consumer Goods", false);

        /// <summary>Represents the qualifying credit bucket 11 ("High Yield &amp; Non-rated - Technology &amp; Telecommunications"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket11 = new BucketCreditQualifying("11", "High Yield & Non-rated - Technology & Telecommunications", false);

        /// <summary>Represents the qualifying credit bucket 12 ("High Yield &amp; Non-rated - Health Care, Utilities &amp; Parastatals"). This field is read-only.</summary>
        public static readonly BucketCreditQualifying Bucket12 = new BucketCreditQualifying("12", "High Yield & Non-rated - Health Care, Utilities & Parastatals", false);

        /// <summary>Represents the residual qualifying credit bucket. This field is read-only.</summary>
        public static readonly BucketCreditQualifying BucketResidual = new BucketCreditQualifying("Residual", "Residual", true);
        #endregion
    }
 
    /// <summary>Specifies the bucket of an equity sensitivity. This class cannot be derived.</summary>
    public sealed class BucketEquity : Enumeration<BucketEquity>, IBucket, IThresholdIdentifier
    {
        #region Members
        private readonly Boolean m_IsResidual;
        #endregion

        #region Properties
        /// <summary>Gets a value indicating whether the bucket is residual.</summary>
        /// <value><c>true</c> if the bucket is residual; otherwise, <c>false</c>.</value>
        public Boolean IsResidual => m_IsResidual;
        #endregion

        #region Constructors
        private BucketEquity(String name, String description, Boolean isResidual) : base(name, description)
        {
            m_IsResidual = isResidual;
        }
        #endregion
 
        #region Values
        /// <summary>Represents the equity bucket 1 ("Large Cap - Emerging Markets - Consumer Goods, Services &amp; Utilities"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket1 = new BucketEquity("1", "Large Cap - Emerging Markets - Consumer Goods, Services & Utilities", false);

        /// <summary>Represents the equity bucket 2 ("Large Cap - Emerging Markets - Industrials &amp; Telecommunications"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket2 = new BucketEquity("2", "Large Cap - Emerging Markets - Industrials & Telecommunications", false);

        /// <summary>Represents the equity bucket 3 ("Large Cap - Emerging Markets - Agriculture, Basic Materials, Energy &amp; Manufacturing"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket3 = new BucketEquity("3", "Large Cap - Emerging Markets - Agriculture, Basic Materials, Energy & Manufacturing", false);

        /// <summary>Represents the equity bucket 4 ("Large Cap - Emerging Markets - Financials"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket4 = new BucketEquity("4", "Large Cap - Emerging Markets - Financials", false);

        /// <summary>Represents the equity bucket 5 ("Large Cap - Developed Markets - Consumer Goods, Services &amp; Utilities"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket5 = new BucketEquity("5", "Large Cap - Developed Markets - Consumer Goods, Services & Utilities", false);

        /// <summary>Represents the equity bucket 6 ("Large Cap - Developed Markets - Industrials &amp; Telecommunications"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket6 = new BucketEquity("6", "Large Cap - Developed Markets - Industrials & Telecommunications", false);

        /// <summary>Represents the equity bucket 7 ("Large Cap - Developed Markets - Agriculture, Basic Materials, Energy &amp; Manufacturing"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket7 = new BucketEquity("7", "Large Cap - Developed Markets - Agriculture, Basic Materials, Energy & Manufacturing", false);

        /// <summary>Represents the equity bucket 8 ("Large Cap - Developed Markets - Financials"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket8 = new BucketEquity("8", "Large Cap - Developed Markets - Financials", false);

        /// <summary>Represents the equity bucket 9 ("Small Cap - Emerging Markets - All Sectors"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket9 = new BucketEquity("9", "Small Cap - Emerging Markets - All Sectors", false);

        /// <summary>Represents the equity bucket 10 ("Small Cap - Developed Markets - All Sectors"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket10 = new BucketEquity("10", "Small Cap - Developed Markets - All Sectors", false);

        /// <summary>Represents the equity bucket 11 ("Indices, Funds &amp; ETFs"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket11 = new BucketEquity("11", "Indices, Funds & ETFs", false);

        /// <summary>Represents the equity bucket 12 ("Volatility Indices"). This field is read-only.</summary>
        public static readonly BucketEquity Bucket12 = new BucketEquity("12", "Volatility Indices", false);

        /// <summary>Represents the residual equity bucket. This field is read-only.</summary>
        public static readonly BucketEquity BucketResidual = new BucketEquity("Residual", "Residual", true);
        #endregion
    }
}