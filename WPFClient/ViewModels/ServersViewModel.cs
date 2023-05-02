using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using ServiceAgent.Contracts;
using WPFClient.Contracts.ViewModels;
using BL.BE;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.Kernel;
using LiveChartsCore.Easing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Measure;
using CommunityToolkit.Mvvm.Messaging;
using WPFClient.Messaging;
using WPFClient.Contracts.Models;
using System.Windows;
using WPFClient.Contracts.Services;
using WPFClient.Models;

namespace WPFClient.ViewModels;

public partial class ServersViewModel : ObservableRecipient, INavigationAware
{
    private readonly IHubConnectionService _hubConnectionService;
    private readonly IGameServer _gameServerModel;
    private readonly IServerUpdate _serverUpdateModel;
    private readonly INavigationService _navigationService;
    private readonly IGame _gameModel;
    private readonly Emulator _emulator;

    private bool isInitialized = false;

    [ObservableProperty]
    private ObservableCollection<BEGameServer> gameServers;
    private ObservableCollection<BEServerUpdate> serverUpdates;
    [ObservableProperty]
    private BEGameServer selectedGameServer;

    #region Gamers graph properties
    [ObservableProperty]
    private ObservableCollection<ISeries> gamersGraphData;

    private ColumnSeries<DateTimePoint> gamersNumColumn;
    private ColumnSeries<DateTimePoint> highScoreColumn;
    private ColumnSeries<DateTimePoint> avgScoreColumn;

    public Axis[] XAxesGamersGraph { get; set; } =
    {
        new Axis
        {
            Labeler = value => value > 0? new DateTime((long) value).ToString("hh:mm:ss")
            : new DateTime(0).ToString("hh:mm:ss"),
            LabelsRotation = 30,
            UnitWidth = TimeSpan.FromSeconds(10).Ticks,
            MinStep = TimeSpan.FromSeconds(10).Ticks,
        }
    };
    #endregion

    #region Health graph properties
    [ObservableProperty]
    private ObservableCollection<ISeries> healthGraphData;

    private ColumnSeries<float> cpuSpeedColumn;
    private ColumnSeries<float> cpuTempratureColumn;
    private ColumnSeries<float> memoryColumn;
    #endregion

    #region Gauges properties
    [ObservableProperty]
    private IEnumerable<ISeries> cpuSpeedGaugeSeries;
    [ObservableProperty]
    private IEnumerable<ISeries> cpuTempratureGaugeSeries;
    [ObservableProperty]
    private IEnumerable<ISeries> memoryGaugeSeries;
    [ObservableProperty]
    private IEnumerable<ISeries> gamersNumGaugeSeries;
    [ObservableProperty]
    private IEnumerable<ISeries> highScoreGaugeSeries;
    [ObservableProperty]
    private IEnumerable<ISeries> avgScoreGaugeSeries;

    ObservableValue CpuGaugeValue { get; set; } = new() { Value = 0 };
    ObservableValue CpuTempratureGaugeValue { get; set; } = new() { Value = 0 };
    ObservableValue MemoryGaugeValue { get; set; } = new() { Value = 0 };
    ObservableValue GamersNumGaugeValue { get; set; } = new() { Value = 0 };
    ObservableValue HighScoreGaugeValue { get; set; } = new() { Value = 0 };
    ObservableValue AvgScoreGaugeValue { get; set; } = new() { Value = 0 };

    #endregion

    public ServersViewModel(IHubConnectionService hubConnectionService, IGameServer gameServer, INavigationService navigationService, IServerUpdate serverUpdate, IGame game, Emulator emulator)
    {
        _hubConnectionService = hubConnectionService;
        _gameServerModel = gameServer;
        _serverUpdateModel = serverUpdate;
        _navigationService = navigationService;
        _gameModel = game;
        _emulator = emulator;

        serverUpdates = new();
        gameServers = new();
        serverUpdates.CollectionChanged += ServerUpdates_CollectionChanged;

        selectedGameServer = gameServers.FirstOrDefault();
        initGraphs();
        initGauges();
        //generateRandomData();
        //RegisterToGameMessage();
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (!isInitialized)
        {
            await Task.Run(async () =>
            {
                await LoadGameServersAsync();
            });
            isInitialized = true;
        }

        await UpdateGameServersAsync();

        if (GameServers != null && GameServers.Count > 0 && SelectedGameServer != null)
        {
            await LoadServerUpdatesAsync();
        }
    }

    public void OnNavigatedFrom()
    {
        _hubConnectionService.StopLiveUpdate();
        StopUpdateGameServersAsync();
    }

    #region Load data
    private async Task LoadGameServersAsync()
    {
        _hubConnectionService.SetTimeout(_emulator.intervalTime + 60);
        await foreach (var server in _hubConnectionService.GetAllAsync<BEGameServer>("GetServers"))
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GameServers.Add(server);
                _gameServerModel.GameServers.Add(server);
            });
        }
    }

    private async Task LoadServerUpdatesAsync()
    {
        var lastUpdate = _serverUpdateModel[SelectedGameServer.Id].LastOrDefault();
        var lastUpdateTime = lastUpdate?.TimeStamp ?? DateTime.MinValue;
        await foreach (var serverUpdate in _hubConnectionService.GetAllLiveAsync<BEServerUpdate>("GetServerUpdates", handleLiveServerUpdate, SelectedGameServer.Id, lastUpdateTime))
        {
            serverUpdates.Add(serverUpdate);
            _serverUpdateModel[serverUpdate.ServerId].Add(serverUpdate);
        }
    }

    private void handleLiveServerUpdate(BEServerUpdate liveServerUpdate)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            serverUpdates.Add(liveServerUpdate);
            _serverUpdateModel[liveServerUpdate.ServerId].Add(liveServerUpdate);
        });
    }

    private async Task UpdateGameServersAsync()
    {
        //TODO
    }

    private void StopUpdateGameServersAsync()
    {
        //TODO
    }

    #endregion

    private void ServerUpdates_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
            foreach (BEServerUpdate serverUpdate in e.NewItems)
            {
                if (serverUpdate.ServerId == SelectedGameServer.Id)
                {
                    updateGraphs(serverUpdate);
                    updateGauges(serverUpdate);
                    SelectedGameServer.CurrentPlayers = serverUpdate.CurrentPlayers;
                    SelectedGameServer.CpuSpeed = serverUpdate.CpuSpeed;
                    SelectedGameServer.CpuTemperature = serverUpdate.CpuTemperature;
                    SelectedGameServer.IsRunning = serverUpdate.IsRunning;
                    SelectedGameServer.MemoryUsage = serverUpdate.MemoryUsage;
                    SelectedGameServer.AvgScore = serverUpdate.AvgScore;
                }
            }
        }
    }

    partial void OnSelectedGameServerChanged(BEGameServer value)
    {
        Task.Run(async () =>
        {
            serverUpdates = new(_serverUpdateModel[value.Id]);
            serverUpdates.CollectionChanged += ServerUpdates_CollectionChanged;
            updateGraphs(serverUpdates);
            updateGauges(serverUpdates.LastOrDefault());

            await LoadServerUpdatesAsync();
        });
    }

    private void ServerUpdates_CollectionChanged1(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        throw new NotImplementedException();
    }


    #region Gauges methods

    private void initGauges()
    {
        CpuSpeedGaugeSeries = new GaugeBuilder()
        .WithLabelsSize(30)
        .WithInnerRadius(50)
        .WithBackgroundInnerRadius(50)
        .WithBackground(new SolidColorPaint(new SKColor(100, 181, 246, 90)))
        .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
        .AddValue(CpuGaugeValue, "Cpu speed", SKColors.YellowGreen, SKColors.Red) // defines the value and the color 
        .BuildSeries();

        CpuTempratureGaugeSeries = new GaugeBuilder()
       .WithLabelsSize(30)
       .WithInnerRadius(50)
       .WithBackgroundInnerRadius(50)
        .WithBackground(new SolidColorPaint(new SKColor(100, 181, 246, 90)))
        .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
        .AddValue(CpuTempratureGaugeValue, "Cpu temprature", SKColors.YellowGreen, SKColors.Red) // defines the value and the color 
       .BuildSeries();

        MemoryGaugeSeries = new GaugeBuilder()
       .WithLabelsSize(30)
       .WithInnerRadius(50)
       .WithBackgroundInnerRadius(50)
        .WithBackground(new SolidColorPaint(new SKColor(100, 181, 246, 90)))
        .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
        .AddValue(MemoryGaugeValue, "Memory", SKColors.YellowGreen, SKColors.Red) // defines the value and the color 
       .BuildSeries();

        GamersNumGaugeSeries = new GaugeBuilder()
        .WithLabelsSize(30)
        .WithInnerRadius(50)
        .WithBackgroundInnerRadius(50)
        .WithBackground(new SolidColorPaint(new SKColor(100, 181, 246, 90)))
        .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
        .AddValue(GamersNumGaugeValue, "Number of gamers", SKColors.YellowGreen, SKColors.Red) // defines the value and the color 
        .BuildSeries();

        HighScoreGaugeSeries = new GaugeBuilder()
       .WithLabelsSize(30)
       .WithInnerRadius(50)
       .WithBackgroundInnerRadius(50)
        .WithBackground(new SolidColorPaint(new SKColor(100, 181, 246, 90)))
        .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
        .AddValue(HighScoreGaugeValue, "High score", SKColors.YellowGreen, SKColors.Red) // defines the value and the color 
       .BuildSeries();

        AvgScoreGaugeSeries = new GaugeBuilder()
       .WithLabelsSize(30)
       .WithInnerRadius(50)
       .WithBackgroundInnerRadius(50)
        .WithBackground(new SolidColorPaint(new SKColor(100, 181, 246, 90)))
        .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
        .AddValue(AvgScoreGaugeValue, "Average score", SKColors.YellowGreen, SKColors.Red) // defines the value and the color 
       .BuildSeries();
    }

    private void updateGauges(BEServerUpdate serverUpdate)
    {
        CpuGaugeValue.Value = Math.Round(SelectedGameServer.CpuSpeed, 1); ;
        CpuTempratureGaugeValue.Value = Math.Round(SelectedGameServer.CpuTemperature, 1);
        MemoryGaugeValue.Value = Math.Round(SelectedGameServer.MemoryUsage, 1);
        GamersNumGaugeValue.Value = SelectedGameServer.CurrentPlayers;
        HighScoreGaugeValue.Value = Math.Round(SelectedGameServer.HighScore, 1);
        AvgScoreGaugeValue.Value = Math.Round(SelectedGameServer.AvgScore, 1);
    }

    #endregion

    #region Gamers Graph Area
    private void initGraphs()
    {
        // Define the data for the graph
        var gamersNumData = new List<DateTimePoint>();
        var highScoreData = new List<DateTimePoint>();
        var avgScoreData = new List<DateTimePoint>();
        var cpuSpeedData = new List<float>();
        var cpuTempratureData = new List<float>();
        var memoryUsageData = new List<float>();

        // Create the gamersGraphData for the graph
        gamersNumColumn = new ColumnSeries<DateTimePoint>
        {
            Name = "Number of Gamers",
            TooltipLabelFormatter = (chartPoint) =>
                $"Number of Gamers {chartPoint.PrimaryValue}",
            Values = gamersNumData,
            Stroke = null,
            Padding = TimeSpan.FromSeconds(10).Ticks,
        };

        highScoreColumn = new ColumnSeries<DateTimePoint>
        {
            Name = "High Score",
            TooltipLabelFormatter = (chartPoint) =>
                $"High Score {chartPoint.PrimaryValue}",
            Values = highScoreData,
            Stroke = null,
            Padding = 20,
        };

        avgScoreColumn = new ColumnSeries<DateTimePoint>
        {
            Name = "Average Score",
            TooltipLabelFormatter = (chartPoint) =>
                $"Average Score {chartPoint.PrimaryValue}",
            Values = avgScoreData,
            Stroke = null,
            Padding = 20,
        };

        cpuSpeedColumn = new ColumnSeries<float>
        {
            Name = "CPU Speed",
            Values = cpuSpeedData,
            Stroke = null,
            Padding = 2
        };

        cpuTempratureColumn = new ColumnSeries<float>
        {
            Name = "CPU Temprature",
            Values = cpuTempratureData,
            Stroke = null,
            Padding = 2,
            Fill = new LinearGradientPaint(
                new[] { SKColors.YellowGreen, SKColors.Red },
                        new SKPoint(0.5f, 0), new SKPoint(0.5f, 1))
        }; 

        memoryColumn = new ColumnSeries<float>
        {
            Name = "Memory Usage",
            Values = memoryUsageData,
            Stroke = null,
            Padding = 2
        };

        // Subscribe to the PointMeasured event to add animation
        gamersNumColumn.PointMeasured += OnPointMeasured;
        highScoreColumn.PointMeasured += OnPointMeasured;
        avgScoreColumn.PointMeasured += OnPointMeasured;
        //cpuSpeedColumn.PointMeasured += OnPointMeasured;
        //cpuTempratureColumn.PointMeasured += OnPointMeasured;
        //memoryColumn.PointMeasured += OnPointMeasured;

        //generates the graphs from the columns
        GamersGraphData = new ObservableCollection<ISeries> { gamersNumColumn, highScoreColumn, avgScoreColumn };
        HealthGraphData = new ObservableCollection<ISeries> { cpuSpeedColumn, cpuTempratureColumn, memoryColumn };
    }

    private void updateGraphs(BEServerUpdate serverUpdate)
    {
        gamersNumColumn.Values = gamersNumColumn.Values.Append(new DateTimePoint(serverUpdate.TimeStamp, serverUpdate.CurrentPlayers));
        highScoreColumn.Values = highScoreColumn.Values.Append(new DateTimePoint(serverUpdate.TimeStamp, serverUpdate.HighScore));
        avgScoreColumn.Values = avgScoreColumn.Values.Append(new DateTimePoint(serverUpdate.TimeStamp, serverUpdate.AvgScore));
        GamersGraphData = new ObservableCollection<ISeries> { gamersNumColumn, highScoreColumn, avgScoreColumn };

        cpuSpeedColumn.Values = cpuSpeedColumn.Values.Append(serverUpdate.CpuSpeed);
        cpuTempratureColumn.Values = cpuTempratureColumn.Values.Append(serverUpdate.CpuTemperature);
        memoryColumn.Values = memoryColumn.Values.Append(serverUpdate.MemoryUsage);
        HealthGraphData = new ObservableCollection<ISeries> { cpuSpeedColumn, cpuTempratureColumn, memoryColumn };

    }

    private void updateGraphs(IEnumerable<BEServerUpdate> serverUpdate)
    {
        gamersNumColumn.Values = serverUpdate.Select(s => new DateTimePoint(s.TimeStamp, s.CurrentPlayers));
        highScoreColumn.Values = serverUpdate.Select(s => new DateTimePoint(s.TimeStamp, s.HighScore));
        avgScoreColumn.Values = serverUpdate.Select(s => new DateTimePoint(s.TimeStamp, s.AvgScore));
        GamersGraphData = new ObservableCollection<ISeries> { gamersNumColumn, highScoreColumn, avgScoreColumn };

        cpuSpeedColumn.Values = serverUpdate.Select(s => s.CpuSpeed);
        cpuTempratureColumn.Values = serverUpdate.Select(s => s.CpuTemperature);
        memoryColumn.Values = serverUpdate.Select(s => s.MemoryUsage);
        HealthGraphData = new ObservableCollection<ISeries> { cpuSpeedColumn, cpuTempratureColumn, memoryColumn };
    }

    private void OnPointMeasured(ChartPoint<DateTimePoint, RoundedRectangleGeometry, LabelGeometry> point)
    {
        // Add animation to the visual of each data point
        var visual = point.Visual;
        if (visual is null) return;

        var delayedFunction = new DelayedFunction(EasingFunctions.BuildCustomElasticOut(1.5f, 0.60f), point, 30f);

        _ = visual
            .TransitionateProperties(
                nameof(visual.Y),
                nameof(visual.Height))
            .WithAnimation(animation =>
                animation
                    .WithDuration(delayedFunction.Speed)
                    .WithEasingFunction(delayedFunction.Function));
    }

    #endregion

    //private async void generateRandomData()
    //{
    //    var cancellationTokenSource = new CancellationTokenSource();
    //    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(40));
    //    try
    //    {
    //        await foreach (var update in Helpers.emulator.GenerateServerUpdatesAsync(serverId: 1, interval: TimeSpan.FromSeconds(1), cancellationTokenSource.Token))
    //        {
    //            serverUpdates.Add(update);
    //        }
    //    }
    //    catch (Exception)
    //    {

    //    }
    //}

    #region Masseging

    //public void RegisterToGameMessage()
    //{
    //    // Register to receive the list request message
    //    WeakReferenceMessenger.Default.Register<GameMessage>(this, OnGameRequest);
    //}

    //private void OnGameRequest(object sender, GameMessage message)
    //{
    //    if(message == null) return;
    //    if(message.PropertyName == "IdsRequest")
    //    {
    //        var ids = GameServers.Select(g => g.GameId).Distinct().ToList();
    //        // Send the list to the requester
    //        WeakReferenceMessenger.Default.Send( new GameMessage(this, "IdsResponse", ids));
    //    }
    //    else if(message.PropertyName == "GamesResponse")
    //    {
    //        //TODO
    //    }
    //}

    //public void RequestGames()
    //{
    //    // Send an empty request to Class B
    //    WeakReferenceMessenger.Default.Send(new ObservableMessage(this, "RequestData"));
    //}

    #endregion

    #region commands
    [RelayCommand]
    private void NavigateToDetail(int gameId)
    {
        var game = _gameModel.Games.FirstOrDefault(g => g.IGDBId == gameId);
        _navigationService.NavigateTo(typeof(GamesDetailViewModel).FullName, game);
    }
    [RelayCommand]
    private void ShowLastedBarsGamers()
    {
        var axis = XAxesGamersGraph[0];
        var gap = (serverUpdates.Count - 10) * 10;
        axis.MinLimit = gap > 0 ? TimeSpan.FromSeconds(gap).Ticks : null;
    }

    #endregion
}
