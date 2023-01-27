using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using RobotEditor.Interfaces;

namespace RobotEditor.Languages.Data
{
    public class VariableHelper
    {
        public static ICollectionView PositionView { get; set; }
        public static ICollectionView PositionCollection { get; set; }

        public static Match FindMatches(Regex matchstring, string filename)
        {
            var text = File.ReadAllText(filename);
            Match result;
            if (string.IsNullOrEmpty(matchstring.ToString()))
            {
                result = null;
            }
            else
            {
                var match = matchstring.Match(text.ToLower());
                result = match;
            }
            return result;
        }

        public class VariableBase : ObservableObject, IVariable
        {
            private string _comment = string.Empty;
            private string _declaration = string.Empty;
            private string _description = string.Empty;
            private BitmapImage _icon;
            private string _name = string.Empty;
            private int _offset = -1;
            private string _path = string.Empty;
            private string _type = string.Empty;
            private string _value = string.Empty;
            public bool IsSelected { get; set; }

            public string Description { get => _description; set => SetProperty(ref _description, value); }

            public BitmapImage Icon { get => _icon; set => SetProperty(ref _icon, value); }

            public string Name { get => _name; set => SetProperty(ref _name, value); }

            public string Type { get => _type; set => SetProperty(ref _type, value); }

            public string Path { get => _path; set => SetProperty(ref _path, value); }

            public string Value { get => _value; set => SetProperty(ref _value, value); }

            public int Offset { get => _offset; set => SetProperty(ref _offset, value); }

            public string Comment { get => _comment; set => SetProperty(ref _comment, value); }

            public string Declaration { get => _declaration; set => SetProperty(ref _declaration, value); }
        }
    }
}