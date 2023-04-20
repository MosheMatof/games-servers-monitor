using System.Windows.Controls;
using WPFClient.ViewModels;

namespace WPFClient.Views;

public partial class GamesDetailPage : Page
{
    public GamesDetailPage(GamesDetailViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
