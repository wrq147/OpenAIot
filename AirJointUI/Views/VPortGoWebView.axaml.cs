using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaWebView;
using WebViewCore.Events;

namespace AirJointUI;

public partial class VPortGoWebView : UserControl
{
    public VPortGoWebView()
    {
        InitializeComponent();
        PART_Button.Click += PART_Button_Click;
        PART_WebView.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.None);
        PART_WebView.WebViewNewWindowRequested += PART_WebView_WebViewNewWindowRequested;
    }
    private void PART_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MyVMLocator.Instance.Window.ContentViewModel = MyVMLocator.Instance.IndexViewModel;
    }

    private void PART_WebView_WebViewNewWindowRequested(object? sender, WebViewCore.Events.WebViewNewWindowEventArgs e)
    {
        e.UrlLoadingStrategy = WebViewCore.Enums.UrlRequestStrategy.OpenInWebView;
    }
}