using Events.Models;
using Events.ViewModels;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;

namespace Events.Views;

public class CreateEventWindowViewModel : ViewModelBase
{
    private readonly IEventRepository _eventRepository;
    private readonly MainWindowViewModel _mainWindowViewModel;

    private string _name;
    private DateTimeOffset? _date;
    private TimeSpan? _time;
    private TimeSpan? _duration;
    private string _location;
    private string _category;
    private string _description;
    private string _errorMessage;

    public CreateEventWindowViewModel(
        MainWindowViewModel mainWindowViewModel, IEventRepository repository)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _eventRepository = repository;
        
        CreateEventCommand = ReactiveCommand.Create(CreateEvent);
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

    public ReactiveCommand<Unit, Task> CreateEventCommand { get; }
    
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

        Event newEvent = new Event(
            Name, dateTime, duration, Location, Category, Description);
        
        await _eventRepository.AddEventAsync(newEvent);
        _mainWindowViewModel.Events.Add(newEvent);
    }
}
