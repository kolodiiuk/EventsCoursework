using Events.Models;
using Events.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Events.Utilities;

namespace Events.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    private IEventRepository _eventRepository;
    private string _filepath;
    private ObservableCollection<Event> _allEventsFromCurrFile;
    private ObservableCollection<Event> _allFilteredEvents;
    private Event _selectedEvent;
    private string _selectedFilter;
    private DateTimeOffset? _dateToFilterBy;
    private DateTimeOffset? _dateToFilterTo;

    public MainWindowViewModel(IEventRepository repository)
    {
        _eventRepository = repository;
        InitializeAsync();

        OpenSearchWindowCommand = ReactiveCommand.Create(OpenSearchWindow);
        OpenCreateEventWindowCommand = ReactiveCommand.Create(OpenCreateEventWindow);
        OpenEditEventWindowCommand = ReactiveCommand.Create(OpenEditEventWindow);
        FilterEventsCommand = ReactiveCommand.Create<string>(FilterEventsByComboboxValues);
        FilterEventsByDateCommand = ReactiveCommand.Create<DateRange>(FilterEventsByDateRange);
        OpenFileCommand = ReactiveCommand.Create(ShowOpenFileDialog);
    }

    public class DateRange
    {
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
    }
    
    #region Button commands

    public ReactiveCommand<Unit, Unit> OpenSearchWindowCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenCreateEventWindowCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenEditEventWindowCommand { get; }
    public ReactiveCommand<string, Unit> FilterEventsCommand { get; }
    public ReactiveCommand<DateRange, Unit> FilterEventsByDateCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }

    #endregion

    public IEnumerable<string> FilterOptions { get; } = new List<string>()
    {
        "All",
        "Today",
        "Tomorrow",
        "The day after tomorrow",
    };
    
    public ObservableCollection<Event> AllEventsFromCurrFile
    {
        get { return _allEventsFromCurrFile; }
        set
        {
            _allEventsFromCurrFile = value;
            OnPropertyChanged();
        }
    }
    
    public ObservableCollection<Event> AllFilteredEvents
    {
        get { return _allFilteredEvents; }
        set
        {
            _allFilteredEvents = value;
            OnPropertyChanged();
        }
    }
    
    public Event SelectedEvent
    {
        get { return _selectedEvent; }
        set
        {
            _selectedEvent = value;
            OnPropertyChanged();
        }
    }

    public string SelectedFilter
    {
        get { return _selectedFilter; }
        set
        {
            if (_selectedFilter != value)
            {
                _selectedFilter = value;
                OnPropertyChanged();
                FilterEventsCommand.Execute(_selectedFilter).Subscribe();
            }
        }
    }
    
    public DateTimeOffset? DateToFilterFrom
    {
        get => _dateToFilterBy;
        set => this.RaiseAndSetIfChanged(ref _dateToFilterBy, value);
    }
    
    public DateTimeOffset? DateToFilterTo
    {
        get => _dateToFilterTo;
        set => this.RaiseAndSetIfChanged(backingField: ref _dateToFilterTo, value);
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
            AllEventsFromCurrFile = new ObservableCollection<Event>(allEvents.Value);
            AllFilteredEvents = AllEventsFromCurrFile;

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Fail("Failed to load events");
        }
    }

    private void FilterEventsByComboboxValues(string filter)
    {
        DateTime? targetDate = null;
        switch (filter)
        {
            case "All":
                AllFilteredEvents = _allEventsFromCurrFile;
                break;
            case "Today":
                targetDate = DateTime.Today.Date;
                Debug.WriteLine("hello");
                break;
            case "Tomorrow":
                targetDate = DateTime.Today.AddDays(1);
                break;
            case "The day after tomorrow":
                targetDate = DateTime.Today.AddDays(2);
                break;
            default:
                return;
        }

        if (targetDate.HasValue)
        {
            AllFilteredEvents = 
                new ObservableCollection<Event>(
                    AllEventsFromCurrFile.Where(
                        e => e.DateTime.HasValue
                             && e.DateTime.Value.Date == targetDate));
        }
    }

    // private void FilterEventsByDateRange()
    private void FilterEventsByDateRange(DateRange range)
    {
        if (range.From.HasValue && range.To.HasValue)
        {
            AllFilteredEvents =
                new ObservableCollection<Event>(
                    AllEventsFromCurrFile.Where(
                        e => e.DateTime.HasValue
                             && e.DateTime.Value.Date >= range.From.Value.Date
                             && e.DateTime.Value.Date <= range.To.Value.Date));
        }
        else if (range.From.HasValue)
        {
            AllFilteredEvents =
                new ObservableCollection<Event>(
                    AllEventsFromCurrFile.Where(
                        e => e.DateTime.HasValue
                             && e.DateTime.Value.Date >= range.From.Value.Date));
        }
        else if (range.To.HasValue)
        {
            AllFilteredEvents =
                new ObservableCollection<Event>(
                    AllEventsFromCurrFile.Where(
                        e => e.DateTime.HasValue
                             && e.DateTime.Value.Date <= range.To.Value.Date));
        }
    }
    
    private async void ShowOpenFileDialog()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Open File",
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter 
                    { Name = "JSON files", Extensions = new List<string> { "json" } },
                new FileDialogFilter 
                    { Name = "Text files", Extensions = new List<string> { "txt" } },
            }
        };

        var mainWindow = Avalonia.Application.Current.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (mainWindow != null)
        {
            var result = await openFileDialog.ShowAsync(mainWindow);
            if (result != null && result.Length > 0)
            {
                _filepath = result[0];
            }
        }
    }

    #region Window Creation

    private void OpenCreateEventWindow()
    {
        var createEventWindowViewModel = new CreateEventWindowViewModel(
            AllEventsFromCurrFile, _eventRepository);
        var createEventWindow = new CreateEventWindow(this, _eventRepository)
            { DataContext = createEventWindowViewModel };

        createEventWindow.Show();
        // var result = NotificationManager.ShowNotification("hello", "yopta");
        // Debug.WriteLine(result.Failure ? result.Error : "success");
    }

    private void OpenEditEventWindow()
    {
        var editEventWindowViewModel = new EditEventWindowViewModel(
            AllEventsFromCurrFile, _selectedEvent, _eventRepository);
        var editEventWindow = new EditEventWindow(
                AllEventsFromCurrFile, _selectedEvent, _eventRepository)
            { DataContext = editEventWindowViewModel };

        editEventWindow.Show();
    }

    private void OpenSearchWindow()
    {
        var searchWindow = new SearchWindow();

        searchWindow.Show();
    }
    
    #endregion
}