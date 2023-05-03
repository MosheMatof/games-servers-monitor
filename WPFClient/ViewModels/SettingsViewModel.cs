using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Microsoft.Extensions.Options;
using ServiceAgent.Contracts;
using WPFClient.Contracts.Services;
using WPFClient.Contracts.ViewModels;
using WPFClient.Models;
using WPFClient.Views;

namespace WPFClient.ViewModels;

// TODO: Change the URL for your privacy policy in the appsettings.json file, currently set to https://YourPrivacyUrlGoesHere
public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly AppConfig _appConfig;
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ISystemService _systemService;
    private readonly IApplicationInfoService _applicationInfoService;
    private readonly IEmulatorService _emulatorService;
    private readonly EmulatorSetupWindow _emulatorSetupWindow;
    private readonly Emulator _emulator;
    private string _versionDescription;
    private ICommand _setThemeCommand;
    private ICommand _privacyStatementCommand;

    [ObservableProperty]
    private AppTheme theme;
    [ObservableProperty]
    private string versionDescription;
    [ObservableProperty]
    private bool isInit;
    [ObservableProperty]
    private bool isRunning;

    public ICommand SetThemeCommand => _setThemeCommand ?? (_setThemeCommand = new RelayCommand<string>(OnSetTheme));

    public ICommand PrivacyStatementCommand => _privacyStatementCommand ?? (_privacyStatementCommand = new RelayCommand(OnPrivacyStatement));

    public SettingsViewModel(IOptions<AppConfig> appConfig, IThemeSelectorService themeSelectorService, ISystemService systemService, IApplicationInfoService applicationInfoService,IEmulatorService emulatorService,EmulatorSetupWindow emulatorSetupWindow,Emulator emulator)
    {
        _appConfig = appConfig.Value;
        _themeSelectorService = themeSelectorService;
        _systemService = systemService;
        _applicationInfoService = applicationInfoService;
        _emulatorService = emulatorService;
        _emulatorSetupWindow = emulatorSetupWindow;
        _emulator = emulator;
    }

    public void OnNavigatedTo(object parameter)
    {
        VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
        Theme = _themeSelectorService.GetCurrentTheme();
        IsInit = _emulator.IsInit;
        IsRunning = _emulator.IsRunning;
    }

    public void OnNavigatedFrom()
    {
    }

    private void OnSetTheme(string themeName)
    {
        var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
        _themeSelectorService.SetTheme(theme);
    }

    private void OnPrivacyStatement()
        => _systemService.OpenInWebBrowser(_appConfig.PrivacyStatement);

    [RelayCommand]
    private async Task StartResume()
    {
        if (IsRunning) { return; }
        var window = App.Current.MainWindow as MetroWindow;
        if (IsInit)
        {
            var success = await _emulatorService.ResumeEmulatorAsync();
            if (success)
                await window.ShowMessageAsync("Success", "Emularor resume successfuly.");
            else
                await window.ShowMessageAsync("Error", "Emularor resume failed.");
            IsRunning = success;
            _emulator.IsRunning = success;
        }
        else
        {
            var dialogResult = await window.ShowChildWindowAsync<bool?>(_emulatorSetupWindow);
            if (dialogResult == null)
                await window.ShowMessageAsync("Error", "Emulator setup failed.");
            if (dialogResult == false)
                await window.ShowMessageAsync("Attention", "the Emulator setup didn't complete, try again.");
            if (dialogResult == true)
            {
                await window.ShowMessageAsync("Success", "Emulator setup completed.");
                IsInit = true;
                _emulator.IsInit = true;
                IsRunning = true;
                _emulator.IsRunning = true;
            }
        }
    }
    [RelayCommand]
    private async Task Stop()
    {
        if (!IsRunning) { return; }
        var success = await _emulatorService.StopEmulatorAsync();
        var window = App.Current.MainWindow as MetroWindow;
        await window.ShowMessageAsync("Success", "Emulator stop.");
        IsRunning = false;
        _emulator.IsRunning = false;
    }
}
