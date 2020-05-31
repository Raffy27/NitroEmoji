using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using WpfAnimatedGif;

namespace NitroEmoji
{

    public class GuildDisplay {
        public string Title { get; set; }
        public ObservableCollection<Image> Emojis { get; set; }
        public bool IsExpanded { get; set; }

        public GuildDisplay(PartialGuild p) {
            this.Title = p.name;
            Emojis = new ObservableCollection<Image>();
        }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private DiscordClient C = new DiscordClient("cache");
        private ObservableCollection<GuildDisplay> Servers = new ObservableCollection<GuildDisplay>();

        public MainWindow() {
            InitializeComponent();
            EmojiList.ItemsSource = Servers;
        }

        private void EmojiClicked(object sender, MouseEventArgs e) {
            var img = sender as Image;
            StatusLabel.Content = img.ToolTip;
        }

        private void EmojiDragged(object sender, MouseEventArgs e) {
            var img = sender as Image;
            DragDrop.DoDragDrop(sender as DependencyObject,
                new DataObject(DataFormats.FileDrop, new string[1] { img.Tag.ToString() }),
                DragDropEffects.All);
        } 

        private async void DownloadEmojis() {
            foreach(PartialGuild g in C.Guilds) {
                var disp = new GuildDisplay(g);
                Servers.Add(disp);
                disp.IsExpanded = g.emojis.Count > 0;
                foreach(PartialEmoji e in g.emojis) {
                    var img = new Image() {
                        Width = 48,
                        Height = 48,
                        ToolTip = ':' + e.name + ':',
                        Tag = C.FromCache(e)
                    };
                    disp.Emojis.Add(img);
                    var data = await C.EmojiFromCache(e);
                    if (e.animated) {
                        ImageBehavior.SetAnimatedSource(img, data);
                    } else {
                        img.Source = data;
                    }                    
                    img.MouseDown += EmojiClicked;
                    img.MouseMove += EmojiDragged;
                }
            }
        }

        private async void LoadEmojis() {
            StatusLabel.Content = "Loading servers...";
            Progress.IsActive = true;
            Task<bool> GuildTask = C.GetGuilds();
            bool success = await GuildTask;
            if (!success) {
                Progress.IsActive = false;
                StatusLabel.Content = "Failed to load servers";
                return;
            }

            StatusLabel.Content = "Loading emojis...";
            Task<bool> LoadTask = C.LoadEmojis();
            success = await LoadTask;
            if (!success) {
                Progress.IsActive = false;
                StatusLabel.Content = "Failed to load emojis";
                return;
            }
            EmojiList.Visibility = Visibility.Visible;
            DownloadEmojis();
            Progress.IsActive = false;
            var bc = new BrushConverter();
            StatusLabel.Background = bc.ConvertFrom("#BF000000") as Brush;
            StatusLabel.Content = "Waiting";
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
                LoadEmojis();
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