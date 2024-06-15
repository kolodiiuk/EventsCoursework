using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using Events.Models;

namespace Events.ViewModels;

public class EditEventWindowViewModel : ViewModelBase
{
    private readonly IEventRepository _repository;
    private readonly ObservableCollection<Event> _eventsCollection;
    private string _name;
    private DateTimeOffset? _date;
    private TimeSpan? _time;
    private TimeSpan? _duration;
    private string _location;
    private string _category;
    private string _description;
    
    public EditEventWindowViewModel(ObservableCollection<Event> eventsCollection, 
        Event selectedEvent, IEventRepository repository)
    {
        SelectedEvent = selectedEvent;
        _repository = repository;
        _eventsCollection = eventsCollection;
        _repository = repository;
        
        EditEventCommand = ReactiveCommand.Create(EditEventAsync);
        DeleteEventCommand = ReactiveCommand.Create(DeleteEventAsync);

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

    public Event SelectedEvent { get; set; }
    public ReactiveCommand<Unit, Task> EditEventCommand { get; set; }
    public ReactiveCommand<Unit, Task> DeleteEventCommand { get; set; }

    private async Task EditEventAsync()
    {
        if (string.IsNullOrWhiteSpace(Name)) return;

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

        SelectedEvent.Name = Name;
        SelectedEvent.DateTime = dateTime;
        SelectedEvent.Duration = duration;
        SelectedEvent.Location = Location;
        SelectedEvent.Category = Category;
        SelectedEvent.Description = Description;

        await _repository.UpdateEventAsync(SelectedEvent, 
            e => e.Id == SelectedEvent.Id);
        var eventToUpdate = _eventsCollection.First(
            e => e.Id == SelectedEvent.Id);

        eventToUpdate.Name = Name;
        eventToUpdate.DateTime = dateTime;
        eventToUpdate.Duration = duration;
        eventToUpdate.Location = Location;
        eventToUpdate.Category = Category;
        eventToUpdate.Description = Description;
    }
    
    private async Task DeleteEventAsync()
    {
        await _repository.DeleteEventAsync(e => e.Id == SelectedEvent.Id);
        _eventsCollection.Remove(SelectedEvent);
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