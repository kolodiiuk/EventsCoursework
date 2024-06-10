using Events.Models;
using Events.Views;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;

namespace Events.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    private readonly IEventRepository _eventRepository;
    private ObservableCollection<Event> _events;
    private Event _selectedEvent;

    public MainWindowViewModel(IEventRepository repository)
    {
        _eventRepository = repository;
        InitializeAsync();

        OpenSearchWindowCommand = ReactiveCommand.Create(OpenSearchWindow);
        OpenCreateEventWindowCommand = ReactiveCommand.Create(OpenCreateEventWindow);
        OpenEditEventWindowCommand = ReactiveCommand.Create(OpenEditEventWindow);
    }

    #region Button commands

    public ReactiveCommand<Unit, Unit> OpenSearchWindowCommand { get; }

    public ReactiveCommand<Unit, Unit> OpenCreateEventWindowCommand { get; }

    public ReactiveCommand<Unit, Unit> OpenEditEventWindowCommand { get; }

     #endregion

    public Event SelectedEvent
    {
        get { return _selectedEvent; }
        set
        {
            _selectedEvent = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Event> Events
    {
        get { return _events; }
        set
        {
            _events = value;
            OnPropertyChanged();
        }
    }

    private async Task InitializeAsync()
    {
        await LoadEvents();
    }

    private async Task<Result> LoadEvents()
    {
        var allEvents = await _eventRepository.GetEventListByConditionAsync(e => true);
        Events = new ObservableCollection<Event>(allEvents.Value);

        // foreach (var @event in Events)
        // {
        //     @event.PropertyChanged += Event_PropertyChanged;
        // }
        Debug.WriteLine("Events count: " + Events.Count);

        return Result.Success();
    }

    private void OpenCreateEventWindow()
    {
        var createEventWindowViewModel = new CreateEventWindowViewModel(this, _eventRepository);
        var createEventWindow = new CreateEventWindow(this, _eventRepository) { DataContext = createEventWindowViewModel };
        createEventWindow.Show();
    }

    private void OpenSearchWindow()
    {
        var searchWindow = new ErrorWindow();
        searchWindow.Show();
    }

    private void OpenEditEventWindow()
    {
        var editEventWindow = new EditEventWindow();
        editEventWindow.Show();
    }
}
