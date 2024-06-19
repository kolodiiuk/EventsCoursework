using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Events.Views;

public partial class OverlapHandlingWindow : Window
{
    public OverlapHandlingWindow(OverlapHandlingViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        viewModel.CloseRequested += Close;
    }
}