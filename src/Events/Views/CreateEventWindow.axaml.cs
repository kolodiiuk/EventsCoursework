using Avalonia.Controls;
using Events.Models;
using Events.ViewModels;

namespace Events.Views;

public partial class CreateEventWindow : Window
{
    public CreateEventWindow(MainWindowViewModel mainWindowViewModel, IEventRepository repository)
    {
        InitializeComponent();
        DataContext = new CreateEventWindowViewModel(mainWindowViewModel, repository);
    }
}