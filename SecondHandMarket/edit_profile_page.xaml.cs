using Microsoft.Maui.Controls;
using SecondHandMarket.Models;
using SecondHandMarket.Requests;
using SecondHandMarket.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

#if ANDROID
using Xamarin.Android.Net;
#endif

namespace SecondHandMarket
{
    public partial class EditProfilePage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private User _currentUser;
        private readonly HttpClient _httpClient;

        public EditProfilePage(User user)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _currentUser = user;

            HttpMessageHandler handler = CreateHttpMessageHandler();
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:5184/")
            };

            LoadUser();
        }

        private HttpMessageHandler CreateHttpMessageHandler()
        {
#if ANDROID
            // 使用 Android 平台的 AndroidClientHandler
            return new AndroidClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
#elif IOS
            // iOS 中建议使用 NSUrlSessionHandler，你也可以尝试 HttpClientHandler
            return new NSUrlSessionHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
#else
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
#endif
        }

        private void LoadUser()
        {
            ProfilePhoto.Source = new UriImageSource
            {
                Uri = new Uri($"{_httpClient.BaseAddress}images/profile/{_currentUser.PhoneNumber}_profile_image.jpg"),
                CachingEnabled = false
            };
            NicknameEntry.Text = _currentUser.Nickname;
            BioEntry.Text = _currentUser.Bio;
        }

        private async void OnPickPhotoClicked(object sender, EventArgs e)
        {
            var result = await MediaPicker.PickPhotoAsync();
            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                ProfilePhoto.Source = ImageSource.FromStream(() => stream);
                SavePhoto(result.FullPath);
            }
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            if (await CheckAndRequestCameraPermission() && await CheckAndRequestStoragePermission())
            {
                var result = await MediaPicker.CapturePhotoAsync();
                if (result != null)
                {
                    var stream = await result.OpenReadAsync();
                    ProfilePhoto.Source = ImageSource.FromStream(() => stream);
                    SavePhoto(result.FullPath);
                }
            }
            else
            {
                await DisplayAlert("权限不足", "需要相机和存储权限来拍照", "确定");
            }
        }
        private void SavePhoto(string photoPath)
        {
            _currentUser.ProfilePhoto = photoPath;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            string nickname = NicknameEntry.Text?.Trim() ?? "";
            string bio = BioEntry.Text?.Trim() ?? "";

            if (nickname.Length < 1 || nickname.Length > 8)
            {
                await DisplayAlert("错误", "昵称需为1到8个字之间", "确定");
                return;
            }

            if (bio.Length > 20 || bio.Length < 1)
            {
                await DisplayAlert("错误", "个人简介不能超过20个字，且不能为空", "确定");
                return;
            }

            // 上传头像文件
            if (!string.IsNullOrEmpty(_currentUser.ProfilePhoto) && File.Exists(_currentUser.ProfilePhoto))
            {
                var uploadSuccess = await UploadProfilePhotoAsync(_currentUser.PhoneNumber, _currentUser.ProfilePhoto);
                if (!uploadSuccess)
                {
                    await DisplayAlert("错误", "头像上传失败", "确定");
                    return;
                }
            }

            var request = new UpdateUserProfileRequest
            {
                PhoneNumber = _currentUser.PhoneNumber,
                Nickname = nickname,
                Bio = bio,
                ProfilePhoto = $"{_currentUser.PhoneNumber}_profile_image.jpg" // 更新头像文件名
            };

            var success = await _databaseService.UpdateUserProfileAsync(request);
            if (success)
            {
                await DisplayAlert("提示", "资料已保存", "确定");
                ((App)Application.Current).CurrentUser.Nickname = nickname;
                ((App)Application.Current).CurrentUser.Bio = bio;
                ((App)Application.Current).CurrentUser.ProfilePhoto = request.ProfilePhoto;
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("错误", "资料保存失败", "确定");
            }
        }

        private async Task<bool> UploadProfilePhotoAsync(string phoneNumber, string photoPath)
        {
            try
            {
                using (var stream = File.OpenRead(photoPath))
                {
                    var form = new MultipartFormDataContent();
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    form.Add(streamContent, "file", $"{phoneNumber}_profile_image.jpg");
                    var response = await _httpClient.PostAsync("api/Users/uploadProfilePhoto", form);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("上传头像异常：" + ex.Message);
                return false;
            }
        }
        private async Task<bool> CheckAndRequestCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }
            return status == PermissionStatus.Granted;
        }

        private async Task<bool> CheckAndRequestStoragePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }
            return status == PermissionStatus.Granted;
        }
    }
}