using System.IO;
using System.Windows;

namespace NitroEmoji
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            Directory.CreateDirectory("cache");
        }
    }
}
