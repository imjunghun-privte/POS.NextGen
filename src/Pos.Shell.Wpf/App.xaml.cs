using System.Windows;

namespace Pos.Shell.Wpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Removed WebView2Loader.EnsureRuntime() because the helper is not present in this project.
            new MainWindow().Show();
        }
    }
}
