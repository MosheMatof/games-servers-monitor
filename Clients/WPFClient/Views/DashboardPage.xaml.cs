using System.Windows.Controls;

using WPFClient.ViewModels;

namespace WPFClient.Views;

public partial class DashboardPage : Page
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
