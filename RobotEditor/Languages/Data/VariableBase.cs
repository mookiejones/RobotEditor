using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Messages;
using RobotEditor.ViewModel;
using RobotEditor.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace RobotEditor.Languages.Data
{
    public class VariableBase : ObservableRecipient, IVariable
    {
        private string _comment;
        private string _declaration;
        private string _description = string.Empty;
        private BitmapImage _icon;
        private string _name;
        private int _offset;
        private string _path;
        private string _type;
        private string _value;
        public static List<IVariable> Variables { get; private set; }
        public bool IsSelected { get; set; }

        public string Description { get => _description; set => SetProperty(ref _description, value); }

        public BitmapImage Icon { get => _icon; set => SetProperty(ref _icon, value); }

        public string Name { get => _name; set => SetProperty(ref _name, value); }

        public string Comment { get => _comment; set => SetProperty(ref _comment, value); }

        public string Path { get => _path; set => SetProperty(ref _path, value); }

        public string Value { get => _value; set => SetProperty(ref _value, value); }

        public string Type { get => _type; set => SetProperty(ref _type, value); }

        public string Declaration { get => _declaration; set => SetProperty(ref _declaration, value); }

        public int Offset { get => _offset; set => SetProperty(ref _offset, value); }

        public static void GetPositions(string filename, AbstractLanguageClass lang, string iconpath)
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundworkerDoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundworkerRunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(new WorkerArgs
            {
                Filename = filename,
                Lang = lang,
                IconPath = iconpath
            });
        }

        private static void BackgroundworkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private static void BackgroundworkerDoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is WorkerArgs workerArgs)
            {
                var bitmapImage = ImageHelper.LoadBitmap(workerArgs.IconPath);
                var instance = Ioc.Default.GetRequiredService<MainViewModel>();
                var fileLanguage = instance.ActiveEditor.FileLanguage;
                var match = VariableHelper.FindMatches(workerArgs.Lang.XYZRegex, workerArgs.Filename);
                var fileNameWithoutExtension =
                    System.IO.Path.GetFileNameWithoutExtension(bitmapImage.UriSource.AbsolutePath);
                var flag = fileNameWithoutExtension != null && fileNameWithoutExtension.Contains("XYZ");
                while (match.Success)
                {
                    var item = new Variable
                    {
                        Icon = bitmapImage,
                        Path = workerArgs.Filename,
                        Offset = match.Index,
                        Type = match.Groups[1].ToString(),
                        Name = match.Groups[2].ToString(),
                        Value = flag ? fileLanguage.ExtractXYZ(match.ToString()) : match.Groups[3].ToString(),
                        Comment = match.Groups[4].ToString()
                    };
                    Variables.Add(item);
                    match = match.NextMatch();
                }
            }
        }

        public static List<IVariable> GetVariables(string filename, Regex regex, string iconpath)
        {
            var list = new List<IVariable>();
            var bitmapImage = ImageHelper.LoadBitmap(iconpath);
            var instance = Ioc.Default.GetRequiredService<MainViewModel>();
            var fileLanguage = instance.ActiveEditor.FileLanguage;
            var match = VariableHelper.FindMatches(regex, filename);
            var fileNameWithoutExtension =
                System.IO.Path.GetFileNameWithoutExtension(bitmapImage.UriSource.AbsolutePath);
            var flag = fileNameWithoutExtension != null && fileNameWithoutExtension.Contains("XYZ");
            List<IVariable> result;
            if (match == null)
            {
                var msg = new ErrorMessage("Variable for " + fileLanguage.RobotType,
                    "Does not exist in VariableBase.GetVariables", MessageType.Error);
                WeakReferenceMessenger.Default.Send<IMessage>(msg);
                result = null;
            }
            else
            {
                while (match.Success)
                {
                    var item = new Variable
                    {
                        Icon = bitmapImage,
                        Path = filename,
                        Offset = match.Index,
                        Type = match.Groups[1].ToString(),
                        Name = match.Groups[2].ToString(),
                        Value = flag ? fileLanguage.ExtractXYZ(match.ToString()) : match.Groups[3].ToString(),
                        Comment = match.Groups[4].ToString()
                    };
                    list.Add(item);
                    match = match.NextMatch();
                }
                result = list;
            }
            return result;
        }

        internal class WorkerArgs
        {
            public string Filename { get; set; }
            public AbstractLanguageClass Lang { get; set; }
            public string IconPath { get; set; }
        }
    }
}