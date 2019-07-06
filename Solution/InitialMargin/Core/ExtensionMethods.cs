#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace InitialMargin.Core
{
    internal static class ExtensionMethods
    {
        #region Decimal
        public static Decimal Normalize(this Decimal value)
        {
            return (value / 1.000000000000000000000000000000000m);
        }
        #endregion

        #region List
        public static List<T> RemoveAllAndGet<T>(this List<T> list, Func<T,Boolean> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            List<T> removedElements = list.Where(predicate).ToList();
            list.RemoveAll(new Predicate<T>(predicate));

            return removedElements;
        }
        #endregion
    }
}