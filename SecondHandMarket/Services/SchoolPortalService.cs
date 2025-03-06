using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SecondHandMarket.Services
{
    public class SchoolPortalService
    {
        private readonly HttpClient _client;
        private readonly CookieContainer _cookieContainer;

        public SchoolPortalService()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = _cookieContainer
            };
            _client = new HttpClient(handler);

            // 更新请求头，模拟真实浏览器
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");
            _client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            _client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        }
        public async Task<List<string>> GetGradesAsync()
        {
            try
            {
                // 成绩查询页面的 URL
                var gradesUrl = "https://newjwc.tyust.edu.cn/jwglxt/cjcx/cjcx_cxDgXscj.html?gnmkdm=N305005&layout=default";
                Debug.WriteLine($"开始获取成绩数据，URL: {gradesUrl}");

                var response = await _client.GetAsync(gradesUrl);
                Debug.WriteLine($"成绩页面响应状态码: {response.StatusCode}");

                var html = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"成绩页面内容长度: {html.Length}");

                // 解析成绩数据
                var gradeList = new List<string>();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // 查找成绩表格
                var table = doc.DocumentNode.SelectSingleNode("//table[@id='tabGrid']");
                if (table != null)
                {
                    Debug.WriteLine("找到成绩表格");
                    var rows = table.SelectNodes(".//tr");
                    if (rows != null)
                    {
                        Debug.WriteLine($"找到 {rows.Count} 行数据");
                        foreach (var row in rows.Skip(1)) // 跳过表头行
                        {
                            var cells = row.SelectNodes(".//td");
                            if (cells != null)
                            {
                                var gradeInfo = string.Join(" | ", cells.Select(cell => cell.InnerText.Trim()));
                                Debug.WriteLine($"成绩信息: {gradeInfo}");
                                gradeList.Add(gradeInfo);
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("未找到成绩表格，尝试其他选择器");
                    // 尝试其他可能的选择器
                    table = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'cjcx')]") ??
                           doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'score')]");

                    if (table != null)
                    {
                        Debug.WriteLine("使用备选选择器找到成绩表格");
                        // 处理表格数据...
                        var rows = table.SelectNodes(".//tr");
                        if (rows != null)
                        {
                            foreach (var row in rows.Skip(1))
                            {
                                var cells = row.SelectNodes(".//td");
                                if (cells != null)
                                {
                                    var gradeInfo = string.Join(" | ", cells.Select(cell => cell.InnerText.Trim()));
                                    gradeList.Add(gradeInfo);
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("无法找到任何包含成绩的表格");
                    }
                }

                Debug.WriteLine($"总共找到 {gradeList.Count} 条成绩记录");
                return gradeList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"获取成绩数据时出错: {ex.Message}");
                Debug.WriteLine($"错误堆栈: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
{
    try
    {
        var loginPageUrl = "https://sso1.tyust.edu.cn/login";
        Debug.WriteLine($"开始访问登录页面: {loginPageUrl}");

        var loginPageResponse = await _client.GetAsync(loginPageUrl);
        var loginPageHtml = await loginPageResponse.Content.ReadAsStringAsync();

        Debug.WriteLine($"登录页面响应状态码: {loginPageResponse.StatusCode}");
        Debug.WriteLine("页面内容片段:");
        Debug.WriteLine(loginPageHtml.Substring(0, Math.Min(loginPageHtml.Length, 1000)));

        var doc = new HtmlDocument();
        doc.LoadHtml(loginPageHtml);

        // 输出所有表单信息
        var allForms = doc.DocumentNode.SelectNodes("//form");
        if (allForms != null)
        {
            Debug.WriteLine($"找到 {allForms.Count} 个表单:");
            foreach (var form in allForms)
            {
                Debug.WriteLine($"表单 ID: {form.GetAttributeValue("id", "无ID")}, " +
                              $"Action: {form.GetAttributeValue("action", "无action")}");
            }
        }

        // 尝试多个选择器查找登录表单
        var loginForm = doc.DocumentNode.SelectSingleNode("//form[@id='fm1']") ??
                       doc.DocumentNode.SelectSingleNode("//form[contains(@class, 'login')]") ??
                       doc.DocumentNode.SelectSingleNode("//form[contains(@action, 'login')]");

        if (loginForm == null)
        {
            Debug.WriteLine("未找到登录表单，尝试获取页面所有表单元素:");
            var formElements = doc.DocumentNode.SelectNodes("//input[@type='text' or @type='password']");
            if (formElements != null)
            {
                foreach (var element in formElements)
                {
                    Debug.WriteLine($"找到输入字段: {element.GetAttributeValue("name", "无名称")}, " +
                                  $"类型: {element.GetAttributeValue("type", "无类型")}");
                }
            }
            return false;
        }

        // 获取所有输入字段
        var formData = new Dictionary<string, string>();
        var inputs = loginForm.SelectNodes(".//input");
        if (inputs != null)
        {
            foreach (var input in inputs)
            {
                var name = input.GetAttributeValue("name", "");
                var value = input.GetAttributeValue("value", "");
                var type = input.GetAttributeValue("type", "");
                if (!string.IsNullOrEmpty(name))
                {
                    formData[name] = value;
                    Debug.WriteLine($"找到表单字段: name={name}, type={type}, value={value}");
                }
            }
        }

        // 添加登录信息
        formData["username"] = username;
        formData["password"] = password;

        // 确保必要的字段存在
        if (!formData.ContainsKey("_eventId")) formData["_eventId"] = "submit";
        if (!formData.ContainsKey("execution")) formData["execution"] = "";
        if (!formData.ContainsKey("geolocation")) formData["geolocation"] = "";

        // ... 其余代码保持不变 ...

        // 模拟登录请求
        var loginUrl = loginForm.GetAttributeValue("action", loginPageUrl);
        var content = new FormUrlEncodedContent(formData);
        var loginResponse = await _client.PostAsync(loginUrl, content);

        Debug.WriteLine($"登录请求响应状态码: {loginResponse.StatusCode}");

        // 检查登录是否成功
        if (loginResponse.StatusCode == HttpStatusCode.OK)
        {
            Debug.WriteLine("登录成功");
            return true;
        }
        else
        {
            Debug.WriteLine("登录失败");
            return false;
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"登录过程出错: {ex.Message}");
        Debug.WriteLine($"错误堆栈: {ex.StackTrace}");
        return false;
    }
}
    }
}