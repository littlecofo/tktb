using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Requests;

namespace SecondHandMarket.Services
{
    public class DatabaseService
    {
        // 使用可配置 Handler 的 HttpClient，开发环境中可忽略证书错误
        private static readonly HttpClient client = new HttpClient(
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            }
        );
        // 请根据实际情况修改服务端地址和端口
        public const string BaseUrl = "https://localhost:5184/api";

        // 获取所有商品数据（由服务端进行返回）
        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var products = await client.GetFromJsonAsync<List<Product>>($"{BaseUrl}/products/all");
                // 如果服务端返回 null，则返回空列表, 避免空引用异常
                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取商品异常" + ex.Message);
                return new List<Product>();
            }
        }

        // 客户端分页：先获取所有产品，再在客户端进行分页（数据量较大时建议服务端支持分页）
        public async Task<List<Product>> GetProductsByPageAsync(int page, int pageSize)
        {
            try
            {
                string requestUrl = $"{BaseUrl}/products?page={page}&pageSize={pageSize}";
                var products = await client.GetFromJsonAsync<List<Product>>(requestUrl);
                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("分页获取商品异常：" + ex.Message);
                return new List<Product>();
            }
        }

        // 搜索功能：根据名称、描述进行简单过滤
        public async Task<List<Product>> SearchProductsAsync(string searchText)
        {
            try
            {
                string requestUrl = $"{BaseUrl}/products/search?query={Uri.EscapeDataString(searchText)}";
                var products = await client.GetFromJsonAsync<List<Product>>(requestUrl);
                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("搜索商品异常：" + ex.Message);
                return new List<Product>();
            }
        }

        // 保存用户信息
        public async Task<bool> SaveUserAsync(User user)
        {
            try
            {
                var response = await client.PostAsJsonAsync($"{BaseUrl}/users", user);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("保存用户异常：" + ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(UpdateUserProfileRequest request)
        {
            try
            {
                var response = await client.PostAsJsonAsync($"{BaseUrl}/users/updateProfile", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("更新用户资料异常：" + ex.Message);
                return false;
            }
        }

        public async Task<bool> UploadProfilePhotoAsync(string phoneNumber, string photoPath)
        {
            try
            {
                using var multipartContent = new MultipartFormDataContent();
                byte[] fileBytes = File.ReadAllBytes(photoPath);
                var byteContent = new ByteArrayContent(fileBytes);
                byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                multipartContent.Add(byteContent, "file", $"{phoneNumber}_profile_image.jpg");

                var response = await client.PostAsync($"{BaseUrl}/users/uploadProfilePhoto", multipartContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("上传头像异常：" + ex.Message);
                return false;
            }
        }

        // 获取用户信息
        public async Task<User> GetUserAsync(string phoneNumber)
        {
            try
            {
                var response = await client.GetAsync($"{BaseUrl}/users/{phoneNumber}");

                // 读取响应内容为字符串
                var responseString = await response.Content.ReadAsStringAsync();
                // 反序列化为复合对象 LoginResult
                var Getresult = JsonSerializer.Deserialize<LoginPage.LoginResult>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Getresult?.user;

            }
            catch (Exception ex)
            {
                Console.WriteLine("获取用户异常：" + ex.Message);
                return null;
            }
        }

        // 保存商品信息
        public async Task<bool> SaveProductAsync(Product product)
        {
            try
            {
                var response = await client.PostAsJsonAsync($"{BaseUrl}/products", product);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("保存商品异常：" + ex.Message);
                return false;
            }
        }

        // 删除商品
        public async Task<bool> DeleteProductAsync(int productId)
        {
            try
            {
                var response = await client.DeleteAsync($"{BaseUrl}/products/{productId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("删除商品异常：" + ex.Message);
                return false;
            }
        }

        // 保存帖子信息
        public async Task<bool> SavePostAsync(Post post)
        {
            try
            {
                Console.WriteLine("发送发帖请求: " + System.Text.Json.JsonSerializer.Serialize(post));
                var response = await client.PostAsJsonAsync($"{BaseUrl}/posts", post);
                Console.WriteLine("发帖请求响应: " + response.StatusCode);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("错误内容: " + errorContent);
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("保存帖子异常：" + ex.Message);
                return false;
            }
        }

        // 获取所有帖子
        public async Task<List<Post>> GetPostsAsync()
        {
            try
            {
                var posts = await client.GetFromJsonAsync<List<Post>>($"{BaseUrl}/posts");
                return posts ?? new List<Post>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取帖子异常：" + ex.Message);
                return new List<Post>();
            }
        }

        // 分页获取帖子
        public async Task<List<Post>> GetPostsByPageAsync(int page, int pageSize)
        {
            try
            {
                string requestUrl = $"{BaseUrl}/posts/page?page={page}&pageSize={pageSize}";
                var posts = await client.GetFromJsonAsync<List<Post>>(requestUrl);
                return posts ?? new List<Post>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("分页获取帖子异常：" + ex.Message);
                return new List<Post>();
            }
        }

        // 搜索帖子
        public async Task<List<Post>> SearchPostsAsync(string searchText)
        {
            try
            {
                string requestUrl = $"{BaseUrl}/posts/search?query={Uri.EscapeDataString(searchText)}";
                var posts = await client.GetFromJsonAsync<List<Post>>(requestUrl);
                return posts ?? new List<Post>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("搜索帖子异常：" + ex.Message);
                return new List<Post>();
            }
        }

        // 获取用户的帖子
        public async Task<List<Post>> GetPostsByUserAsync(string userPhoneNumber)
        {
            try
            {
                string requestUrl = $"{BaseUrl}/posts/user/{Uri.EscapeDataString(userPhoneNumber)}";
                var posts = await client.GetFromJsonAsync<List<Post>>(requestUrl);
                return posts ?? new List<Post>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取用户帖子异常：" + ex.Message);
                return new List<Post>();
            }
        }
        // 获取分类的帖子
        public async Task<List<Post>> GetPostsByCategoryAsync(string category)
        {
            var allPosts = await GetPostsAsync();
            return allPosts
                   .Where(p => p.Category == category)
                   .ToList();
        }

        // 删除帖子
        public async Task<bool> DeletePostAsync(int postId)
        {
            try
            {
                var response = await client.DeleteAsync($"{BaseUrl}/posts/{postId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("删除帖子异常：" + ex.Message);
                return false;
            }
        }

        // 保存评论
        public async Task<bool> SaveCommentAsync(Comment comment)
        {
            try
            {
                var response = await client.PostAsJsonAsync($"{BaseUrl}/comments", comment);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("保存评论异常：" + ex.Message);
                return false;
            }
        }

        // 获取帖子的评论
        public async Task<List<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            try
            {
                var comments = await client.GetFromJsonAsync<List<Comment>>($"{BaseUrl}/comments/{postId}");
                return comments ?? new List<Comment>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取评论异常：" + ex.Message);
                return new List<Comment>();
            }
        }

        // 分页获取帖子的评论
        public async Task<List<Comment>> GetCommentsByPostIdAndPageAsync(int postId, int page, int pageSize)
        {
            try
            {
                var url = $"{BaseUrl}/comments/{postId}/page/{page}/size/{pageSize}";
                var comments = await client.GetFromJsonAsync<List<Comment>>(url);
                return comments ?? new List<Comment>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取评论异常：" + ex.Message);
                return new List<Comment>();
            }
        }

        public async Task<List<Product>> GetProductsByOwnerAndPageAsync(string ownerPhoneNumber, int page, int pageSize)
        {
            try
            {
                string requestUrl = $"{BaseUrl}/products/owner/{Uri.EscapeDataString(ownerPhoneNumber)}?page={page}&pageSize={pageSize}";
                var products = await client.GetFromJsonAsync<List<Product>>(requestUrl);
                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取用户商品异常：" + ex.Message);
                return new List<Product>();
            }
        }

        // 新增方法：通过服务端接口获取图片
        public async Task<ImageSource> LoadImageFromServerAsync(string filePath)
        {
            try
            {
                string requestUrl = $"{BaseUrl}/file/getImage?filePath={Uri.EscapeDataString(filePath)}";
                var response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    return ImageSource.FromStream(() => stream);
                }
                else
                {
                    // 错误处理或返回默认图片
                    Console.WriteLine($"获取图片失败，状态码：{response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("加载图片异常：" + ex.Message);
                return null;
            }
        }
    }
}