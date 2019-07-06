#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using InitialMargin.Core;
#endregion

namespace InitialMargin.IO
{
    public sealed class FxRatesManager
    {
        #region Members
        private readonly Encoding m_Encoding;
        private readonly IFormatProvider m_FormatProvider;
        #endregion

        #region Members (Static)
        private static readonly String[] s_HeaderFields =
        {
            "Currency1", "Currency2", "Rate"
        };
        #endregion

        #region Properties
        public Encoding Encoding => m_Encoding;

        public IFormatProvider FormatProvider => m_FormatProvider;
        #endregion

        #region Constructors
        public FxRatesManager(Encoding encoding, IFormatProvider formatProvider)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (formatProvider == null)
                throw new ArgumentNullException(nameof(formatProvider));

            m_Encoding = encoding;
            m_FormatProvider = formatProvider;
        }

        public FxRatesManager() : this(Encoding.UTF8, CultureInfo.CurrentCulture) { }
        #endregion

        #region Methods
        public FxRatesProvider Read(String filePath)
        {
            return Read(filePath, CsvUtilities.DefaultDelimiter, false);
        }

        public FxRatesProvider Read(String filePath, Char delimiter)
        {
            return Read(filePath, delimiter, false);
        }

        public FxRatesProvider Read(String filePath, Boolean header)
        {
            return Read(filePath, CsvUtilities.DefaultDelimiter, header);
        }

        public FxRatesProvider Read(String filePath, Char delimiter, Boolean header)
        {
            if (!CsvUtilities.IsValidDelimiter(delimiter))
                throw new ArgumentException($"Invalid delimiter specified (accepted delimiters are: {CsvUtilities.ValidDelimiters}).", nameof(delimiter));

            List<String[]> fieldsMatrix = CsvParser.Parse(filePath, m_Encoding, delimiter, header);
            
            FxRatesProvider ratesProvider = new FxRatesProvider();

            if (fieldsMatrix.Count == 0)
                return ratesProvider;

            foreach (var tuple in fieldsMatrix.Select((x, i) => new { Index = i, Value = x }))
            {
                Int32 index = tuple.Index + (header ? 2 : 1);
                String[] values = tuple.Value;

                if (values.Length != 3)
                    throw new InvalidDataException($"[{filePath}, Line {index}] The rates file contains an entry whose number of columns is not equal to 3.");

                if (!Currency.TryParse(values[0], out Currency currency1))
                    throw new InvalidDataException($"[{filePath}, Line {index}] The rates file contains an entry whose first currency ({values[0]}) is invalid.");

                if (!Currency.TryParse(values[1], out Currency currency2))
                    throw new InvalidDataException($"[{filePath}, Line {index}] The rates file contains an entry whose second currency ({values[1]}) is invalid.");

                if (!Decimal.TryParse(values[2], NumberStyles.Any, m_FormatProvider, out Decimal rate) || (rate <= 0m))
                    throw new InvalidDataException($"[{filePath}, Line {index}] The rates file contains an entry whose rate is invalid.");

                if (currency1 == currency2)
                {
                    if (rate != 1m)
                        throw new InvalidDataException($"[{filePath}, Line {index}] The rates file contains an entry with two identical currencies and a rate not equal to 1.");

                    continue;
                }

                CurrencyPair currencyPair = CurrencyPair.Of(currency1, currency2);

                if (ratesProvider.OriginalRates.ContainsKey(currencyPair))
                    throw new InvalidDataException($"[{filePath}, Line {index}] The rates file contains a duplicate entry.");

                ratesProvider.AddRate(currencyPair, rate);
            }

            return ratesProvider;
        }

        public void Write(String filePath, FxRatesProvider ratesProvider)
        {
            Write(filePath, ratesProvider, CsvUtilities.DefaultDelimiter, false);
        }

        public void Write(String filePath, FxRatesProvider ratesProvider, Char delimiter)
        {
            Write(filePath, ratesProvider, delimiter, false);
        }

        public void Write(String filePath, FxRatesProvider ratesProvider, Boolean header)
        {
            Write(filePath, ratesProvider, CsvUtilities.DefaultDelimiter, header);
        }

        public void Write(String filePath, FxRatesProvider ratesProvider, Char delimiter, Boolean header)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid file path specified.", nameof(filePath));

            if (Path.GetExtension(filePath).ToUpperInvariant() != ".CSV")
                throw new ArgumentException("The specified file must be a CSV.", nameof(filePath));

            if (ratesProvider == null)
                throw new ArgumentNullException(nameof(ratesProvider));

            if (!CsvUtilities.IsValidDelimiter(delimiter))
                throw new ArgumentException($"Invalid delimiter specified (accepted delimiters are: {CsvUtilities.ValidDelimiters}).", nameof(delimiter));

            ReadOnlyDictionary<CurrencyPair,Decimal> rates = ratesProvider.OriginalRates;
            List<String[]> fieldsMatrix = new List<String[]>(rates.Count);

            foreach (KeyValuePair<CurrencyPair,Decimal> rate in rates)
            {
                CurrencyPair currencyPair = rate.Key;

                String[] row = new String[3];
                row[0] = currencyPair.Currency1.ToString().ToUpperInvariant();
                row[1] = currencyPair.Currency2.ToString().ToUpperInvariant();
                row[2] = rate.Value.ToString(m_FormatProvider);

                fieldsMatrix.Add(row);
            }

            String result = CsvUtilities.FinalizeFieldsMatrix(header ? s_HeaderFields : null, fieldsMatrix, delimiter, Environment.NewLine);

            File.WriteAllText(filePath, result, m_Encoding);
        }
        #endregion
    }
}