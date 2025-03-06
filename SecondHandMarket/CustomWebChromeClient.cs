#if ANDROID
using Android.Runtime;
using Android.Webkit;
using Android.OS;

namespace SecondHandMarket
{
    public class CustomWebChromeClient : WebChromeClient
    {
        public override bool OnCreateWindow(Android.Webkit.WebView view, bool isDialog, bool isUserGesture, Message resultMsg)
        {
            var hitTestResult = view.GetHitTestResult();
            if (hitTestResult?.Extra != null)
            {
                view.LoadUrl(hitTestResult.Extra);
            }
            return false;
        }
    }
}
#endif