#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using InitialMargin.Core;
#endregion

namespace InitialMargin.IO
{
    /// <summary>Represents a data manager in charge of writing calculation results into CSV files, using a tabular structure. This class cannot be derived.</summary>
    public sealed class OutputWriterCsv : IOutputWriter
    {
        #region Members
        private readonly Encoding m_Encoding;
        private readonly IFormatProvider m_FormatProvider;
        #endregion

        #region Members (Static)
        private static readonly String[] s_CsvHeaderFields =
        {
            "Level",
            "Category1",
            "Category2",
            "Risk",
            "Sensitivity",
            "Bucket",
            "Weighting",
            "ExposureAmount"
        };
        #endregion

        #region Properties
        /// <summary>Gets the encoding used by the output writer.</summary>
        /// <value>An <see cref="T:System.Text.Encoding"/> object.</value>
        public Encoding Encoding => m_Encoding;

        /// <summary>Gets the culture-specific format used by the output writer.</summary>
        /// <value>An <see cref="T:System.IFormatProvider"/> object.</value>
        public IFormatProvider FormatProvider => m_FormatProvider;
        #endregion

        #region Constructors
        private OutputWriterCsv(Encoding encoding, IFormatProvider formatProvider)
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
        private void WriteCsvRecursive(IMargin margin, List<String[]> fieldsMatrix)
        {
            Int32 level = margin.Level;
            String[] fieldsRow;

            if (fieldsMatrix.Count == 0)
            {
                fieldsRow = new String[s_CsvHeaderFields.Length];
                fieldsRow[0] = level.ToString(m_FormatProvider);
                fieldsRow[1] = margin.Identifier;

                for (Int32 i = 2; i < s_CsvHeaderFields.Length - 1; ++i)
                    fieldsRow[i] = String.Empty;
            }
            else
            {
                fieldsRow = (String[])fieldsMatrix.Last().Clone();
                fieldsRow[0] = level.ToString(m_FormatProvider);
                fieldsRow[level - 1] = margin.Identifier;

                for (Int32 i = level; i < s_CsvHeaderFields.Length - 1; ++i)
                    fieldsRow[i] = String.Empty;
            }

            fieldsRow[s_CsvHeaderFields.Length - 1] = Amount.Round(margin.Value, 2).ToString(m_FormatProvider, CurrencyCodeSymbol.None);

            fieldsMatrix.Add(fieldsRow);

            foreach (IMargin child in margin.Children)
                WriteCsvRecursive(child, fieldsMatrix);
        }

        /// <summary>Writes the specified calculation result into the specified CSV file, using the default delimiter (see <see cref="P:InitialMargin.IO.CsvUtilities.DefaultDelimiter"/>).</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to write to.</param>
        /// <param name="margin">The <see cref="T:InitialMargin.Core.MarginTotal"/> object to write.</param>
        public void Write(String filePath, MarginTotal margin)
        {
            Write(filePath, margin, CsvUtilities.DefaultDelimiter);
        }

        /// <summary>Writes the specified calculation result into the specified CSV file, using the specified delimiter.</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to write to.</param>
        /// <param name="margin">The <see cref="T:InitialMargin.Core.MarginTotal"/> object to write.</param>
        /// <param name="delimiter">The <see cref="T:System.Char"/> representing the delimiter.</param>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="filePath">filePath</paramref> is invalid or does not refer to a CSV file, or when <paramref name="delimiter">delimiter</paramref> is invalid (see <see cref="M:InitialMargin.IO.CsvUtilities.IsValidDelimiter(System.Char)"/>).</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="margin">margin</paramref> is <c>null</c>.</exception>
        public void Write(String filePath, MarginTotal margin, Char delimiter)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid file path specified.", nameof(filePath));

            if (Path.GetExtension(filePath).ToUpperInvariant() != ".CSV")
                throw new ArgumentException("The specified file must be a CSV.", nameof(filePath));

            if (margin == null)
                throw new ArgumentNullException(nameof(margin));

            if (!CsvUtilities.IsValidDelimiter(delimiter))
                throw new ArgumentException($"Invalid delimiter specified (accepted delimiters are: {CsvUtilities.ValidDelimiters}).", nameof(delimiter));

            List<String[]> fieldsMatrix = new List<String[]>();
            WriteCsvRecursive(margin, fieldsMatrix);

            String result = CsvUtilities.FinalizeFieldsMatrix(s_CsvHeaderFields, fieldsMatrix, delimiter, Environment.NewLine);

            File.WriteAllText(filePath, result, m_Encoding);
        }
        #endregion

        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="encoding">The <see cref="T:System.Text.Encoding"/> representing the character encoding in which output files are written.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider"/> supplying the culture-specific format.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.IO.OutputWriterCsv"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="encoding">encoding</paramref> is <c>null</c> or when <paramref name="formatProvider">formatProvider</paramref> is <c>null</c>.</exception>
        public static OutputWriterCsv Of(Encoding encoding, IFormatProvider formatProvider)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (formatProvider == null)
                throw new ArgumentNullException(nameof(formatProvider));

            return (new OutputWriterCsv(encoding, formatProvider));
        }

        /// <summary>Initializes and returns a new instance using <see cref="P:System.Text.Encoding.UTF8"/> as character encoding and <see cref="P:System.Globalization.CultureInfo.CurrentCulture"/> as culture-specific format.</summary>
        /// <returns>A new instance of <see cref="T:InitialMargin.IO.OutputWriterCsv"/> initialized with the default parameters.</returns>
        public static OutputWriterCsv OfDefault()
        {
            return (new OutputWriterCsv(Encoding.UTF8, CultureInfo.CurrentCulture));
        }
    }

    /// <summary>Represents a data manager in charge of writing calculation results into plain-text files, using a tree structure. This class cannot be derived.</summary>
    public sealed class OutputWriterTree : IOutputWriter
    {
        #region Constants
        private const String TREE_CORNER = " └─";
        private const String TREE_CROSS = " ├─";
        private const String TREE_LINE = " │ ";
        private const String TREE_SPACE = "   ";
        #endregion

        #region Members
        private readonly Encoding m_Encoding;
        private readonly IFormatProvider m_FormatProvider;
        #endregion

        #region Members (Static)
        private static readonly String[] s_ValidExtensions =
        {
            ".LOG", ".OUT", ".TXT"
        };
        #endregion

        #region Properties
        /// <summary>Gets the encoding used by the output writer.</summary>
        /// <value>An <see cref="T:System.Text.Encoding"/> object.</value>
        public Encoding Encoding => m_Encoding;

        /// <summary>Gets the culture-specific format used by the output writer.</summary>
        /// <value>An <see cref="T:System.IFormatProvider"/> object.</value>
        public IFormatProvider FormatProvider => m_FormatProvider;
        #endregion

        #region Constructors
        private OutputWriterTree(Encoding encoding, IFormatProvider formatProvider)
        {
            m_Encoding = encoding;
            m_FormatProvider = formatProvider;
        }
        #endregion

        #region Methods
        private void WriteTreeNode(IMargin margin, StringBuilder builder, String indent)
        {
            builder.Append($"{margin.Name}");

            if (margin.Name != margin.Identifier)
            {
                if (margin.Identifier.Contains(" ") && (margin.Name != "Add-on"))
                    builder.Append($" \"{margin.Identifier}\"");
                else
                    builder.Append($" {margin.Identifier}");
            }

            builder.AppendLine($" = {Amount.Round(margin.Value, 2).ToString(m_FormatProvider, CurrencyCodeSymbol.None)}");

            Int32 lastIndex = margin.ChildrenCount - 1;

            for (Int32 i = 0; i < margin.ChildrenCount; ++i)
            {
                IMargin child = margin.Children[i];
                WriteTreeNodeChild(child, builder, indent, (i == lastIndex));
            }
        }

        private void WriteTreeNodeChild(IMargin margin, StringBuilder builder, String indent, Boolean isLastChild)
        {
            builder.Append(indent);

            if (isLastChild)
            {
                builder.Append(TREE_CORNER);
                indent += TREE_SPACE;
            }
            else
            {
                builder.Append(TREE_CROSS);
                indent += TREE_LINE;
            }

            WriteTreeNode(margin, builder, indent);
        }

        /// <summary>Writes the specified calculation result into the specified plain-text file.</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to write to.</param>
        /// <param name="margin">The <see cref="T:InitialMargin.Core.MarginTotal"/> object to write.</param>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="filePath">filePath</paramref> is invalid or does not refer to a plain-text file.</exception>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="margin">margin</paramref> is <c>null</c>.</exception>
        public void Write(String filePath, MarginTotal margin)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid file path specified.", nameof(filePath));

            if (!s_ValidExtensions.Contains(Path.GetExtension(filePath).ToUpperInvariant()))
                throw new ArgumentException($"Invalid file extension specified (accepted extensions are: {String.Join(", ", s_ValidExtensions).Replace(".", String.Empty)}).", nameof(filePath));

            if (margin == null)
                throw new ArgumentNullException(nameof(margin));

            StringBuilder builder = new StringBuilder("Total");
            builder.AppendLine($" = {Amount.Round(margin.Value, 2).ToString(m_FormatProvider, CurrencyCodeSymbol.None)}");

            if (margin.ChildrenCount > 0)
            {
                Int32 lastIndex = margin.ChildrenCount - 1;

                for (Int32 i = 0; i < margin.ChildrenCount; ++i)
                    WriteTreeNodeChild(margin.Children[i], builder, String.Empty, (i == lastIndex));
            }

            String result = builder.ToString().Trim();

            File.WriteAllText(filePath, result, m_Encoding);
        }
        #endregion

        #region Methods (Static)
        /// <summary>Initializes and returns a new instance using the specified parameters.</summary>
        /// <param name="encoding">The <see cref="T:System.Text.Encoding"/> representing the character encoding in which output files are written.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider"/> supplying the culture-specific format.</param>
        /// <returns>A new instance of <see cref="T:InitialMargin.IO.OutputWriterTree"/> initialized with the specified parameters.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="encoding">encoding</paramref> is <c>null</c> or when <paramref name="formatProvider">formatProvider</paramref> is <c>null</c>.</exception>
        public static OutputWriterTree Of(Encoding encoding, IFormatProvider formatProvider)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (formatProvider == null)
                throw new ArgumentNullException(nameof(formatProvider));

            return (new OutputWriterTree(encoding, formatProvider));
        }

        /// <summary>Initializes and returns a new instance using <see cref="P:System.Text.Encoding.UTF8"/> as character encoding and <see cref="P:System.Globalization.CultureInfo.CurrentCulture"/> as culture-specific format.</summary>
        /// <returns>A new instance of <see cref="T:InitialMargin.IO.OutputWriterTree"/> initialized with the default parameters.</returns>
        public static OutputWriterTree OfDefault()
        {
            return (new OutputWriterTree(Encoding.UTF8, CultureInfo.CurrentCulture));
        }
        #endregion
    }
}