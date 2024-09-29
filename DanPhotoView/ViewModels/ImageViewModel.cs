using CommunityToolkit.Mvvm.ComponentModel;

namespace DanPhotoView.ViewModels;

public partial class ImageViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _imagePath;

    public void ShowInfo(string imagePath)
    {
        ImagePath = imagePath;
    }

}
