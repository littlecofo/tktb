using System.Diagnostics;
using System.Text.Json;
using Microsoft.Maui.Controls;
using WebSecondHandApp.Client;

namespace SecondHandMarket
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // 太科市场按钮点击事件
        private async void OnMarketClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MarketPage());
        }

        // 个人空间按钮点击事件
        private async void OnProfileClicked(object sender, EventArgs e)
        {
            if (!((App)Application.Current).IsLoggedIn)
            {
                await Navigation.PushAsync(new LoginPage());
            }
            else
            {
                await Navigation.PushAsync(new ProfilePage());
            }
        }

        // 太科贴吧按钮点击事件
        private async void OnForumClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ForumPage());
        }

        // 检查更新按钮点击事件
        private async void OnCheckForUpdatesClicked(object sender, EventArgs e)
        {
            await CheckForUpdates();
        }

        public async Task CheckForUpdates()
        {
            try
            {
                var response = await App.HttpClient.GetAsync("api/version");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var versionResponse = JsonSerializer.Deserialize<VersionResponse>(responseContent, options);
                        var versionInfo = versionResponse?.Data;

                        if (versionInfo == null)
                        {
                            Debug.Print("版本信息为空");
                            return;
                        }

                        if (string.IsNullOrEmpty(VersionTracking.CurrentVersion))
                        {
                            Debug.Print("当前版本为空");
                            return;
                        }

                        var currentVersion = Version.Parse("1.0.3");
                        var latestVersion = Version.Parse(versionInfo.Version);
                        Debug.Print("当前版本：" + currentVersion);
                        Debug.Print("最新版本：" + latestVersion);

                        if (latestVersion > currentVersion)
                        {
                            var result = await DisplayAlert("更新提示", "当前版本：" + currentVersion+"最新版本：" + latestVersion+"是否更新？", "是", "否");
                            if (result)
                            {
                                await Launcher.OpenAsync(new Uri(versionInfo.DownloadUrl));
                            }
                        }
                        else if (latestVersion == currentVersion)
                        {
                            await DisplayAlert("更新提示", "当前已是最新版本", "确定");
                        }
                        else
                        {
                            await DisplayAlert("更新提示", "你的版本居然比最新版还高?好奇怪噢", "确定");
                        }
                    }
                    else
                    {
                        Debug.Print("响应内容为空");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("检查更新异常：" + ex.ToString());
                Console.WriteLine("检查更新异常：" + ex.InnerException?.Message);
            }
        }
    }
}