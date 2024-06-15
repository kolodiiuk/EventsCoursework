using System.Collections.ObjectModel;
using Events.Models;

namespace Events.ViewModels;

public class SearchWindowViewModel : ViewModelBase
{
    private ObservableCollection<Event> _events;

    public SearchWindowViewModel(MainWindowViewModel mainWindowViewModel)
    {
        
    }
    public ObservableCollection<Event> Events
    {
        get { return _events; }
        set
        {
            _events = value;
            OnPropertyChanged();
        }
    }
}