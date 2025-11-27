using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Windows;
using System.Diagnostics;
using Pos.Shell.Wpf.Messaging;

namespace Pos.Shell.Wpf;

public partial class MainWindow : Window
{
    private PosMessageRouter? _router;

    public sealed class PingPayload
    {
        public string? From { get; set; }
        public DateTimeOffset? SentAt { get; set; }
    }


    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await WebView.EnsureCoreWebView2Async();

        // ★ 반드시 추가해야 함
        WebView.CoreWebView2.Settings.IsWebMessageEnabled = true;

        // 라우터 초기화
        _router = new PosMessageRouter(WebView);
        RegisterHandlers(_router);

        WebView.CoreWebView2.Settings.IsScriptEnabled = true;

        // ★ alert, confirm, prompt 비활성화
        WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"
            window.alert = function() { console.log('[blocked] alert called'); };
            window.confirm = function() { console.log('[blocked] confirm called'); return false; };
            window.prompt = function() { console.log('[blocked] prompt called'); return null; };
        ");

        var appPath = Directory.GetCurrentDirectory();
        var spaPath = Path.Combine(appPath, "wwwroot", "index.html");

        if (!File.Exists(spaPath))
        {
            MessageBox.Show($"SPA 파일을 찾을 수 없습니다:\n{spaPath}");
            return;
        }

        // ★ 메시지 수신 이벤트는 초기화 후에 등록해야 함
        WebView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

        WebView.CoreWebView2.Navigate(spaPath);
    }

    private async void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        if (_router is null)
            return;

        var json = e.WebMessageAsJson;
        Debug.WriteLine($"[Shell] WebMessageReceived: {json}");

        await _router.HandleIncomingAsync(json);
    }

    private void RegisterHandlers(PosMessageRouter router)
    {
        router.RegisterHandler<PingPayload>("ping", async (envelope, payload) =>
        {
            Debug.WriteLine($"[Shell] ping from SPA: {payload?.From}, at {payload?.SentAt}");

            await router.SendAsync(
                type: "pong",
                payload: new
                {
                    message = "PONG from WPF",
                    serverTime = DateTimeOffset.UtcNow
                },
                isResponse: true,
                correlationId: envelope.CorrelationId
            );
        });
    }
}
