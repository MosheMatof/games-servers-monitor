using System.Windows.Controls;

using WPFClient.ViewModels;

namespace WPFClient.Views;

public partial class AnalyzePage : Page
{
    public AnalyzePage(AnalyzeViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
