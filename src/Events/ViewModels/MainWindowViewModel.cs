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
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Events.FileAccess;
using Events.Models.Specifications;
using Events.Utilities;

namespace Events.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private IEventRepository _eventRepository;
    private string _filepath;
    
    private ObservableCollection<Event> _allEventsFromCurrFile;
    private ObservableCollection<Event> _allFilteredEvents;
    
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

    public MainWindowViewModel(IEventRepository repository)
    {
        _eventRepository = repository;
        
        InitializeAsync();

        OpenCreateEventWindowCommand = ReactiveCommand.Create(OpenCreateEventWindow);
        OpenEditEventWindowCommand = ReactiveCommand.Create(OpenEditEventWindow);
        FilterEventsComboboxCommand = ReactiveCommand.Create<string>(FilterEventsByComboboxValues);
        FilterEventSearchButtonCommand = ReactiveCommand.Create(FilterEvents);
        OpenFileCommand = ReactiveCommand.Create(ShowOpenFileDialog);
        NewListCommand = ReactiveCommand.Create(CreateNewList);
    }
    
    #region Button commands

    public ReactiveCommand<Unit, Unit> OpenCreateEventWindowCommand { get; }
    
    public ReactiveCommand<Unit, Unit> OpenEditEventWindowCommand { get; }
    
    public ReactiveCommand<string, Unit> FilterEventsComboboxCommand { get; }
    
    public ReactiveCommand<Unit, Unit> FilterEventSearchButtonCommand { get; }
    
    public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }

    public ReactiveCommand<Unit, Unit> NewListCommand { get; }
    
    #endregion

    #region Collections and Selected Event Properties

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
        set => this.RaiseAndSetIfChanged(ref _dateToFilterBy, value);
    }
    
    public DateTimeOffset? DateToFilterTo
    {
        get => _dateToFilterTo;
        set => this.RaiseAndSetIfChanged(ref _dateToFilterTo, value);
    }

    public string NameFilter
    {
        get => _nameFilter;
        set => this.RaiseAndSetIfChanged(ref _nameFilter, value);
    }

    public bool? DoneFilter
    {
        get => _doneFilter;
        set => this.RaiseAndSetIfChanged(ref _doneFilter, value);
    }

    public TimeSpan? TimeFilter
    {
        get => _timeFilter;
        set => this.RaiseAndSetIfChanged(ref _timeFilter, value);
    }

    public TimeSpan? DurationFilter
    {
        get => _durationFilter;
        set => this.RaiseAndSetIfChanged(ref _durationFilter, value);
    }

    public string? LocationFilter
    {
        get => _locationFilter;
        set => this.RaiseAndSetIfChanged(ref _locationFilter, value);
    }

    public string? CategoryFilter
    {
        get => _categoryFilter;
        set => this.RaiseAndSetIfChanged(ref _categoryFilter, value);
    }

    public string? DescriptionFilter
    {
        get => _descriptionFilter;
        set => this.RaiseAndSetIfChanged(ref _descriptionFilter, value);
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

    private readonly Dictionary<string, DateTime> _dateRangeCombobox = new()
    {
        { "Today", DateTime.Today.Date },
        { "Tomorrow", DateTime.Today.AddDays(1) },
        { "The day after tomorrow", DateTime.Today.AddDays(2) }
    };

    public IEnumerable<string> FilterOptions { get; } = new List<string>()
    {
        "All",
        "Today",
        "Tomorrow",
        "The day after tomorrow",
    };
    #endregion

    #region Initialization

    private async Task<Result> InitializeAsync()
    {
        var result = await LoadEvents();
        result.OnFailure(() => Debug.WriteLine(result.Error));
        
        return result;
    }

    private async Task<Result> LoadEvents(string filepath = null)
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

    #endregion
    
    #region Filtering

    private void FilterEventsByComboboxValues(string filter)
    {
        if (filter == "All")
        {
            AllFilteredEvents = AllEventsFromCurrFile;
        }
        else
        {
            var spec = new DateSpecification(_dateRangeCombobox[filter], _dateRangeCombobox[filter]);
            AllFilteredEvents =
                new ObservableCollection<Event>(AllEventsFromCurrFile.Where(e => spec.IsSatisfiedBy(e)));
        }
    }

    private CompositeSpecification CombineSpecifications()
    {
        var specifications = new List<ISpecification>();

        if (!string.IsNullOrEmpty(NameFilter))
        {
            specifications.Add(new NameSpecification(NameFilter));
        }

        if (DateToFilterFrom != null && DateToFilterTo != null)
        {
            specifications.Add(new DateSpecification(DateToFilterFrom.Value.Date, DateToFilterTo.Value.Date));
        }
        else if (DateToFilterFrom != null)
        {
            specifications.Add(new DateSpecification(DateToFilterFrom.Value.Date, DateTime.MaxValue));
        }
        else if (DateToFilterTo != null)
        {
            specifications.Add(new DateSpecification(DateTime.MinValue, DateToFilterTo.Value.Date));
        }
        
        if (TimeFilter.HasValue)
        {
            specifications.Add(new TimeSpecification(TimeFilter.Value));
        }
        if (DurationFilter.HasValue)
        {
            specifications.Add(new DurationSpecification(DurationFilter.Value));
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
        AllFilteredEvents = new ObservableCollection<Event>(
            AllEventsFromCurrFile.Where(e => combinedSpecification.IsSatisfiedBy(e)));
        
        ResetFilterProperties();
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

    #endregion
    
    #region New and Open File Dialogs

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
        var result = await openFileDialog.ShowAsync(mainWindow);
        if (result != null && result.Length > 0)
        {
            _filepath = result[0];
            _eventRepository = new EventRepository(_filepath);
            var allEvents = await _eventRepository.GetEventListByConditionAsync(e => true);
            if (allEvents.IsSuccess)
            {
                AllEventsFromCurrFile = new ObservableCollection<Event>(allEvents.Value);
                AllFilteredEvents = AllEventsFromCurrFile;
            }
        }
    }

    private async void CreateNewList()
    {
        var createFileDialog = new SaveFileDialog
        {
            Title = "Create New List",
            
            DefaultExtension = "json",
            InitialFileName = "events",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter 
                    { Name = "JSON files", Extensions = new List<string> { "json" } },
            }
        };
    
        var mainWindow = Avalonia.Application.Current.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (mainWindow != null)
        {
            var result = await createFileDialog.ShowAsync(mainWindow);
            if (!string.IsNullOrEmpty(result))
            {
                await using var file = File.Create(result);
            }
        }
    }
    #endregion
   
    #region Window Creation

    private void OpenCreateEventWindow()
    {
        var createEventWindowViewModel = new CreateEventWindowViewModel(
            AllEventsFromCurrFile, _eventRepository);
        var createEventWindow = new CreateEventWindow(this, _eventRepository)
            { DataContext = createEventWindowViewModel };

        createEventWindow.Show();
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

    #endregion
}