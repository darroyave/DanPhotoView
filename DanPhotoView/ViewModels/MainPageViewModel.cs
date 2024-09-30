using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DanPhotoView.Models;
using DanPhotoView.Service;
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

    private readonly IPopupService _popupService;
    private readonly IImageService _imageService;

    public MainPageViewModel(
        IPopupService popupService, 
        IImageService imageService)
    {
        _popupService = popupService;
        _imageService = imageService;

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

                        var thumbStream = _imageService.CreateThumbnail(stream, 100, 100); 

                        return thumbStream;
                    }),
                    Path = file
                };

                MainThread.BeginInvokeOnMainThread(() => Images.Add(imageItem));
            });
        }

        LoadedImagesCount += ImagesBatchSize;
    }
    
    [RelayCommand]
    private void ShowImagePopup(ImageItem imageItem)
    {
        if (imageItem != null)
        {
            _popupService.ShowPopup<ImageViewModel>(
                onPresenting: viewModel => viewModel.ShowInfo(imageItem.Path));
        }
    }
}
