using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SecondHandMarket.Models;

namespace SecondHandMarket.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = DatabaseService.BaseUrl; // 请根据实际情况修改

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 修改后的 UploadProductAsync 方法，支持上传图片
        public async Task<bool> UploadProductAsync(Product product, MemoryStream imageStream, string imageFileName)
        {
            try
            {
                var form = new MultipartFormDataContent();

                // 添加文本字段
                form.Add(new StringContent(product.Name), "Name");
                form.Add(new StringContent(product.Description), "Description");
                form.Add(new StringContent(product.Price.ToString()), "Price");
                form.Add(new StringContent(product.OwnerPhoneNumber), "OwnerPhoneNumber");
                form.Add(new StringContent(product.CreatedAt.ToString("o")), "CreatedAt");

                // 添加图片文件（如果有）
                if (imageStream != null && !string.IsNullOrEmpty(imageFileName))
                {
                    // 这里假设图片为 JPEG 格式，你可根据实际情况调整
                    imageStream.Position = 0;
                    var streamContent = new StreamContent(imageStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    form.Add(streamContent, "ProfilePhoto", imageFileName);
                }

                var response = await _httpClient.PostAsync($"{BaseUrl}/products", form);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("上传商品异常：" + ex.Message);
                return false;
            }
        }
    }
}