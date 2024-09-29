using CommunityToolkit.Maui.Views;
using DanPhotoView.ViewModels;

namespace DanPhotoView.Controls;

public partial class ImagePopup : Popup
{
	public ImagePopup(ImageViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
    }
}