using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage; // 添加此引用
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SecondHandMarket
{
    public partial class GradeQueryPage : ContentPage
    {
        private string url;
        public GradeQueryPage(string Url)
        {
            InitializeComponent();
            url = Url;
        }

        // WebView导航完成事件
        private async void LoginWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            Debug.WriteLine($"WebView导航到URL：{e.Url}");
            // 当进入统一门户时自动模拟点击教务系统入口按钮
            if (e.Url.Contains("ronghemenhu"))
            {
                // 登录成功后，等待页面加载完毕
                await Task.Delay(1000);

                // 模拟点击入口按钮，这里根据图片的 alt 属性定位按钮
                try
                {
                    await LoginWebView.EvaluateJavaScriptAsync("window.open = function(url){ window.location.href = url; };");
                    await LoginWebView.EvaluateJavaScriptAsync("document.querySelector('img[alt=\"教务系统\"]').click();");
                    await Task.Delay(1000);
                    LoginWebView.Source = url;
                    Debug.WriteLine("已模拟点击教务系统入口按钮");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("点击教务系统入口按钮时发生错误: " + ex.Message);
                }

                // 等待重定向及Cookie更新
                await Task.Delay(1000);
                var cookie = await LoginWebView.EvaluateJavaScriptAsync("document.cookie");
                Debug.WriteLine("自动获取到Cookie: " + cookie);
                // 保存cookie，下次可以直接使用
                Preferences.Default.Set("EducationalSystemCookie", cookie);
                Debug.WriteLine("Cookie已保存到本地");

                // 此处可以继续后续的操作，如自动跳转到教务系统查询页面
            }
        }

        // 手动点击按钮获取Cookie（备用）
        // private async void OnLoginButtonClicked(object sender, EventArgs e)
        // {
        //     var cookie = await LoginWebView.EvaluateJavaScriptAsync("document.cookie");
        //     Debug.WriteLine("手动获取到Cookie: " + cookie);
        //     if (!string.IsNullOrEmpty(cookie))
        //     {
        //         await DisplayAlert("提示", "已获取到Cookie，后续查询成绩时将使用该Cookie", "确定");
        //         // 保存cookie
        //         Preferences.Default.Set("EducationalSystemCookie", cookie);
        //         Debug.WriteLine("Cookie已保存到本地");

        //         // 在此处保存cookie，或传递给后续服务使用
        //         await Task.Delay(1000);
        //         LoginWebView.Source = "https://newjwc.tyust.edu.cn/jwglxt/xtgl/index_initMenu.html";
        //     }
        //     else
        //     {
        //         await DisplayAlert("提示", "未获取到Cookie，请确保登录成功", "确定");
        //     }
        // }
    }
}