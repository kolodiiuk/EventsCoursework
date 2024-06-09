using Events.Models;
using Events.ViewModels;
using ReactiveUI;
using System;
using System.Reactive;

namespace Events.Views;

public class CreateEventWindowViewModel : ViewModelBase
{
    private readonly IEventRepository _eventRepository;
    private MainWindowViewModel _mainWindowViewModel;
    private string _eventName;
    private DateTime _eventDate;
    private TimeSpan _eventTime;
    public ReactiveCommand<Unit, Unit> CreateEventCommand { get; }

    public CreateEventWindowViewModel(MainWindowViewModel mainWindowViewModel, IEventRepository repository)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _eventRepository = repository;
        CreateEventCommand = ReactiveCommand.Create(CreateEvent);
    }

    private void CreateEvent()
    {
        var newEvent = new Event("gregeg", DateTime.Now, default, "grege", "hobby", "erger");
        _mainWindowViewModel.Events.Add(newEvent);
    }
}
