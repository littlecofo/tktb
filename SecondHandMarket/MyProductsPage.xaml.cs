using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecondHandMarket
{
    public partial class MyProductsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private List<Product> _products;
        private int _currentPage = 0;
        private const int PageSize = 10;

        public MyProductsPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadProducts();
        }

        private async void LoadProducts()
        {
            string ownerPhoneNumber = ((App)Application.Current).CurrentUserPhoneNumber;
            _products = await _databaseService.GetProductsByOwnerAndPageAsync(ownerPhoneNumber, _currentPage, PageSize);
            ProductsCollectionView.ItemsSource = _products;
        }

        private async void OnLoadMoreClicked(object sender, EventArgs e)
        {
            _currentPage++;
            string ownerPhoneNumber = ((App)Application.Current).CurrentUserPhoneNumber;
            var moreProducts = await _databaseService.GetProductsByOwnerAndPageAsync(ownerPhoneNumber, _currentPage, PageSize);
            if (moreProducts.Count > 0)
            {
                _products.AddRange(moreProducts);
                ProductsCollectionView.ItemsSource = null;
                ProductsCollectionView.ItemsSource = _products;
            }
            else
            {
                await DisplayAlert("提示", "没有更多商品了", "确定");
            }
        }

        private async void OnDeleteProductClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button?.BindingContext is Product product)
            {
                bool confirm = await DisplayAlert("确认删除", "确定要删除此商品吗？", "删除", "取消");
                if (confirm)
                {
                    bool result = await _databaseService.DeleteProductAsync(product.Id);
                    if (result)
                    {
                        _products.Remove(product);
                        ProductsCollectionView.ItemsSource = null;
                        ProductsCollectionView.ItemsSource = _products;
                        await DisplayAlert("提示", "删除成功", "确定");
                    }
                    else
                    {
                        await DisplayAlert("提示", "删除失败，请重试", "确定");
                    }
                }
            }
        }

        private async void OnAddProductClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditProductPage(new Product { OwnerPhoneNumber = ((App)Application.Current).CurrentUserPhoneNumber }));
        }

        // 删除 OnEditProductClicked 方法
        // private async void OnEditProductClicked(object sender, EventArgs e)
        // {
        //     var button = sender as Button;
        //     var product = button.CommandParameter as Product;
        //     await Navigation.PushAsync(new EditProductPage(product));
        // }
    }
}