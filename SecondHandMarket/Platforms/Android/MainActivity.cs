using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.Content;
using Android;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace SecondHandMarket
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // 强制使用 TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // 动态请求网络权限
            RequestNetworkPermissions();

        }

        private void RequestNetworkPermissions()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Internet) != Permission.Granted)
            {
                RequestPermissions(new[] { Manifest.Permission.Internet }, 0);
            }
        }

    }
}