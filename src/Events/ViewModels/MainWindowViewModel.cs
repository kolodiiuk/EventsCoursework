using Events.Models;
using Events.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;

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
        FilterEventsCommand = ReactiveCommand.CreateFromTask(FilterEvents);
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
        try
        {
            var allEvents = await _eventRepository.
                GetEventListByConditionAsync(e => true);
            Events = new ObservableCollection<Event>(allEvents.Value);

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Fail("Failed to load events");
        }
    }

    private string _selectedFilter;

    public string SelectedFilter
    {
        get { return _selectedFilter; }
        set
        {
            _selectedFilter = value;
            OnPropertyChanged();
            FilterEventsCommand.Execute().Subscribe();
        }
    }

    public ReactiveCommand<Unit, Unit> FilterEventsCommand { get; }

    private async Task FilterEvents()
    {
        switch (SelectedFilter)
        {
            case "All events":
                // Load all events
                break;
            case "Today":
                // Load today's events
                break;
            case "Tomorrow":
                // Load tomorrow's events
                break;
            case "The day after tomorrow":
                // Load the day after tomorrow's events
                break;
        }
    }

    #region Window Creation

    private void OpenCreateEventWindow()
    {
        var createEventWindowViewModel = new CreateEventWindowViewModel(
            this, _eventRepository);
        var createEventWindow = new CreateEventWindow(this, _eventRepository)
            { DataContext = createEventWindowViewModel };

        createEventWindow.Show();
    }

    private void OpenEditEventWindow()
    {
        var editEventWindowViewModel = new EditEventWindowViewModel(
            this, _selectedEvent, _eventRepository);
        var editEventWindow = new EditEventWindow(
                this, _selectedEvent, _eventRepository)
            { DataContext = editEventWindowViewModel };

        editEventWindow.Show();
    }

    private void OpenSearchWindow()
    {
        var searchWindow = new ErrorWindow();

        searchWindow.Show();
    }

    #endregion
}