using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Events.Models;
using Events.ViewModels;

namespace Events.Views;

public partial class EditEventWindow : Window
{
    private EditEventWindowViewModel _viewModel;
    private MainWindowViewModel _mainWindowViewModel;

    public EditEventWindow(Guid id, IEventDataProvider dataProvider, MainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();
        _viewModel = new EditEventWindowViewModel(id, dataProvider);
        DataContext = _viewModel;
        _viewModel.EventUpdated += mainWindowViewModel.ReloadEvents;
        _mainWindowViewModel = mainWindowViewModel;
    }

    public EditEventWindow()
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
            _viewModel.EventUpdated -= _mainWindowViewModel.ReloadEvents;
        }
    }
}