using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TexterRename.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace TexterRename;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) =>
        {
            App.MainWindow?.SetTitleBar(TopTitleBar.TitleBarElement);
            SourceRow.FocusInput();
        };
    }

    private void OnModeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MappingRow is null) return;

        string selectedMode = (ModeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Default";
        bool isCustom = selectedMode == "Custom";
        MappingRow.IsBrowseEnabled = isCustom;

        if (!isCustom)
        {
            MappingRow.FolderPath = string.Empty;
        }
    }

    private async void OnBrowseSourceClick(object sender, RoutedEventArgs e)
    {
        string? folder = await PickFolderAsync();
        if (!string.IsNullOrEmpty(folder))
        {
            SourceRow.FolderPath = folder;
            SourceRow.FocusInput();
        }
    }

    private async void OnBrowseOutputClick(object sender, RoutedEventArgs e)
    {
        string? folder = await PickFolderAsync();
        if (!string.IsNullOrEmpty(folder))
        {
            OutputRow.FolderPath = folder;
            OutputRow.FocusInput();
        }
    }

    private async void OnBrowseMappingClick(object sender, RoutedEventArgs e)
    {
        string? folder = await PickFolderAsync();
        if (string.IsNullOrEmpty(folder)) return;

        if (File.Exists(Path.Combine(folder, AppConfig.MappingFilename)))
        {
            MappingRow.FolderPath = folder;
            MappingRow.FocusInput();
        }
        else
        {
            await ShowMessageAsync("Error", $"{AppConfig.MappingFilename} not found in this folder.");
        }
    }

    private void OnClearAllClick(object sender, RoutedEventArgs e)
    {
        ModeComboBox.SelectedIndex = 0;
        SourceRow.FolderPath = string.Empty;
        OutputRow.FolderPath = string.Empty;
        MappingRow.FolderPath = string.Empty;
    }

    private async void OnStartProcessClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SourceRow.FolderPath))
        {
            await ShowMessageAsync("Error", "Source folder is required");
            return;
        }

        if (string.IsNullOrWhiteSpace(OutputRow.FolderPath))
        {
            await ShowMessageAsync("Error", "Output folder is required");
            return;
        }

        string selectedMode = (ModeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Default";
        var rules = selectedMode == "Custom"
            ? TextureMapper.LoadMapping(MappingRow.FolderPath)
            : TextureMapper.LoadDefaultMapping();

        if (rules is null || rules.Count == 0)
        {
            await ShowMessageAsync("Error", $"{AppConfig.MappingFilename} could not be loaded");
            return;
        }

        int count = TextureMapper.ProcessFiles(SourceRow.FolderPath, OutputRow.FolderPath, rules);
        if (count == 0)
        {
            await ShowMessageAsync("Finished", $"No files matched {AppConfig.MappingFilename}");
        }
        else
        {
            await ShowMessageAsync("Success", $"{count} files processed successfully.");
        }
    }

    private static async Task<string?> PickFolderAsync()
    {
        var picker = new FolderPicker();
        picker.FileTypeFilter.Add("*");

        if (App.MainWindow is not null)
        {
            nint hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(picker, hwnd);
        }

        StorageFolder? folder = await picker.PickSingleFolderAsync();
        return folder?.Path;
    }

    private async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot,
            RequestedTheme = ElementTheme.Dark
        };
        await dialog.ShowAsync();
    }
}
