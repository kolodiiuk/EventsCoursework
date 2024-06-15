using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Events.Models;

namespace Events.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.Closed += MainWindow_Closed;
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }
}
