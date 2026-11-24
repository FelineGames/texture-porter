using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TexterRename.Controls;

public sealed partial class CustomTitleBar : UserControl
{
    public UIElement TitleBarElement => DragRegion;

    public CustomTitleBar()
    {
        this.InitializeComponent();
    }
}
