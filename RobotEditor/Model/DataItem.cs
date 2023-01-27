namespace RobotEditor.Model
{
    public sealed class DataItem
    {
        public DataItem(string title)
        {
            Title = title;
        }

        private string Title { get; set; }
    }
}