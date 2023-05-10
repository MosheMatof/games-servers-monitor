using System.Windows.Controls;

using WPFClient.ViewModels;

namespace WPFClient.Views;

public partial class GamesPage : Page
{
    public GamesPage(GamesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
