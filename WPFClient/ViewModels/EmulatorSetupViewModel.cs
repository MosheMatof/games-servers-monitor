using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using ServiceAgent.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFClient.Models;

namespace WPFClient.ViewModels
{
    public partial class EmulatorSetupViewModel : ObservableValidator
    {
        private readonly IEmulatorService _emulatorService;
        private readonly Emulator _emulator = new();

        //[ObservableProperty]
        //private int numericValue;

        [Required(ErrorMessage = "Number of servers is required")]
        [Range(1, 20, ErrorMessage = "Number of servers must be between 0 - 20")]
        [ObservableProperty]
        private int numberOfServers = 1;

        [Required(ErrorMessage = "Number of games is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of games must be greater than zero")]
        [ObservableProperty]
        private int numberOfGames = 1;

        [ObservableProperty]
        private DateTime startDate;

        [Required(ErrorMessage = "Interval time is required")]
        [Range(10, int.MaxValue, ErrorMessage = "Interval time must be greater than or equal to 10")]
        [ObservableProperty]
        private int intervalTime = 10;

        [ObservableProperty]
        private bool isStarting = false;

        public EmulatorSetupViewModel(IEmulatorService emulatorService)
        {
            _emulatorService = emulatorService;
        }
        [RelayCommand]
        private void OnClose(object sender)
        {
            var window = sender as ChildWindow;
            if (IsStarting)
            {
                if (!_emulator.isStarting)
                {
                    MessageBox.Show("Emulator is in initialize procces, please press cancel to stop it before closing", "Error");
                    return;
                }
                else
                {
                    window.Close(childWindowResult: false);
                }
            }
            else
            {
                window.Close(childWindowResult: false);
            }
        }

        [RelayCommand]
        private async Task StartEmulator(object sender)
        {
            var window = sender as ChildWindow;
            try
            {
                IsStarting = true;
                var success = await Task.Run(async () => await _emulatorService.StartEmulator(NumberOfGames, NumberOfServers, IntervalTime * 1000));
                if (success)
                {
                    _emulator.numberOfGames = NumberOfGames;
                    _emulator.numberOfServers = NumberOfServers;
                    _emulator.intervalTime = IntervalTime;
                    _emulator.startDate = StartDate;
                    _emulator.isStarting = IsStarting;
                }
                window.Close(success);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                window.Close(childWindowResult: null);
            }
        }
        [RelayCommand]
        private void Cancel(object sender)
        {
            var window = sender as ChildWindow;
            if (IsStarting)
            {
                var result = MessageBox.Show("Are you sure you want to stop the emulator?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _emulatorService.StopEmulator();
                    IsStarting = false;
                }
            }
            window.Close(childWindowResult: false);
        }
    }
}
