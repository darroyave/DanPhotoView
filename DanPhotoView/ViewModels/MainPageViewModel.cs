using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DanPhotoView.Models;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace DanPhotoView.ViewModels;

public partial class MainPageViewModel: ObservableObject
{
    [ObservableProperty]
    private int _loadedImagesCount = 0;

    [ObservableProperty]
    private int _imagesBatchSize = 50;

    [ObservableProperty]
    private ObservableCollection<ImageItem> _images = [];

    [ObservableProperty]
    private ObservableCollection<DirectoryItem> _directories = [];

    [ObservableProperty]
    private DirectoryItem? selectedDirectory;

    public MainPageViewModel()
    {
        LoadDirectories();
    }

    public void LoadDirectories()
    {
        string rootPath = "c:\\App\\Photographers"; 

        if (Directory.Exists(rootPath))
        {
            var path = Directory.GetDirectories(rootPath);

            foreach (var dir in path)
            {
                Directories.Add(new DirectoryItem() { Name = dir});
            }
        }
    }

    [RelayCommand]

    private async Task SelectedDirectoryAsync(DirectoryItem directory)
    {
        SelectedDirectory = directory;

        Images.Clear();

        await LoadImages(directory.Name);
    }

    [RelayCommand]
    private async Task RemainingItems()
    {
        if (SelectedDirectory != null)
        {
            await LoadImages(SelectedDirectory.Name);
        }
    }

    public async Task LoadImages(string folderPath)
    {
        if (!Directory.Exists(folderPath)) return;

        var allImageFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
            .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                        || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                        || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
             .ToList();

        var remainingImages = allImageFiles.Skip(LoadedImagesCount).Take(ImagesBatchSize).ToList();

        if (!remainingImages.Any()) return;

        foreach (var file in remainingImages)
        {
            await Task.Run(() =>
            {
                var imageItem = new ImageItem
                {
                    ImageSource = ImageSource.FromStream(() =>
                    {
                        using var stream = File.OpenRead(file);

                        var resizedStream = ResizeImage(stream, 100, 100); 

                        return resizedStream;
                    })
                };

                MainThread.BeginInvokeOnMainThread(() => Images.Add(imageItem));
            });
        }

        LoadedImagesCount += ImagesBatchSize;
    }
    private Stream ResizeImage(Stream inputStream, int targetWidth, int targetHeight)
    {
        using var original = SKBitmap.Decode(inputStream);

        float widthRatio = (float)targetWidth / original.Width;
        float heightRatio = (float)targetHeight / original.Height;
        float scale = Math.Min(widthRatio, heightRatio); 

        int finalWidth = (int)(original.Width * scale);
        int finalHeight = (int)(original.Height * scale);

        var resized = new SKBitmap(finalWidth, finalHeight);

        using (var canvas = new SKCanvas(resized))
        {
            var paint = new SKPaint
            {
                FilterQuality = SKFilterQuality.High 
            };

            canvas.DrawBitmap(original, new SKRect(0, 0, finalWidth, finalHeight), paint);
        }
    
        var image = SKImage.FromBitmap(resized);

        var outputStream = new MemoryStream();

        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
        {
            data.SaveTo(outputStream);
        }

        outputStream.Position = 0;

        return outputStream;
    }
}
