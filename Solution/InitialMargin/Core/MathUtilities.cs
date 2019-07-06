#region Using Directives
using System;
using System.Collections;
#endregion

namespace InitialMargin.Core
{
    internal static class MathUtilities
    {
        #region Members
        private static readonly BitArray s_SquareExponent = new BitArray(BitConverter.GetBytes(2u));
        #endregion

        #region Methods
        public static Decimal Square(Decimal value)
        {
            Decimal result = 1m;

            for (Int32 i = s_SquareExponent.Count - 1; i >= 0; --i)
            {
                result *= result;

                if (s_SquareExponent[i])
                    result *= value;
            }

            return result;
        }

        public static Decimal SquareRoot(Decimal value)
        {
            if (value < 0m)
                throw new ArgumentOutOfRangeException(nameof(value), "Invalid value specified.");

            if (value == 0m)
                return 0m;

            Decimal previousGuess = 0m;
            Decimal guess = value / 2m;

            while (Math.Abs(guess - previousGuess) > 0m)
            {
                previousGuess = guess;
                guess = ((value / guess) + guess) / 2m;
            }

            return guess;
        }
        #endregion
    }
}