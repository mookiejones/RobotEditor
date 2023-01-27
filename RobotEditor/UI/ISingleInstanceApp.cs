using System.Collections.Generic;
using System.Threading.Tasks;

namespace RobotEditor.UI
{
    public interface ISingleInstanceApp
    {
        bool SignalExternalCommandLineArgs(IEnumerable<string> args);
    }
}
