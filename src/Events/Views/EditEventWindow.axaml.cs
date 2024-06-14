using Avalonia.Controls;
using Events.Models;
using Events.ViewModels;

namespace Events.Views;

public partial class EditEventWindow : Window
{
    public EditEventWindow(
        MainWindowViewModel mainWindowViewModel, Event @event, IEventRepository repository)
    {
        InitializeComponent();

        DataContext = new EditEventWindowViewModel(mainWindowViewModel, @event, repository);
    }
}