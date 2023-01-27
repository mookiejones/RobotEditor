using System.Collections.Generic;

namespace RobotEditor.UI
{
    public interface ISingleInstanceApp
    {
        bool SignalExternalCommandLineArgs(IEnumerable<string> args);
    }
}
