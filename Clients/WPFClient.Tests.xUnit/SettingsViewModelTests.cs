using Microsoft.Extensions.Options;

using Moq;
using ServiceAgent.Contracts;
using WPFClient.Contracts.Services;
using WPFClient.Models;
using WPFClient.ViewModels;
using WPFClient.Views;
using Xunit;

namespace WPFClient.Tests.XUnit;

public class SettingsViewModelTests
{
    public SettingsViewModelTests()
    {
    }

    [Fact]
    public void TestSettingsViewModel_SetCurrentTheme()
    {
        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        mockThemeSelectorService.Setup(mock => mock.GetCurrentTheme()).Returns(AppTheme.Light);
        var mockAppConfig = new Mock<IOptions<AppConfig>>();
        var mockSystemService = new Mock<ISystemService>();
        var mockApplicationInfoService = new Mock<IApplicationInfoService>();
        var mockEmulatorService = new Mock<IEmulatorService>();
        var mockEmulatorSetupWindow = new Mock<EmulatorSetupWindow>();
        var mockEmulator = new Mock<Emulator>(); 

        var settingsVm = new SettingsViewModel(mockAppConfig.Object, mockThemeSelectorService.Object, mockSystemService.Object, mockApplicationInfoService.Object,mockEmulatorService.Object,mockEmulatorSetupWindow.Object,mockEmulator.Object);
        settingsVm.OnNavigatedTo(null);

        Assert.Equal(AppTheme.Light, settingsVm.Theme);
    }

    [Fact]
    public void TestSettingsViewModel_SetCurrentVersion()
    {
        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        var mockAppConfig = new Mock<IOptions<AppConfig>>();
        var mockSystemService = new Mock<ISystemService>();
        var mockApplicationInfoService = new Mock<IApplicationInfoService>();
        var mockEmulatorService = new Mock<IEmulatorService>();
        var mockEmulatorSetupWindow = new Mock<EmulatorSetupWindow>();
        var mockEmulator = new Mock<Emulator>();
        var testVersion = new Version(1, 2, 3, 4);
        mockApplicationInfoService.Setup(mock => mock.GetVersion()).Returns(testVersion);

        var settingsVm = new SettingsViewModel(mockAppConfig.Object, mockThemeSelectorService.Object, mockSystemService.Object, mockApplicationInfoService.Object, mockEmulatorService.Object, mockEmulatorSetupWindow.Object, mockEmulator.Object);
        settingsVm.OnNavigatedTo(null);

        Assert.Equal($"WPFClient - {testVersion}", settingsVm.VersionDescription);
    }

    [Fact]
    public void TestSettingsViewModel_SetThemeCommand()
    {
        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        var mockAppConfig = new Mock<IOptions<AppConfig>>();
        var mockSystemService = new Mock<ISystemService>();
        var mockApplicationInfoService = new Mock<IApplicationInfoService>();
        var mockEmulatorService = new Mock<IEmulatorService>();
        var mockEmulatorSetupWindow = new Mock<EmulatorSetupWindow>();
        var mockEmulator = new Mock<Emulator>();

        var settingsVm = new SettingsViewModel(mockAppConfig.Object, mockThemeSelectorService.Object, mockSystemService.Object, mockApplicationInfoService.Object, mockEmulatorService.Object, mockEmulatorSetupWindow.Object, mockEmulator.Object);
        settingsVm.SetThemeCommand.Execute(AppTheme.Light.ToString());

        mockThemeSelectorService.Verify(mock => mock.SetTheme(AppTheme.Light));
    }
}
