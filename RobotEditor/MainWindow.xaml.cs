using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

using RobotEditor.Classes;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Messages;
using RobotEditor.Properties;
using RobotEditor.ViewModel;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;

namespace RobotEditor
{
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
            var layoutDocumentPane =
                DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();
            if (layoutDocumentPane != null && layoutDocumentPane.ChildrenCount == 0)
            {
                var instance = Ioc.Default.GetRequiredService<MainViewModel>();
                instance.AddNewFile();
            }
            ProcessArgs();
        }

        private static void OpenFile(string filename)
        {
            var instance = Ioc.Default.GetRequiredService<MainViewModel>();
            instance.Open(filename);
        }

        private static void LoadOpenFiles()
        {
            var array = Settings.Default.OpenDocuments.Split(new[] {';'});
            for (var i = 0; i < array.Length - 1; i++)
                if (File.Exists(array[i]))
                    OpenFile(array[i]);
        }

        private static void ProcessArgs()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            for (var i = 1; i < commandLineArgs.Length; i++)
                OpenFile(commandLineArgs[i]);
        }

        [Localizable(false)]
        private void DropFiles(object sender, DragEventArgs e)
        {
            var array = (string[]) e.Data.GetData(DataFormats.FileDrop);


            foreach (var msg in array.Select(text => new WindowMessage("File Dropped",  text, MessageType.Information)))
            {
                WeakReferenceMessenger.Default.Send(msg);
            }
        }

        void onDragEnter(object sender, DragEventArgs e)
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
            var delayMilliseconds = (int) delay.TotalMilliseconds;
            if (delayMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException("delay", delay, Properties.Resources.ValueMustBePositive);
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            SafeThreadAsyncCall(delegate
            {
                var t = new Timer
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
            var layoutDocumentPane =
                DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();
            if (layoutDocumentPane != null)
            {
                foreach (var current in
                    from doc in layoutDocumentPane.Children
                    select doc.Content as DocumentViewModel
                    into d
                    where d != null && d.FilePath != null
                    select d)
                {
                    var settings = Settings.Default;

                    settings.OpenDocuments = settings.OpenDocuments + current.FilePath + ';';
                }
            }
            Settings.Default.Save();
            SaveLayout();
            var instance = Ioc.Default.GetRequiredService<MainViewModel>();
            instance.IsClosing = true;
            App.Application?.Shutdown();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadItems();
            Splasher.CloseSplash();
            //  LoadLayout();


            var msg = new WindowMessage("Application Loaded", "Application Loaded", MessageType.Information);
            WeakReferenceMessenger.Default.Send<IMessage>(msg);
        }

        private void SaveLayout()
        {
            var xmlLayoutSerializer = new XmlLayoutSerializer(DockManager);
            using (var streamWriter = new StreamWriter(Global.DockConfig))
            {
                xmlLayoutSerializer.Serialize(streamWriter);
            }
        }


        private void LoadLayout()
        {
            if (File.Exists(Global.DockConfig))
            {
                var xmlLayoutSerializer = new XmlLayoutSerializer(DockManager);
                using (new StreamReader(Global.DockConfig))
                {
                    xmlLayoutSerializer.Deserialize(Global.DockConfig);
                }
            }
        }

        public void CloseWindow(object param)
        {
            var ad = param as IEditorDocument;
            var layoutDocumentPane =
                DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();
            if (layoutDocumentPane != null)
            {
                using (var enumerator = (
                    from c in layoutDocumentPane.Children
                    where c.Content.Equals(ad)
                    select c).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        layoutDocumentPane.Children.Remove(current);
                    }
                }
            }
        }
    }
}