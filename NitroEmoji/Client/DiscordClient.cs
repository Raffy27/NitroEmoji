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
    class DiscordClient
    {
        public string Token;

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
                var response = e.Response as HttpWebResponse;
                if (response != null) {
                    var res = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Debug.WriteLine("Login failed: {0} => {1}", (int)response.StatusCode, res);
                }                
                return false;
            }
        }
    }
}
