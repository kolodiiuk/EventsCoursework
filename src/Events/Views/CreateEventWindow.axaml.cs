using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Events.Models;
using Events.ViewModels;

namespace Events.Views;

public partial class CreateEventWindow : Window
{
    private CreateEventWindowViewModel _viewModel;
    private MainWindowViewModel _mainWindowViewModel;

    public CreateEventWindow(MainWindowViewModel mainWindowViewModel, IEventDataProvider repository)
    {
        InitializeComponent();
        _viewModel = new CreateEventWindowViewModel(repository);
        DataContext = _viewModel;
        _viewModel.EventCreated += mainWindowViewModel.ReloadEvents;
        _mainWindowViewModel = mainWindowViewModel;
    }

    public CreateEventWindow()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        if (_viewModel != null)
        {
            _viewModel.EventCreated -= _mainWindowViewModel.ReloadEvents;
        }
    }
}