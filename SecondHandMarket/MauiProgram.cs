using Microsoft.Extensions.Logging;

namespace SecondHandMarket
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if ANDROID
Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("CustomWebChromeClient", (handler, view) =>
{
    if (handler.PlatformView is Android.Webkit.WebView androidWebView)
    {
        androidWebView.Settings.JavaScriptEnabled = true;
        androidWebView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
        androidWebView.Settings.DomStorageEnabled = true;
        androidWebView.Settings.SetSupportMultipleWindows(true);  // 启用多窗口支持
        androidWebView.SetWebChromeClient(new CustomWebChromeClient());
    }
});
#endif

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
