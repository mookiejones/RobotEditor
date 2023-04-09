using System;

namespace RobotEditor.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            DataItem item = new("Welcome to MVVM Light");
            callback(item, null);
        }
    }
}