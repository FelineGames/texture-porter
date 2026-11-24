using System;
using System.IO;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using TexterRename.Core;
using Windows.Graphics;

namespace TexterRename;

public partial class App : Application
{
    public static Window? MainWindow { get; private set; }

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new Window
        {
            Title = "Port Generic Textures",
            Content = new MainPage()
        };

        ConfigureWindow(MainWindow);
        MainWindow.Activate();
    }

    private static void ConfigureWindow(Window window)
    {
        AppWindow appWindow = window.AppWindow;
        appWindow.Title = "Port Generic Textures";
        appWindow.Resize(new SizeInt32(AppConfig.WindowWidth, AppConfig.WindowHeight));

        // Sets the taskbar & Alt+Tab icon from your root app_icon.png
        string iconPath = Path.Combine(AppContext.BaseDirectory, "app_icon.png");
        if (File.Exists(iconPath))
        {
            appWindow.SetIcon(iconPath);
        }

        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            presenter.IsResizable = false;
        }

        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            window.ExtendsContentIntoTitleBar = true;
            AppWindowTitleBar titleBar = appWindow.TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = ColorHelper.FromArgb(255, 225, 228, 232);
            titleBar.ButtonHoverBackgroundColor = ColorHelper.FromArgb(255, 42, 46, 54);
            titleBar.ButtonHoverForegroundColor = Colors.White;
        }
    }
}
