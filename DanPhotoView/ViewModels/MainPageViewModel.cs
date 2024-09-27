using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DanPhotoView.Models;
using System.Collections.ObjectModel;

namespace DanPhotoView.ViewModels;

public partial class MainPageViewModel: ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ImageItem> _images = [];

    [ObservableProperty]
    public ObservableCollection<DirectoryItem> _directories = [];

    public MainPageViewModel()
    {
        LoadDirectories();
    }

    public void LoadDirectories()
    {
        string rootPath = "c:\\Design\\Photographers"; 

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
        LoadImages(directory.Name);
    }

    public void LoadImages(string folderPath)
    {
        Images.Clear();

        if (Directory.Exists(folderPath))
        {
            var imageFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
               .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                        || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                        || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase));

            foreach (var file in imageFiles)
            {
                var imageItem = new ImageItem
                {
                    ImageSource = ImageSource.FromFile(file)
                };

                Images.Add(imageItem);
            }
        }
    }
}
