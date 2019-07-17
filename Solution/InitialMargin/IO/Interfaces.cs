#region Using Directives
using System;
using System.Text;
using InitialMargin.Core;
#endregion

namespace InitialMargin.IO
{
    /// <summary>Defines the contract of a data manager.</summary>
    public interface IDataManager
    {
        #region Properties
        /// <summary>Gets the encoding used by the data manager.</summary>
        /// <value>An <see cref="T:System.Text.Encoding"/> object.</value>
        Encoding Encoding { get; }

        /// <summary>Gets the culture-specific format used by the data manager.</summary>
        /// <value>An <see cref="T:System.IFormatProvider"/> object.</value>
        IFormatProvider FormatProvider { get; }
        #endregion
    }

    /// <summary>Defines the contract of an output writer.</summary>
    public interface IOutputWriter : IDataManager
    {
        #region Methods
        /// <summary>Writes the specified calculation result into the specified file.</summary>
        /// <param name="filePath">The <see cref="T:System.String"/> representing the file to write to.</param>
        /// <param name="margin">The <see cref="T:InitialMargin.Core.MarginTotal"/> object to write.</param>
        void Write(String filePath, MarginTotal margin);
        #endregion
    }
}