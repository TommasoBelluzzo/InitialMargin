#region Using Directives
using System;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    public sealed class Currency : Enumeration<Currency>, IBucket, IThresholdIdentifier
    {
        #region Members
        private readonly CurrencyCategory m_Category;
        private readonly CurrencyLiquidity m_Liquidity;
        private readonly CurrencyVolatility m_Volatility;
        #endregion

        #region Members (Static)
        private static readonly String[] s_CategoryFrequentlyTraded =
        {
            "BRL", "CNY", "HKD", "INR", "KRW",
            "MXN", "NOK", "NZD", "RUB", "SEK",
            "SGD", "TRY", "ZAR"
        };

        private static readonly String[] s_CategorySignificantlyMaterial =
        {
            "AUD", "CAD", "CHF", "EUR", "GBP",
            "JPY", "USD"
        };

        private static readonly String[] s_LiquidityMedium =
        {
            "AUD", "CAD", "CHF", "DKK", "HKD",
            "KRW", "NOK", "NZD", "SEK", "SGD",
            "TWD"
        };

        private static readonly String[] s_LiquidityHigh =
        {
            "EUR", "GBP", "USD"
        };

        private static readonly String[] s_VolatilityLow =
        {
            "JPY"
        };

        private static readonly String[] s_VolatilityRegular = s_LiquidityMedium.Concat(s_LiquidityHigh).ToArray();
        #endregion

        #region Properties
        public Boolean IsResidual => false;

        public CurrencyCategory Category => m_Category;

        public CurrencyLiquidity Liquidity => m_Liquidity;

        public CurrencyVolatility Volatility => m_Volatility;
        #endregion

        #region Constructors
        private Currency(String name, String description) : base(name, description)
        {
            if (!DataValidator.IsValidCurrency(name))
                throw new ArgumentException("Invalid code specified.", nameof(name));

            if (s_CategoryFrequentlyTraded.Contains(name))
                m_Category = CurrencyCategory.FrequentlyTraded;
            else if (s_CategorySignificantlyMaterial.Contains(name))
                m_Category = CurrencyCategory.SignificantlyMaterial;
            else
                m_Category = CurrencyCategory.Other;

            if (s_VolatilityLow.Contains(name))
            {
                m_Liquidity = CurrencyLiquidity.Undefined;
                m_Volatility = CurrencyVolatility.Low;
            }
            else if (s_VolatilityRegular.Contains(name))
            {
                if (s_LiquidityHigh.Contains(name))
                    m_Liquidity = CurrencyLiquidity.High;
                else
                    m_Liquidity = CurrencyLiquidity.Medium;

                m_Volatility = CurrencyVolatility.Regular;
            }
            else
            {
                m_Liquidity = CurrencyLiquidity.Undefined;
                m_Volatility = CurrencyVolatility.High;
            }
        }
        #endregion

        #region Values
        public static readonly Currency Aed = new Currency("AED", "UAE Dirham");
        public static readonly Currency Afn = new Currency("AFN", "Afghan Afghani");
        public static readonly Currency All = new Currency("ALL", "Albanian Lek");
        public static readonly Currency Amd = new Currency("AMD", "Armenian Dram");
        public static readonly Currency Ang = new Currency("ANG", "Netherlands Antillean Guilder");
        public static readonly Currency Aoa = new Currency("AOA", "Angolan Kwanza");
        public static readonly Currency Ars = new Currency("ARS", "Argentine Peso");
        public static readonly Currency Aud = new Currency("AUD", "Australian Dollar");
        public static readonly Currency Awg = new Currency("AWG", "Aruban Florin");
        public static readonly Currency Azn = new Currency("AZN", "Azerbaijan Manat");
        public static readonly Currency Bam = new Currency("BAM", "Convertible Mark");
        public static readonly Currency Bbd = new Currency("BBD", "Barbados Dollar");
        public static readonly Currency Bdt = new Currency("BDT", "Bangladeshi Taka");
        public static readonly Currency Bgn = new Currency("BGN", "Bulgarian Lev");
        public static readonly Currency Bhd = new Currency("BHD", "Bahraini Dinar");
        public static readonly Currency Bif = new Currency("BIF", "Burundi Franc");
        public static readonly Currency Bmd = new Currency("BMD", "Bermudian Dollar");
        public static readonly Currency Bnd = new Currency("BND", "Brunei Dollar");
        public static readonly Currency Bob = new Currency("BOB", "Bolivian Boliviano");
        public static readonly Currency Brl = new Currency("BRL", "Brazilian Real");
        public static readonly Currency Bsd = new Currency("BSD", "Bahamian Dollar");
        public static readonly Currency Btn = new Currency("BTN", "Bhutanese Ngultrum");
        public static readonly Currency Bwp = new Currency("BWP", "Botswana Pula");
        public static readonly Currency Byn = new Currency("BYN", "Belarusian Ruble");
        public static readonly Currency Bzd = new Currency("BZD", "Belize Dollar");
        public static readonly Currency Cad = new Currency("CAD", "Canadian Dollar");
        public static readonly Currency Cdf = new Currency("CDF", "Congolese Franc");
        public static readonly Currency Chf = new Currency("CHF", "Swiss Franc");
        public static readonly Currency Clp = new Currency("CLP", "Chilean Peso");
        public static readonly Currency Cny = new Currency("CNY", "Yuan Renminbi");
        public static readonly Currency Cop = new Currency("COP", "Colombian Peso");
        public static readonly Currency Crc = new Currency("CRC", "Costa Rican Colon");
        public static readonly Currency Cup = new Currency("CUP", "Cuban Peso");
        public static readonly Currency Cve = new Currency("CVE", "Cabo Verde Escudo");
        public static readonly Currency Czk = new Currency("CZK", "Czech Koruna");
        public static readonly Currency Djf = new Currency("DJF", "Djibouti Franc");
        public static readonly Currency Dkk = new Currency("DKK", "Danish Krone");
        public static readonly Currency Dop = new Currency("DOP", "Dominican Peso");
        public static readonly Currency Dzd = new Currency("DZD", "Algerian Dinar");
        public static readonly Currency Egp = new Currency("EGP", "Egyptian Pound");
        public static readonly Currency Ern = new Currency("ERN", "Eritrean Nakfa");
        public static readonly Currency Etb = new Currency("ETB", "Ethiopian Birr");
        public static readonly Currency Eur = new Currency("EUR", "Euro");
        public static readonly Currency Fjd = new Currency("FJD", "Fiji Dollar");
        public static readonly Currency Fkp = new Currency("FKP", "Falkland Islands Pound");
        public static readonly Currency Gbp = new Currency("GBP", "Pound Sterling");
        public static readonly Currency Gel = new Currency("GEL", "Georgian Lari");
        public static readonly Currency Ghs = new Currency("GHS", "Ghana Cedi");
        public static readonly Currency Gip = new Currency("GIP", "Gibraltar Pound");
        public static readonly Currency Gmd = new Currency("GMD", "Gambian Dalasi");
        public static readonly Currency Gnf = new Currency("GNF", "Guinean Franc");
        public static readonly Currency Gtq = new Currency("GTQ", "Guatemalan Quetzal");
        public static readonly Currency Gyd = new Currency("GYD", "Guyana Dollar");
        public static readonly Currency Hkd = new Currency("HKD", "Hong Kong Dollar");
        public static readonly Currency Hnl = new Currency("HNL", "Honduran Lempira");
        public static readonly Currency Hrk = new Currency("HRK", "Croatian Kuna");
        public static readonly Currency Htg = new Currency("HTG", "Haitian Gourde");
        public static readonly Currency Huf = new Currency("HUF", "Hungarian Forint");
        public static readonly Currency Idr = new Currency("IDR", "Indonesian Rupiah");
        public static readonly Currency Ils = new Currency("ILS", "Israeli Sheqel");
        public static readonly Currency Inr = new Currency("INR", "Indian Rupee");
        public static readonly Currency Iqd = new Currency("IQD", "Iraqi Dinar");
        public static readonly Currency Irr = new Currency("IRR", "Iranian Rial");
        public static readonly Currency Isk = new Currency("ISK", "Iceland Krona");
        public static readonly Currency Jmd = new Currency("JMD", "Jamaican Dollar");
        public static readonly Currency Jod = new Currency("JOD", "Jordanian Dinar");
        public static readonly Currency Jpy = new Currency("JPY", "Japanese Yen");
        public static readonly Currency Kes = new Currency("KES", "Kenyan Shilling");
        public static readonly Currency Kgs = new Currency("KGS", "Kyrgyzstani Som");
        public static readonly Currency Khr = new Currency("KHR", "Omani Riel");
        public static readonly Currency Kmf = new Currency("KMF", "Comorian Franc ");
        public static readonly Currency Kpw = new Currency("KPW", "North Korean Won");
        public static readonly Currency Krw = new Currency("KRW", "South Korean Won");
        public static readonly Currency Kwd = new Currency("KWD", "Kuwaiti Dinar");
        public static readonly Currency Kyd = new Currency("KYD", "Cayman Islands Dollar");
        public static readonly Currency Kzt = new Currency("KZT", "Kazakhstani Tenge");
        public static readonly Currency Lak = new Currency("LAK", "Lao Kip");
        public static readonly Currency Lbp = new Currency("LBP", "Lebanese Pound");
        public static readonly Currency Lkr = new Currency("LKR", "Sri Lanka Rupee");
        public static readonly Currency Lrd = new Currency("LRD", "Liberian Dollar");
        public static readonly Currency Lsl = new Currency("LSL", "Basotho Loti");
        public static readonly Currency Lyd = new Currency("LYD", "Libyan Dinar");
        public static readonly Currency Mad = new Currency("MAD", "Moroccan Dirham");
        public static readonly Currency Mdl = new Currency("MDL", "Moldovan Leu");
        public static readonly Currency Mga = new Currency("MGA", "Malagasy Ariary");
        public static readonly Currency Mkd = new Currency("MKD", "Macedonian Denar");
        public static readonly Currency Mmk = new Currency("MMK", "Burmese Kyat");
        public static readonly Currency Mnt = new Currency("MNT", "Mongolian Tugrik");
        public static readonly Currency Mop = new Currency("MOP", "Macanese Pataca");
        public static readonly Currency Mru = new Currency("MRU", "Mauritanian Ouguiya");
        public static readonly Currency Mur = new Currency("MUR", "Mauritius Rupee");
        public static readonly Currency Mvr = new Currency("MVR", "Maldivian Rufiyaa");
        public static readonly Currency Mwk = new Currency("MWK", "Malawi Kwacha");
        public static readonly Currency Mxn = new Currency("MXN", "Mexican Peso");
        public static readonly Currency Myr = new Currency("MYR", "Malaysian Ringgit");
        public static readonly Currency Mzn = new Currency("MZN", "Mozambique Metical");
        public static readonly Currency Nad = new Currency("NAD", "Namibia Dollar");
        public static readonly Currency Ngn = new Currency("NGN", "Nigerian Naira");
        public static readonly Currency Nio = new Currency("NIO", "Nicaraguan Cordoba");
        public static readonly Currency Nok = new Currency("NOK", "Norwegian Krone");
        public static readonly Currency Npr = new Currency("NPR", "Nepalese Rupee");
        public static readonly Currency Nzd = new Currency("NZD", "New Zealand Dollar");
        public static readonly Currency Omr = new Currency("OMR", "Omani Rial");
        public static readonly Currency Pab = new Currency("PAB", "Panamanian Balboa");
        public static readonly Currency Pen = new Currency("PEN", "Peruvian Sol");
        public static readonly Currency Pgk = new Currency("PGK", "Papua New Guinean Kina");
        public static readonly Currency Php = new Currency("PHP", "Philippine Peso");
        public static readonly Currency Pkr = new Currency("PKR", "Pakistan Rupee");
        public static readonly Currency Pln = new Currency("PLN", "Polish Zloty");
        public static readonly Currency Pyg = new Currency("PYG", "Paraguayan Guarani");
        public static readonly Currency Qar = new Currency("QAR", "Qatari Rial");
        public static readonly Currency Ron = new Currency("RON", "Romanian Leu");
        public static readonly Currency Rsd = new Currency("RSD", "Serbian Dinar");
        public static readonly Currency Rub = new Currency("RUB", "Russian Ruble");
        public static readonly Currency Rwf = new Currency("RWF", "Rwanda Franc");
        public static readonly Currency Sar = new Currency("SAR", "Saudi Riyal");
        public static readonly Currency Sbd = new Currency("SBD", "Solomon Islands Dollar");
        public static readonly Currency Scr = new Currency("SCR", "Seychelles Rupee");
        public static readonly Currency Sdg = new Currency("SDG", "Sudanese Pound");
        public static readonly Currency Sek = new Currency("SEK", "Swedish Krona");
        public static readonly Currency Sgd = new Currency("SGD", "Singapore Dollar");
        public static readonly Currency Shp = new Currency("SHP", "Saint Helena Pound");
        public static readonly Currency Sll = new Currency("SLL", "Sierra Leonean Leone");
        public static readonly Currency Sos = new Currency("SOS", "Somali Shilling");
        public static readonly Currency Srd = new Currency("SRD", "Surinam Dollar");
        public static readonly Currency Stn = new Currency("STN", "Sao Tomean Dobra");
        public static readonly Currency Svc = new Currency("SVC", "El Salvador Colon");
        public static readonly Currency Syp = new Currency("SYP", "Syrian Pound");
        public static readonly Currency Szl = new Currency("SZL", "Swazi Lilangeni");
        public static readonly Currency Thb = new Currency("THB", "Thai Baht");
        public static readonly Currency Tjs = new Currency("TJS", "Tajikistani Samani");
        public static readonly Currency Tmt = new Currency("TMT", "Turkmenistan Manat");
        public static readonly Currency Tnd = new Currency("TND", "Tunisian Dinar");
        public static readonly Currency Top = new Currency("TOP", "Tongan Pa'anga");
        public static readonly Currency Try = new Currency("TRY", "Turkish Lira");
        public static readonly Currency Ttd = new Currency("TTD", "Trinidadian Dollar");
        public static readonly Currency Tvd = new Currency("TVD", "Tuvaluan Dollar");
        public static readonly Currency Twd = new Currency("TWD", "Taiwan Dollar");
        public static readonly Currency Tzs = new Currency("TZS", "Tanzanian Shilling");
        public static readonly Currency Uah = new Currency("UAH", "Ukrainian Hryvnia");
        public static readonly Currency Ugx = new Currency("UGX", "Uganda Shilling");
        public static readonly Currency Usd = new Currency("USD", "United States Dollar");
        public static readonly Currency Uyu = new Currency("UYU", "Peso Uruguayo");
        public static readonly Currency Uzs = new Currency("UZS", "Uzbekistan Sum");
        public static readonly Currency Ves = new Currency("VES", "Venezuelan Soberano");
        public static readonly Currency Vnd = new Currency("VND", "Vietnamese Dong");
        public static readonly Currency Vuv = new Currency("VUV", "Vanuatu Vatu");
        public static readonly Currency Wst = new Currency("WST", "Samoan Tala");
        public static readonly Currency Yer = new Currency("YER", "Yemeni Rial");
        public static readonly Currency Zar = new Currency("ZAR", "South African Rand");
        public static readonly Currency Zmw = new Currency("ZMW", "Zambian Kwacha");
        public static readonly Currency Zwl = new Currency("ZWL", "Zimbabwean Dollar");
        #endregion
    }
}