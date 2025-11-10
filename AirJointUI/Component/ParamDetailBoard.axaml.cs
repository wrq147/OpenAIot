using AirJointUI.Models;
using AirJointUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AirJointUI;

public partial class ParamDetailBoard : Window
{
    public ParamDetailBoard(ParamObject data)
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