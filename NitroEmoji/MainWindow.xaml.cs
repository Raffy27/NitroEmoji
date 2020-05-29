using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using NitroEmoji.Client;

namespace NitroEmoji
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private DiscordClient C = new DiscordClient();

        public MainWindow() {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e) {
            StatusLabel.Content = "Logging in...";
            Task<bool> Login = C.Login(EmailBox.Text, PasswordBox.Password);
            LoginButton.IsEnabled = false;
            bool success = await Login;
            if (!success) {
                StatusLabel.Content = "Login failed";
                LoginButton.IsEnabled = true;
            } else {
                StatusLabel.Content = "Login successful";
                LoginContainer.Visibility = Visibility.Hidden;
            }
        }

        private void ClearDefault(object sender, RoutedEventArgs e) {
            if(sender is PasswordBox) {
                var t = sender as PasswordBox;
                if (t.Password == "Password") {
                    t.Password = "";
                }
            } else {
                var t = sender as TextBox;
                if (t.Text == "Email") {
                    t.Text = "";
                }
            }
        }
    }
}
