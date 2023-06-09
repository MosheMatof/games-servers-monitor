﻿using System.Windows.Controls;

using MahApps.Metro.Controls;

using WPFClient.Contracts.Views;
using WPFClient.ViewModels;

namespace WPFClient.Views;

public partial class ShellDialogWindow : MetroWindow, IShellDialogWindow
{
    public ShellDialogWindow(ShellDialogViewModel viewModel)
    {
        InitializeComponent();
        viewModel.SetResult = OnSetResult;
        DataContext = viewModel;
    }

    public Frame GetDialogFrame()
        => dialogFrame;

    private void OnSetResult(bool? result)
    {
        DialogResult = result;
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}
