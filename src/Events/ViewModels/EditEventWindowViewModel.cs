using ReactiveUI;
using System.Reactive;

namespace Events.ViewModels;

public class EditEventWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> EditEventCommand { get; set; }
}