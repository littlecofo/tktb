using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Services;
using System;
using System.Threading.Tasks;

namespace SecondHandMarket
{
    public partial class ProfilePage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private User _currentUser;

        public ProfilePage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadUserProfile();
        }

        private async void LoadUserProfile()
        {
            string phoneNumber = ((App)Application.Current).CurrentUserPhoneNumber;
            _currentUser = ((App)Application.Current).CurrentUser;

            if (_currentUser != null)
            {
                // 通过服务端接口获取头像图片
                ProfilePhoto.Source = await _databaseService.LoadImageFromServerAsync($"/var/www/websecondhandapp/wwwroot/images/profile/{_currentUser.PhoneNumber}_profile_image.jpg");

                Nickname.Text = _currentUser.Nickname;
                Bio.Text = _currentUser.Bio;
                Experience.Text = GetLevelText(_currentUser.Experience);
                Experience.TextColor = GetLevelColor(_currentUser.Experience);
                ExperienceDetail.Text = GetExperienceDetail(_currentUser.Experience);
                Title.Text = $"称号: {_currentUser.Title}";
                Points.Text = $"积分: {_currentUser.Points}";
            }
        }

        private string GetLevelText(int experience)
        {
            if (experience < 0)
                return "Lv-1";
            else if (experience < 100)
                return "Lv0";
            else if (experience < 200)
                return "Lv1";
            else if (experience < 350)
                return "Lv2";
            else if (experience < 500)
                return "Lv3";
            else if (experience < 700)
                return "Lv4";
            else if (experience < 1000)
                return "Lv5";
            else
                return "Lv6";
        }

        private Color GetLevelColor(int experience)
        {
            if (experience < 0)
                return Colors.Gray;
            else if (experience < 100)
                return Colors.Green;
            else if (experience < 200)
                return Colors.Blue;
            else if (experience < 350)
                return Colors.Purple;
            else if (experience < 500)
                return Colors.Orange;
            else if (experience < 700)
                return Colors.Red;
            else if (experience < 1000)
                return Colors.Gold;
            else
                return Colors.Black;
        }

        private string GetExperienceDetail(int experience)
        {
            int nextLevelExperience = 0;
            if (experience < 0)
                nextLevelExperience = 0;
            else if (experience < 100)
                nextLevelExperience = 100;
            else if (experience < 200)
                nextLevelExperience = 200;
            else if (experience < 350)
                nextLevelExperience = 350;
            else if (experience < 500)
                nextLevelExperience = 500;
            else if (experience < 700)
                nextLevelExperience = 700;
            else if (experience < 1000)
                nextLevelExperience = 1000;
            else
                nextLevelExperience = 1500; // Lv6 之后的经验值

            return $"{experience}/{nextLevelExperience}";
        }

        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditProfilePage(_currentUser));
        }

        private async void OnMyProductsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyProductsPage());
        }

        private async void OnMyPostsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyPostsPage(_currentUser.PhoneNumber));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            ((App)Application.Current).IsLoggedIn = false;
            ((App)Application.Current).CurrentUserPhoneNumber = null;
            await Navigation.PopToRootAsync();
        }
    }
}