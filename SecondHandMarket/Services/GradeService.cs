using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SecondHandMarket.Services
{
    public class GradeService
{
    private readonly HttpClient _client;

    public GradeService()
    {
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = false, // 禁止自动重定向
            UseCookies = true,         // 启用 Cookie 容器
            CookieContainer = new CookieContainer()
        };
        _client = new HttpClient(handler);
    }

    public async Task<List<string>> GetGradesAsync(string gradesUrl, string cookies)
    {
        Debug.WriteLine($"使用的 Cookie: {cookies}");
        
        // 设置请求头
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("Cookie", cookies);
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        _client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

        var request = new HttpRequestMessage(HttpMethod.Get, gradesUrl);
        var response = await _client.SendAsync(request);

        // 处理重定向
        if (response.StatusCode == System.Net.HttpStatusCode.Found)
        {
            var redirectUrl = response.Headers.Location.ToString();
            Debug.WriteLine($"被重定向到登录页面: {redirectUrl}");
            Debug.WriteLine("需要重新登录获取新的 Cookie");
            throw new UnauthorizedAccessException("登录已过期，请重新登录");
        }

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        
        // 输出更详细的 HTML 内容信息
        Debug.WriteLine($"页面标题: {GetPageTitle(html)}");
        Debug.WriteLine($"页面内容长度: {html.Length}");
        Debug.WriteLine("页面关键内容:");
        Debug.WriteLine(html.Substring(0, Math.Min(html.Length, 2000)));

        return ParseGrades(html);
    }

    private string GetPageTitle(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var titleNode = doc.DocumentNode.SelectSingleNode("//title");
        return titleNode?.InnerText ?? "未找到标题";
    }

    private List<string> ParseGrades(string html)
    {
        var gradeList = new List<string>();
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // 输出所有可能包含成绩的表格的 id
        var tables = doc.DocumentNode.SelectNodes("//table");
        if (tables != null)
        {
            Debug.WriteLine("找到的所有表格:");
            foreach (var t in tables)
            {
                Debug.WriteLine($"表格 ID: {t.Id}, Class: {t.GetAttributeValue("class", "")}");
            }
        }

        // 尝试查找成绩表格
        var table = doc.DocumentNode.SelectSingleNode("//table[@id='tabGrid']");
        if (table == null)
        {
            Debug.WriteLine("未找到 id='tabGrid' 的表格，尝试其他选择器");
            // 尝试其他可能的选择器
            table = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'cjcx')]") ??
                   doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'score')]");
        }

        if (table != null)
        {
            // ... 原有的表格处理代码 ...
        }
        else
        {
            Debug.WriteLine("未找到任何可能包含成绩的表格");
        }

        return gradeList;
    }
}
}