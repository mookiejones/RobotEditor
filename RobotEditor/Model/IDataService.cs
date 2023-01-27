using System;

namespace RobotEditor.Model
{
    public interface IDataService
    {
        void GetData(Action<DataItem, Exception> callback);
    }
}