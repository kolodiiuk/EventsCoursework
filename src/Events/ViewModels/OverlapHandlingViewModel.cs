using System;
using System.Reactive;
using Events.Models;
using ReactiveUI;

namespace Events.Views;

public class OverlapHandlingViewModel
{
    public OverlapHandlingViewModel(Action onConfirm)
    {
        AddEventCommand = ReactiveCommand.Create(onConfirm);
        CancelCommand = ReactiveCommand.Create(CloseDialog);
    }

    public ReactiveCommand<Unit, Unit> AddEventCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    
    public event Action CloseRequested;
    
    private void CloseDialog()
    {
        CloseRequested?.Invoke();
    }
}