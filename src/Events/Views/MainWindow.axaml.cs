using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using Events.Models;
using Events.ViewModels;

namespace Events.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Closed += (sender, e) => 
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.SubmitChanges();
            }
        };
        Closed += (sender, e) => Environment.Exit(0);
    }

    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
