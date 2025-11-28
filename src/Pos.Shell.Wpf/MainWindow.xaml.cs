using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Pos.Shell.Wpf.Messaging;

namespace Pos.Shell.Wpf;

public partial class MainWindow : Window
{
    private PosMessageRouter? _router;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await WebView.EnsureCoreWebView2Async();

        // 메시지 기능 활성화
        WebView.CoreWebView2.Settings.IsWebMessageEnabled = true;
        WebView.CoreWebView2.Settings.IsScriptEnabled = true;

        // alert/confirm/prompt 차단
        await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"
            window.alert = function () { console.log('[alert blocked]'); };
            window.confirm = function () { console.log('[confirm blocked]'); return false; };
            window.prompt = function () { console.log('[prompt blocked]'); return null; };
        ");

        // 라우터 초기화
        _router = new PosMessageRouter(WebView);
        RegisterHandlers(_router);

        // 메시지 수신
        WebView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

        // SPA 로드
        var appPath = Directory.GetCurrentDirectory();
        var spaPath = Path.Combine(appPath, "wwwroot", "index.html");

        if (!File.Exists(spaPath))
        {
            MessageBox.Show($"SPA 파일을 찾을 수 없습니다:\n{spaPath}");
            return;
        }

        WebView.CoreWebView2.Navigate(spaPath);
    }

    // JS -> WPF 메시지
    private async void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        if (_router is null)
            return;

        var json = e.WebMessageAsJson;
        Debug.WriteLine($"[Shell] WebMessageReceived: {json}");
        await _router.HandleIncomingAsync(json);
    }

    // 핸들러 등록
    private void RegisterHandlers(PosMessageRouter router)
    {
        router.RegisterHandler<PingPayload>("ping", async (env, payload) =>
        {
            Debug.WriteLine($"[Shell] ping from SPA: {payload?.From}, at {payload?.SentAt}");

            await router.SendAsync(
                type: "pong",
                payload: new { message = "PONG from WPF", serverTime = DateTimeOffset.UtcNow },
                isResponse: true,
                correlationId: env.CorrelationId
            );
        });
    }


    /* -------------------------------------------------------------
     *   DEBUG 전용 ESC / ALT+F4 차단 
     *   (요청하신 대로 "디버그 모드에서만 적용되는 버전")
     * ------------------------------------------------------------- */

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
#if DEBUG

#else
        // ESC 차단
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            return;
        }

        // Alt+F4 차단
        if (e.SystemKey == Key.F4)
        {
            e.Handled = true;
            return;
        }
#endif

        base.OnPreviewKeyDown(e);
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
#if DEBUG
        
#else
        // RELEASE에서는 절대 닫히지 않게 유지 (키오스크 모드)
        e.Cancel = true;
        return;
#endif
    }
}

// ping payload 예시
public sealed class PingPayload
{
    public string? From { get; set; }
    public DateTimeOffset? SentAt { get; set; }
}
