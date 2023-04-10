using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using RobotEditor.Controls;
using RobotEditor.Enums;
using RobotEditor.Messages;
using RobotEditor.UI;
using RobotEditor.ViewModel;
using RobotEditor.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace RobotEditor;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string Unique = "RobotEditor";
    public static App Application;

    //static App()
    //{
    //    DispatcherHelper.Initialize();
    //}

    public bool SignalExternalCommandLineArgs(IEnumerable<string> args)
    {
        _ = MainWindow.Activate();
        MainViewModel main = Ioc.Default.GetRequiredService<MainViewModel>();
        main.LoadFile(args);
        return true;
    }

    [Localizable(false)]
    private static bool CheckEnvironment()
    {
        // Safety check: our setup already checks that .NET 4 is installed, but we manually check the .NET version in case SharpDevelop is
        // used on another machine than it was installed on (e.g. "SharpDevelop on USB stick")
        if (Environment.Version < new Version(4, 0, 30319))
        {
            _ = MessageBox.Show(string.Format("This version of {0} requires .Net 4.0. Your are using:{1}",
                Assembly.GetExecutingAssembly().GetName().Name, Environment.Version));
            return false;
        }
        // Work around a WPF issue when %WINDIR% is set to an incorrect path
        string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows,
            Environment.SpecialFolderOption.DoNotVerify);
        if (Environment.GetEnvironmentVariable("WINDIR") != windir)
        {
            Environment.SetEnvironmentVariable("WINDIR", windir);
        }
        return true;
    }

    private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        ErrorMessage msg = new("App", e.Exception, MessageType.Error);
        _ = WeakReferenceMessenger.Default.Send(msg);

        Console.Write(e);
        e.Handled = true;
    }

    protected override void OnExit(ExitEventArgs e) => base.OnExit(e);

    [Localizable(false)]
    protected override void OnStartup(StartupEventArgs e)
    {
        Splasher.Splash = new SplashScreenWindow();
        Splasher.ShowSplash();


#if DEBUG
//            Control.CheckForIllegalCrossThreadCalls = true;
#endif
        if (!CheckEnvironment())
        {
            return;
        }

      
        //    Application = new App();

        //    Application.InitializeComponent();
        //   Application.Run();


        //  var _tools = Workspace.Instance.Tools;
        //  foreach (var tool in _tools)
        //  {
        //      if (tool is RobotEditor.GUI.FindReplaceViewModel)
        //      {
        //          var obj = tool as RobotEditor.GUI.FindReplaceViewModel;
        //          System.Xml.Serialization.XmlSerializer serial = new System.Xml.Serialization.XmlSerializer(typeof(RobotEditor.GUI.Results));
        //          System.IO.TextWriter writer = new System.IO.StreamWriter("D:\\results.xml");
        //          serial.Serialize(writer,obj.FindReplaceResults);
        //      }
        //  }
        // Allow single instance code to perform cleanup operations
    
        if (e.Args.Length > 0)
        {
            foreach (string v in e.Args)
            {
            }
            Debugger.Break();
            MessageBox.Show(e.ToString());
            MessageBox.Show("You have the latest version.");
            Shutdown();
        }


        var location = Assembly.GetExecutingAssembly()?.Location;
        JumpTask task = new()
        {
            Title = "Check for Updates",
            Arguments = "/update",
            Description = "Checks for Software Updates",
            CustomCategory = "Actions",
            IconResourcePath =location,
            ApplicationPath = location
        };

        
        Assembly asm = Assembly.GetExecutingAssembly();

        JumpTask version = new()
        {
            CustomCategory = "Version",
            Title = asm?.GetName()?.Version?.ToString()??"",
            IconResourcePath = asm?.Location,
            IconResourceIndex = 0
        };

        JumpList jumpList = new();
        jumpList.JumpItems.Add(version);
        jumpList.ShowFrequentCategory = true;
        jumpList.ShowRecentCategory = true;
        JumpList.SetJumpList(Current, jumpList);
        jumpList.Apply();

        base.OnStartup(e);
    }
}