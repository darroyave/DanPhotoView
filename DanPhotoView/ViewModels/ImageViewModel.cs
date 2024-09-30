using CommunityToolkit.Mvvm.ComponentModel;

namespace DanPhotoView.ViewModels;

public partial class ImageViewModel : ObservableObject
{

    [ObservableProperty]
    private string? _imagePath;

    [ObservableProperty]
    private double? _popupMaxHeight;

    [ObservableProperty]
    private double? _popupMaxWidth;

    public void ShowInfo(string imagePath)
    {
        ImagePath = imagePath;
    }

}
