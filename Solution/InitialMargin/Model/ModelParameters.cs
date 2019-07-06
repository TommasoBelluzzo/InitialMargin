#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InitialMargin.Core;
#endregion

namespace InitialMargin.Model
{
    internal static class ModelParameters
    {
        #region Parameters
        private const Decimal CORRELATION_BASE = 0.05m;
        private const Decimal CURVATURE_CAP = 1.00m;
        private const Decimal CURVATURE_DAYS = 14.00m;
        private const Decimal CURVATURE_FACTOR = 0.50m;
        private const Decimal HVR_CURVATURE = 1.00m;
        private const Decimal MM = 1000000.00m;
        private const Decimal RISKWEIGHT_BASECORRELATION = 19.00m;
        private const Decimal RISKWEIGHT_CURVATURE = 1.00m;
        private const Decimal VOLATILITYWEIGHT_DEFAULT = 1.00m;
        private const Decimal Z_SCORE_990 = 2.32634787404084110089m;
        private const Decimal Z_SCORE_995 = 2.57582930354890076098m;
        
        private static readonly Decimal VOLATILITY_FACTOR = MathUtilities.SquareRoot(Calendar.DaysPerYear / CURVATURE_DAYS) / Z_SCORE_990;
        
        private static readonly Dictionary<SensitivityRisk,Dictionary<SensitivityRisk,Decimal>> CORRELATIONS_RISK = new Dictionary<SensitivityRisk,Dictionary<SensitivityRisk,Decimal>>
        {
            [SensitivityRisk.Commodity] = new Dictionary<SensitivityRisk,Decimal>
            {
                [SensitivityRisk.Commodity] = 1.00m,
                [SensitivityRisk.CreditNonQualifying] = 0.22m,
                [SensitivityRisk.CreditQualifying] = 0.45m,
                [SensitivityRisk.Equity] = 0.39m,
                [SensitivityRisk.Fx] = 0.32m,
                [SensitivityRisk.Rates] = 0.30m
            },
            [SensitivityRisk.CreditNonQualifying] = new Dictionary<SensitivityRisk,Decimal>
            {
                [SensitivityRisk.Commodity] = 0.22m,
                [SensitivityRisk.CreditNonQualifying] = 1.00m,
                [SensitivityRisk.CreditQualifying] = 0.26m,
                [SensitivityRisk.Equity] = 0.17m,
                [SensitivityRisk.Fx] = 0.11m,
                [SensitivityRisk.Rates] = 0.15m
            },
            [SensitivityRisk.CreditQualifying] = new Dictionary<SensitivityRisk,Decimal>
            {
                [SensitivityRisk.Commodity] = 0.45m,
                [SensitivityRisk.CreditNonQualifying] = 0.26m,
                [SensitivityRisk.CreditQualifying] = 1.00m,
                [SensitivityRisk.Equity] = 0.65m,
                [SensitivityRisk.Fx] = 0.24m,
                [SensitivityRisk.Rates] = 0.26m
            },
            [SensitivityRisk.Equity] = new Dictionary<SensitivityRisk,Decimal>
            {
                [SensitivityRisk.Commodity] = 0.39m,
                [SensitivityRisk.CreditNonQualifying] = 0.17m,
                [SensitivityRisk.CreditQualifying] = 0.65m,
                [SensitivityRisk.Equity] = 1.00m,
                [SensitivityRisk.Fx] = 0.23m,
                [SensitivityRisk.Rates] = 0.19m
            },
            [SensitivityRisk.Fx] = new Dictionary<SensitivityRisk,Decimal>
            {
                [SensitivityRisk.Commodity] = 0.32m,
                [SensitivityRisk.CreditNonQualifying] = 0.11m,
                [SensitivityRisk.CreditQualifying] = 0.24m,
                [SensitivityRisk.Equity] = 0.23m,
                [SensitivityRisk.Fx] = 1.00m,
                [SensitivityRisk.Rates] = 0.26m
            },
            [SensitivityRisk.Rates] = new Dictionary<SensitivityRisk,Decimal>
            {
                [SensitivityRisk.Commodity] = 0.30m,
                [SensitivityRisk.CreditNonQualifying] = 0.15m,
                [SensitivityRisk.CreditQualifying] = 0.26m,
                [SensitivityRisk.Equity] = 0.19m,
                [SensitivityRisk.Fx] = 0.26m,
                [SensitivityRisk.Rates] = 1.00m
            }
        };

        private static readonly Dictionary<SensitivityRisk,Decimal> HVRS_VEGA = new Dictionary<SensitivityRisk,Decimal>
        {
            [SensitivityRisk.Commodity] = 0.74m,
            [SensitivityRisk.Equity] = 0.59m,
            [SensitivityRisk.Fx] = 0.63m,
            [SensitivityRisk.Rates] = 0.62m
        };
        #endregion

        #region Members
        private static readonly Dictionary<SensitivityRisk,ParametersProvider> s_Providers = new Dictionary<SensitivityRisk,ParametersProvider>
        {
            [SensitivityRisk.Commodity] = new ParametersProviderCommodity(),
            [SensitivityRisk.CreditNonQualifying] = new ParametersProviderCreditNonQualifying(),
            [SensitivityRisk.CreditQualifying] = new ParametersProviderCreditQualifying(),
            [SensitivityRisk.Equity] = new ParametersProviderEquity(),
            [SensitivityRisk.Fx] = new ParametersProviderFx(),
            [SensitivityRisk.Rates] = new ParametersProviderRates()
        };
        #endregion

        #region Methods
        public static Amount GetThreshold(SensitivityRisk risk, SensitivityCategory category, IThresholdIdentifier thresholdIdentifier)
        {
            if (!Enum.IsDefined(typeof(SensitivityCategory), category))
                throw new InvalidEnumArgumentException("Invalid sensitivity category specified.");

            if (!Enum.IsDefined(typeof(SensitivityRisk), risk))
                throw new InvalidEnumArgumentException("Invalid sensitivity risk specified.");

            if (thresholdIdentifier == null)
                throw new ArgumentNullException(nameof(thresholdIdentifier));

            switch (category)
            {
                case SensitivityCategory.Delta:
                    return Amount.Of(Currency.Usd, (s_Providers[risk].GetThresholdDelta(thresholdIdentifier) * MM));

                case SensitivityCategory.Vega:
                    return Amount.Of(Currency.Usd, (s_Providers[risk].GetThresholdVega(thresholdIdentifier) * MM));

                default:
                    return Amount.Zero(Currency.Usd);
            }
        }

        public static Decimal GetCorrelationBase(Sensitivity sensitivity1, Sensitivity sensitivity2)
        {
            if (sensitivity1 == null)
                throw new ArgumentNullException(nameof(sensitivity1));

            if (sensitivity2 == null)
                throw new ArgumentNullException(nameof(sensitivity2));

            return CORRELATION_BASE;
        }

        public static Decimal GetCorrelationBucket(SensitivityRisk risk, IBucket bucket1, IBucket bucket2)
        {
            if (!Enum.IsDefined(typeof(SensitivityRisk), risk))
                throw new InvalidEnumArgumentException("Invalid sensitivity risk specified.");

            return s_Providers[risk].GetCorrelationBucket(bucket1, bucket2);
        }

        public static Decimal GetCorrelationRisk(SensitivityRisk risk1, SensitivityRisk risk2)
        {
            if (!Enum.IsDefined(typeof(SensitivityRisk), risk1))
                throw new InvalidEnumArgumentException("Invalid sensitivity risk specified.");

            if (!Enum.IsDefined(typeof(SensitivityRisk), risk2))
                throw new InvalidEnumArgumentException("Invalid sensitivity risk specified.");

            return CORRELATIONS_RISK[risk1][risk2];
        }

        public static Decimal GetCorrelationSensitivity(SensitivityRisk risk, Sensitivity sensitivity1, Sensitivity sensitivity2)
        {
            if (!Enum.IsDefined(typeof(SensitivityRisk), risk))
                throw new InvalidEnumArgumentException("Invalid sensitivity risk specified.");

            return s_Providers[risk].GetCorrelationSensitivity(sensitivity1, sensitivity2);
        }

        public static Decimal GetCurvatureLambda(Decimal theta)
        {
            return (((MathUtilities.Square(Z_SCORE_995) - 1.00m) * (1.00m + theta)) - theta);
        }

        public static Decimal GetCurvatureScaledDays(Decimal days)
        {
            if (days <= 0m)
                throw new ArgumentOutOfRangeException(nameof(days), "Invalid number of days specified.");

            return (CURVATURE_FACTOR * Math.Min(CURVATURE_CAP, CURVATURE_DAYS / days));
        }

        public static Decimal GetCurvatureScaleFactor(SensitivityRisk risk)
        {
            if (!Enum.IsDefined(typeof(SensitivityRisk), risk))
                throw new InvalidEnumArgumentException("Invalid sensitivity risk specified.");

            if (risk != SensitivityRisk.Rates)
                return 1.00m;

            return (1.00m / MathUtilities.Square(HVRS_VEGA[risk]));
        }

        public static Decimal GetWeightRisk(SensitivityCategory category, Sensitivity sensitivity)
        {
            if (!Enum.IsDefined(typeof(SensitivityCategory), category))
                throw new InvalidEnumArgumentException("Invalid sensitivity category specified.");

            if (sensitivity == null)
                throw new ArgumentNullException(nameof(sensitivity));

            switch (category)
            {
                case SensitivityCategory.Curvature:
                    return RISKWEIGHT_CURVATURE;

                case SensitivityCategory.Delta:
                    return s_Providers[sensitivity.Risk].GetRiskWeightDelta(sensitivity);

                case SensitivityCategory.Vega:
                    return s_Providers[sensitivity.Risk].GetRiskWeightVega(sensitivity);

                default:
                    return RISKWEIGHT_BASECORRELATION;
            }
        }

        public static Decimal GetWeightVolatility(Sensitivity sensitivity)
        {
            if (sensitivity == null)
                throw new ArgumentNullException(nameof(sensitivity));

            if ((sensitivity.Product == Product.Credit) || (sensitivity.Risk == SensitivityRisk.Rates))
                return VOLATILITYWEIGHT_DEFAULT;

            Decimal hvr = (sensitivity.Category == SensitivityCategory.Vega) ? HVRS_VEGA[sensitivity.Risk] : HVR_CURVATURE;
            Decimal volatilityWeight = hvr * VOLATILITY_FACTOR * s_Providers[sensitivity.Risk].GetRiskWeightDelta(sensitivity);

            return volatilityWeight;
        }
        #endregion

        #region Nesting (Classes)
        private abstract class ParametersProvider
        {
            #region Methods
            public abstract Decimal GetCorrelationBucket(IBucket bucket1, IBucket bucket2);

            public abstract Decimal GetCorrelationSensitivity(Sensitivity sensitivity1, Sensitivity sensitivity2);

            public abstract Decimal GetRiskWeightDelta(Sensitivity sensitivity);

            public abstract Decimal GetRiskWeightVega(Sensitivity sensitivity);

            public abstract Decimal GetThresholdDelta(IThresholdIdentifier thresholdIdentifier);

            public abstract Decimal GetThresholdVega(IThresholdIdentifier thresholdIdentifier);
            #endregion
        }

        private sealed class ParametersProviderCommodity : ParametersProvider
        {
            #region Parameters
            private const Decimal RISKWEIGHT_VEGA = 0.27m;

            private static readonly Dictionary<IBucket,Dictionary<IBucket,Decimal>> CORRELATIONS_BUCKET = new Dictionary<IBucket,Dictionary<IBucket,Decimal>>
            {
                [BucketCommodity.Bucket1] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 1.00m,
                    [BucketCommodity.Bucket2] = 0.16m,
                    [BucketCommodity.Bucket3] = 0.11m,
                    [BucketCommodity.Bucket4] = 0.19m,
                    [BucketCommodity.Bucket5] = 0.22m,
                    [BucketCommodity.Bucket6] = 0.12m,
                    [BucketCommodity.Bucket7] = 0.22m,
                    [BucketCommodity.Bucket8] = 0.02m,
                    [BucketCommodity.Bucket9] = 0.27m,
                    [BucketCommodity.Bucket10] = 0.08m,
                    [BucketCommodity.Bucket11] = 0.11m,
                    [BucketCommodity.Bucket12] = 0.05m,
                    [BucketCommodity.Bucket13] = 0.04m,
                    [BucketCommodity.Bucket14] = 0.06m,
                    [BucketCommodity.Bucket15] = 0.01m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.10m
                },
                [BucketCommodity.Bucket2] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.16m,
                    [BucketCommodity.Bucket2] = 1.00m,
                    [BucketCommodity.Bucket3] = 0.89m,
                    [BucketCommodity.Bucket4] = 0.94m,
                    [BucketCommodity.Bucket5] = 0.93m,
                    [BucketCommodity.Bucket6] = 0.32m,
                    [BucketCommodity.Bucket7] = 0.24m,
                    [BucketCommodity.Bucket8] = 0.19m,
                    [BucketCommodity.Bucket9] = 0.21m,
                    [BucketCommodity.Bucket10] = 0.06m,
                    [BucketCommodity.Bucket11] = 0.39m,
                    [BucketCommodity.Bucket12] = 0.23m,
                    [BucketCommodity.Bucket13] = 0.39m,
                    [BucketCommodity.Bucket14] = 0.29m,
                    [BucketCommodity.Bucket15] = 0.13m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.66m
                },
                [BucketCommodity.Bucket3] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.11m,
                    [BucketCommodity.Bucket2] = 0.89m,
                    [BucketCommodity.Bucket3] = 1.00m,
                    [BucketCommodity.Bucket4] = 0.87m,
                    [BucketCommodity.Bucket5] = 0.88m,
                    [BucketCommodity.Bucket6] = 0.17m,
                    [BucketCommodity.Bucket7] = 0.17m,
                    [BucketCommodity.Bucket8] = 0.13m,
                    [BucketCommodity.Bucket9] = 0.12m,
                    [BucketCommodity.Bucket10] = 0.03m,
                    [BucketCommodity.Bucket11] = 0.24m,
                    [BucketCommodity.Bucket12] = 0.04m,
                    [BucketCommodity.Bucket13] = 0.27m,
                    [BucketCommodity.Bucket14] = 0.19m,
                    [BucketCommodity.Bucket15] = 0.08m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.61m
                },
                [BucketCommodity.Bucket4] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.19m,
                    [BucketCommodity.Bucket2] = 0.94m,
                    [BucketCommodity.Bucket3] = 0.87m,
                    [BucketCommodity.Bucket4] = 1.00m,
                    [BucketCommodity.Bucket5] = 0.92m,
                    [BucketCommodity.Bucket6] = 0.37m,
                    [BucketCommodity.Bucket7] = 0.27m,
                    [BucketCommodity.Bucket8] = 0.21m,
                    [BucketCommodity.Bucket9] = 0.21m,
                    [BucketCommodity.Bucket10] = 0.03m,
                    [BucketCommodity.Bucket11] = 0.36m,
                    [BucketCommodity.Bucket12] = 0.16m,
                    [BucketCommodity.Bucket13] = 0.27m,
                    [BucketCommodity.Bucket14] = 0.28m,
                    [BucketCommodity.Bucket15] = 0.09m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.64m
                },
                [BucketCommodity.Bucket5] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.22m,
                    [BucketCommodity.Bucket2] = 0.93m,
                    [BucketCommodity.Bucket3] = 0.88m,
                    [BucketCommodity.Bucket4] = 0.92m,
                    [BucketCommodity.Bucket5] = 1.00m,
                    [BucketCommodity.Bucket6] = 0.29m,
                    [BucketCommodity.Bucket7] = 0.26m,
                    [BucketCommodity.Bucket8] = 0.19m,
                    [BucketCommodity.Bucket9] = 0.23m,
                    [BucketCommodity.Bucket10] = 0.10m,
                    [BucketCommodity.Bucket11] = 0.40m,
                    [BucketCommodity.Bucket12] = 0.27m,
                    [BucketCommodity.Bucket13] = 0.38m,
                    [BucketCommodity.Bucket14] = 0.30m,
                    [BucketCommodity.Bucket15] = 0.15m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.64m
                },
                [BucketCommodity.Bucket6] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.12m,
                    [BucketCommodity.Bucket2] = 0.32m,
                    [BucketCommodity.Bucket3] = 0.17m,
                    [BucketCommodity.Bucket4] = 0.37m,
                    [BucketCommodity.Bucket5] = 0.29m,
                    [BucketCommodity.Bucket6] = 1.00m,
                    [BucketCommodity.Bucket7] = 0.19m,
                    [BucketCommodity.Bucket8] = 0.60m,
                    [BucketCommodity.Bucket9] = 0.18m,
                    [BucketCommodity.Bucket10] = 0.09m,
                    [BucketCommodity.Bucket11] = 0.22m,
                    [BucketCommodity.Bucket12] = 0.09m,
                    [BucketCommodity.Bucket13] = 0.14m,
                    [BucketCommodity.Bucket14] = 0.16m,
                    [BucketCommodity.Bucket15] = 0.10m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.37m
                },
                [BucketCommodity.Bucket7] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.22m,
                    [BucketCommodity.Bucket2] = 0.24m,
                    [BucketCommodity.Bucket3] = 0.17m,
                    [BucketCommodity.Bucket4] = 0.27m,
                    [BucketCommodity.Bucket5] = 0.26m,
                    [BucketCommodity.Bucket6] = 0.19m,
                    [BucketCommodity.Bucket7] = 1.00m,
                    [BucketCommodity.Bucket8] = 0.06m,
                    [BucketCommodity.Bucket9] = 0.68m,
                    [BucketCommodity.Bucket10] = 0.16m,
                    [BucketCommodity.Bucket11] = 0.21m,
                    [BucketCommodity.Bucket12] = 0.10m,
                    [BucketCommodity.Bucket13] = 0.24m,
                    [BucketCommodity.Bucket14] = 0.25m,
                    [BucketCommodity.Bucket15] = -0.01m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.27m
                },
                [BucketCommodity.Bucket8] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.02m,
                    [BucketCommodity.Bucket2] = 0.19m,
                    [BucketCommodity.Bucket3] = 0.13m,
                    [BucketCommodity.Bucket4] = 0.21m,
                    [BucketCommodity.Bucket5] = 0.19m,
                    [BucketCommodity.Bucket6] = 0.60m,
                    [BucketCommodity.Bucket7] = 0.06m,
                    [BucketCommodity.Bucket8] = 1.00m,
                    [BucketCommodity.Bucket9] = 0.12m,
                    [BucketCommodity.Bucket10] = 0.01m,
                    [BucketCommodity.Bucket11] = 0.10m,
                    [BucketCommodity.Bucket12] = 0.03m,
                    [BucketCommodity.Bucket13] = 0.02m,
                    [BucketCommodity.Bucket14] = 0.07m,
                    [BucketCommodity.Bucket15] = 0.10m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.21m
                },
                [BucketCommodity.Bucket9] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.27m,
                    [BucketCommodity.Bucket2] = 0.21m,
                    [BucketCommodity.Bucket3] = 0.12m,
                    [BucketCommodity.Bucket4] = 0.21m,
                    [BucketCommodity.Bucket5] = 0.23m,
                    [BucketCommodity.Bucket6] = 0.18m,
                    [BucketCommodity.Bucket7] = 0.68m,
                    [BucketCommodity.Bucket8] = 0.12m,
                    [BucketCommodity.Bucket9] = 1.00m,
                    [BucketCommodity.Bucket10] = 0.05m,
                    [BucketCommodity.Bucket11] = 0.16m,
                    [BucketCommodity.Bucket12] = 0.03m,
                    [BucketCommodity.Bucket13] = 0.19m,
                    [BucketCommodity.Bucket14] = 0.16m,
                    [BucketCommodity.Bucket15] = -0.01m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.19m
                },
                [BucketCommodity.Bucket10] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.08m,
                    [BucketCommodity.Bucket2] = 0.06m,
                    [BucketCommodity.Bucket3] = 0.03m,
                    [BucketCommodity.Bucket4] = 0.03m,
                    [BucketCommodity.Bucket5] = 0.10m,
                    [BucketCommodity.Bucket6] = 0.09m,
                    [BucketCommodity.Bucket7] = 0.16m,
                    [BucketCommodity.Bucket8] = 0.01m,
                    [BucketCommodity.Bucket9] = 0.05m,
                    [BucketCommodity.Bucket10] = 1.00m,
                    [BucketCommodity.Bucket11] = 0.08m,
                    [BucketCommodity.Bucket12] = 0.04m,
                    [BucketCommodity.Bucket13] = 0.05m,
                    [BucketCommodity.Bucket14] = 0.11m,
                    [BucketCommodity.Bucket15] = 0.02m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.00m
                },
                [BucketCommodity.Bucket11] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.11m,
                    [BucketCommodity.Bucket2] = 0.39m,
                    [BucketCommodity.Bucket3] = 0.24m,
                    [BucketCommodity.Bucket4] = 0.36m,
                    [BucketCommodity.Bucket5] = 0.40m,
                    [BucketCommodity.Bucket6] = 0.22m,
                    [BucketCommodity.Bucket7] = 0.21m,
                    [BucketCommodity.Bucket8] = 0.10m,
                    [BucketCommodity.Bucket9] = 0.16m,
                    [BucketCommodity.Bucket10] = 0.08m,
                    [BucketCommodity.Bucket11] = 1.00m,
                    [BucketCommodity.Bucket12] = 0.34m,
                    [BucketCommodity.Bucket13] = 0.19m,
                    [BucketCommodity.Bucket14] = 0.22m,
                    [BucketCommodity.Bucket15] = 0.15m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.34m
                },
                [BucketCommodity.Bucket12] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.05m,
                    [BucketCommodity.Bucket2] = 0.23m,
                    [BucketCommodity.Bucket3] = 0.04m,
                    [BucketCommodity.Bucket4] = 0.16m,
                    [BucketCommodity.Bucket5] = 0.27m,
                    [BucketCommodity.Bucket6] = 0.09m,
                    [BucketCommodity.Bucket7] = 0.10m,
                    [BucketCommodity.Bucket8] = 0.03m,
                    [BucketCommodity.Bucket9] = 0.03m,
                    [BucketCommodity.Bucket10] = 0.04m,
                    [BucketCommodity.Bucket11] = 0.34m,
                    [BucketCommodity.Bucket12] = 1.00m,
                    [BucketCommodity.Bucket13] = 0.14m,
                    [BucketCommodity.Bucket14] = 0.26m,
                    [BucketCommodity.Bucket15] = 0.09m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.20m
                },
                [BucketCommodity.Bucket13] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.04m,
                    [BucketCommodity.Bucket2] = 0.39m,
                    [BucketCommodity.Bucket3] = 0.27m,
                    [BucketCommodity.Bucket4] = 0.27m,
                    [BucketCommodity.Bucket5] = 0.38m,
                    [BucketCommodity.Bucket6] = 0.14m,
                    [BucketCommodity.Bucket7] = 0.24m,
                    [BucketCommodity.Bucket8] = 0.02m,
                    [BucketCommodity.Bucket9] = 0.19m,
                    [BucketCommodity.Bucket10] = 0.05m,
                    [BucketCommodity.Bucket11] = 0.19m,
                    [BucketCommodity.Bucket12] = 0.14m,
                    [BucketCommodity.Bucket13] = 1.00m,
                    [BucketCommodity.Bucket14] = 0.30m,
                    [BucketCommodity.Bucket15] = 0.16m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.40m
                },
                [BucketCommodity.Bucket14] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.06m,
                    [BucketCommodity.Bucket2] = 0.29m,
                    [BucketCommodity.Bucket3] = 0.19m,
                    [BucketCommodity.Bucket4] = 0.28m,
                    [BucketCommodity.Bucket5] = 0.30m,
                    [BucketCommodity.Bucket6] = 0.16m,
                    [BucketCommodity.Bucket7] = 0.25m,
                    [BucketCommodity.Bucket8] = 0.07m,
                    [BucketCommodity.Bucket9] = 0.16m,
                    [BucketCommodity.Bucket10] = 0.11m,
                    [BucketCommodity.Bucket11] = 0.22m,
                    [BucketCommodity.Bucket12] = 0.26m,
                    [BucketCommodity.Bucket13] = 0.30m,
                    [BucketCommodity.Bucket14] = 1.00m,
                    [BucketCommodity.Bucket15] = 0.09m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.30m
                },
                [BucketCommodity.Bucket15] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.01m,
                    [BucketCommodity.Bucket2] = 0.13m,
                    [BucketCommodity.Bucket3] = 0.08m,
                    [BucketCommodity.Bucket4] = 0.09m,
                    [BucketCommodity.Bucket5] = 0.15m,
                    [BucketCommodity.Bucket6] = 0.10m,
                    [BucketCommodity.Bucket7] = -0.01m,
                    [BucketCommodity.Bucket8] = 0.10m,
                    [BucketCommodity.Bucket9] = -0.01m,
                    [BucketCommodity.Bucket10] = 0.02m,
                    [BucketCommodity.Bucket11] = 0.15m,
                    [BucketCommodity.Bucket12] = 0.09m,
                    [BucketCommodity.Bucket13] = 0.16m,
                    [BucketCommodity.Bucket14] = 0.09m,
                    [BucketCommodity.Bucket15] = 1.00m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 0.16m
                },
                [BucketCommodity.Bucket16] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.00m,
                    [BucketCommodity.Bucket2] = 0.00m,
                    [BucketCommodity.Bucket3] = 0.00m,
                    [BucketCommodity.Bucket4] = 0.00m,
                    [BucketCommodity.Bucket5] = 0.00m,
                    [BucketCommodity.Bucket6] = 0.00m,
                    [BucketCommodity.Bucket7] = 0.00m,
                    [BucketCommodity.Bucket8] = 0.00m,
                    [BucketCommodity.Bucket9] = 0.00m,
                    [BucketCommodity.Bucket10] = 0.00m,
                    [BucketCommodity.Bucket11] = 0.00m,
                    [BucketCommodity.Bucket12] = 0.00m,
                    [BucketCommodity.Bucket13] = 0.00m,
                    [BucketCommodity.Bucket14] = 0.00m,
                    [BucketCommodity.Bucket15] = 0.00m,
                    [BucketCommodity.Bucket16] = 1.00m,
                    [BucketCommodity.Bucket17] = 0.00m
                },
                [BucketCommodity.Bucket17] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCommodity.Bucket1] = 0.10m,
                    [BucketCommodity.Bucket2] = 0.66m,
                    [BucketCommodity.Bucket3] = 0.61m,
                    [BucketCommodity.Bucket4] = 0.64m,
                    [BucketCommodity.Bucket5] = 0.64m,
                    [BucketCommodity.Bucket6] = 0.37m,
                    [BucketCommodity.Bucket7] = 0.27m,
                    [BucketCommodity.Bucket8] = 0.21m,
                    [BucketCommodity.Bucket9] = 0.19m,
                    [BucketCommodity.Bucket10] = 0.00m,
                    [BucketCommodity.Bucket11] = 0.34m,
                    [BucketCommodity.Bucket12] = 0.20m,
                    [BucketCommodity.Bucket13] = 0.40m,
                    [BucketCommodity.Bucket14] = 0.30m,
                    [BucketCommodity.Bucket15] = 0.16m,
                    [BucketCommodity.Bucket16] = 0.00m,
                    [BucketCommodity.Bucket17] = 1.00m
                }
            };

            private static readonly Dictionary<IBucket,Decimal> CORRELATIONS_SENSITIVITY = new Dictionary<IBucket,Decimal>
            {
                [BucketCommodity.Bucket1] = 0.27m,
                [BucketCommodity.Bucket2] = 0.97m,
                [BucketCommodity.Bucket3] = 0.92m,
                [BucketCommodity.Bucket4] = 0.97m,
                [BucketCommodity.Bucket5] = 0.99m,
                [BucketCommodity.Bucket6] = 1.00m,
                [BucketCommodity.Bucket7] = 1.00m,
                [BucketCommodity.Bucket8] = 0.40m,
                [BucketCommodity.Bucket9] = 0.73m,
                [BucketCommodity.Bucket10] = 0.13m,
                [BucketCommodity.Bucket11] = 0.53m,
                [BucketCommodity.Bucket12] = 0.64m,
                [BucketCommodity.Bucket13] = 0.63m,
                [BucketCommodity.Bucket14] = 0.26m,
                [BucketCommodity.Bucket15] = 0.26m,
                [BucketCommodity.Bucket16] = 0.00m,
                [BucketCommodity.Bucket17] = 0.38m
            };

            private static readonly Dictionary<IBucket,Decimal> RISKWEIGHTS_DELTA = new Dictionary<IBucket,Decimal>
            {
                [BucketCommodity.Bucket1] = 19.00m,
                [BucketCommodity.Bucket2] = 20.00m,
                [BucketCommodity.Bucket3] = 17.00m,
                [BucketCommodity.Bucket4] = 19.00m,
                [BucketCommodity.Bucket5] = 24.00m,
                [BucketCommodity.Bucket6] = 22.00m,
                [BucketCommodity.Bucket7] = 26.00m,
                [BucketCommodity.Bucket8] = 50.00m,
                [BucketCommodity.Bucket9] = 27.00m,
                [BucketCommodity.Bucket10] = 54.00m,
                [BucketCommodity.Bucket11] = 20.00m,
                [BucketCommodity.Bucket12] = 20.00m,
                [BucketCommodity.Bucket13] = 17.00m,
                [BucketCommodity.Bucket14] = 14.00m,
                [BucketCommodity.Bucket15] = 10.00m,
                [BucketCommodity.Bucket16] = 54.00m,
                [BucketCommodity.Bucket17] = 16.00m
            };

            private static readonly Dictionary<IThresholdIdentifier,Decimal> THRESHOLDS_DELTA = new Dictionary<IThresholdIdentifier,Decimal>
            {
                [BucketCommodity.Bucket1] = 700.00m,
                [BucketCommodity.Bucket2] = 3600.00m,
                [BucketCommodity.Bucket3] = 2700.00m,
                [BucketCommodity.Bucket4] = 2700.00m,
                [BucketCommodity.Bucket5] = 2700.00m,
                [BucketCommodity.Bucket6] = 2600.00m,
                [BucketCommodity.Bucket7] = 2600.00m,
                [BucketCommodity.Bucket8] = 1900.00m,
                [BucketCommodity.Bucket9] = 1900.00m,
                [BucketCommodity.Bucket10] = 52.00m,
                [BucketCommodity.Bucket11] = 2000.00m,
                [BucketCommodity.Bucket12] = 3200.00m,
                [BucketCommodity.Bucket13] = 1100.00m,
                [BucketCommodity.Bucket14] = 1100.00m,
                [BucketCommodity.Bucket15] = 1100.00m,
                [BucketCommodity.Bucket16] = 52.00m,
                [BucketCommodity.Bucket17] = 5200.00m
            };

            private static readonly Dictionary<IThresholdIdentifier,Decimal> THRESHOLDS_VEGA = new Dictionary<IThresholdIdentifier,Decimal>
            {
                [BucketCommodity.Bucket1] = 250.00m,
                [BucketCommodity.Bucket2] = 1800.00m,
                [BucketCommodity.Bucket3] = 320.00m,
                [BucketCommodity.Bucket4] = 320.00m,
                [BucketCommodity.Bucket5] = 320.00m,
                [BucketCommodity.Bucket6] = 2200.00m,
                [BucketCommodity.Bucket7] = 2200.00m,
                [BucketCommodity.Bucket8] = 780.00m,
                [BucketCommodity.Bucket9] = 780.00m,
                [BucketCommodity.Bucket10] = 99.00m,
                [BucketCommodity.Bucket11] = 420.00m,
                [BucketCommodity.Bucket12] = 650.00m,
                [BucketCommodity.Bucket13] = 570.00m,
                [BucketCommodity.Bucket14] = 570.00m,
                [BucketCommodity.Bucket15] = 570.00m,
                [BucketCommodity.Bucket16] = 99.00m,
                [BucketCommodity.Bucket17] = 330.00m
            };
            #endregion

            #region Methods
            public override Decimal GetCorrelationBucket(IBucket bucket1, IBucket bucket2)
            {
                if (bucket1 == null)
                    throw new ArgumentNullException(nameof(bucket1));

                if (bucket2 == null)
                    throw new ArgumentNullException(nameof(bucket2));

                return CORRELATIONS_BUCKET[bucket1][bucket2];
            }

            public override Decimal GetCorrelationSensitivity(Sensitivity sensitivity1, Sensitivity sensitivity2)
            {
                if (sensitivity1 == null)
                    throw new ArgumentNullException(nameof(sensitivity1));

                if (sensitivity2 == null)
                    throw new ArgumentNullException(nameof(sensitivity2));

                IBucket bucket = (new List<Sensitivity> { sensitivity1, sensitivity2 })
                    .Select(x => x.Bucket)
                    .Distinct().Single();

                return CORRELATIONS_SENSITIVITY[bucket];
            }

            public override Decimal GetRiskWeightDelta(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                return RISKWEIGHTS_DELTA[sensitivity.Bucket];
            }

            public override Decimal GetRiskWeightVega(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                return RISKWEIGHT_VEGA;
            }

            public override Decimal GetThresholdDelta(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                return THRESHOLDS_DELTA[thresholdIdentifier];
            }

            public override Decimal GetThresholdVega(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                return THRESHOLDS_VEGA[thresholdIdentifier];
            }
            #endregion
        }

        private sealed class ParametersProviderCreditNonQualifying : ParametersProvider
        {
            #region Parameters
            private const Decimal CORRELATION_BUCKET = 0.16m;
            private const Decimal CORRELATION_SENSITIVITY_DIFFERENT_AGGREGATE = 0.20m;
            private const Decimal CORRELATION_SENSITIVITY_DIFFERENT_RESIDUAL = 0.50m;
            private const Decimal CORRELATION_SENSITIVITY_SAME_AGGREGATE = 0.57m;
            private const Decimal CORRELATION_SENSITIVITY_SAME_RESIDUAL = 0.50m;
            private const Decimal RISKWEIGHT_VEGA = 0.27m;
            private const Decimal THRESHOLD_VEGA = 54.00m;

            private static readonly Dictionary<IBucket,Decimal> RISKWEIGHTS_DELTA = new Dictionary<IBucket,Decimal>
            {
                [BucketCreditNonQualifying.Bucket1] = 150.00m,
                [BucketCreditNonQualifying.Bucket2] = 1200.00m,
                [BucketCreditNonQualifying.BucketResidual] = 1200.00m
            };

            private static readonly Dictionary<IThresholdIdentifier,Decimal> THRESHOLDS_DELTA = new Dictionary<IThresholdIdentifier,Decimal>
            {
                [BucketCreditNonQualifying.Bucket1] = 9.50m,
                [BucketCreditNonQualifying.Bucket2] = 0.50m,
                [BucketCreditNonQualifying.BucketResidual] = 0.50m
            };
            #endregion

            #region Methods
            public override Decimal GetCorrelationBucket(IBucket bucket1, IBucket bucket2)
            {
                if (bucket1 == null)
                    throw new ArgumentNullException(nameof(bucket1));

                if (bucket2 == null)
                    throw new ArgumentNullException(nameof(bucket2));

                return CORRELATION_BUCKET;
            }

            public override Decimal GetCorrelationSensitivity(Sensitivity sensitivity1, Sensitivity sensitivity2)
            {
                if (sensitivity1 == null)
                    throw new ArgumentNullException(nameof(sensitivity1));

                if (sensitivity2 == null)
                    throw new ArgumentNullException(nameof(sensitivity2));

                IBucket bucket = (new List<Sensitivity> { sensitivity1, sensitivity2 })
                    .Select(x => x.Bucket)
                    .Distinct().Single();

                Decimal correlation;

                if (String.Equals(sensitivity1.Qualifier, sensitivity2.Qualifier, StringComparison.Ordinal))
                {
                    if (bucket.IsResidual)
                        correlation = CORRELATION_SENSITIVITY_SAME_RESIDUAL;
                    else
                        correlation = CORRELATION_SENSITIVITY_SAME_AGGREGATE;
                }
                else
                {
                    if (bucket.IsResidual)
                        correlation = CORRELATION_SENSITIVITY_DIFFERENT_RESIDUAL;
                    else
                        correlation = CORRELATION_SENSITIVITY_DIFFERENT_AGGREGATE;
                }

                return correlation;
            }

            public override Decimal GetRiskWeightDelta(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                return RISKWEIGHTS_DELTA[sensitivity.Bucket];
            }

            public override Decimal GetRiskWeightVega(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                return RISKWEIGHT_VEGA;
            }

            public override Decimal GetThresholdDelta(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                return THRESHOLDS_DELTA[thresholdIdentifier];
            }

            public override Decimal GetThresholdVega(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                return THRESHOLD_VEGA;
            }
            #endregion
        }

        private sealed class ParametersProviderCreditQualifying : ParametersProvider
        {
            #region Parameters
            private const Decimal CORRELATION_SENSITIVITY_DIFFERENT_AGGREGATE = 0.39m;
            private const Decimal CORRELATION_SENSITIVITY_DIFFERENT_RESIDUAL = 0.50m;
            private const Decimal CORRELATION_SENSITIVITY_SAME_AGGREGATE = 0.96m;
            private const Decimal CORRELATION_SENSITIVITY_SAME_RESIDUAL = 0.50m;
            private const Decimal RISKWEIGHT_VEGA = 0.27m;
            private const Decimal THRESHOLD_VEGA = 250.00m;

            private static readonly Dictionary<IBucket,Dictionary<IBucket,Decimal>> CORRELATIONS_BUCKET = new Dictionary<IBucket,Dictionary<IBucket,Decimal>>
            {
                [BucketCreditQualifying.Bucket1] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 1.00m,
                    [BucketCreditQualifying.Bucket2] = 0.38m,
                    [BucketCreditQualifying.Bucket3] = 0.36m,
                    [BucketCreditQualifying.Bucket4] = 0.36m,
                    [BucketCreditQualifying.Bucket5] = 0.39m,
                    [BucketCreditQualifying.Bucket6] = 0.35m,
                    [BucketCreditQualifying.Bucket7] = 0.34m,
                    [BucketCreditQualifying.Bucket8] = 0.32m,
                    [BucketCreditQualifying.Bucket9] = 0.34m,
                    [BucketCreditQualifying.Bucket10] = 0.33m,
                    [BucketCreditQualifying.Bucket11] = 0.34m,
                    [BucketCreditQualifying.Bucket12] = 0.31m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.Bucket2] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.38m,
                    [BucketCreditQualifying.Bucket2] = 1.00m,
                    [BucketCreditQualifying.Bucket3] = 0.41m,
                    [BucketCreditQualifying.Bucket4] = 0.41m,
                    [BucketCreditQualifying.Bucket5] = 0.43m,
                    [BucketCreditQualifying.Bucket6] = 0.40m,
                    [BucketCreditQualifying.Bucket7] = 0.29m,
                    [BucketCreditQualifying.Bucket8] = 0.38m,
                    [BucketCreditQualifying.Bucket9] = 0.38m,
                    [BucketCreditQualifying.Bucket10] = 0.38m,
                    [BucketCreditQualifying.Bucket11] = 0.38m,
                    [BucketCreditQualifying.Bucket12] = 0.34m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.Bucket3] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.36m,
                    [BucketCreditQualifying.Bucket2] = 0.41m,
                    [BucketCreditQualifying.Bucket3] = 1.00m,
                    [BucketCreditQualifying.Bucket4] = 0.41m,
                    [BucketCreditQualifying.Bucket5] = 0.42m,
                    [BucketCreditQualifying.Bucket6] = 0.39m,
                    [BucketCreditQualifying.Bucket7] = 0.30m,
                    [BucketCreditQualifying.Bucket8] = 0.34m,
                    [BucketCreditQualifying.Bucket9] = 0.39m,
                    [BucketCreditQualifying.Bucket10] = 0.37m,
                    [BucketCreditQualifying.Bucket11] = 0.38m,
                    [BucketCreditQualifying.Bucket12] = 0.35m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.Bucket4] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.36m,
                    [BucketCreditQualifying.Bucket2] = 0.41m,
                    [BucketCreditQualifying.Bucket3] = 0.41m,
                    [BucketCreditQualifying.Bucket4] = 1.00m,
                    [BucketCreditQualifying.Bucket5] = 0.43m,
                    [BucketCreditQualifying.Bucket6] = 0.40m,
                    [BucketCreditQualifying.Bucket7] = 0.28m,
                    [BucketCreditQualifying.Bucket8] = 0.33m,
                    [BucketCreditQualifying.Bucket9] = 0.37m,
                    [BucketCreditQualifying.Bucket10] = 0.38m,
                    [BucketCreditQualifying.Bucket11] = 0.38m,
                    [BucketCreditQualifying.Bucket12] = 0.34m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.Bucket5] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.39m,
                    [BucketCreditQualifying.Bucket2] = 0.43m,
                    [BucketCreditQualifying.Bucket3] = 0.42m,
                    [BucketCreditQualifying.Bucket4] = 0.43m,
                    [BucketCreditQualifying.Bucket5] = 1.00m,
                    [BucketCreditQualifying.Bucket6] = 0.42m,
                    [BucketCreditQualifying.Bucket7] = 0.31m,
                    [BucketCreditQualifying.Bucket8] = 0.35m,
                    [BucketCreditQualifying.Bucket9] = 0.38m,
                    [BucketCreditQualifying.Bucket10] = 0.39m,
                    [BucketCreditQualifying.Bucket11] = 0.41m,
                    [BucketCreditQualifying.Bucket12] = 0.36m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.Bucket6] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.35m,
                    [BucketCreditQualifying.Bucket2] = 0.40m,
                    [BucketCreditQualifying.Bucket3] = 0.39m,
                    [BucketCreditQualifying.Bucket4] = 0.40m,
                    [BucketCreditQualifying.Bucket5] = 0.42m,
                    [BucketCreditQualifying.Bucket6] = 1.00m,
                    [BucketCreditQualifying.Bucket7] = 0.27m,
                    [BucketCreditQualifying.Bucket8] = 0.32m,
                    [BucketCreditQualifying.Bucket9] = 0.34m,
                    [BucketCreditQualifying.Bucket10] = 0.35m,
                    [BucketCreditQualifying.Bucket11] = 0.36m,
                    [BucketCreditQualifying.Bucket12] = 0.33m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.Bucket7] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.34m,
                    [BucketCreditQualifying.Bucket2] = 0.29m,
                    [BucketCreditQualifying.Bucket3] = 0.30m,
                    [BucketCreditQualifying.Bucket4] = 0.28m,
                    [BucketCreditQualifying.Bucket5] = 0.31m,
                    [BucketCreditQualifying.Bucket6] = 0.27m,
                    [BucketCreditQualifying.Bucket7] = 1.00m,
                    [BucketCreditQualifying.Bucket8] = 0.24m,
                    [BucketCreditQualifying.Bucket9] = 0.28m,
                    [BucketCreditQualifying.Bucket10] = 0.27m,
                    [BucketCreditQualifying.Bucket11] = 0.27m,
                    [BucketCreditQualifying.Bucket12] = 0.26m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.Bucket8] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.32m,
                    [BucketCreditQualifying.Bucket2] = 0.38m,
                    [BucketCreditQualifying.Bucket3] = 0.34m,
                    [BucketCreditQualifying.Bucket4] = 0.33m,
                    [BucketCreditQualifying.Bucket5] = 0.35m,
                    [BucketCreditQualifying.Bucket6] = 0.32m,
                    [BucketCreditQualifying.Bucket7] = 0.24m,
                    [BucketCreditQualifying.Bucket8] = 1.00m,
                    [BucketCreditQualifying.Bucket9] = 0.33m,
                    [BucketCreditQualifying.Bucket10] = 0.32m,
                    [BucketCreditQualifying.Bucket11] = 0.32m,
                    [BucketCreditQualifying.Bucket12] = 0.29m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.Bucket9] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.34m,
                    [BucketCreditQualifying.Bucket2] = 0.38m,
                    [BucketCreditQualifying.Bucket3] = 0.39m,
                    [BucketCreditQualifying.Bucket4] = 0.37m,
                    [BucketCreditQualifying.Bucket5] = 0.38m,
                    [BucketCreditQualifying.Bucket6] = 0.34m,
                    [BucketCreditQualifying.Bucket7] = 0.28m,
                    [BucketCreditQualifying.Bucket8] = 0.33m,
                    [BucketCreditQualifying.Bucket9] = 1.00m,
                    [BucketCreditQualifying.Bucket10] = 0.35m,
                    [BucketCreditQualifying.Bucket11] = 0.35m,
                    [BucketCreditQualifying.Bucket12] = 0.33m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.Bucket10] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.33m,
                    [BucketCreditQualifying.Bucket2] = 0.38m,
                    [BucketCreditQualifying.Bucket3] = 0.37m,
                    [BucketCreditQualifying.Bucket4] = 0.38m,
                    [BucketCreditQualifying.Bucket5] = 0.39m,
                    [BucketCreditQualifying.Bucket6] = 0.35m,
                    [BucketCreditQualifying.Bucket7] = 0.27m,
                    [BucketCreditQualifying.Bucket8] = 0.32m,
                    [BucketCreditQualifying.Bucket9] = 0.35m,
                    [BucketCreditQualifying.Bucket10] = 1.00m,
                    [BucketCreditQualifying.Bucket11] = 0.36m,
                    [BucketCreditQualifying.Bucket12] = 0.32m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.Bucket11] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.34m,
                    [BucketCreditQualifying.Bucket2] = 0.38m,
                    [BucketCreditQualifying.Bucket3] = 0.38m,
                    [BucketCreditQualifying.Bucket4] = 0.38m,
                    [BucketCreditQualifying.Bucket5] = 0.41m,
                    [BucketCreditQualifying.Bucket6] = 0.36m,
                    [BucketCreditQualifying.Bucket7] = 0.27m,
                    [BucketCreditQualifying.Bucket8] = 0.32m,
                    [BucketCreditQualifying.Bucket9] = 0.35m,
                    [BucketCreditQualifying.Bucket10] = 0.36m,
                    [BucketCreditQualifying.Bucket11] = 1.00m,
                    [BucketCreditQualifying.Bucket12] = 0.33m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.Bucket12] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.31m,
                    [BucketCreditQualifying.Bucket2] = 0.34m,
                    [BucketCreditQualifying.Bucket3] = 0.35m,
                    [BucketCreditQualifying.Bucket4] = 0.34m,
                    [BucketCreditQualifying.Bucket5] = 0.36m,
                    [BucketCreditQualifying.Bucket6] = 0.33m,
                    [BucketCreditQualifying.Bucket7] = 0.26m,
                    [BucketCreditQualifying.Bucket8] = 0.29m,
                    [BucketCreditQualifying.Bucket9] = 0.33m,
                    [BucketCreditQualifying.Bucket10] = 0.32m,
                    [BucketCreditQualifying.Bucket11] = 0.33m,
                    [BucketCreditQualifying.Bucket12] = 1.00m,
                    [BucketCreditQualifying.BucketResidual] = 0.00m
                },
                [BucketCreditQualifying.BucketResidual] = new Dictionary<IBucket,Decimal>
                {
                    [BucketCreditQualifying.Bucket1] = 0.00m,
                    [BucketCreditQualifying.Bucket2] = 0.00m,
                    [BucketCreditQualifying.Bucket3] = 0.00m,
                    [BucketCreditQualifying.Bucket4] = 0.00m,
                    [BucketCreditQualifying.Bucket5] = 0.00m,
                    [BucketCreditQualifying.Bucket6] = 0.00m,
                    [BucketCreditQualifying.Bucket7] = 0.00m,
                    [BucketCreditQualifying.Bucket8] = 0.00m,
                    [BucketCreditQualifying.Bucket9] = 0.00m,
                    [BucketCreditQualifying.Bucket10] = 0.00m,
                    [BucketCreditQualifying.Bucket11] = 0.00m,
                    [BucketCreditQualifying.Bucket12] = 0.00m,
                    [BucketCreditQualifying.BucketResidual] = 1.00m
                }
            };

            private static readonly Dictionary<IBucket,Decimal> RISKWEIGHTS_DELTA = new Dictionary<IBucket,Decimal>
            {
                [BucketCreditQualifying.Bucket1] = 69.00m,
                [BucketCreditQualifying.Bucket2] = 107.00m,
                [BucketCreditQualifying.Bucket3] = 72.00m,
                [BucketCreditQualifying.Bucket4] = 55.00m,
                [BucketCreditQualifying.Bucket5] = 48.00m,
                [BucketCreditQualifying.Bucket6] = 41.00m,
                [BucketCreditQualifying.Bucket7] = 166.00m,
                [BucketCreditQualifying.Bucket8] = 187.00m,
                [BucketCreditQualifying.Bucket9] = 177.00m,
                [BucketCreditQualifying.Bucket10] = 187.00m,
                [BucketCreditQualifying.Bucket11] = 129.00m,
                [BucketCreditQualifying.Bucket12] = 136.00m,
                [BucketCreditQualifying.BucketResidual] = 187.00m
            };

            private static readonly Dictionary<IThresholdIdentifier,Decimal> THRESHOLDS_DELTA = new Dictionary<IThresholdIdentifier,Decimal>
            {
                [BucketCreditQualifying.Bucket1] = 1.00m,
                [BucketCreditQualifying.Bucket2] = 0.24m,
                [BucketCreditQualifying.Bucket3] = 0.24m,
                [BucketCreditQualifying.Bucket4] = 0.24m,
                [BucketCreditQualifying.Bucket5] = 0.24m,
                [BucketCreditQualifying.Bucket6] = 0.24m,
                [BucketCreditQualifying.Bucket7] = 1.00m,
                [BucketCreditQualifying.Bucket8] = 0.24m,
                [BucketCreditQualifying.Bucket9] = 0.24m,
                [BucketCreditQualifying.Bucket10] = 0.24m,
                [BucketCreditQualifying.Bucket11] = 0.24m,
                [BucketCreditQualifying.Bucket12] = 0.24m,
                [BucketCreditQualifying.BucketResidual] = 0.24m
            };
            #endregion

            #region Methods
            public override Decimal GetCorrelationBucket(IBucket bucket1, IBucket bucket2)
            {
                if (bucket1 == null)
                    throw new ArgumentNullException(nameof(bucket1));

                if (bucket2 == null)
                    throw new ArgumentNullException(nameof(bucket2));

                return CORRELATIONS_BUCKET[bucket1][bucket2];
            }

            public override Decimal GetCorrelationSensitivity(Sensitivity sensitivity1, Sensitivity sensitivity2)
            {
                if (sensitivity1 == null)
                    throw new ArgumentNullException(nameof(sensitivity1));

                if (sensitivity2 == null)
                    throw new ArgumentNullException(nameof(sensitivity2));

                IBucket bucket = (new List<Sensitivity> { sensitivity1, sensitivity2 })
                    .Select(x => x.Bucket)
                    .Distinct().Single();

                Decimal correlation;

                if (String.Equals(sensitivity1.Qualifier, sensitivity2.Qualifier, StringComparison.Ordinal))
                {
                    if (bucket.IsResidual)
                        correlation = CORRELATION_SENSITIVITY_SAME_RESIDUAL;
                    else
                        correlation = CORRELATION_SENSITIVITY_SAME_AGGREGATE;
                }
                else
                {
                    if (bucket.IsResidual)
                        correlation = CORRELATION_SENSITIVITY_DIFFERENT_RESIDUAL;
                    else
                        correlation = CORRELATION_SENSITIVITY_DIFFERENT_AGGREGATE;
                }

                return correlation;
            }

            public override Decimal GetRiskWeightDelta(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                return RISKWEIGHTS_DELTA[sensitivity.Bucket];
            }

            public override Decimal GetRiskWeightVega(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                return RISKWEIGHT_VEGA;
            }

            public override Decimal GetThresholdDelta(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                return THRESHOLDS_DELTA[thresholdIdentifier];
            }

            public override Decimal GetThresholdVega(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                return THRESHOLD_VEGA;
            }
            #endregion
        }

        private sealed class ParametersProviderEquity : ParametersProvider
        {
            #region Parameters

            private static readonly Dictionary<IBucket, Dictionary<IBucket, Decimal>> CORRELATIONS_BUCKET = new Dictionary<IBucket, Dictionary<IBucket, Decimal>>
            {
                [BucketEquity.Bucket1] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 1.00m,
                    [BucketEquity.Bucket2] = 0.16m,
                    [BucketEquity.Bucket3] = 0.16m,
                    [BucketEquity.Bucket4] = 0.17m,
                    [BucketEquity.Bucket5] = 0.13m,
                    [BucketEquity.Bucket6] = 0.15m,
                    [BucketEquity.Bucket7] = 0.15m,
                    [BucketEquity.Bucket8] = 0.15m,
                    [BucketEquity.Bucket9] = 0.13m,
                    [BucketEquity.Bucket10] = 0.11m,
                    [BucketEquity.Bucket11] = 0.19m,
                    [BucketEquity.Bucket12] = 0.19m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.Bucket2] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.16m,
                    [BucketEquity.Bucket2] = 1.00m,
                    [BucketEquity.Bucket3] = 0.20m,
                    [BucketEquity.Bucket4] = 0.20m,
                    [BucketEquity.Bucket5] = 0.14m,
                    [BucketEquity.Bucket6] = 0.16m,
                    [BucketEquity.Bucket7] = 0.16m,
                    [BucketEquity.Bucket8] = 0.16m,
                    [BucketEquity.Bucket9] = 0.15m,
                    [BucketEquity.Bucket10] = 0.13m,
                    [BucketEquity.Bucket11] = 0.20m,
                    [BucketEquity.Bucket12] = 0.20m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.Bucket3] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.16m,
                    [BucketEquity.Bucket2] = 0.20m,
                    [BucketEquity.Bucket3] = 1.00m,
                    [BucketEquity.Bucket4] = 0.22m,
                    [BucketEquity.Bucket5] = 0.15m,
                    [BucketEquity.Bucket6] = 0.19m,
                    [BucketEquity.Bucket7] = 0.22m,
                    [BucketEquity.Bucket8] = 0.19m,
                    [BucketEquity.Bucket9] = 0.16m,
                    [BucketEquity.Bucket10] = 0.15m,
                    [BucketEquity.Bucket11] = 0.25m,
                    [BucketEquity.Bucket12] = 0.25m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.Bucket4] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.17m,
                    [BucketEquity.Bucket2] = 0.20m,
                    [BucketEquity.Bucket3] = 0.22m,
                    [BucketEquity.Bucket4] = 1.00m,
                    [BucketEquity.Bucket5] = 0.17m,
                    [BucketEquity.Bucket6] = 0.21m,
                    [BucketEquity.Bucket7] = 0.21m,
                    [BucketEquity.Bucket8] = 0.21m,
                    [BucketEquity.Bucket9] = 0.17m,
                    [BucketEquity.Bucket10] = 0.15m,
                    [BucketEquity.Bucket11] = 0.27m,
                    [BucketEquity.Bucket12] = 0.27m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.Bucket5] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.13m,
                    [BucketEquity.Bucket2] = 0.14m,
                    [BucketEquity.Bucket3] = 0.15m,
                    [BucketEquity.Bucket4] = 0.17m,
                    [BucketEquity.Bucket5] = 1.00m,
                    [BucketEquity.Bucket6] = 0.25m,
                    [BucketEquity.Bucket7] = 0.23m,
                    [BucketEquity.Bucket8] = 0.26m,
                    [BucketEquity.Bucket9] = 0.14m,
                    [BucketEquity.Bucket10] = 0.17m,
                    [BucketEquity.Bucket11] = 0.32m,
                    [BucketEquity.Bucket12] = 0.32m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.Bucket6] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.15m,
                    [BucketEquity.Bucket2] = 0.16m,
                    [BucketEquity.Bucket3] = 0.19m,
                    [BucketEquity.Bucket4] = 0.21m,
                    [BucketEquity.Bucket5] = 0.25m,
                    [BucketEquity.Bucket6] = 1.00m,
                    [BucketEquity.Bucket7] = 0.30m,
                    [BucketEquity.Bucket8] = 0.31m,
                    [BucketEquity.Bucket9] = 0.16m,
                    [BucketEquity.Bucket10] = 0.21m,
                    [BucketEquity.Bucket11] = 0.38m,
                    [BucketEquity.Bucket12] = 0.38m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.Bucket7] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.15m,
                    [BucketEquity.Bucket2] = 0.16m,
                    [BucketEquity.Bucket3] = 0.22m,
                    [BucketEquity.Bucket4] = 0.21m,
                    [BucketEquity.Bucket5] = 0.23m,
                    [BucketEquity.Bucket6] = 0.30m,
                    [BucketEquity.Bucket7] = 1.00m,
                    [BucketEquity.Bucket8] = 0.29m,
                    [BucketEquity.Bucket9] = 0.16m,
                    [BucketEquity.Bucket10] = 0.21m,
                    [BucketEquity.Bucket11] = 0.38m,
                    [BucketEquity.Bucket12] = 0.38m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.Bucket8] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.15m,
                    [BucketEquity.Bucket2] = 0.16m,
                    [BucketEquity.Bucket3] = 0.19m,
                    [BucketEquity.Bucket4] = 0.21m,
                    [BucketEquity.Bucket5] = 0.26m,
                    [BucketEquity.Bucket6] = 0.31m,
                    [BucketEquity.Bucket7] = 0.29m,
                    [BucketEquity.Bucket8] = 1.00m,
                    [BucketEquity.Bucket9] = 0.17m,
                    [BucketEquity.Bucket10] = 0.21m,
                    [BucketEquity.Bucket11] = 0.39m,
                    [BucketEquity.Bucket12] = 0.39m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.Bucket9] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.13m,
                    [BucketEquity.Bucket2] = 0.15m,
                    [BucketEquity.Bucket3] = 0.16m,
                    [BucketEquity.Bucket4] = 0.17m,
                    [BucketEquity.Bucket5] = 0.14m,
                    [BucketEquity.Bucket6] = 0.16m,
                    [BucketEquity.Bucket7] = 0.16m,
                    [BucketEquity.Bucket8] = 0.17m,
                    [BucketEquity.Bucket9] = 1.00m,
                    [BucketEquity.Bucket10] = 0.13m,
                    [BucketEquity.Bucket11] = 0.21m,
                    [BucketEquity.Bucket12] = 0.21m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.Bucket10] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.11m,
                    [BucketEquity.Bucket2] = 0.13m,
                    [BucketEquity.Bucket3] = 0.15m,
                    [BucketEquity.Bucket4] = 0.15m,
                    [BucketEquity.Bucket5] = 0.17m,
                    [BucketEquity.Bucket6] = 0.21m,
                    [BucketEquity.Bucket7] = 0.21m,
                    [BucketEquity.Bucket8] = 0.21m,
                    [BucketEquity.Bucket9] = 0.13m,
                    [BucketEquity.Bucket10] = 1.00m,
                    [BucketEquity.Bucket11] = 0.25m,
                    [BucketEquity.Bucket12] = 0.25m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.Bucket11] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.19m,
                    [BucketEquity.Bucket2] = 0.20m,
                    [BucketEquity.Bucket3] = 0.25m,
                    [BucketEquity.Bucket4] = 0.27m,
                    [BucketEquity.Bucket5] = 0.32m,
                    [BucketEquity.Bucket6] = 0.38m,
                    [BucketEquity.Bucket7] = 0.38m,
                    [BucketEquity.Bucket8] = 0.39m,
                    [BucketEquity.Bucket9] = 0.21m,
                    [BucketEquity.Bucket10] = 0.25m,
                    [BucketEquity.Bucket11] = 1.00m,
                    [BucketEquity.Bucket12] = 0.51m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.Bucket12] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.19m,
                    [BucketEquity.Bucket2] = 0.20m,
                    [BucketEquity.Bucket3] = 0.25m,
                    [BucketEquity.Bucket4] = 0.27m,
                    [BucketEquity.Bucket5] = 0.32m,
                    [BucketEquity.Bucket6] = 0.38m,
                    [BucketEquity.Bucket7] = 0.38m,
                    [BucketEquity.Bucket8] = 0.39m,
                    [BucketEquity.Bucket9] = 0.21m,
                    [BucketEquity.Bucket10] = 0.25m,
                    [BucketEquity.Bucket11] = 0.51m,
                    [BucketEquity.Bucket12] = 1.00m,
                    [BucketEquity.BucketResidual] = 0.00m
                },
                [BucketEquity.BucketResidual] = new Dictionary<IBucket, Decimal>
                {
                    [BucketEquity.Bucket1] = 0.00m,
                    [BucketEquity.Bucket2] = 0.00m,
                    [BucketEquity.Bucket3] = 0.00m,
                    [BucketEquity.Bucket4] = 0.00m,
                    [BucketEquity.Bucket5] = 0.00m,
                    [BucketEquity.Bucket6] = 0.00m,
                    [BucketEquity.Bucket7] = 0.00m,
                    [BucketEquity.Bucket8] = 0.00m,
                    [BucketEquity.Bucket9] = 0.00m,
                    [BucketEquity.Bucket10] = 0.00m,
                    [BucketEquity.Bucket11] = 0.00m,
                    [BucketEquity.Bucket12] = 0.00m,
                    [BucketEquity.BucketResidual] = 1.00m
                }
            };

            private static readonly Dictionary<IBucket, Decimal> CORRELATIONS_SENSITIVITY = new Dictionary<IBucket, Decimal>
            {
                [BucketEquity.Bucket1] = 0.14m,
                [BucketEquity.Bucket2] = 0.20m,
                [BucketEquity.Bucket3] = 0.25m,
                [BucketEquity.Bucket4] = 0.23m,
                [BucketEquity.Bucket5] = 0.23m,
                [BucketEquity.Bucket6] = 0.32m,
                [BucketEquity.Bucket7] = 0.35m,
                [BucketEquity.Bucket8] = 0.32m,
                [BucketEquity.Bucket9] = 0.17m,
                [BucketEquity.Bucket10] = 0.16m,
                [BucketEquity.Bucket11] = 0.51m,
                [BucketEquity.Bucket12] = 0.51m,
                [BucketEquity.BucketResidual] = 0.00m
            };

            private static readonly Dictionary<IBucket, Decimal> RISKWEIGHTS_DELTA = new Dictionary<IBucket, Decimal>
            {
                [BucketEquity.Bucket1] = 24.00m,
                [BucketEquity.Bucket2] = 30.00m,
                [BucketEquity.Bucket3] = 31.00m,
                [BucketEquity.Bucket4] = 25.00m,
                [BucketEquity.Bucket5] = 21.00m,
                [BucketEquity.Bucket6] = 22.00m,
                [BucketEquity.Bucket7] = 27.00m,
                [BucketEquity.Bucket8] = 24.00m,
                [BucketEquity.Bucket9] = 33.00m,
                [BucketEquity.Bucket10] = 34.00m,
                [BucketEquity.Bucket11] = 17.00m,
                [BucketEquity.Bucket12] = 17.00m,
                [BucketEquity.BucketResidual] = 34.00m
            };

            private static readonly Dictionary<IBucket, Decimal> RISKWEIGHTS_VEGA = new Dictionary<IBucket, Decimal>
            {
                [BucketEquity.Bucket1] = 0.28m,
                [BucketEquity.Bucket2] = 0.28m,
                [BucketEquity.Bucket3] = 0.28m,
                [BucketEquity.Bucket4] = 0.28m,
                [BucketEquity.Bucket5] = 0.28m,
                [BucketEquity.Bucket6] = 0.28m,
                [BucketEquity.Bucket7] = 0.28m,
                [BucketEquity.Bucket8] = 0.28m,
                [BucketEquity.Bucket9] = 0.28m,
                [BucketEquity.Bucket10] = 0.28m,
                [BucketEquity.Bucket11] = 0.28m,
                [BucketEquity.Bucket12] = 0.63m,
                [BucketEquity.BucketResidual] = 0.28m
            };

            private static readonly Dictionary<IThresholdIdentifier, Decimal> THRESHOLDS_DELTA = new Dictionary<IThresholdIdentifier, Decimal>
            {
                [BucketEquity.Bucket1] = 8.40m,
                [BucketEquity.Bucket2] = 8.40m,
                [BucketEquity.Bucket3] = 8.40m,
                [BucketEquity.Bucket4] = 8.40m,
                [BucketEquity.Bucket5] = 26.00m,
                [BucketEquity.Bucket6] = 26.00m,
                [BucketEquity.Bucket7] = 26.00m,
                [BucketEquity.Bucket8] = 26.00m,
                [BucketEquity.Bucket9] = 1.80m,
                [BucketEquity.Bucket10] = 1.90m,
                [BucketEquity.Bucket11] = 540.00m,
                [BucketEquity.Bucket12] = 540.00m,
                [BucketEquity.BucketResidual] = 1.80m
            };

            private static readonly Dictionary<IThresholdIdentifier, Decimal> THRESHOLDS_VEGA = new Dictionary<IThresholdIdentifier, Decimal>
            {
                [BucketEquity.Bucket1] = 220.00m,
                [BucketEquity.Bucket2] = 220.00m,
                [BucketEquity.Bucket3] = 220.00m,
                [BucketEquity.Bucket4] = 220.00m,
                [BucketEquity.Bucket5] = 2300.00m,
                [BucketEquity.Bucket6] = 2300.00m,
                [BucketEquity.Bucket7] = 2300.00m,
                [BucketEquity.Bucket8] = 2300.00m,
                [BucketEquity.Bucket9] = 43.00m,
                [BucketEquity.Bucket10] = 250.00m,
                [BucketEquity.Bucket11] = 8100.00m,
                [BucketEquity.Bucket12] = 8100.00m,
                [BucketEquity.BucketResidual] = 43.00m
            };
            #endregion

            #region Methods
            public override Decimal GetCorrelationBucket(IBucket bucket1, IBucket bucket2)
            {
                if (bucket1 == null)
                    throw new ArgumentNullException(nameof(bucket1));

                if (bucket2 == null)
                    throw new ArgumentNullException(nameof(bucket2));

                return CORRELATIONS_BUCKET[bucket1][bucket2];
            }

            public override Decimal GetCorrelationSensitivity(Sensitivity sensitivity1, Sensitivity sensitivity2)
            {
                if (sensitivity1 == null)
                    throw new ArgumentNullException(nameof(sensitivity1));

                if (sensitivity2 == null)
                    throw new ArgumentNullException(nameof(sensitivity2));

                IBucket bucket = (new List<Sensitivity> { sensitivity1, sensitivity2 })
                    .Select(x => x.Bucket)
                    .Distinct().Single();

                return CORRELATIONS_SENSITIVITY[bucket];
            }

            public override Decimal GetRiskWeightDelta(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                return RISKWEIGHTS_DELTA[sensitivity.Bucket];
            }

            public override Decimal GetRiskWeightVega(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                return RISKWEIGHTS_VEGA[sensitivity.Bucket];
            }

            public override Decimal GetThresholdDelta(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                return THRESHOLDS_DELTA[thresholdIdentifier];
            }

            public override Decimal GetThresholdVega(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                return THRESHOLDS_VEGA[thresholdIdentifier];
            }
            #endregion
        }

        private sealed class ParametersProviderFx : ParametersProvider
        {
            #region Parameters
            private const Decimal CORRELATION_BUCKET = 1.00m;
            private const Decimal CORRELATION_SENSITIVITY = 0.50m;
            private const Decimal RISKWEIGHT_DELTA = 8.10m;
            private const Decimal RISKWEIGHT_VEGA = 0.30m;

            private static readonly Dictionary<CurrencyCategory, Decimal> THRESHOLDS_DELTA = new Dictionary<CurrencyCategory, Decimal>
            {
                [CurrencyCategory.FrequentlyTraded] = 2900.00m,
                [CurrencyCategory.SignificantlyMaterial] = 9700.00m,
                [CurrencyCategory.Other] = 450.00m
            };

            private static readonly Dictionary<CurrencyCategory, Dictionary<CurrencyCategory, Decimal>> THRESHOLDS_VEGA = new Dictionary<CurrencyCategory, Dictionary<CurrencyCategory, Decimal>>
            {
                [CurrencyCategory.FrequentlyTraded] = new Dictionary<CurrencyCategory, Decimal>
                {
                    [CurrencyCategory.FrequentlyTraded] = 410.00m,
                    [CurrencyCategory.SignificantlyMaterial] = 1000.00m,
                    [CurrencyCategory.Other] = 210.00m
                },
                [CurrencyCategory.SignificantlyMaterial] = new Dictionary<CurrencyCategory, Decimal>
                {
                    [CurrencyCategory.FrequentlyTraded] = 1000.00m,
                    [CurrencyCategory.SignificantlyMaterial] = 2000.00m,
                    [CurrencyCategory.Other] = 320.00m
                },
                [CurrencyCategory.Other] = new Dictionary<CurrencyCategory, Decimal>
                {
                    [CurrencyCategory.FrequentlyTraded] = 210.00m,
                    [CurrencyCategory.SignificantlyMaterial] = 320.00m,
                    [CurrencyCategory.Other] = 150.00m
                }
            };
            #endregion

            #region Methods
            public override Decimal GetCorrelationBucket(IBucket bucket1, IBucket bucket2)
            {
                if (bucket1 == null)
                    throw new ArgumentNullException(nameof(bucket1));

                if (bucket2 == null)
                    throw new ArgumentNullException(nameof(bucket2));

                return CORRELATION_BUCKET;
            }

            public override Decimal GetCorrelationSensitivity(Sensitivity sensitivity1, Sensitivity sensitivity2)
            {
                if (sensitivity1 == null)
                    throw new ArgumentNullException(nameof(sensitivity1));

                if (sensitivity2 == null)
                    throw new ArgumentNullException(nameof(sensitivity2));

                return CORRELATION_SENSITIVITY;
            }

            public override Decimal GetRiskWeightDelta(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                return RISKWEIGHT_DELTA;
            }

            public override Decimal GetRiskWeightVega(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                return RISKWEIGHT_VEGA;
            }

            public override Decimal GetThresholdDelta(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                return THRESHOLDS_DELTA[((Currency)thresholdIdentifier).Category];
            }

            public override Decimal GetThresholdVega(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                CurrencyPair pair = (CurrencyPair)thresholdIdentifier;
                Currency currency1 = pair.Currency1;
                Currency currency2 = pair.Currency2;

                return THRESHOLDS_VEGA[currency1.Category][currency2.Category];
            }
            #endregion
        }

        private sealed class ParametersProviderRates : ParametersProvider
        {
            #region Parameters
            private const Decimal CORRELATION_BUCKET = 0.21m;
            private const Decimal CORRELATION_SENSITIVITY_CROSSCURRENCYBASIS = 0.19m;
            private const Decimal CORRELATION_SENSITIVITY_CURVES = 0.98m;
            private const Decimal CORRELATION_SENSITIVITY_INFLATION = 0.33m;
            private const Decimal RISKWEIGHT_DELTA_CROSSCURRENCYBASIS = 21.00m;
            private const Decimal RISKWEIGHT_DELTA_INFLATION = 48.00m;
            private const Decimal RISKWEIGHT_VEGA = 0.16m;
            private const Decimal THRESHOLD_DELTA_HIGH = 12.00m;
            private const Decimal THRESHOLD_DELTA_LOW = 170.00m;
            private const Decimal THRESHOLD_DELTA_REGULAR_HIGH = 210.00m;
            private const Decimal THRESHOLD_DELTA_REGULAR_MEDIUM = 27.00m;
            private const Decimal THRESHOLD_VEGA_HIGH = 120.00m;
            private const Decimal THRESHOLD_VEGA_LOW = 770.00m;
            private const Decimal THRESHOLD_VEGA_REGULAR_HIGH = 2200.00m;
            private const Decimal THRESHOLD_VEGA_REGULAR_MEDIUM = 190.00m;

            private static readonly Dictionary<CurrencyVolatility, Dictionary<Tenor, Decimal>> RISKWEIGHTS_DELTA_INTERESTRATE = new Dictionary<CurrencyVolatility, Dictionary<Tenor, Decimal>>
            {
                [CurrencyVolatility.Low] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 33.00m,
                    [Tenor.M1] = 20.00m,
                    [Tenor.M3] = 10.00m,
                    [Tenor.M6] = 11.00m,
                    [Tenor.Y1] = 14.00m,
                    [Tenor.Y2] = 20.00m,
                    [Tenor.Y3] = 22.00m,
                    [Tenor.Y5] = 20.00m,
                    [Tenor.Y10] = 20.00m,
                    [Tenor.Y15] = 21.00m,
                    [Tenor.Y20] = 23.00m,
                    [Tenor.Y30] = 27.00m
                },
                [CurrencyVolatility.Regular] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 114.00m,
                    [Tenor.M1] = 115.00m,
                    [Tenor.M3] = 102.00m,
                    [Tenor.M6] = 71.00m,
                    [Tenor.Y1] = 61.00m,
                    [Tenor.Y2] = 52.00m,
                    [Tenor.Y3] = 50.00m,
                    [Tenor.Y5] = 51.00m,
                    [Tenor.Y10] = 51.00m,
                    [Tenor.Y15] = 51.00m,
                    [Tenor.Y20] = 54.00m,
                    [Tenor.Y30] = 62.00m
                },
                [CurrencyVolatility.High] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 91.00m,
                    [Tenor.M1] = 91.00m,
                    [Tenor.M3] = 95.00m,
                    [Tenor.M6] = 88.00m,
                    [Tenor.Y1] = 99.00m,
                    [Tenor.Y2] = 101.00m,
                    [Tenor.Y3] = 101.00m,
                    [Tenor.Y5] = 99.00m,
                    [Tenor.Y10] = 108.00m,
                    [Tenor.Y15] = 100.00m,
                    [Tenor.Y20] = 101.00m,
                    [Tenor.Y30] = 101.00m
                }
            };

            private static readonly Dictionary<Tenor, Dictionary<Tenor, Decimal>> CORRELATIONS_SENSITIVITY_INTERESTRATE = new Dictionary<Tenor, Dictionary<Tenor, Decimal>>
            {
                [Tenor.W2] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 1.00m,
                    [Tenor.M1] = 0.63m,
                    [Tenor.M3] = 0.59m,
                    [Tenor.M6] = 0.47m,
                    [Tenor.Y1] = 0.31m,
                    [Tenor.Y2] = 0.22m,
                    [Tenor.Y3] = 0.18m,
                    [Tenor.Y5] = 0.14m,
                    [Tenor.Y10] = 0.09m,
                    [Tenor.Y15] = 0.06m,
                    [Tenor.Y20] = 0.04m,
                    [Tenor.Y30] = 0.05m
                },
                [Tenor.M1] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 0.63m,
                    [Tenor.M1] = 1.00m,
                    [Tenor.M3] = 0.79m,
                    [Tenor.M6] = 0.67m,
                    [Tenor.Y1] = 0.52m,
                    [Tenor.Y2] = 0.42m,
                    [Tenor.Y3] = 0.37m,
                    [Tenor.Y5] = 0.30m,
                    [Tenor.Y10] = 0.23m,
                    [Tenor.Y15] = 0.18m,
                    [Tenor.Y20] = 0.15m,
                    [Tenor.Y30] = 0.13m
                },
                [Tenor.M3] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 0.59m,
                    [Tenor.M1] = 0.79m,
                    [Tenor.M3] = 1.00m,
                    [Tenor.M6] = 0.84m,
                    [Tenor.Y1] = 0.68m,
                    [Tenor.Y2] = 0.56m,
                    [Tenor.Y3] = 0.50m,
                    [Tenor.Y5] = 0.42m,
                    [Tenor.Y10] = 0.32m,
                    [Tenor.Y15] = 0.26m,
                    [Tenor.Y20] = 0.24m,
                    [Tenor.Y30] = 0.21m
                },
                [Tenor.M6] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 0.47m,
                    [Tenor.M1] = 0.67m,
                    [Tenor.M3] = 0.84m,
                    [Tenor.M6] = 1.00m,
                    [Tenor.Y1] = 0.86m,
                    [Tenor.Y2] = 0.76m,
                    [Tenor.Y3] = 0.69m,
                    [Tenor.Y5] = 0.60m,
                    [Tenor.Y10] = 0.48m,
                    [Tenor.Y15] = 0.42m,
                    [Tenor.Y20] = 0.38m,
                    [Tenor.Y30] = 0.33m
                },
                [Tenor.Y1] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 0.31m,
                    [Tenor.M1] = 0.52m,
                    [Tenor.M3] = 0.68m,
                    [Tenor.M6] = 0.86m,
                    [Tenor.Y1] = 1.00m,
                    [Tenor.Y2] = 0.94m,
                    [Tenor.Y3] = 0.89m,
                    [Tenor.Y5] = 0.80m,
                    [Tenor.Y10] = 0.67m,
                    [Tenor.Y15] = 0.60m,
                    [Tenor.Y20] = 0.57m,
                    [Tenor.Y30] = 0.53m
                },
                [Tenor.Y2] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 0.22m,
                    [Tenor.M1] = 0.42m,
                    [Tenor.M3] = 0.56m,
                    [Tenor.M6] = 0.76m,
                    [Tenor.Y1] = 0.94m,
                    [Tenor.Y2] = 1.00m,
                    [Tenor.Y3] = 0.98m,
                    [Tenor.Y5] = 0.91m,
                    [Tenor.Y10] = 0.79m,
                    [Tenor.Y15] = 0.73m,
                    [Tenor.Y20] = 0.70m,
                    [Tenor.Y30] = 0.66m
                },
                [Tenor.Y3] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 0.18m,
                    [Tenor.M1] = 0.37m,
                    [Tenor.M3] = 0.50m,
                    [Tenor.M6] = 0.69m,
                    [Tenor.Y1] = 0.89m,
                    [Tenor.Y2] = 0.98m,
                    [Tenor.Y3] = 1.00m,
                    [Tenor.Y5] = 0.96m,
                    [Tenor.Y10] = 0.87m,
                    [Tenor.Y15] = 0.81m,
                    [Tenor.Y20] = 0.78m,
                    [Tenor.Y30] = 0.74m
                },
                [Tenor.Y5] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 0.14m,
                    [Tenor.M1] = 0.30m,
                    [Tenor.M3] = 0.42m,
                    [Tenor.M6] = 0.60m,
                    [Tenor.Y1] = 0.80m,
                    [Tenor.Y2] = 0.91m,
                    [Tenor.Y3] = 0.96m,
                    [Tenor.Y5] = 1.00m,
                    [Tenor.Y10] = 0.95m,
                    [Tenor.Y15] = 0.91m,
                    [Tenor.Y20] = 0.88m,
                    [Tenor.Y30] = 0.84m
                },
                [Tenor.Y10] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 0.09m,
                    [Tenor.M1] = 0.23m,
                    [Tenor.M3] = 0.32m,
                    [Tenor.M6] = 0.48m,
                    [Tenor.Y1] = 0.67m,
                    [Tenor.Y2] = 0.79m,
                    [Tenor.Y3] = 0.87m,
                    [Tenor.Y5] = 0.95m,
                    [Tenor.Y10] = 1.00m,
                    [Tenor.Y15] = 0.98m,
                    [Tenor.Y20] = 0.97m,
                    [Tenor.Y30] = 0.94m
                },
                [Tenor.Y15] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 0.06m,
                    [Tenor.M1] = 0.18m,
                    [Tenor.M3] = 0.26m,
                    [Tenor.M6] = 0.42m,
                    [Tenor.Y1] = 0.60m,
                    [Tenor.Y2] = 0.73m,
                    [Tenor.Y3] = 0.81m,
                    [Tenor.Y5] = 0.91m,
                    [Tenor.Y10] = 0.98m,
                    [Tenor.Y15] = 1.00m,
                    [Tenor.Y20] = 0.99m,
                    [Tenor.Y30] = 0.97m
                },
                [Tenor.Y20] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 0.04m,
                    [Tenor.M1] = 0.15m,
                    [Tenor.M3] = 0.24m,
                    [Tenor.M6] = 0.38m,
                    [Tenor.Y1] = 0.57m,
                    [Tenor.Y2] = 0.70m,
                    [Tenor.Y3] = 0.78m,
                    [Tenor.Y5] = 0.88m,
                    [Tenor.Y10] = 0.97m,
                    [Tenor.Y15] = 0.99m,
                    [Tenor.Y20] = 1.00m,
                    [Tenor.Y30] = 0.99m
                },
                [Tenor.Y30] = new Dictionary<Tenor, Decimal>
                {
                    [Tenor.W2] = 0.05m,
                    [Tenor.M1] = 0.13m,
                    [Tenor.M3] = 0.21m,
                    [Tenor.M6] = 0.33m,
                    [Tenor.Y1] = 0.53m,
                    [Tenor.Y2] = 0.66m,
                    [Tenor.Y3] = 0.74m,
                    [Tenor.Y5] = 0.84m,
                    [Tenor.Y10] = 0.94m,
                    [Tenor.Y15] = 0.97m,
                    [Tenor.Y20] = 0.99m,
                    [Tenor.Y30] = 1.00m
                }
            };
            #endregion

            #region Methods
            public override Decimal GetCorrelationBucket(IBucket bucket1, IBucket bucket2)
            {
                if (bucket1 == null)
                    throw new ArgumentNullException(nameof(bucket1));

                if (bucket2 == null)
                    throw new ArgumentNullException(nameof(bucket2));

                return CORRELATION_BUCKET;
            }

            public override Decimal GetCorrelationSensitivity(Sensitivity sensitivity1, Sensitivity sensitivity2)
            {
                if (sensitivity1 == null)
                    throw new ArgumentNullException(nameof(sensitivity1));

                if (sensitivity2 == null)
                    throw new ArgumentNullException(nameof(sensitivity2));

                List<Sensitivity> sensitivities = new List<Sensitivity> { sensitivity1, sensitivity2 };

                if (sensitivities.Any(x => x.Subrisk == SensitivitySubrisk.CrossCurrencyBasis))
                    return CORRELATION_SENSITIVITY_CROSSCURRENCYBASIS;

                if (sensitivities.Any(x => x.Subrisk == SensitivitySubrisk.Inflation))
                    return CORRELATION_SENSITIVITY_INFLATION;

                Decimal correlation = CORRELATIONS_SENSITIVITY_INTERESTRATE[sensitivity1.Tenor][sensitivity2.Tenor];

                if (!String.Equals(sensitivity1.Label2, sensitivity2.Label2, StringComparison.Ordinal))
                    correlation *= CORRELATION_SENSITIVITY_CURVES;

                return correlation;
            }

            public override Decimal GetRiskWeightDelta(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                switch (sensitivity.Subrisk)
                {
                    case SensitivitySubrisk.Inflation:
                        return RISKWEIGHT_DELTA_INFLATION;

                    case SensitivitySubrisk.InterestRate:
                        return RISKWEIGHTS_DELTA_INTERESTRATE[sensitivity.Currency.Volatility][sensitivity.Tenor];

                    default:
                        return RISKWEIGHT_DELTA_CROSSCURRENCYBASIS;
                }
            }

            public override Decimal GetRiskWeightVega(Sensitivity sensitivity)
            {
                if (sensitivity == null)
                    throw new ArgumentNullException(nameof(sensitivity));

                return RISKWEIGHT_VEGA;
            }

            public override Decimal GetThresholdDelta(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                Currency currency = (Currency)thresholdIdentifier;

                switch (currency.Volatility)
                {
                    case CurrencyVolatility.High:
                        return THRESHOLD_DELTA_HIGH;

                    case CurrencyVolatility.Low:
                        return THRESHOLD_DELTA_LOW;

                    default:
                    {
                        if (currency.Liquidity == CurrencyLiquidity.High)
                            return THRESHOLD_DELTA_REGULAR_HIGH;

                        return THRESHOLD_DELTA_REGULAR_MEDIUM;
                    }
                }
            }

            public override Decimal GetThresholdVega(IThresholdIdentifier thresholdIdentifier)
            {
                if (thresholdIdentifier == null)
                    throw new ArgumentNullException(nameof(thresholdIdentifier));

                Currency currency = (Currency)thresholdIdentifier;

                switch (currency.Volatility)
                {
                    case CurrencyVolatility.High:
                        return THRESHOLD_VEGA_HIGH;

                    case CurrencyVolatility.Low:
                        return THRESHOLD_VEGA_LOW;

                    default:
                    {
                        if (currency.Liquidity == CurrencyLiquidity.High)
                            return THRESHOLD_VEGA_REGULAR_HIGH;

                        return THRESHOLD_VEGA_REGULAR_MEDIUM;
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}
