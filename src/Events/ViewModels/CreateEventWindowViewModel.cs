using Events.Models;
using Events.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Events.Utilities;

namespace Events.Views;

public class CreateEventWindowViewModel : ViewModelBase
{
    private readonly IEventRepository _eventRepository;

    private string _name;
    private DateTimeOffset? _date;
    private TimeSpan? _time;
    private TimeSpan? _duration;
    private string _location;
    private string _category;
    private string _description;
    private bool? _done;
    private readonly ObservableCollection<Event> _eventsToUpdate;
    private List<string> _suggestions;

    public CreateEventWindowViewModel(
        ObservableCollection<Event> eventsToUpdate, IEventRepository repository)
    {
        _eventRepository = repository;
        _eventsToUpdate = eventsToUpdate;
        
        // todo: categories
        Suggestions = new List<string>
        {
            "Work",
            "School",
            "Family",
            "Friends",
            "Other"
        };
        
        CreateEventCommand = ReactiveCommand.CreateFromTask(CreateEvent);
        OpenOverlapHandlingWindowCommand = ReactiveCommand.Create(OpenOverlapHandlingWindow);
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
        set => this.RaiseAndSetIfChanged(ref _date, value);
    }

    public TimeSpan? Time
    {
        get => _time;
        set => this.RaiseAndSetIfChanged(ref _time, value);
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

    public ReactiveCommand<Unit, Unit> CreateEventCommand { get; }
    private ReactiveCommand<Unit, Unit> OpenOverlapHandlingWindowCommand { get; }

    private async Task CreateEvent()
    {
        var eventFromProperties = CreateEventFromProperties();

        if (eventFromProperties.IsSuccess)
        {
            await _eventRepository.AddEventAsync(eventFromProperties.Value);
            _eventsToUpdate.Add(eventFromProperties.Value);
        }
    }
    
    private Result<Event> CreateEventFromProperties()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return Result<Event>.Fail<Event>("Name is required.");
        }

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
            return Result<Event>.Fail<Event>("Time is required.");
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

        Event newEvent = null;
        if (!AreEventsOverlapping(dateTime, duration))
        {
            newEvent = new Event(Name, dateTime, duration, Location, Category, Description, false);
            return Result<Event>.Success(newEvent);
        }
        else
        {
            OpenOverlapHandlingWindowCommand.Execute().Subscribe();
            return Result<Event>.Fail<Event>("Event overlaps with another event.");
        }
    }

    private bool AreEventsOverlapping(DateTime? dateTime, TimeSpan? duration)
    {
        var newEventStart = dateTime;
        var newEventEnd = dateTime + duration;
        var intersectingEvent = _eventsToUpdate.FirstOrDefault(e =>
        {
            var eventStart = e.DateTime;
            var eventEnd = e.DateTime + e.Duration;
            return newEventStart >= eventStart && newEventStart <= eventEnd ||
                   newEventEnd >= eventStart && newEventEnd <= eventEnd ||
                   eventStart >= newEventStart && eventStart <= newEventEnd ||
                   eventEnd >= newEventStart && eventEnd <= newEventEnd;
        });

        return intersectingEvent != null;
    }

    private void OpenOverlapHandlingWindow()
    {
        var overlapHandlingViewModel = new OverlapHandlingViewModel(() => CreateEventFromProperties());
        var overlapHandlingWindow = new OverlapHandlingWindow(overlapHandlingViewModel)
            { DataContext = overlapHandlingViewModel };

        overlapHandlingWindow.Show();
    }
}