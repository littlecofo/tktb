using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Services;
using System;

namespace SecondHandMarket
{
    public partial class EditPostPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private Post _currentPost;

        public EditPostPage(Post post)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _currentPost = post;
            LoadPost();
        }

        private void LoadPost()
        {
            TitleEntry.Text = _currentPost.Title;
            CategoryPicker.SelectedItem = _currentPost.Category;
            ContentEditor.Text = _currentPost.Content;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // 检查标题长度
            if (TitleEntry.Text.Length > 15)
            {
                await DisplayAlert("错误", "标题不能超过15个字符", "确定");
                return;
            }

            // 检查分类是否为空
            if (CategoryPicker.SelectedItem == null)
            {
                await DisplayAlert("错误", "请选择一个分类", "确定");
                return;
            }

            _currentPost.Title = TitleEntry.Text;
            _currentPost.Category = CategoryPicker.SelectedItem?.ToString();
            _currentPost.Content = ContentEditor.Text;
            _currentPost.CreatedAt = DateTime.Now;
            _currentPost.AuthorNickname = "匿名用户";

            await _databaseService.SavePostAsync(_currentPost);

            await DisplayAlert("提示", "帖子已发布", "确定");
            await Navigation.PopAsync();
        }

        private void OnBoldClicked(object sender, EventArgs e)
        {
            ContentEditor.Text += "<b></b>";
        }

        private void OnItalicClicked(object sender, EventArgs e)
        {
            ContentEditor.Text += "<i></i>";
        }

        private void OnUnderlineClicked(object sender, EventArgs e)
        {
            ContentEditor.Text += "<u></u>";
        }
    }
}