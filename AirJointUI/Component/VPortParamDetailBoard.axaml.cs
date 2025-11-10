using AirJointUI.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AirJointUI;

public partial class VPortParamDetailBoard : Window
{
    public VPortParamDetailBoard(ParamObject data)
    {
        InitializeComponent();
        DataContext = data;
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void HandleClick(object sender, RoutedEventArgs e)
    {
        Close(null);
    }
}