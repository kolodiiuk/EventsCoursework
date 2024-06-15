using Events.Models;
using Events.ViewModels;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

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
    private string _errorMessage;
    private ObservableCollection<Event> _eventsToUpdate;

    public CreateEventWindowViewModel(
        ObservableCollection<Event> eventsToUpdate, IEventRepository repository)
    {
        _eventRepository = repository;
        _eventsToUpdate = eventsToUpdate;

        CreateEventCommand = ReactiveCommand.CreateFromTask(CreateEvent);
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

    public ReactiveCommand<Unit, Unit> CreateEventCommand { get; }
    
    private async Task CreateEvent()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return;
        }

        DateTime? dateTime = null;
        if (Date.HasValue && Time.HasValue)
        {
            dateTime = Date.Value.Date + Time.Value;
        }

        TimeSpan? duration = null;
        if (Duration.HasValue)
        {
            duration = Duration.Value;
        }

        var newEventStart = dateTime;
        var newEventEnd = dateTime + duration;
        var events = await _eventRepository.GetEventListByConditionAsync(e => true);
        var intersectingEvent = events.Value.FirstOrDefault(e =>
        {
            var eventStart = e.DateTime;
            var eventEnd = e.DateTime + e.Duration;
            return newEventStart >= eventStart && newEventStart <= eventEnd ||
                   newEventEnd >= eventStart && newEventEnd <= eventEnd ||
                   eventStart >= newEventStart && eventStart <= newEventEnd ||
                   eventEnd >= newEventStart && eventEnd <= newEventEnd;
        });
        
        if (intersectingEvent != null)
        {
            //todo: implement handling of intersecting events
        }

        Event newEvent = new Event(
            Name, dateTime, duration, Location, Category, Description);
        
        await _eventRepository.AddEventAsync(newEvent);
        // _mainWindowViewModel.AllEventsFromCurrFile.Add(newEvent);
        _eventsToUpdate.Add(newEvent);
    }
}
