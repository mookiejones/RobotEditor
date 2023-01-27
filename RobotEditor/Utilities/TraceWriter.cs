using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace RobotEditor.Classes
{
    public static class TraceWriter
    {
        [Localizable(false), DebuggerStepThrough]
        public static void Trace(string message) => System.Diagnostics.Trace.WriteLine(string.Format("{0} : {1}",
                DateTime.Now.ToString(CultureInfo.InvariantCulture), message));
    }
}