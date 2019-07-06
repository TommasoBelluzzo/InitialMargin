#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace InitialMargin.IO
{
    internal static class CsvUtilities
    {
        #region Members
        private static readonly Dictionary<String,Char> s_ValidDelimiters = new Dictionary<String,Char>
        {
            ["Comma"] = ',',
            ["Semicolon"] = ';',
            ["Space"] = ' ',
            ["Tab"] = '\t'
        };
        #endregion

        #region Properties
        public static Char DefaultDelimiter => s_ValidDelimiters.Values.ElementAt(0);

        public static String ValidDelimiters => String.Join(", ", s_ValidDelimiters.Keys).ToLowerInvariant();
        #endregion

        #region Methods
        public static Boolean IsValidDelimiter(Char delimiter)
        {
            return s_ValidDelimiters.Values.Contains(delimiter);
        }

        public static String FinalizeFieldsMatrix(String[] headerFields, List<String[]> fieldsMatrix, Char delimiter, String newLine)
        {
            if (fieldsMatrix == null)
                throw new ArgumentNullException(nameof(fieldsMatrix));

            if ((headerFields != null) && fieldsMatrix.Any(x => x.Length != headerFields.Length))
                throw new ArgumentException("");

            if (!IsValidDelimiter(delimiter))
                throw new ArgumentException($"Invalid delimiter specified (accepted delimiters are: {s_ValidDelimiters}).", nameof(delimiter));

            String d = delimiter.ToString();

            List<String> rows = fieldsMatrix
                .Select(fr => String.Join(d.ToString(), fr.Select(f => f.Contains(d) ? $"\"{f}\"" : f)))
                .ToList();

            String result;

            if (headerFields == null)
                result = String.Join(newLine, rows);
            else
                result = String.Concat(String.Join(d, headerFields), newLine, String.Join(newLine, rows));

            return result.Trim();
        }
        #endregion
    }
}
