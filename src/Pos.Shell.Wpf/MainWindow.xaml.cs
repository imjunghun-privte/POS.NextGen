using System.Windows;

namespace Pos.Shell.Wpf
{
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

            // WPF → Web SPA 메시지 보내기
            WebView.CoreWebView2.WebMessageReceived += (_, args) =>
            {
                string json = args.WebMessageAsJson;
                // Adapter Layer → Web으로 이벤트 전달
            };
        }
    }
}
