using DanPhotoView.ViewModels;

namespace DanPhotoView;

public partial class MainPage : ContentPage
{

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

}
