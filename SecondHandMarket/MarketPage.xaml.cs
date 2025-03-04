using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace SecondHandMarket
{
    public partial class MarketPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private List<Product> _products;
        private int _currentPage = 0;
        private const int PageSize = 10;
        private System.Timers.Timer _searchDelayTimer;

        public MarketPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadProducts();
        }

        private async void LoadProducts()
        {
            _products = await _databaseService.GetProductsByPageAsync(_currentPage, PageSize);
            ProductsCollectionView.ItemsSource = _products;
        }

        private async void OnLoadMoreClicked(object sender, EventArgs e)
        {
            _currentPage++;
            var moreProducts = await _databaseService.GetProductsByPageAsync(_currentPage, PageSize);
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

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_searchDelayTimer != null)
            {
                _searchDelayTimer.Stop();
                _searchDelayTimer.Dispose();
            }

            _searchDelayTimer = new System.Timers.Timer(500); // 延迟500毫秒
            _searchDelayTimer.Elapsed += async (s, args) =>
            {
                _searchDelayTimer.Stop();
                _searchDelayTimer.Dispose();

                string searchText = e.NewTextValue;
                if (string.IsNullOrEmpty(searchText))
                {
                    _currentPage = 0;
                    Device.BeginInvokeOnMainThread(() => LoadProducts());
                }
                else
                {
                    _products = await _databaseService.SearchProductsAsync(searchText);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ProductsCollectionView.ItemsSource = _products;
                    });
                }
            };
            _searchDelayTimer.Start();
        }

        private async void OnProductTapped(object sender, EventArgs e)
        {
            var tappedEventArgs = e as TappedEventArgs;
            var selectedProduct = tappedEventArgs.Parameter as Product;
            if (selectedProduct != null)
            {
                await Navigation.PushAsync(new ProductDetailPage(selectedProduct));
            }
        }

        private async void OnImageBindingContextChanged(object sender, EventArgs e)
        {
            var image = sender as Image;
            if (image?.BindingContext is Product product)
            {
                // 构建正确的图片路径
                string imagePath = $"/var/www/websecondhandapp/wwwroot{product.Image}";
                image.Source = await _databaseService.LoadImageFromServerAsync(imagePath);
            }
        }
    }
}