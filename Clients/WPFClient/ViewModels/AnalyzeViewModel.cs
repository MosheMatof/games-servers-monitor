using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using WPFClient.Contracts.ViewModels;


namespace WPFClient.ViewModels;

public class AnalyzeViewModel : ObservableObject, INavigationAware
{


    public AnalyzeViewModel()
    {
    }

    public void OnNavigatedTo(object parameter)
    {

    }

    public void OnNavigatedFrom()
    {
    }
}
