using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecondHandMarket
{
    public partial class MyPostsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly string _userPhoneNumber;

        public MyPostsPage(string userPhoneNumber)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _userPhoneNumber = userPhoneNumber;
            LoadUserPosts();
        }

        private async void LoadUserPosts()
        {
            List<Post> userPosts = await _databaseService.GetPostsByUserAsync(_userPhoneNumber);
            PostsCollectionView.ItemsSource = userPosts;
        }

        private async void OnPostSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                var selectedPost = e.CurrentSelection[0] as Post;
                if (selectedPost != null)
                {
                    await Navigation.PushAsync(new PostDetailPage(selectedPost));
                }
                PostsCollectionView.SelectedItem = null;
            }
        }
        private async void OnDeletePostClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var post = button.CommandParameter as Post;

            bool confirm = await DisplayAlert("确认删除", $"确定要删除帖子 '{post.Title}' 吗？", "删除", "取消");
            if (confirm)
            {
                await _databaseService.DeletePostAsync(post.Id);
                LoadUserPosts();
            }
        }
    }
}