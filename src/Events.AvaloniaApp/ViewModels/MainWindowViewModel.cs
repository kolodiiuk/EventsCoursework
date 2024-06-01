using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Events.AvaloniaApp.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
#pragma warning disable CA1822 // Mark members as static
    public string Greeting => "Welcome to Avalonia!";
#pragma warning restore CA1822 // Mark members as static
    // private readonly IBusinessService _businessService;
    // private readonly IRepository _eventRepository;

    private ObservableCollection<Event> _events;
    private Event _selectedEvent;

    public ObservableCollection<Event> Events
    {
        get => _events;
        set
        {
            _events = value;
            OnPropertyChanged(nameof(Events));
        }
    }

    public Event SelectedEvent
    {
        get => _selectedEvent;
        set
        {
            _selectedEvent = value;
            OnPropertyChanged(nameof(SelectedEvent));
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public ObservableCollection<Event> FilteredEntities { get; } 
        = new ObservableCollection<Event>();
    
    public ICommand CreateEvent { get; set; }
    public ICommand DeleteEvent { get; set; }
    public ICommand UpdateEvent { get; set; }
    public ICommand SaveFilteredEvents { get; set; }
    public ICommand FilterEvents { get; set; }
    public ICommand SaveEventsToFile { get; set; }
    public ICommand LoadEventsFromFile { get; set; }
    public ICommand SaveStatisticsToFile { get; set; }
    public ICommand LoadStatisticsFromFile { get; set; }
    public ICommand SearchEvents { get; set; }
    
    private readonly IRepository _eventRepository;

    public MainWindowViewModel(IRepository eventRepository)
    {
        _eventRepository = eventRepository;

        Events = new ObservableCollection<Event>()
        {
            new Event(
                "Event 1",
                DateTime.Now,
                TimeSpan.FromHours(1),
                "Location 1",
                Category.Business,
                "Description 1"),
            new Event(
                "Event 2",
                DateTime.Now,
                TimeSpan.FromHours(2),
                "Location 2",
                Category.Business,
                "Description 2"),
            new Event(
                "Event 3",
                DateTime.Now,
                TimeSpan.FromHours(3),
                "Location 3",
                Category.Business,
                "Description 3"),
            new Event(
                "Event 4",
                DateTime.Now,
                TimeSpan.FromHours(4),
                "Location 4",
                Category.Business,
                "Description 4"),
            new Event(
                "Event 5",
                DateTime.Now,
                TimeSpan.FromHours(5),
                "Location 5",
                Category.Business,
                "Description 5"),
            new Event(
                "Event 6",
                DateTime.Now,
                TimeSpan.FromHours(6),
                "Location 6",
                Category.Business,
                "Description 6"),
            new Event(
                "Event 7",
                DateTime.Now,
                TimeSpan.FromHours(7),
                "Location 7",
                Category.Business,
                "Description 7"),
            new Event(
                "Event 8",
                DateTime.Now,
                TimeSpan.FromHours(8),
                "Location 8",
                Category.Business,
                "Description 8"),
            new Event(
                "Event 9",
                DateTime.Now,
                TimeSpan.FromHours(9),
                "Location 9",
                Category.Business,
                "Description 9"),
            new Event(
                "Event 10",
                DateTime.Now,
                TimeSpan.FromHours(10),
                "Location 10",
                Category.Business,
                "Description 10"),
            new Event(
                "Event 11",
                DateTime.Now,
                TimeSpan.FromHours(11),
                "Location 11",
                Category.Business,
                "Description 11"),
            new Event(
                "Event 12",
                DateTime.Now,
                TimeSpan.FromHours(12),
                "Location 12",
                Category.Business,
                "Description 12"),
            new Event(
                "Event 13",
                DateTime.Now,
                TimeSpan.FromHours(13),
                "Location 13",
                Category.Business,
                "Description 13"),
        };
        // Initialize commands
        // CreateEvent = new RelayCommand(CreateEventMethod);
        // DeleteEvent = new RelayCommand(DeleteEventMethod);
        // UpdateEvent = new RelayCommand(UpdateEventMethod);
    }

 
    // private async void CreateEventMethod(object parameter)
    // {
    //     if (parameter is Event newEvent)
    //     {
    //         var result = await _eventRepository.AddAsync(newEvent);
    //         // if (result.IsFailure)
    //         // {
    //         //     // Handle failure
    //         // }
    //     }
    // }
    //
    // private async void DeleteEventMethod(object parameter)
    // {
    //     if (parameter is Event eventToDelete)
    //     {
    //         var result = await _eventRepository.DeleteAsync(e => e == eventToDelete);
    //         // if (result.IsFailure)
    //         // {
    //             // Handle failure
    //         // }
    //     }
    // }
    //
    // private async void UpdateEventMethod(object parameter)
    // {
    //     if (parameter is Event updatedEvent)
    //     {
    //         var result = await _eventRepository.UpdateAsync(updatedEvent, e => e == updatedEvent);
    //         // if (result.IsFailure)
    //         // {
    //         //     // Handle failure
    //         // }
    //     }
    // }
} 
