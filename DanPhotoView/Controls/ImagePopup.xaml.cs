using CommunityToolkit.Maui.Views;
using DanPhotoView.ViewModels;

namespace DanPhotoView.Controls;

public partial class ImagePopup : Popup
{
    public ImagePopup(ImageViewModel viewModel)
	{
		InitializeComponent();

        var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

        panelMain.WidthRequest = screenWidth * 0.8;
        panelMain.HeightRequest = screenHeight * 0.8;

        viewModel.PopupMaxHeight = screenHeight * 0.7;  
        viewModel.PopupMaxWidth = screenWidth * 0.8;    

        BindingContext = viewModel;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Close();
    }

}