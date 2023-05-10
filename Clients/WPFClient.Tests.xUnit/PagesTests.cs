using System.IO;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceAgent;
using ServiceAgent.Contracts;
using WPFClient.Contracts.Services;
using WPFClient.Models;
using WPFClient.Services;
using WPFClient.ViewModels;
using WPFClient.Views;

using Xunit;

namespace WPFClient.Tests.XUnit;

public class PagesTests
{
    private readonly IHost _host;

    public PagesTests()
    {
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => c.SetBasePath(appLocation))
            .ConfigureServices(ConfigureServices)
            .Build();
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // Core Services
        services.AddHttpClient();

        // Services
        services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
        services.AddSingleton<ISystemService, SystemService>();
        services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
        services.AddSingleton<IPageService, PageService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<IEmulatorService,EmulatorService>();

        // ViewModels
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ServersViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<GamesViewModel>();
        services.AddTransient<AnalyzeViewModel>();

        // Configuration
        services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
    }

    [Fact]
    public void TestSettingsViewModelCreation()
    {
        var vm = _host.Services.GetService(typeof(SettingsViewModel));
        Assert.NotNull(vm);
    }

    [Fact]
    public void TestGetSettingsPageType()
    {
        if (_host.Services.GetService(typeof(IPageService)) is IPageService pageService)
        {
            var pageType = pageService.GetPageType(typeof(SettingsViewModel).FullName);
            Assert.Equal(typeof(SettingsPage), pageType);
        }
        else
        {
            Assert.True(false, $"Can't resolve {nameof(IPageService)}");
        }
    }

    // TODO: Add tests for functionality you add to ServersViewModel.
    [Fact]
    public void TestServersViewModelCreation()
    {
        var vm = _host.Services.GetService(typeof(ServersViewModel));
        Assert.NotNull(vm);
    }

    [Fact]
    public void TestGetServersPageType()
    {
        if (_host.Services.GetService(typeof(IPageService)) is IPageService pageService)
        {
            var pageType = pageService.GetPageType(typeof(ServersViewModel).FullName);
            Assert.Equal(typeof(ServersPage), pageType);
        }
        else
        {
            Assert.True(false, $"Can't resolve {nameof(IPageService)}");
        }
    }

    // TODO: Add tests for functionality you add to MainViewModel.
    [Fact]
    public void TestMainViewModelCreation()
    {
        var vm = _host.Services.GetService(typeof(DashboardViewModel));
        Assert.NotNull(vm);
    }

    [Fact]
    public void TestGetMainPageType()
    {
        if (_host.Services.GetService(typeof(IPageService)) is IPageService pageService)
        {
            var pageType = pageService.GetPageType(typeof(DashboardViewModel).FullName);
            Assert.Equal(typeof(DashboardPage), pageType);
        }
        else
        {
            Assert.True(false, $"Can't resolve {nameof(IPageService)}");
        }
    }

    // TODO: Add tests for functionality you add to GamesViewModel.
    [Fact]
    public void TestGamesViewModelCreation()
    {
        var vm = _host.Services.GetService(typeof(GamesViewModel));
        Assert.NotNull(vm);
    }

    [Fact]
    public void TestGetGamesPageType()
    {
        if (_host.Services.GetService(typeof(IPageService)) is IPageService pageService)
        {
            var pageType = pageService.GetPageType(typeof(GamesViewModel).FullName);
            Assert.Equal(typeof(GamesPage), pageType);
        }
        else
        {
            Assert.True(false, $"Can't resolve {nameof(IPageService)}");
        }
    }

    // TODO: Add tests for functionality you add to AnalyzeViewModel.
    [Fact]
    public void TestAnalyzeViewModelCreation()
    {
        var vm = _host.Services.GetService(typeof(AnalyzeViewModel));
        Assert.NotNull(vm);
    }

    [Fact]
    public void TestGetAnalyzePageType()
    {
        if (_host.Services.GetService(typeof(IPageService)) is IPageService pageService)
        {
            var pageType = pageService.GetPageType(typeof(AnalyzeViewModel).FullName);
            Assert.Equal(typeof(AnalyzePage), pageType);
        }
        else
        {
            Assert.True(false, $"Can't resolve {nameof(IPageService)}");
        }
    }
}
