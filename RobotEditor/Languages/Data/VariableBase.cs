using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Messages;
using RobotEditor.Utilities;
using RobotEditor.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace RobotEditor.Languages.Data;

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
        BackgroundWorker backgroundWorker = new();
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
            BitmapImage bitmapImage = ImageHelper.LoadBitmap(workerArgs.IconPath);
            MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
            AbstractLanguageClass fileLanguage = instance.ActiveEditor.FileLanguage;
            Match match = VariableHelper.FindMatches(workerArgs.Lang.XYZRegex, workerArgs.Filename);
            string fileNameWithoutExtension =
                System.IO.Path.GetFileNameWithoutExtension(bitmapImage.UriSource.AbsolutePath);
            bool flag = fileNameWithoutExtension != null && fileNameWithoutExtension.Contains("XYZ");
            while (match.Success)
            {
                Variable item = new()
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
        List<IVariable> list = new();
        BitmapImage bitmapImage = ImageHelper.LoadBitmap(iconpath);
        MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
        AbstractLanguageClass fileLanguage = instance.ActiveEditor.FileLanguage;
        Match match = VariableHelper.FindMatches(regex, filename);
        string fileNameWithoutExtension =
            System.IO.Path.GetFileNameWithoutExtension(bitmapImage.UriSource.AbsolutePath);
        bool flag = fileNameWithoutExtension != null && fileNameWithoutExtension.Contains("XYZ");
        List<IVariable> result;
        if (match == null)
        {
            ErrorMessage msg = new("Variable for " + fileLanguage.RobotType,
                "Does not exist in VariableBase.GetVariables", MessageType.Error);
            _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            result = null;
        }
        else
        {
            while (match.Success)
            {
                Variable item = new()
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