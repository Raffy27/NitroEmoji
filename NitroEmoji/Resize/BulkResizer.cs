using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NitroEmoji.Resize
{
    class BulkResizer
    {
        private static Task<int> RunProcessAsync(Process process) {
            var tcs = new TaskCompletionSource<int>();

            process.EnableRaisingEvents = true;

            process.Exited += (sender, args) => {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();

            return tcs.Task;
        }

        public static async Task ResizeGif(string path) {
            var gifsicle = new Process() {
                StartInfo = {
                    FileName = "gifsicle.exe",
                    Arguments = "--batch --resize-fit 50x50 \"" + path + "\"",
                    WindowStyle = ProcessWindowStyle.Hidden
        }
            };
            await RunProcessAsync(gifsicle);
        }

        public static async Task ResizeGifs(string dir) {
            await ResizeGif(dir + "\\*.gif");
        }

        public static async Task ResizePng(string path) {
            var b = new BitmapImage(new Uri(path));
            int max = (int)Math.Max(b.PixelWidth, b.PixelHeight);
            if (max <= 50) {
                return;
            }
            var factor = 50.0 / max;
            var t = new TransformedBitmap(b, new ScaleTransform(factor, factor));
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(t));

            using (var fileStream = new FileStream(path, FileMode.Create)) {
                encoder.Save(fileStream);
            }
        }

        public static async Task ResizePngs(string dir) {
            var d = new DirectoryInfo(dir);
            foreach (var file in d.GetFiles("*.png")) {
                await ResizePng(file.FullName);
            }
        }

    }
}
