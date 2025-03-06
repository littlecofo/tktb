using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Requests;
using SecondHandMarket.Services;
using static SecondHandMarket.RegisterPage;

namespace SecondHandMarket
{
    public partial class LoginPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public LoginPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string phoneNumber = UsernameEntry.Text;
            string password = PasswordEntry.Text;
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("错误", "请输入账号和密码", "确定");
                return;
            }

            var loginData = new LoginRequest
            {
                PhoneNumber = phoneNumber,
                Password = password
            };

            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                using var client = new HttpClient(handler);
                client.BaseAddress = new Uri("https://"+App.BaseAddress+"/");
                var response = await client.PostAsJsonAsync("api/Users/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var loginResult = JsonSerializer.Deserialize<LoginResult>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await DisplayAlert("成功", loginResult?.message ?? "登录成功", "确定");
                    ((App)Application.Current).IsLoggedIn = true;
                    ((App)Application.Current).CurrentUserPhoneNumber = phoneNumber;
                    ((App)Application.Current).CurrentUser = loginResult?.user;

                    // 保存账号和密码到本地存储
                    Preferences.Set("PhoneNumber", phoneNumber);
                    Preferences.Set("Password", password);

                    await Navigation.PopAsync();
                }
                else
                {
                    var errorResult = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("错误", errorResult, "确定");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("登录异常", ex.Message, "确定");
            }
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            await DisplayAlert("提示", "忘记密码功能尚未实现", "确定");
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }

        public class LoginResult
        {
            public string message { get; set; }

            [JsonPropertyName("data")]
            public SecondHandMarket.Models.User user { get; set; }
        }
    }
}