using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BL.BE;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ServiceAgent.Contracts;
using Windows.Networking.NetworkOperators;
using WPFClient.Contracts.Models;
using WPFClient.Contracts.Services;
using WPFClient.Contracts.ViewModels;
using WPFClient.Messaging;
using WPFClient.Views;

namespace WPFClient.ViewModels;

public partial class GamesViewModel : ObservableRecipient, INavigationAware
{
    private readonly IGameService _gameService;
    private readonly INavigationService _navigationService;

    private readonly IGame _gameModel;
    private readonly IGameServer _gameServerModel;

    private bool isInitialized = false;

    [ObservableProperty]
    private ObservableCollection<BEGame> games = new();
    [ObservableProperty]
    private BEGame selectedGame;

    private GamesDetailPage _detailPage;
    public GamesDetailPage DetailPage
    {
        get => _detailPage;
        set => SetProperty(ref _detailPage, value);
    }

    public GamesViewModel(IGameService gameService, INavigationService navigationService, IGame game, IGameServer gameServer, GamesDetailPage detailPage)
    {
        _gameService = gameService;
        _navigationService = navigationService;
        _gameModel = game;
        _gameServerModel = gameServer;
        _detailPage = detailPage;   
    }

    public async void OnNavigatedTo(object parameter)
    {
        await Task.Run(async () =>
        {
            await Initialize();
            isInitialized = true;
        });
    }

    public void OnNavigatedFrom()
    {
    }

    public async Task Initialize()
    {
        if (!isInitialized)
        {
            try
            {
                await LoadGamesAsync();
                isInitialized = true;
            }
            catch (Exception ex)
            {
                // handle exception
            }
        }
    }

    private async Task LoadGamesAsync()
    {
        //check is null or empty
        if (_gameServerModel != null && _gameServerModel.GameServers.Count > 0)
        {
            var gameIds = _gameServerModel.GameServers.Select(x => x.GameId).Distinct().ToList();

            await foreach (var game in _gameService.GetGameInfoAsync(gameIds).ConfigureAwait(false))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.Games.Add(game);
                    _gameModel.Games.Add(game);
                });
            }
        }
        else
        {
            MessageBox.Show("ERROR: the game server model has not initialize yet");
        }
    }


    //#region Masseging

    //public void RegisterToGameMessage()
    //{
    //    // Register to receive the list request message
    //    WeakReferenceMessenger.Default.Register<GameMessage>(this, OnGameRequest);
    //}

    //private async void OnGameRequest(object sender, GameMessage message)
    //{
    //    if (message == null) return;
    //    if (message.PropertyName == "IdsResponse")
    //    {
    //        if (message.Value is List<int> ids)
    //        {
    //            await foreach (var g in _gameService.GetGameInfoAsync(ids))
    //            {
    //                this.Games.Add(g);
    //            }
    //        }
    //    }
    //    else if (message.PropertyName == "GamesRequest")
    //    {
    //        //TODO
    //    }

    //}

    //public void RequestGames()
    //{
    //    // Send an empty request to Class B
    //    WeakReferenceMessenger.Default.Send(new GameMessage(this, "RequestData"));
    //}

    //#endregion

    #region commands
    [RelayCommand]
    private void NavigateToDetail(BEGame game)
    {
        //_navigationService.NavigateTo(typeof(GamesDetailViewModel).FullName, game);
        if (DetailPage.DataContext is GamesDetailViewModel gamesDetailViewModel)
        {
            gamesDetailViewModel.Game = game;
            OnPropertyChanged(nameof(DetailPage));
        }
    }

    #endregion
}
