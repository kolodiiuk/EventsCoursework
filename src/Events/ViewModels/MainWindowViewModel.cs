using Events.Models;
using Events.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Events.FileAccess;
using Events.Models.Specifications;
using Events.Utilities;

namespace Events.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private IEventDataProvider _dataProvider;
    private string _filepath;
    private readonly Timer _timer; 
    private int _notificationThresholdMinutes = 1;
    private int? _notificationThresholdMinutesTemp;
    private List<Guid> _shownNotfications = new();
    private bool _showNotifications = true;

    private ObservableCollection<Event> _filteredEvents;
    private ObservableCollection<Event> _upcomingEvents;
    private ObservableCollection<Event> _pastEvents;
    private Event _selectedEvent;
    private string _selectedFilter;
    private DateTimeOffset? _dateToFilterBy;
    private DateTimeOffset? _dateToFilterTo;
    private bool? _doneFilter;
    private string _nameFilter;
    private string? _descriptionFilter;
    private string? _locationFilter;
    private string? _categoryFilter;
    private TimeSpan? _durationFilter;
    private TimeSpan? _timeFilter;

    private string? _statistics = "No statistics available";
    
    public MainWindowViewModel(IEventDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
        
        _timer = new Timer(60000);
        _timer.Elapsed += CheckUpcomingEvents;
        _timer.Start();
        
        Initialize();

        OpenCreateEventWindowCommand = ReactiveCommand.Create(OpenCreateEventWindow);
        OpenEditEventWindowCommand = ReactiveCommand.Create(OpenEditEventWindow);
        FilterEventsComboboxCommand = ReactiveCommand.Create<string>(FilterEventsByComboboxValues);
        FilterEventSearchButtonCommand = ReactiveCommand.Create(FilterEvents);
        OpenFileCommand = ReactiveCommand.Create(ShowOpenFileDialog);
        NewListCommand = ReactiveCommand.Create(CreateNewList);
        SaveAllEventsCommand = ReactiveCommand.Create(SaveAllEventsFromListToTxt);
        SaveFilteredEventsCommand = ReactiveCommand.Create(SaveFilteredEventsToTxt);
        UpdatePastEventsCommand = ReactiveCommand.Create(UpdatePastEvents);
        UpdateUpcomingEventsCommand = ReactiveCommand.Create(UpdateUpcomingEvents);
        SaveReminderSettingsCommand = ReactiveCommand.Create(SaveReminderSettings);
        UpdateStatsCommand = ReactiveCommand.Create(UpdateStatistics);
    }

    #region Button commands

    public ReactiveCommand<Unit, Unit> OpenCreateEventWindowCommand { get; set; }
    public ReactiveCommand<Unit, Unit> OpenEditEventWindowCommand { get; set; }
    public ReactiveCommand<string, Unit> FilterEventsComboboxCommand { get; set; }
    public ReactiveCommand<Unit, Unit> FilterEventSearchButtonCommand { get; set; }
    public ReactiveCommand<Unit, Unit> OpenFileCommand { get; set; }
    public ReactiveCommand<Unit, Unit> NewListCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SaveAllEventsCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SaveFilteredEventsCommand { get; set; }
    public ReactiveCommand<Unit, Unit> UpdatePastEventsCommand { get; set; }
    public ReactiveCommand<Unit, Unit> UpdateUpcomingEventsCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SaveReminderSettingsCommand { get; set; }
    public ReactiveCommand<Unit, Unit> UpdateStatsCommand { get; set; }
    
    #endregion

    #region Collections and Selected Event Properties

    public ObservableCollection<Event> FilteredEvents
    {
        get { return _filteredEvents; }
        set
        {
            _filteredEvents = value;
            OnPropertyChanged();
        }
    }
    
    public ObservableCollection<Event> UpcomingEvents
    {
        get { return _upcomingEvents; }
        set
        {
            _upcomingEvents = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Event> PastEvents
    {
        get { return _pastEvents; }
        set
        {
            _pastEvents = value;
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

    #endregion
    
    #region Filtering Properties

    public string SelectedFilter
    {
        get { return _selectedFilter; }
        set
        {
            if (_selectedFilter == value) return;
            _selectedFilter = value;
            OnPropertyChanged();
            FilterEventsComboboxCommand.Execute(_selectedFilter).Subscribe();
        }
    }
    
    public DateTimeOffset? DateToFilterFrom
    {
        get => _dateToFilterBy;
        set
        {
            this.RaiseAndSetIfChanged(ref _dateToFilterBy, value); 
            OnPropertyChanged();
        }
    }

    public DateTimeOffset? DateToFilterTo
    {
        get => _dateToFilterTo;
        set
        {
            this.RaiseAndSetIfChanged(ref _dateToFilterTo, value); 
            OnPropertyChanged();
        }
    }

    public string NameFilter
    {
        get => _nameFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref _nameFilter, value); 
            OnPropertyChanged();
        }
    }

    public bool? DoneFilter
    {
        get => _doneFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref _doneFilter, value); 
            OnPropertyChanged();
        }
    }

    public TimeSpan? TimeFilter
    {
        get => _timeFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref _timeFilter, value); 
            OnPropertyChanged();
        }
    }

    public TimeSpan? DurationFilter
    {
        get => _durationFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref _durationFilter, value); 
            OnPropertyChanged();
        }
    }

    public string? LocationFilter
    {
        get => _locationFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref _locationFilter, value);
            OnPropertyChanged();
        }
    }

    public string? CategoryFilter
    {
        get => _categoryFilter;
        
        set
        {
            this.RaiseAndSetIfChanged(ref _categoryFilter, value);
            OnPropertyChanged();
        }
    }

    public string? DescriptionFilter
    {
        get => _descriptionFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref _descriptionFilter, value);
            OnPropertyChanged();
        }
    }

    #endregion

    #region UI Properties

    public List<string> Suggestions { get; } = new List<string>
    {
        "Work",
        "School",
        "Family",
        "Friends",
        "Other"
    };

    public IEnumerable<string> FilterOptions { get; } = new List<string>()
    {
        "All",
        "Today",
        "Tomorrow",
        "The day after tomorrow",
    };
    
    private readonly Dictionary<string, DateTime> _dateRangeCombobox = new()
    {
        { "Today", DateTime.Today.Date },
        { "Tomorrow", DateTime.Today.AddDays(1) },
        { "The day after tomorrow", DateTime.Today.AddDays(2) }
    };

    #endregion

    #region Other UI Properties
    public string? Statistics 
    { 
        get => _statistics;
        set
        {
            this.RaiseAndSetIfChanged(ref _statistics, value); 
            OnPropertyChanged();
        }
    } 
    
    public int? NotificationThresholdMinutesTemp
    {
        get => _notificationThresholdMinutesTemp;
        set => this.RaiseAndSetIfChanged(ref _notificationThresholdMinutesTemp, value);
    }
    
    #endregion

    #region Initialization and Helper Methods

    private void Initialize()
    {
        FilteredEvents = new ObservableCollection<Event>();
        ReloadEvents();
    }

    public void ReloadEvents() 
    {
        var allEvents = _dataProvider.GetAllEvents().Value;
        FilteredEvents.Clear();
        foreach (var e in allEvents)
        {
            FilteredEvents.Add(e);
        }

        UpcomingEvents = new ObservableCollection<Event>(
            allEvents.Where(e => e.DateTime > DateTime.Today));
        PastEvents = new ObservableCollection<Event>(
            allEvents.Where(e => e.DateTime < DateTime.Today));
    }

    private void CheckUpcomingEvents(object sender, ElapsedEventArgs e)
    {
        SaveChanges();

        if (_showNotifications)
        {
            var upcomingEvents = _dataProvider.GetAllEvents().Value
                .Where(@event => @event.DateTime > DateTime.Now &&
                                 @event.DateTime < DateTime.Now.AddMinutes(_notificationThresholdMinutes));

            foreach (var upcomingEvent in upcomingEvents)
            {
                if (_shownNotfications.Contains(upcomingEvent.Id)) continue;
                NotificationManager.ShowNotification(
                    upcomingEvent.Name, $"In 5 minutes: {upcomingEvent.Name}");
                _shownNotfications.Add(upcomingEvent.Id);
            }
        }
    }

    private void UpdateStatistics()
    {
        var statistics = new Statistics(_dataProvider.GetAllEvents().Value);
        Statistics = statistics.StringRepresentation;
    }
    
    public void SaveReminderSettings()
    {
        if (_notificationThresholdMinutesTemp >= 1 
            && _notificationThresholdMinutesTemp <= 10080)
        {
            _notificationThresholdMinutes = _notificationThresholdMinutesTemp ?? 1;
            _showNotifications = true;
        } 
        else if (_notificationThresholdMinutesTemp == 0)
        {
            _showNotifications = false;
        }
    }

    #endregion
    
    #region Filtering
    private CompositeSpecification CombineSpecifications()
    {
        var specifications = new List<ISpecification>();

        if (!string.IsNullOrEmpty(NameFilter))
        {
            specifications.Add(new NameSpecification(NameFilter));
        }

        if (DateToFilterFrom != null && DateToFilterTo != null)
        {
            specifications.Add(
                new DateSpecification(
                    DateToFilterFrom.Value.Date, DateToFilterTo.Value.Date));
        }
        else if (DateToFilterFrom != null)
        {
            specifications.Add(
                new DateSpecification(
                    DateToFilterFrom.Value.Date, DateTime.MaxValue));
        }
        else if (DateToFilterTo != null)
        {
            specifications.Add(
                new DateSpecification(
                    DateTime.MinValue, DateToFilterTo.Value.Date));
        }
        
        if (TimeFilter.HasValue)
        {
            specifications.Add(new TimeSpecification(TimeFilter.Value));
        }
        if (!string.IsNullOrEmpty(LocationFilter))
        {
            specifications.Add(new LocationSpecification(LocationFilter));
        }
        if (!string.IsNullOrEmpty(CategoryFilter))
        {
            specifications.Add(new CategorySpecification(CategoryFilter));
        }
        if (!string.IsNullOrEmpty(DescriptionFilter))
        {
            specifications.Add(new DescriptionSpecification(DescriptionFilter));
        }
        if (DoneFilter.HasValue)
        {
            specifications.Add(new DoneSpecification(DoneFilter.Value));
        }
        
        return new CompositeSpecification(specifications.ToArray());
    }
    
    private void FilterEvents()
    {
        var combinedSpecification = CombineSpecifications();
        var allEvents = _dataProvider.GetAllEvents().Value;
        var allFilteredEvents = allEvents.
            Where(e => combinedSpecification.IsSatisfiedBy(e));
        
        FilteredEvents = new ObservableCollection<Event>(allFilteredEvents);
        
        ResetFilterProperties();
    }

    private void FilterEventsByComboboxValues(string filter)
    {
        if (filter == "All")
        {
            var allEvents = _dataProvider.GetAllEvents().Value;
            if (allEvents == null) return;
            
            FilteredEvents.Clear();
            foreach (var e in allEvents)
            {
                FilteredEvents.Add(e);
            }
        }
        else
        {
            var spec = new DateSpecification(
                _dateRangeCombobox[filter], _dateRangeCombobox[filter]);
            
            FilteredEvents =
                new ObservableCollection<Event>(
                    _dataProvider.GetAllEvents().Value.
                    Where(e => spec.IsSatisfiedBy(e)));
        }
    }
   
    private void ResetFilterProperties()
    {
        NameFilter = string.Empty;
        DoneFilter = null;
        TimeFilter = null;
        DurationFilter = null;
        LocationFilter = string.Empty;
        CategoryFilter = string.Empty;
        DescriptionFilter = string.Empty;
        DateToFilterFrom = null;
        DateToFilterTo = null;
    }

    private void UpdateUpcomingEvents()
    {
        UpcomingEvents.Clear();
        
        var upcomingEvents = _dataProvider.GetAllEvents().Value
            .Where(e => e.DateTime > DateTime.Now);
        foreach (var e in upcomingEvents)
        {
            UpcomingEvents.Add(e);
        }
    }

    private void UpdatePastEvents()
    {
        PastEvents.Clear();
        
        var pastEvents = _dataProvider.GetAllEvents().Value
            .Where(e => e.DateTime < DateTime.Now);
        foreach (var e in pastEvents)
        {
            PastEvents.Add(e);
        }
    }
    
    #endregion
    
    #region File Dialogs and File Operations
    
    public void SaveChanges()
    {
        _dataProvider.SubmitChanges();
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

        if (mainWindow == null) return;
        
        var filepath = await openFileDialog.ShowAsync(mainWindow);
        if (filepath != null && filepath.Length > 0)
        {
            _filepath = filepath[0];
            SaveChanges();
            _dataProvider = new EventsJsonEventDataProvider(_filepath);
            var allEvents = _dataProvider.GetAllEvents();
            if (allEvents.IsSuccess)
            {
                FilteredEvents = new ObservableCollection<Event>(allEvents.Value);
            }
        }
    }
    
    private async void CreateNewList()
    {
        var filepath = await ShowCreateFileDialog();
        
        if (!string.IsNullOrEmpty(filepath))
        {
            await using var file = File.Create(filepath);
        }
    }
    
    private async void SaveAllEventsFromListToTxt()
    {
        var sb = CreateStringBuilderFromEvents(_dataProvider.GetAllEvents().Value);
        var filepath = await ShowCreateFileDialog();
        if (string.IsNullOrEmpty(filepath)) return;
        await File.WriteAllTextAsync(filepath, sb.ToString());
    }

    private async void SaveFilteredEventsToTxt()
    {
        var sb = CreateStringBuilderFromEvents(FilteredEvents);
        var filepath = await ShowCreateFileDialog();
        if (string.IsNullOrEmpty(filepath)) return;
        await File.WriteAllTextAsync(filepath, sb.ToString());
    }
    
    private StringBuilder CreateStringBuilderFromEvents(
        IEnumerable<Event> events)
    {
        var sb = new StringBuilder();
        
        foreach (var e in events)
        {
            sb.AppendLine(e.ToString());
        }

        return sb;
    }
    
    private async Task<string> ShowCreateFileDialog()
    {
        var createFileDialog = new SaveFileDialog
        {
            Title = "Create New List",
            
            DefaultExtension = "json",
            InitialFileName = "events",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter
                {
                    Name = "JSON files", Extensions = new List<string> { "json" },
                },
                new FileDialogFilter()
                {
                    Name = "Text files", Extensions = new List<string> { "txt" },
                }
            }
        };

        var mainWindow = Avalonia.Application.Current.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        var result = string.Empty;
        if (mainWindow != null)
        {
            result = await createFileDialog.ShowAsync(mainWindow);
        }

        return result;
    }

    #endregion

    #region Window Creation

    private void OpenCreateEventWindow()
    {
        var createEventWindowViewModel = new CreateEventWindowViewModel(_dataProvider);
        createEventWindowViewModel.EventCreated += ReloadEvents;
        var createEventWindow = new CreateEventWindow(this, _dataProvider) 
            { DataContext = createEventWindowViewModel };

        createEventWindow.Show();
    }

    private void OpenEditEventWindow()
    {
        if (_selectedEvent == null) return;

        var editEventWindowViewModel =
            new EditEventWindowViewModel(_selectedEvent.Id, _dataProvider);
        editEventWindowViewModel.EventUpdated += ReloadEvents;
        var editEventWindow = new EditEventWindow(_selectedEvent.Id, 
                _dataProvider, this)
            { DataContext = editEventWindowViewModel };

        editEventWindow.Show();
    }

    #endregion
}
