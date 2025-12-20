
using AirJointUI.Utils;
using AirJointUI.Views;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
namespace AirJointUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        Console.WriteLine($"全局异常：{ex}");
    }

    int defX = 6;
    public override void OnFrameworkInitializationCompleted()
    {
        //初始化语言
        string showlan = "zh";
        if (File.Exists("lang"))
        {
            showlan = File.ReadAllText("lang");
        }

        string bkt = "0";
        if (File.Exists("bktime"))
        {
            bkt = File.ReadAllText("bktime");
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            //修改息屏时间
            int secc = Convert.ToInt32(bkt) * 60;
            var mmidle = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"xset s " + secc + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            mmidle.Start();
            mmidle.StandardOutput.ReadToEnd();
            mmidle.WaitForExit();
        }

        //设置横竖屏
        string dirval = "Horiz";
        if (File.Exists("dirf"))
        {
            dirval = File.ReadAllText("dirf");
        }
        MyVMLocator.Instance.IsPortraitDisplay = dirval != "Horiz";

        string ffconfig;
        if (File.Exists("conf"))
        {
            ffconfig = File.ReadAllText("conf");
            string[] tarr = ffconfig.Split(",");
            if (tarr.Length > 0)
            {
                if (tarr[0] != "")
                {
                    defX = Convert.ToInt32(tarr[0]);
                }
            }
            if (tarr.Length > 1)
            {
                if (tarr[1] != "")
                {
                    MyVMLocator.Instance.WifiName = tarr[1];
                }
            }
        }

        I18NExt.Culture = new CultureInfo(showlan);
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            desktop.MainWindow = new MainWindow
            {
                DataContext = MyVMLocator.Instance.Window,
            };
            // 设置全屏模式
            desktop.MainWindow.WindowState = WindowState.FullScreen;
            desktop.MainWindow.ExtendClientAreaToDecorationsHint = true;
            desktop.MainWindow.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
            desktop.MainWindow.ExtendClientAreaTitleBarHeightHint = -1;
            if (MyVMLocator.Instance.IsPortraitDisplay)
            {
                desktop.MainWindow.Width = 1080;
                desktop.MainWindow.Height = 1920;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    //修改触摸旋转
                    var appxx = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = "-c \"xrandr -o left\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };

                    appxx.Start();
                    appxx.StandardOutput.ReadToEnd();
                    appxx.WaitForExit();


                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = "-c \"xinput set-prop " + defX + " 'Coordinate Transformation Matrix' 0 -1 1 1 0 0 0 0 1\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                }

            }
            else
            {
                desktop.MainWindow.Width = 1920;
                desktop.MainWindow.Height = 1080;
            }
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainWindow
            {
                DataContext = MyVMLocator.Instance.Window
            };
            if (MyVMLocator.Instance.IsPortraitDisplay)
            {
                singleViewPlatform.MainView.Width = 1080;
                singleViewPlatform.MainView.Height = 1920;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    //修改触摸旋转
                    var appxx = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = "-c \"xrandr -o left\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };

                    appxx.Start();
                    appxx.StandardOutput.ReadToEnd();
                    appxx.WaitForExit();


                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = "-c \"xinput set-prop " + defX + " 'Coordinate Transformation Matrix' 0 -1 1 1 0 0 0 0 1\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                }

            }
            else
            {
                singleViewPlatform.MainView.Width = 1920;
                singleViewPlatform.MainView.Height = 1080;
            }
        }

        //启用加载页面
        MyVMLocator.Instance.Window.ContentViewModel = MyVMLocator.Instance.LoadViewModel;
        Task.Run(async () =>
        {
            await MyVMLocator.Instance.LoadViewModel.UpdateProgress();
        });
        base.OnFrameworkInitializationCompleted();

    }
}
