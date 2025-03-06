using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Services;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SecondHandMarket
{
    public partial class EditProductPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private Product _currentProduct;
        // 保存选取图片的数据
        private MemoryStream _imageStream;
        private string _imageFileName;
        private readonly HttpClient _httpClient;

        public EditProductPage(Product product)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _currentProduct = product;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://"+App.BaseAddress+"/")
            };

            LoadProduct();
        }

        private void LoadProduct()
        {
            NameEntry.Text = _currentProduct.Name;
            DescriptionEntry.Text = _currentProduct.Description;
            PriceEntry.Text = _currentProduct.Price.ToString(CultureInfo.InvariantCulture);
            ProductImage.Source = _currentProduct.Image;
        }

        private async void OnPickPhotoClicked(object sender, EventArgs e)
        {
            var result = await MediaPicker.PickPhotoAsync();
            if (result != null)
            {
                using (var stream = await result.OpenReadAsync())
                {
                    // 复制到内存流，确保后续可以多次读取
                    _imageStream = new MemoryStream();
                    await stream.CopyToAsync(_imageStream);
                    _imageStream.Position = 0;
                }
                _imageFileName = result.FileName;
                ProductImage.Source = ImageSource.FromStream(() => {
                    // 返回新的 MemoryStream 拷贝用于预览
                    var previewStream = new MemoryStream();
                    _imageStream.Position = 0;
                    _imageStream.CopyTo(previewStream);
                    previewStream.Position = 0;
                    return previewStream;
                });
            }
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            var result = await MediaPicker.CapturePhotoAsync();
            if (result != null)
            {
                using (var stream = await result.OpenReadAsync())
                {
                    _imageStream = new MemoryStream();
                    await stream.CopyToAsync(_imageStream);
                    _imageStream.Position = 0;
                }
                _imageFileName = result.FileName;
                ProductImage.Source = ImageSource.FromStream(() => {
                    var previewStream = new MemoryStream();
                    _imageStream.Position = 0;
                    _imageStream.CopyTo(previewStream);
                    previewStream.Position = 0;
                    return previewStream;
                });
            }
        }

        // 不再直接将本地路径写入 _currentProduct.Image，而是由服务端返回修改后的图片 URL
        private void SavePhoto(string photoPath)
        {
            // 此方法不再使用
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // 检查商品名称是否为空及字数限制（1~10字）
            string name = NameEntry.Text?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                await DisplayAlert("错误", "商品名称不能为空", "确定");
                return;
            }
            if (name.Length < 1 || name.Length > 10)
            {
                await DisplayAlert("错误", "商品名称长度必须在1到10字之间", "确定");
                return;
            }
            
            // 检查商品介绍是否为空及字数限制（不超过100字）
            string description = DescriptionEntry.Text?.Trim();
            if (string.IsNullOrEmpty(description))
            {
                await DisplayAlert("错误", "商品介绍不能为空", "确定");
                return;
            }
            if (description.Length > 100)
            {
                await DisplayAlert("错误", "商品介绍不能超过100字", "确定");
                return;
            }
            
            // 提醒卖家最好在介绍中留下企业微信联系方式
            if (!(description.Contains("企微") || description.Contains("企业微信") || description.Contains("微信")))
            {
                await DisplayAlert("提示", "建议您在商品介绍中留下企业微信的联系方式，以便买家联系", "确定");
            }
            
            // 解析价格
            if (!decimal.TryParse(PriceEntry.Text, out decimal price))
            {
                await DisplayAlert("错误", "请输入正确的价格", "确定");
                return;
            }
            
            _currentProduct.Name = name;
            _currentProduct.Description = description;
            _currentProduct.Price = price;
            _currentProduct.CreatedAt = DateTime.Now;
            
            // 假设登录后已保存用户手机号
            _currentProduct.OwnerPhoneNumber = ((App)Application.Current).CurrentUserPhoneNumber;  
            
            // 调用 ProductService 上传商品数据及图片
            var productService = new ProductService(_httpClient);
            bool success = await productService.UploadProductAsync(_currentProduct, _imageStream, _imageFileName);
            
            if (success)
            {
                await DisplayAlert("提示", "商品已上传", "确定");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("错误", "商品上传失败", "确定");
            }
        }
    }
}