#region Using Directives
using System;
using System.Text.RegularExpressions;
#endregion

namespace InitialMargin.Core
{
    internal static class DataValidator
    {
        #region Members
        private static readonly Regex s_RegexCurrency = new Regex(@"^[A-Z]{3}$", RegexOptions.Compiled);
        private static readonly Regex s_RegexIsin = new Regex(@"^ISIN:[A-Z]{2}[A-Z0-9]{9}[0-9]{1}$", RegexOptions.Compiled);
        private static readonly Regex s_RegexLiborExternal = new Regex(@"^(Libor\d+)M$", RegexOptions.Compiled);
        private static readonly Regex s_RegexLiborInternal = new Regex(@"^(Libor\d+)m$", RegexOptions.Compiled);
        private static readonly Regex s_RegexNotionalQualifier = new Regex(@"^(?=.*[A-Z])[A-Z0-9]+$", (RegexOptions.Compiled | RegexOptions.IgnoreCase));
        private static readonly Regex s_RegexRegulationsBlank = new Regex(@"^(?: *|\[ ?\])$", RegexOptions.Compiled);
        private static readonly Regex s_RegexRegulationsFilled = new Regex(@"^[A-Z]{3,5}(?:,[A-Z]{3,5})*$", RegexOptions.Compiled);
        private static readonly Regex s_RegexTenor = new Regex(@"^([1-9][0-9]?)(w|m|y)$", RegexOptions.Compiled);
        private static readonly Regex s_RegexTradeReference = new Regex(@"^(?!.*?[-_]{2,}|.*?[-_]$)[A-Z0-9][A-Z0-9-_]*$", (RegexOptions.Compiled | RegexOptions.IgnoreCase));
        #endregion

        #region Methods
        public static Boolean BlankRegulations(String regulations)
        {
            if (String.IsNullOrWhiteSpace(regulations))
                return false;

            return s_RegexRegulationsBlank.IsMatch(regulations);
        }

        public static Boolean FilledRegulations(String regulations)
        {
            if (String.IsNullOrWhiteSpace(regulations))
                return false;

            return s_RegexRegulationsFilled.IsMatch(regulations);
        }

        public static Boolean IsValidCurrency(String currency)
        {
            if (String.IsNullOrWhiteSpace(currency))
                return false;

            return s_RegexCurrency.IsMatch(currency);
        }

        public static Boolean IsValidNotionalQualifier(String qualifier)
        {
            if (String.IsNullOrWhiteSpace(qualifier))
                return false;

            return s_RegexNotionalQualifier.IsMatch(qualifier);
        }

        public static Boolean IsValidQualifier(String qualifier, Boolean isinOnly)
        {
            if (String.IsNullOrWhiteSpace(qualifier))
                return false;

            if (isinOnly)
                return s_RegexIsin.IsMatch(qualifier);

            return (!qualifier.StartsWith("ISIN:", StringComparison.OrdinalIgnoreCase) || s_RegexIsin.IsMatch(qualifier));
        }

        public static Boolean IsValidTradeReference(String tradeReference)
        {
            if (String.IsNullOrWhiteSpace(tradeReference))
                return false;

            return s_RegexTradeReference.IsMatch(tradeReference);
        }

        public static Match MatchTenor(String tenor)
        {
            if (String.IsNullOrWhiteSpace(tenor))
                return null;

            return s_RegexTenor.Match(tenor);
        }

        public static String FormatLibor(String curve, Boolean toInternal)
        {
            if (String.IsNullOrWhiteSpace(curve))
                return curve;

            if (toInternal)
            {
                if (curve.Equals("OIS", StringComparison.Ordinal))
                    return "Ois";

                return s_RegexLiborInternal.Replace(curve, "$1M");
            }

            if (curve.Equals("Ois", StringComparison.Ordinal))
                return "OIS";

            return s_RegexLiborExternal.Replace(curve, "$1m");
        }
        #endregion
    }
}
