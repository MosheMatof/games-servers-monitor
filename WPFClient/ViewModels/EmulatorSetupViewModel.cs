using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ServiceAgent.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFClient.ViewModels
{
    public partial class EmulatorSetupViewModel : ObservableObject
    {
        private readonly IEmulatorService _emulatorService;

        [ObservableProperty]
        private int numericValue;
        [ObservableProperty]
        private int numberOfServers;
        [ObservableProperty]
        private int numberOfGames;
        [ObservableProperty]
        private DateTime startDate;
        [ObservableProperty]
        private int intervalTime;
        [ObservableProperty]
        private bool isStarting;

        public Action<bool?> SetResult { get; set; }

        public EmulatorSetupViewModel(IEmulatorService emulatorService)
        {
            _emulatorService = emulatorService;
        }
        [RelayCommand]
        private void OnClose()
        {
            bool result = true;
            SetResult(result);
        }


        [RelayCommand]
        private void Increase()
        {
            NumericValue++;
        }
        [RelayCommand]
        private void Decrease()
        {
            NumericValue--;
        }
        [RelayCommand]
        private async void StartEmulator()
        {
            IsStarting = true;
            var sucsess = await _emulatorService.StartEmulator(NumberOfGames, NumberOfServers, IntervalTime * 1000);
            //MetroWindow window = Application.Current.MainWindow as MetroWindow;
            //if (sucsess)
            //{
            //    await window.ShowMessageAsync("Success", "Emulator started successfully");
            //}
            //else
            //{
            //    await window.ShowMessageAsync("Error", "Emulator failed to start");
            //}
            //window.Close();
        }
        [RelayCommand]
        private void Cancel()
        {
            // Implement logic to handle Cancel button click here
        }

    }
}
