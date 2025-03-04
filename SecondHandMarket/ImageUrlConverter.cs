using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using SecondHandMarket.Services;

namespace SecondHandMarket
{
    public class ImageUrlConverter : IValueConverter
    {
        private readonly DatabaseService _databaseService;

        public ImageUrlConverter()
        {
            _databaseService = new DatabaseService();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imagePath && !string.IsNullOrEmpty(imagePath))
            {
                var taskCompletionSource = new TaskCompletionSource<ImageSource>();
                LoadImageAsync(imagePath, taskCompletionSource);
                return taskCompletionSource.Task;
            }
            return null;
        }

        private async void LoadImageAsync(string imagePath, TaskCompletionSource<ImageSource> taskCompletionSource)
        {
            var imageSource = await _databaseService.LoadImageFromServerAsync(imagePath);
            taskCompletionSource.SetResult(imageSource);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}