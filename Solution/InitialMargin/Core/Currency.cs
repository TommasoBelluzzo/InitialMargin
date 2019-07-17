#region Using Directives
using System;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    /// <summary>Represents a currency. This class cannot be derived.</summary>
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
        /// <summary>Gets a value indicating whether the currency is associated to a residual bucket.</summary>
        /// <value><c>true</c> if the currency is associated to a residual bucket; otherwise, <c>false</c>.</value>
        public Boolean IsResidual => false;

        /// <summary>Gets the category of the currency.</summary>
        /// <value>An enumerator value of type <see cref="T:InitialMargin.Core.CurrencyCategory"/>.</value>
        public CurrencyCategory Category => m_Category;

        /// <summary>Gets the liquidity of the currency.</summary>
        /// <value>An enumerator value of type <see cref="T:InitialMargin.Core.CurrencyLiquidity"/>.</value>
        public CurrencyLiquidity Liquidity => m_Liquidity;

        /// <summary>Gets the volatility of the currency.</summary>
        /// <value>An enumerator value of type <see cref="T:InitialMargin.Core.CurrencyVolatility"/>.</value>
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
        /// <summary>Represents the AED currency. This field is read-only.</summary>
        public static readonly Currency Aed = new Currency("AED", "UAE Dirham");

        /// <summary>Represents the AFN currency. This field is read-only.</summary>
        public static readonly Currency Afn = new Currency("AFN", "Afghan Afghani");

        /// <summary>Represents the ALL currency. This field is read-only.</summary>
        public static readonly Currency All = new Currency("ALL", "Albanian Lek");

        /// <summary>Represents the AMD currency. This field is read-only.</summary>
        public static readonly Currency Amd = new Currency("AMD", "Armenian Dram");

        /// <summary>Represents the ANG currency. This field is read-only.</summary>
        public static readonly Currency Ang = new Currency("ANG", "Netherlands Antillean Guilder");

        /// <summary>Represents the AOA currency. This field is read-only.</summary>
        public static readonly Currency Aoa = new Currency("AOA", "Angolan Kwanza");

        /// <summary>Represents the ARS currency. This field is read-only.</summary>
        public static readonly Currency Ars = new Currency("ARS", "Argentine Peso");

        /// <summary>Represents the AUD currency. This field is read-only.</summary>
        public static readonly Currency Aud = new Currency("AUD", "Australian Dollar");

        /// <summary>Represents the AWG currency. This field is read-only.</summary>
        public static readonly Currency Awg = new Currency("AWG", "Aruban Florin");

        /// <summary>Represents the AZN currency. This field is read-only.</summary>
        public static readonly Currency Azn = new Currency("AZN", "Azerbaijan Manat");

        /// <summary>Represents the BAM currency. This field is read-only.</summary>
        public static readonly Currency Bam = new Currency("BAM", "Convertible Mark");

        /// <summary>Represents the BBD currency. This field is read-only.</summary>
        public static readonly Currency Bbd = new Currency("BBD", "Barbados Dollar");

        /// <summary>Represents the BDT currency. This field is read-only.</summary>
        public static readonly Currency Bdt = new Currency("BDT", "Bangladeshi Taka");

        /// <summary>Represents the BGN currency. This field is read-only.</summary>
        public static readonly Currency Bgn = new Currency("BGN", "Bulgarian Lev");

        /// <summary>Represents the BHD currency. This field is read-only.</summary>
        public static readonly Currency Bhd = new Currency("BHD", "Bahraini Dinar");

        /// <summary>Represents the BIF currency. This field is read-only.</summary>
        public static readonly Currency Bif = new Currency("BIF", "Burundi Franc");

        /// <summary>Represents the BMD currency. This field is read-only.</summary>
        public static readonly Currency Bmd = new Currency("BMD", "Bermudian Dollar");

        /// <summary>Represents the BND currency. This field is read-only.</summary>
        public static readonly Currency Bnd = new Currency("BND", "Brunei Dollar");

        /// <summary>Represents the BOB currency. This field is read-only.</summary>
        public static readonly Currency Bob = new Currency("BOB", "Bolivian Boliviano");

        /// <summary>Represents the BRL currency. This field is read-only.</summary>
        public static readonly Currency Brl = new Currency("BRL", "Brazilian Real");

        /// <summary>Represents the BSD currency. This field is read-only.</summary>
        public static readonly Currency Bsd = new Currency("BSD", "Bahamian Dollar");

        /// <summary>Represents the BTN currency. This field is read-only.</summary>
        public static readonly Currency Btn = new Currency("BTN", "Bhutanese Ngultrum");

        /// <summary>Represents the BWP currency. This field is read-only.</summary>
        public static readonly Currency Bwp = new Currency("BWP", "Botswana Pula");

        /// <summary>Represents the BYN currency. This field is read-only.</summary>
        public static readonly Currency Byn = new Currency("BYN", "Belarusian Ruble");

        /// <summary>Represents the BZD currency. This field is read-only.</summary>
        public static readonly Currency Bzd = new Currency("BZD", "Belize Dollar");

        /// <summary>Represents the CAD currency. This field is read-only.</summary>
        public static readonly Currency Cad = new Currency("CAD", "Canadian Dollar");

        /// <summary>Represents the CDF currency. This field is read-only.</summary>
        public static readonly Currency Cdf = new Currency("CDF", "Congolese Franc");

        /// <summary>Represents the CHF currency. This field is read-only.</summary>
        public static readonly Currency Chf = new Currency("CHF", "Swiss Franc");

        /// <summary>Represents the CLP currency. This field is read-only.</summary>
        public static readonly Currency Clp = new Currency("CLP", "Chilean Peso");

        /// <summary>Represents the CNY currency. This field is read-only.</summary>
        public static readonly Currency Cny = new Currency("CNY", "Yuan Renminbi");

        /// <summary>Represents the COP currency. This field is read-only.</summary>
        public static readonly Currency Cop = new Currency("COP", "Colombian Peso");

        /// <summary>Represents the CRC currency. This field is read-only.</summary>
        public static readonly Currency Crc = new Currency("CRC", "Costa Rican Colon");

        /// <summary>Represents the CUP currency. This field is read-only.</summary>
        public static readonly Currency Cup = new Currency("CUP", "Cuban Peso");

        /// <summary>Represents the CVE currency. This field is read-only.</summary>
        public static readonly Currency Cve = new Currency("CVE", "Cabo Verde Escudo");

        /// <summary>Represents the CZK currency. This field is read-only.</summary>
        public static readonly Currency Czk = new Currency("CZK", "Czech Koruna");

        /// <summary>Represents the DJF currency. This field is read-only.</summary>
        public static readonly Currency Djf = new Currency("DJF", "Djibouti Franc");

        /// <summary>Represents the DKK currency. This field is read-only.</summary>
        public static readonly Currency Dkk = new Currency("DKK", "Danish Krone");

        /// <summary>Represents the DOP currency. This field is read-only.</summary>
        public static readonly Currency Dop = new Currency("DOP", "Dominican Peso");

        /// <summary>Represents the DZD currency. This field is read-only.</summary>
        public static readonly Currency Dzd = new Currency("DZD", "Algerian Dinar");

        /// <summary>Represents the EGP currency. This field is read-only.</summary>
        public static readonly Currency Egp = new Currency("EGP", "Egyptian Pound");

        /// <summary>Represents the ERN currency. This field is read-only.</summary>
        public static readonly Currency Ern = new Currency("ERN", "Eritrean Nakfa");

        /// <summary>Represents the ETB currency. This field is read-only.</summary>
        public static readonly Currency Etb = new Currency("ETB", "Ethiopian Birr");

        /// <summary>Represents the EUR currency. This field is read-only.</summary>
        public static readonly Currency Eur = new Currency("EUR", "Euro");

        /// <summary>Represents the FJD currency. This field is read-only.</summary>
        public static readonly Currency Fjd = new Currency("FJD", "Fiji Dollar");

        /// <summary>Represents the FKP currency. This field is read-only.</summary>
        public static readonly Currency Fkp = new Currency("FKP", "Falkland Islands Pound");

        /// <summary>Represents the GBP currency. This field is read-only.</summary>
        public static readonly Currency Gbp = new Currency("GBP", "Pound Sterling");

        /// <summary>Represents the GEL currency. This field is read-only.</summary>
        public static readonly Currency Gel = new Currency("GEL", "Georgian Lari");

        /// <summary>Represents the GHS currency. This field is read-only.</summary>
        public static readonly Currency Ghs = new Currency("GHS", "Ghana Cedi");

        /// <summary>Represents the GIP currency. This field is read-only.</summary>
        public static readonly Currency Gip = new Currency("GIP", "Gibraltar Pound");

        /// <summary>Represents the GMD currency. This field is read-only.</summary>
        public static readonly Currency Gmd = new Currency("GMD", "Gambian Dalasi");

        /// <summary>Represents the GNF currency. This field is read-only.</summary>
        public static readonly Currency Gnf = new Currency("GNF", "Guinean Franc");

        /// <summary>Represents the GTQ currency. This field is read-only.</summary>
        public static readonly Currency Gtq = new Currency("GTQ", "Guatemalan Quetzal");

        /// <summary>Represents the GYD currency. This field is read-only.</summary>
        public static readonly Currency Gyd = new Currency("GYD", "Guyana Dollar");

        /// <summary>Represents the HKD currency. This field is read-only.</summary>
        public static readonly Currency Hkd = new Currency("HKD", "Hong Kong Dollar");

        /// <summary>Represents the HNL currency. This field is read-only.</summary>
        public static readonly Currency Hnl = new Currency("HNL", "Honduran Lempira");

        /// <summary>Represents the HRK currency. This field is read-only.</summary>
        public static readonly Currency Hrk = new Currency("HRK", "Croatian Kuna");

        /// <summary>Represents the HTG currency. This field is read-only.</summary>
        public static readonly Currency Htg = new Currency("HTG", "Haitian Gourde");

        /// <summary>Represents the HUF currency. This field is read-only.</summary>
        public static readonly Currency Huf = new Currency("HUF", "Hungarian Forint");

        /// <summary>Represents the IDR currency. This field is read-only.</summary>
        public static readonly Currency Idr = new Currency("IDR", "Indonesian Rupiah");

        /// <summary>Represents the ILS currency. This field is read-only.</summary>
        public static readonly Currency Ils = new Currency("ILS", "Israeli Sheqel");

        /// <summary>Represents the INR currency. This field is read-only.</summary>
        public static readonly Currency Inr = new Currency("INR", "Indian Rupee");

        /// <summary>Represents the IQD currency. This field is read-only.</summary>
        public static readonly Currency Iqd = new Currency("IQD", "Iraqi Dinar");

        /// <summary>Represents the IRR currency. This field is read-only.</summary>
        public static readonly Currency Irr = new Currency("IRR", "Iranian Rial");

        /// <summary>Represents the ISK currency. This field is read-only.</summary>
        public static readonly Currency Isk = new Currency("ISK", "Iceland Krona");

        /// <summary>Represents the JMD currency. This field is read-only.</summary>
        public static readonly Currency Jmd = new Currency("JMD", "Jamaican Dollar");

        /// <summary>Represents the JOD currency. This field is read-only.</summary>
        public static readonly Currency Jod = new Currency("JOD", "Jordanian Dinar");

        /// <summary>Represents the JPY currency. This field is read-only.</summary>
        public static readonly Currency Jpy = new Currency("JPY", "Japanese Yen");

        /// <summary>Represents the KES currency. This field is read-only.</summary>
        public static readonly Currency Kes = new Currency("KES", "Kenyan Shilling");

        /// <summary>Represents the KGS currency. This field is read-only.</summary>
        public static readonly Currency Kgs = new Currency("KGS", "Kyrgyzstani Som");

        /// <summary>Represents the KHR currency. This field is read-only.</summary>
        public static readonly Currency Khr = new Currency("KHR", "Omani Riel");

        /// <summary>Represents the KMF currency. This field is read-only.</summary>
        public static readonly Currency Kmf = new Currency("KMF", "Comorian Franc ");

        /// <summary>Represents the KPW currency. This field is read-only.</summary>
        public static readonly Currency Kpw = new Currency("KPW", "North Korean Won");

        /// <summary>Represents the KRW currency. This field is read-only.</summary>
        public static readonly Currency Krw = new Currency("KRW", "South Korean Won");

        /// <summary>Represents the KWD currency. This field is read-only.</summary>
        public static readonly Currency Kwd = new Currency("KWD", "Kuwaiti Dinar");

        /// <summary>Represents the KYD currency. This field is read-only.</summary>
        public static readonly Currency Kyd = new Currency("KYD", "Cayman Islands Dollar");

        /// <summary>Represents the KZT currency. This field is read-only.</summary>
        public static readonly Currency Kzt = new Currency("KZT", "Kazakhstani Tenge");

        /// <summary>Represents the LAK currency. This field is read-only.</summary>
        public static readonly Currency Lak = new Currency("LAK", "Lao Kip");

        /// <summary>Represents the LBP currency. This field is read-only.</summary>
        public static readonly Currency Lbp = new Currency("LBP", "Lebanese Pound");

        /// <summary>Represents the LKR currency. This field is read-only.</summary>
        public static readonly Currency Lkr = new Currency("LKR", "Sri Lanka Rupee");

        /// <summary>Represents the LRD currency. This field is read-only.</summary>
        public static readonly Currency Lrd = new Currency("LRD", "Liberian Dollar");

        /// <summary>Represents the LSL currency. This field is read-only.</summary>
        public static readonly Currency Lsl = new Currency("LSL", "Basotho Loti");

        /// <summary>Represents the LYD currency. This field is read-only.</summary>
        public static readonly Currency Lyd = new Currency("LYD", "Libyan Dinar");

        /// <summary>Represents the MAD currency. This field is read-only.</summary>
        public static readonly Currency Mad = new Currency("MAD", "Moroccan Dirham");

        /// <summary>Represents the MDL currency. This field is read-only.</summary>
        public static readonly Currency Mdl = new Currency("MDL", "Moldovan Leu");

        /// <summary>Represents the MGA currency. This field is read-only.</summary>
        public static readonly Currency Mga = new Currency("MGA", "Malagasy Ariary");

        /// <summary>Represents the MKD currency. This field is read-only.</summary>
        public static readonly Currency Mkd = new Currency("MKD", "Macedonian Denar");

        /// <summary>Represents the MMK currency. This field is read-only.</summary>
        public static readonly Currency Mmk = new Currency("MMK", "Burmese Kyat");

        /// <summary>Represents the MNT currency. This field is read-only.</summary>
        public static readonly Currency Mnt = new Currency("MNT", "Mongolian Tugrik");

        /// <summary>Represents the MOP currency. This field is read-only.</summary>
        public static readonly Currency Mop = new Currency("MOP", "Macanese Pataca");

        /// <summary>Represents the MRU currency. This field is read-only.</summary>
        public static readonly Currency Mru = new Currency("MRU", "Mauritanian Ouguiya");

        /// <summary>Represents the MUR currency. This field is read-only.</summary>
        public static readonly Currency Mur = new Currency("MUR", "Mauritius Rupee");

        /// <summary>Represents the MVR currency. This field is read-only.</summary>
        public static readonly Currency Mvr = new Currency("MVR", "Maldivian Rufiyaa");

        /// <summary>Represents the MWK currency. This field is read-only.</summary>
        public static readonly Currency Mwk = new Currency("MWK", "Malawi Kwacha");

        /// <summary>Represents the MXN currency. This field is read-only.</summary>
        public static readonly Currency Mxn = new Currency("MXN", "Mexican Peso");

        /// <summary>Represents the MYR currency. This field is read-only.</summary>
        public static readonly Currency Myr = new Currency("MYR", "Malaysian Ringgit");

        /// <summary>Represents the MZN currency. This field is read-only.</summary>
        public static readonly Currency Mzn = new Currency("MZN", "Mozambique Metical");

        /// <summary>Represents the NAD currency. This field is read-only.</summary>
        public static readonly Currency Nad = new Currency("NAD", "Namibia Dollar");

        /// <summary>Represents the NGN currency. This field is read-only.</summary>
        public static readonly Currency Ngn = new Currency("NGN", "Nigerian Naira");

        /// <summary>Represents the NIO currency. This field is read-only.</summary>
        public static readonly Currency Nio = new Currency("NIO", "Nicaraguan Cordoba");

        /// <summary>Represents the NOK currency. This field is read-only.</summary>
        public static readonly Currency Nok = new Currency("NOK", "Norwegian Krone");

        /// <summary>Represents the NPR currency. This field is read-only.</summary>
        public static readonly Currency Npr = new Currency("NPR", "Nepalese Rupee");

        /// <summary>Represents the NZD currency. This field is read-only.</summary>
        public static readonly Currency Nzd = new Currency("NZD", "New Zealand Dollar");

        /// <summary>Represents the OMR currency. This field is read-only.</summary>
        public static readonly Currency Omr = new Currency("OMR", "Omani Rial");

        /// <summary>Represents the PAB currency. This field is read-only.</summary>
        public static readonly Currency Pab = new Currency("PAB", "Panamanian Balboa");

        /// <summary>Represents the PEN currency. This field is read-only.</summary>
        public static readonly Currency Pen = new Currency("PEN", "Peruvian Sol");

        /// <summary>Represents the PGK currency. This field is read-only.</summary>
        public static readonly Currency Pgk = new Currency("PGK", "Papua New Guinean Kina");

        /// <summary>Represents the PHP currency. This field is read-only.</summary>
        public static readonly Currency Php = new Currency("PHP", "Philippine Peso");

        /// <summary>Represents the PKR currency. This field is read-only.</summary>
        public static readonly Currency Pkr = new Currency("PKR", "Pakistan Rupee");

        /// <summary>Represents the PLN currency. This field is read-only.</summary>
        public static readonly Currency Pln = new Currency("PLN", "Polish Zloty");

        /// <summary>Represents the PYG currency. This field is read-only.</summary>
        public static readonly Currency Pyg = new Currency("PYG", "Paraguayan Guarani");

        /// <summary>Represents the QAR currency. This field is read-only.</summary>
        public static readonly Currency Qar = new Currency("QAR", "Qatari Rial");

        /// <summary>Represents the RON currency. This field is read-only.</summary>
        public static readonly Currency Ron = new Currency("RON", "Romanian Leu");

        /// <summary>Represents the RSD currency. This field is read-only.</summary>
        public static readonly Currency Rsd = new Currency("RSD", "Serbian Dinar");

        /// <summary>Represents the RUB currency. This field is read-only.</summary>
        public static readonly Currency Rub = new Currency("RUB", "Russian Ruble");

        /// <summary>Represents the RWF currency. This field is read-only.</summary>
        public static readonly Currency Rwf = new Currency("RWF", "Rwanda Franc");

        /// <summary>Represents the SAR currency. This field is read-only.</summary>
        public static readonly Currency Sar = new Currency("SAR", "Saudi Riyal");

        /// <summary>Represents the SBD currency. This field is read-only.</summary>
        public static readonly Currency Sbd = new Currency("SBD", "Solomon Islands Dollar");

        /// <summary>Represents the SCR currency. This field is read-only.</summary>
        public static readonly Currency Scr = new Currency("SCR", "Seychelles Rupee");

        /// <summary>Represents the SDG currency. This field is read-only.</summary>
        public static readonly Currency Sdg = new Currency("SDG", "Sudanese Pound");

        /// <summary>Represents the SEK currency. This field is read-only.</summary>
        public static readonly Currency Sek = new Currency("SEK", "Swedish Krona");

        /// <summary>Represents the SGD currency. This field is read-only.</summary>
        public static readonly Currency Sgd = new Currency("SGD", "Singapore Dollar");

        /// <summary>Represents the SHP currency. This field is read-only.</summary>
        public static readonly Currency Shp = new Currency("SHP", "Saint Helena Pound");

        /// <summary>Represents the SLL currency. This field is read-only.</summary>
        public static readonly Currency Sll = new Currency("SLL", "Sierra Leonean Leone");

        /// <summary>Represents the SOS currency. This field is read-only.</summary>
        public static readonly Currency Sos = new Currency("SOS", "Somali Shilling");

        /// <summary>Represents the SRD currency. This field is read-only.</summary>
        public static readonly Currency Srd = new Currency("SRD", "Surinam Dollar");

        /// <summary>Represents the STN currency. This field is read-only.</summary>
        public static readonly Currency Stn = new Currency("STN", "Sao Tomean Dobra");

        /// <summary>Represents the SVC currency. This field is read-only.</summary>
        public static readonly Currency Svc = new Currency("SVC", "El Salvador Colon");

        /// <summary>Represents the SYP currency. This field is read-only.</summary>
        public static readonly Currency Syp = new Currency("SYP", "Syrian Pound");

        /// <summary>Represents the SZL currency. This field is read-only.</summary>
        public static readonly Currency Szl = new Currency("SZL", "Swazi Lilangeni");

        /// <summary>Represents the THB currency. This field is read-only.</summary>
        public static readonly Currency Thb = new Currency("THB", "Thai Baht");

        /// <summary>Represents the TJS currency. This field is read-only.</summary>
        public static readonly Currency Tjs = new Currency("TJS", "Tajikistani Samani");

        /// <summary>Represents the TMT currency. This field is read-only.</summary>
        public static readonly Currency Tmt = new Currency("TMT", "Turkmenistan Manat");

        /// <summary>Represents the TND currency. This field is read-only.</summary>
        public static readonly Currency Tnd = new Currency("TND", "Tunisian Dinar");

        /// <summary>Represents the TOP currency. This field is read-only.</summary>
        public static readonly Currency Top = new Currency("TOP", "Tongan Pa'anga");

        /// <summary>Represents the TRY currency. This field is read-only.</summary>
        public static readonly Currency Try = new Currency("TRY", "Turkish Lira");

        /// <summary>Represents the TTD currency. This field is read-only.</summary>
        public static readonly Currency Ttd = new Currency("TTD", "Trinidadian Dollar");

        /// <summary>Represents the TVD currency. This field is read-only.</summary>
        public static readonly Currency Tvd = new Currency("TVD", "Tuvaluan Dollar");

        /// <summary>Represents the TWD currency. This field is read-only.</summary>
        public static readonly Currency Twd = new Currency("TWD", "Taiwan Dollar");

        /// <summary>Represents the TZS currency. This field is read-only.</summary>
        public static readonly Currency Tzs = new Currency("TZS", "Tanzanian Shilling");

        /// <summary>Represents the UAH currency. This field is read-only.</summary>
        public static readonly Currency Uah = new Currency("UAH", "Ukrainian Hryvnia");

        /// <summary>Represents the UGX currency. This field is read-only.</summary>
        public static readonly Currency Ugx = new Currency("UGX", "Uganda Shilling");

        /// <summary>Represents the USD currency. This field is read-only.</summary>
        public static readonly Currency Usd = new Currency("USD", "United States Dollar");

        /// <summary>Represents the UYU currency. This field is read-only.</summary>
        public static readonly Currency Uyu = new Currency("UYU", "Peso Uruguayo");

        /// <summary>Represents the UZS currency. This field is read-only.</summary>
        public static readonly Currency Uzs = new Currency("UZS", "Uzbekistan Sum");

        /// <summary>Represents the VES currency. This field is read-only.</summary>
        public static readonly Currency Ves = new Currency("VES", "Venezuelan Soberano");

        /// <summary>Represents the VND currency. This field is read-only.</summary>
        public static readonly Currency Vnd = new Currency("VND", "Vietnamese Dong");

        /// <summary>Represents the VUV currency. This field is read-only.</summary>
        public static readonly Currency Vuv = new Currency("VUV", "Vanuatu Vatu");

        /// <summary>Represents the WST currency. This field is read-only.</summary>
        public static readonly Currency Wst = new Currency("WST", "Samoan Tala");

        /// <summary>Represents the YER currency. This field is read-only.</summary>
        public static readonly Currency Yer = new Currency("YER", "Yemeni Rial");

        /// <summary>Represents the ZAR currency. This field is read-only.</summary>
        public static readonly Currency Zar = new Currency("ZAR", "South African Rand");

        /// <summary>Represents the ZMW currency. This field is read-only.</summary>
        public static readonly Currency Zmw = new Currency("ZMW", "Zambian Kwacha");

        /// <summary>Represents the ZWL currency. This field is read-only.</summary>
        public static readonly Currency Zwl = new Currency("ZWL", "Zimbabwean Dollar");
        #endregion
    }
}