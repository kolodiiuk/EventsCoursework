using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using Events.Models;
using Events.Utilities;
using Events.Views;

namespace Events.ViewModels;

public class EditEventWindowViewModel : ViewModelBase
{
    private readonly IEventDataProvider _dataProvider;
    private Event _updatedEvent;
    
    private string _name;
    private DateTimeOffset? _date;
    private TimeSpan? _time;
    private TimeSpan? _duration;
    private string _location;
    private string _category;
    private string _description;  
    private bool? _done;
    private Guid _id;

    private DateTime? _dateTime;

    public EditEventWindowViewModel(Guid id, IEventDataProvider dataProvider)
    {
        _id = id;
        _dataProvider = dataProvider;
        SelectedEvent = _dataProvider.GetEventById(_id).Value;
        
        UpdateEventCommand = ReactiveCommand.Create(UpdateEventWithOverlapCheck);
        DeleteEventCommand = ReactiveCommand.Create(DeleteEvent);
        OpenOverlapHandlingWindowCommand = ReactiveCommand.Create(OpenOverlapHandlingWindow);

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

    public List<string> Suggestions { get; set; } = new List<string>
    {
        "Work",
        "School",
        "Family",
        "Friends",
        "Other"
    };

    public ReactiveCommand<Unit, Unit> UpdateEventCommand { get; set; }
    
    public ReactiveCommand<Unit, Unit> DeleteEventCommand { get; set; }
    
    private ReactiveCommand<Unit, Unit> OpenOverlapHandlingWindowCommand { get; }

    private Event SelectedEvent { get; }
    
    public event Action EventUpdated;
    
    private void UpdateEvent()
    {
        var selectedEvent = _dataProvider.GetEventById(_id).Value;
        
        selectedEvent.Name = Name;
        selectedEvent.DateTime = _dateTime;
        selectedEvent.Duration = Duration.Value;
        selectedEvent.Location = Location;
        selectedEvent.Category = Category;
        selectedEvent.Description = Description;
        selectedEvent.Done = Done;
        
        var result = _dataProvider.UpdateEvent(selectedEvent);
        if (result.IsSuccess)
        {
            EventUpdated?.Invoke();
        }
    }

    private void UpdateEventWithOverlapCheck()
    {
        if (string.IsNullOrWhiteSpace(Name)) return;

        DateTime? dateTime = null;
        _dateTime = dateTime;
        if (Date.HasValue && Time.HasValue)
        {
            _dateTime = Date.Value.Date + Time.Value;
        }
        else if (Date.HasValue)
        {
            _dateTime = Date.Value.Date;
        }
        else if (Time.HasValue)
        {
            return;
        }
        else if (!Date.HasValue && !Time.HasValue)
        {
            _dateTime = null;
        }

        TimeSpan? duration = null;
        if (Duration.HasValue)
        {
            duration = Duration.Value;
        }
        
        if (!AreEventsOverlapping(_dateTime, duration))
        {
            UpdateEvent();
        }
        else
        {
            OpenOverlapHandlingWindowCommand.Execute().Subscribe();
        }
    }
    
    private bool AreEventsOverlapping(DateTime? dateTime, TimeSpan? duration)
    {
        var newEventStart = dateTime;
        var newEventEnd = dateTime + duration;
        var intersectingEvent = _dataProvider.GetAllEvents().Value.
            FirstOrDefault(e =>
        {
            var eventStart = e.DateTime;
            var eventEnd = e.DateTime + e.Duration;
            return e.Id != _id && (
                   newEventStart >= eventStart && newEventStart <= eventEnd ||
                   newEventEnd >= eventStart && newEventEnd <= eventEnd ||
                   eventStart >= newEventStart && eventStart <= newEventEnd ||
                   eventEnd >= newEventStart && eventEnd <= newEventEnd);
        });

        return intersectingEvent != null;
    }
    
    private void DeleteEvent()
    {
        var result = _dataProvider.DeleteEvent(_id);
        if (result.IsSuccess)
        {
            EventUpdated?.Invoke();
        }
    }
    
    private void OpenOverlapHandlingWindow()
    {
        var overlapHandlingViewModel = new OverlapHandlingViewModel();
        overlapHandlingViewModel.ConfirmEventCreation += UpdateEvent;
        var overlapHandlingWindow = new OverlapHandlingWindow(overlapHandlingViewModel)
            { DataContext = overlapHandlingViewModel };

        overlapHandlingWindow.Show();
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
