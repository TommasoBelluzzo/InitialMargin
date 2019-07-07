#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using InitialMargin.Core;
using InitialMargin.Model;
using InitialMargin.Schedule;
#endregion

namespace InitialMargin.IO
{
    public static class CrifManager
    {
        #region Members
        private static readonly String[] s_Properties =
        {
            "RiskType", "IMModel", "ProductClass",
            "Qualifier","Bucket", "Label1", "Label2",
            "Amount", "AmountCurrency", "AmountUSD",
            "CollectRegulations", "PostRegulations",
            "PortfolioID", "TradeID", "EndDate"
        };

        private static readonly String[] s_PropertiesAmount =
        {
            "Amount", "AmountCurrency", "AmountUSD"
        };

        private static readonly String[] s_PropertiesCommon =
        {
            "RiskType", "IMModel", "CollectRegulations", "PostRegulations"
        };

        private static readonly String[] s_PropertiesModel =
        {
            "ProductClass"
        };

        private static readonly String[] s_PropertiesScalar =
        {
            "Qualifier", "Amount"
        };

        private static readonly String[] s_PropertiesTrade =
        {
            "Qualifier", "PortfolioID", "TradeID", "EndDate"
        };
        #endregion

        #region Methods
        private static AddOnFixedAmount ReadAddOnFixedAmount(EntryObject o)
        {
            if (o.InitialMarginModel != "SIMM")
                throw new InvalidDataException("The CRIF file contains a Param_AddOnFixedAmount entry with the IMModel property not set to \"SIMM\".");

            foreach (String property in s_Properties.Except(s_PropertiesAmount.Union(s_PropertiesCommon)))
            {
                if (!String.IsNullOrEmpty((String)o.GetType().GetProperty(property)?.GetValue(o)))
                    throw new InvalidDataException($"The CRIF file cannot specify the {property} property for Param_AddOnFixedAmount entries.");
            }

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);

            return AddOnFixedAmount.Of(amount, regulationsInfo);
        }

        private static AddOnNotional ReadAddOnNotional(EntryObject o)
        {
            foreach (String property in s_Properties.Except(s_PropertiesAmount.Union(s_PropertiesCommon).Union(s_PropertiesTrade)))
            {
                if (!String.IsNullOrEmpty((String)o.GetType().GetProperty(property)?.GetValue(o)))
                    throw new InvalidDataException($"The CRIF file cannot specify the {property} property for Notional entries.");
            }

            if (!DataValidator.IsValidNotionalQualifier(o.Qualifier))
                throw new InvalidDataException("The CRIF file contains a Notional entry with an invalid Qualifier property.");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            return AddOnNotional.Of(o.Qualifier, amount, regulationsInfo, tradeInfo);
        }

        private static AddOnNotionalFactor ReadAddOnNotionalFactor(EntryObject o)
        {
            foreach (String property in s_Properties.Except(s_PropertiesCommon.Union(s_PropertiesScalar)))
            {
                if (!String.IsNullOrEmpty((String)o.GetType().GetProperty(property)?.GetValue(o)))
                    throw new InvalidDataException($"The CRIF file cannot specify the {property} property for Param_AddOnNotionalFactor entries.");
            }

            if (!DataValidator.IsValidNotionalQualifier(o.Qualifier))
                throw new InvalidDataException("The CRIF file contains a Param_AddOnNotionalFactor entry with an invalid Qualifier property.");

            if (!Decimal.TryParse(o.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal factor))
                throw new InvalidDataException("The CRIF file contains a Param_AddOnNotionalFactor entry with an invalid Amount property.");

            factor /= 100m;

            if ((factor <= 0m) || (factor > 1m))
                throw new InvalidDataException("The CRIF file contains a Param_AddOnNotionalFactor entry whose factor is not between 0 and 100.");

            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);

            return AddOnNotionalFactor.Of(o.Qualifier, factor, regulationsInfo);
        }

        private static AddOnProductMultiplier ReadProductMultiplier(EntryObject o)
        {
            foreach (String property in s_Properties.Except(s_PropertiesCommon.Union(s_PropertiesScalar)))
            {
                if (!String.IsNullOrEmpty((String)o.GetType().GetProperty(property)?.GetValue(o)))
                    throw new InvalidDataException($"The CRIF file cannot specify the {property} property for Param_ProductClassMultiplier entries.");
            }

            if (!Enum.TryParse(o.Qualifier, out Model.Product product))
                throw new InvalidDataException("The CRIF file contains a Param_ProductClassMultiplier entry with an invalid Qualifier property.");

            if (!Decimal.TryParse(o.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal multiplier))
                throw new InvalidDataException("The CRIF file contains a Param_ProductClassMultiplier entry with an invalid Amount property.");

            multiplier -= 1m;

            if (multiplier <= 0m)
                throw new InvalidDataException("The CRIF file contains a Param_ProductClassMultiplier entry with a multiplier less than 1.");

            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);

            return AddOnProductMultiplier.Of(product, multiplier, regulationsInfo);
        }

        private static Amount ReadAmount(EntryObject o)
        {
            Boolean amountLocalDefined = Decimal.TryParse(o.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal amountLocal);
            Boolean amountCurrencyDefined = Currency.TryParse(o.AmountCurrency, out Currency amountCurrency);
            Boolean amountUsdDefined = Decimal.TryParse(o.AmountUsd, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal amountUsd);

            if (amountLocalDefined && amountCurrencyDefined)
            {
                if (amountUsdDefined && (amountCurrency == Currency.Usd) && (amountLocal != amountUsd))
                    throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with AmountCurrency set to \"USD\" and mismatching values of Amount and AmountUSD properties.");

                return Amount.Of(amountCurrency, amountLocal);
            }

            if (amountLocalDefined || amountCurrencyDefined)
            {
                if (amountLocalDefined)
                    throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with a valid Amount property and an invalid or undefined AmountCurrency property.");

                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with a valid AmountCurrency property and an invalid or undefined Amount property.");
            }

            if (!amountUsdDefined)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with no amount information (either Amount and AmountCurrency properties or AmountUSD property must be defined).");

            return Amount.Of(Currency.Usd, amountUsd);
        }

        private static EntryObject ToEntryObject(String[] values, Dictionary<String,Int32> propertiesMapping)
        {
            return (new EntryObject
            {
                Amount = values[propertiesMapping["Amount"]],
                AmountCurrency = values[propertiesMapping["AmountCurrency"]],
                AmountUsd = values[propertiesMapping["AmountUSD"]],
                Bucket = values[propertiesMapping["Bucket"]],
                CollectRegulations = values[propertiesMapping["CollectRegulations"]],
                EndDate = values[propertiesMapping["EndDate"]],
                InitialMarginModel = values[propertiesMapping["IMModel"]],
                Label1 = values[propertiesMapping["Label1"]],
                Label2 = values[propertiesMapping["Label2"]],
                PortfolioId = values[propertiesMapping["PortfolioID"]],
                PostRegulations = values[propertiesMapping["PostRegulations"]],
                ProductClass = values[propertiesMapping["ProductClass"]],
                Qualifier = values[propertiesMapping["Qualifier"]],
                RiskType = values[propertiesMapping["RiskType"]],
                TradeId = values[propertiesMapping["TradeID"]]
            });
        }

        private static Notional ReadNotional(EntryObject o)
        {
            foreach (String property in s_Properties.Except(s_PropertiesAmount.Union(s_PropertiesCommon).Union(s_PropertiesTrade).Union(s_PropertiesModel)))
            {
                if (!String.IsNullOrEmpty((String)o.GetType().GetProperty(property)?.GetValue(o)))
                    throw new InvalidDataException($"The CRIF file cannot specify the {property} property for Notional entries.");
            }

            if (!Enum.TryParse(o.ProductClass, out Schedule.Product product))
                throw new InvalidDataException("The CRIF file contains a Notional entry with an invalid ProductClass property.");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            return Notional.Of(product, amount, regulationsInfo, tradeInfo);
        }

        private static PresentValue ReadPresentValue(EntryObject o)
        {
            foreach (String property in s_Properties.Except(s_PropertiesAmount.Union(s_PropertiesCommon).Union(s_PropertiesTrade).Union(s_PropertiesModel)))
            {
                if (!String.IsNullOrEmpty((String)o.GetType().GetProperty(property)?.GetValue(o)))
                    throw new InvalidDataException($"The CRIF file cannot specify the {property} property for PV entries.");
            }

            if (!Enum.TryParse(o.ProductClass, out Schedule.Product product))
                throw new InvalidDataException("The CRIF file contains a PV entry with an invalid ProductClass property.");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            return PresentValue.Of(product, amount, regulationsInfo, tradeInfo);
        }

        private static RegulationsInfo ReadRegulationsInfo(EntryObject o)
        {
            if (o.CollectRegulations == null)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid CollectRegulations property.");

            if (o.PostRegulations == null)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid PostRegulations property.");

            List<Regulation> collectRegulations;

            if (DataValidator.BlankRegulations(o.CollectRegulations))
                collectRegulations = new List<Regulation>(0);
            else if (DataValidator.FilledRegulations(o.CollectRegulations))
            {
                List<String> collectRegulationsTokens = o.CollectRegulations
                    .Split(',')
                    .Select(x => String.Concat(x.Substring(0, 1), x.Substring(1).ToLowerInvariant()))
                    .ToList();

                collectRegulations = new List<Regulation>(collectRegulationsTokens.Count);

                foreach (String collectRegulationsToken in collectRegulationsTokens)
                {
                    if (!Enum.TryParse(collectRegulationsToken, out Regulation regulation))
                        throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with one or more invalid items in the CollectRegulations property.");

                    if (collectRegulations.Contains(regulation))
                        throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with one or more duplicate items in the CollectRegulations property.");

                    collectRegulations.Add(regulation);
                }
            }
            else
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid CollectRegulations property.");

            List<Regulation> postRegulations;

            if (DataValidator.BlankRegulations(o.PostRegulations))
                postRegulations = new List<Regulation>(0);
            else if (DataValidator.FilledRegulations(o.PostRegulations))
            {
                List<String> postRegulationsTokens = o.PostRegulations
                    .Split(',')
                    .Select(x => String.Concat(x.Substring(0, 1), x.Substring(1).ToLowerInvariant()))
                    .ToList();

                postRegulations = new List<Regulation>(postRegulationsTokens.Count);

                foreach (String postRegulationsToken in postRegulationsTokens)
                {
                    if (!Enum.TryParse(postRegulationsToken, out Regulation regulation))
                        throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with one or more invalid items in the PostRegulations property.");

                    if (postRegulations.Contains(regulation))
                        throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with one or more duplicate items in the PostRegulations property.");

                    postRegulations.Add(regulation);
                }
            }
            else
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid PostRegulations property.");

            return RegulationsInfo.Of(collectRegulations, postRegulations);
        }

        private static Sensitivity ReadSensitivityBaseCorrelation(EntryObject o)
        {
            if (!String.IsNullOrEmpty(o.Bucket) || !String.IsNullOrEmpty(o.Label1) || !String.IsNullOrEmpty(o.Label2))
                throw new InvalidDataException("The CRIF file cannot specify Bucket, Label1 and Label2 properties for Risk_BaseCorr entries.");

            if (!Enum.TryParse(o.ProductClass, out Model.Product product))
                throw new InvalidDataException("The CRIF file contains a Risk_BaseCorr entry with an invalid ProductClass property.");

            if (product != Model.Product.Credit)
                throw new InvalidDataException($"The CRIF file contains a Risk_BaseCorr entry associated to a product class other than {Model.Product.Credit}.");

            if (String.IsNullOrEmpty(o.Qualifier))
                throw new InvalidDataException("The CRIF file contains a Risk_BaseCorr entry with an invalid Qualifier property.");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            return Sensitivity.BaseCorrelation(o.Qualifier, amount, regulationsInfo, tradeInfo);
        }

        private static Sensitivity ParseSensitivityCommodity(EntryObject o)
        {
            if (!Enum.TryParse(o.ProductClass, out Model.Product product))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid ProductClass property.");

            if (product != Model.Product.Commodity)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry associated to a product class other than {Model.Product.Commodity}.");

            if (String.IsNullOrEmpty(o.Qualifier))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Qualifier property.");

            if (!BucketCommodity.TryParse(o.Bucket, out BucketCommodity bucket))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Bucket property.");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            if (o.RiskType == "Risk_Commodity")
            {
                if (!String.IsNullOrEmpty(o.Label1) || !String.IsNullOrEmpty(o.Label2))
                    throw new InvalidDataException("The CRIF file cannot specify Label1 and Label2 properties for Risk_Commodity entries.");

                return Sensitivity.CommodityDelta(o.Qualifier, bucket, amount, regulationsInfo, tradeInfo);
            }

            if (!Tenor.TryParse(o.Label1, out Tenor tenor))
                throw new InvalidDataException("The CRIF file contains a Risk_CommodityVol entry with an invalid Label1 property.");

            if (!String.IsNullOrEmpty(o.Label2))
                throw new InvalidDataException("The CRIF file cannot specify the Label2 property for Risk_CommodityVol entries.");

            return Sensitivity.CommodityVega(o.Qualifier, bucket, tenor, amount, regulationsInfo, tradeInfo);
        }

        private static Sensitivity ParseSensitivityCreditNonQualifying(EntryObject o)
        {
            if (!Enum.TryParse(o.ProductClass, out Model.Product product))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid ProductClass property.");

            if (product != Model.Product.Credit)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry associated to a product class other than {Model.Product.Credit}.");

            if (!BucketCreditNonQualifying.TryParse(o.Bucket, out BucketCreditNonQualifying bucket))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Bucket property.");

            if (!Tenor.TryParse(o.Label1, out Tenor tenor))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Label1 property.");

            if (!tenor.IsCreditTenor)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry associated to an improper tenor (accepted tenors are: {String.Join(", ", Tenor.Values.Where(x => x.IsCreditTenor).Select(x => x.Name))}.");

            if (!String.IsNullOrEmpty(o.Label2))
                throw new InvalidDataException($"The CRIF file cannot specify the Label2 property for {o.RiskType} entries.");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            if (o.RiskType == "Risk_CreditNonQ")
            {
                if (!DataValidator.IsValidQualifier(o.Qualifier, true))
                    throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Qualifier property.");

                return Sensitivity.CreditNonQualifyingDelta(o.Qualifier, bucket, tenor, amount, regulationsInfo, tradeInfo);
            }

            return Sensitivity.CreditNonQualifyingVega(o.Qualifier, bucket, tenor, amount, regulationsInfo, tradeInfo);
        }

        private static Sensitivity ParseSensitivityCreditQualifying(EntryObject o)
        {
            if (!Enum.TryParse(o.ProductClass, out Model.Product product))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid ProductClass property.");

            if (product != Model.Product.Credit)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry associated to a product class other than {Model.Product.Credit}.");

            if (!BucketCreditQualifying.TryParse(o.Bucket, out BucketCreditQualifying bucket))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Bucket property.");

            if (!Tenor.TryParse(o.Label1, out Tenor tenor))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Label1 property.");

            if (!tenor.IsCreditTenor)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry associated to an improper tenor (accepted tenors are: {String.Join(", ", Tenor.Values.Where(x => x.IsCreditTenor).Select(x => x.Name))}).");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            if (o.RiskType == "Risk_CreditQ")
            {
                if (!DataValidator.IsValidQualifier(o.Qualifier, true))
                    throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Qualifier property.");

                Boolean securitization;

                if (String.IsNullOrEmpty(o.Label2))
                    securitization = false;
                else if (o.Label2 == "Sec")
                    securitization = true;
                else
                    throw new InvalidDataException("The CRIF file contains a Risk_CreditQ entry with an invalid Label2 property.");

                return Sensitivity.CreditQualifyingDelta(o.Qualifier, bucket, tenor, securitization, amount, regulationsInfo, tradeInfo);
            }

            if (!String.IsNullOrEmpty(o.Label2))
                throw new InvalidDataException("The CRIF file cannot specify the Label2 property for Risk_CreditVol entries.");

            return Sensitivity.CreditQualifyingVega(o.Qualifier, bucket, tenor, amount, regulationsInfo, tradeInfo);
        }

        private static Sensitivity ReadSensitivityCrossCurrencyBasis(EntryObject o)
        {
            if (!String.IsNullOrEmpty(o.Bucket) || !String.IsNullOrEmpty(o.Label1) || !String.IsNullOrEmpty(o.Label2))
                throw new InvalidDataException("The CRIF file cannot specify Bucket, Label1 and Label2 properties for Risk_XCcyBasis entries.");

            if (!Enum.TryParse(o.ProductClass, out Model.Product product))
                throw new InvalidDataException("The CRIF file contains a Risk_XCcyBasis entry with an invalid ProductClass property.");

            if (product != Model.Product.RatesFx)
                throw new InvalidDataException($"The CRIF file contains a Risk_XCcyBasis entry associated to a product class other than {Model.Product.RatesFx}.");

            if (!Currency.TryParse(o.Qualifier, out Currency currency))
                throw new InvalidDataException("The CRIF file contains a Risk_XCcyBasis entry with an invalid Qualifier property.");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            return Sensitivity.CrossCurrencyBasis(currency, amount, regulationsInfo, tradeInfo);
        }

        private static Sensitivity ReadSensitivityEquity(EntryObject o)
        {
            if (!Enum.TryParse(o.ProductClass, out Model.Product product))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid ProductClass property.");

            if (product != Model.Product.Equity)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry associated to a product class other than {Model.Product.Equity}.");

            if (!DataValidator.IsValidQualifier(o.Qualifier, false))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Qualifier property.");

            if (!BucketEquity.TryParse(o.Bucket, out BucketEquity bucket))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Bucket property.");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            if (o.RiskType == "Risk_Equity")
            {
                if (!String.IsNullOrEmpty(o.Label1) || !String.IsNullOrEmpty(o.Label2))
                    throw new InvalidDataException("The CRIF file cannot specify Label1 and Label2 properties for Risk_Equity entries.");

                return Sensitivity.EquityDelta(o.Qualifier, bucket, amount, regulationsInfo, tradeInfo);
            }

            if (!Tenor.TryParse(o.Label1, out Tenor tenor))
                throw new InvalidDataException("The CRIF file contains a Risk_EquityVol entry with an invalid Label1 property.");

            if (!String.IsNullOrEmpty(o.Label2))
                throw new InvalidDataException("The CRIF file cannot specify the Label2 property for Risk_EquityVol entries.");

            return Sensitivity.EquityVega(o.Qualifier, bucket, tenor, amount, regulationsInfo, tradeInfo);
        }

        private static Sensitivity ReadSensitivityFx(EntryObject o)
        {
            if (!Enum.TryParse(o.ProductClass, out Model.Product product))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid ProductClass property.");

            if (product != Model.Product.RatesFx)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry associated to a product class other than {Model.Product.RatesFx}.");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            if (o.RiskType == "Risk_FX")
            {
                if (!Currency.TryParse(o.Qualifier, out Currency currency))
                    throw new InvalidDataException("The CRIF file contains a Risk_FX entry with an invalid Qualifier property.");

                if (!String.IsNullOrEmpty(o.Bucket) || !String.IsNullOrEmpty(o.Label1) || !String.IsNullOrEmpty(o.Label2))
                    throw new InvalidDataException("The CRIF file cannot specify Bucket, Label1 and Label2 properties for Risk_FX entries.");

                return Sensitivity.FxDelta(currency, amount, regulationsInfo, tradeInfo);
            }

            if (!CurrencyPair.TryParse(o.Qualifier, out CurrencyPair pair))
                throw new InvalidDataException("The CRIF file contains a Risk_FXVol entry with an invalid Qualifier property.");

            if (!Tenor.TryParse(o.Label1, out Tenor tenor))
                throw new InvalidDataException("The CRIF file contains a Risk_FXVol entry with an invalid Label1 property.");

            if (!String.IsNullOrEmpty(o.Bucket) || !String.IsNullOrEmpty(o.Label2))
                throw new InvalidDataException("The CRIF file cannot specify the Label2 property for Risk_EquityVol entries.");

            return Sensitivity.FxVega(pair, tenor, amount, regulationsInfo, tradeInfo);
        }

        private static Sensitivity ReadSensitivityInflation(EntryObject o)
        {
            if (!Enum.TryParse(o.ProductClass, out Model.Product product))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid ProductClass property.");

            if (product != Model.Product.RatesFx)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry associated to a product class other than {Model.Product.RatesFx}.");

            if (!Currency.TryParse(o.Qualifier, out Currency currency))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Qualifier property.");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            if (o.RiskType == "Risk_Inflation")
            {
                if (!String.IsNullOrEmpty(o.Bucket) || !String.IsNullOrEmpty(o.Label1) || !String.IsNullOrEmpty(o.Label2))
                    throw new InvalidDataException("The CRIF file cannot specify Bucket, Label1 and Label2 properties for Risk_Inflation entries.");

                return Sensitivity.InflationDelta(currency, amount, regulationsInfo, tradeInfo);
            }

            if (!Tenor.TryParse(o.Label1, out Tenor tenor))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Label1 property.");

            if (!String.IsNullOrEmpty(o.Bucket) || !String.IsNullOrEmpty(o.Label2))
                throw new InvalidDataException("The CRIF file cannot specify Bucket and Label2 properties for Risk_InflationVol entries.");

            return Sensitivity.InflationVega(currency, tenor, amount, regulationsInfo, tradeInfo);
        }

        private static Sensitivity ReadSensitivityInterestRate(EntryObject o)
        {
            if (!Enum.TryParse(o.ProductClass, out Model.Product product))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid ProductClass property.");

            if (product != Model.Product.RatesFx)
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry associated to a product class other than {Model.Product.RatesFx}.");

            if (!Currency.TryParse(o.Qualifier, out Currency currency))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Qualifier property.");

            if (!Tenor.TryParse(o.Label1, out Tenor tenor))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid Label1 property.");

            Amount amount = ReadAmount(o);
            RegulationsInfo regulationsInfo = ReadRegulationsInfo(o);
            TradeInfo tradeInfo = ReadTradeInfo(o);

            if (o.RiskType == "Risk_IRCurve")
            {
                if (String.IsNullOrEmpty(o.Bucket))
                    throw new InvalidDataException("The CRIF file contains a Risk_IRCurve entry with an invalid Bucket property.");

                String currencyVolatility = ((Int32)currency.Volatility + 1).ToString();

                if (o.Bucket != currencyVolatility)
                    throw new InvalidDataException($"The CRIF file contains a Risk_IRCurve entry with mismatching currency volatility and volatility bucket (for {currency} the Bucket property must be set to {currencyVolatility}).");

                o.Label2 = DataValidator.FormatLibor(o.Label2, true);

                if (!Enum.TryParse(o.Label2, out Curve curve))
                    throw new InvalidDataException("The CRIF file contains a Risk_IRCurve entry with an invalid Label2 property.");

                if ((currency != Currency.Usd) && ((curve == Curve.Municipal) || (curve == Curve.Prime)))
                    throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with a {curve} curve associated to a currency other than {Currency.Usd}.");

                return Sensitivity.InterestRateDelta(currency, tenor, curve, amount, regulationsInfo, tradeInfo);
            }

            if (!String.IsNullOrEmpty(o.Bucket) || !String.IsNullOrEmpty(o.Label2))
                throw new InvalidDataException("The CRIF file cannot specify Bucket and Label2 properties for Risk_IRVol entries.");

            return Sensitivity.InterestRateVega(currency, tenor, amount, regulationsInfo, tradeInfo);
        }

        private static TradeInfo ReadTradeInfo(EntryObject o)
        {
            if (String.IsNullOrEmpty(o.PortfolioId))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid portfolio identifier.");

            if (String.IsNullOrEmpty(o.TradeId))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid trade identifier.");

            if (!DateTime.TryParseExact(o.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
                throw new InvalidDataException($"The CRIF file contains a {o.RiskType} entry with an invalid EndDate property.");

            return TradeInfo.Of(o.PortfolioId, o.TradeId, endDate);
        }

        public static ICollection<DataEntity> Read(String filePath)
        {
            List<String[]> fieldsMatrix = CsvParser.Parse(filePath, Encoding.UTF8, '\t', false);

            if (fieldsMatrix.Count == 0)
                throw new InvalidDataException($"[{filePath}] The CRIF file is empty.");

            String[] properties = fieldsMatrix.ElementAt(0);
            fieldsMatrix = fieldsMatrix.Skip(1).ToList();

            if (properties.Any(String.IsNullOrWhiteSpace))
                throw new InvalidDataException($"[{filePath}] The CRIF file contains empty column headers.");

            if (s_Properties.Intersect(properties).Count() != s_Properties.Length)
                throw new InvalidDataException($"[{filePath}] The CRIF file does not contain all the required column headers: {String.Join(", ", s_Properties)}.");

            Dictionary<String,Int32> propertiesMapping = new Dictionary<String,Int32>(s_Properties.Length);

            foreach (String property in s_Properties)
                propertiesMapping.Add(property, Array.IndexOf(properties, property));

            List<DataEntity> entities = new List<DataEntity>();

            foreach (var tuple in fieldsMatrix.Select((x, i) => new { Index = i, Value = x }))
            {
                Int32 index = tuple.Index + 2;
                String[] values = tuple.Value;

                if (values.Length != properties.Length)
                    throw new InvalidDataException($"[{filePath}, Line {index}] The CRIF file contains an entry whose number of columns doesn't match the number of column headers.");

                EntryObject o = ToEntryObject(values, propertiesMapping);

                if ((o.InitialMarginModel != "Schedule") && (o.InitialMarginModel != "SIMM"))
                    throw new InvalidDataException($"[{filePath}, Line {index}] The CRIF file contains an entry with an invalid IMModel property.");

                DataEntity entity = null;

                try
                {
                    switch (o.RiskType)
                    {
                        case "Notional":
                        {
                            if (o.InitialMarginModel == "SIMM")
                                entity = ReadAddOnNotional(o);
                            else
                                entity = ReadNotional(o);
                            
                            break;
                        }

                        case "Param_AddOnFixedAmount":
                            entity = ReadAddOnFixedAmount(o);
                            break;

                        case "Param_AddOnNotionalFactor":
                            entity = ReadAddOnNotionalFactor(o);
                            break;

                        case "Param_ProductClassMultiplier":
                            entity = ReadProductMultiplier(o);
                            break;

                        case "PV":
                            entity = ReadPresentValue(o);
                            break;

                        case "Risk_BaseCorr":
                            entity = ReadSensitivityBaseCorrelation(o);
                            break;

                        case "Risk_Commodity":
                        case "Risk_CommodityVol":
                            entity = ParseSensitivityCommodity(o);
                            break;

                        case "Risk_CreditNonQ":
                        case "Risk_CreditVolNonQ":
                            entity = ParseSensitivityCreditNonQualifying(o);
                            break;

                        case "Risk_CreditQ":
                        case "Risk_CreditVol":
                            entity = ParseSensitivityCreditQualifying(o);
                            break;

                        case "Risk_Equity":
                        case "Risk_EquityVol":
                            entity = ReadSensitivityEquity(o);
                            break;

                        case "Risk_FX":
                        case "Risk_FXVol":
                            entity = ReadSensitivityFx(o);
                            break;

                        case "Risk_Inflation":
                        case "Risk_InflationVol":
                            entity = ReadSensitivityInflation(o);
                            break;

                        case "Risk_IRCurve":
                        case "Risk_IRVol":
                            entity = ReadSensitivityInterestRate(o);
                            break;

                        case "Risk_XCcyBasis":
                            entity = ReadSensitivityCrossCurrencyBasis(o);
                            break;
                    }
                }
                catch (InvalidDataException e)
                {
                    throw new InvalidDataException($"[{filePath}, Line {index}] {e.Message}");
                }

                if (entity == null)
                    throw new InvalidDataException($"[{filePath}, Line {index}] The CRIF file contains an entry with an invalid RiskType property.");

                entities.Add(entity);
            }

            return entities;
        }

        private static void WriteDataEntity(DataEntity dataEntity, List<String[]> fieldsMatrix)
        {
            String[] fieldsRow = Enumerable.Repeat(String.Empty, s_Properties.Length).ToArray();
            fieldsRow[10] = String.Join(",", dataEntity.CollectRegulations.Select(x => x.ToString().ToUpperInvariant()));
            fieldsRow[11] = String.Join(",", dataEntity.PostRegulations.Select(x => x.ToString().ToUpperInvariant()));

            if (dataEntity is DataParameter dataParameter)
            {
                fieldsRow[1] = "SIMM";

                switch (dataParameter)
                {
                    case AddOnNotionalFactor addOnNotionalFactor:
                        fieldsRow[0] = "Param_AddOnNotionalFactor";
                        fieldsRow[3] = addOnNotionalFactor.Qualifier;
                        fieldsRow[7] = (dataParameter.Parameter * 100m).Normalize().ToString(CultureInfo.InvariantCulture);
                        break;

                    case AddOnProductMultiplier addOnProductMultiplier:
                        fieldsRow[0] = "Param_ProductClassMultiplier";
                        fieldsRow[3] = addOnProductMultiplier.Product.ToString();
                        fieldsRow[7] = (dataParameter.Parameter + 1m).Normalize().ToString(CultureInfo.InvariantCulture);
                        break;

                    default:
                        throw new InvalidDataException($"Invalid data entity type {dataEntity.GetType().Name}.");
                }
            }
            else if (dataEntity is DataValue dataValue)
            {
                Amount amount = dataValue.Amount;

                if (amount.Currency != Currency.Usd)
                {
                    fieldsRow[7] = amount.ToString(CultureInfo.InvariantCulture, CurrencyCodeSymbol.None);
                    fieldsRow[8] = amount.Currency.ToString().ToUpperInvariant();
                }
                else
                    fieldsRow[9] = amount.ToString(CultureInfo.InvariantCulture, CurrencyCodeSymbol.None);

                switch (dataEntity)
                {
                    case AddOnFixedAmount _:
                        fieldsRow[0] = "Param_AddOnFixedAmount";
                        fieldsRow[1] = "SIMM";
                        break;

                    case AddOnNotional addOnNotional:
                        fieldsRow[0] = "Notional";
                        fieldsRow[1] = "SIMM";
                        fieldsRow[3] = addOnNotional.Qualifier;
                        fieldsRow[12] = addOnNotional.PortfolioId;
                        fieldsRow[13] = addOnNotional.TradeId;
                        fieldsRow[14] = addOnNotional.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        break;

                    case Notional notional:
                        fieldsRow[0] = "Notional";
                        fieldsRow[1] = "Schedule";
                        fieldsRow[2] = notional.Product.ToString();
                        fieldsRow[12] = notional.PortfolioId;
                        fieldsRow[13] = notional.TradeId;
                        fieldsRow[14] = notional.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        break;

                    case PresentValue presentValue:
                        fieldsRow[0] = "PV";
                        fieldsRow[1] = "Schedule";
                        fieldsRow[2] = presentValue.Product.ToString();
                        fieldsRow[12] = presentValue.PortfolioId;
                        fieldsRow[13] = presentValue.TradeId;
                        fieldsRow[14] = presentValue.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        break;

                    case Sensitivity sensitivity:
                    {
                        fieldsRow[0] = sensitivity.Identifier;
                        fieldsRow[1] = "SIMM";
                        fieldsRow[2] = sensitivity.Product.ToString();
                        fieldsRow[3] = sensitivity.Qualifier.Replace("/", String.Empty);

                        if (sensitivity.Bucket is Currency currency)
                        {
                            if (sensitivity.Identifier == "Risk_IRCurve")
                                fieldsRow[4] = ((Int32)currency.Volatility + 1).ToString();
                        }
                        else if (!(sensitivity.Bucket is Placeholder))
                            fieldsRow[4] = sensitivity.Bucket.ToString();

                        fieldsRow[5] = sensitivity.Label1;
                        fieldsRow[6] = DataValidator.FormatLibor(sensitivity.Label2, false);
                        fieldsRow[12] = sensitivity.PortfolioId;
                        fieldsRow[13] = sensitivity.TradeId;
                        fieldsRow[14] = sensitivity.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        
                        break;
                    }

                    default:
                        throw new InvalidDataException($"Invalid data entity type {dataEntity.GetType().Name}.");
                }
            }

            fieldsMatrix.Add(fieldsRow);
        }

        public static void Write(String filePath, ICollection<DataEntity> dataEntities)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid file path specified.", nameof(filePath));

            if (dataEntities == null)
                throw new ArgumentNullException(nameof(dataEntities));

            if (dataEntities.Any(x => x == null))
                throw new ArgumentException("One or more data entities are null.", nameof(dataEntities));

            if (dataEntities.Any(x => (x is Sensitivity sensitivity) && (String.IsNullOrEmpty(sensitivity.Identifier) || (sensitivity.Category == SensitivityCategory.Curvature))))
                throw new ArgumentException("The specified data entities contain sensitivities produced by a transformation process.", nameof(dataEntities));

            List<String[]> fieldsMatrix = new List<String[]>(dataEntities.Count);
            
            foreach (DataEntity dataEntity in dataEntities)
                WriteDataEntity(dataEntity, fieldsMatrix);

            String result = CsvUtilities.FinalizeFieldsMatrix(s_Properties, fieldsMatrix, '\t', "\n");

            File.WriteAllText(filePath, result, Encoding.UTF8);
        }
        #endregion

        #region Nesting (Structures)
        private struct EntryObject
        {
            #region Members
            public String Amount;
            public String AmountCurrency;
            public String AmountUsd;
            public String Bucket;
            public String CollectRegulations;
            public String EndDate;
            public String InitialMarginModel;
            public String Label1;
            public String Label2;
            public String PortfolioId;
            public String PostRegulations;
            public String ProductClass;
            public String Qualifier;
            public String RiskType;
            public String TradeId;
            #endregion
        }
        #endregion
    }
}