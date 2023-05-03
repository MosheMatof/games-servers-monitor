using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceAgent;
using ServiceAgent.Contracts;
using Windows.Devices.Sensors;
using WPFClient.Activation;
using WPFClient.Contracts.Activation;
using WPFClient.Contracts.Models;
using WPFClient.Contracts.Services;
using WPFClient.Contracts.Views;
using WPFClient.Models;
using WPFClient.Services;
using WPFClient.ViewModels;
using WPFClient.Views;

namespace WPFClient;

public partial class App : Application
{
    private IHost _host;

    public T GetService<T>()
        where T : class
        => _host.Services.GetService(typeof(T)) as T;

    public App()
    {
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureAppConfiguration(c =>
                {
                    c.SetBasePath(appLocation);
                    c.AddInMemoryCollection();
                })
                .ConfigureServices(ConfigureServices)
                .Build();

       

        await _host.StartAsync();

        LiveCharts.Configure(config =>
                config
                    // registers SkiaSharp as the library backend
                    // REQUIRED unless you build your own
                    .AddSkiaSharp()

                    // adds the default supported types
                    // OPTIONAL but highly recommend
                    .AddDefaultMappers()

                    // select a theme, default is Light
                    // OPTIONAL
                    .AddDarkTheme()
                    //.AddLightTheme()

                // finally register your own mappers
                // you can learn more about mappers at:
                // ToDo add website link...
                //.HasMap<City>((city, point) =>
                //{
                //    point.PrimaryValue = city.Population;
                //    point.SecondaryValue = point.Context.Index;
                //})
                // .HasMap<Foo>( .... )
                // .HasMap<Bar>( .... )
                );
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // TODO: Register your services, viewmodels and pages here

        // App Host
        services.AddHostedService<ApplicationHostService>();

        // Activation Handlers
        services.AddSingleton<IActivationHandler, ToastNotificationActivationHandler>();

        // Service agent Services
        services.AddSingleton<IHubConnectionService, HubConnectionService>();
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<IEmulatorService, EmulatorService>();

        // Services
        services.AddSingleton<IWindowManagerService, WindowManagerService>();
        services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
        services.AddSingleton<ISystemService, SystemService>();
        services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
        services.AddSingleton<IPageService, PageService>();
        services.AddSingleton<INavigationService, NavigationService>();

        // Views and ViewModels
        services.AddSingleton<IShellWindow, ShellWindow>();
        services.AddSingleton<ShellViewModel>();

        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<DashboardPage>();

        services.AddSingleton<ServersViewModel>();
        services.AddSingleton<ServersPage>();

        services.AddSingleton<GamesViewModel>();
        services.AddSingleton<GamesPage>();

        services.AddSingleton<GamesDetailViewModel>();
        services.AddSingleton<GamesDetailPage>();

        services.AddSingleton<AnalyzeViewModel>();
        services.AddSingleton<AnalyzePage>();

        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<SettingsPage>();

        services.AddSingleton<EmulatorSetupViewModel>();
        services.AddSingleton<EmulatorSetupWindow>();

        services.AddSingleton<IShellDialogWindow, ShellDialogWindow>();
        services.AddSingleton<ShellDialogViewModel>();


        //Models
        services.AddSingleton<IGame, Game>();
        services.AddSingleton<IGameServer, GameServer>();
        services.AddSingleton<IServerUpdate, ServerUpdate>();
        services.AddSingleton<Emulator>();

        //add http client
        services.AddHttpClient<IGameService, GameService>(HttpClient =>
        {
            HttpClient.BaseAddress = new Uri("http://localhost:8770/");
        });

        // Configuration
        services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        _host = null;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // TODO: log and handle the exception
    }
}
