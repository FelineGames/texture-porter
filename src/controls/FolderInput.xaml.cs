using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

namespace TexterRename.Controls;

public sealed partial class FolderInputRow : UserControl
{
    public event RoutedEventHandler? BrowseClicked;

    public FolderInputRow()
    {
        this.InitializeComponent();
    }

    public string Label
    {
        get => RowLabel.Text;
        set => RowLabel.Text = value;
    }

    public string IconData
    {
        set => RowIcon.Data = (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), value);
    }

    public string FolderPath
    {
        get => PathBox.Text;
        set => PathBox.Text = value;
    }

    public bool IsBrowseEnabled
    {
        get => BrowseButton.IsEnabled;
        set => BrowseButton.IsEnabled = value;
    }

    public void FocusInput() => PathBox.Focus(FocusState.Programmatic);

    private void OnBrowseButtonClick(object sender, RoutedEventArgs e)
    {
        BrowseClicked?.Invoke(this, e);
    }
}
