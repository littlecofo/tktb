using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace WebSecondHandApp.Client
{
    public class CustomHttpClientHandler : HttpClientHandler
    {
        public CustomHttpClientHandler()
        {
            // 自定义服务器证书验证回调
            ServerCertificateCustomValidationCallback = (request, certificate, chain, sslPolicyErrors) =>
            {
                Debug.WriteLine($"请求的地址: {request.RequestUri}");
                Debug.WriteLine($"服务器证书主题: {certificate.Subject}");

                if (chain != null)
                {
                    Debug.WriteLine("证书链信息:");
                    foreach (var element in chain.ChainElements)
                    {
                        Debug.WriteLine($" -> {element.Certificate.Subject}");
                    }
                }
                Debug.WriteLine($"SSL策略错误: {sslPolicyErrors}");

                // 仅在无错误时通过验证，生产环境中不建议直接返回 true
                return sslPolicyErrors == SslPolicyErrors.None;
            };
        }
    }
}