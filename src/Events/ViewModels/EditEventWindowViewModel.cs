using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using Events.Models;
using Events.Utilities;

namespace Events.ViewModels;

public class EditEventWindowViewModel : ViewModelBase
{
    private readonly IEventDataProvider _dataProvider;
    private readonly ObservableCollection<Event> _eventsCollection;
    private string _name;
    private DateTimeOffset? _date;
    private TimeSpan? _time;
    private TimeSpan? _duration;
    private string _location;
    private string _category;
    private string _description;  
    private List<string> _suggestions;
    private bool? _done;

    public EditEventWindowViewModel(ObservableCollection<Event> eventsCollection, 
        Event selectedEvent, IEventDataProvider dataProvider)
    {
        SelectedEvent = selectedEvent;
        _dataProvider = dataProvider;
        _eventsCollection = eventsCollection;
        _dataProvider = dataProvider;
        Suggestions = new List<string>
        {
            "Work",
            "School",
            "Family",
            "Friends",
            "Other"
        };

        EditEventCommand = ReactiveCommand.Create(EditEvent);
        DeleteEventCommand = ReactiveCommand.Create(DeleteEvent);

        InitializeProperties();
    }

    [Required]
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public DateTimeOffset? Date
    {
        get => _date;
        set
        {
            _date = value;
            UpdateDateTime();
            this.RaiseAndSetIfChanged(ref _date, value);
        }
    }

    public TimeSpan? Time
    {
        get => _time;
        set
        {
            _time = value;
            UpdateDateTime();
            this.RaiseAndSetIfChanged(ref _time, value);
        }
    }

    public TimeSpan? Duration
    {
        get => _duration;
        set => this.RaiseAndSetIfChanged(ref _duration, value);
    }

    public string? Location
    {
        get => _location;
        set => this.RaiseAndSetIfChanged(ref _location, value);
    }

    public string? Category
    {
        get => _category;
        set => this.RaiseAndSetIfChanged(ref _category, value);
    }

    public string? Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    public bool? Done
    {
        get => _done;
        set => this.RaiseAndSetIfChanged(ref _done, value);
    }
    
    public List<string> Suggestions
    {
        get => _suggestions;
        set => this.RaiseAndSetIfChanged(ref _suggestions, value);
    }

    private Event SelectedEvent { get; }

    #region Commands
    public ReactiveCommand<Unit, Unit> EditEventCommand { get; set; }
    public ReactiveCommand<Unit, Unit> DeleteEventCommand { get; set; }

    #endregion
    
    // todo: adjust for changed repository method
    private void EditEvent()
    {
        var updateViewCollection = UpdateEventInViewCollection();

        if (updateViewCollection.IsSuccess)
        {
            _dataProvider.UpdateEvent(SelectedEvent);
        }
    }

    //todo: adjust for changed repository method
    private void DeleteEvent()
    {
        var writeToFile = _dataProvider.DeleteEvent(SelectedEvent.Id);

        if (writeToFile.IsSuccess)
        {
            _eventsCollection.Remove(SelectedEvent);
        }
    }
    
    private Result UpdateEventInViewCollection()
    {
        if (string.IsNullOrWhiteSpace(Name)) return Result.Fail("Name is required");

        DateTime? dateTime = null;
        if (Date.HasValue && Time.HasValue)
        {
            dateTime = Date.Value.Date + Time.Value;
        }
        else if (Date.HasValue)
        {
            dateTime = Date.Value.Date;
        }
        else if (Time.HasValue)
        {
            return Result.Fail("Date is required, when time is set.");
        }
        else if (!Date.HasValue && !Time.HasValue)
        {
            dateTime = null;
        }

        TimeSpan? duration = null;
        if (Duration.HasValue)
        {
            duration = Duration.Value;
        }

        SelectedEvent.Name = Name;
        SelectedEvent.DateTime = dateTime;
        SelectedEvent.Duration = duration;
        SelectedEvent.Location = Location;
        SelectedEvent.Category = Category;
        SelectedEvent.Description = Description;
        SelectedEvent.Done = Done;
        
        return Result.Success();
    }
    
    private void InitializeProperties()
    {
        if (SelectedEvent == null) return;
        
        _name = SelectedEvent.Name;
        _date = SelectedEvent.DateTime?.ToUniversalTime();
        _time = SelectedEvent.DateTime?.TimeOfDay;
        _duration = SelectedEvent.Duration;
        _location = SelectedEvent.Location;
        _category = SelectedEvent.Category;
        _description = SelectedEvent.Description;
        _done = SelectedEvent.Done;
    }
    
    private void UpdateDateTime()
    {
        if (_date.HasValue && _time.HasValue)
        {
            SelectedEvent.DateTime = _date.Value.Date + _time.Value;
        }
        else
        {
            SelectedEvent.DateTime = null;
        }
    }
}
