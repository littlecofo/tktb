using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecondHandMarket
{
    public partial class PostDetailPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private Post _currentPost;
        private List<Comment> _comments;
        private int _currentCommentPage = 0;
        private const int CommentPageSize = 10;
        private bool _isLoadingMoreComments = false;
        private bool _hasMoreComments = true;
        private const string BaseUrl = "https://47.95.30.190";

        public PostDetailPage(Post post)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _currentPost = post;
            _comments = new List<Comment>(); // 初始化 _comments 字段
            LoadPostDetails();
            LoadComments();
        }

        private async void LoadPostDetails()
        {
            var author = await _databaseService.GetUserAsync(_currentPost.AuthorPhoneNumber);
            TitleLabel.Text = _currentPost.Title;
            // 通过 DatabaseService 异步获取作者头像（用于界面上直接显示）
            var profileSource = await _databaseService.LoadImageFromServerAsync($"/var/www/websecondhandapp/wwwroot/images/profile/{author.ProfilePhoto}");
            AuthorProfilePhoto.Source = profileSource;
            AuthorLabel.Text = $"作者: {author.Nickname}";
            AuthorLevel.Text = $"等级: {author.Level}";
            AuthorTitle.Text = $"头衔: {author.Title}";
            CategoryLabel.Text = $"分类: {_currentPost.Category}";
            ContentLabel.Text = _currentPost.Content; // 使用 HtmlLabel 控件
            ContentLabel.Text = _currentPost.Content.Replace("\n", "<br>");
        }

        private async void LoadComments()
        {
            _comments = await _databaseService.GetCommentsByPostIdAndPageAsync(_currentPost.Id, _currentCommentPage, CommentPageSize);

            // 对于每个评论，直接将图片路径赋值给 AuthorProfilePhoto（因为该属性是 string 类型）
            for (int i = 0; i < _comments.Count; i++)
            {
                var comment = _comments[i];
                var author = await _databaseService.GetUserAsync(comment.AuthorPhoneNumber);
                comment.AuthorNickname = author.Nickname;
                comment.AuthorProfilePhoto = $"/var/www/websecondhandapp/wwwroot/images/profile/{comment.AuthorPhoneNumber}_profile_image.jpg";
                comment.FloorNumber = i + 1 + _currentCommentPage * CommentPageSize; // 设置楼号
            }

            CommentsCollectionView.ItemsSource = _comments;
        }
        private async void OnCommentsScrolled(object sender, EventArgs e)
        {
            if (_isLoadingMoreComments || !_hasMoreComments)
                return;

            _isLoadingMoreComments = true;
            await LoadMoreComments();
            _isLoadingMoreComments = false;
        }

        private async Task LoadMoreComments()
        {
            _currentCommentPage++;
            var moreComments = await _databaseService.GetCommentsByPostIdAndPageAsync(_currentPost.Id, _currentCommentPage, CommentPageSize);

            if (moreComments.Count > 0)
            {
                // 对于新加载的评论，同样直接赋值图片路径
                for (int i = 0; i < moreComments.Count; i++)
                {
                    var comment = moreComments[i];
                    var author = await _databaseService.GetUserAsync(comment.AuthorPhoneNumber);
                    comment.AuthorNickname = author.Nickname;
                    comment.AuthorProfilePhoto = $"/var/www/websecondhandapp/wwwroot/images/profile/{comment.AuthorPhoneNumber}_profile_image.jpg";
                    comment.FloorNumber = _comments.Count + i + 1; // 设置楼号
                }

                _comments.AddRange(moreComments);
                CommentsCollectionView.ItemsSource = null;
                CommentsCollectionView.ItemsSource = _comments;
            }
            else
            {
                _hasMoreComments = false;
                await DisplayAlert("提示", "没有更多留言了", "确定");
            }
        }

        private async void OnSendCommentClicked(object sender, EventArgs e)
        {
            if (!((App)Application.Current).IsLoggedIn)
            {
                await Navigation.PushAsync(new LoginPage());
                return;
            }

            var currentUser = ((App)Application.Current).CurrentUser;
            // 这里显示评论时，保存头像路径到 Comment 模型中
            var comment = new Comment
            {
                PostId = _currentPost.Id,
                AuthorPhoneNumber = currentUser.PhoneNumber,
                Content = CommentEntry.Text,
                CreatedAt = DateTime.Now,
                AuthorNickname = currentUser.Nickname,
                AuthorProfilePhoto = currentUser.ProfilePhoto,
                FloorNumber = _comments.Count + 1 // 设置楼号
            };

            await _databaseService.SaveCommentAsync(comment);
            CommentEntry.Text = string.Empty;
            _currentCommentPage = 0;
            _hasMoreComments = true;
            LoadComments();
        }
        private async void OnCommentImageBindingContextChanged(object sender, EventArgs e)
        {
            var image = sender as Image;
            if (image?.BindingContext is Comment comment && !string.IsNullOrEmpty(comment.AuthorProfilePhoto))
            {
                // 使用 comment.AuthorProfilePhoto（存放头像路径字符串）调用 DatabaseService 获取 ImageSource
                image.Source = await _databaseService.LoadImageFromServerAsync(comment.AuthorProfilePhoto);
            }
        }
    }
}