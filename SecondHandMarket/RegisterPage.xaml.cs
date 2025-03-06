using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Requests;
using SecondHandMarket.Services;
using System;
using System.Net.Http.Json;

namespace SecondHandMarket
{
    public partial class RegisterPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly VerificationService _verificationService;
        private readonly HttpClient _httpClient;
        // 保存最近一次验证码发送的时间
        private DateTime? _lastVerificationSentTime;

        public RegisterPage()
        {
            InitializeComponent();
            _verificationService = new VerificationService();
            _databaseService = new DatabaseService();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:5184/")
            };
        }

        private async void OnGetVerificationCodeClicked(object sender, EventArgs e)
        {
            // 获取手机号
            string phoneNumber = PhoneNumberEntry.Text?.Trim();
            if (string.IsNullOrEmpty(phoneNumber))
            {
                await DisplayAlert("错误", "请输入手机号", "确定");
                return;
            }
            // 检查手机号格式
            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^1[3456789]\d{9}$"))
            {
                await DisplayAlert("错误", "手机号格式错误", "确定");
                return;
            }

            // 判断验证码发送时间，限制60秒发送一次
            if (_lastVerificationSentTime != null && (DateTime.Now - _lastVerificationSentTime.Value) < TimeSpan.FromSeconds(60))
            {
                await DisplayAlert("错误", "验证码已发送，请60秒后再试", "确定");
                return;
            }

            // 生成并发送验证码，真实环境代码
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Users/sendCode", phoneNumber);
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("成功", "验证码已发送", "确定");
                    _lastVerificationSentTime = DateTime.Now;
                }
                else
                {
                    var errorResult = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("错误", errorResult, "确定");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("异常", ex.Message, "确定");
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PhoneNumberEntry.Text) || string.IsNullOrEmpty(VerificationCodeEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
            {
                await DisplayAlert("错误", "所有字段都是必填的", "确定");
                return;
            }
            //设置局部变量，测试用
            string phoneNumber = PhoneNumberEntry.Text?.Trim();
            string verificationCode = VerificationCodeEntry.Text?.Trim();
            string password = PasswordEntry.Text;

            // 检查密码长度
            if (password.Length < 8 || password.Length > 20)
            {
                await DisplayAlert("错误", "密码必须在8至20位之间", "确定");
                return;
            }

            // 检查手机号格式
            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^1[3456789]\d{9}$"))
            {
                await DisplayAlert("错误", "手机号格式错误", "确定");
                return;
            }

            //定义注册请求对象，真实环境代码
            var registerData = new RegisterRequest
            {
                PhoneNumber = PhoneNumberEntry.Text?.Trim(),
                Code = VerificationCodeEntry.Text?.Trim(),
                Password = PasswordEntry.Text
            };

            // 链接服务器检查是否注册成功
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Users/register", registerData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseResult>();
                    await DisplayAlert("成功", result?.message ?? "注册成功", "确定");
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
                await DisplayAlert("异常", ex.Message, "确定");
            }
        }

        public class ResponseResult
        {
            public string message { get; set; }
        }
    }
}