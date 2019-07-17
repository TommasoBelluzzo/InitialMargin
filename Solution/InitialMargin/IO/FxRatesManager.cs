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
    /// <summary>Represents a data manager in charge of reading and writing rates providers. This class cannot be derived.</summary>
    public sealed class FxRatesManager : IDataManager
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
        /// <summary>Gets the encoding used by the rates manager.</summary>
        /// <value>An <see cref="T:System.Text.Encoding"/> object.</value>
        public Encoding Encoding => m_Encoding;

        /// <summary>Gets the culture-specific format used by the rates manager.</summary>
        /// <value>An <see cref="T:System.IFormatProvider"/> object.</value>
        public IFormatProvider FormatProvider => m_FormatProvider;
        #endregion

        #region Constructors
        private FxRatesManager(Encoding encoding, IFormatProvider formatProvider)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (formatProvider == null)
                throw new ArgumentNullException(nameof(formatProvider));

            m_Encoding = encoding;
            m_FormatProvider = formatProvider;
        }
        #endregion

        #region Methods
        /// <summary>Opens a rates file and reads its content, using the default delimiter (see <see cref="P:InitialMargin.IO.CsvUtilities.DefaultDelimiter"/>) and assuming the file does not contain a header.</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to open for reading.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.FxRatesProvider"/> object containing the rates defined in the file.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="filePath">filePath</paramref> is invalid or does not refer to a CSV file.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">Thrown when <paramref name="filePath">filePath</paramref> could not be found.</exception>
        /// <exception cref="T:System.IO.InvalidDataException">Thrown when the rates file contains invalid or malformed data.</exception>
        public FxRatesProvider Read(String filePath)
        {
            return Read(filePath, CsvUtilities.DefaultDelimiter, false);
        }

        /// <summary>Opens a rates file and reads its content, using the specified delimiter and assuming the file does not contain a header.</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to open for reading.</param>
        /// <param name="delimiter">The <see cref="T:System.Char"/> representing the delimiter.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.FxRatesProvider"/> object containing the rates defined in the file.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="filePath">filePath</paramref> is invalid or does not refer to a CSV file, or when <paramref name="delimiter">delimiter</paramref> is invalid (see <see cref="M:InitialMargin.IO.CsvUtilities.IsValidDelimiter(System.Char)"/>).</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">Thrown when <paramref name="filePath">filePath</paramref> could not be found.</exception>
        /// <exception cref="T:System.IO.InvalidDataException">Thrown when the rates file contains invalid or malformed data.</exception>
        public FxRatesProvider Read(String filePath, Char delimiter)
        {
            return Read(filePath, delimiter, false);
        }

        /// <summary>Opens a rates file and reads its content, using the default delimiter (see <see cref="P:InitialMargin.IO.CsvUtilities.DefaultDelimiter"/>). A parameter specifies whether the file contains a header that must be skipped.</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to open for reading.</param>
        /// <param name="header"><c>true</c> if the rates file contains a header that must be skipped; otherwise, <c>false</c>.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.FxRatesProvider"/> object containing the rates defined in the file.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="filePath">filePath</paramref> is invalid or does not refer to a CSV file.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">Thrown when <paramref name="filePath">filePath</paramref> could not be found.</exception>
        /// <exception cref="T:System.IO.InvalidDataException">Thrown when the rates file contains invalid or malformed data.</exception>
        public FxRatesProvider Read(String filePath, Boolean header)
        {
            return Read(filePath, CsvUtilities.DefaultDelimiter, header);
        }

        /// <summary>Opens a rates file and reads its content, using the specified delimiter. A parameter specifies whether the file contains a header that must be skipped.</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to open for reading.</param>
        /// <param name="delimiter">The <see cref="T:System.Char"/> representing the delimiter.</param>
        /// <param name="header"><c>true</c> if the rates file contains a header that must be skipped; otherwise, <c>false</c>.</param>
        /// <returns>An <see cref="T:InitialMargin.Core.FxRatesProvider"/> object containing the rates defined in the file.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="filePath">filePath</paramref> is invalid or does not refer to a CSV file, or when <paramref name="delimiter">delimiter</paramref> is invalid (see <see cref="M:InitialMargin.IO.CsvUtilities.IsValidDelimiter(System.Char)"/>).</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">Thrown when <paramref name="filePath">filePath</paramref> could not be found.</exception>
        /// <exception cref="T:System.IO.InvalidDataException">Thrown when the rates file contains invalid or malformed data.</exception>
        public FxRatesProvider Read(String filePath, Char delimiter, Boolean header)
        {
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

        /// <summary>Creates a new rates file and writes the content of the specified rates provider to it, using the default delimiter (see <see cref="P:InitialMargin.IO.CsvUtilities.DefaultDelimiter"/>) and without prepending a header. If the target file already exists, it is overwritten.</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to write to.</param>
        /// <param name="ratesProvider">The <see cref="T:InitialMargin.Core.FxRatesProvider"/> object to write.</param>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="filePath">filePath</paramref> is invalid or does not refer to a CSV file.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="ratesProvider">ratesProvider</paramref> is <c>null</c>.</exception>
        public void Write(String filePath, FxRatesProvider ratesProvider)
        {
            Write(filePath, ratesProvider, CsvUtilities.DefaultDelimiter, false);
        }

        /// <summary>Creates a new rates file and writes the content of the specified rates provider to it, using the specified delimiter and without prepending a header. If the target file already exists, it is overwritten.</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to write to.</param>
        /// <param name="ratesProvider">The <see cref="T:InitialMargin.Core.FxRatesProvider"/> object to write.</param>
        /// <param name="delimiter">The <see cref="T:System.Char"/> representing the delimiter.</param>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="filePath">filePath</paramref> is invalid or does not refer to a CSV file, or when <paramref name="delimiter">delimiter</paramref> is invalid (see <see cref="M:InitialMargin.IO.CsvUtilities.IsValidDelimiter(System.Char)"/>).</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="ratesProvider">ratesProvider</paramref> is <c>null</c>.</exception>
        public void Write(String filePath, FxRatesProvider ratesProvider, Char delimiter)
        {
            Write(filePath, ratesProvider, delimiter, false);
        }

        /// <summary>Creates a new rates file and writes the content of the specified rates provider to it, using the default delimiter (see <see cref="P:InitialMargin.IO.CsvUtilities.DefaultDelimiter"/>). A parameter specifies whether the file must contain a header. If the target file already exists, it is overwritten.</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to write to.</param>
        /// <param name="ratesProvider">The <see cref="T:InitialMargin.Core.FxRatesProvider"/> object to write.</param>
        /// <param name="header"><c>true</c> if the rates file must contain a header; otherwise, <c>false</c>.</param>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="filePath">filePath</paramref> is invalid or does not refer to a CSV file.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="ratesProvider">ratesProvider</paramref> is <c>null</c>.</exception>
        public void Write(String filePath, FxRatesProvider ratesProvider, Boolean header)
        {
            Write(filePath, ratesProvider, CsvUtilities.DefaultDelimiter, header);
        }

        /// <summary>Creates a new rates file and writes the content of the specified rates provider to it, using the specified delimiter. A parameter specifies whether the file must contain a header. If the target file already exists, it is overwritten.</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to write to.</param>
        /// <param name="ratesProvider">The <see cref="T:InitialMargin.Core.FxRatesProvider"/> object to write.</param>
        /// <param name="delimiter">The <see cref="T:System.Char"/> representing the delimiter.</param>
        /// <param name="header"><c>true</c> if the rates file must contain a header; otherwise, <c>false</c>.</param>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="filePath">filePath</paramref> is invalid or does not refer to a CSV file, or when <paramref name="delimiter">delimiter</paramref> is invalid (see <see cref="M:InitialMargin.IO.CsvUtilities.IsValidDelimiter(System.Char)"/>).</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="ratesProvider">ratesProvider</paramref> is <c>null</c>.</exception>
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
                row[0] = currencyPair.CurrencyBase.ToString().ToUpperInvariant();
                row[1] = currencyPair.CurrencyCounter.ToString().ToUpperInvariant();
                row[2] = rate.Value.ToString(m_FormatProvider);

                fieldsMatrix.Add(row);
            }

            String result = CsvUtilities.FinalizeFieldsMatrix(header ? s_HeaderFields : null, fieldsMatrix, delimiter, Environment.NewLine);

            File.WriteAllText(filePath, result, m_Encoding);
        }
        #endregion

        #region Methods (Static)
        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="encoding">The <see cref="T:System.Text.Encoding"/> representing the character encoding in which output files are written.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider"/> supplying the culture-specific format.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.IO.FxRatesManager"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="encoding">encoding</paramref> is <c>null</c> or when <paramref name="formatProvider">formatProvider</paramref> is <c>null</c>.</exception>
        public static FxRatesManager Of(Encoding encoding, IFormatProvider formatProvider)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (formatProvider == null)
                throw new ArgumentNullException(nameof(formatProvider));

            return (new FxRatesManager(encoding, formatProvider));
        }

        /// <summary>Initializes and returns a new instance using <see cref="P:System.Text.Encoding.UTF8"/> as character encoding and <see cref="P:System.Globalization.CultureInfo.CurrentCulture"/> as culture-specific format.</summary>
        /// <returns>A new instance of <see cref="T:InitialMargin.IO.FxRatesManager"/> initialized with the default parameters.</returns>
        public static FxRatesManager OfDefault()
        {
            return (new FxRatesManager(Encoding.UTF8, CultureInfo.CurrentCulture));
        }
        #endregion
    }
}