using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Services;
using System;
using System.Diagnostics;

namespace SecondHandMarket
{
    public partial class ProductDetailPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private Product _currentProduct;
        // 根据实际情况修改基础 URL
        private const string BaseUrl = "https://localhost:5184";

        public ProductDetailPage(Product product)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _currentProduct = product;
            LoadProductDetails();
        }

        private async void LoadProductDetails()
        {
            var seller = await _databaseService.GetUserAsync(_currentProduct.OwnerPhoneNumber);
            Debug.Print(seller.Nickname);
            if (seller != null)
            {
                ProductImage.Source = await _databaseService.LoadImageFromServerAsync($"/var/www/websecondhandapp/wwwroot{_currentProduct.Image}");
                ProductName.Text = _currentProduct.Name;
                // 设置商品发布日期
                ProductDate.Text = $"发布日期: {_currentProduct.CreatedAt:yyyy-MM-dd}";
                ProductPrice.Text = $"价格: {_currentProduct.Price:C}";
                ProductDescription.Text = _currentProduct.Description;
                SellerProfilePhoto.Source = await _databaseService.LoadImageFromServerAsync($"/var/www/websecondhandapp/wwwroot/images/profile/{seller.ProfilePhoto}");
                SellerNickname.Text = $"卖家: {seller.Nickname}";
                SellerTitle.Text = $"称号: {seller.Title}";
                SellerLevel.Text = $"等级: {seller.Level}";
            }
            else
            {
                await DisplayAlert("错误", "无法加载卖家信息", "确定");
            }
        }
    }
}