#region Using Directives
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using InitialMargin.Core;
using Calendar = InitialMargin.Core.Calendar;
#endregion

namespace InitialMargin.Model
{
    public sealed class Tenor : Enumeration<Tenor>
    {
        #region Members
        private readonly Boolean m_IsCreditTenor;
        private readonly Decimal m_Days;
        private readonly Int32 m_Value;
        private readonly String m_Period;
        #endregion

        #region Properties
        public Boolean IsCreditTenor => m_IsCreditTenor;

        public Decimal Days => m_Days;

        public Int32 Value => m_Value;

        public String Period => m_Period;
        #endregion

        #region Constructors
        private Tenor(String name, String description, Boolean isCreditTenor) : base(name, description)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Match m = DataValidator.MatchTenor(name);

            if ((m == null) || !m.Success)
                throw new ArgumentException("Invalid name specified.", nameof(name));

            if (description == null)
                throw new ArgumentNullException(nameof(description));

            m_IsCreditTenor = isCreditTenor;
            m_Value = Int32.Parse(m.Groups[1].Value, CultureInfo.CurrentCulture);
            m_Period = m.Groups[2].Value;

            switch (m_Period)
            {
                case "w":
                    m_Days = Calendar.DaysPerWeek * m_Value;
                    break;

                case "m":
                    m_Days = Calendar.DaysPerMonth * m_Value;
                    break;

                default:
                    m_Days = Calendar.DaysPerYear * m_Value;
                    break;
            }
        }
        #endregion

        #region Values
        public static readonly Tenor W2 = new Tenor("2w", "2 Weeks Tenor", false);
        public static readonly Tenor M1 = new Tenor("1m", "1 Month Tenor", false);
        public static readonly Tenor M3 = new Tenor("3m", "3 Months Tenor", false);
        public static readonly Tenor M6 = new Tenor("6m", "6 Months Tenor", false);
        public static readonly Tenor Y1 = new Tenor("1y", "1 Year Tenor", true);
        public static readonly Tenor Y2 = new Tenor("2y", "2 Years Tenor", true);
        public static readonly Tenor Y3 = new Tenor("3y", "3 Years Tenor", true);
        public static readonly Tenor Y5 = new Tenor("5y", "5 Years Tenor", true);
        public static readonly Tenor Y10 = new Tenor("10y", "10 Years Tenor", true);
        public static readonly Tenor Y15 = new Tenor("15y", "15 Years Tenor", false);
        public static readonly Tenor Y20 = new Tenor("20y", "20 Years Tenor", false);
        public static readonly Tenor Y30 = new Tenor("30y", "30 Years Tenor", false);
        #endregion
    }
}