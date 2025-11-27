using Microsoft.Web.WebView2.Core;

using System.IO;
using System.Windows;

namespace Pos.Shell.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await WebView.EnsureCoreWebView2Async();

        var appPath = Directory.GetCurrentDirectory();
        var spaPath = Path.Combine(appPath, "wwwroot", "index.html");

        if (!File.Exists(spaPath))
        {
            MessageBox.Show($"SPA 파일을 찾을 수 없습니다:\n{spaPath}");
            return;
        }

        WebView.CoreWebView2.Navigate(spaPath);

        WebView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
    }

    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var json = e.WebMessageAsJson;
        MessageBox.Show($"JS → WPF 메시지 수신: {json}");
    }

    public void SendToSpa(object data)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        WebView.CoreWebView2.PostWebMessageAsJson(json);
    }
}
