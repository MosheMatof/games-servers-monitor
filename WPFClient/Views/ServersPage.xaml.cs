using System.Windows.Controls;

using WPFClient.ViewModels;

namespace WPFClient.Views;

public partial class ServersPage : Page
{
    public ServersPage(ServersViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
