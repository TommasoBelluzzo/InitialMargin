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
        public Encoding Encoding => m_Encoding;

        public IFormatProvider FormatProvider => m_FormatProvider;
        #endregion

        #region Constructors
        public OutputWriterCsv(Encoding encoding, IFormatProvider formatProvider)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (formatProvider == null)
                throw new ArgumentNullException(nameof(formatProvider));

            m_Encoding = encoding;
            m_FormatProvider = formatProvider;
        }

        public OutputWriterCsv() : this(Encoding.UTF8, CultureInfo.CurrentCulture) { }
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

        public void Write(String filePath, MarginTotal margin)
        {
            Write(filePath, margin, CsvUtilities.DefaultDelimiter);
        }

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
    }

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
        public Encoding Encoding => m_Encoding;

        public IFormatProvider FormatProvider => m_FormatProvider;
        #endregion

        #region Constructors
        public OutputWriterTree(Encoding encoding, IFormatProvider formatProvider)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (formatProvider == null)
                throw new ArgumentNullException(nameof(formatProvider));

            m_Encoding = encoding;
            m_FormatProvider = formatProvider;
        }

        public OutputWriterTree() : this(Encoding.UTF8, CultureInfo.CurrentCulture) { }
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
    }
}