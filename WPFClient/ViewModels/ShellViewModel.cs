using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlzEx;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using MahApps.Metro.SimpleChildWindow;
using Windows.Graphics.Printing3D;
using WPFClient.Contracts.Services;
using WPFClient.Properties;
using WPFClient.Views;

namespace WPFClient.ViewModels;

public class ShellViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IWindowManagerService _windowManagerService;
    private readonly EmulatorSetupWindow _emulatorSetupWindow;

    private HamburgerMenuItem _selectedMenuItem;
    private HamburgerMenuItem _selectedOptionsMenuItem;
    private RelayCommand _goBackCommand;
    private ICommand _menuItemInvokedCommand;
    private ICommand _optionsMenuItemInvokedCommand;
    private ICommand _loadedCommand;
    private ICommand _unloadedCommand;

    public HamburgerMenuItem SelectedMenuItem
    {
        get { return _selectedMenuItem; }
        set { SetProperty(ref _selectedMenuItem, value); }
    }

    public HamburgerMenuItem SelectedOptionsMenuItem
    {
        get { return _selectedOptionsMenuItem; }
        set { SetProperty(ref _selectedOptionsMenuItem, value); }
    }

    // TODO: Change the icons and titles for all HamburgerMenuItems here.
    public ObservableCollection<HamburgerMenuItem> MenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
    {
        new HamburgerMenuIconItem() { Label = Resources.ShellMainPage, Icon = PackIconMaterialKind.Home, TargetPageType = typeof(DashboardViewModel), },
        new HamburgerMenuIconItem() { Label = Resources.ShellServersPage, Icon = PackIconMaterialKind.ServerNetwork, TargetPageType = typeof(ServersViewModel) },
        new HamburgerMenuIconItem() { Label = Resources.ShellGamesPage, Icon = PackIconMaterialKind.GamepadCircle, TargetPageType = typeof(GamesViewModel) },
        new HamburgerMenuIconItem() { Label = Resources.ShellAnalyzePage, Icon = PackIconMaterialKind.Graph, TargetPageType = typeof(AnalyzeViewModel) },
    };

    public ObservableCollection<HamburgerMenuItem> OptionMenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
    {
        new HamburgerMenuIconItem() { Label = Resources.ShellSettingsPage, Icon = PackIconMaterialKind.SettingsHelper, TargetPageType = typeof(SettingsViewModel) }
    };

    public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(OnGoBack, CanGoBack));

    public ICommand MenuItemInvokedCommand => _menuItemInvokedCommand ?? (_menuItemInvokedCommand = new RelayCommand(OnMenuItemInvoked));

    public ICommand OptionsMenuItemInvokedCommand => _optionsMenuItemInvokedCommand ?? (_optionsMenuItemInvokedCommand = new RelayCommand(OnOptionsMenuItemInvoked));

    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(OnUnloaded));

    public ShellViewModel(INavigationService navigationService, IWindowManagerService windowManagerService, EmulatorSetupWindow emulatorSetupWindow)
    {
        _navigationService = navigationService;
        _windowManagerService = windowManagerService;
        _emulatorSetupWindow = emulatorSetupWindow;
    }

    private async void OnLoaded()
    {
        _navigationService.Navigated += OnNavigated;

        var window = App.Current.MainWindow as MetroWindow;
        var dialogResult = await window.ShowChildWindowAsync<bool?>(_emulatorSetupWindow);
        if (dialogResult == null)
            await window.ShowMessageAsync("Error", "Emulator setup failed.");
        if (dialogResult == false)
            await window.ShowMessageAsync("Attention", "the Emulator setup didn't complete, you can set it later in the stting, in order to see all the data and grapgs.");
        if(dialogResult == true)
            await window.ShowMessageAsync("Success", "Emulator setup completed.");
    }

    private void OnUnloaded()
    {
        _navigationService.Navigated -= OnNavigated;
    }

    private bool CanGoBack()
        => _navigationService.CanGoBack;

    private void OnGoBack()
        => _navigationService.GoBack();

    private void OnMenuItemInvoked()
        => NavigateTo(SelectedMenuItem.TargetPageType);

    private void OnOptionsMenuItemInvoked()
        => NavigateTo(SelectedOptionsMenuItem.TargetPageType);

    private void NavigateTo(Type targetViewModel)
    {
        if (targetViewModel != null)
        {
            _navigationService.NavigateTo(targetViewModel.FullName);
        }
    }

    private void OnNavigated(object sender, string viewModelName)
    {
        var item = MenuItems
                    .OfType<HamburgerMenuItem>()
                    .FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
        if (item != null)
        {
            SelectedMenuItem = item;
        }
        else
        {
            SelectedOptionsMenuItem = OptionMenuItems
                    .OfType<HamburgerMenuItem>()
                    .FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
        }

        GoBackCommand.NotifyCanExecuteChanged();
    }
}
