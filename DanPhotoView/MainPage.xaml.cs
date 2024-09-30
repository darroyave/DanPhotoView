using DanPhotoView.ViewModels;
using Microsoft.Maui.Controls;

namespace DanPhotoView;

public partial class MainPage : ContentPage
{
    private const double ItemWidth = 100;

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

        SizeChanged += MainPage_SizeChanged;
    }

    private void MainPage_SizeChanged(object? sender, EventArgs e)
    {
        var screenWidth = Width;

        // Calcular el nuevo Span
        int span = CalculateSpan(screenWidth);

        // Actualizar el Span del GridItemsLayout
        if (gridItemsLayout != null)
        {
            gridItemsLayout.Span = span;
        }
    }

    private int CalculateSpan(double screenWidth)
    {
        double totalPadding = 20; 
        double availableWidth = screenWidth - totalPadding;

        int span = (int)Math.Floor(availableWidth / ItemWidth);
        return Math.Max(span, 1);
    }
}
