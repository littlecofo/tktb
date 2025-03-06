using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text.Json;

namespace SecondHandMarket
{
    public partial class App : Application
    {
        public bool IsLoggedIn { get; set; } = false;
        public string CurrentUserPhoneNumber { get; set; }
        public User CurrentUser { get; set; }
        public static HttpClient HttpClient { get; private set; }
        public static string BaseAddress { get; } = "localhost:5001";

        static App()
        {
            // 注释掉全局忽略SSL证书验证
            // ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        public App()
        {
            // 在创建 HttpClient 之前设置全局协议
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // 配置 HttpClientHandler 以忽略 SSL 证书验证错误
            var handler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            // 使用指定的 HttpClientHandler 实例化 HttpClient
            HttpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://"+App.BaseAddress+"/")
            };

            InitializeComponent();
            //ClearCachedLoginInfo();
            //ClearCachedCookies();
            // 尝试自动登录
            AttemptAutoLogin();

            MainPage = new NavigationPage(new MainPage());
        }

        private async void AttemptAutoLogin()
        {
            string phoneNumber = Preferences.Get("PhoneNumber", null);
            string password = Preferences.Get("Password", null);

            if (!string.IsNullOrEmpty(phoneNumber) && !string.IsNullOrEmpty(password))
            {
                var loginData = new Requests.LoginRequest
                {
                    PhoneNumber = phoneNumber,
                    Password = password
                };

                try
                {
                    var response = await HttpClient.PostAsJsonAsync("api/Users/login", loginData);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var loginResult = JsonSerializer.Deserialize<LoginPage.LoginResult>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        IsLoggedIn = true;
                        CurrentUserPhoneNumber = phoneNumber;
                        CurrentUser = loginResult?.user;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("自动登录异常：" + ex.ToString());
                }
            }
            
        }
        // 你可以在发布前调用该方法清除缓存登录信息
private void ClearCachedLoginInfo()
{
    Preferences.Remove("PhoneNumber");
    Preferences.Remove("Password");
}
// 清除缓存的 Cookie
    private void ClearCachedCookies()
    {
        Preferences.Remove("EducationalSystemCookie");
    }
    }

    public class VersionResponse
    {
        public VersionInfo Data { get; set; }
    }

    public class VersionInfo
    {
        public string Version { get; set; }
        public string DownloadUrl { get; set; }
    }
    
    

}