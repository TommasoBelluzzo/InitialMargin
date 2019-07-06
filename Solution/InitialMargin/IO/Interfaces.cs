#region Using Directives
using System;
using InitialMargin.Core;
#endregion

namespace InitialMargin.IO
{
    public interface IOutputWriter
    {
        #region Methods
        void Write(String filePath, MarginTotal margin);
        #endregion
    }
}