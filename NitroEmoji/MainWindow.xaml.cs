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
using NitroEmoji.Resize;
using WpfAnimatedGif;

namespace NitroEmoji
{

    public delegate double EmojiEvent(object sender, MouseEventArgs e);

    public class GuildDisplay {
        public string Title { get; set; }
        public ObservableCollection<Image> Emojis { get; set; }
        public bool IsExpanded { get; set; }

        public GuildDisplay(PartialGuild p) {
            this.Title = p.name;
            Emojis = new ObservableCollection<Image>();
        }

        public async Task AddEmoji(PartialEmoji e, DiscordClient source, MouseButtonEventHandler onClick, MouseEventHandler onMove) {
            var img = new Image() {
                Width = 48,
                Height = 48,
                ToolTip = ':' + e.name + ':',
                Tag = source.FromCache(e)
            };
            this.Emojis.Add(img);
            var data = await source.EmojiFromCache(e);
            if (e.animated) {
                ImageBehavior.SetAnimatedSource(img, data);
            } else {
                img.Source = data;
            }
            img.MouseDown += onClick;
            img.MouseMove += onMove;
        }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public static RoutedCommand AcceptToken = new RoutedCommand();
        public static RoutedCommand AddExtra = new RoutedCommand();
        public static RoutedCommand DisplayHelp = new RoutedCommand();

        private DiscordClient C = new DiscordClient("cache");
        private ObservableCollection<GuildDisplay> Servers = new ObservableCollection<GuildDisplay>();

        public MainWindow() {
            InitializeComponent();
            EmojiList.ItemsSource = Servers;
            AcceptToken.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control));
            AddExtra.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            DisplayHelp.InputGestures.Add(new KeyGesture(Key.F1));
        }

        private void HelpRequested(object sender, ExecutedRoutedEventArgs e) {
            var body = "";
            MessageBox.Show(body, "Help");
        }

        private bool ExtraExists() {
            return Servers.Count > 0 && Servers[0].Title == "Extra";
        }

        private async void AddExtraEmoji(object sender, ExecutedRoutedEventArgs e) {
            var id = Clipboard.GetText();
            if (!DiscordClient.IDValid(id)) {
                StatusLabel.Content = "Invalid emoji ID";
                return;
            }

            var x = MessageBox.Show("Is this emoji animated?", "New emoji", MessageBoxButton.YesNo);
            if(x == MessageBoxResult.Cancel) {
                return;
            }
            var p = new PartialEmoji(id, "extra" + id, x == MessageBoxResult.Yes);

            GuildDisplay Extra;
            if (!ExtraExists()) {
                Extra = new GuildDisplay(new PartialGuild("0", "Extra"));
                Servers.Insert(0, Extra);
            } else {
                Extra = Servers[0];
            }
            Extra.IsExpanded = true;

            await Extra.AddEmoji(p, C, EmojiClicked, EmojiDragged);
            if (p.animated) {
                await BulkResizer.ResizeGif(C.FromCache(p));
            } else {
                await BulkResizer.ResizePng(C.FromCache(p));
            }
            StatusLabel.Content = "Emoji added";
        }

        private void TokenChange(object sender, ExecutedRoutedEventArgs e) {
            var t = Clipboard.GetText().Trim('"');
            if (t.Length < 59) {
                StatusLabel.Content = "Invalid token";
                return;
            }
            AcceptToken.InputGestures.Clear();
            C.Token = t;
            LoginContainer.Visibility = Visibility.Hidden;
            LoadEmojis();
        }

        private async Task ResizeEmojis() {
            await BulkResizer.ResizeGifs(C.Cache);
            await BulkResizer.ResizePngs(C.Cache);
        }

        private void EmojiClicked(object sender, MouseEventArgs e) {
            var img = sender as Image;
            StatusLabel.Content = img.ToolTip;
        }

        private void EmojiDragged(object sender, MouseEventArgs e) {
            if (e.LeftButton != MouseButtonState.Pressed) 
                return;
            var img = sender as Image;
            DragDrop.DoDragDrop(sender as DependencyObject,
                new DataObject(DataFormats.FileDrop, new string[1] { img.Tag.ToString() }),
                DragDropEffects.All);
        }

        private async Task DownloadEmojis() {
            foreach(PartialGuild g in C.Guilds) {
                var disp = new GuildDisplay(g);
                Servers.Add(disp);
                disp.IsExpanded = g.emojis.Count > 0;
                foreach(PartialEmoji e in g.emojis) {
                    await disp.AddEmoji(e, C, EmojiClicked, EmojiDragged);
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
            var bc = new BrushConverter();
            StatusLabel.Background = bc.ConvertFrom("#BF000000") as Brush;
            await DownloadEmojis();
            StatusLabel.Content = "Resizing emojis...";
            await ResizeEmojis();
            Progress.IsActive = false;
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