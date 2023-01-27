using RobotEditor.Languages.Data;
using System.Collections.Generic;

namespace RobotEditor.Interfaces
{
    public interface IPosition
    {
        string RawValue { get; set; }
        string Scope { get; set; }
        string Name { get; set; }
        string Type { get; set; }
        IEnumerable<PositionValue> PositionalValues { get; }
        void ParseValues();
        string ExtractFromMatch();
    }
}