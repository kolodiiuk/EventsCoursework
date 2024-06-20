using System;
using System.Reactive;
using Events.Models;
using ReactiveUI;

namespace Events.Views;

public class OverlapHandlingViewModel
{
    public OverlapHandlingViewModel()
    {
        AddEventCommand = ReactiveCommand.Create(Confirm);
        CancelCommand = ReactiveCommand.Create(CloseDialog);
    }

    public ReactiveCommand<Unit, Unit> AddEventCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public event Action CloseRequested;

    public event Action ConfirmEventCreation;

    private void Confirm()
    {
        ConfirmEventCreation?.Invoke();
        CloseDialog();
    }

    private void CloseDialog()
    {
        CloseRequested?.Invoke();
    }
}