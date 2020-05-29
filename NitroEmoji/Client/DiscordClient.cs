using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NitroEmoji.Client
{

    class PartialGuild
    {
        public string id;
        public string name;

        public PartialGuild(string id, string name) {
            this.id = id;
            this.name = name;
        }

    }

    class DiscordClient
    {
        public string Token;
        public List<PartialGuild> Guilds = new List<PartialGuild>();

        private void HandleError(WebException e, string taskName) {
            var response = e.Response as HttpWebResponse;
            if (response != null) {
                var res = new StreamReader(response.GetResponseStream()).ReadToEnd();
                Debug.WriteLine("{0} failed: {1} => {2}", taskName, (int)response.StatusCode, res);
            }
        }

        public async Task<bool> Login(string email, string pass) {
            WebClient w = new WebClient();

            string payload = $"{{\"email\":\"{email}\",\"password\":\"{pass}\",\"undelete\":false,\"captcha_key\":null,\"login_source\":null,\"gift_code_sku_id\":null}}";
            w.Headers.Set("Content-Type", "application/json");
            try {
                var res = await w.UploadStringTaskAsync("https://discord.com/api/v6/auth/login", payload);
                dynamic data = JObject.Parse(res);
                Token = data.token;
                return true;
            } catch (WebException e) {
                HandleError(e, "Login");
                return false;
            }
        }

        public async Task<bool> GetGuilds() {
            WebClient w = new WebClient();
            w.Headers.Set("Authorization", Token);
            try {
                var res = await w.DownloadStringTaskAsync("https://discord.com/api/v6/users/@me/guilds");
                dynamic data = JArray.Parse(res);
                foreach(dynamic guild in data) {
                    string id = guild.id;
                    string name = guild.name;
                    Guilds.Add(new PartialGuild(id, name));
                }
                return true;
            } catch (WebException e) {
                HandleError(e, "Guild list");
                return false;
            }
        }

        public async Task<bool> LoadEmojis() {
            return true;
        }


    }
}
