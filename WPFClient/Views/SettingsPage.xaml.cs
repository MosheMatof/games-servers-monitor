using System.Windows.Controls;

using WPFClient.ViewModels;

namespace WPFClient.Views;

public partial class SettingsPage : Page
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
