using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SecondHandMarket
{
    public partial class ForumPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private List<Post> _posts;
        private int _currentPage = 0;
        private const int PageSize = 10;


        public ForumPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadPosts();
        }

        private async void LoadPosts()
        {
            Debug.WriteLine($"LoadPosts called with _currentPage: {_currentPage}");
            _posts = await _databaseService.GetPostsByPageAsync(_currentPage, PageSize);
            PostsCollectionView.ItemsSource = _posts;
            _currentPage++;
            Debug.WriteLine($"LoadPosts completed, _currentPage incremented to: {_currentPage}");
        }

        private async void OnLoadMoreClicked(object sender, EventArgs e)
        {

            Debug.WriteLine($"OnLoadMoreClicked called with _currentPage: {_currentPage}");
            var morePosts = await _databaseService.GetPostsByPageAsync(_currentPage, PageSize);
            if (morePosts.Count > 0)
            {
                _posts.AddRange(morePosts);
                PostsCollectionView.ItemsSource = null;
                PostsCollectionView.ItemsSource = _posts;
                _currentPage++;
                Debug.WriteLine($"OnLoadMoreClicked completed, _currentPage incremented to: {_currentPage}");
            }
            else
            {
                await DisplayAlert("提示", "没有更多帖子了", "确定");
            }

        }

        private async void OnAddPostClicked(object sender, EventArgs e)
        {
            if (!((App)Application.Current).IsLoggedIn)
            {
                await Navigation.PushAsync(new LoginPage());
            }
            else
            {
                await Navigation.PushAsync(new EditPostPage(new Post { AuthorPhoneNumber = ((App)Application.Current).CurrentUserPhoneNumber }));
            }
        }

        private async void OnPostTitleTapped(object sender, EventArgs e)
        {
            var label = sender as Label;
            var selectedPost = label?.BindingContext as Post;
            if (selectedPost != null)
            {
                await Navigation.PushAsync(new PostDetailPage(selectedPost));
            }
        }

        private async void OnPostSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedPost = e.CurrentSelection.FirstOrDefault() as Post;
            if (selectedPost != null)
            {
                await Navigation.PushAsync(new PostDetailPage(selectedPost));
                PostsCollectionView.SelectedItem = null; // 清除选择
            }
        }

        
        private async void OnSearchBarButtonPressed(object sender, EventArgs e)
        {
            string searchText = PostSearchBar.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                _posts = await _databaseService.SearchPostsAsync(searchText);
                PostsCollectionView.ItemsSource = _posts;
            }
            else
            {
                _currentPage = 0;
                LoadPosts();
            }
        }
        

        
        private async void OnCategoryPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedCategory = CategoryPicker.SelectedItem as string;
            if (selectedCategory == "全部")
            {
                _currentPage = 0;
                LoadPosts();
            }
            else
            {
                _posts = await _databaseService.GetPostsByCategoryAsync(selectedCategory);
                PostsCollectionView.ItemsSource = _posts;
            }
        }
        
    }
}