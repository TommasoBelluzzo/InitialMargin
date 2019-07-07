#region Using Directives
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace InitialMargin.Tests
{
    public static class Utilities
    {
        #region Members
        private static readonly Regex s_RegexExtension = new Regex(@"^\.[A-Z0-9]+$", (RegexOptions.Compiled | RegexOptions.IgnoreCase));
        #endregion

        #region Methods
        private static String GetDataPath()
        {
            Uri codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            String codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            String dataPath = Path.Combine(Path.GetDirectoryName(codeBasePath), "Data");

            return dataPath;
        }

        public static Decimal Round(Decimal value, Int32 digits)
        {
            return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }

        public static Decimal Round(Double value, Int32 digits)
        {
            return Round(Convert.ToDecimal(value), digits);
        }

        public static String ComputeHash(String input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("Invalid input specified.", nameof(input));

            using (SHA1Managed sha1 = new SHA1Managed())
            {
                Byte[] bytes = Encoding.UTF8.GetBytes(input);
                Byte[] hash = sha1.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder(hash.Length * 2);

                foreach (Byte b in hash)
                    builder.Append(b.ToString("X2"));

                return builder.ToString();
            }
        }

        public static String GetRandomFilePath(String fileExtension)
        {
            if (String.IsNullOrWhiteSpace(fileExtension) || !s_RegexExtension.IsMatch(fileExtension))
                throw new ArgumentException("Invalid file extension specified.", nameof(fileExtension));

            fileExtension = fileExtension.ToLowerInvariant();

            String fileName;

            do
            {
                String guid = Guid.NewGuid().ToString().Replace("-", String.Empty).ToLowerInvariant();
                fileName = Path.Combine(GetDataPath(), String.Concat(guid, fileExtension));
            }
            while (File.Exists(fileName));

            return fileName;
        }

        public static String GetStaticFilePath(String fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Invalid file name specified.", nameof(fileName));

            String filePath = Path.Combine(GetDataPath(), fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("The specified file could not be found.", filePath);

            return filePath;
        }
        #endregion
    }
}