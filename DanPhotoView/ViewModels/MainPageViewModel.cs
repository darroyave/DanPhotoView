using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DanPhotoView.Models;
using DanPhotoView.Service;
using System.Collections.ObjectModel;

namespace DanPhotoView.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly IFolderPicker? _folderPicker;

    [ObservableProperty]
    private int _thumbWidth = 200;

    [ObservableProperty]
    private int _thumbHeight = 200;

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

    [ObservableProperty]
    private string _rootPath = "c:\\App\\Photographers";

    private readonly IPopupService _popupService;
    private readonly IImageService _imageService;

    public MainPageViewModel(
        IPopupService popupService,
        IImageService imageService,
        IFolderPicker folderPicker)
    {
        _popupService = popupService;
        _imageService = imageService;
        _folderPicker = folderPicker;

        LoadDirectories();
    }

    public void LoadDirectories()
    {
        Directories.Clear();

        if (Directory.Exists(RootPath))
        {
            var path = Directory.GetDirectories(RootPath);

            foreach (var dir in path)
            {
                Directories.Add(new DirectoryItem() { Name = dir });
            }
        }
    }

    [RelayCommand]

    private async Task SelectedDirectoryAsync(DirectoryItem directory)
    {
        SelectedDirectory = directory;

        Images.Clear();

        LoadedImagesCount = 0;

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
                        var stream = File.OpenRead(file);

                        var thumbStream = _imageService.CreateThumbnail(stream, ThumbWidth, ThumbHeight);

                        return thumbStream;
                    }),
                    Path = file
                };

                MainThread.BeginInvokeOnMainThread(() => Images.Add(imageItem));
            });
        }

        LoadedImagesCount += remainingImages.Count;
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

    [RelayCommand]
    async Task PickFolder()
    {
        var result = await _folderPicker.PickAsync(default);
        if (result.IsSuccessful && result.Folder != null)
        {
            RootPath = result.Folder.Path;

            LoadDirectories();

            SelectedDirectory = null;
            Images.Clear();
            LoadedImagesCount = 0;
        }
    }

}
