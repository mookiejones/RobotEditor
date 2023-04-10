using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using RobotEditor.Controls;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Messages;
using RobotEditor.Properties;
using RobotEditor.ViewModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;

namespace RobotEditor;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public sealed partial class MainWindow
{
    public static MainWindow Instance { get; set; }

    #region Constructor

    /// <summary>
    ///     Initializes a new instance of the MainWindow class.
    /// </summary>
    public MainWindow()
    {
        Instance = this;
        InitializeComponent();
        //   ThemeManager.Current.ChangeTheme(Application.Current, "Light");
        KeyDown += (s, e) => StatusBarViewModel.Instance.ManageKeys(s, e);
    }

    #endregion

    private void LoadItems()
    {
        LoadOpenFiles();
        var layoutDocumentPane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
        if (layoutDocumentPane != null && layoutDocumentPane.ChildrenCount == 0)
        {
            MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
            instance.AddNewFile();
        }
        ProcessArgs();
    }

    private static void OpenFile(string filename)
    {
        MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
        _ = instance.Open(filename);
    }

    private static void LoadOpenFiles()
    {
        string[] array = Settings.Default.OpenDocuments.Split(new[] { ';' });
        for (int i = 0; i < array.Length - 1; i++)
        {
            if (File.Exists(array[i]))
            {
                OpenFile(array[i]);
            }
        }
    }

    private static void ProcessArgs()
    {
        string[] commandLineArgs = Environment.GetCommandLineArgs();
        for (int i = 1; i < commandLineArgs.Length; i++)
        {
            OpenFile(commandLineArgs[i]);
        }
    }

    [Localizable(false)]
    private void DropFiles(object sender, DragEventArgs e)
    {
        string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);


        foreach (WindowMessage msg in array.Select(text => new WindowMessage("File Dropped", text, MessageType.Information)))
        {
            _ = WeakReferenceMessenger.Default.Send(msg);
        }
    }

    private void onDragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
    }

    public static void SafeThreadAsyncCall(Action method)
    {
    }

    public static void CallLater(TimeSpan delay, Action method)
    {
        int delayMilliseconds = (int)delay.TotalMilliseconds;
        if (delayMilliseconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), delay, "ValueMustBePositive");
        }
        if (method == null)
        {
            throw new ArgumentNullException(nameof(method));
        }
        SafeThreadAsyncCall(delegate
        {
            Timer t = new()
            {
                Interval = Math.Max(1, delayMilliseconds)
            };
            t.Tick += delegate
            {
                t.Stop();
                t.Dispose();
                method();
            };
            t.Start();
        });
    }

    private void WindowClosing(object sender, CancelEventArgs e)
    {
        Settings.Default.OpenDocuments = string.Empty;
        var layoutDocumentPane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
        if (layoutDocumentPane != null)
        {
            foreach (DocumentViewModel current in
                from doc in layoutDocumentPane.Children
                select doc.Content as DocumentViewModel
                into d
                where d != null && d.FilePath != null
                select d)
            {
                Settings settings = Settings.Default;

                settings.OpenDocuments = settings.OpenDocuments + current.FilePath + ';';
            }
        }
        Settings.Default.Save();
        SaveLayout();
        MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
        instance.IsClosing = true;
        App.Application?.Shutdown();
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
        LoadItems();
        Splasher.CloseSplash();
        //  LoadLayout();


        WindowMessage msg = new("Application Loaded", "Application Loaded", MessageType.Information);
        _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
    }

    private void SaveLayout()
    {
        XmlLayoutSerializer xmlLayoutSerializer = new(DockManager);
        using StreamWriter streamWriter = new(Global.DockConfig);
        xmlLayoutSerializer.Serialize(streamWriter);
    }

    public void CloseWindow(object param)
    {

        if (param is not IEditorDocument ad)
            return; ;

        var layoutDocumentPane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
        if (layoutDocumentPane != null)
        {
            using System.Collections.Generic.IEnumerator<LayoutContent> enumerator = (
                from c in layoutDocumentPane.Children
                where c.Content.Equals(ad)
                select c).GetEnumerator();
            if (enumerator.MoveNext())
            {
                LayoutContent current = enumerator.Current;
                _ = layoutDocumentPane.Children.Remove(current);
            }
        }
    }
}