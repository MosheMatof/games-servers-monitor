

using BL.BE;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ServiceAgent.Contracts;
using WPFClient.Contracts.Models;
using WPFClient.Contracts.ViewModels;

namespace WPFClient.ViewModels;

public partial class GamesDetailViewModel : ObservableObject, INavigationAware
{
    private readonly IGameService _gameService;
    private readonly IGame _gameModel;

    [ObservableProperty]
    private BEGame game;

    public GamesDetailViewModel(IGameService gameService, IGame game)
    {
        _gameService = gameService;
        _gameModel = game;
    }

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is BEGame game)
        {
            this.Game = game;
        }
        if (parameter is int GameId)
        {
            //var g = await _gameService.GetGameInfoAsync(GameId);
            this.Game = _gameModel.Games.Where(g => g.IGDBId == GameId).FirstOrDefault();
        }
    }

    public void OnNavigatedFrom()
    {
    }


}
