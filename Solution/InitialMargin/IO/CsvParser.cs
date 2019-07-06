#region Using Directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#endregion

namespace InitialMargin.IO
{
    internal sealed class CsvParser : IDisposable
    {
        #region Members
        private readonly Char m_Delimiter;
        private Boolean m_Disposed;
        private Int32 m_PeekedEmptyLines;
        private String m_PeekedLine;
        private TextReader m_Reader;
        #endregion

        #region Properties
        public Boolean EndOfData => !CanRead();
        #endregion

        #region Constructors
        private CsvParser(String path, Char delimiter, Encoding encoding)
        {
            m_Delimiter = delimiter;
            m_Reader = new StreamReader(path, encoding);
        }
        #endregion

        #region Destructors
        ~CsvParser()
        {
            Dispose(false);
        }
        #endregion

        #region Methods
        private Boolean CanRead()
        {
            if (m_PeekedLine != null)
                return true;

            if (m_PeekedEmptyLines > 0)
                return false;

            String nextLine = ReadNextLine(true, out Int32 ignoredEmptyLines);
            
            if (nextLine == null)
            {
                if (ignoredEmptyLines > 0)
                    m_PeekedEmptyLines = ignoredEmptyLines;

                return false;
            }

            m_PeekedLine = nextLine;
            m_PeekedEmptyLines = ignoredEmptyLines;

            return true;
        }

        private String ParseField(ref String line, ref Int32 index)
        {
            if (line[index] == '"')
                return ParseFieldAfterQuote(ref line, ref index);

            Int32 delimiterIndex = line.IndexOf(m_Delimiter, index);

            String field;

            if (delimiterIndex >= 0)
            {
                field = line.Substring(index, delimiterIndex - index);
                index = delimiterIndex + 1;
            }
            else
            {
                field = line.Substring(index).TrimEnd('\n', '\r');
                index = line.Length;
            }

            return field.Trim();
        }

        private String ParseFieldAfterQuote(ref String line, ref Int32 index)
        {
            Boolean quoteClosed = false;
            Int32 i = index + 1;
            StringBuilder builder = new StringBuilder();

            do
            {
                while (i < line.Length)
                {
                    Char currentCharacter = line[i];

                    if (currentCharacter == '"')
                    {
                        if ((i + 1) < line.Length)
                        {
                            Char nextCharacter = line[i + 1];

                            if (nextCharacter == '"')
                            {
                                builder.Append(nextCharacter);
                                i += 2;

                                continue;
                            }
                        }

                        quoteClosed = true;
                        ++i;

                        break;

                    }

                    builder.Append(currentCharacter);
                    ++i;
                }

                if (!quoteClosed && (i >= line.Length))
                {
                    String nextLine = ReadNextLine(false, out Int32 _);
                    
                    if (nextLine == null)
                        throw new InvalidDataException("A quoted field is not closed.");
                    
                    line += nextLine;
                }

            }
            while (!quoteClosed && (i < line.Length));

            Boolean isMalformed = true;

            if (quoteClosed)
            {
                if (i >= line.Length)
                    isMalformed = false;
                else
                {
                    Char currentCharacter = line[i];

                    if (currentCharacter == m_Delimiter)
                        isMalformed = false;
                    else if ((currentCharacter == '\n') || (currentCharacter == '\r'))
                    {
                        if ((currentCharacter == '\r') && ((i + 1) < line.Length) && (line[i + 1] == '\n'))
                            ++i;

                        ++i;

                        isMalformed = false;
                    }
                }
            }

            if (isMalformed)
                throw new InvalidDataException("There are trailing characters after a closing quote.");

            index = i + 1;

            return builder.ToString().Trim();
        }

        private String ReadNextLine(Boolean ignoreEmptyLines, out Int32 ignoredEmptyLineCount)
        {
            String line = ReadNextLineOrWhitespace();

            if ((line == null) && (m_PeekedEmptyLines > 0))
                m_PeekedEmptyLines = 0;

            ignoredEmptyLineCount = 0;

            if (ignoreEmptyLines)
            {
                while ((line != null) && (String.IsNullOrEmpty(line) || line.All(x => (x == '\n') || (x == '\r'))))
                {
                    ++ignoredEmptyLineCount;
                    line = ReadNextLineOrWhitespace();
                }
            }

            return line;
        }

        private String ReadNextLineOrWhitespace()
        {
            if (m_PeekedLine != null)
            {
                String temp = m_PeekedLine;
                m_PeekedLine = null;
                m_PeekedEmptyLines = 0;

                return temp;
            }

            StringBuilder builder = new StringBuilder();

            while (true)
            {
                Int32 character = m_Reader.Read();

                if (character == -1)
                    break;

                builder.Append((Char)character);

                if ((character == '\n') || (character == '\r'))
                {
                    if ((character == '\r') && (m_Reader.Peek() == '\n'))
                        builder.Append((Char)m_Reader.Read());

                    break;
                }
            }

            return ((builder.Length == 0) ? null : builder.ToString());
        }

        private void Dispose(Boolean disposing)
        {
            if (m_Disposed)
                return;

            if (disposing && (m_Reader != null))
            {
                m_Reader.Dispose();
                m_Reader = null;
            }

            m_Disposed = true;
        }

        public String[] ReadFields()
        {
            String line = ReadNextLine(true, out Int32 _);

            if (line == null)
                return null;

            Int32 index = 0;
            List<String> fields = new List<String>();

            while (index < line.Length)
                fields.Add(ParseField(ref line, ref index));

            if ((line.Length == 0) || (line[line.Length - 1] == m_Delimiter))
                fields.Add(String.Empty);

            return fields.ToArray();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Methods (Static)
        public static List<String[]> Parse(String filePath, Encoding encoding, Char delimiter, Boolean skipHeader)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid CSV file specified.", nameof(filePath));

            if (Path.GetExtension(filePath).ToUpperInvariant() != ".CSV")
                throw new ArgumentException("The specified file must be a valid CSV.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("The specified file could not be found.", filePath);

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (!CsvUtilities.IsValidDelimiter(delimiter))
                throw new ArgumentException($"Invalid delimiter specified (accepted delimiters are: {CsvUtilities.ValidDelimiters}).", nameof(delimiter));

            CsvParser parser = null;
            List<String[]> fieldsMatrix = new List<String[]>();

            try
            {
                parser = new CsvParser(filePath, delimiter, encoding);

                while (!parser.EndOfData)
                    fieldsMatrix.Add(parser.ReadFields());
            }
            finally
            {
                parser?.Dispose();
            }

            if (skipHeader && (fieldsMatrix.Count > 0))
                fieldsMatrix.Remove(fieldsMatrix.ElementAt(0));

            fieldsMatrix.RemoveAll(x => x == null);

            return fieldsMatrix;
        }
        #endregion
    }
}
