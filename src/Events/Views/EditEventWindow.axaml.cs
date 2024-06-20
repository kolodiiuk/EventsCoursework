using System.Collections.ObjectModel;
using Avalonia.Controls;
using Events.Models;
using Events.ViewModels;

namespace Events.Views;

public partial class EditEventWindow : Window
{
    public EditEventWindow(
        ObservableCollection<Event> events, Event @event, IEventDataProvider repository)
    {
        InitializeComponent();

        DataContext = new EditEventWindowViewModel(events, @event, repository);
    }
}